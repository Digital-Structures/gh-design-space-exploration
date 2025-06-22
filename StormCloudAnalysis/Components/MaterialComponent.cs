// Decompiled with JetBrains decompiler
// Type: StormCloudAnalysis.Components.MaterialComponent
// Assembly: StormCloudAnalysis, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0D28C9E-619E-488E-B920-E321934C44B1
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloudAnalysis.dll

using Grasshopper.Kernel;
using StormCloudAnalysis.Parameters;
using StormCloudAnalysis.Types;
using StructureEngine.Model;
using System;
using System.Drawing;

#nullable disable
namespace StormCloudAnalysis.Components
{
  public class MaterialComponent : GH_Component
  {
    public MaterialComponent()
      : base("Material", "Mat", "Material Properties", "StormCloud", "Analysis")
    {
    }

    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddNumberParameter("Young", "E", "Young's Modulus [ksi]", GH_ParamAccess.item);
      pManager.AddNumberParameter("Density", "d", "Material Density [lb/ft^3]", GH_ParamAccess.item);
      pManager.AddNumberParameter("StressAllow", "s", "Maximum Allowable Stress [ksi]", GH_ParamAccess.item);
      pManager.AddNumberParameter("Poisson", "p", "Poisson Ratio [/]", GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "N", "Material Name", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddParameter((IGH_Param) new MaterialParameter());
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      double destination1 = 0.0;
      double destination2 = 0.0;
      double destination3 = 0.0;
      double destination4 = 0.0;
      string destination5 = (string) null;
      if (!DA.GetData<double>(0, ref destination1) || !DA.GetData<double>(1, ref destination2) || !DA.GetData<double>(2, ref destination3) || !DA.GetData<double>(3, ref destination4) || !DA.GetData<string>(4, ref destination5))
        return;
      MaterialType data = new MaterialType(new Material(destination1, destination4, destination2, destination3, destination5));
      DA.SetData(0, (object) data);
    }

    protected override Bitmap Icon => (Bitmap) null;

    public override Guid ComponentGuid => new Guid("{3b379949-0f68-4693-ac2c-9f5ceeec2373}");
  }
}
