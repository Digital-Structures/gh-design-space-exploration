// Decompiled with JetBrains decompiler
// Type: StormCloudAnalysis.Components.LoadComponent
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
  public class LoadComponent : GH_Component
  {
    public LoadComponent()
      : base("Point Load", "L", "Creates Point Load Applied on Structural Node", "StormCloud", "Analysis")
    {
    }

    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddParameter((IGH_Param) new StructureParameter());
      pManager.AddPointParameter("Point", "Pt", "Loaded Point", GH_ParamAccess.list);
      pManager.AddVectorParameter("Load", "L", "Load Vector", GH_ParamAccess.item);
      pManager.AddTextParameter("Loadcase", "LC", "Loadcase name", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddParameter((IGH_Param) new StructureParameter());
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      ComputedStructure target = new ComputedStructure();
      List<Point3d> point3dList = new List<Point3d>();
      Vector3d vector3d = new Vector3d();
      string n = "";
      StormCloudAnalysis.Types.StructureType destination = new StormCloudAnalysis.Types.StructureType();
      if (!DA.GetData<StormCloudAnalysis.Types.StructureType>(0, ref destination))
        return;
      destination.CastTo<ComputedStructure>(ref target);
      ComputedStructure structure = destination.Value;
      LoadCase lc = new LoadCase(n);
      double x = vector3d.X;
      double y = vector3d.Y;
      double z = vector3d.Z;
      foreach (Point3d point in point3dList)
      {
        foreach (StructureEngine.Model.Node node in structure.Nodes)
        {
          if (node.IsAtPoint(point))
          {
            Load load1 = new Load(x, lc, node.DOFs[0]);
            Load load2 = new Load(y, lc, node.DOFs[1]);
            Load load3 = new Load(z, lc, node.DOFs[2]);
            lc.Loads.Add(load1);
            lc.Loads.Add(load2);
            lc.Loads.Add(load3);
          }
        }
      }
      structure.LoadCases.Add(lc);
      StormCloudAnalysis.Types.StructureType data = new StormCloudAnalysis.Types.StructureType(structure);
      DA.SetData(0, (object) data);
    }

    protected override Bitmap Icon => Resources.Resources.load;

    public override Guid ComponentGuid => new Guid("{a7d699c1-9d6f-4d3e-a80a-df639edb180a}");
  }
}
