using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StormCloud.ViewModel;


namespace StormCloud
{
    public class InterOptComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        // custom attribute to override double click mouse event on component and open a WPF window
 
        public InterOptComponentAttributes(IGH_Component component) : base(component)
        {
            MyComponent = (InterOptComponent)component;
        }

        InterOptComponent MyComponent;

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            MyComponent.DesignView.InitialDesign = new DesignVM(MyComponent.DesignLines, MyComponent.DesignCurves, MyComponent.DesignMeshes, MyComponent.DesignBreps, false, true, MyComponent.Score, MyComponent.Design);
            Window w = new StormCloudWindow(MyComponent.DesignView, MyComponent);
            w.Show();
            return base.RespondToMouseDoubleClick(sender, e);
        }
    }
}
