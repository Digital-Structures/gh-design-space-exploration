// Decompiled with JetBrains decompiler
// Type: StormCloudAnalysis.Components.SupportComponent
// Assembly: StormCloudAnalysis, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0D28C9E-619E-488E-B920-E321934C44B1
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloudAnalysis.dll

using Grasshopper.Kernel;
using Rhino.Geometry;
using StormCloudAnalysis.Parameters;
using StructureEngine.Model;
using System;
using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace StormCloudAnalysis.Components
{
  public class SupportComponent : GH_Component
  {
    public SupportComponent()
      : base("Support", "S", "Creates Support", "StormCloud", "Analysis")
    {
    }

    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddParameter((IGH_Param) new StructureParameter());
      pManager.AddPointParameter("Support Point", "Pt", "Support Point", GH_ParamAccess.list);
      pManager.AddBooleanParameter("Translation", "T", "True if translational movement prevented", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Rotation", "R", "True if rotational movement prevented", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddParameter((IGH_Param) new StructureParameter());
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      ComputedStructure target = new ComputedStructure();
      List<Point3d> list = new List<Point3d>();
      bool destination1 = true;
      bool destination2 = true;
      StormCloudAnalysis.Types.StructureType destination3 = new StormCloudAnalysis.Types.StructureType();
      if (!DA.GetData<StormCloudAnalysis.Types.StructureType>(0, ref destination3) || !DA.GetDataList<Point3d>(1, list) || !DA.GetData<bool>(2, ref destination1) || !DA.GetData<bool>(3, ref destination2))
        return;
      destination3.CastTo<ComputedStructure>(ref target);
      ComputedStructure structure = destination3.Value;
      foreach (Point3d point in list)
      {
        foreach (StructureEngine.Model.Node node in structure.Nodes)
        {
          if (node.IsAtPoint(point))
          {
            node.DOFs[0].Fixed = destination1;
            node.DOFs[1].Fixed = destination1;
            node.DOFs[2].Fixed = destination1;
          }
        }
      }
      foreach (Point3d point in list)
      {
        foreach (StructureEngine.Model.Node node in structure.Nodes)
        {
          if (node.IsAtPoint(point))
          {
            node.DOFs[3].Fixed = destination2;
            node.DOFs[4].Fixed = destination2;
            node.DOFs[5].Fixed = destination2;
          }
        }
      }
      StormCloudAnalysis.Types.StructureType data = new StormCloudAnalysis.Types.StructureType(structure);
      DA.SetData(0, (object) data);
    }

    protected override Bitmap Icon => (Bitmap) null;

    public override Guid ComponentGuid => new Guid("{99c200e4-c426-4c25-93ae-614d52acb798}");
  }
}
