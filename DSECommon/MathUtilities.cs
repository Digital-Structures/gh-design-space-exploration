using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace DSECommon
{
    public static class MathUtilities
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
}
