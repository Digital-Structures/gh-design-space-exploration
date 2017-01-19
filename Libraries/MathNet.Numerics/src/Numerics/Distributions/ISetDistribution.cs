
namespace MathNet.Numerics.Distributions
{
    public interface ISetDistribution : IContinuousDistribution
    {
        new double Mean {get; set;}
        new double StdDev { get; set; }
    }
}
