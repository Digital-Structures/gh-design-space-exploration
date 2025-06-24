using System.Collections.Generic;

namespace StructureEngine.MachineLearning
{
    public interface IFunction
    {
        double GetOutput(List<double> inputs);
    }
}
