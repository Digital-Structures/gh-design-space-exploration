// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.Node
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B2AD7AF7-B053-4B89-AC37-223D1F22213F
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\Testing\bin\Debug\StructureEngineCS.dll

using Rhino.Geometry;
using System;

#nullable disable
namespace StructureEngine.Model
{
  public class Node
  {
    public Node(DOF[] dofs) => this.DOFs = dofs;

    public Node(Point3d point, bool isframe)
    {
      this.DOFs = new DOF[6]
      {
        new DOF(point.X),
        new DOF(point.Y),
        new DOF(point.Z),
        new DOF(0.0, isframe),
        new DOF(0.0, isframe),
        new DOF(0.0, isframe)
      };
    }

    public void SetIndex(int index)
    {
      this.Index = index;
      for (int index1 = 0; index1 < 6; ++index1)
        this.DOFs[index1].Index = index * 6 + index1;
    }

    public Point3d ToRhinoPoint()
    {
      return new Point3d(this.DOFs[0].Coord, this.DOFs[1].Coord, this.DOFs[2].Coord);
    }

    public bool IsAtPoint(Point3d point)
    {
      int num;
      if (this.DOFs[0].Coord.Equals(point.X))
      {
        double coord = this.DOFs[1].Coord;
        if (coord.Equals(point.Y))
        {
          coord = this.DOFs[2].Coord;
          num = coord.Equals(point.Z) ? 1 : 0;
          goto label_4;
        }
      }
      num = 0;
label_4:
      return num != 0;
    }

    public DOF[] DOFs { get; set; }

    public int Index { get; set; }

    public double X => this.DOFs[0].Coord;

    public double Y => this.DOFs[1].Coord;

    public double Z => this.DOFs[2].Coord;

    public double RotX => this.DOFs[3].Coord;

    public double RotY => this.DOFs[4].Coord;

    public double RotZ => this.DOFs[5].Coord;

    public void CopyTo(Node newnode)
    {
      for (int index = 0; index < this.DOFs.Length; ++index)
        this.DOFs[index].CopyTo(newnode.DOFs[index]);
      newnode.Index = this.Index;
    }

    public double DistanceFrom(Node that)
    {
      double d = 0.0;
      for (int index = 0; index < this.DOFs.Length; ++index)
        d += Math.Pow(this.DOFs[index].Coord - that.DOFs[index].Coord, 2.0);
      return Math.Sqrt(d);
    }
  }
}
