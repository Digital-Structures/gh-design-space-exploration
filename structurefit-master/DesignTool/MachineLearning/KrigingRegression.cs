using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace StructureEngine.MachineLearning
{
    public static class MathUtility
    {
        public static Vector<double> PointwisePower(Vector<double> b, Vector<double> p)
        {
            if (b.Count != p.Count)
            {
                throw new ArgumentException("Vector lengths must match.");
            }

            else
            {
                int n = b.Count;
                Vector<double> v = new DenseVector(n);
                for (int i = 0; i < n; i++)
                {
                    v[i] = Math.Pow(b[i], p[i]);
                }

                return v;
            }
        }

        public static Vector<double> ListToVector(List<double> list)
        {
            Vector<double> v = new DenseVector(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                v[i] = list[i];
            }
            return v;
        }

        public static Vector<double> ListToVector(List<int> list)
        {
            Vector<double> v = new DenseVector(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                v[i] = list[i];
            }
            return v;
        }

        public static Tuple<List<double>, List<int>> GetDistances(Matrix<double> X)
        {
            int n = X.RowCount;
            //Vector<double> d = new DenseVector(n * (n - 1) / 2, 0);
            List<double> d = new List<double>();

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    //d[(i - 1) * n - (i - 1) * 1 / 2 + j - i] = (X.Row(i) - X.Row(j)).Norm(1);
                    d.Add((X.Row(i) - X.Row(j)).Norm(1));
                }
            }

            List<double> d_unique = new List<double>();
            List<int> J = new List<int>();
            foreach (var g in d.GroupBy(v => v))
            {
                d_unique.Add(g.Key);
                J.Add(g.Count());
            }

            return new Tuple<List<double>, List<int>>(d_unique, J);
        }
    }

    public class KrigingRegression : Regression
    {
        public KrigingRegression(List<Observation> lobs)
        {
            TrainData = lobs;
            X = Regression.GetSampleMatrix(lobs);
            y = Regression.GetObsVector(lobs);
            N = lobs.Count;
            one = new DenseVector(n, 1);
            P = new DenseVector(lobs[0].Features.Count, 2);
        }

        private Vector<double> Theta;
        private Vector<double> LogTheta;
        private Matrix<double> Psi;
        private DenseLU PsiLU;
        private Matrix<double> X;
        private Vector<double> y;
        private int N;
        private double mu;
        private double sigsqr;
        private Vector<double> one;
        private Vector<double> P;

        public override void BuildModel(int z1, double z2, int z3, int z4)
        {
            // Find values for Theta
            int numVars = TrainData[0].Features.Count;
            MLE mle = new MLE(TrainData, P);
            List<double> lb = new List<double>();
            List<double> ub = new List<double>();
            for (int i = 0; i < numVars; i++)
            {
                lb.Add(-3);
                ub.Add(2);
            }
            SimpleSearch ss = new SimpleSearch(mle, true, lb, ub);
            Tuple<double, List<double>> t = ss.FindBest(10, 20, 0.01);
            Vector<double> theta = new DenseVector(t.Item2.Count);
            for (int i = 0; i < t.Item2.Count; i++)
            {
                theta[i] = t.Item2[i];
            }
            this.LogTheta = theta;
            Vector<double> ten = new DenseVector(theta.Count, 10);
            this.Theta = MathUtility.PointwisePower(ten, LogTheta);

            // Find values for Psi, U, mu, and sigma squared
            Tuple<DenseLU, Matrix<double>, double, double> t2 = mle.GetLUFactor(t.Item2);
            this.PsiLU = t2.Item1;
            this.Psi = t2.Item2;
            mu = t2.Item3;
            sigsqr = t2.Item4;
        }

        public override double Predict(Observation test)
        {
            Vector<double> x = GetSampleVector(test);

            Vector<double> psi = new DenseVector(n);
            for (int i = 0; i < n; i++)
            {
                Vector<double> v = X.Row(i) - x;
                v = MathUtility.PointwisePower(v, P);
                v = v.PointwiseMultiply(Theta);
                double sum = v.Sum();
                psi[i] = Math.Exp(-1 * sum);
            }

            Vector<double> summand = psi.ToRowMatrix().Multiply(PsiLU.Solve(y - one * mu));
            double f = mu + summand[0];

            return f;
        }

        public override Regression Clone()
        {
            KrigingRegression copy = new KrigingRegression(this.TrainData);

            copy.Theta = this.Theta.Clone();
            copy.Psi = this.Psi.Clone();
            copy.mu = this.mu;
            copy.sigsqr = this.sigsqr;
            copy.PsiLU = this.PsiLU;

            return copy;
        }
    }

    public class MLE : IFunction
    {
        public MLE(List<Observation> lobs, Vector<double> power)
        {
            LObs = lobs;
            p = power;
            X = Regression.GetSampleMatrix(LObs);
            y = Regression.GetObsVector(LObs);
        }
        private List<Observation> LObs;
        private Vector<double> p;
        private Matrix<double> X;
        private Vector<double> y;
        //private Matrix<double> Psi;
        private Vector<double> one;
        private int n;

        public double GetOutput(List<double> inputs)
        {
            this.n = LObs.Count;
            this.one = new DenseVector(n, 1);
            Matrix<double> Psi = this.GetPsi(inputs);

            double NegLnLikelihood = 0;

            if (Psi.Determinant() <= 0) // i.e. if the matrix is not positive definite
            {
                NegLnLikelihood = Math.Pow(10, 4) ;
            }

            else
            {
                Tuple<double[], DenseLU> t = GetParams(Psi, one, n);
                DenseLU lu = t.Item2;
                Vector<double> diag = lu.U.Diagonal();
                Vector<double> logAbs = new DenseVector(diag.Count);
                for (int i = 0; i < diag.Count; i++)
                {
                    logAbs[i] = Math.Log(Math.Abs(diag[i]));
                }
                double sum = logAbs.Sum();
                //double test = 2 * sum;
                double test = sum;
                
                //double _LnDetPsi = chol.DeterminantLn;
                double LnDetPsi = test;
                double[] Params = t.Item1;
                double sigsqr = Params[1];

                NegLnLikelihood = -1 * (-(n / 2) * Math.Log(sigsqr) - 0.5 * LnDetPsi);
            }

            return NegLnLikelihood;
        }

        public Tuple<DenseLU, Matrix<double>, double, double> GetLUFactor(List<double> inputs)
        {
            Matrix<double> psi = GetPsi(inputs);
            Tuple<double[], DenseLU> t = GetParams(psi, one, n);
            return new Tuple<DenseLU, Matrix<double>, double, double>(t.Item2, psi, t.Item1[0], t.Item1[1]);
        }

        private Matrix<double> GetPsi(List<double> inputs)
        {
            Vector<double> parameters = new DenseVector(inputs.Count);
            for (int i = 0; i < inputs.Count; i++)
            {
                parameters[i] = inputs[i];
            }

            Vector<double> ten = new DenseVector(parameters.Count, 10);
            Vector<double> theta = MathUtility.PointwisePower(ten, parameters);
            
            int n = LObs.Count;
            Vector<double> one = new DenseVector(n, 1);

            Matrix<double> Psi = new DenseMatrix(n, n, 0);
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    Vector<double> v = X.Row(i) - (X.Row(j));
                    v = MathUtility.PointwisePower(v, p);
                    v = v.PointwiseMultiply(theta);
                    double sum = v.Sum();
                    Psi[i, j] = Math.Exp(-1 * sum);
                }
            }
            Matrix<double> Psit = Psi.Transpose();
            Matrix<double> eye = new DiagonalMatrix(n, n, 1);
            Matrix<double> noise = eye.Multiply(Math.Pow(2, -52));
            Psi = Psi.Add(Psit).Add(eye).Add(noise);

            return Psi;
        }

        private Tuple<double[], DenseLU> GetParams(Matrix<double> Psi, Vector<double> one, int n)
        {
            DenseLU dlu = new DenseLU((DenseMatrix)Psi);
            Vector<double> top = one.ToRowMatrix().Multiply(dlu.Solve(y));
            Vector<double> bottom = one.ToRowMatrix().Multiply(dlu.Solve(one));

            double mu = top[0] / bottom[0];

            Vector<double> sigsqr_v = ((y - one * mu).ToRowMatrix().Multiply(dlu.Solve((y - one * mu)))).Divide(n);
            double sigsqr = sigsqr_v[0];

            return new Tuple<double[], DenseLU>(new double[] { mu, sigsqr }, dlu);
        }
    }
}
