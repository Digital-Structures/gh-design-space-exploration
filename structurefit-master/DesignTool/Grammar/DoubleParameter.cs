using System;
using MathNet.Numerics.Distributions;

namespace StructureEngine.Grammar
{
    public class DoubleParameter : IRuleParameter
    {
        public DoubleParameter(double mi, double ma, string name)
        {
            this.Min = mi;
            this.Max = ma;
            this.Name = name;
        }

        public string Name
        {
            get;
            private set;
        }

        public double Min
        {
            get;
            private set;
        }

        public double Max
        {
            get;
            private set;
        }

        public object GetRandomValue()
        {
            double d = Utilities.MyRandom.NextDouble() * (Max - Min) + Min;
            return d;
        }

        public object Mutate(ISetDistribution dist, double rate, object value)
        {
            double sigma = rate * (double)Math.Abs(Max - Min) / 2.0;
            dist.Mean = (double)value;
            dist.StdDev = sigma;

            double newvalue = dist.Sample();
            newvalue = Math.Max(newvalue, Min);
            newvalue = Math.Min(newvalue, Max);

            return newvalue;
        }
    }
}
