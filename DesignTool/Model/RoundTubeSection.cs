using System;
using System.Collections.Generic;
using System.Linq;

namespace StructureEngine.Model
{
    public class RoundTubeSection : ISection
    {
        public RoundTubeSection(double p, string n)
        {
            this.SectionParameter = p;
            this.Name = n;
        }

        public RoundTubeSection() // Default settings
        {
            this.SectionParameter = 0.05;
            this.Name = "RT0.05";
        }

        public SectionType Type
        {
            get
            {
                return SectionType.RoundTube;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public double GetReqEnvArea(Dictionary<LoadCase, double> force, double sigma, double E, double L)
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
            else // for compression, assume a hollow pipe with x% wall thickness
            {
                // I = PI/4 * r^4 - PI/4 * (0.95r)^4 = PI/4 * (r^4 - (1-0.95^4)r^4)
                // = 0.145686 r^4
                double percent = 1 - (double)this.SectionParameter;

                // safety factor of 3
                double sf = 3;
                double Ireq = sf * (Math.Abs(f) * Math.Pow(L, 2)) /
                                  (Math.Pow(Math.PI, 2) * E);

                double rreq = Math.Pow((4 / ((1 - Math.Pow(percent, 4)) * Math.PI)) * Ireq, 0.25);

                double a_stress = Math.Abs(f) / sigma;
                double a_buckling = Math.PI * Math.Pow(rreq, 2) * (1 - Math.Pow(percent, 2));

                return (Math.Max(a_stress, a_buckling));
            }
        }

        public double GetReqThickness(double reqArea)
        {
            double percent = 1 - (double)SectionParameter;
            double r = Math.Sqrt(reqArea / (Math.PI * (1 - Math.Pow(percent, 2))));
            return 2 * r;
        }

        public double GetReqMomInertia(double reqArea)
        {
            double percent = 1 - (double)SectionParameter;
            double r = Math.Sqrt(reqArea / (Math.PI * (1 - Math.Pow(percent, 2))));
            return (Math.PI / 4) * Math.Pow(r, 4) * (1 - Math.Pow(percent, 4));
        }

        public double SectionParameter // wall thickness as percent of radius
        {
            get;
            set;
        }

        public ISection SectionClone()
        {
            return new RoundTubeSection((double)this.SectionParameter, this.Name);
        }
    }
}
