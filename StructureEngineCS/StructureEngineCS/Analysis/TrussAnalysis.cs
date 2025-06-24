// Decompiled with JetBrains decompiler
// Type: StructureEngine.Analysis.TrussAnalysis
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Analysis
{
  public class TrussAnalysis : BaseTrussAnalysis, IAnalysis
  {
    public double Analyze(IDesign d)
    {
      ComputedStructure comp = (ComputedStructure) d;
      try
      {
        if (!comp.Analyzed)
        {
          this.RunAnalysis(comp);
          comp.Analyzed = true;
        }
        List<double> source = new List<double>();
        foreach (ComputedMember computedMember in comp.ComputedMembers)
          source.Add(computedMember.Weight);
        return source.Sum();
      }
      catch
      {
        return double.NaN;
      }
    }
  }
}
