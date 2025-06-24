// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.Member
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B2AD7AF7-B053-4B89-AC37-223D1F22213F
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\Testing\bin\Debug\StructureEngineCS.dll

using System;

#nullable disable
namespace StructureEngine.Model
{
  public class Member
  {
    public Member(Node i, Node j)
    {
      this.NodeI = i;
      this.NodeJ = j;
    }

    public Node NodeI { get; set; }

    public Node NodeJ { get; set; }

    public ISection SectionType { get; set; }

    public bool IsTruss { get; set; }

    public double Length
    {
      get
      {
        return Math.Sqrt(Math.Pow(this.NodeI.DOFs[0].Coord - this.NodeJ.DOFs[0].Coord, 2.0) + Math.Pow(this.NodeI.DOFs[1].Coord - this.NodeJ.DOFs[1].Coord, 2.0));
      }
    }

    public double Angle
    {
      get
      {
        double num1 = this.NodeJ.DOFs[0].Coord - this.NodeI.DOFs[0].Coord;
        double num2 = Math.Atan((this.NodeJ.DOFs[1].Coord - this.NodeI.DOFs[1].Coord) / num1);
        return num1 >= 0.0 ? num2 : Math.PI + num2;
      }
    }

    public double CosX
    {
      get
      {
        double num = this.NodeJ.DOFs[0].Coord - this.NodeI.DOFs[0].Coord;
        double length = this.Length;
        return num / this.Length;
      }
    }

    public double CosY
    {
      get
      {
        double num = this.NodeJ.DOFs[1].Coord - this.NodeI.DOFs[1].Coord;
        double length = this.Length;
        return num / this.Length;
      }
    }

    public double CosZ
    {
      get
      {
        double num = this.NodeJ.DOFs[2].Coord - this.NodeI.DOFs[2].Coord;
        double length = this.Length;
        return num / this.Length;
      }
    }

    public double Area { get; set; }

    public double MomentofInertiaX { get; set; }

    public double MomentofInertiaY { get; set; }

    public double TorsionalStiffnessJ { get; set; }

    public Material Material { get; set; }

    public Member MemberClone() => this.CloneImpl();

    protected virtual Member CloneImpl()
    {
      Member m = new Member(this.NodeI, this.NodeJ);
      this.CopyTo(m);
      return m;
    }

    internal virtual void CopyTo(Member m) => m.Area = this.Area;
  }
}
