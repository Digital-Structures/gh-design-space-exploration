// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.Material
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B2AD7AF7-B053-4B89-AC37-223D1F22213F
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\Testing\bin\Debug\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Model
{
  public class Material
  {
    public string Name;

    public double E { get; set; }

    public double Poisson { get; set; }

    public double Density { get; set; }

    public double StressAllow { get; set; }

    public Material(double e, double p, double d, double s, string n)
    {
      this.E = e;
      this.Poisson = p;
      this.Density = d;
      this.StressAllow = s;
      this.Name = n;
    }

    public Material MaterialClone()
    {
      return new Material(this.E, this.Poisson, this.Density, this.StressAllow, this.Name);
    }
  }
}
