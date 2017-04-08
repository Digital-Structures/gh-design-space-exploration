using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using DSECommon;
using System.Drawing;
using Accord.MachineLearning;





namespace Cluster
{
    class ClusterComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public ClusterComponent MyComponent;

        public ClusterComponentAttributes(IGH_Component component)
            : base(component)
        {
            MyComponent = (ClusterComponent)component;

            this.DesignMapSorted = new List<List<List<double>>>();
            this.ClusterAves = new List<List<double>>();
            this.DesignMap = new List<List<double>>();
            this.ClusterMaxs = new List<List<double>>();
            this.ClusterMins = new List<List<double>>();

        }


        // Create variables
        public List<int> LabelsList;
        List<List<double>> DesignMap;
        List<List<List<double>>> DesignMapSorted;
        List<List<double>> ClusterAves;
        List<List<double>> ClusterMaxs;
        List<List<double>> ClusterMins;
        List<int> ClusterLabelsList;
        int numVars;


        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {


            // Read in Cluster number slider
           List<IGH_Param> sliderListClust = new List<IGH_Param>();
           foreach (IGH_Param src2 in MyComponent.Params.Input[4].Sources)
            {
                sliderListClust.Add(src2);
            }
            Grasshopper.Kernel.Special.GH_NumberSlider clusterSlider = (Grasshopper.Kernel.Special.GH_NumberSlider) sliderListClust[0];



            if (!MyComponent.ClusterDone)

            {
                //run clustering process
                KMeans kmeans = new KMeans(MyComponent.numClusters);


                double[][] data = MyComponent.DesignMap.Select(a => a.ToArray()).ToArray();
                double[] weights = null;

                // int[] labels = kmeans.Learn(data,weights);

                for (int i = 0; i < data.Count(); i++)
                {
                    data[i] = data[i].Take(data[i].Count() - MyComponent.numObjs).ToArray();
                }      


                int[] labels = kmeans.Compute(data);

                LabelsList = labels.OfType<int>().ToList();


                // Set cluster slider bounds, values to default while clustering is run
                clusterSlider.TrySetSliderValue((decimal) 0);
                clusterSlider.Slider.Minimum = ((decimal) 0);
                clusterSlider.Slider.Maximum = ((decimal) MyComponent.numClusters);


                // list management    
                this.DesignMap = MyComponent.DesignMap;
                this.numVars = MyComponent.numVars;

                // create Sorted list
                for (int i = 0; i < MyComponent.numClusters; i++)
                {

                    DesignMapSorted.Add(new List<List<double>>());
                    for (int j = 0; j < DesignMap.Count; j++)
                    {

                        if (LabelsList[j] == i)
                        {

                            DesignMapSorted[i].Add(DesignMap[j]);

                        }
                    }
                }

                for (int i = 0; i < MyComponent.numClusters; i++)
                {

                    ClusterAves.Add(new List<double>());
                    ClusterMaxs.Add(new List<double>());
                    ClusterMins.Add(new List<double>());

                    double[] sum = new double[numVars];
                    double[] average = new double[numVars];
                    double[] max = new double[numVars];
                    double[] min = new double[numVars];

                    for (int l = 0; l < numVars; l++)

                    {
                        sum[l] = 0;
                        max[l] = double.MinValue;
                        min[l] = double.MaxValue;
                    }

                    for (int j = 0; j < DesignMapSorted[i].Count; j++)

                    {


                        for (int k = 0; k < numVars; k++)

                        {
                            sum[k] = sum[k] + DesignMapSorted[i][j][k];

                            if (DesignMapSorted[i][j][k] > max[k])

                            {
                                max[k] = DesignMapSorted[i][j][k];
                            }
                            else if (DesignMapSorted[i][j][k] < min[k])

                            {

                                min[k] = DesignMapSorted[i][j][k];
                            }

                            average[k] = sum[k] / DesignMapSorted[i].Count;

                        }


                    }

                    for (int k = 0; k < numVars; k++)
                    {
                        ClusterAves[i].Add(average[k]);
                        ClusterMaxs[i].Add(max[k]);
                        ClusterMins[i].Add(min[k]);
                    }
                }


                ClusterAves.Insert(0, MyComponent.VarsVals);
                ClusterMaxs.Insert(0, MyComponent.MaxVals);
                ClusterMins.Insert(0, MyComponent.MinVals);


                //for (int i = 0; i < DesignMapSorted.Count; i++)

                //{
                //LabelsList[i] = LabelsList[i] + 1;
                //}
                

            }



            



            List<IGH_Param> sliderList = new List<IGH_Param>();

            foreach (IGH_Param src in MyComponent.Params.Input[0].Sources)
            {
                sliderList.Add(src);
            }

            for (int i = 0; i < numVars; i++)
            {

                if (MyComponent.index != 0)

                {

                    Grasshopper.Kernel.Special.GH_NumberSlider nslider = (Grasshopper.Kernel.Special.GH_NumberSlider)sliderList[i];

                    double adjmin = ClusterMins[MyComponent.index][i] + (1 - MyComponent.flexibility) * (ClusterAves[MyComponent.index][i] - ClusterMins[MyComponent.index][i]);
                    double adjmax = ClusterMaxs[MyComponent.index][i] - (1 - MyComponent.flexibility) * (ClusterMaxs[MyComponent.index][i] - ClusterAves[MyComponent.index][i]);

                    nslider.TrySetSliderValue((decimal)ClusterAves[MyComponent.index][i]);
                    nslider.Slider.Minimum = ((decimal)adjmin);
                    nslider.Slider.Maximum = ((decimal)adjmax);

                } else
                {
                    Grasshopper.Kernel.Special.GH_NumberSlider nslider = (Grasshopper.Kernel.Special.GH_NumberSlider)sliderList[i];

                    nslider.TrySetSliderValue((decimal)ClusterAves[MyComponent.index][i]);
                    nslider.Slider.Minimum = ((decimal)ClusterMins[MyComponent.index][i]);
                    nslider.Slider.Maximum = ((decimal)ClusterMaxs[MyComponent.index][i]);

                }

            }

            MyComponent.ClusterDone = true;
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);

            return base.RespondToMouseDoubleClick(sender, e);
        }
      



    }

   

    }


