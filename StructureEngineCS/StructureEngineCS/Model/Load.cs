// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.Load
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

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
