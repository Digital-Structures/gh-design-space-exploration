using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using StructureEngine.Analysis;
using StructureEngine.MachineLearning;
using StructureEngine.Model;

namespace StructureEngine.Evolutionary
{
    public class PopulationGenerator
    {

        public PopulationGenerator(EvoParams p, IList<IDesign> newSeeds)
        {
            Seeds = newSeeds;
            this.Initialize(p);
        }

        public PopulationGenerator(EvoParams p)
        {
            this.Initialize(p);
        }

        private void Initialize(EvoParams p)
        {
            Params = p;
            NormalGenerator = new Normal();
            NormalGenerator.RandomSource = Utilities.MyRandom;
            UniformGenerator = new ContinuousUniform();
            UniformGenerator.RandomSource = Utilities.MyRandom;
            //AnalysisEngine = an;
        }

        private EvoParams Params
        {
            get;
            set;
        }

        public IList<IDesign> Seeds
        {
            get;
            private set;
        }

        //private IAnalysis AnalysisEngine;

        private Normal NormalGenerator
        {
            get;
            set;
        }

        private ContinuousUniform UniformGenerator
        {
            get;
            set;
        }

        //private Random RandGenerator
        //{
        //    get;
        //    set;
        //}

        public IList<IDesign> GenerateAndSelectTop(int number, IDesign myStructure)
        {
            // get new generation
            IList<IDesign> designs = this.NewGeneration(myStructure, NormalGenerator, null);

            // sort and identify top performers
            IList<IDesign> best = this.FindTopDesigns(designs, number);

            return best;
        }

        public IList<IDesign> MultiGenerateAndSelectTop(int number, IDesign myStructure, int generations)
        {
            IList<IDesign> gen = GenerateAndSelectTop(number, myStructure);
            for (int i = 1; i < generations; i++)
            {
                List<IDesign> seeds = new List<IDesign>();
                seeds.AddRange(gen.Take(2));
                PopulationGenerator p = new PopulationGenerator(this.Params, seeds);
                gen = p.GenerateAndSelectTop(number, myStructure);
            }

            return gen;
        }

        public IList<IDesign> GenerateAndSelectTop(int number, IDesign myStructure, Regression reg)
        {
            // create RegAnalysis object from regression model
            RegAnalysis rega = new RegAnalysis(reg);

            // get new generation
            IList<IDesign> designs = this.NewGeneration(myStructure, NormalGenerator, rega);

            // sort and identify top performers
            List<IDesign> best = this.FindTopDesigns(designs, 2 * number);

            // use structural analysis to verify top performers
            //TrussAnalysis ta = new TrussAnalysis();
            foreach (IDesign s in best)
            {
                double score = EvaluateDesign(s, rega);
            }
            best.Sort(CompareStructures);

            // choose final top
            best.RemoveRange(number, best.Count - number);
            return best;
        }

        private IList<IDesign> NewGeneration(IDesign myStructure, ISetDistribution dist, IAnalysis a)
        {
            // set up base structure
            IDesign typ = Seeds == null || Seeds.Count == 0 ? myStructure : Seeds[0];
            typ.Setup();


            // generate new generation
            List<IDesign> designs = new List<IDesign>();

            // start "stopwatch"
            long before = DateTime.Now.Ticks;

            int count = 0;
            while (count < Params.GenSize)
            {
                // generate random new structure
                IDesign randStructure = this.GenerateDesign(typ, dist);

                //// copy into computed structure for analysis
                //IDesign compStructure = new IDesign(randStructure); // copy

                // analyze
                double score = this.EvaluateDesign(randStructure, a);
                if (!double.IsNaN(score))
                {
                    designs.Add(randStructure);
                    count++;
                }
            }

            // stop the "stopwatch"
            long after = DateTime.Now.Ticks;
            TimeSpan elapsedTime = new TimeSpan(after - before);
            double milliseconds = elapsedTime.TotalMilliseconds;

            return designs;
        }

        private List<IDesign> FindTopDesigns(IList<IDesign> designs, int number)
        {
            List<IDesign> myList = (List<IDesign>)designs;
            myList.Sort(CompareStructures);
            int i = 0;
            List<IDesign> structures = new List<IDesign>();
            List<IDesign> badstructures = new List<IDesign>();

            while (structures.Count < number && i < myList.Count)
            {
                IDesign candidate = myList[i];
                if (CheckDiversity(candidate, structures))
                {
                    structures.Add(candidate);
                }
                else
                {
                    badstructures.Add(candidate);
                }
                i++;
            }


            // add more structures to population if there aren't enough good ones
            if (structures.Count < number)
            {
                int howManyMore = number - structures.Count;
                structures.AddRange(badstructures.Take(howManyMore));

                // resort list in case of newly added designs
                structures.Sort(CompareStructures);
            }

            return structures;
        }

        private double EvaluateDesign(IDesign comp, IAnalysis a)
        {
            if (a == null)
            {
                return comp.Score;
            }
            else
            {
                return a.Analyze(comp);
            }
        }

        private bool CheckDiversity(IDesign s, List<IDesign> list)
        {
            IDivBooster div = s.GetDivBooster();
            return div.IsDiverse(list, s, Params.MutRate);
        }

        private IDesign GenerateDesign(IDesign typ, ISetDistribution dist)
        {
             // generate new structure through crossover of parent seeds
            IDesign newStructure = typ.Crossover(this.Seeds);

             // mutate new structure
            newStructure = newStructure.Mutate(dist, Params.MutRate);

            return newStructure;
        }
                
        public static int CompareStructures(IDesign s1, IDesign s2)
        {
            return s1.Score.CompareTo(s2.Score);
        }
    }
}
