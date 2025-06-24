using System;
using System.Collections.Generic;
using StructureEngine.Analysis;
using StructureEngine.Evolutionary;
using StructureEngine.Model;
using MathNet.Numerics.Distributions;

namespace StructureEngine.Grammar
{
    public abstract class BaseShape : ElementGroup, IShape
    {
        public abstract Structure ConvertToStructure();

        public abstract IDesign DesignClone();

        public abstract IShape Clone();

        abstract public double Score
        {
            get;
            set;
        }

        public void UpdateScore(double s)
        {
            Score = s;
        }

        public IAnalysis MyAnalysis
        {
            get;
            set;
        }

        public abstract bool LooksSame(IShape that);

        public ShapeHistory History
        {
            get;
            set;
        }

        protected IShape BaseClone(IShape copy)
        {
            copy.History = this.History.Clone();
            copy.Parent1 = this.Parent1;
            copy.Parent2 = this.Parent2;
            copy.SplicePoint1 = this.SplicePoint1;
            copy.SplicePoint2 = this.SplicePoint2;
            copy.Score = this.Score;
            return copy;
        }

        //private void ReplaceWith(IShape newshape)
        //{
        //    this = newshape.Clone();
        //    this.Parent1 = newshape.Parent1.Clone();
        //    this.Parent2 = newshape.Parent1.Clone();
        //    this.SplicePoint1 = newshape.SplicePoint1;
        //    this.SplicePoint2 = newshape.SplicePoint2;
        //}

        public IShape GoBack()
        {
            int k = History.Derivation.Count; // index of most recent shape
            if (k > 0) // if current is not the first model in history
            {
                IGrammar g = this.GetGrammar();
                ShapeHistory histclone = History.Clone();
                histclone.GoBack();
                IShape newshape = histclone.ApplyHistory(g);
                return newshape;
            }
            else
            {
                return this;
            }
        }

        public IShape Parent1
        {
            get;
            set;
        }

        public IShape Parent2
        {
            get;
            set;
        }

        public int SplicePoint1
        {
            get;
            set;
        }

        public int SplicePoint2
        {
            get;
            set;
        }

        public abstract IShapeState ShapeState
        {
            get;
            set;
        }

        public abstract IGrammar GetGrammar();

        public IDesign Crossover(IList<IDesign> seeds)
        {
            IDesign pick;
            if (seeds.Count == 0)
            {
                IGrammar g = this.GetGrammar();
                RandomComputation rc = new RandomComputation(g);
                pick = rc.GenerateRandShape();
            }

            else if (seeds.Count == 1)
            {
                pick = seeds[0].DesignClone();
            }

            else
            {
                IGrammar g = this.GetGrammar();
                IShape c1 = (IShape)seeds[Utilities.MyRandom.Next(seeds.Count)];
                IShape c2 = (IShape)seeds[Utilities.MyRandom.Next(seeds.Count)];
                List<IShape> crossed = g.AllCrossover(c1, c2);
                pick = crossed[Utilities.MyRandom.Next(crossed.Count)];
            }

            //IAnalysis a = this.GetGrammar().GetAnalysis();
            //pick.Score = a.Analyze(pick);

            return pick;
        }

        public IDesign Mutate(ISetDistribution dist, double rate)
        {
            // decide whether to mutate
            int max;
            if (rate == 0)
            {
                max = 1;
            }
            else
            {
                double maxd = Math.Pow(rate, -1);
                max = Convert.ToInt32(Math.Round(maxd)) + 1;
            }
            
            //int max = rate < 0.5 ? 10 : 2;

            foreach (RuleSet rs in this.History.Derivation)
            {
                for (int i = 0; i < rs.Rule.Params.Count; i++)
                {
                    // decide whether to mutate
                    int m = dist.RandomSource.Next(max);
                    bool mutate = m == 1;

                    // mutate 
                    if (mutate)
                    {
                        rs.Param[i] = rs.Rule.Params[i].Mutate(dist, rate, rs.Param[i]);
                    }
                }
            }

            IDesign d = this.History.ApplyHistory(this.GetGrammar());
            //IAnalysis a = this.GetGrammar().GetAnalysis();
            //d.Score = a.Analyze(d);

            return d;
        }

        public void Setup()
        {
        }

        public IDivBooster GetDivBooster()
        {
            return new GramDivBooster();
        }

        public IList<double> GetFeatures()
        {
            throw new NotImplementedException();
        }

        public IList<double[]> GetBounds()
        {
            throw new NotImplementedException();
        }

        public double GetOutput()
        {
            throw new NotImplementedException();
        }

        //public double[] ZeroPoint
        //{
        //    get { throw new NotImplementedException(); }
        //}

        public double[] ZeroPoint
        {
            get
            {
                return new double[] { this.ZeroShapePoint.X, this.ZeroShapePoint.Y };
            }
        }

        //public IDesignVM GetVM()
        //{
        //    return new ShapeVM(this, this.GetGrammar());
        //}

        public int GetMaxPop()
        {
            BaseGrammar g = (BaseGrammar)GetGrammar();
            return g.UniqueRules.Count * 10;
        }

        public IDesign GenerateFromVars(double[] v)
        {
            throw new NotImplementedException();
        }

        public List<IVariable> DesignVariables
        {
            get { throw new NotImplementedException(); }
        }

        public double? CompTime
        {
            get;
            set;
        }

        public double[] GetPoints()
        {
            throw new NotImplementedException();
        }

        public List<IDesign> GetCornerDesigns()
        {
            throw new NotImplementedException();
        }
    }
}
