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

namespace Cluster
{
    class ClusterComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        CaptureComponent MyComponent;

        public ClusterComponentAttributes(IGH_Component component)
            : base(component)
        {
            MyComponent = (CaptureComponent)component;
        }

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            // Reset list of objective values
            MyComponent.ObjValues = new List<List<double>>();
            MyComponent.PropertyValues = new List<List<double>>();
            MyComponent.FirstRead = true;
            MyComponent.Iterating = true;
         
            MyComponent.Iterating = false;
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
            return base.RespondToMouseDoubleClick(sender, e);
        }
      

    }
}
