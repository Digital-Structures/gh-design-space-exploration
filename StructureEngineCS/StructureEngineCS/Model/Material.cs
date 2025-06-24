// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.Material
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Model
{
  public class Material
  {
    public string Name;

    public double E { get; set; }

    public double Density { get; set; }

    public double StressAllow { get; set; }

    public Material(double e, double d, double s, string n)
    {
      this.E = e;
      this.Density = d;
      this.StressAllow = s;
      this.Name = n;
    }

    public Material MaterialClone()
    {
      return new Material(this.E, this.Density, this.StressAllow, this.Name);
    }
  }
}
