using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using StructureEngine.Analysis;
//using StructureEngine.DesignModes;
using StructureEngine.Model;

namespace StructureEngine.MachineLearning.Testing
{
    public class RegCase
    {
        public RegCase(IDesign p, int n, int n1, int n15, bool r, bool e, bool k, Matrix<double> w, SampleType s, string name)
        {
            this.Problem = p;
            this.NumSamples = n;
            this.nUnderOne = n1;
            this.nUnderOnePointFive = n15;
            this.BuildRF = r;
            this.BuildENN = e;
            this.BuildKR = k;
            this.ErrorWeights = w;
            this.SamplingPlan = s;
            this.ProbName = name;
        }

        public IDesign Problem;
        public int NumSamples, nUnderOne, nUnderOnePointFive;
        public string ProbName;
        public bool BuildRF, BuildENN, BuildKR;
        public Matrix<double> ErrorWeights;
        public SampleType SamplingPlan;

        public RegCase Clone()
        {
            return new RegCase(Problem.DesignClone(), NumSamples, nUnderOne, nUnderOnePointFive, BuildRF, BuildENN, BuildKR, ErrorWeights.Clone(), SamplingPlan, this.ProbName);
        }
    }

    public class ParametricStudy
    {
        public ParametricStudy()
        {
            this.Cases = new List<RegCase>();
            this.NumRuns = 10;
            this.AddCases();
        }

        public List<RegressionReport> Run(IAnalysis MyAnalysis)
        {
            var myReports = new List<RegressionReport>();
            SurrogateModelBuilder builder = new SurrogateModelBuilder(MyAnalysis);
            foreach (RegCase reg in Cases)
            {
                myReports.Add(builder.BuildModel(reg));
            }
            return myReports;
        }

        public List<RegCase> Cases;
        private int NumRuns;

        private void AddCases()
        {
            var setup = new StructureSetup();
            var plist = new List<Tuple<IDesign, string>> { new Tuple<IDesign, string>(setup.Designs[3], "RigidFrame") };
            var nlist = new List<int> { 20, 50, 100, 200, 400 };
            var mlist = new List<bool[]> { new bool[] { true, false, false } };
            var elist = new List<bool> { false };
            var klist = new List<bool> { false };
            var ilist = new List<Matrix<double>> { new DenseMatrix(1, 6, new double[] { 0, 0, 1, 1, 1, 1 }) };
            var slist = new List<SampleType> { SampleType.WeightedLatinHypercube };
            var wlist = new List<int[]> { new int[] { 2, 1 } };

            foreach (Tuple<IDesign, string> p in plist)
            {
                foreach (int n in nlist)
                {
                    foreach (bool[] m in mlist)
                    {
                        foreach (Matrix<double> i in ilist)
                        {
                            foreach (SampleType s in slist)
                            {
                                foreach (int[] w in wlist)
                                {
                                    RegCase c = new RegCase(p.Item1, n, w[0], w[1], m[0], m[1], m[2], i, s, p.Item2);
                                    for (int iter = 0; iter < NumRuns; iter++)
                                    {
                                        Cases.Add(c);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
