using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.GUI;

namespace DSOptimization
{ 
    //VARIABLE VIEW MODEL
    //Manages the bounds and values of input variables to be optimized
    public class VarVM : BaseVM, IStepDataElement
    {
        public enum Direction { X, Y, Z, None };
        public IVariable DesignVar;

        public double OriginalValue { get; set; } //Original value of the variable before optimization
        public double BestSolutionValue { get; set; } //Minimum value obtained through the optimization process

        public Design Design;

        //CONSTRUCTOR
        //default values obtained from original Grasshopper component variables
        public VarVM(IVariable dvar, Design d)
        {
            DesignVar = dvar;
            this.Design = d;

            //value trackers
            this.OriginalValue = DesignVar.CurrentValue;
            this.BestSolutionValue = DesignVar.CurrentValue;

            this._name = DesignVar.Parameter.NickName;

            this._value = DesignVar.CurrentValue;
            this._min = DesignVar.Min;
            this._max = DesignVar.Max;
            this.IsActive = true;

            this.OptRunning = false;
        }

        public VarVM(IVariable dvar)
        {
            DesignVar = dvar;

            //value trackers
            this.OriginalValue = DesignVar.CurrentValue;
            this.BestSolutionValue = DesignVar.CurrentValue;

            this._name = DesignVar.Parameter.NickName;

            this._value = DesignVar.CurrentValue;
            this._min = DesignVar.Min;
            this._max = DesignVar.Max;
            this.IsActive = true;

            this.OptRunning = false;
        }

        //DIRECTION
        private Direction _dir;
        public virtual int Dir
        {
            get
            {
                return this.DesignVar.Dir;
            }
            set 
            {
                this._dir = (Direction)value;
            }
        }

        //NAME
        //The name of an individual variable
        private string _name;
        public string Name
        {
            get
            { return _name; }
            set
            {
                if (CheckPropertyChanged<string>("Name", ref _name, ref value))
                {
                    //Prevent naming geometries after individual control points 
                    if (!(DesignVar is GeoVariable))
                    {
                        DesignVar.Parameter.NickName = this._name;
                    }
                }
            }
        }

        //VALUE
        //Current value of the individual variable
        private double _value;
        public virtual double Value
        {
            get
            { return _value; }
            set
            {
                //Update value if change is in bounds
                if (!(value <= this.Max && value >= this.Min))
                    this.OpenDialog = true;
                else if (CheckPropertyChanged<double>("Value", ref _value, ref value))
                {
                    System.Action run = delegate ()
                    {
                        for(int i = 0; i < this.Design.ActiveVariables.Count; i++)
                        {
                            if(Design.ActiveVariables[i] == DesignVar)
                            {
                                Design.ActiveVariables[i].UpdateValue(value);
                                return;
                            }
                        }
                    };
                    Rhino.RhinoApp.MainApplicationWindow.Invoke(run);
                }
            }
        }

        //MIN
        //Minimum value the variable should hold
        private double _min;
        public virtual double Min
        {
            get
            { return _min; }
            set
            {
                //Invalid Bounds, display an error
                if (value > this._max)
                {
                    this.OpenDialog = true;
                }

                else if (CheckPropertyChanged<double>("Min", ref _min, ref value))
                {
                    DesignVar.Min = this._min;

                    //Ensure the value of the slider is not outside the new min bound
                    if (this._min > DesignVar.CurrentValue)
                    {
                        DesignVar.UpdateValue(this._min);
                        this.Value = this._min;
                    }
                }
            }
        }

        //MAX
        //Maximum value the variable should hold
        private double _max;
        public virtual double Max
        {
            get
            { return _max; }
            set
            {
                //Invalid Bounds, display an error
                if (value < this._min)
                {
                    this.OpenDialog = true;
                }
                else if (CheckPropertyChanged<double>("Max", ref _max, ref value))
                {
                    DesignVar.Max = this._max;

                    //Ensure the value of the slider is not outside the new max bound
                    if (_max < DesignVar.CurrentValue)
                    {
                        DesignVar.UpdateValue(this._max);
                        this.Value = this._max;
                    }
                }
            }
        }

        //OPEN DIALOG
        //Boolean to determine whether a bounds error was thrown
        private bool opendialog;
        public virtual bool OpenDialog
        {
            get { return this.opendialog; }
            set { CheckPropertyChanged<bool>("OpenDialog", ref opendialog, ref value); }
        }

        //IS ACTIVE
        //Determines whether variable will be considered in optimization
        private bool _isactive;
        public bool IsActive
        {
            get
            {
                return _isactive;
            }
            set
            {
                if (CheckPropertyChanged<bool>("IsActive", ref _isactive, ref value))
                {
                    DesignVar.IsActive = this._isactive;
                }
            }
        }

        //OPTIMIZATION FINISHED
        //Update UI sliders to reflect optimized values
        public virtual void OptimizationFinished()
        {
            this.ChangesEnabled = true;
            this.Value = DesignVar.CurrentValue;
        }

        //UPDATE BEST SOLUTION VALUE 
        //Should be called when the current value of the variable corresponds to the current
        //best solution of the objective
        public void UpdateBestSolutionValue()
        {
            this.BestSolutionValue = this.DesignVar.CurrentValue;
        }

        public void SetBestSolution()
        {
            //Do we want to switch the original value to be this best solution?
            this.Value = this.BestSolutionValue;
            this.DesignVar.UpdateValue(this.Value);
        }

        public void ResetValue()
        {
            this.Value = this.OriginalValue;
            this.DesignVar.UpdateValue(this.Value);
            this.BestSolutionValue = this.OriginalValue;
        }
    }
}
