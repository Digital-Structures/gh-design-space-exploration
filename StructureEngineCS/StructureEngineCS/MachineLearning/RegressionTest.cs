// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.RegressionTest
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public class RegressionTest
  {
    public List<Observation> TestSet { get; set; }

    public ErrorMeasures Error { get; set; }

    public RegressionTest Clone()
    {
      List<Observation> range = this.TestSet.GetRange(0, this.TestSet.Count);
      return new RegressionTest()
      {
        TestSet = range,
        Error = this.Error.Clone()
      };
    }
  }
}
