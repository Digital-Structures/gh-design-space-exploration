// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.ShapeLine
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public class ShapeLine : IElement
  {
    public ShapeLine(ShapePoint i, ShapePoint j)
    {
      this.Start = i;
      this.End = j;
      this.Loads = new List<ShapePointLoad>();
    }

    public ShapeLine(ShapePoint i, double angle, double length)
    {
      this.Start = i;
      double num = angle * Math.PI / 180.0;
      this.End = new ShapePoint(i.X + length * Math.Cos(num), i.Y + length * Math.Sin(num));
      this.Loads = new List<ShapePointLoad>();
    }

    public ShapePoint Start { get; set; }

    public ShapePoint End { get; set; }

    public ShapePoint MidPoint => new ShapeLine(this.Start, this.Rotation, this.Length * 0.5).End;

    public ShapePoint Joint { get; set; }

    public List<ShapePointLoad> Loads { get; set; }

    public double Length => this.Start.GetDistance(this.End);

    public double DistLoad { get; set; }

    public double AxialForce { get; set; }

    public double VerticalForce { get; set; }

    public double HorizontalForce { get; set; }

    public double GetAxialForce()
    {
      if (this.VerticalForce == 0.0 && this.HorizontalForce == 0.0)
        return 0.0;
      if (this.VerticalForce != 0.0 && this.HorizontalForce == 0.0)
        return Math.Abs(this.VerticalForce / Math.Sin(Math.PI * this.Rotation / 180.0));
      return this.VerticalForce == 0.0 && this.HorizontalForce != 0.0 ? Math.Abs(this.HorizontalForce / Math.Cos(Math.PI * this.Rotation / 180.0)) : Math.Sqrt(Math.Pow(this.HorizontalForce, 2.0) + Math.Pow(this.VerticalForce, 2.0));
    }

    public double BendingMoment { get; set; }

    public double Rotation => this.End.GetAngle(this.Start);

    public double ReqThickness { get; set; }

    public double ReqArea { get; set; }

    IElement IElement.Clone() => (IElement) this.Clone();

    public ShapeLine Clone()
    {
      ShapeLine shapeLine = new ShapeLine(this.Start.Clone(), this.End.Clone());
      List<ShapePointLoad> shapePointLoadList = new List<ShapePointLoad>();
      foreach (ShapePointLoad load in this.Loads)
      {
        ShapePointLoad shapePointLoad = load.Clone();
        shapePointLoadList.Add(shapePointLoad);
      }
      shapeLine.Loads = shapePointLoadList;
      shapeLine.AxialForce = this.AxialForce;
      shapeLine.BendingMoment = this.BendingMoment;
      shapeLine.ReqArea = this.ReqArea;
      shapeLine.ReqThickness = this.ReqThickness;
      shapeLine.DistLoad = this.DistLoad;
      return shapeLine;
    }

    public bool IsSame(IElement elem)
    {
      ShapeLine shapeLine = elem as ShapeLine;
      return elem != null && shapeLine != null && this.Start.IsSame((IElement) shapeLine.Start) && this.End.IsSame((IElement) shapeLine.End);
    }

    public void Copy(ShapeLine l)
    {
      this.Start.Copy(l.Start);
      this.End.Copy(l.End);
    }

    public void Scale(double scale)
    {
      this.Copy(new ShapeLine(this.Start, this.Rotation, this.Length * scale));
    }

    public void Rotate(double angle) => this.End.RotateAbout(angle, this.Start);

    public void RotateCenter(double angle, ShapePoint center)
    {
      this.Start.RotateAbout(angle, center);
      this.End.RotateAbout(angle, center);
    }

    public void ReverseLine()
    {
      ShapePoint end = this.End;
      ShapePoint start = this.Start;
      this.Start = end;
      this.End = start;
    }

    public List<ShapePoint> Subdivide(int num)
    {
      List<ShapePoint> shapePointList = new List<ShapePoint>();
      if (num > 1)
      {
        double num1 = this.Length / (double) num;
        double num2 = this.Rotation * Math.PI / 180.0;
        ShapePoint shapePoint1 = this.Start;
        for (int index = 0; index < num - 1; ++index)
        {
          ShapePoint shapePoint2 = new ShapePoint(shapePoint1.X + num1 * Math.Cos(num2), shapePoint1.Y + num1 * Math.Sin(num2));
          shapePointList.Add(shapePoint2);
          shapePoint1 = shapePoint2;
        }
      }
      return shapePointList;
    }

    public List<ShapeLine> GetSubdivide(int num)
    {
      List<ShapeLine> subdivide = new List<ShapeLine>();
      List<ShapePoint> shapePointList = this.Subdivide(num);
      shapePointList.Insert(0, this.Start);
      shapePointList.Add(this.End);
      for (int index = 0; index < num; ++index)
        subdivide.Add(new ShapeLine(shapePointList[index], shapePointList[index + 1])
        {
          DistLoad = this.DistLoad
        });
      return subdivide;
    }

    public double[] SlopeIntercept()
    {
      double num1 = (this.End.Y - this.Start.Y) / (this.End.X - this.Start.X);
      double num2 = this.Start.Y - num1 * this.Start.X;
      return new double[2]{ num1, num2 };
    }

    public double XIntercept()
    {
      double[] numArray = this.SlopeIntercept();
      double d = numArray[0];
      double num = numArray[1];
      return !double.IsInfinity(d) ? -num / d : this.Start.X;
    }

    public double[] Parabola_bc(double a)
    {
      double x1 = this.Start.X;
      double y1 = this.Start.Y;
      double x2 = this.End.X;
      double y2 = this.End.Y;
      Vector<double> vector1 = (Vector<double>) CreateVector.Dense<double>(2);
      vector1[0] = y1 - a * Math.Pow(x1, 2.0);
      vector1[1] = y2 - a * Math.Pow(x2, 2.0);
      DenseMatrix denseMatrix = new DenseMatrix(2);
      ((Matrix<double>) denseMatrix)[0, 0] = x1;
      ((Matrix<double>) denseMatrix)[0, 1] = 1.0;
      ((Matrix<double>) denseMatrix)[1, 0] = x2;
      ((Matrix<double>) denseMatrix)[1, 1] = 1.0;
      Vector<double> vector2 =  (denseMatrix).LU().Solve(vector1);
      return new double[2]{ vector2[0], vector2[1] };
    }

    public bool IsPointOnLine(ShapePoint point)
    {
      double[] numArray = this.SlopeIntercept();
      double d = numArray[0];
      double num1 = numArray[1];
      double num2 = this.XIntercept();
      bool flag = false;
      if (Math.Abs(double.IsInfinity(d) ? point.X - num2 : point.Y - d * point.X - num1) < 1E-05 && point.IsBetween(this.Start, this.End))
        flag = true;
      return flag;
    }

    public ShapePoint FindIntersection(ShapeLine that)
    {
      double[] numArray1 = this.SlopeIntercept();
      double[] numArray2 = that.SlopeIntercept();
      double d1 = numArray1[0];
      double num1 = numArray1[1];
      double d2 = numArray2[0];
      double num2 = numArray2[1];
      if (!double.IsInfinity(d1) && !double.IsInfinity(d2))
      {
        DenseMatrix denseMatrix = new DenseMatrix(2);
        ((Matrix<double>) denseMatrix)[0, 0] = -d1;
        ((Matrix<double>) denseMatrix)[0, 1] = 1.0;
        ((Matrix<double>) denseMatrix)[1, 0] = -d2;
        ((Matrix<double>) denseMatrix)[1, 1] = 1.0;
        Vector<double> vector1 = (Vector<double>) CreateVector.Dense<double>(2);
        vector1[0] = num1;
        vector1[1] = num2;
        Vector<double> vector2 = (denseMatrix).LU().Solve(vector1);
        return new ShapePoint(vector2[0], vector2[1]);
      }
      if (double.IsInfinity(d1) && !double.IsInfinity(d2))
      {
        double x = this.XIntercept();
        double y = d2 * x + num2;
        return new ShapePoint(x, y);
      }
      if (double.IsInfinity(d1) || !double.IsInfinity(d2))
        return (ShapePoint) null;
      double x1 = that.XIntercept();
      double y1 = d1 * x1 + num1;
      return new ShapePoint(x1, y1);
    }

    public double AngleBetween(ShapeLine that)
    {
      ShapePoint commonPoint = this.FindCommonPoint(that);
      ShapePoint shapePoint1 = this.End.Clone();
      if (this.Start.IsSame((IElement) commonPoint))
        shapePoint1 = this.End.Clone();
      else if (this.End.IsSame((IElement) commonPoint))
        shapePoint1 = this.Start.Clone();
      double num1 = Math.Atan2(shapePoint1.Y - commonPoint.Y, shapePoint1.X - commonPoint.X) * 180.0 / Math.PI;
      ShapePoint shapePoint2 = that.End.Clone();
      if (that.Start.IsSame((IElement) commonPoint))
        shapePoint2 = that.End.Clone();
      else if (that.End.IsSame((IElement) commonPoint))
        shapePoint2 = that.Start.Clone();
      double num2 = Math.Atan2(shapePoint2.Y - commonPoint.Y, shapePoint2.X - commonPoint.X) * 180.0 / Math.PI;
      return Math.Abs(num1 - num2);
    }

    public ShapePoint FindCommonPoint(ShapeLine line2)
    {
      List<ShapePoint> shapePointList = new List<ShapePoint>();
      shapePointList.Add(this.Start);
      shapePointList.Add(this.End);
      shapePointList.Add(line2.Start);
      shapePointList.Add(line2.End);
      ShapePoint commonPoint = this.Start.Clone();
      for (int index1 = 0; index1 < shapePointList.Count; ++index1)
      {
        for (int index2 = index1 + 1; index2 < shapePointList.Count; ++index2)
        {
          if (shapePointList[index1].IsSame((IElement) shapePointList[index2]))
          {
            commonPoint = shapePointList[index1];
            break;
          }
        }
      }
      return commonPoint;
    }

    public bool HasCommonPoint(ShapeLine line2)
    {
      return this.Start.IsSame((IElement) line2.Start) || this.End.IsSame((IElement) line2.Start) || this.Start.IsSame((IElement) line2.End) || this.End.IsSame((IElement) line2.End);
    }

    public bool HasCommonPoints(ShapeLine line2)
    {
      if (this.Start.IsSame((IElement) line2.Start) && this.End.IsSame((IElement) line2.End))
        return true;
      return this.Start.IsSame((IElement) line2.End) && this.End.IsSame((IElement) line2.Start);
    }

    public List<ShapePoint> ThreePoints(ShapeLine line2)
    {
      List<ShapePoint> shapePointList1 = new List<ShapePoint>();
      shapePointList1.Add(this.Start);
      shapePointList1.Add(this.End);
      shapePointList1.Add(line2.Start);
      shapePointList1.Add(line2.End);
      List<ShapePoint> shapePointList2 = new List<ShapePoint>();
      shapePointList2.Add(shapePointList1[0]);
      for (int index1 = 1; index1 < shapePointList1.Count; ++index1)
      {
        bool flag = false;
        ShapePoint elem = shapePointList1[index1];
        for (int index2 = 0; index2 < shapePointList2.Count; ++index2)
        {
          flag = shapePointList2[index2].IsSame((IElement) elem);
          if (flag)
            break;
        }
        if (!flag)
          shapePointList2.Add(shapePointList1[index1]);
      }
      return shapePointList2;
    }
  }
}
