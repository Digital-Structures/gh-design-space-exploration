using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSOptimization
{
    public class Constraint
    {
        public enum ConstraintType { lessthan, morethan, equalto };
        public DSOptimizerComponent MyComponent { get; set; }
        public double CurrentValue { get { return MyComponent.Constraints[ConstraintIndex]; } }
        public double LimitValue { get; set; }
        public ConstraintType MyType { get; set; }
        public int ConstraintIndex { get; set; }
        public double Gradient { get; set; }

        public Constraint(DSOptimizerComponent mycomponent, ConstraintType constraintType, int constraintindex, 
            double limit = 0, bool active = true)
        {
            MyComponent = mycomponent;
            this.ConstraintIndex = constraintindex;
            this.MyType = constraintType;
            this.LimitValue = limit;
            this.IsActive = active;
        }

        //IS ACTIVE
        //Determines whether variable should be considered in optimization
        public bool IsActive
        {
            get; set;
        }
    }
}
