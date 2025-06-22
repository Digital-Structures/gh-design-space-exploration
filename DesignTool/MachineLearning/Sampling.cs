using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using StructureEngine.Model;

namespace StructureEngine.MachineLearning
{
    public enum SampleType
    {
        RandomUniform,
        WeightedRandomUniform,
        RandomLatinHypercube,
        WeightedLatinHypercube,
        Adaptive
    }

    public class Sampling
    {
        public delegate bool IsCancel();
        public IsCancel Cancel;

        public delegate void OnSample(int num, int total);
        public OnSample Sample;

        public Sampling(IDesign typ, ISetDistribution dist)
        {
            this.Typ = typ;
            this.Dist = dist;
            this.PopSize = 1;
            this.NumGen = 1;
            dist.RandomSource = Utilities.MyRandom;
        }

        private IDesign Typ;
        private ISetDistribution Dist;
        private int PopSize;
        private int NumGen;

        public List<Observation> GetSample(int n, SampleType type, int nUnderOne, int nUnderOnePointFive)
        {
            List<Observation> samples;
            switch (type)
            {
                case SampleType.RandomUniform:
                    samples = RandUniform(n);
                    break;
                case SampleType.WeightedRandomUniform:
                    samples = WeightedRandUniform(n, nUnderOne, nUnderOnePointFive);
                    break;
                case SampleType.RandomLatinHypercube:
                    //samples = BestLatinHypercube(n, PopSize, NumGen);
                    samples = LatinHypercube(n, Typ.DesignVariables.Count);
                    break;
                case SampleType.WeightedLatinHypercube:
                    samples = WeightedLatinHypercube(n, nUnderOne, nUnderOnePointFive);
                    break;
                case SampleType.Adaptive:
                    samples = Adaptive(n, 4);
                    break;
                default:
                    samples = RandUniform(n);
                    break;
            }
            //if (samples != null)
            //{
            //    samples.AddRange(GetCornerPoints());
            //}
            return samples;
        }

        private List<Observation> GetCornerPoints()
        {
            List<Observation> cornerpoints = new List<Observation>();
            List<IDesign> corners = Typ.GetCornerDesigns();
            foreach (IDesign d in corners)
            {
                Observation o = new Observation(d);
                double score = o.Output;
                cornerpoints.Add(o);
            }
            return cornerpoints;
        }

        private List<Observation> RandUniform(int n)
        {
            List<KeyValuePair<double, Observation>> obsList = new List<KeyValuePair<double, Observation>>();
            while (obsList.Count < n)
            {
                // cancel
                if (Cancel != null && Cancel()) return null;
                // progress
                if (Sample != null) Sample(obsList.Count, n);

                IDesign s = Typ.Mutate(Dist, 1);
                Observation obs = new Observation(s);
                double o = obs.Output;
                obsList.Add(new KeyValuePair<double, Observation>(obs.Output, obs));
            }
            List<Observation> simList = new List<Observation>();
            for (int i = 0; i < obsList.Count; i++)
            {
                simList.Add(obsList[i].Value);
            }
            return simList;
        }

        private List<Observation> WeightedRandUniform(int n, int nUnderOne, int nUnderOnePointFive)
        {
            List<Observation> simList = new List<Observation>();
            int count = 0;
            while (count < n)
            {
                // cancel
                if (Cancel != null && Cancel()) return null;
                // progress
                if (Sample != null) Sample(simList.Count, n);

                IDesign s = Typ.Mutate(Dist, 1);
                Observation obs = new Observation(s);
                double o = obs.Output;

                simList.Add(obs);
                count++;
                if (obs.Output <= 1.5)
                {
                    for (int j = 0; j < nUnderOnePointFive; j++)
                    {
                        simList.Add(obs);
                    }
                }
                if (obs.Output <= 1)
                {
                    for (int j = 0; j < nUnderOne; j++)
                    {
                        simList.Add(obs);
                    }
                }
            }
            return simList;
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

            // add small amount of "noise"
            for (int i = 0; i < X.RowCount; i++)
            {
                for (int j = 0; j < X.ColumnCount; j++)
                {
                    X[i, j] += this.Dist.RandomSource.NextDouble() * Math.Pow(10, -4);
                }
            }

            return X;
        }

        private List<Observation> LatinHypercube(int n, int k)
        {
            Matrix<double> X = RandLatinHypercube(n, k);
            return this.ProjectPoints(X);
        }

