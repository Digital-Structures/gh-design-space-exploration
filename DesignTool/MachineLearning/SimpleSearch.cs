using System;
using System.Collections.Generic;

namespace StructureEngine.MachineLearning
{
    public class SimpleSearch
    {
        public SimpleSearch(IFunction function, bool isMin, List<double> lowerBounds, List<double> upperBounds)
        {
            MyFunction = function;
            IsMin = isMin;
            LowerBounds = lowerBounds;
            UpperBounds = upperBounds;
            NumVars = LowerBounds.Count;
            MaxMutRate = 0.5;
            TopRatio = 0.2;
        }

        private IFunction MyFunction;
        private bool IsMin;
        private List<double> LowerBounds, UpperBounds;
        private int NumVars;
        private double MaxMutRate;
        private double TopRatio;

        public Tuple<double, List<double>> FindBest(int nIterations, int genSize, double stopCriteria)
        {
            List<Tuple<double, List<double>>> gen = InstantiateSearch(genSize);
            double bestValue = Double.MaxValue;
            Tuple<double, List<double>> best = gen[0];
            for (int i = 0; i < nIterations; i++)
            {
                gen.Sort(new PopComparer());
                if (!IsMin) { gen.Reverse(); }
                best = gen[0];

                if (Math.Abs(best.Item1 - bestValue) <= stopCriteria)
                {
                    //break;
                }

                gen = Mutate(gen, MaxMutRate - Math.Pow((i / (double)nIterations), 2) * MaxMutRate);
                //gen = Mutate(gen, MaxMutRate);
                bestValue = best.Item1;
            }
            return best;
        }

        private List<Tuple<double, List<double>>> InstantiateSearch(int n)
        {
            List<Tuple<double, List<double>>> gen1 = new List<Tuple<double, List<double>>>();
            while (gen1.Count < n)
            {
                List<double> vars = new List<double>();
                for (int i = 0; i < NumVars; i++)
                {
                    double r = Utilities.MyRandom.NextDouble() * (UpperBounds[i] - LowerBounds[i]) + LowerBounds[i];
                    vars.Add(r);
                }
                double result = MyFunction.GetOutput(vars);
                if (!Double.IsNaN(result))
                {
                    gen1.Add(new Tuple<double, List<double>>(result, vars));
                }
            }
            return gen1;
        }

        private List<Tuple<double, List<double>>> Mutate(List<Tuple<double, List<double>>> pop, double mutRate)
        {
            int nBest = (int)Math.Round(pop.Count * TopRatio);
            int nRest = pop.Count - nBest;

            List<Tuple<double, List<double>>> best = pop.GetRange(0, nBest);
            pop.RemoveRange(0, nBest);
            List<Tuple<double, List<double>>> newGen = new List<Tuple<double, List<double>>>();
            newGen.AddRange(best);

            for (int i = 0; i < nRest; i++)
            {
                int randInt1 = Utilities.MyRandom.Next(0, nBest - 1);
                int randInt2 = Utilities.MyRandom.Next(0, nBest - 1);
                Tuple<double, List<double>> parent1 = best[randInt1];
                Tuple<double, List<double>> parent2 = best[randInt2];
                Tuple<double, List<double>> mut = MutateElement(parent1, parent2, mutRate);
                newGen.Add(mut);
            }

            return newGen;
        }

        private Tuple<double, List<double>> MutateElement(Tuple<double, List<double>> el1, Tuple<double, List<double>> el2, double mutRate)
        {
            List<double> newVars = new List<double>();
            bool isGood = false;
            double output = 0;
            
            while (!isGood)
            {
                for (int i = 0; i < NumVars; i++)
                {
                    double lb = LowerBounds[i];
                    double ub = UpperBounds[i];
                    double range = ub - lb;
                    double v1 = el1.Item2[i];
                    double v2 = el2.Item2[i];

                    // crossover
                    double r1 = Utilities.MyRandom.NextDouble();
                    double r2 = 1.0 - r1;
                    double value = v1 * r1 + v2 * r2;

                    // mutation
                    double rand = Utilities.MyRandom.NextDouble() - 0.5;
                    rand = rand * mutRate * range;
                    double newVar = value + rand;
                    if (newVar < lb)
                    {
                        newVar = lb;
                    }
                    else if (newVar > ub)
                    {
                        newVar = ub;
                    }

                    newVars.Add(newVar);
                }


                output = MyFunction.GetOutput(newVars);

                if (!Double.IsNaN(output))
                {
                    isGood = true;
                }
            }

            return new Tuple<double, List<double>>(output, newVars);
        }
    }

    public class PopComparer : IComparer<Tuple<double, List<double>>>
    {
        public int Compare(Tuple<double, List<double>> x, Tuple<double, List<double>> y)
        {
            int compareResult = x.Item1.CompareTo(y.Item1);
            return compareResult;
        }
    }
}
