// Decompiled with JetBrains decompiler
// Type: StructureEngine.Analysis.TrussDispAnalysis
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Analysis
{
  public class TrussDispAnalysis : BaseTrussAnalysis, IAnalysis
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
        foreach (DOF doF in comp.DOFs)
        {
          double num = doF.Disp.Values.Select<double, double>((Func<double, double>) (x => Math.Abs(x))).Max();
          source.Add(num);
        }
        return source.Max();
      }
      catch
      {
        return double.NaN;
      }
    }
  }
}
