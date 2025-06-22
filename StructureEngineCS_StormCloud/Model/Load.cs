// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.Load
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B2AD7AF7-B053-4B89-AC37-223D1F22213F
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\Testing\bin\Debug\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Model
{
  public class Load
  {
    public double Value;
    public LoadCase Case;
    public DOF myDOF;

    public Load(double l, LoadCase lc, DOF d)
    {
      this.Value = l;
      this.Case = lc;
      this.myDOF = d;
    }

    public Load Clone(LoadCase newLC) => new Load(this.Value, newLC, this.myDOF);
  }
}
