using System;
using System.Net;
using StructureEngine.Model;
using MathNet.Numerics.LinearAlgebra.Generic;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using StructureEngine.MachineLearning;
using MathNet.Numerics.Distributions;
using System.Linq;
using netDxf;
using StructureEngine.Serialization;
using StructureEngine.Analysis;

namespace StructureEngine.Test
{
    public class DSGenerator
    {
        public DSGenerator()
        {
        }

        public DSGenerator(IDesign problem)
        {
            this.Problem = problem;
        }

        private IDesign Problem;

        public Tuple<Matrix<double>, IList<IDesign>> GetData(IDesign prob, int num)
        {
            this.Problem = prob;
            int numVars = Problem.DesignVariables.Count;

            Sampling samp = new Sampling(Problem, new ContinuousUniform());
            List<Observation> samples = samp.GetSample(num, SampleType.RandomLatinHypercube, 0, 0);
            List<IDesign> des = samples.Select(o => o.obsDesign).ToList();

            Matrix<double> m = this.MatfromDes(des);

            return new Tuple<Matrix<double>, IList<IDesign>>(m, des);
        }

        public Tuple<Matrix<double>, IList<IDesign>> GetData(IDesign prob, int num, double[] centers, double[] ranges)
        {


            int numVars = prob.DesignVariables.Count;

            if (prob.DesignVariables.Count != centers.Length || prob.DesignVariables.Count != ranges.Length)
            {
                throw new Exception("Centers and ranges must be defined for each design variable.");
            }

            for (int i = 0; i < prob.DesignVariables.Count; i++)
            {
                CoordVar d = prob.DesignVariables[i] as CoordVar;
                d.ShiftCenter(centers[i]);
                d.Project(0.5);
                d.AllowableVariation = d.AllowableVariation * ranges[i];
            }

            this.Problem = prob;
            Sampling samp = new Sampling(Problem, new ContinuousUniform());
            List<Observation> samples = samp.GetSample(num, SampleType.RandomLatinHypercube, 0, 0);
            List<IDesign> des = samples.Select(o => o.obsDesign).ToList();

            foreach (IDesign design in des)
            {
                for (int i = 0; i < design.DesignVariables.Count; i++)
                {
                    CoordVar d = design.DesignVariables[i] as CoordVar;
                    d.AllowableVariation = d.AllowableVariation / ranges[i];
                    double value = d.GetPoint();
                    d.ShiftCenter(1 - centers[i]);
                    d.Project(value - (0.5 - centers[i]));
                }
            }


            Matrix<double> m = this.MatfromDes(des);

            return new Tuple<Matrix<double>, IList<IDesign>>(m, des);
        }

        public Matrix<double> MatfromDes(IList<IDesign> designs)
        {
            int num = designs.Count;
            int numVars = designs[0].DesignVariables.Count;

            Matrix<double> m = new DenseMatrix(num, numVars + 1);

            for (int i = 0; i < num; i++)
            {
                IDesign d = designs[i];
                List<double> vars = d.DesignVariables.Select(v => v.GetPoint()).ToList();
                vars.Add(d.Score);

                //// include second design objective
                //if (d as ComputedStructure != null)
                //{
                //    var cs = (ComputedStructure)d;
                //    var dispAnalysis = new TrussDispAnalysis();
                //    double disp = dispAnalysis.Analyze(cs);
                //    vars.Add(disp);
                //}

                m.SetRow(i, vars.ToArray());
            }

            return m;
        }

        public IList<Tuple<Matrix<double>, IList<IDesign>>> GetBaseData(IDesign prob, int num, int numFixed)
        {
            //IList<Tuple<Matrix<double>, IList<IDesign>>> allData = new List<Tuple<Matrix<double>, IList<IDesign>>>();

            //// Generate baseline data that holds all variables fixed except for each variable pair
            //int numVars = prob.DesignVariables.Count;

            //for (int i = 1; i < numVars; i++)
            //{
            //    for (int j = 0; j < i; j++)
            //    {
            //        IDesign probClone = prob.DesignClone();
            //        List<IVariable> vars = probClone.DesignVariables.ToList();

            //        // Lock all but 2 variables
            //        foreach (IVariable v in probClone.DesignVariables)
            //        {
            //            if (v as DOF != null)
            //            {
            //                var d = (DOF)v;

            //                // Check index in cloned list since that list isn't changing
            //                if (vars.IndexOf(d) != i && vars.IndexOf(d) != j)
            //                {
            //                    d.Free = false;
            //                    d.AllowableVariation = 0;
            //                }
            //            }

            //            else
            //            {
            //                throw new Exception("Method only works for ComputedStructure objects.");
            //            }
            //        }

            //        // Sample 2-variable design space
            //        Tuple<Matrix<double>, IList<IDesign>> data = GetData(probClone, num);
            //        allData.Add(data);
            //    }
            //}

            //return allData;

            var allData = new List<Tuple<Matrix<double>, IList<IDesign>>>();
            var newList = new List<IDesign>();
            SetFixed(newList, prob.DesignClone(), 0, numFixed);
            foreach (IDesign des in newList)
            {
                Tuple<Matrix<double>, IList<IDesign>> data = GetData(des, num);
                allData.Add(data);
            }

            return allData;
        }

        public void SetFixed(IList<IDesign> runningList, IDesign des, int varIndex, int toFix)
        {
            // End condition
            if (toFix == 0) 
            {
                runningList.Add(des);
                return; 
            }
            if (varIndex >= des.DesignVariables.Count)
            {
                return; 
            }

            // Case 1: fix variable
            var desCopy = des.DesignClone();
            IVariable v = desCopy.DesignVariables[varIndex];
            if (!(v is CoordVar))
            {
                throw new Exception("Method only works for ComputedStructure objects.");
            }
            var d = (CoordVar)v;
            d.AllowableVariation = 0;
            SetFixed(runningList, desCopy, varIndex, toFix - 1);

            // Case 2: allow variable to remain free
            SetFixed(runningList, des, varIndex + 1, toFix);            
        }

        public IDesign GetDesign(double[] vars)
        {
            IDesign newdes = Problem.GenerateFromVars(vars);
            return newdes;
        }

        public DxfDocument DrawDesigns(List<double[]> allvars, int nrows, int ncols)
        {
            if (allvars.Count != nrows * ncols)
            {
                throw new Exception("Rows and columns must multiply to total.");
            }

            List<IDesign> designs = new List<IDesign>();
            foreach (double[] var in allvars)
            {
                IDesign des = GetDesign(var);
                designs.Add(des);
            }

            List<ComputedStructure> structures = designs.Cast<ComputedStructure>().ToList();

            DXFMaker drawer = new DXFMaker();
            DxfDocument doc = drawer.TableStructureDXF(structures, nrows, ncols);


            return doc;
        }
    }
}
