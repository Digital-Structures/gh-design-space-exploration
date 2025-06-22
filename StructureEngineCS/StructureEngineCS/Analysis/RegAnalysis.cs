// Decompiled with JetBrains decompiler
// Type: StructureEngine.Analysis.RegAnalysis
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.MachineLearning;
using StructureEngine.Model;

#nullable disable
namespace StructureEngine.Analysis
{
  public class RegAnalysis : IAnalysis
  {
    private Regression Reg;

    public RegAnalysis(Regression r) => this.Reg = r;

    public double Analyze(IDesign comp) => this.Reg.Predict(new Observation(comp));
  }
}
