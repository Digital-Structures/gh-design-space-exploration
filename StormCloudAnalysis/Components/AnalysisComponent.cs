// Decompiled with JetBrains decompiler
// Type: StormCloudAnalysis.Components.AnalysisComponent
// Assembly: StormCloudAnalysis, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0D28C9E-619E-488E-B920-E321934C44B1
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloudAnalysis.dll

using Grasshopper.Kernel;
using StormCloudAnalysis.Parameters;
using StructureEngine.Analysis;
using StructureEngine.Model;
using System;
using System.Drawing;

#nullable disable
namespace StormCloudAnalysis.Components
{
  public class AnalysisComponent : GH_Component
  {
    public AnalysisComponent()
      : base("Analysis", "A", "Analyzes and Sizes the Structure", "StormCloud", "Analysis")
    {
    }

    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddParameter((IGH_Param) new StructureParameter());
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddParameter((IGH_Param) new StructureParameter());
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      ComputedStructure target = new ComputedStructure();
      StormCloudAnalysis.Types.StructureType destination = new StormCloudAnalysis.Types.StructureType();
      if (!DA.GetData<StormCloudAnalysis.Types.StructureType>(0, ref destination))
        return;
      destination.CastTo<ComputedStructure>(ref target);
      ComputedStructure computedStructure = destination.Value;
      new FrameAnalysis().RunAnalysis(computedStructure);
      StormCloudAnalysis.Types.StructureType data = new StormCloudAnalysis.Types.StructureType(computedStructure);
      DA.SetData(0, (object) data);
    }

    protected override Bitmap Icon => (Bitmap) null;

    public override Guid ComponentGuid => new Guid("{2a305dd4-ff74-4156-8e77-675148e77e04}");
  }
}
