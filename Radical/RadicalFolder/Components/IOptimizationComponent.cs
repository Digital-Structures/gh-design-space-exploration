using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radical.Components
{
    public interface IOptimizationComponent
    {
        List<double> Objectives { get; set; }
        List<double> Constraints { get; set; }
        List<double> Evolution { get; set; }
    }
}
