// Decompiled with JetBrains decompiler
// Type: StructureEngine.GraphicStatics.ProblemSetup
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Grammar;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.GraphicStatics
{
  public class ProblemSetup
  {
    public List<PointLoad> Loads;
    public ShapePoint Start;
    public ShapePoint End;
    public double DeltaX;
    public double DeltaY;
    public double Slope;
    public double Int;
    public List<double> SegWidths;
    public double TotalLoad;
    public ForcePolygon ForceP;
    public FormDiagram FormD;
    public ShapeLine ClosingString;

    public ProblemSetup(ShapePoint start, ShapePoint end, int inc, double loadperlength)
    {
      this.Start = start;
      this.End = end;
      this.Setup(inc, loadperlength);
    }

    public ProblemSetup(
      ShapePoint start,
      ShapePoint end,
      List<ShapeLine> loads,
      List<double> widths)
    {
      this.Start = start;
      this.End = end;
      this.SetupLoads(loads);
      this.SegWidths = widths;
    }

    private void Setup(int inc, double loadperlength)
    {
      this.GenerateLoads(inc, loadperlength);
      this.ClosingString = new ShapeLine(this.Start, this.End);
      this.TotalLoad = (this.End.X - this.Start.X) * loadperlength;
    }

    private void SetupLoads(List<ShapeLine> loads)
    {
      List<PointLoad> pointLoadList = new List<PointLoad>();
      foreach (ShapeLine load in loads)
      {
        PointLoad pointLoad = new PointLoad(load.Start, load.End.Y - load.Start.Y);
        pointLoadList.Add(pointLoad);
      }
      this.Loads = pointLoadList;
      this.DeltaX = this.End.X - this.Start.X;
      this.DeltaY = this.End.Y - this.Start.Y;
      this.Slope = this.DeltaY / this.DeltaX;
    }

    private void GenerateLoads(int inc, double loadperlength)
    {
      this.DeltaX = this.End.X - this.Start.X;
      this.DeltaY = this.End.Y - this.Start.Y;
      this.Slope = this.DeltaY / this.DeltaX;
      this.Int = this.Start.Y - this.Slope * this.Start.X;
      this.SegWidths = new List<double>();
      double num = this.DeltaX / (double) inc;
      for (int index = 0; index < inc; ++index)
        this.SegWidths.Add(num);
      double l = loadperlength * num;
      List<PointLoad> pointLoadList = new List<PointLoad>();
      for (int index = 0; index < inc; ++index)
      {
        double x = this.Start.X + this.SegWidths[index] * ((double) index + 0.5);
        double y = this.Slope * x + this.Int;
        PointLoad pointLoad = new PointLoad(new ShapePoint(x, y), l);
        pointLoadList.Add(pointLoad);
      }
      this.Loads = pointLoadList;
    }

    public void DrawFcP(double h) => this.ForceP = new ForcePolygon(this, h);

    public void DrawFmD(double h) => this.FormD = new FormDiagram(this, h);
  }
}
