// Decompiled with JetBrains decompiler
// Type: StormCloudAnalysis.Parameters.StructureParameter
// Assembly: StormCloudAnalysis, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0D28C9E-619E-488E-B920-E321934C44B1
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloudAnalysis.dll

using Grasshopper.Kernel;
using StormCloudAnalysis.Types;
using System;
using System.Drawing;

#nullable disable
namespace StormCloudAnalysis.Parameters
{
  public class StructureParameter : GH_Param<StructureType>
  {
    public StructureParameter()
      : base("Structure", "Struct", "Represents a structure", "StormCloud", "Analysis", GH_ParamAccess.item)
    {
    }

    protected override Bitmap Icon => base.Icon;

    public override Guid ComponentGuid => new Guid("{E1CD5876-F2E3-4E34-90CA-689D8C92C609}");
  }
}
