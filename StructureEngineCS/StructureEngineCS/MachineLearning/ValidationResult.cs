// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.ValidationResult
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.MachineLearning
{
  public class ValidationResult
  {
    public ValidationResult(ErrorMeasures e, double p, Regression m)
    {
      this.Error = e;
      this.Parameter = p;
      this.Model = m;
    }

    public Regression Model { get; set; }

    public ErrorMeasures Error { get; set; }

    public double Parameter { get; set; }
  }
}
