using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using StormCloud.ViewModel;


namespace StormCloud.Evolutionary
{
    public static class EvolutionaryUtilities
    {

        public static Random MyRandom = new Random(1);

        public static Normal NormalGenerator = new Normal() { RandomSource = MyRandom };

        public static ContinuousUniform UniformGenerator = new ContinuousUniform() { RandomSource = MyRandom };

        public static List<Design> NewGeneration(Design myFirstDesign, List<Design> seeds, IContinuousDistribution dist, int popsize, double rate) 
        {
            // generate new generation
            List<Design> Designs = new List<Design>();
            int count = 0;
            while (count < popsize)
            {
                // generate random new design
                Design randDesign = GenerateDesign( myFirstDesign, seeds, dist, rate);
                count++;
                Designs.Add(randDesign.DesignClone());
            }
            return Designs;
        }

        public static List<DesignVM> FindTopDesignsVM(List<DesignVM> DesignVMs, int number, double rate)
        {
            // number = 10 Designs to select e.g.
            List<DesignVM> myList = (List<DesignVM>)DesignVMs;
            myList.Sort(CompareDesigns);
            int i = 0;
            List<DesignVM> topDesigns = new List<DesignVM>();
            List<DesignVM> badDesigns = new List<DesignVM>();

            while (topDesigns.Count < number && i < myList.Count)
            {
                DesignVM candidate = (DesignVM)myList[i];
                if (CheckDiversity(candidate, topDesigns,rate))
                {
                    topDesigns.Add(candidate);
                }
                else
                {
                    badDesigns.Add(candidate);
                }
                i++;
            }


            // add more designs to population if there aren't enough good ones
            if (topDesigns.Count < number)
            {
                int howManyMore = number - topDesigns.Count;
                topDesigns.AddRange(badDesigns.Take(howManyMore));

                // resort list in case of newly added Designs
                topDesigns.Sort(CompareDesigns);
            }
            return topDesigns;
        }


        public static Design GenerateDesign(Design myFirstDesign, List<Design> seeds, IContinuousDistribution dist, double rate)
        {
            // generate new design through crossover of parent seeds
            Design copy = myFirstDesign.DesignClone();
            Design design = Crossover(copy, seeds);

            // mutate new structure
            Design newDesign= Mutate(design, dist, rate);
            return newDesign;
        }

        public static Design Mutate(Design d, IContinuousDistribution dist, double rate)
        {
            Design copy = d.DesignClone();
            List<DesignVar> newdvars = new List<DesignVar>();
            // loop over all the design variables

            foreach (DesignVar var in copy.DesignVariables)
            {
                DesignVar varcopy = var.VarClone();
                varcopy.Mutate(rate, dist);
                varcopy.FixConstraint();
                newdvars.Add(varcopy);
            }
            copy.DesignVariables = newdvars;
            return copy;
        }
        public static Design Crossover(Design d, List<Design> seeds)
        {
            Design crossed = d.DesignClone();

            // if no parents are selected, crossover does not occur
            if (seeds == null || seeds.Count == 0)
            {
                return crossed;
            }

            // otherwise, return a crossover of all the parent seeds
            else
            {
                for (int i = 0; i < d.DesignVariables.Count; i++)
                {
                    DesignVar var = crossed.DesignVariables[i];
                    List<DesignVar> seedvars = new List<DesignVar>();

                    foreach (Design s in seeds)
                    {
                        seedvars.Add(s.DesignVariables[i]);
                    }

                    var.Crossover(seedvars);
                }

                return crossed;
            }
        }
       
        public static bool IsDiverse(List<DesignVM> existing, DesignVM candidate, double rate)
        {

            double diff = GetDiffSize(candidate,rate);
            foreach (DesignVM s in existing)
            {
                double dist = GetDistance(s, candidate);
                if (dist < diff)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool CheckDiversity(DesignVM s, List<DesignVM> list, double rate)
        {
            return IsDiverse(list, s, rate);
        }

        private static double GetDistance(DesignVM d1, DesignVM  d2)
        {
            double dist = 0;
            for (int i = 0; i < d1.Design.DesignVariables.Count; i++)
            {
                double var1 = d1.Design.DesignVariables[i].Value;
                double var2 = d2.Design.DesignVariables[i].Value;
                double sq = Math.Pow((var1 - var2), 2);
                dist += sq;
            }
            dist = Math.Sqrt(dist);
            return dist;
        }

        public static double SizeDesignSpace(DesignVM d)
        {
            double size = 0;
            foreach (DesignVar var in d.Design.DesignVariables)
            {
                size += Math.Pow(var.Max - var.Min, 2);
            }
            size = Math.Sqrt(size);
            return size;
        }
        
        private static double GetDiffSize(DesignVM d, double rate)
        {
            // find size of design space
            double size = SizeDesignSpace(d);

            // top performers must differ from each other by x% of the design space in each variable
            double allowablesize = size * rate;
            double diffpercent = 0.3;
            return diffpercent * allowablesize;
        }

        public static int CompareDesigns(DesignVM d1, DesignVM d2)
        {
            return d1.Score.CompareTo(d2.Score);
        }
    }
}
