using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows;
using DSECommon;

namespace Stepper
{
    //STEPPER VM
    //View Model to mediate communication between StepperWindow and StepperComponent
    public class StepperVM : BaseVM
    {
        //Variables and properties
        private StepperComponent MyComponent;
        public List<List<double>> Objectives;
        public List<List<double>> Variables;

        public StepperVM() {}

        //CONSTRUCTOR
        public StepperVM(StepperComponent stepper)
        {
            //StepperComponent
            this.MyComponent = stepper;

            //Set up list of lists to store the value evolution of each objective
            this.Objectives = new List<List<double>>();
            for (int i = 0; i < this.MyComponent.ObjInput.Count; i++)
                this.Objectives.Add(new List<double>());

            //Set up list of lists to store the value evolution of each variable
            this.Variables= new List<List<double>>();
            foreach (DSEVariable var in this.MyComponent.VarsList)
                this.Objectives.Add(new List<double>());
        }

        //ON WINDOW CLOSING
        //Alert the component that the window has been closed
        //(and therefore a new window can open on double click)
        public void OnWindowClosing()
        {
            this.MyComponent.IsWindowOpen = false;
        }
    }
}
