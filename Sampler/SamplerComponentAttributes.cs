using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;

namespace Sampler 
{


    class SamplerComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        // Component
        SamplerComponent MyComponent;

        // Variables Declaration

        // Constructor
        public SamplerComponentAttributes(IGH_Component component) : base(component)
        {
            MyComponent = (SamplerComponent)component;
        }

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            MyComponent.Util.Sample();
            MyComponent.Util.WriteOutputToFile(MyComponent.Output);
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
            return base.RespondToMouseDoubleClick(sender, e);
        }

        static DataTree<T> ListOfListsToTree<T>(List<List<T>> listofLists)
        {
            DataTree<T> tree = new DataTree<T>();
            for (int i = 0; i < listofLists.Count; i++)
            {
                tree.AddRange(listofLists[i], new GH_Path(i));
            }
            return tree;
        }
    }
}
