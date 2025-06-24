// Decompiled with JetBrains decompiler
// Type: StormCloudAnalysis.Test.TestConnectivyComponent
// Assembly: StormCloudAnalysis, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0D28C9E-619E-488E-B920-E321934C44B1
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloudAnalysis.dll

using Rhino.Geometry;
using StructureEngine.Model;
using System.Collections.Generic;

#nullable disable
namespace StormCloudAnalysis.Test
{
  public class TestConnectivyComponent
  {
    public static void Main()
    {
      List<Line> lineList = new List<Line>();
      Line line1 = new Line(new Point3d(0.0, 0.0, 0.0), new Point3d(0.0, 1.0, 0.0));
      Line line2 = new Line(new Point3d(0.0, 0.0, 0.0), new Point3d(0.0, 0.0, 1.0));
      lineList.Add(line1);
      lineList.Add(line2);
      List<Point3d> point3dList = new List<Point3d>();
      List<StructureEngine.Model.Node> nodes = new List<StructureEngine.Model.Node>();
      List<Member> members = new List<Member>();
      foreach (Line line3 in lineList)
      {
        point3dList.Add(line3.From);
        point3dList.Add(line3.To);
        int index1 = point3dList.Count - 2;
        int index2 = point3dList.Count - 1;
        for (int index3 = 0; index3 < point3dList.Count - 2; ++index3)
        {
          if (object.Equals((object) point3dList[point3dList.Count - 2], (object) point3dList[index3]))
          {
            point3dList.RemoveAt(point3dList.Count - 2);
            index1 = index3;
          }
        }
        Point3d point3d = point3dList[index1];
        DOF dof1 = new DOF(point3d.X);
        point3d = point3dList[index1];
        DOF dof2 = new DOF(point3d.Y);
        point3d = point3dList[index1];
        DOF dof3 = new DOF(point3d.Z);
        StructureEngine.Model.Node i = new StructureEngine.Model.Node(new DOF[3]
        {
          dof1,
          dof2,
          dof3
        });
        i.Index = index1;
        for (int index4 = 0; index4 < point3dList.Count - 1; ++index4)
        {
          if (object.Equals((object) point3dList[point3dList.Count - 1], (object) point3dList[index4]))
          {
            point3dList.RemoveAt(point3dList.Count - 1);
            index2 = index4;
          }
        }
        point3d = point3dList[index2];
        DOF dof4 = new DOF(point3d.X);
        point3d = point3dList[index2];
        DOF dof5 = new DOF(point3d.Y);
        point3d = point3dList[index2];
        DOF dof6 = new DOF(point3d.Z);
        StructureEngine.Model.Node j = new StructureEngine.Model.Node(new DOF[3]
        {
          dof4,
          dof5,
          dof6
        });
        j.Index = index2;
        Member member = new Member(i, j);
        nodes.Add(i);
        nodes.Add(j);
        members.Add(member);
      }
      StormCloudAnalysis.Types.StructureType structureType = new StormCloudAnalysis.Types.StructureType(new ComputedStructure(new Structure(nodes, members)));
    }
  }
}
