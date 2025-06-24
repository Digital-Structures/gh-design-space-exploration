// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.DoubleIntComparer
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public class DoubleIntComparer : IComparer<Tuple<double, int>>
  {
    public int Compare(Tuple<double, int> x, Tuple<double, int> y) => x.Item1.CompareTo(y.Item1);
  }
}
