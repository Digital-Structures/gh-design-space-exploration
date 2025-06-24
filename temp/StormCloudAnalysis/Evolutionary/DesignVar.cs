using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace StormCloud.Evolutionary
{
    public class DesignVar
    {
        public DesignVar(double val, double min, double max)
        {
            this.Value = val;
            this.Min = min;
            this.Max = max;
        }

        public double Value
        {
            get;
            set;
        }


        public double Min
        {
            get;
            set;
        }

        public double Max
        {
            get;
            set;
        }

        public double Variation
        {
            get
            {
                return (Max - Min) / 2;
            }
            private set {}
        }

        public DesignVar VarClone()
        {
            return new DesignVar(this.Value, this.Min, this.Max);
        }


       
        public void Mutate(double globalrate, IContinuousDistribution dist)
        {
            double sigma = globalrate * Variation;
            dist = new Normal(this.Value, sigma);
            //dist.Mean = this.Value;
            //dist.StdDev = sigma;

            double newval = dist.Sample();
            this.Value = newval;
        }

        public void Crossover(System.Collections.Generic.List<DesignVar> mylist)
        {
            double newval = 0;
            double normalize = 0;
            foreach (DesignVar d in mylist)
            {
                double weight = EvolutionaryUtilities.MyRandom.NextDouble();
                newval += d.Value * weight;
                normalize += weight;
            }

            newval = newval / normalize;
            this.Value = newval;
        }

        public bool CheckConstraint()
        {
            return (Value < Max && Value > Min);
        }

        public void FixConstraint()
        {
            if (!CheckConstraint())
            {
                if (Value > Max)
                {
                    this.Value = Max;
                }
                else if (Value < Min)
                {
                    this.Value = Min;
                }
            }
        }

        public int GetBytes()
        {
            double range = (double)this.Variation * 2;
            double log = Math.Log(range, 2);
            int bytes = Convert.ToInt32(log);
            return Math.Max(4, bytes);
        }

        public void Project(double d)
        {
            this.Value = this.Min + d * 2 * (double)this.Variation;
        }

        public void ShiftCenter(double c)
        {
            this.Value = this.Min + c * 2 * (double)this.Variation;
        }

        public double GetPoint()
        {
            return (this.Value - this.Min) / (2 * (double)this.Variation);
        }

        public DesignVar Copy(DesignVar dvar)
        {
            double val = dvar.Value;
            double min = dvar.Min;
            double max = dvar.Max;

            return new DesignVar(val, min, max);
        }
    }
}
