// Decompiled with JetBrains decompiler
// Type: StormCloudAnalysis.Types.MaterialType
// Assembly: StormCloudAnalysis, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0D28C9E-619E-488E-B920-E321934C44B1
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloudAnalysis.dll

using Grasshopper.Kernel.Types;
using StructureEngine.Model;

#nullable disable
namespace StormCloudAnalysis.Types
{
  public class MaterialType : GH_Goo<Material>
  {
    public MaterialType() => this.Value = new Material(29000.0, 0.3, 7500.0, 50.0, "steel");

    public MaterialType(double e, double d, double s, double p, string n)
    {
      this.Value = new Material(e, d, s, p, n);
    }

    public MaterialType(Material material) => this.Value = material;

    public Material MaterialValue { get; set; }

    public override IGH_Goo Duplicate() => (IGH_Goo) new MaterialType(this.Value);

    public override bool IsValid => true;

    public override string ToString() => "Material";

    public override string TypeName => "Material";

    public override string TypeDescription => "Describes a material";
  }
}
