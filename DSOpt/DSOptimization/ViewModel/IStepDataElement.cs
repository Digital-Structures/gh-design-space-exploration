using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSOptimization
{
    public interface IStepDataElement: IViewModel
    {
        double Value { get; set; }
        string Name { get; set; }
    }
}
