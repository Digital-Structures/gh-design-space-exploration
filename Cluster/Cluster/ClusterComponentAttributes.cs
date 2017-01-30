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
        }

        public List<int> labelsList;

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {


            // Reset list of objective values
            
            MyComponent.Iterating = true;
           
            KMeans kmeans = new KMeans(MyComponent.numClusters);
            double[][] data = MyComponent.DesignMap.Select(a => a.ToArray()).ToArray();
            double[] weights = null;

            // int[] labels = kmeans.Learn(data,weights);

            int[] labels = kmeans.Compute(data);

            labelsList = labels.OfType<int>().ToList();

            
           
            MyComponent.Iterating = false;
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);

            return base.RespondToMouseDoubleClick(sender, e);
        }
      

    }
}
