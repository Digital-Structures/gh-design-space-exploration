// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.ElementGroup
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Grammar
{
  public abstract class ElementGroup : IEnumerable<IElement>, IEnumerable, IElementGroup
  {
    public IEnumerable<ShapePoint> Points
    {
      get
      {
        return this.Lines.SelectMany<ShapeLine, ShapePoint>((Func<ShapeLine, IEnumerable<ShapePoint>>) (l => (IEnumerable<ShapePoint>) new ShapePoint[2]
        {
          l.Start,
          l.End
        })).Concat<ShapePoint>(this.Areas.SelectMany<ShapeArea, ShapePoint>((Func<ShapeArea, IEnumerable<ShapePoint>>) (a => (IEnumerable<ShapePoint>) a.Points))).Distinct<ShapePoint>();
      }
    }

    public IEnumerable<ShapeLine> Lines => this.OfType<ShapeLine>();

    public IEnumerable<ShapeArea> Areas => this.OfType<ShapeArea>();

    public void Rotate(double angle, ShapePoint center)
    {
      foreach (ShapePoint point in this.Points)
        point.RotateAbout(angle, center);
    }

    public void Translate(double x, double y)
    {
      foreach (ShapePoint point in this.Points)
      {
        point.X += x;
        point.Y += y;
      }
    }

    public virtual void Scale(double scalex, double scaley)
    {
      double x = this.ZeroShapePoint.X;
      double y = this.ZeroShapePoint.Y;
      ShapePoint zeroShapePoint = this.ZeroShapePoint;
      foreach (ShapePoint point in this.Points)
      {
        ShapeLine shapeLine1 = new ShapeLine(zeroShapePoint, point);
        ShapeLine shapeLine2 = shapeLine1.Clone();
        shapeLine2.Scale(scalex);
        ShapeLine shapeLine3 = shapeLine1.Clone();
        shapeLine3.Scale(scaley);
        point.X = shapeLine2.End.X;
        point.Y = shapeLine3.End.Y;
      }
      this.Translate(-this.ZeroShapePoint.X + x, -this.ZeroShapePoint.Y + y);
    }

    public ShapePoint ZeroShapePoint
    {
      get
      {
        double num1 = 0.0;
        double num2 = 0.0;
        foreach (ShapeLine line in this.Lines)
        {
          num1 = Math.Min(num1, line.Start.X);
          num1 = Math.Min(num1, line.End.X);
          num2 = Math.Min(num2, line.Start.Y);
          num2 = Math.Min(num2, line.End.Y);
        }
        return new ShapePoint(num1, num2);
      }
    }

    public double[] Dimensions
    {
      get
      {
        double val1_1 = 0.0;
        double val1_2 = 0.0;
        foreach (ShapeLine line in this.Lines)
        {
          val1_1 = Math.Max(val1_1, line.Start.X);
          val1_1 = Math.Max(val1_1, line.End.X);
          val1_2 = Math.Max(val1_2, line.Start.Y);
          val1_2 = Math.Max(val1_2, line.End.Y);
        }
        return new double[2]
        {
          val1_1 - this.ZeroShapePoint.X,
          val1_2 - this.ZeroShapePoint.Y
        };
      }
    }

    public abstract IEnumerator<IElement> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
      IEnumerator<IElement> enumer = this.GetEnumerator();
      while (enumer.MoveNext())
        yield return (object) enumer.Current;
    }
  }
}
