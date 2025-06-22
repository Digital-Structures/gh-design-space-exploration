namespace Tilde
{
    using Grasshopper.Kernel;
    using Grasshopper.Kernel.Attributes;
    using System;

    internal class TildeComponentAttributes : GH_ComponentAttributes
    {
        private TildeComponent MyComponent;

        public TildeComponentAttributes(IGH_Component component) : base(component)
        {
            this.MyComponent = (TildeComponent) component;
        }

        //NOTE: All obsolete with change to run on button

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            this.MyComponent.modelType = "Test";
            this.buildModel();
            this.MyComponent.modelCreated = true;
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
            return base.RespondToMouseDoubleClick(sender, e);
        }

        private void buildModel()
        {
            ProblemBuilder pb = new ProblemBuilder(this.MyComponent);
            pb.Start();
        }
    }
}

