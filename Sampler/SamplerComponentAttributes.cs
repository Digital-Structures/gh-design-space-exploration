using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using DSECommon;

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
            if (MyComponent.Seed != 0) { MyComponent.MyRand = new Random(MyComponent.Seed); } // reset Random to give same result each time.
            MyComponent.Util.Sample();

            if (MyComponent.Dir != "None")
           
            {
                PrintAllSolutions();
                MyComponent.FilesWritten = "Yes";
            }
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
            return base.RespondToMouseDoubleClick(sender, e);
        }

        public void PrintAllSolutions()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"" + MyComponent.Dir + MyComponent.Filename + ".csv");
            for (int i = 0; i < MyComponent.Output.Count; i++)
            {
                string design = "";
                List<double> currentDesign = MyComponent.Output[i];
                for (int j = 0; j < currentDesign.Count-1; j++)
                {
                    design = design + currentDesign[j] + ",";
                }

                design = design + currentDesign[currentDesign.Count - 1];

                file.WriteLine(design);
            }
            file.Close();
        }

    }
}
