// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.BridgeAnalysis
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.LinearAlgebra.Factorization;
using StructureEngine.Analysis;
using StructureEngine.GraphicStatics;
using StructureEngine.Model;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Bridge
{
  public class BridgeAnalysis : IAnalysis
  {
    public void SolveHorMoment(BridgeShape s)
    {
      int count = s.Deck.Count;
      int num1 = count + 1;
      int num2 = num1;
      Matrix<double> matrix = (Matrix<double>) CreateMatrix.Dense<double>(num2, num1, 0.0);
      Vector<double> vector = (Vector<double>) CreateVector.Dense<double>(num1, 0.0);
      for (int index = 1; index < num1 - 1; ++index)
      {
        ShapeLine shapeLine1 = s.Deck[index];
        ShapeLine shapeLine2 = s.Deck[index - 1];
        matrix[index, index - 1] = shapeLine2.Length;
        matrix[index, index] = 2.0 * (shapeLine2.Length + shapeLine1.Length);
        matrix[index, index + 1] = shapeLine1.Length;
      }
      matrix[0, 0] = 1.0;
      matrix[num2 - 1, num1 - 1] = 1.0;
      Vector<double> moments = ((DenseMatrix) matrix).LU().Solve(vector);
      for (int index = 0; index < count; ++index)
        s.Deck[index].BendingMoment = Math.Max(Math.Abs(moments[index]), Math.Abs(moments[index + 1]));
      this.SolveVertForce(s, moments);
    }

    private void SolveVertForce(BridgeShape s, Vector<double> moments)
    {
      int count = s.Infill2.Count;
      for (int index = 0; index < count; ++index)
      {
        double distLoad1 = s.Deck[index + 1].DistLoad;
        double distLoad2 = s.Deck[index].DistLoad;
        double length1 = s.Deck[index + 1].Length;
        double length2 = s.Deck[index].Length;
        double moment1 = moments[index + 2];
        double moment2 = moments[index + 1];
        double moment3 = moments[index];
        double num1 = length1;
        double num2 = distLoad1 * num1 / 2.0 + distLoad2 * length2 / 2.0 + moment3 / length2 + moment1 / length1 - moment2 / length2 - moment2 / length1;
        s.Infill2[index].VerticalForce = num2;
      }
      foreach (ShapeLine shapeLine in s.Infill2)
        shapeLine.AxialForce = shapeLine.GetAxialForce();
      if (s.IsSuspension)
        return;
      double distLoad3 = s.Deck[0].DistLoad;
      double length3 = s.Deck[0].Length;
      double moment4 = moments[1];
      double moment5 = moments[0];
      double num3 = length3;
      double num4 = distLoad3 * num3 / 2.0 + moment4 / length3 - moment5 / length3;
      s.Infill[0].VerticalForce = num4;
      s.Infill[0].AxialForce = s.Infill[0].GetAxialForce();
      double distLoad4 = s.Deck[count].DistLoad;
      double length4 = s.Deck[count].Length;
      double moment6 = moments[count];
      double moment7 = moments[count + 1];
      double num5 = length4;
      double num6 = distLoad4 * num5 / 2.0 + moment6 / length4 - moment7 / length4;
      s.Infill[s.Infill.Count - 1].VerticalForce = num6;
      s.Infill[s.Infill.Count - 1].AxialForce = s.Infill[s.Infill.Count - 1].GetAxialForce();
    }

    private void SolveFunicular(BridgeShape s, double force)
    {
      List<ShapeLine> loads = new List<ShapeLine>();
      List<double> widths = new List<double>();
      for (int index = 0; index < s.Infill2.Count; ++index)
      {
        double x = s.Infill2[index].Start.X;
        double num = index != 0 ? (index != s.Infill2.Count - 1 ? Math.Abs(s.Infill2[index].Start.X - s.Infill2[index - 1].Start.X) : Math.Abs(s.Infill2[index].Start.X - s.Infill2[index - 1].Start.X)) : Math.Abs(s.Infill2[index].Start.X - s.Deck[0].Start.X);
        widths.Add(num);
        ShapeLine shapeLine = new ShapeLine(new ShapePoint(x, 0.0), s.Infill2[index].Rotation + 180.0, s.Infill2[index].AxialForce);
        loads.Add(shapeLine);
      }
      widths.Add(Math.Abs(s.Deck[s.Deck.Count - 1].End.X - s.Infill2[s.Infill2.Count - 1].Start.X));
      ProblemSetup problemSetup = new ProblemSetup(s.Deck[0].Start, s.Deck[s.Deck.Count - 1].End, loads, widths);
      problemSetup.DrawFcP(force);
      problemSetup.DrawFmD(force);
    }

    public double Analyze(IDesign d)
    {
      BridgeShape s = (BridgeShape) d;
      double num1 = 3000.0 / 2000.0 * 490.0 / Math.Pow(12.0, 3.0);
      double num2 = 100.0 / 27.0 / Math.Pow(12.0, 3.0);
      this.SolveHorMoment(s);
      this.SizeMembers(s);
      double num3 = 0.0;
      double num4 = 0.0;
      foreach (ShapeLine shapeLine in s.Deck)
        num4 += shapeLine.ReqArea * shapeLine.Length * 12.0;
      foreach (ShapeLine shapeLine in s.Infill2)
        num3 += shapeLine.ReqArea * shapeLine.Length * 12.0;
      foreach (ShapeLine shapeLine in s.Infill)
        num3 += shapeLine.ReqArea * shapeLine.Length * 12.0;
      double num5 = num3 * num1 + num4 * num2;
      foreach (ShapePoint point in s.Points)
        num5 += 50.0;
      s.Score = num5 * 2.0;
      return s.Score;
    }

    private void SizeMembers(BridgeShape s)
    {
      double val2 = 0.0;
      foreach (ShapeLine shapeLine in s.Deck)
        val2 = Math.Max(shapeLine.BendingMoment, val2);
      double num1 = 30.0;
      double num2 = Math.Max(5.0, 1.6 * val2 / (4.0 * num1) + 2.5);
      foreach (ShapeLine shapeLine in s.Deck)
      {
        shapeLine.ReqThickness = num2;
        shapeLine.ReqArea = num2 * 30.0 * 12.0;
      }
      foreach (ShapeLine l in s.Infill2)
        this.SizeAxialSteel(l);
      foreach (ShapeLine l in s.Infill)
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
