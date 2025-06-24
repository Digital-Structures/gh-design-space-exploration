// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Simple.SimpleAnalysis
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using StructureEngine.Analysis;
using StructureEngine.GraphicStatics;
using StructureEngine.Model;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Simple
{
  public class SimpleAnalysis : IAnalysis
  {
    public void SolveHorMoment(SimpleShape s)
    {
      int count = s.Horizontal.Count;
      int num1 = count + 1;
      int num2 = num1;
      Matrix<double> matrix = (Matrix<double>)  CreateMatrix.Dense<double>(num2, num1, 0.0);
      Vector<double> vector = (Vector<double>) CreateVector.Dense<double>(num1, 0.0);
      for (int index = 1; index < num1 - 1; ++index)
      {
        ShapeLine shapeLine1 = s.Horizontal[index];
        ShapeLine shapeLine2 = s.Horizontal[index - 1];
        matrix[index, index - 1] = shapeLine2.Length;
        matrix[index, index] = 2.0 * (shapeLine2.Length + shapeLine1.Length);
        matrix[index, index + 1] = shapeLine1.Length;
        double num3 = shapeLine2.DistLoad * Math.Pow(shapeLine2.Length, 3.0) / 12.0;
        double num4 = shapeLine1.DistLoad * Math.Pow(shapeLine1.Length, 3.0) / 12.0;
        vector[index] = -6.0 * (shapeLine2.DistLoad * Math.Pow(shapeLine2.Length, 3.0) / 12.0 * 0.5 + shapeLine1.DistLoad * Math.Pow(shapeLine1.Length, 3.0) / 12.0 * 0.5);
        double num5 = vector[index];
      }
      matrix[0, 0] = 1.0;
      matrix[num2 - 1, num1 - 1] = 1.0;
      Vector<double> moments =  matrix.LU().Solve(vector);
      for (int index = 0; index < count; ++index)
        s.Horizontal[index].BendingMoment = Math.Max(Math.Abs(moments[index]), Math.Abs(moments[index + 1]));
      this.SolveVertForce(s, moments);
    }

    private void SolveVertForce(SimpleShape s, Vector<double> moments)
    {
      int count = s.Verticals.Count;
      for (int index = 0; index < count; ++index)
      {
        double distLoad1 = s.Horizontal[index + 1].DistLoad;
        double distLoad2 = s.Horizontal[index].DistLoad;
        double length1 = s.Horizontal[index + 1].Length;
        double length2 = s.Horizontal[index].Length;
        double moment1 = moments[index + 2];
        double moment2 = moments[index + 1];
        double moment3 = moments[index];
        double num1 = length1;
        double num2 = distLoad1 * num1 / 2.0 + distLoad2 * length2 / 2.0 + moment3 / length2 + moment1 / length1 - moment2 / length2 - moment2 / length1;
        s.Verticals[index].AxialForce = num2;
      }
    }

    public void SolveFunicular(SimpleShape s, double force)
    {
      List<ShapeLine> loads = new List<ShapeLine>();
      List<double> widths = new List<double>();
      for (int index = 0; index < s.Verticals.Count; ++index)
      {
        double x = s.Verticals[index].Start.X;
        double num = index != 0 ? (index != s.Verticals.Count - 1 ? Math.Abs(s.Verticals[index].Start.X - s.Verticals[index - 1].Start.X) : Math.Abs(s.Verticals[index].Start.X - s.Verticals[index - 1].Start.X)) : Math.Abs(s.Verticals[index].Start.X - s.Start.X);
        widths.Add(num);
        ShapePoint i = new ShapePoint(x, 0.0);
        ShapePoint j = new ShapePoint(x, i.Y - s.Verticals[index].AxialForce);
        ShapeLine shapeLine = new ShapeLine(i, j);
        loads.Add(shapeLine);
      }
      widths.Add(Math.Abs(s.End.X - s.Verticals[s.Verticals.Count - 1].Start.X));
      ProblemSetup problemSetup = new ProblemSetup(s.Horizontal[0].Start, s.Horizontal[s.Horizontal.Count - 1].End, loads, widths);
      problemSetup.DrawFcP(force);
      problemSetup.DrawFmD(force);
      s.Funicular.AddRange((IEnumerable<ShapeLine>) problemSetup.FormD.Segments);
    }

    public double Analyze(IDesign d)
    {
      SimpleShape s = (SimpleShape) d;
      double num1 = 3000.0 / 2000.0 * 490.0 / Math.Pow(12.0, 3.0);
      double num2 = 100.0 / 27.0 / Math.Pow(12.0, 3.0);
      this.SizeMembers(s);
      double num3 = 0.0;
      double num4 = 0.0;
      foreach (ShapeLine shapeLine in s.Horizontal)
        num4 += shapeLine.ReqArea * shapeLine.Length * 12.0;
      foreach (ShapeLine vertical in s.Verticals)
        num3 += vertical.ReqArea * vertical.Length * 12.0;
      foreach (ShapeLine shapeLine in s.Funicular)
        num3 += shapeLine.ReqArea * shapeLine.Length * 12.0;
      double num5 = num3 * num1 + num4 * num2;
      foreach (ShapePoint point in s.Points)
        num5 += 50.0;
      s.Score = num5 * 2.0;
      return s.Score;
    }

    private void SizeMembers(SimpleShape s)
    {
      double val2 = 0.0;
      foreach (ShapeLine shapeLine in s.Horizontal)
        val2 = Math.Max(shapeLine.BendingMoment, val2);
      double num1 = 10.0;
      double num2 = Math.Max(5.0, 1.6 * val2 / (4.0 * num1) + 2.5);
      foreach (ShapeLine shapeLine in s.Horizontal)
      {
        shapeLine.ReqThickness = num2;
        shapeLine.ReqArea = num2 * 10.0 * 12.0;
      }
      foreach (ShapeLine vertical in s.Verticals)
        this.SizeAxialSteel(vertical);
      foreach (ShapeLine l in s.Funicular)
        this.SizeAxialSteel(l);
    }

    private void SizeAxialSteel(ShapeLine l)
    {
      double axialForce = l.AxialForce;
      if (axialForce > 0.0)
      {
        l.ReqArea = Math.Abs(axialForce) / 20.0;
        l.ReqThickness = Math.Sqrt(l.ReqArea / Math.PI) * 2.0;
      }
      else if (axialForce < 0.0)
      {
        double x1 = 1.0 - 0.05;
        double num = 3.0 * (Math.Abs(axialForce) * Math.Pow(l.Length * 12.0, 2.0)) / (Math.Pow(Math.PI, 2.0) * 29000.0);
        double x2 = Math.Pow(4.0 / ((1.0 - Math.Pow(x1, 4.0)) * Math.PI) * num, 0.25);
        double val1 = Math.Abs(axialForce) / 20.0;
        double val2 = Math.PI * Math.Pow(x2, 2.0) * (1.0 - Math.Pow(x1, 2.0));
        l.ReqArea = Math.Max(val1, val2);
        l.ReqThickness = 2.0 * x2;
      }
      else
      {
        l.ReqArea = 0.0;
        l.ReqThickness = 0.0;
      }
    }
  }
}
