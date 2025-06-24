// Decompiled with JetBrains decompiler
// Type: StormCloudAnalysis.Components.Display
// Assembly: StormCloudAnalysis, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0D28C9E-619E-488E-B920-E321934C44B1
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloudAnalysis.dll

using Grasshopper.Kernel;
using Rhino.Geometry;
using StormCloudAnalysis.Parameters;
using StructureEngine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace StormCloudAnalysis.Components
{
  public class Display : GH_Component
  {
    public Display()
      : base(nameof (Display), "A", "Analyzes and Sizes the Structure", "StormCloud", "Analysis")
    {
    }

    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddParameter((IGH_Param) new StructureParameter());
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddLineParameter("Lines", "L", "Lines", GH_ParamAccess.list);
      pManager.AddPointParameter("Points", "P", "Points", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      ComputedStructure target = new ComputedStructure();
      StormCloudAnalysis.Types.StructureType destination = new StormCloudAnalysis.Types.StructureType();
      if (!DA.GetData<StormCloudAnalysis.Types.StructureType>(0, ref destination))
        return;
      destination.CastTo<ComputedStructure>(ref target);
      ComputedStructure computedStructure = destination.Value;
      List<Point3d> data1 = new List<Point3d>();
      List<Line> data2 = new List<Line>();
      foreach (Member member in computedStructure.Members)
      {
        Point3d rhinoPoint1 = member.NodeI.ToRhinoPoint();
        Point3d rhinoPoint2 = member.NodeJ.ToRhinoPoint();
        data2.Add(new Line(rhinoPoint1, rhinoPoint2));
      }
      foreach (StructureEngine.Model.Node node in computedStructure.Nodes)
      {
        Point3d rhinoPoint = node.ToRhinoPoint();
        data1.Add(rhinoPoint);
      }
      DA.SetDataList(0, (IEnumerable) data2);
      DA.SetDataList(1, (IEnumerable) data1);
    }

    protected override Bitmap Icon => (Bitmap) null;

    public override Guid ComponentGuid => new Guid("{268984e1-3662-470c-8e8b-a339f696ffb0}");
  }
}
