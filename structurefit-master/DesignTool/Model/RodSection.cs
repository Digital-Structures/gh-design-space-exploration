using System;
using System.Collections.Generic;
using System.Linq;


namespace StructureEngine.Model
{
    public class RodSection : ISection
    {
        public RodSection(double p, string n)
        {
            this.SectionParameter = p;
            this.Name = n;
        }

        public RodSection() // Default settings
        {
            this.Name = "Rod";
        }

        public SectionType Type
        {
            get
            {
                return SectionType.Rod;
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
            else
            {
                // safety factor of 3
                double sf = 3;
                double Ireq = sf * (Math.Abs(f) * Math.Pow(L, 2)) /
                                  (Math.Pow(Math.PI, 2) * E);

                double rreq = Math.Pow(4 / Math.PI * Ireq, 0.25);

                double a_stress = Math.Abs(f) / sigma;
                double a_buckling = Math.PI * Math.Pow(rreq, 2);

                return (Math.Max(a_stress, a_buckling));
            }
        }

        public double GetReqThickness(double reqArea)
        {
            double r = Math.Sqrt(reqArea / Math.PI);
            return 2 * r;
        }

        public double GetReqMomInertia(double reqArea)
        {
            double r = Math.Sqrt(reqArea / Math.PI);
            double I = Math.PI * Math.Pow(r, 4) / 4;
            return I;
        }

        public double SectionParameter // N/A
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
            return new RodSection((double)this.SectionParameter, this.Name);
        }
    }
}
