using System;
using System.Collections.Generic;
using StructureEngine.Model;

namespace StructureEngine.Evolutionary
{
    public class ParDivBooster : IDivBooster
    {
        public ParDivBooster()
        {
        }

        private List<IDesign> Best;

        private ComputedStructure Candidate;

        double MutationRate;

        public bool IsDiverse(List<IDesign> existing, IDesign candidate, double rate)
        {
            this.Best = existing;
            this.Candidate = (ComputedStructure)candidate;
            this.MutationRate = rate;

            double diff = this.GetDiffSize();
            foreach (ComputedStructure s in this.Best)
            {
                double dist = GetDistance(s, Candidate);
                if (dist < diff)
                {
                    return false;
                }
            }
            return true;
        }

        private double GetDistance(ComputedStructure s1, ComputedStructure s2)
        {
            double dist = 0;
            for (int i = 0; i < s1.DesignVariables.Count; i++)
            {
                double var1 = s1.DesignVariables[i].Value;
                double var2 = s2.DesignVariables[i].Value;
                double sq = Math.Pow((var1 - var2), 2);
                dist += sq;
            }
            dist = Math.Sqrt(dist);
            return dist;
        }

        private double GetDiffSize()
        {
            // find size of design space
            double size = Candidate.SizeDesignSpace();

            // top performers must differ from each other by x% of the design space in each variable
            double allowablesize = size * MutationRate;
            double diffpercent = 0.3;

            return diffpercent * allowablesize;
        }
    }
}