        private List<Observation> WeightedLatinHypercube(int n, int nUnderOne, int nUnderOnePointFive)
        {
            List<Observation> unweighted = this.BestLatinHypercube(n, PopSize, NumGen);
            if (unweighted == null) { return null; } // handle cancel event
            unweighted.Sort(new ObsCompare());
            List<Observation> weighted = new List<Observation>();
            int i = 0;
            int count = 0;
            while (count < n)
            {
                if (i < unweighted.Count)
                {
                    double score = unweighted[i].Output;
                    weighted.Add(unweighted[i]);
                    count++;
                    if (score <= 1.5)
                    {
                        for (int j = 0; j < nUnderOnePointFive; j++)
                        {
                            weighted.Add(unweighted[i]);
                        }
                    }
                    if (score <= 1.0)
                    {
                        for (int j = 0; j < nUnderOne; j++)
                        {
                            weighted.Add(unweighted[i]);
                        }
                    }
                }
                else
                {
                    List<Observation> unweighted2 = this.BestLatinHypercube(n, PopSize, NumGen);
                    if (unweighted2 == null) { return null; } // handle cancel event
                    unweighted.AddRange(unweighted2);
                }
                i++;
            }

            return weighted;
        }

        private List<Observation> Adaptive(int n, int iters)
        {
            int num = n / iters;
            double mutdelta = 1.0 / (iters);
            double mutrate = 1;
            List<Observation> best = new List<Observation>();
            List<Observation> finals = new List<Observation>();
            best.Add(new Observation(Typ));
            for (int i = 0; i < iters; i++)
            {
                List<Observation> seeds = best.ToList<Observation>();
                best.Clear();
                for (int j = 0; j < num; j++)
                {
                    // cancel
                    if (Cancel != null && Cancel()) return null;
                    // progress
                    if (Sample != null) Sample(finals.Count, n);

                    IDesign seed = seeds[Dist.RandomSource.Next(seeds.Count)].obsDesign.DesignClone();
                    IDesign s = seed.Mutate(Dist, mutrate);
                    Observation obs = new Observation(s);
                    double o = obs.Output;
                    best.Add(obs);
                    if (o <= 1.5)
                    {
                        best.Add(obs);
                    }
                    if (o <= 1)
                    {
                        best.Add(obs);
                    }
                }
                mutrate = mutrate - mutdelta;
                finals.AddRange(best);
            }
            return finals;
        }

        private Vector<double> Shuffle(Vector<double> v)
        {
            Random r = Dist.RandomSource;
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

        private List<Observation> BestLatinHypercube(int n, int p, int g)
        {
            int k = Typ.DesignVariables.Count;
            int[] q = new int[] { 5, 10, 50, 500 };

            Matrix<double> XStart = RandLatinHypercube(n, k);
            List<Matrix<double>> candidates = new List<Matrix<double>>();

            foreach (int possq in q)
            {
                candidates.Add(GetBestPhiq(XStart, p, g, possq));
            }

            candidates.Sort(new MMComparer());
            Matrix<double> X = candidates[0];

            return this.ProjectPoints(X);

        }

        private List<Observation> ProjectPoints(Matrix<double> X)
        {
            int n = X.RowCount;
            int k = X.ColumnCount;

            // project onto design space hypercube
            List<Observation> lobs = new List<Observation>();
            for (int i = 0; i < n; i++)
            {
                // cancel
                if (Cancel != null && Cancel()) return null;
                // progress
                if (Sample != null) Sample(lobs.Count, n);

                double[] points = new double[k];
                for (int j = 0; j < k; j++)
                {
                    points[j] = X[i, j];
                }
                IDesign d = Typ.GenerateFromVars(points);
                Observation o = new Observation(d);
                double score = o.Output;
                lobs.Add(o);
            }

            return lobs;
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
                while(gen.Count < p)
                {
                    int numPert = Dist.RandomSource.Next((int)Math.Floor(best.RowCount / 2.0));
                    gen.Add(this.Perturb(best, numPert, Dist.RandomSource));
                }
                gen = this.SortPhiq(gen, q);
                best = gen[0];
            }
            return best;
        }

        private double GetPhiq(Matrix<double> X, double q)
        {
            Tuple<List<double>, List<int>> t = MathUtility.GetDistances(X);
            Vector<double> d = MathUtility.ListToVector(t.Item1);
            Vector<double> J = MathUtility.ListToVector(t.Item2);

            double Phiq = Math.Pow(J.PointwiseMultiply(MathUtility.PointwisePower(d, (new DenseVector(d.Count, -q)))).Sum(), 1/q);
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

    }

    public class DoubleIntComparer : IComparer<Tuple<double, int>>
    {
        public int Compare(Tuple<double, int> x, Tuple<double, int> y)
        {
            return x.Item1.CompareTo(y.Item1);
        }
    }

    public class PhiqCompararer : IComparer<Tuple<Matrix<double>, double>>
    {
        public int Compare(Tuple<Matrix<double>, double> x, Tuple<Matrix<double>, double> y)
        {
            return x.Item2.CompareTo(y.Item2);
        }
    }

    public class MMComparer : IComparer<Matrix<double>>
    {
        public int Compare(Matrix<double> x, Matrix<double> y)
        {
            if (this.SortRows(x).Equals(this.SortRows(y)))
            {
                return 0;
            }
            else
            {
                Tuple<List<double>, List<int>> t1 = MathUtility.GetDistances(x);
                Tuple<List<double>, List<int>> t2 = MathUtility.GetDistances(y);
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
