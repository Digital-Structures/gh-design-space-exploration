
using MathNet.Numerics.Distributions;

namespace StructureEngine.Grammar
{
    public interface IRuleParameter
    {
        object GetRandomValue();
        string Name
        {
            get;
        }
        object Mutate(ISetDistribution dist, double rate, object value);
    }
}
