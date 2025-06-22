using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Radical.TestComponents;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;

namespace Radical.Integration
{
    public interface IDesign
    {
        // Ok
        List<IVariable> Variables { get; set; }
        List<IVariable> ActiveVariables { get; }
        List<IDesignGeometry> Geometries { get; set; }
        List<Constraint> Constraints { get; }
        //void Sample(int alg);
        
        // to remove if possible
        DSOptimizerComponent MyComponent { get; set; }
    }
}
