// Decompiled with JetBrains decompiler
// Type: StructureEngine.Analysis.BaseTrussAnalysis
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using StructureEngine.Model;
using System;
using System.Linq;

#nullable disable
namespace StructureEngine.Analysis
{
  public abstract class BaseTrussAnalysis
  {
    protected void RunAnalysis(ComputedStructure comp)
    {
      long ticks1 = DateTime.Now.Ticks;
      Matrix<double> stiffnessMatrix = comp.StiffnessMatrix;
      long ticks2 = DateTime.Now.Ticks;
      int num1 = comp.DOFs.Count<DOF>((Func<DOF, bool>) (d => d.Pinned));
      int num2 = comp.DOFs.Count - num1;
      Matrix<double> matrix1 = stiffnessMatrix.SubMatrix(0, num2, 0, num2);
      Matrix<double> matrix2 = stiffnessMatrix.SubMatrix(num2, num1, 0, num2);
      long ticks3 = DateTime.Now.Ticks;
      double num3 = matrix1.Determinant();
      if (num3 <= 0.0)
        return;
      for (int index = 0; index < matrix1.RowCount; ++index)
      {
        if (matrix1[index, index] == 0.0)
          return;
      }
      long ticks4 = DateTime.Now.Ticks;
      foreach (ComputedMember member in comp.Members)
        member.AxialForce.Clear();
      foreach (DOF doF in comp.DOFs)
        doF.Disp.Clear();
      foreach (LoadCase loadCase in comp.LoadCases)
      {
        Vector<double> loadVector = loadCase.GetLoadVector((Structure) comp);
        Vector<double> vector1 = loadVector.SubVector(0, num2);
        Vector<double> vector2 = ((DenseMatrix)matrix1).LU().Solve(vector1);
        long ticks5 = DateTime.Now.Ticks;
        Vector<double> vector3 = (Vector<double>) CreateVector.Dense<double>(comp.DOFs.Count);
        for (int index = 0; index < vector2.Count; ++index)
          vector3[index] = vector2[index];
        for (int i = 0; i < vector3.Count; i++)
          comp.DOFs.Single<DOF>((Func<DOF, bool>) (d => d.Index == i)).Disp.Add(loadCase, vector3[i]);
        Vector<double> vector4 = matrix2.Multiply(vector2);
        for (int index = 0; index < vector4.Count; ++index)
          loadVector[num2 + index] = vector4[index];
        long ticks6 = DateTime.Now.Ticks;
        foreach (ComputedMember computedMember in comp.ComputedMembers)
        {
          Vector<double> vector5 = (Vector<double>) CreateVector.Dense<double>(4);
          for (int index = 0; index < 4; ++index)
            vector5[index] = vector3[computedMember.LocalDOFs[index].Index];
          Vector<double> vector6 = computedMember.CalculateStiffness().SubMatrix(2, 2, 0, 4).Multiply(vector5);
          DenseMatrix denseMatrix = new DenseMatrix(1, comp.Members[0].NodeI.DOFs.Length);
          ((Matrix<double>) denseMatrix)[0, 0] = Math.Cos(computedMember.Angle);
          ((Matrix<double>) denseMatrix)[0, 1] = Math.Sin(computedMember.Angle);
          Vector<double> vector7 = ((Matrix<double>) denseMatrix).Multiply(vector6);
          computedMember.AxialForce.Add(loadCase, vector7[0]);
        }
      }
      long ticks7 = DateTime.Now.Ticks;
      comp.DetK = num3;
      long ticks8 = DateTime.Now.Ticks;
      double totalMilliseconds1 = new TimeSpan(DateTime.Now.Ticks - ticks1).TotalMilliseconds;
      TimeSpan timeSpan = new TimeSpan(ticks2 - ticks1);
      double totalMilliseconds2 = timeSpan.TotalMilliseconds;
      timeSpan = new TimeSpan(ticks3 - ticks2);
      double totalMilliseconds3 = timeSpan.TotalMilliseconds;
      double totalMilliseconds4 = new TimeSpan(ticks4 - ticks3).TotalMilliseconds;
      long num4 = ticks7;
      double totalMilliseconds5 = new TimeSpan(ticks8 - num4).TotalMilliseconds;
      if (comp.CompTime.HasValue)
        return;
      comp.CompTime = new double?(totalMilliseconds1);
    }

    private bool CheckMemberSizes(ComputedStructure comp)
    {
      bool flag = true;
      foreach (ComputedMember member in comp.Members)
      {
        double reqArea = member.ReqArea;
        double area = member.Area;
        if (Math.Abs(reqArea - area) / reqArea > 0.1)
        {
          member.Area = reqArea;
          flag = false;
        }
      }
      return flag;
    }
  }
}
