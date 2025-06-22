// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.ComputedMember
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Model
{
  public class ComputedMember : Member
  {
    public Dictionary<LoadCase, double> AxialForce;

    public ComputedMember(Node i, Node j)
      : base(i, j)
    {
      this.AxialForce = new Dictionary<LoadCase, double>();
    }

    public double MaxAxialForce
    {
      get
      {
        return Math.Max(Math.Abs(this.AxialForce.Values.Max()), Math.Abs(this.AxialForce.Values.Min()));
      }
    }

    public double ReqArea
    {
      get
      {
        return this.SectionType.GetReqEnvArea(this.AxialForce, this.Material.StressAllow, this.Material.E, this.Length);
      }
    }

    public double GetLCArea(LoadCase lc)
    {
      return this.SectionType.GetReqArea(this.AxialForce[lc], this.Material.StressAllow, this.Material.E, this.Length);
    }

    public double Weight => this.ReqArea * this.Length * this.Material.Density;

    public Matrix<double> CalculateStiffness()
    {
            Matrix<double> k = new DenseMatrix(4, 4);

            // Element Stiffness Matrix k:
            // [ (cos(theta))^2  sin(theta)*cos(theta)  -(cos(theta))^2  -sin(theta)*cos(theta) ]
            // [ sin(theta)*cos(theta)  (sin(theta))^2  -sin(theta)*cos(theta)  -(sin(theta))^2 ]
            // [ -(cos(theta))^2  -sin(theta)*cos(theta)  (cos(theta))^2  sin(theta)*cos(theta) ]
            // [ -sin(theta)*cos(theta)  -(sin(theta))^2  sin(theta)*cos(theta)  (sin(theta))^2 ]

            k[0, 0] = Math.Pow(Math.Cos(this.Angle), 2);
            k[0, 1] = Math.Sin(this.Angle) * Math.Cos(this.Angle);
            k[1, 1] = Math.Pow(Math.Sin(this.Angle), 2);
            k[1, 0] = k[0, 1];

            Matrix<double> submat = k.SubMatrix(0, 2, 0, 2);

            k.SetSubMatrix(2, 2, 2, 2, submat);
            k.SetSubMatrix(2, 2, 0, 2, submat.Negate());
            k.SetSubMatrix(0, 2, 2, 2, submat.Negate());

            return k * (this.Material.E * this.Area / this.Length);
        }

        public DOF[] LocalDOFs
    {
      get
      {
        return new DOF[4]
        {
          this.NodeI.DOFs[0],
          this.NodeI.DOFs[1],
          this.NodeJ.DOFs[0],
          this.NodeJ.DOFs[1]
        };
      }
    }

    public ComputedMember MemberClone() => (ComputedMember) this.CloneImpl();

    protected override Member CloneImpl()
    {
      ComputedMember m = new ComputedMember(this.NodeI, this.NodeJ);
      this.CopyTo((Member) m);
      return (Member) m;
    }

    internal override void CopyTo(Member m) => base.CopyTo(m);
  }
}
