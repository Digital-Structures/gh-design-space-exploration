// Decompiled with JetBrains decompiler
// Type: StormCloudAnalysis.Components.SectionComponent
// Assembly: StormCloudAnalysis, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0D28C9E-619E-488E-B920-E321934C44B1
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloudAnalysis.dll

using Grasshopper.Kernel;
using System;
using System.Drawing;

#nullable disable
namespace StormCloudAnalysis.Components
{
  public class SectionComponent : GH_Component
  {
    public SectionComponent()
      : base("Section", "Sec", "Generates Base Section for Analysis", "StormCloud", "Analysis")
    {
    }

    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
    }

    protected override Bitmap Icon => (Bitmap) null;

    public override Guid ComponentGuid => new Guid("{1e7fb7a7-f65f-4bb5-b853-4f9ef3a6cc7e}");
  }
}
