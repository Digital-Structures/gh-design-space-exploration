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



            if (!MyComponent.ClusterDone)

            {
                //run clustering process
                KMeans kmeans = new KMeans(MyComponent.numClusters);
                double[][] data = MyComponent.DesignMap.Select(a => a.ToArray()).ToArray();
                double[] weights = null;

                // int[] labels = kmeans.Learn(data,weights);

                int[] labels = kmeans.Compute(data);

                LabelsList = labels.OfType<int>().ToList();



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


            }

            //this.MyComponent.Params.Input[5].RemoveAllSources();
        
            // Find sliders.
            var localSliders = new List<Grasshopper.Kernel.Special.GH_NumberSlider>();

            // Remove old sliders

            for (int i = 0; i < numVars; i++)
            {

                foreach (IGH_DocumentObject obj in Grasshopper.Instances.ActiveCanvas.Document.Objects)
                {
                    var slider = obj as Grasshopper.Kernel.Special.GH_NumberSlider;
                    if (slider == null)
                        continue;
                    if (slider.NickName.Contains("Variable_"))
                    {
                        Grasshopper.Instances.ActiveCanvas.Document.RemoveObject(slider, true);
                        break;
                     
                    }
                }


            }

            //add new sliders

            for (int i = 0; i < numVars; i++)

            {

                //instantiate  new slider
                Grasshopper.Kernel.Special.GH_NumberSlider slid = new Grasshopper.Kernel.Special.GH_NumberSlider();
                slid.CreateAttributes(); //sets up default values, and makes sure your slider doesn't crash rhino

                //customise slider (position, ranges etc)
                int inputcount = this.MyComponent.Params.Input[0].SourceCount;
                slid.Attributes.Pivot = new PointF((float)this.MyComponent.Attributes.DocObject.Attributes.Bounds.Left - slid.Attributes.Bounds.Width - 30, (float)this.MyComponent.Params.Input[1].Attributes.Bounds.Y + inputcount * 30);
                slid.Slider.Maximum = Convert.ToDecimal(ClusterMaxs[MyComponent.index][i]);
                slid.Slider.Minimum = Convert.ToDecimal(ClusterMins[MyComponent.index][i]); 
                slid.Slider.DecimalPlaces = 2;

                int id = i + 1;
                slid.NickName = "Variable_" + id.ToString();  
                slid.SetSliderValue((decimal)((ClusterAves[MyComponent.index][i])));


                //Until now, the slider is a hypothetical object.
                // This command makes it 'real' and adds it to the canvas.
                Grasshopper.Instances.ActiveCanvas.Document.AddObject(slid, false);

                

                //Connect the new slider to this component
                this.MyComponent.Params.Input[5].AddSource(slid);

            }





            MyComponent.ClusterDone = true;
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);

            return base.RespondToMouseDoubleClick(sender, e);
        }
      



    }

   

    }


