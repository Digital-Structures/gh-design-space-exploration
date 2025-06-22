using GH_IO.Serialization;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI.RemotePanel;
using Grasshopper.Kernel;
using System;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper.Kernel.Special;

namespace StormCloud.Evolutionary
{
    class GH_VarNumberSlider : GH_NumberSlider, IVariable
    {
        public double Value // change to decimal??
        {
            get
            {
                return (double)this.CurrentValue;
            }
            set
            {
                this.SetSliderValue((decimal)Value);
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
                double v = (double)paramval.Value;
                newval += v * weight;
                normalize += weight;
            }

            newval = newval / normalize;
            this.SetSliderValue((decimal)newval);
        }

        public bool CheckConstraint()
        {
            throw new NotImplementedException();
        }

        public void FixConstraint()
        {
            throw new NotImplementedException();
        }

        public int GetBytes()
        {
            throw new NotImplementedException();
        }

        public double Min
        {
            get { return (double)this.Slider.Minimum; }
        }

        public double Max
        {
            get { return (double)this.Slider.Maximum; }
        }
    }
}
