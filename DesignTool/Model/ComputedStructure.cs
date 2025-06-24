using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using StructureEngine.Analysis;
using StructureEngine.Evolutionary;

namespace StructureEngine.Model
{
    public class ComputedStructure : Structure, IDesign
    {
        public ComputedStructure() : base()
        {
        }
        public ComputedStructure(Structure s) : base()
        {
            // "clone" the structure
            s.CopyTo(this);
        }

        public override IDesign GenerateFromVars(double[] v)
        {
            ComputedStructure gen = (ComputedStructure)this.DesignClone();
            for (int i = 0; i < v.Length; i++)
            {
                gen.DesignVariables[i].Project(v[i]);
            }

            // ensure mutated structure meets constraints
            gen.CheckConstraints();

            // include master/mirror rules
            //StructureVM svm = new StructureVM();
            //svm.Reset(gen);
            //svm.EnforceRelationships();
            gen.EnforceRelationships();

            return gen;
        }

        private double _DetK;
        public double DetK
        {
            get
            {
                if (!this.Analyzed)
                {
                    MyAnalysis.Analyze(this);
                }
                return _DetK;
                //return 1;
            }
            set
            {
                _DetK = value;
            }
        }

        private double? _Score;
        public double Score
        {
            get
            {
                if (_Score == null)
                {
                    return MyAnalysis.Analyze(this);
                }
                else
                {
                    return (double)_Score;
                }
            }
        }

        public void UpdateScore(double s)
        {
            this._Score = s;
        }


        private IAnalysis MyAnalysis
        {
            get
            {
                if (this.StructType == StructureType.Truss)
                {
                    return new TrussAnalysis();
                }
                else if (this.StructType == StructureType.Frame)
                {
                    return new EquivFrameAnalysis();
                }
                else
                {
                    throw new Exception("Structural type must be defined.");
                }
            }
        }

        public bool Analyzed;

        public void SetStart()
        {
            if (StructType == StructureType.Frame)
            {
                EquivFrameAnalysis efa = (EquivFrameAnalysis)MyAnalysis;
                efa.SetMemberAreas(this);
                efa.OrderEnvNodes(this);
                //return cs;
            }
            else if (StructType == StructureType.Truss)
            {
                return;
                //return this;
            }
            else
            {
                throw new Exception("Structure must have a type.");
            }
        }

        private void MemberContribution(ComputedMember m, Matrix<double> K, int dofs)
        {
            for (int j = 0; j < 2 * dofs; j++)
            {
                for (int l = 0; l < 2 * dofs; l++)
                {
                    int DOFj = m.LocalDOFs[j].Index;
                    int DOFk = m.LocalDOFs[l].Index;
                    double k_jl = m.CalculateStiffness()[j, l];

                    K[DOFj, DOFk] += k_jl;
                }
            }
        }

        private Matrix<double> CalculateStiffnessConst()
        {
            int dofs = Members[0].NodeI.DOFs.Length;

            Matrix<double> K = new DenseMatrix(this.DOFs.Count);
            foreach (ComputedMember m in UnaffectedMembers)
            {
                this.MemberContribution(m, K, dofs);
            }

            return K;
        }
        private Matrix<double> CalculateStiffnessVar()
        {
            long start = DateTime.Now.Ticks;
            int dofs = Members[0].NodeI.DOFs.Length;

            Matrix<double> K = new DenseMatrix(this.DOFs.Count);
            foreach (ComputedMember m in AffectedMembers)
            {
                this.MemberContribution(m, K, dofs);
            }

            long stop = DateTime.Now.Ticks;
            double ms = (new TimeSpan(stop - start)).Milliseconds;
            return K;
        }

        private Matrix<double> StiffnessConstant;
        private Matrix<double> StiffnessVariable;
        public Matrix<double> StiffnessMatrix
        {
            get
            {
                if (StiffnessConstant == null)
                {
                    StiffnessConstant = this.CalculateStiffnessConst();
                }
                if (StiffnessVariable == null)
                {
                    StiffnessVariable = this.CalculateStiffnessVar();
                }
                return StiffnessVariable + StiffnessConstant;
                //return StiffnessVariable;
            }
        }
        public void ClearStiffness()
        {
            this.StiffnessConstant = null;
            this.StiffnessVariable = null;
        }

        public IEnumerable<ComputedMember> ComputedMembers
        {
            get
            {
                return this.Members.Cast<ComputedMember>();
            }
        }
        public override Member GetNewMember(Node i, Node j)
        {
            return new ComputedMember(i, j);
        }

        new public IDesign DesignClone()
        {
            return (ComputedStructure)this.CloneImpl();
        }
        protected override Structure CloneImpl()
        {
            var cs = new ComputedStructure();
            this.CopyTo(cs);
            return cs;
        }
        internal override void CopyTo(Structure s)
        {
            base.CopyTo(s);

            var cs = s as ComputedStructure;
            if (cs != null)
            {
                //cs._Result = _Result == null ? null : new Results()
                //{
                //    //DetK = _Result.DetK,
                //    //Disp = _Result.Disp,
                //    Weight = _Result.Weight
                //};
                //cs.DetK = this.DetK;
                //cs.Score = this.Score;
                cs.CompTime = this.CompTime;
                cs.StiffnessConstant = this.StiffnessConstant;
                cs.Analyzed = false;
            }
        }

        public IDesign Crossover(IList<IDesign> seeds)
        {
            ComputedStructure crossed = (ComputedStructure)this.DesignClone();

            // if no parents are selected, crossover does not occur
            if (seeds == null || seeds.Count == 0)
            {
                return crossed;
            }

            // otherwise, return a crossover of all the parent seeds
            else
            {
                for (int i = 0; i < this.DesignVariables.Count; i++)
                {
                    IVariable var = crossed.DesignVariables[i];
                    List<IVariable> seedvars = new List<IVariable>();

                    foreach (IDesign s in seeds)
                    {
                        ComputedStructure cs = (ComputedStructure)s;
                        seedvars.Add(cs.DesignVariables[i]);
                    }

                    var.Crossover(seedvars);
                }

                return crossed;
            }
        }

        public void Setup()
        {
            int dimension = Nodes[0].DOFs.Length;
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Index = i;
            }
        }

        public IDesign Mutate(ISetDistribution dist, double rate)
        {
            ComputedStructure copy = (ComputedStructure)this.DesignClone();

            // loop over all the design variables
            foreach (IVariable var in copy.DesignVariables)
            {
                var.Mutate(rate, dist);
            }

            // ensure mutated structure meets constraints
            copy.CheckConstraints();

            // include master/mirror rules
            //StructureVM svm = new StructureVM();
            //svm.Reset(copy);
            //svm.EnforceRelationships();
            copy.EnforceRelationships();

            //// update stiffness matrix to reflect new geometry
            //copy.StiffnessVariable = copy.CalculateStiffnessVar();

            return copy;
        }

        public IDivBooster GetDivBooster()
        {
            return new ParDivBooster();
        }



        public double GetOutput()
        {
            //TrussAnalysis ta = new TrussAnalysis();
            //double score = ta.Analyze(this);
            ////double basescore = MainPage.CommonData.BaseStructure.Score;
            ////return score / basescore;

            //return score;
            return this.Score;
        }

        //public IDesignVM GetVM()
        //{
        //    return new ComputedStructureVM(this);
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
