using System;
using System.Collections.Generic;
using System.Linq;

namespace StructureEngine.Model
{
    public class RectangularSection : ISection
    {
        public RectangularSection(double p, string n)
        {
            this.SectionParameter = p;
            this.Name = n;
        }

        public RectangularSection() // Default settings
        {
            this.SectionParameter = 0.009525; // meters for polycarbonte; TODO: update
            this.Name = "RecPC";
        }

        public SectionType Type
        {
            get
            {
                return SectionType.Rectangular;
            }
        }

        public double GetReqEnvArea(Dictionary<LoadCase, double>  force, double sigma, double E, double L)
        {
            List<double> reqarea = new List<double>();
            foreach (double f in force.Values)
            {
                reqarea.Add(GetReqArea(f, sigma, E, L));
            }
            return reqarea.Max();
        }

        public double GetReqArea(double f, double sigma, double E, double L)
        {
            if (f > 0)
            {
                return (Math.Abs(f) / sigma);
            }
            else // for compression, assume a rectangle with x% thickness
            {
                // I = Min(bh^3/12, b^3h/12)
                // safety factor of 3
                double sf = 1;
                double Ireq = sf * (Math.Abs(f) * Math.Pow(L, 2)) /
                                  (Math.Pow(Math.PI, 2) * E);

                double hreq1 = Math.Pow(Ireq * 12 / (double)SectionParameter, 1.0 / 3.0); // assuming thickness > height
                double hreq2 = Ireq * 12 / (Math.Pow((double)SectionParameter, 3)); // assuming height > thickness
                double hreq = Math.Min(hreq1, hreq2);

                double a_stress = Math.Abs(f) / sigma;
                double a_buckling = (double)SectionParameter * hreq;

                return (Math.Max(a_stress, a_buckling));
            }
        }

        public double GetReqThickness(double reqArea)
        {
            return reqArea / (double)SectionParameter;
        }

        public double GetReqMomInertia(double reqArea)
        {
            double h = reqArea / (double)SectionParameter;
            double I = Math.Min(Math.Pow(h, 3) * (double)SectionParameter, h * (Math.Pow((double)SectionParameter, 3))) / 12.0;
            return I;
        }

        public double SectionParameter // thickness
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public ISection SectionClone()
        {
            return new RectangularSection((double)this.SectionParameter, this.Name);
        }
    }
}
