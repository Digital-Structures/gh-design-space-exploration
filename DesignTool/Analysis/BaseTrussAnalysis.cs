using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic;
using StructureEngine.Model;
using System.Collections.Generic;

namespace StructureEngine.Analysis
{
    public abstract class BaseTrussAnalysis
    {
        protected void RunAnalysis(ComputedStructure comp)
        {
            // start "stopwatch"
            long before = DateTime.Now.Ticks;

            Matrix<double> K = comp.StiffnessMatrix;
            long getK = DateTime.Now.Ticks;

            // Determine free and pinned DOFs
            int DOFs_pinned = comp.DOFs.Count(d => d.Pinned);
            int DOFS_free = comp.DOFs.Count - DOFs_pinned;

            // Assemble and partition stiffness matrix
            Matrix<double> Kff = K.SubMatrix(0, DOFS_free, 0, DOFS_free);
            Matrix<double> Ksf = K.SubMatrix(DOFS_free, DOFs_pinned, 0, DOFS_free);
            long partitionK = DateTime.Now.Ticks;

            // Check to make sure det !<= 0, which would mean structure is unstable
            double detK = Kff.Determinant();
            if (detK <= 0)
            {
                return;
            }

            // Check for diagonal elements equal to 0, which would mean structure is unstable
            for (int i = 0; i < Kff.RowCount; i++)
            {
                if (Kff[i, i] == 0)
                {
                    return;
                }
            }

            long posdefK = DateTime.Now.Ticks;

            foreach (ComputedMember m in comp.Members)
            {
                m.AxialForce.Clear();
            }

            foreach (DOF d in comp.DOFs)
            {
                d.Disp.Clear();
            }

            foreach (LoadCase lc in comp.LoadCases)
            {
                Vector<double> P = lc.GetLoadVector(comp);

                // Assemble and partition load vector
                Vector<double> Pf = P.SubVector(0, DOFS_free);

                // Solve for displacement vector
                Vector<double> Uf = new DenseLU((DenseMatrix)Kff).Solve(Pf);
                long solveK = DateTime.Now.Ticks;

                Vector<double> U = new DenseVector(comp.DOFs.Count);
                for (int i = 0; i < Uf.Count; i++)
                {
                    U[i] = Uf[i];
                }

                // Decorate DOFs with displacements for this load case
                for (int i = 0; i < U.Count; i++ )
                {
                    DOF myDOF = comp.DOFs.Single(d => d.Index == i);
                    myDOF.Disp.Add(lc, U[i]);
                }

                // Solve for reactions and complete load vector
                Vector<double> Ps = Ksf.Multiply(Uf);
                for (int i = 0; i < Ps.Count; i++)
                {
                    P[DOFS_free + i] = Ps[i];
                }
                long getReactions = DateTime.Now.Ticks;

                foreach (ComputedMember m in comp.ComputedMembers)
                {
                    Vector<double> disp = new DenseVector(4);
                    for (int j = 0; j < 4; j++)
                    {
                        disp[j] = U[m.LocalDOFs[j].Index];
                    }

                    Matrix<double> k = m.CalculateStiffness(); // local stiffness matrix
                    Vector<double> t = k.SubMatrix(2, 2, 0, 4).Multiply(disp);
                    Matrix<double> trig = new DenseMatrix(1, comp.Members[0].NodeI.DOFs.Length);

                    trig[0, 0] = Math.Cos(m.Angle);
                    trig[0, 1] = Math.Sin(m.Angle);

                    Vector<double> force = trig.Multiply(t);

                    //Array.Resize<double>(ref m.AxialForce, m.AxialForce.Length + 1);
                    //m.AxialForce[m.AxialForce.Length - 1] = force[0];
                    m.AxialForce.Add(lc, force[0]);
                }
            }

            long localSolve = DateTime.Now.Ticks;

            comp.DetK = detK;

            //Results r = new Results();
            ////r.Disp = U;
            //r.Weight = memberWeights.Sum();
            //r.DetK = detK;

            long getResults = DateTime.Now.Ticks;

            //comp.Result = r;

            // stop "stopwatch"
            long after = DateTime.Now.Ticks;
            TimeSpan elapsedTime = new TimeSpan(after - before);
            double milliseconds = elapsedTime.TotalMilliseconds;

            // collect extra time information
            double getKTime = (new TimeSpan(getK - before)).TotalMilliseconds;
            double partitionKTime = (new TimeSpan(partitionK - getK)).TotalMilliseconds;
            double posdefKTime = (new TimeSpan(posdefK - partitionK)).TotalMilliseconds;
            //double solveKTime = (new TimeSpan(solveK - posdefK)).TotalMilliseconds;
            //double getReactionsTime = (new TimeSpan(getReactions - solveK)).TotalMilliseconds;
            //double localSolveTime = (new TimeSpan(localSolve - getReactions)).TotalMilliseconds;
            double getResultsTime = (new TimeSpan(getResults - localSolve)).TotalMilliseconds;

            if (comp.CompTime == null)
            {
                comp.CompTime = milliseconds;
            }
        }

        private bool CheckMemberSizes(ComputedStructure comp)
        {
            bool closeEnough = true;

            foreach (ComputedMember m in comp.Members)
            {
                double req = m.ReqArea;
                double a = m.Area;

                if (Math.Abs(req - a) / req > 0.10) // if the required area is more than 10% off
                {
                    m.Area = req;
                    closeEnough = false;
                }
            }

            return closeEnough;
        }
    }
}
