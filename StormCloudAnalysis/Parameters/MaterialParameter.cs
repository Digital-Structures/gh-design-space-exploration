// Decompiled with JetBrains decompiler
// Type: StormCloudAnalysis.Parameters.MaterialParameter
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
  public class MaterialParameter : GH_Param<MaterialType>
  {
    public MaterialParameter()
      : base("Material", "Mat", "Represents a material", "StormCloud", "Analysis", GH_ParamAccess.item)
    {
    }

    protected override Bitmap Icon => base.Icon;

    public override Guid ComponentGuid => new Guid("{C9C84ACB-4F91-4DC5-B6FD-D4748176507C}");
  }
}
