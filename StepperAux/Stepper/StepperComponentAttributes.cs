using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;

namespace Stepper
{
    // Token: 0x0200001A RID: 26
    public class StepperComponentAttributes : GH_ComponentAttributes
    {
        // Token: 0x060000DD RID: 221 RVA: 0x00005514 File Offset: 0x00003714
        public StepperComponentAttributes(IGH_Component component) : base(component)
        {
            this.MyComponent = (StepperComponent)component;
        }

        // Token: 0x060000DE RID: 222 RVA: 0x0000552C File Offset: 0x0000372C
        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            bool flag = !this.MyComponent.IsWindowOpen && this.MyComponent.InputsSatisfied;
            if (flag)
            {
                this.MyComponent.IsWindowOpen = true;
                Design design = new Design(this.MyComponent);
                Thread thread = new Thread(delegate ()
                {
                    StepperVM svm = new StepperVM(design, this.MyComponent);
                    Window window = new StepperWindow(svm);
                    window.Show();
                    Dispatcher.Run();
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            return base.RespondToMouseDoubleClick(sender, e);
        }

        // Token: 0x0400005A RID: 90
        private StepperComponent MyComponent;
    }
}
