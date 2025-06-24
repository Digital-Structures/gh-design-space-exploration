// Decompiled with JetBrains decompiler
// Type: StructureEngine.Analysis.FrameAnalysis
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B2AD7AF7-B053-4B89-AC37-223D1F22213F
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\Testing\bin\Debug\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.LinearAlgebra;
using StructureEngine.Model;
using System;
using System.Linq;

#nullable disable
namespace StructureEngine.Analysis
{
  public class FrameAnalysis
  {
    public void RunAnalysis(ComputedStructure comp)
    {
      int rowLength = comp.DOFs.Count<DOF>((Func<DOF, bool>) (d => d.Fixed));
      int num = comp.DOFs.Count - rowLength;
      Matrix<double> stiffnessMatrix = comp.StiffnessMatrix;
      Matrix<double> matrix1 = stiffnessMatrix.SubMatrix(0, num, 0, num);
      Console.WriteLine(num);
      Console.WriteLine(rowLength);
      Matrix<double> matrix2 = stiffnessMatrix.SubMatrix(num, rowLength, 0, num);
      for (int index = 0; index < matrix1.RowCount; ++index)
        Console.WriteLine(matrix1[index, index]);
      Console.WriteLine("CheckPoint1");
      if (matrix1.Determinant() <= 0.0)
        throw new Exception("Unstable Structure(1)");
      Console.WriteLine("CheckPoint2");
      for (int index = 0; index < matrix1.RowCount; ++index)
      {
        if (matrix1[index, index] == 0.0)
          throw new Exception("Unstable Structure(2)");
      }
      Console.WriteLine("CheckPoint3");
      foreach (ComputedMember member in comp.Members)
        member.AxialForce.Clear();
      foreach (DOF doF in comp.DOFs)
        doF.Disp.Clear();
      foreach (LoadCase loadCase in comp.LoadCases)
      {
        Vector<double> loadVector = loadCase.GetLoadVector((Structure) comp);
        Console.WriteLine("CheckPoint4");
        Vector<double> input = loadVector.SubVector(0, num);
        Console.WriteLine("CheckPoint5");
        Vector<double> rightSide = ((DenseMatrix)matrix1).LU().Solve(input);
        long ticks = DateTime.Now.Ticks;
        Console.WriteLine("CheckPoint6");
        Vector<double> vector1 = (Vector<double>) new DenseVector(comp.DOFs.Count);
        for (int index = 0; index < rightSide.Count; ++index)
          vector1[index] = rightSide[index];
        Console.WriteLine("CheckPoint7");
        for (int i = 0; i < vector1.Count; ++i)
          comp.DOFs.Single<DOF>((Func<DOF, bool>) (d => d.Index == i)).Disp.Add(loadCase, vector1[i]);
        Console.WriteLine("CheckPoint8");
        Vector<double> vector2 = matrix2.Multiply(rightSide);
        for (int index = 0; index < vector2.Count; ++index)
          loadVector[num + index] = vector2[index];
        Console.WriteLine("CheckPoint9");
        foreach (ComputedMember computedMember in comp.ComputedMembers)
        {
          Vector<double> vector3 = (Vector<double>) new DenseVector(12);
          for (int index = 0; index < 12; ++index)
            vector3[index] = vector1[computedMember.LocalDOFs[index].Index];
          Vector<double> vector4 = computedMember.CalculateLocalStiffness() * computedMember.CalculateRotation() * vector3;
          computedMember.AxialForce.Add(loadCase, vector4[0]);
        }
        Console.WriteLine("CheckPoint10");
      }
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
