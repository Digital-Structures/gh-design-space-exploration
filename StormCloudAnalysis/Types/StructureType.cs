// Decompiled with JetBrains decompiler
// Type: StormCloudAnalysis.Types.StructureType
// Assembly: StormCloudAnalysis, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0D28C9E-619E-488E-B920-E321934C44B1
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloudAnalysis.dll

using Grasshopper.Kernel.Types;
using StructureEngine.Model;

#nullable disable
namespace StormCloudAnalysis.Types
{
  public class StructureType : GH_Goo<ComputedStructure>
  {
    public StructureType() => this.Value = new ComputedStructure();

    public StructureType(ComputedStructure structure) => this.Value = structure;

    public override string TypeName => "Structure";

    public override string TypeDescription => "Describes an assembled structure";

    public override IGH_Goo Duplicate() => (IGH_Goo) new StructureType(this.Value);

    public override bool IsValid => true;

    public override string ToString() => "Structure";
  }
}
