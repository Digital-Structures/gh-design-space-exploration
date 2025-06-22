using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using StructureEngine.Model;
using System.Collections.Generic;

namespace StructureEngine.Test
{
    public class DSPlotter2D
    {
        public DSPlotter2D()
        {
            this.SetProblem();
        }

        private IDesign Problem;

        private void SetProblem()
        {
            StructureSetup s = new StructureSetup();
            IDesign p = s.Designs[0];
            if (p as ComputedStructure != null)
            {
                ComputedStructure c = (ComputedStructure)p;
                c.Nodes[1].DOFs[0].SetCoord(39.3701); // inches = 1m
                c.Nodes[1].DOFs[1].SetCoord(-39.3701); // inches = -1m
                c.Nodes[2].DOFs[0].SetCoord(118.11); // inches = 3m
                c.Nodes[2].DOFs[1].SetCoord(-39.3701); // inches = -1m
                c.Nodes[3].DOFs[0].SetCoord(78.7402); // inches = 2m
                c.Nodes[4].DOFs[0].SetCoord(157.48); // inches = 4m

                foreach (IVariable v in p.DesignVariables)
                {
                    v.SetConstraint();
                }

                c.Nodes[1].ConvertDOFtoVar(0, 39.3); // inches =  almost 1m
                c.Nodes[1].ConvertDOFtoVar(1, 157.48); // inches = 4m

                c.LoadCases[0].GetLoad(c.Nodes[3].DOFs[1]).Value = -22.48; // kips = 100KN
                c.SymmetryLine[0] = 78.7402; // inches = 2m

                this.Problem = p;
            }

            else
            {
                throw new Exception("Problem is not of type ComputedStructure.");
            }
        }

        public Matrix<double> GenerateData(int nR, int nC)
        {
            Matrix<double> m = new DenseMatrix(nR, nC);
            double deltaR = 1.0 / ((double)nR - 1.0);
            double deltaC = 1.0 / ((double)nC - 1.0);
            double minScore = Double.MaxValue;

            //for (double i = 0; i <= 1; i = i + deltaR)
            //{
            //    ind2 = 0;
            //    for (double j = 0; j <= 1; j = j + deltaC)
            //    {
            //        IDesign des = Problem.GenerateFromVars(new double[] { i, j });
            //        double score = des.Score;
            //        minScore = Math.Min(score, minScore);
            //        m[ind1, ind2] = score;
            //        ind2++;
            //    }
            //    ind1++;
            //}

            for (int i = 0; i < nR; i++)
            {
                for (int j = 0; j < nC; j++)
                {
                    double varX = i * deltaR;
                    double varY = j * deltaC;
                    IDesign des = Problem.GenerateFromVars(new double[] { varX, varY });
                    double score = des.Score;
                    minScore = Math.Min(score, minScore);
                    m[i, j] = score;
                }
            }

            m = m.PointwiseDivide(new DenseMatrix(nR, nC, minScore));
            return m;
        }

        public IEnumerable<IDesign> GetDesigns(Matrix<double> data, double score, double tolerance)
        {
            IList<IDesign> designs = new List<IDesign>();
            double m = data.RowCount;
            double n = data.ColumnCount;

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double diff = Math.Abs(data[i, j] - score);
                    if (diff <= tolerance)
                    {
                        if (Math.Abs(i / (m - 1.0) - 1.0) <= 0.001)
                        {
                        }
                        IDesign des = Problem.GenerateFromVars(new double[] { i / (m - 1.0), j / (n - 1.0) });
                        designs.Add(des); 
                    }
                }
            }

            return designs;
        }
    }
}
