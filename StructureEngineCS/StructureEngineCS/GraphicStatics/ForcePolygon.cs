// Decompiled with JetBrains decompiler
// Type: StructureEngine.GraphicStatics.ForcePolygon
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Grammar;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.GraphicStatics
{
  public class ForcePolygon
  {
    public ProblemSetup ProbSet;
    public double H;
    public List<ShapeLine> LoadLine;
    public List<ShapeLine> Rays;
    public ShapePoint ReactionPoint;
    public ShapePoint Pole;

    public ForcePolygon(ProblemSetup ps, double h)
    {
      this.ProbSet = ps;
      this.H = h;
      this.LoadLine = new List<ShapeLine>();
      this.Rays = new List<ShapeLine>();
      this.GenerateFP();
    }

    private void GenerateFP()
    {
      List<ShapePoint> shapePointList = new List<ShapePoint>();
      List<ShapeLine> shapeLineList = new List<ShapeLine>();
      ShapePoint shapePoint1 = new ShapePoint(0.0, 0.0);
      shapePointList.Add(shapePoint1);
      for (int index = 1; index < this.ProbSet.Loads.Count + 1; ++index)
      {
        ShapePoint shapePoint2 = new ShapePoint(0.0, shapePointList[index - 1].Y + this.ProbSet.Loads[index - 1].Load);
        shapePointList.Add(shapePoint2);
      }
      for (int index = 0; index < shapePointList.Count - 1; ++index)
      {
        ShapeLine shapeLine = new ShapeLine(shapePointList[index], shapePointList[index + 1]);
        shapeLineList.Add(shapeLine);
      }
      this.LoadLine = shapeLineList;
      this.ReactionPoint = new ShapePoint(0.0, shapeLineList[shapeLineList.Count - 1].End.Y + this.GetReaction());
      this.Pole = new ShapePoint(0.0 + this.H, this.ProbSet.Slope * (0.0 + this.H) + this.ReactionPoint.Y);
      this.Rays.Add(new ShapeLine(this.LoadLine[0].Start, this.Pole));
      foreach (ShapeLine shapeLine in this.LoadLine)
        this.Rays.Add(new ShapeLine(shapeLine.End, this.Pole));
    }

    private double GetReaction()
    {
      double num1 = 0.0;
      foreach (PointLoad load1 in this.ProbSet.Loads)
      {
        double num2 = Math.Abs(load1.Point.X - this.ProbSet.Start.X);
        double load2 = load1.Load;
        num1 += load2 * num2;
      }
      return Math.Abs(num1) / Math.Abs(this.ProbSet.End.X - this.ProbSet.Start.X);
    }
  }
}
