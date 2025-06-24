using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StormCloud.Evolutionary;

namespace StormCloud.Evolutionary
{
    public class Design
    {
        public Design()
        {
        }

        // Constructor to use
        public Design(List<DesignVar> dvar)
        {
            this.DesignVariables = dvar;
        }

        public List<DesignVar> DesignVariables
        {
            get;
            set;
        }

        public void CheckConstraints()
        {
            foreach (DesignVar var in this.DesignVariables)
            {
                if (!var.CheckConstraint())
                {
                    var.FixConstraint();
                }
            }
        }

        public Design DesignClone()
        {
            List<DesignVar> designvars = new List<DesignVar>();
            foreach (DesignVar dvar in this.DesignVariables)
            {
                double val = dvar.Value;
                double min = dvar.Min;
                double max = dvar.Max;
                DesignVar newdvar = new DesignVar(val, min, max);
                designvars.Add(newdvar);
                Console.WriteLine("SAME OBJECT");
                Console.Write(newdvar == dvar);
                Console.WriteLine(100000);
            }
            return new Design(this.DesignVariables);
        }



        public Design Crossover(List<Design> seeds)
        {
            Design crossed = (Design)this.DesignClone();

            // if no parents are selected, crossover does not occur
            if (seeds == null || seeds.Count == 0)
            {
                return crossed; // return original design if no seed, then mutate it
            }

            // otherwise, return a crossover of all the parent seeds
            else
            {
                for (int i = 0; i < this.DesignVariables.Count; i++)
                {
                    DesignVar var = crossed.DesignVariables[i];
                    List<DesignVar> seedvars = new List<DesignVar>();

                    foreach (Design d in seeds)
                    {
                        Design cd = (Design)d;
                        seedvars.Add(cd.DesignVariables[i]);
                    }

                    var.Crossover(seedvars);
                }

                return crossed;
            }
        }

        public Design Mutate(MathNet.Numerics.Distributions.IContinuousDistribution dist, double rate)
        {
            Design copy = (Design)this.DesignClone();

            // loop over all the design variables
            foreach (DesignVar var in copy.DesignVariables)
            {
                var.Mutate(rate, dist);
            }

            copy.CheckConstraints();
            return copy;
        }
    }
}