// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.RegressionReport
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.MachineLearning.Testing;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public class RegressionReport
  {
    public RegressionReport(RegCase c) => this.Properties = c;

    public Regression Model { get; set; }

    public RegressionTest TestResults { get; set; }

    public double Milliseconds { get; set; }

    public RegCase Properties { get; set; }

    public RegressionReport Clone()
    {
      RegressionReport regressionReport = new RegressionReport(this.Properties.Clone());
      if (this.Model != null)
        regressionReport.Model = this.Model.Clone();
      if (this.TestResults != null)
        regressionReport.TestResults = this.TestResults.Clone();
      regressionReport.Milliseconds = this.Milliseconds;
      return regressionReport;
    }
  }
}
