using System;
using MathNet.Numerics.Distributions;

namespace StructureEngine.Grammar
{
    public class IntParameter : IRuleParameter
    {
        public IntParameter(int mi, int ma, string name)
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
              
        public int Min
        {
            get;
            private set;
        }

        public int Max
        {
            get;
            private set;
        }

        public object GetRandomValue()
        {
            return Utilities.MyRandom.Next(Min, Max + 1);
        }

        public object Mutate(ISetDistribution dist, double rate, object value)
        {
            double sigma = rate * (double)Math.Abs(Max - Min) / 2.0;
            dist.Mean = Convert.ToDouble((int)value);
            dist.StdDev = sigma;

            double newvalue = dist.Sample();
            newvalue = Math.Max(newvalue, Min);
            newvalue = Math.Min(newvalue, Max);

            return Convert.ToInt32(newvalue);
        }
    }
}
