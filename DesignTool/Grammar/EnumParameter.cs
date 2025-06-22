using System.Collections.Generic;
using MathNet.Numerics.Distributions;

namespace StructureEngine.Grammar
{
    public class EnumParameter : IRuleParameter
    {
        public EnumParameter(List<string> enums, string name)
        {
            this.Name = name;
            this.Enums = enums;
        }

        public List<string> Enums
        {
            get;
            private set;
        }

        public object GetRandomValue()
        {
            int rand = Utilities.MyRandom.Next(Enums.Count);
            return Enums[rand];
        }

        public string Name
        {
            get;
            private set;
        }

        public object Mutate(ISetDistribution dist, double rate, object value)
        {
            List<bool> odds = new List<bool>();
            bool mutate = dist.RandomSource.NextDouble() < rate;

            List<string> others = new List<string>();
            others.AddRange(this.Enums);
            others.Remove((string)value);

            string replacement;
            if (mutate)
            {
                replacement = others[dist.RandomSource.Next(others.Count)];
            }
            else
            {
                replacement = (string)value;
            }

            return replacement;
        }
    }
}
