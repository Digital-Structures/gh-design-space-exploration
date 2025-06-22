using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radical;
using DSOptimization;

namespace Stepper
{
    //OBJECTIVE VM
    //View Model for storing properties of input objectives
    public class ObjectiveVM : BaseVM, IStepDataElement
    {
        private StepperVM Stepper;
        public int index;

        //CONSTRUCTOR
        public ObjectiveVM(double value, StepperVM stepper)
        {
            this._name = "Objective";
            this._val = value;
            this.Stepper = stepper;
        }

        //NAME
        //The name of the objective function
        private string _name;
        public string Name
        {
            get { return this._name; }
            set
            {
                //Update the name of the objective and notify property changed
                if (CheckPropertyChanged<string>("Name", ref _name, ref value))
                    this.Stepper.ObjectiveNamesChanged();
            }
        }

        //VALUE
        //The current value of the objective function
        //Read only, can't be changed by the user
        private double _val;
        public double Value
        {
            get { return this._val; }
            set { CheckPropertyChanged<double>("Value", ref _val, ref value); }
        }
    }
}
