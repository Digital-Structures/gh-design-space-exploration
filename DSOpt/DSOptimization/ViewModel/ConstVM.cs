using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radical;

namespace DSOptimization
{ 
    public class ConstVM:BaseVM
    {
        public ConstVM()
        {
        }

        public ConstVM(Constraint constraint, GraphVM graph_vm)
        {
            this.Constraint = constraint;
            this._name = "C." + (Constraint.ConstraintIndex + 1).ToString();
            this._constraintlimit = constraint.LimitValue;
            this._constrainttype = (int)constraint.MyType;
            this._currentvalue = Constraint.CurrentValue;
            this._isactive = constraint.IsActive;
            this.GraphVM = graph_vm;
            this.GraphVM.LineGraphName = this.Name;
            this.OptRunning = false;
        }
        public GraphVM GraphVM;
        public Constraint Constraint;

        private double _currentvalue;
        public double CurrentValue
        {
            get
            { return _currentvalue; }
            set
            {
                if (CheckPropertyChanged<double>("CurrentValue", ref _currentvalue, ref value))
                {
                }
            }
        }

        //OPTIMIZATION FINISHED
        //Update UI sliders to reflect optimized values
        public void OptimizationFinished()
        {
            this.ChangesEnabled = true;
            this.CurrentValue = Constraint.CurrentValue;
        }

        private double _constraintlimit;
        public double ConstraintLimit
        {
            get
            { return _constraintlimit; }
            set
            {
                if (CheckPropertyChanged<double>("ConstraintLimit", ref _constraintlimit, ref value))
                {
                    Constraint.LimitValue = ConstraintLimit;
                }
            }

        }

        private string _name;
        public string Name
        {
            get
            { return _name; }
            set
            {
                if (CheckPropertyChanged<string>("Name", ref _name, ref value))
                {
                    this.GraphVM.LineGraphName = value;
                }
            }
        }

        private bool _isactive;
        public bool IsActive
        {
            get
            {
                return _isactive;
            }
            set
            {
                if (CheckPropertyChanged<bool>("IsEnabled", ref _isactive, ref value))
                {
                    Constraint.IsActive = IsActive;           
                }
            }
        }
        
        private int _constrainttype;
        public int ConstraintType
        {
            get
            { return (int)_constrainttype; }
            set
            {
                if (CheckPropertyChanged("ConstraintType", ref _constrainttype, ref value))
                {
                    Constraint.MyType = (Constraint.ConstraintType)ConstraintType;
                }
            }
        }
    }
}
