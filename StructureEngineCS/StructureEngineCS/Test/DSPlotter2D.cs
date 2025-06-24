// Decompiled with JetBrains decompiler
// Type: StructureEngine.Test.DSPlotter2D
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using StructureEngine.Model;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Test
{
  public class DSPlotter2D
  {
    private IDesign Problem;

    public DSPlotter2D() => this.SetProblem();

    private void SetProblem()
    {
      IDesign design = new StructureSetup().Designs[0];
      ComputedStructure computedStructure = design is ComputedStructure ? (ComputedStructure) design : throw new Exception("Problem is not of type ComputedStructure.");
      computedStructure.Nodes[1].DOFs[0].Coord = 39.3701;
      computedStructure.Nodes[1].DOFs[1].Coord = -39.3701;
      computedStructure.Nodes[2].DOFs[0].Coord = 118.11;
      computedStructure.Nodes[2].DOFs[1].Coord = -39.3701;
      computedStructure.Nodes[3].DOFs[0].Coord = 78.7402;
      computedStructure.Nodes[4].DOFs[0].Coord = 157.48;
      foreach (IVariable designVariable in design.DesignVariables)
        designVariable.SetConstraint();
      computedStructure.Nodes[3].DOFs[1].Free = false;
      computedStructure.Nodes[3].DOFs[1].AllowableVariation = new double?(0.0);
      computedStructure.Nodes[1].DOFs[0].AllowableVariation = new double?(39.3);
      computedStructure.Nodes[1].DOFs[1].AllowableVariation = new double?(157.48);
      computedStructure.LoadCases[0].GetLoad(computedStructure.Nodes[3].DOFs[1]).Value = -22.48;
      computedStructure.SymmetryLine[0] = new double?(78.7402);
      this.Problem = design;
    }

    public Matrix<double> GenerateData(int nR, int nC)
    {
      Matrix<double> matrix = (Matrix<double>) CreateMatrix.Dense<double>(nR, nC);
      double num1 = 1.0 / ((double) nR - 1.0);
      double num2 = 1.0 / ((double) nC - 1.0);
      double val2 = double.MaxValue;
      for (int index1 = 0; index1 < nR; ++index1)
      {
        for (int index2 = 0; index2 < nC; ++index2)
        {
          double score = this.Problem.GenerateFromVars(new double[2]
          {
            (double) index1 * num1,
            (double) index2 * num2
          }).Score;
          val2 = Math.Min(score, val2);
          matrix[index1, index2] = score;
        }
      }
      return matrix.PointwiseDivide((Matrix<double>) CreateMatrix.Dense<double>(nR, nC, val2));
    }

    public IEnumerable<IDesign> GetDesigns(Matrix<double> data, double score, double tolerance)
    {
      IList<IDesign> designs = (IList<IDesign>) new List<IDesign>();
      double rowCount = (double) data.RowCount;
      double columnCount = (double) data.ColumnCount;
      for (int index1 = 0; (double) index1 < rowCount; ++index1)
      {
        for (int index2 = 0; (double) index2 < columnCount; ++index2)
        {
          if (Math.Abs(data[index1, index2] - score) <= tolerance)
          {
            Math.Abs((double) index1 / (rowCount - 1.0) - 1.0);
            IDesign fromVars = this.Problem.GenerateFromVars(new double[2]
            {
              (double) index1 / (rowCount - 1.0),
              (double) index2 / (columnCount - 1.0)
            });
            designs.Add(fromVars);
          }
        }
      }
      return (IEnumerable<IDesign>) designs;
    }
  }
}
