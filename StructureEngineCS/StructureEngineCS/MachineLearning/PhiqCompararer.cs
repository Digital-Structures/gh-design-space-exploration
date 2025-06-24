// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.PhiqCompararer
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public class PhiqCompararer : IComparer<Tuple<Matrix<double>, double>>
  {
    public int Compare(Tuple<Matrix<double>, double> x, Tuple<Matrix<double>, double> y)
    {
      return x.Item2.CompareTo(y.Item2);
    }
  }
}
