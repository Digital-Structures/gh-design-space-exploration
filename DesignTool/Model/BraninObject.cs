using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using StructureEngine.Analysis;

namespace StructureEngine.Model
{
    public class BraninObject : BaseDesign, IDesign
    {
        public BraninObject(double num1, double num2)
        {
            this.x1 = new CoordVar(2.5, 7.5);
            this.x2 = new CoordVar(7.5, 7.5);
            x1.Value = num1;
            x2.Value = num2;
        }

        public CoordVar x1, x2;

        public double GetOutput()
        {
            BraninTestFunction f = new BraninTestFunction();
            return f.Analyze(this);
        }

        public double Score
        {
            get
            {
                return GetOutput();
            }
            private set
            {
                Score = value;
            }
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

        public IDesign DesignClone()
        {
            return new BraninObject(x1.Value, x2.Value);
        }

        public override IDesign GenerateFromVars(double[] v)
        {
            BraninObject gen = (BraninObject)this.DesignClone();
            for (int i = 0; i < v.Length; i++)
            {
                gen.DesignVariables[i].Project(v[i]);
            }

            return gen;
        }

        public override List<IVariable> DesignVariables
        {
            get
            {
                List<IVariable> vars = new List<IVariable>();
                vars.Add(x1);
                vars.Add(x2);
                return vars;
            }
        }

        public IDesign Crossover(IList<IDesign> seeds)
        {
            throw new NotImplementedException();
        }
        public IDesign Mutate(ISetDistribution dist, double rate)
        {
            BraninObject copy = (BraninObject)this.DesignClone();

            // loop over all the design variables
            foreach (IVariable var in copy.DesignVariables)
            {
                var.Mutate(rate, dist);
                var.FixConstraint();
            }

            return copy;
        }
        public void Setup()
        {
            throw new NotImplementedException();
        }
        public Evolutionary.IDivBooster GetDivBooster()
        {
            throw new NotImplementedException();
        }
        public double[] Dimensions
        {
            get { throw new NotImplementedException(); }
        }
        public double[] ZeroPoint
        {
            get { throw new NotImplementedException(); }
        }
        //public IDesignVM GetVM()
        //{
        //    return new ComputedStructureVM(new ComputedStructure());
        //}
        public int GetMaxPop()
        {
            int max = 0;
            foreach (IVariable v in DesignVariables)
            {
                max += v.GetBytes(); // = n
            }
            //return max * 4 * 2; // return 2 * 4n
            return Math.Min(max * 4 * 2, 150);
        }
    }
}
