using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Grasshopper;

using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using Grasshopper.Kernel.Parameters;

using Grasshopper.Kernel.Types;


namespace Writer
{
    class WriterComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {

        Writer MyComponent;

        public WriterComponentAttributes(IGH_Component component)
            : base(component)
        {
            MyComponent = (Writer)component;


        }


        // Variables
        List<GH_NumberSlider> variablesSliders = new List<GH_NumberSlider>();
  
        public List<List<double>> allSolutionsTrack = new List<List<double>>();
        

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {

            
            MyComponent.WriterDone = true;
            

            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);

            return base.RespondToMouseDoubleClick(sender, e);
        }





    }



}