using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using DSECommon;

namespace Sampler
{
    public class SamplerUtilities
    {
        public SamplerUtilities(SamplerComponent comp)
        {
            MyComponent = comp;
        }

        private SamplerComponent MyComponent;

        // This is the main method that produces the sample data based on the input scheme.
        public void Sample()
        {
            switch (MyComponent.Scheme)
            {
                case 0:
                    MyComponent.Output = this.RandomUniform();
                    return;
                case 1:
                    MyComponent.Output = this.Grid();
                    return;
                case 2:
                    MyComponent.Output = this.LHC();
                    return;
                default:
                    return;
            }
        }

        private List<List<double>> RandomUniform()
        {
            List<List<double>> samplesList = new List<List<double>>();

            Random r = MyComponent.MyRand;

            for (int i = 0; i < MyComponent.NSamples; i++)
            {
                List<double> thisSample = new List<double>();
                foreach (var v in MyComponent.VarsList)
                {
                    thisSample.Add(r.NextDouble());
                }
                samplesList.Add(thisSample);
            }
            return Scale(samplesList);
        }

        private List<List<double>> Grid()
        {
            double nBinsOpt = Math.Pow(MyComponent.NSamples, (1 / (double)MyComponent.VarsList.Count));
            int nBins = (int)Math.Ceiling(nBinsOpt);
            int nSamplesFinal = (int)Math.Pow(nBins, MyComponent.VarsList.Count);

            List<double> binValues = new List<double>();
            double bin = (1 / (double)(nBins))/(double)2;

            for (int i = 0; i < nBins; i++)
            {
                binValues.Add(bin);
                bin = bin + 1 / (double)(nBins);
            }

            List<List<double>> samplesList = this.PermsWithRep(binValues, MyComponent.VarsList.Count);

            return Scale(samplesList);
        }

        private List<List<double>> PermsWithRep(List<double> values, int n)
        {
            double[] arr = new double[n];
            IEnumerable <double[]> perms = PermsWithRepHelper(values, arr, n, 0);
            List<List<double>> list = new List<List<double>>();
            foreach (double[] array in perms)
            {
                List<double> l = array.ToList<double>();
                list.Add(l);
            }
            return list;
        }

        private IEnumerable<double[]> PermsWithRepHelper(List<double> values, double[] arr, int n, int i)
        {
            foreach (double val in values)
            {
                arr[i] = val;
                if (i + 1 == n)
                {
                    yield return arr;
                }
                else
                {
                    foreach (var r in PermsWithRepHelper(values, arr, n, i + 1))
                    {
                        yield return r;
                    }
                }
            }
        }

        private List<List<double>> LHC()
        {
            Matrix<double> samplesMatrix = BestLatinHypercube(MyComponent.NSamples);

            List<List<double>> samplesList = new List<List<double>>();
            for (int i = 0; i < samplesMatrix.RowCount; i ++)
            {
                List<double> l = samplesMatrix.Row(i).ToList<double>();
                samplesList.Add(l);
            }

            return Scale(samplesList);
        }

        private Matrix<double> BestLatinHypercube(int n)
        {
            int k = MyComponent.VarsList.Count;
            int[] q = new int[] { 5, 10, 50, 500 };

            Matrix<double> XStart = RandLatinHypercube(n, k);
            List<Matrix<double>> candidates = new List<Matrix<double>>();

            foreach (int possq in q)
            {
                candidates.Add(GetBestPhiq(XStart, 1, 1, possq));
            }

            candidates.Sort(new MMComparer());
            return candidates[0];
        }

        private Matrix<double> RandLatinHypercube(int n, int k)
        {
            // generate "sampling plan"
            Matrix<double> X = new DenseMatrix(n, k);
            for (int i = 0; i < k; i++)
            {
                Vector<double> v = new DenseVector(n);
                for (int j = 0; j < n; j++)
                {
                    v[j] = j;
                }
                v = this.Shuffle(v);
                X.SetColumn(i, v);
            }
            X = X.Add(new DenseMatrix(n, k, 0.5));
            X = X.PointwiseDivide(new DenseMatrix(n, k, n));

            //// add small amount of "noise"
            //for (int i = 0; i < X.RowCount; i++)
            //{
            //    for (int j = 0; j < X.ColumnCount; j++)
            //    {
            //        X[i, j] += MyComponent.MyRand.NextDouble() * Math.Pow(10, -4);
            //    }
            //}

            return X;
        }

        private Vector<double> Shuffle(Vector<double> v)
        {
            Random r = MyComponent.MyRand;
            int n = v.Count;
            while (n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                double value = v[k];
                v[k] = v[n];
                v[n] = value;
            }
            return v;
        }

        private Matrix<double> Perturb(Matrix<double> X_old, int pertNum, Random rand)
        {
            Matrix<double> X = X_old.Clone();
            int n = X.RowCount;
            int k = X.ColumnCount;

            for (int i = 0; i < pertNum; i++)
            {
                int col = (int)Math.Floor(rand.NextDouble() * k);
                int el1 = 1;
                int el2 = 1;
                while (el1 == el2)
                {
                    el1 = (int)Math.Floor(rand.NextDouble() * n);
                    el2 = (int)Math.Floor(rand.NextDouble() * n);
                }

                double temp = X[el1, col];
                X[el1, col] = X[el2, col];
                X[el2, col] = temp;
            }

            return X;
        }

        private Matrix<double> GetBestPhiq(Matrix<double> XStart, int p, int g, double q)
        {
            Matrix<double> best = XStart;
            for (int i = 0; i < g; i++)
            {
                List<Matrix<double>> gen = new List<Matrix<double>>();
                gen.Add(best);
                while (gen.Count < p)
                {
                    int numPert = MyComponent.MyRand.Next((int)Math.Floor(best.RowCount / 2.0));
                    gen.Add(this.Perturb(best, numPert, MyComponent.MyRand));
                }
                gen = this.SortPhiq(gen, q);
                best = gen[0];
            }
            return best;
        }

