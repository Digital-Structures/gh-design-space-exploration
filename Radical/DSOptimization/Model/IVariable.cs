using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;

namespace DSOptimization
{
    public interface IVariable
    {
        /// <summary>
        /// Numerical value of the variable before optimization
        /// May be used to reset design or as a reference for variable changes
        /// </summary>
        double ReferenceValue { get; set; } 
        double CurrentValue { get; set; }
        double Max { get; set; }
        double Min { get; set; }
        int Dir { get; set; }
        void UpdateValue(double x);
        double Gradient();
        bool IsActive { get; set; }
        IGH_Param Parameter { get; }
    }

    //public interface IGeoVariable:IVariable
    //{
    //    IDesignGeometry Geometry { get; set; } //geometry to which the variable belongs
    //    int Dir { get; set; }
    //}
}
