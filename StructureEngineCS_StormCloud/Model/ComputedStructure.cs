// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.ComputedStructure
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B2AD7AF7-B053-4B89-AC37-223D1F22213F
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\Testing\bin\Debug\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Model
{
  public class ComputedStructure : Structure
  {
    private Matrix<double> Stiffness;

    public ComputedStructure()
    {
    }

    public ComputedStructure(Structure s) => s.CopyTo((Structure) this);

    private void MemberContribution(ComputedMember m, Matrix<double> K, int dofs)
    {
      Matrix<double> stiffness = m.CalculateStiffness();
      int index1 = m.NodeI.Index;
      int index2 = m.NodeJ.Index;
      for (int row1 = 0; row1 < 2 * dofs; ++row1)
      {
        for (int column1 = 0; column1 < 2 * dofs; ++column1)
        {
          int index3 = m.LocalDOFs[row1].Index;
          int index4 = m.LocalDOFs[column1].Index;
          double num = stiffness[row1, column1];
          Matrix<double> matrix;
          int row2;
          int column2;
          (matrix = K)[row2 = index3, column2 = index4] = matrix[row2, column2] + num;
        }
      }
    }

    private Matrix<double> CalculateStiffness()
    {
      int dofs = 6;
      Matrix<double> K = (Matrix<double>) new DenseMatrix(this.DOFs.Count);
      foreach (ComputedMember member in this.Members)
        this.MemberContribution(member, K, dofs);
      return K;
    }

    public Matrix<double> StiffnessMatrix
    {
      get
      {
        if (this.Stiffness == null)
          this.Stiffness = this.CalculateStiffness();
        return this.Stiffness;
      }
    }

    public void ClearStiffness() => this.Stiffness = (Matrix<double>) null;

    public IEnumerable<ComputedMember> ComputedMembers => this.Members.Cast<ComputedMember>();

    public override Member GetNewMember(Node i, Node j) => (Member) new ComputedMember(i, j);
  }
}
