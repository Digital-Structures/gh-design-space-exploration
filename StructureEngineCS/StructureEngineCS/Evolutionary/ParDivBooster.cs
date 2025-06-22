// Decompiled with JetBrains decompiler
// Type: StructureEngine.Evolutionary.ParDivBooster
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Evolutionary
{
  public class ParDivBooster : IDivBooster
  {
    private List<IDesign> Best;
    private ComputedStructure Candidate;
    private double MutationRate;

    public bool IsDiverse(List<IDesign> existing, IDesign candidate, double rate)
    {
      this.Best = existing;
      this.Candidate = (ComputedStructure) candidate;
      this.MutationRate = rate;
      double diffSize = this.GetDiffSize();
      foreach (ComputedStructure s1 in this.Best)
      {
        if (this.GetDistance(s1, this.Candidate) < diffSize)
          return false;
      }
      return true;
    }

    private double GetDistance(ComputedStructure s1, ComputedStructure s2)
    {
      double d = 0.0;
      for (int index = 0; index < s1.DesignVariables.Count; ++index)
      {
        double num = Math.Pow(s1.DesignVariables[index].Value - s2.DesignVariables[index].Value, 2.0);
        d += num;
      }
      return Math.Sqrt(d);
    }

    private double GetDiffSize() => 0.3 * (this.Candidate.SizeDesignSpace() * this.MutationRate);
  }
}
