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

namespace Sift
{
    class SiftComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        SiftComponent MyComponent;

        public SiftComponentAttributes(IGH_Component component)
            : base(component)
        {
            MyComponent = (SiftComponent)component;
        }

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            GHUtilities.ChangeSliders(MyComponent.SlidersList, MyComponent.DesignMap[MyComponent.Index]);
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false);      
            return base.RespondToMouseDoubleClick(sender, e);
        }



    }

      

        
      

    }

