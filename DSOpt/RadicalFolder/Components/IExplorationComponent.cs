using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radical.Components
{
    public interface IExplorationComponent
    {
        List<double> dVars { get; set; }
        List<double> dProp { get; set; }
        int nSamples { get; set; }

    }
}
