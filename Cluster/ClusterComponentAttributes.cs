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

            //Set all cluster values

            if(MyComponent.ClusterDone)

            {
                DesignMapSorted = MyComponent.DesignMapSorted;
                ClusterAves = MyComponent.ClusterAves;
                ClusterMaxs = MyComponent.ClusterMaxs;
                ClusterMins = MyComponent.ClusterMins;
                ClusterLabelsList = MyComponent.ClusterLabelsList;
                int numVars = MyComponent.numVars;

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

            }

            MyComponent.ClusterDone = true;
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);

            return base.RespondToMouseDoubleClick(sender, e);
        }
      



    }

   

    }


