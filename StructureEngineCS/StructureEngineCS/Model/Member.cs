// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.Member
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

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

    public bool Envelope { get; set; }

    public Node NodeI { get; set; }

    public Node NodeJ { get; set; }

    public ISection SectionType { get; set; }

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

    public double Area { get; set; }

    public Material Material { get; set; }

    public Member MemberClone() => this.CloneImpl();

    protected virtual Member CloneImpl()
    {
      Member m = new Member(this.NodeI, this.NodeJ);
      this.CopyTo(m);
      return m;
    }

    internal virtual void CopyTo(Member m)
    {
      m.Envelope = this.Envelope;
      m.Area = this.Area;
    }
  }
}
