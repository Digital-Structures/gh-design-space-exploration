using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StormCloud.Evolutionary;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace StormCloud.Evolutionary
{
    public class DVariableGH : IVariable
    {
        public DVariableGH(GH_NumberSlider slider)
        {
            this.Slider = slider;
        }
        
        public DVariableGH(double val, double min, double max)// : base(val)
        {
            this.Value= val;
            this.Min = min;
            this.Max = max;
        }
        
        public DVariableGH(double val, double min, double max, GH_NumberSlider slider)// : base(val)
        {
            this.Value = val;
            this.Min = min;
            this.Max = max;
            this.Slider = slider;
        }

        public GH_NumberSlider Slider
        {
            get;
            set;
        }

        public double Value // change to decimal??
        {
            get
            {
                return (double)this.Slider.CurrentValue;
            }
            set
            {
                this.Slider.SetSliderValue((decimal)value);
            }
        }

        public double AllowableVariation
        {
            get
            {
                return (this.Max - this.Min) / 2;
            }
        }

        public double MeanValue
        {
            get
            {
                return (this.Max + this.Min) / 2;
            }
        }

        public void Mutate(double globalrate, MathNet.Numerics.Distributions.IContinuousDistribution dist)
        {
            double sigma = globalrate * AllowableVariation;
         
            //dist.Mean = this.Value;
            //dist.StdDev = sigma;

            dist = new MathNet.Numerics.Distributions.Normal(this.Value, sigma);

            double newval = dist.Sample();
            this.Value = newval;
        }

        public void Crossover(System.Collections.Generic.List<IVariable> mylist)
        {
            double newval = 0;
            double normalize = 0;
            foreach (IVariable paramval in mylist)
            {
                double weight = Utilities.MyRandom.NextDouble();
                double v = paramval.Value;
                newval += v * weight;
                normalize += weight;
            }

            newval = newval / normalize;
                        this.Value = newval;
            //this.SetSliderValue((decimal)newval);
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

        public double Min
        {
            get
            { 
                return (double)this.Slider.Slider.Minimum; 
            }
            private set { }
        }

        public double Max
        {
            get
            {
                return (double)this.Slider.Slider.Maximum;
            }
            private set { }
        }

        public int GetBytes()
        {
            double range = (double)this.AllowableVariation * 2;
            double log = Math.Log(range, 2);
            int bytes = Convert.ToInt32(log);
            return Math.Max(4, bytes);
        }
    }
}
