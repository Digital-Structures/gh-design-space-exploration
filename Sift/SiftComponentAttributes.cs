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

            if (MyComponent.SlidersList.Count != MyComponent.DesignMap[MyComponent.Index].Count)
            {
                throw new Exception("Error: Number of sliders and number of target values must be equal.");
            }

            for (int i = 0; i < MyComponent.SlidersList.Count; i++)
            {
                Grasshopper.Kernel.Special.GH_NumberSlider s = MyComponent.SlidersList[i];
                double d = MyComponent.DesignMap[MyComponent.Index][i];
                s.SetSliderValue((decimal)d);
            }

            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false, GH_SolutionMode.Default);

            //Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false);      
            return base.RespondToMouseDoubleClick(sender, e);
        }

    }



}

