// Decompiled with JetBrains decompiler
// Type: StormCloudAnalysis.ConnectivityComponent
// Assembly: StormCloudAnalysis, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0D28C9E-619E-488E-B920-E321934C44B1
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloudAnalysis.dll

using Grasshopper.Kernel;
using Rhino.Geometry;
using StormCloudAnalysis.Parameters;
using StormCloudAnalysis.Types;
using StructureEngine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

#nullable disable
namespace StormCloudAnalysis
{
  public class ConnectivityComponent : GH_Component
  {
    private const int STD_OUTPUT_HANDLE = -11;
    private const int MY_CODE_PAGE = 437;

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern int AllocConsole();

    public ConnectivityComponent()
      : base("Assembly", "A", "Assembles the Structure", "StormCloud", "Analysis")
    {
    }

    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddLineParameter("Lines", "L", "Line(s) Representing the Structure", GH_ParamAccess.list);
      pManager.AddParameter((IGH_Param) new MaterialParameter());
      pManager.AddTextParameter("Section", "S", "Section Type for the Structure: Rod or Square", GH_ParamAccess.item);
      pManager.AddBooleanParameter("IsFrame?", "Frale?", "Type of Structure", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddPointParameter("Nodes", "sN", "Model Node Coordinates", GH_ParamAccess.list);
      pManager.AddParameter((IGH_Param) new StructureParameter());
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      ConnectivityComponent.AllocConsole();
      List<Line> list = new List<Line>();
      MaterialType destination1 = new MaterialType();
      string destination2 = "";
      bool destination3 = true;
      if (!DA.GetDataList<Line>(0, list) || !DA.GetData<MaterialType>(1, ref destination1) || !DA.GetData<string>(2, ref destination2) || !DA.GetData<bool>(3, ref destination3))
        return;
      Material material = destination1.Value;
      ISection section = (ISection) new RodSection();
      if (destination2 == "Square")
        section = (ISection) new SquareSection();
      List<Point3d> data1 = new List<Point3d>();
      List<StructureEngine.Model.Node> nodes = new List<StructureEngine.Model.Node>();
      List<Member> members = new List<Member>();
      foreach (Line line in list)
      {
        Point3d start = line.From;
        Point3d end = line.To;
        int index1 = nodes.FindIndex((Predicate<StructureEngine.Model.Node>) (n => n.IsAtPoint(start)));
        int index2 = nodes.FindIndex((Predicate<StructureEngine.Model.Node>) (n => n.IsAtPoint(end)));
        StructureEngine.Model.Node i = new StructureEngine.Model.Node(start, destination3);
        StructureEngine.Model.Node j = new StructureEngine.Model.Node(end, destination3);
        if (index1 == -1 && index2 == -1)
        {
          i.SetIndex(nodes.Count);
          nodes.Add(i);
          j.SetIndex(nodes.Count + 1);
          nodes.Add(j);
        }
        else if (index1 != -1 && index2 == -1)
        {
          i = nodes[index1];
          j.SetIndex(nodes.Count);
          nodes.Add(j);
        }
        else if (index1 == -1 && index2 != -1)
        {
          i.SetIndex(nodes.Count);
          nodes.Add(i);
          j = nodes[index2];
        }
        if (index1 == -1 || index2 == -1)
          members.Add(new Member(i, j)
          {
            Material = material,
            SectionType = section
          });
      }
      StormCloudAnalysis.Types.StructureType data2 = new StormCloudAnalysis.Types.StructureType(new ComputedStructure(new Structure(nodes, members)));
      DA.SetDataList(0, (IEnumerable) data1);
      DA.SetData(1, (object) data2);
    }

    protected override Bitmap Icon => (Bitmap) null;

    public override Guid ComponentGuid => new Guid("{8a621274-9035-4973-988b-2608ac106edf}");
  }
}
