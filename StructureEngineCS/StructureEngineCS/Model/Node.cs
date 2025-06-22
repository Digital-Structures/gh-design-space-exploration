

using System;

#nullable disable
namespace StructureEngine.Model
{
  public class Node
  {
    public Node(DOF[] dofs) => this.DOFs = dofs;

    public DOF[] DOFs { get; set; }

    public int Index { get; set; }

    public double X => this.DOFs[0].Coord;

    public double Y => this.DOFs[1].Coord;

    public void CopyTo(Node newnode)
    {
      for (int index = 0; index < this.DOFs.Length; ++index)
        this.DOFs[index].CopyTo(newnode.DOFs[index]);
      newnode.Index = this.Index;
    }

    public void EnforceRelationships(Structure s)
    {
      for (int index = 0; index < this.DOFs.Length; ++index)
      {
        foreach (ParametricRelation relation in this.DOFs[index].Relations)
        {
          if (relation.Relation == RelationType.Master)
          {
            foreach (Node listener in relation.Listeners)
            {
              if (index == 0)
                listener.DOFs[0].Coord = this.DOFs[0].Coord;
              else
                listener.DOFs[1].Coord = this.DOFs[1].Coord;
            }
          }
          else if (relation.Relation == RelationType.Mirror)
          {
            foreach (Node listener in relation.Listeners)
            {
              if (index == 0)
                listener.DOFs[0].Coord = 2.0 * s.SymmetryLine[index].Value - this.DOFs[0].Coord;
              else
                listener.DOFs[1].Coord = 2.0 * s.SymmetryLine[index].Value - this.DOFs[1].Coord;
            }
          }
          else if (relation.Relation == RelationType.Offset)
          {
            foreach (Node listener in relation.Listeners)
            {
              if (index == 0)
                listener.DOFs[0].Coord = this.DOFs[0].Coord + (double) relation.Parameter;
              else
                listener.DOFs[1].Coord = this.DOFs[1].Coord + (double) relation.Parameter;
            }
          }
          else if (relation.Relation == RelationType.Average)
          {
            Node parameter = (Node) relation.Parameter;
            foreach (Node listener in relation.Listeners)
            {
              double num = this.DistanceFrom(listener) / this.DistanceFrom(parameter);
              if (index == 0)
                listener.DOFs[0].Coord = this.DOFs[0].Coord * (1.0 - num) + parameter.DOFs[0].Coord * num;
              else
                listener.DOFs[1].Coord = this.DOFs[1].Coord * (1.0 - num) + parameter.DOFs[1].Coord * num;
            }
          }
        }
      }
    }

    public double DistanceFrom(Node that)
    {
      double d = 0.0;
      for (int index = 0; index < this.DOFs.Length; ++index)
        d += Math.Pow(this.DOFs[index].Value - that.DOFs[index].Value, 2.0);
      return Math.Sqrt(d);
    }
  }
}
