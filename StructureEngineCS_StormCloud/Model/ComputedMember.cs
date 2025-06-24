// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.ComputedMember
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B2AD7AF7-B053-4B89-AC37-223D1F22213F
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\Testing\bin\Debug\StructureEngineCS.dll

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
    public Dictionary<LoadCase, double> Mx;
    public Dictionary<LoadCase, double> My;
    public Dictionary<LoadCase, double> Mz;
    public Dictionary<LoadCase, List<double>> InternalForces;

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

    public double MaxMx => Math.Max(Math.Abs(this.Mx.Values.Max()), Math.Abs(this.Mx.Values.Min()));

    public double MaxMy => Math.Max(Math.Abs(this.My.Values.Max()), Math.Abs(this.My.Values.Min()));

    public double MaxMz => Math.Max(Math.Abs(this.Mz.Values.Max()), Math.Abs(this.Mz.Values.Min()));

    public double ReqArea
    {
      get
      {
        return this.SectionType.GetReqEnvArea(this.InternalForces, this.Material.StressAllow, this.Material.E, this.Length);
      }
    }


    public double GetLCArea(LoadCase lc)
    {
      return this.SectionType.GetReqArea(this.InternalForces[lc], this.Material.StressAllow, this.Material.E, this.Length);
    }

    public double Weight => this.ReqArea * this.Length * this.Material.Density;

    public Matrix<double> CalculateStiffness()
    {
      Matrix<double> rotation = this.CalculateRotation();
      Matrix<double> localStiffness = this.CalculateLocalStiffness();
      Matrix<double> stiffness = rotation.Transpose() * localStiffness * rotation;
      if (this.IsTruss)
      {
        Matrix<double> subMatrix1 = (Matrix<double>) new DenseMatrix(5, 12);
        Matrix<double> subMatrix2 = subMatrix1.Transpose();
        stiffness.SetSubMatrix(7, 5, 0, 11, subMatrix1);
        stiffness.SetSubMatrix(0, 11, 7, 5, subMatrix2);
        stiffness.SetSubMatrix(1, 5, 0, 11, subMatrix1);
        stiffness.SetSubMatrix(0, 11, 1, 5, subMatrix2);
      }
      return stiffness;
    }

    public Matrix<double> CalculateLocalStiffness()
    {
      Matrix<double> matrix = (Matrix<double>) new DenseMatrix(12, 12);
      double e = this.Material.E;
      double poisson = this.Material.Poisson;
      double area = this.Area;
      double momentofInertiaX = this.MomentofInertiaX;
      double momentofInertiaY = this.MomentofInertiaY;
      double torsionalStiffnessJ = this.TorsionalStiffnessJ;
      double length = this.Length;
      double num1 = 10.0;
      double num2 = 1000.0;
      double num3 = 1000.0;
      double num4 = 10000.0;
      double num5 = num1 / length;
      double num6 = 12.0 * num2 / Math.Pow(length, 3.0);
      double num7 = 6.0 * num2 / Math.Pow(length, 2.0);
      double num8 = 12.0 * num3 / Math.Pow(length, 3.0);
      double num9 = 6.0 * num3 / Math.Pow(length, 2.0);
      double num10 = num4 / 2.0 / (1.0 + poisson) / length;
      double num11 = 4.0 * num3 / length;
      double num12 = 2.0 * num3 / length;
      double num13 = 4.0 * num2 / length;
      double num14 = 2.0 * num2 / length;
      Matrix<double> subMatrix1 = (Matrix<double>) new DenseMatrix(6, 6);
      subMatrix1[0, 0] = num5;
      subMatrix1[1, 1] = num6;
      subMatrix1[1, 5] = num7;
      subMatrix1[2, 2] = num8;
      subMatrix1[2, 4] = -num9;
      subMatrix1[3, 3] = num10;
      subMatrix1[4, 2] = -num9;
      subMatrix1[4, 4] = num11;
      subMatrix1[5, 1] = num7;
      subMatrix1[5, 5] = num13;
      Matrix<double> subMatrix2 = (Matrix<double>) new DenseMatrix(6, 6);
      subMatrix2[0, 0] = -num5;
      subMatrix2[1, 1] = -num6;
      subMatrix2[1, 5] = num7;
      subMatrix2[2, 2] = -num8;
      subMatrix2[2, 4] = -num9;
      subMatrix2[3, 3] = -num10;
      subMatrix2[4, 2] = num9;
      subMatrix2[4, 4] = num12;
      subMatrix2[5, 1] = -num7;
      subMatrix2[5, 5] = num14;
      Matrix<double> subMatrix3 = (Matrix<double>) new DenseMatrix(6, 6);
      subMatrix1[0, 0] = num5;
      subMatrix1[1, 1] = num6;
      subMatrix1[1, 5] = -num7;
      subMatrix1[2, 2] = num8;
      subMatrix1[2, 4] = num9;
      subMatrix1[3, 3] = num10;
      subMatrix1[4, 2] = num9;
      subMatrix1[4, 4] = num11;
      subMatrix1[5, 1] = -num7;
      subMatrix1[5, 5] = num13;
      matrix.SetSubMatrix(0, 6, 0, 6, subMatrix1);
      matrix.SetSubMatrix(0, 6, 6, 6, subMatrix2);
      matrix.SetSubMatrix(6, 6, 0, 6, subMatrix2);
      matrix.SetSubMatrix(6, 6, 6, 6, subMatrix3);
      return e * matrix;
    }

    public Matrix<double> CalculateRotation()
    {
      Matrix<double> rotation = (Matrix<double>) new DenseMatrix(12, 12);
      double cosX = this.CosX;
      double cosY = this.CosY;
      double cosZ = this.CosZ;
      double num1 = cosX * cosY / Math.Sqrt(Math.Pow(cosX, 2.0) + Math.Pow(cosZ, 2.0));
      double num2 = Math.Sqrt(Math.Pow(cosX, 2.0) + Math.Pow(cosZ, 2.0));
      double num3 = -cosX * cosZ / Math.Sqrt(Math.Pow(cosX, 2.0) + Math.Pow(cosZ, 2.0));
      double num4 = -cosZ / Math.Sqrt(Math.Pow(cosX, 2.0) + Math.Pow(cosZ, 2.0));
      double num5 = 0.0;
      double num6 = cosX / Math.Sqrt(Math.Pow(cosX, 2.0) + Math.Pow(cosZ, 2.0));
      Matrix<double> subMatrix = (Matrix<double>) new DenseMatrix(3, 3);
      subMatrix[0, 0] = cosX;
      subMatrix[0, 1] = cosY;
      subMatrix[0, 2] = cosZ;
      subMatrix[1, 0] = num1;
      subMatrix[1, 1] = num2;
      subMatrix[1, 2] = num3;
      subMatrix[2, 0] = num4;
      subMatrix[2, 1] = num5;
      subMatrix[2, 2] = num6;
      rotation.SetSubMatrix(0, 3, 0, 3, subMatrix);
      rotation.SetSubMatrix(3, 3, 3, 3, subMatrix);
      rotation.SetSubMatrix(3, 3, 3, 3, subMatrix);
      rotation.SetSubMatrix(3, 3, 3, 3, subMatrix);
      return rotation;
    }

    public DOF[] LocalDOFs
    {
      get
      {
        return new DOF[12]
        {
          this.NodeI.DOFs[0],
          this.NodeI.DOFs[1],
          this.NodeI.DOFs[2],
          this.NodeI.DOFs[3],
          this.NodeI.DOFs[4],
          this.NodeI.DOFs[5],
          this.NodeJ.DOFs[0],
          this.NodeJ.DOFs[1],
          this.NodeJ.DOFs[2],
          this.NodeJ.DOFs[3],
          this.NodeJ.DOFs[4],
          this.NodeJ.DOFs[5]
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
