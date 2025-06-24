// Decompiled with JetBrains decompiler
// Type: StructureEngine.Analysis.BraninTestFunction
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System;

#nullable disable
namespace StructureEngine.Analysis
{
  public class BraninTestFunction : IAnalysis
  {
    public double Analyze(IDesign d)
    {
      if (d is BraninObject braninObject)
        return this.ComputeBranin(braninObject.DesignVariables[0].Value, braninObject.DesignVariables[1].Value);
      throw new Exception("Can only analyze a Branin Object");
    }

    private double ComputeBranin(double x1, double x2)
    {
      return Math.Pow(x2 - 5.1 / (4.0 * Math.Pow(Math.PI, 2.0)) * x2 + 5.0 / Math.PI * x1 - 6.0, 2.0) + 10.0 * (0.96021126422702618 * Math.Cos(x1) + 1.0) + 5.0 * x1;
    }
  }
}
