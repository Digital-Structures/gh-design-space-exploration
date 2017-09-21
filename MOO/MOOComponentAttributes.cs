using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MOO
{
    class MOOComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {

        MOO MyComponent;

        public MOOComponentAttributes(IGH_Component component)
            : base(component)
        {
            MyComponent = (MOO)component;


        }

        
        // Variables
        List<GH_NumberSlider> variablesSliders = new List<GH_NumberSlider>();
        public int solutionsCounter = 0; // Count designs evaluated
        public List<List<double>> allSolutionsTrack = new List<List<double>>();

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            if (MyComponent.Seed != 0) { MyComponent.MyRand = new Random(MyComponent.Seed); } // reset Random to give same result each time.

            // Reset list of objective values
            MyComponent.ObjValues = new List<List<double>>();
            MyComponent.ObjValues = new List<List<double>>();
            

           

            variablesSliders = MyComponent.readSlidersList();
            NSGAIIProblem problem = new NSGAIIProblem("ArrayReal", MyComponent, solutionsCounter);
            NSGAIIRunner runner = new NSGAIIRunner(null, problem, null, MyComponent);
            allSolutionsTrack = problem.allSolutions;
            problem.PrintAllSolutions();
            problem.PrintLogFile();
            solutionsCounter++;


            MyComponent.MooDone = true;
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
            MessageBox.Show("Finished");

            //System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\New folder\123d.txt");
            //file.WriteLine("" + problem.allSolutions.Count);
            //file.Close();

            return base.RespondToMouseDoubleClick(sender, e);
        }

       

          

        }
    }


