// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.MathUtility
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public static class MathUtility
  {
    public static Vector<double> PointwisePower(Vector<double> b, Vector<double> p)
    {
      if (b.Count != p.Count)
        throw new ArgumentException("Vector lengths must match.");
      int count = b.Count;
      Vector<double> vector = (Vector<double>) CreateVector.Dense<double>(count);
      for (int index = 0; index < count; ++index)
        vector[index] = Math.Pow(b[index], p[index]);
      return vector;
    }

    public static Vector<double> ListToVector(List<double> list)
    {
      Vector<double> vector = (Vector<double>) CreateVector.Dense<double>(list.Count);
      for (int index = 0; index < list.Count; ++index)
        vector[index] = list[index];
      return vector;
    }

    public static Vector<double> ListToVector(List<int> list)
    {
      Vector<double> vector = (Vector<double>) CreateVector.Dense<double>(list.Count);
      for (int index = 0; index < list.Count; ++index)
        vector[index] = (double) list[index];
      return vector;
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