        private double GetPhiq(Matrix<double> X, double q)
        {
            Tuple<List<double>, List<int>> t = MathUtilities.GetDistances(X);
            Vector<double> d = MathUtilities.ListToVector(t.Item1);
            Vector<double> J = MathUtilities.ListToVector(t.Item2);

            double Phiq = Math.Pow(J.PointwiseMultiply(MathUtilities.PointwisePower(d, (new DenseVector(d.Count, -q)))).Sum(), 1 / q);
            return Phiq;
        }

        private List<Matrix<double>> SortPhiq(List<Matrix<double>> XList, double q)
        {
            List<Tuple<Matrix<double>, double>> sort = new List<Tuple<Matrix<double>, double>>();
            for (int i = 0; i < XList.Count; i++)
            {
                sort.Add(new Tuple<Matrix<double>, double>(XList[i], this.GetPhiq(XList[i], q)));
            }
            sort.Sort(new PhiqCompararer());
            List<Matrix<double>> sorted = new List<Matrix<double>>();
            foreach (Tuple<Matrix<double>, double> t in sort)
            {
                sorted.Add(t.Item1);
            }
            return sorted;
        }

        private List<int> Perms(int n)
        {
            List<int> indsAscend = new List<int>();
            for (int i = 1; i < n + 1; i++)
            {
                indsAscend.Add(i);
            }

            int count = 1;
            List<int> inds = new List<int>();

            while (count <= n)
            {
                int ind = MyComponent.MyRand.Next(1, n + 1);
                if (!inds.Contains(ind))
                {
                    inds.Add(ind);
                    count += 1;
                }
            }
            return inds;
        }

        private List<List<double>> Scale(List<List<double>> samples)
        {
            List<double> rmin = new List<double>();
            List<double> rmax = new List<double>();

            foreach (DSEVariable myVar in MyComponent.VarsList)
            {
                rmin.Add(myVar.LowerBound);
                rmax.Add(myVar.UpperBound);
            }

            var scaledSamples = new List<List<double>>();

            foreach (List<double> sample in samples)
            {
                List<double> scaledVars = new List<double>();
                
                for (int j = 0; j < MyComponent.VarsList.Count; j++)
                {
                    scaledVars.Add((sample[j]) * Math.Abs(rmax[j] - rmin[j]) + rmin[j]);
                }
                scaledSamples.Add(scaledVars);
            }

            return scaledSamples;
        }

        public void WriteOutputToFile(List<List<double>> output)
        {
            string a = null;
            for (int i = 0; i < output.Count; i++)
            {
                string b = null;
                for (int j = 0; j < output[i].Count; j++)
                {
                    b = b + output[i][j] + " ";
                }
                a = a + b + "\r\n";
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter(@"" + MyComponent.Path + MyComponent.Prefix + ".csv");
            file.Write(a);
            file.Close();
        }

        
    }

    class DoubleIntComparer : IComparer<Tuple<double, int>>
    {
        public int Compare(Tuple<double, int> x, Tuple<double, int> y)
        {
            return x.Item1.CompareTo(y.Item1);
        }
    }

    class PhiqCompararer : IComparer<Tuple<Matrix<double>, double>>
    {
        public int Compare(Tuple<Matrix<double>, double> x, Tuple<Matrix<double>, double> y)
        {
            return x.Item2.CompareTo(y.Item2);
        }
    }

    class MMComparer : IComparer<Matrix<double>>
    {
        public int Compare(Matrix<double> x, Matrix<double> y)
        {
            if (this.SortRows(x).Equals(this.SortRows(y)))
            {
                return 0;
            }
            else
            {
                Tuple<List<double>, List<int>> t1 = MathUtilities.GetDistances(x);
                Tuple<List<double>, List<int>> t2 = MathUtilities.GetDistances(y);
                int m1 = t1.Item1.Count;
                int m2 = t2.Item1.Count;

                List<double> V1 = new List<double>();
                List<double> V2 = new List<double>();

                for (int i = 0; i < m1; i++)
                {
                    V1.Add(t1.Item1[i]);
                    V1.Add(-t1.Item2[i]);
                }

                for (int i = 0; i < m2; i++)
                {
                    V2.Add(t2.Item1[i]);
                    V2.Add(-t2.Item2[i]);
                }

                int m = Math.Min(m1, m2);
                V1 = V1.GetRange(0, m);
                V2 = V2.GetRange(0, m);

                List<int> c = new List<int>();
                for (int i = 0; i < m; i++)
                {
                    int rating = V1[i] > V2[i] ? 1 : V1[i] < V2[i] ? -2 : 0;
                    c.Add(rating);
                }

                if (c.Sum() == 0)
                {
                    return 0;
                }

                else
                {
                    int i = 0;
                    while (c[i] != 0)
                    {
                        i++;
                    }
                    return c[i];
                }
            }
        }

        private Matrix<double> SortRows(Matrix<double> M)
        {
            List<Tuple<double, int>> ind = new List<Tuple<double, int>>();
            for (int i = 0; i < M.RowCount; i++)
            {
                ind.Add(new Tuple<double, int>(M.Column(0)[i], i));
            }
            ind.Sort(new DoubleIntComparer());
            int[] indices = new int[ind.Count];
            int j = 0;
            foreach (Tuple<double, int> t in ind)
            {
                indices[j] = t.Item2;
                j++;
            }
            M.PermuteRows(new Permutation(indices));
            return M;
        }
    }
}
