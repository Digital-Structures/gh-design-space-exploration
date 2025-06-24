// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.ShapePoint
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;

#nullable disable
namespace StructureEngine.Grammar
{
  public class ShapePoint : IElement
  {
    public ShapePoint(double x, double y)
    {
      this.X = x;
      this.Y = y;
    }

    public double X { get; set; }

    public double Y { get; set; }

    public double GetDistance(ShapePoint p2)
    {
      return Math.Sqrt(Math.Pow(this.X - p2.X, 2.0) + Math.Pow(this.Y - p2.Y, 2.0));
    }

    public double GetAngle(ShapePoint start)
    {
      return Math.Atan2(this.Y - start.Y, this.X - start.X) * 180.0 / Math.PI;
    }

    public bool IsSame(IElement elem)
    {
      ShapePoint shapePoint = elem as ShapePoint;
      if (elem == null || shapePoint == null)
        return false;
      double num1 = Math.Abs(this.X - shapePoint.X);
      double num2 = Math.Abs(this.Y - shapePoint.Y);
      return num1 < 0.001 && num2 < 0.001;
    }

    public void RotateAbout(double angle, ShapePoint center)
    {
      double distance = this.GetDistance(center);
      double num1 = this.GetAngle(center) + angle;
      double num2 = distance * Math.Cos(num1 * Math.PI / 180.0);
      double num3 = distance * Math.Sin(num1 * Math.PI / 180.0);
      this.X = center.X + num2;
      this.Y = center.Y + num3;
    }

    public double[] RotateCoords(ShapePoint origin, double angle)
    {
      double distance = this.GetDistance(origin);
      return new double[2]
      {
        distance * Math.Cos(Math.PI * angle / 180.0),
        distance * Math.Sin(Math.PI * angle / 180.0)
      };
    }

    IElement IElement.Clone() => (IElement) this.Clone();

    public ShapePoint Clone() => new ShapePoint(this.X, this.Y);

    public void Copy(ShapePoint p)
    {
      this.X = p.X;
      this.Y = p.Y;
    }

    public bool IsBetween(ShapePoint p1, ShapePoint p2)
    {
      bool flag = false;
      double x1 = p1.X;
      double x2 = p2.X;
      if (x2 < x1)
      {
        x1 = p2.X;
        x2 = p1.X;
      }
      double y1 = p1.Y;
      double y2 = p2.Y;
      if (y2 < y1)
      {
        y1 = p2.Y;
        y2 = p1.Y;
      }
      if ((this.X < x2 || Math.Abs(this.X - x2) < 0.0001) && (this.X > x1 || Math.Abs(this.X - x1) < 0.0001) && this.Y <= y2 && this.Y >= y1)
        flag = true;
      return flag;
    }

    public ShapePoint GetProjection(ShapeLine l)
    {
      double[] numArray = l.SlopeIntercept();
      double num1 = numArray[0];
      double num2 = numArray[1];
      double x;
      double y;
      if (num1 != 0.0)
      {
        double num3 = -1.0 / num1;
        x = (this.Y - num3 * this.X - num2) / (num1 - num3);
        y = num1 * x + num2;
      }
      else
      {
        x = this.X;
        y = num2;
      }
      return new ShapePoint(x, y);
    }

    public ShapeLine GetProjectionPath(ShapeLine l) => new ShapeLine(this, this.GetProjection(l));

    public ShapePoint ReflectAcross(ShapeLine l)
    {
      ShapeLine projectionPath = this.GetProjectionPath(l);
      return new ShapeLine(this, projectionPath.Rotation, 2.0 * projectionPath.Length).End;
    }
  }
}
