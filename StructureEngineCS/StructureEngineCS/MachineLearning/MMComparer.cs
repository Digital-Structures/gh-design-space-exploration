// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.MMComparer
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public class MMComparer : IComparer<Matrix<double>>
  {
    public int Compare(Matrix<double> x, Matrix<double> y)
    {
      if (this.SortRows(x).Equals(this.SortRows(y)))
        return 0;
      Tuple<List<double>, List<int>> distances1 = MathUtility.GetDistances(x);
      Tuple<List<double>, List<int>> distances2 = MathUtility.GetDistances(y);
      int count1 = distances1.Item1.Count;
      int count2 = distances2.Item1.Count;
      List<double> doubleList1 = new List<double>();
      List<double> doubleList2 = new List<double>();
      for (int index = 0; index < count1; ++index)
      {
        doubleList1.Add(distances1.Item1[index]);
        doubleList1.Add((double) -distances1.Item2[index]);
      }
      for (int index = 0; index < count2; ++index)
      {
        doubleList2.Add(distances2.Item1[index]);
        doubleList2.Add((double) -distances2.Item2[index]);
      }
      int count3 = Math.Min(count1, count2);
      List<double> range1 = doubleList1.GetRange(0, count3);
      List<double> range2 = doubleList2.GetRange(0, count3);
      List<int> source = new List<int>();
      for (int index = 0; index < count3; ++index)
      {
        int num = range1[index] > range2[index] ? 1 : (range1[index] < range2[index] ? -2 : 0);
        source.Add(num);
      }
      if (source.Sum() == 0)
        return 0;
      int index1 = 0;
      while (source[index1] != 0)
        ++index1;
      return source[index1];
    }

    private Matrix<double> SortRows(Matrix<double> M)
    {
      List<Tuple<double, int>> tupleList = new List<Tuple<double, int>>();
      for (int index = 0; index < M.RowCount; ++index)
        tupleList.Add(new Tuple<double, int>(M.Column(0)[index], index));
      tupleList.Sort((IComparer<Tuple<double, int>>) new DoubleIntComparer());
      int[] numArray = new int[tupleList.Count];
      int index1 = 0;
      foreach (Tuple<double, int> tuple in tupleList)
      {
        numArray[index1] = tuple.Item2;
        ++index1;
      }
      M.PermuteRows(new Permutation(numArray));
      return M;
    }
  }
}
