using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace StructureEngine.Grammar
{
    public class ShapeLine : IElement
    {
        public ShapeLine(ShapePoint i, ShapePoint j)
        {
            this.Start = i;
            this.End = j;
            List<ShapePointLoad> loads = new List<ShapePointLoad>();
            this.Loads = loads;
        }

        // TODO: this is wrong, rotation is correct!!
        public ShapeLine(ShapePoint i, double angle, double length) 
        {
            this.Start = i;
            double angleRad = angle * Math.PI / 180;
            double end_x = i.X + length * Math.Cos(angleRad);
            double end_y = i.Y + length * Math.Sin(angleRad);
            this.End = new ShapePoint(end_x, end_y);
            List<ShapePointLoad> loads = new List<ShapePointLoad>();
            this.Loads = loads;
        }

        public ShapePoint Start
        {
            get;
            set;
        }

        public ShapePoint End
        {
            get;
            set;
        }

        public ShapePoint MidPoint
        {
            get
            {
                ShapeLine sl = new ShapeLine(this.Start, this.Rotation, this.Length * 0.5);
                return sl.End;
            }
        }

        public ShapePoint Joint
        {
            get;
            set;
        }

        public List<ShapePointLoad> Loads
        {
            get;
            set;
        }

        public double Length
        {
            get
            {
                return Start.GetDistance(End);
            }
        }

        public double DistLoad
        {
            get;
            set;
        }

        public double AxialForce
        {
            get;
            set;
        }

        public double VerticalForce
        {
            get;
            set;
        }

        public double HorizontalForce
        {
            get;
            set;
        }

        public double GetAxialForce()
        {
            if (this.VerticalForce == 0 && this.HorizontalForce == 0)
            {
                return 0;
            }
            else if (this.VerticalForce != 0 && this.HorizontalForce == 0)
            {
                return Math.Abs(this.VerticalForce / Math.Sin(Math.PI * this.Rotation / 180.0));
            }
            else if (this.VerticalForce == 0 && this.HorizontalForce != 0)
            {
                return Math.Abs(this.HorizontalForce / Math.Cos(Math.PI * this.Rotation / 180.0));
            }
            else
            {
                return Math.Sqrt(Math.Pow(this.HorizontalForce, 2) + Math.Pow(this.VerticalForce, 2));
            }
        }

        public double BendingMoment
        {
            get;
            set;
        }

        public double Rotation
        {
            get
            {
                return this.End.GetAngle(this.Start);
            }
        }

        public double ReqThickness
        {
            get;
            set;
        }

        public double ReqArea
        {
            get;
            set;
        }

        IElement IElement.Clone()
        {
            return this.Clone();
        }

        public ShapeLine Clone()
        {
            ShapeLine copy = new ShapeLine(this.Start.Clone(), this.End.Clone());
            List<ShapePointLoad> list = new List<ShapePointLoad>();
            foreach (ShapePointLoad pl in this.Loads)
            {
                ShapePointLoad plcopy = pl.Clone();
                list.Add(plcopy);
            }
            copy.Loads = list;
            copy.AxialForce = this.AxialForce;
            copy.BendingMoment = this.BendingMoment;
            copy.ReqArea = this.ReqArea;
            copy.ReqThickness = this.ReqThickness;
            copy.DistLoad = this.DistLoad;
            return copy;
        }

        public bool IsSame(IElement elem)
        {
            var that = elem as ShapeLine;
            if (elem == null || that == null)
            {
                return false;
            }

            return (this.Start.IsSame(that.Start) && this.End.IsSame(that.End));
        }

        public void Copy(ShapeLine l)
        {
            this.Start.Copy(l.Start);
            this.End.Copy(l.End);
        }

        public void Scale(double scale)
        {
            ShapeLine newsl = new ShapeLine(this.Start, this.Rotation, this.Length * scale);
            this.Copy(newsl);
        }

        public void Rotate(double angle)
        {
            this.End.RotateAbout(angle, this.Start);
        }

        public void RotateCenter(double angle, ShapePoint center)
        {
            this.Start.RotateAbout(angle, center);
            this.End.RotateAbout(angle, center);
        }

        public void ReverseLine()
        {
            ShapePoint newstart = this.End;
            ShapePoint newend = this.Start;
            this.Start = newstart;
            this.End = newend;
        }

        public List<ShapePoint> Subdivide(int num)
        {
            List<ShapePoint> points = new List<ShapePoint>();
            //points.Add(this.Start);
            if (num > 1)
            {
                double inc = this.Length / (double)num;
                double angleRad = this.Rotation * Math.PI / 180;

                ShapePoint last = this.Start;
                for (int i = 0; i < num - 1; i++)
                {
                    ShapePoint next = new ShapePoint(last.X + inc * Math.Cos(angleRad) , last.Y + inc * Math.Sin(angleRad));
                    points.Add(next);
                    last = next;
                }
            }
            //points.Add(this.End);
            return points;
        }

        public List<ShapeLine> GetSubdivide(int num)
        {
            List<ShapeLine> lines = new List<ShapeLine>();
            List<ShapePoint> p = this.Subdivide(num);
            p.Insert(0, this.Start);
            p.Add(this.End);

            for (int i = 0; i < num; i++)
            {
                var l = new ShapeLine(p[i], p[i + 1]);
                l.DistLoad = this.DistLoad;
                lines.Add(l);
            }

            return lines;
        }

        public double[] SlopeIntercept()
        {
            double m = (this.End.Y - this.Start.Y) / (this.End.X - this.Start.X);
            double b = this.Start.Y - m * this.Start.X;
            return new double[] { m, b };
        }

        public double XIntercept()
        {
            double[] mb = this.SlopeIntercept();
            double m = mb[0];
            double b = mb[1];
            double a;

            if (double.IsInfinity(m))
            {
                a = this.Start.X;
            }

            else
            {
                a = -b / m;
            }

            return a;
        }

        public double[] Parabola_bc(double a)
        {
            // y = ax^2 + bx + c; given a, find b and c
            double x1 = this.Start.X;
            double y1 = this.Start.Y;
            double x2 = this.End.X;
            double y2 = this.End.Y;
            Vector<double> b = new DenseVector(2);
            b[0] = y1 - a * Math.Pow(x1, 2);
            b[1] = y2 - a * Math.Pow(x2, 2);
            Matrix<double> A = new DenseMatrix(2);
            A[0, 0] = x1;
            A[0, 1] = 1;
            A[1, 0] = x2;
            A[1, 1] = 1;
            Vector<double> BC = new DenseLU((DenseMatrix)A).Solve(b);
            double[] bc = new double[] { BC[0], BC[1] };
            return bc;
        }

        public bool IsPointOnLine(ShapePoint point)
        {
            double[] mb = this.SlopeIntercept();
            double m = mb[0];
            double b = mb[1];
            double x = this.XIntercept();
            bool check = false;
            double zero = 0;
            if (!double.IsInfinity(m))
            {
                zero = point.Y - m * point.X - b;
            }
            else
            {
                zero = point.X - x;
            }

            if (Math.Abs(zero) < 0.00001)
            {
                if (point.IsBetween(this.Start, this.End))
                {
                    check = true;
                }
            }
            return check;
        }

        public ShapePoint FindIntersection(ShapeLine that)
        {
            double[] mb_this = this.SlopeIntercept();
            double[] mb_that = that.SlopeIntercept();
            double m1 = mb_this[0];
            double b1 = mb_this[1];
            double m2 = mb_that[0];
            double b2 = mb_that[1];
            if (!double.IsInfinity(m1) && !double.IsInfinity(m2))
            {

                Matrix<double> system = new DenseMatrix(2);
                system[0, 0] = -m1;
                system[0, 1] = 1;
                system[1, 0] = -m2;
                system[1, 1] = 1;
                Vector<double> b = new DenseVector(2);
                b[0] = b1;
                b[1] = b2;
                Vector<double> x = new DenseLU((DenseMatrix)system).Solve(b);
                return new ShapePoint(x[0], x[1]);
            }
            else if (double.IsInfinity(m1) && !double.IsInfinity(m2))
            {
                double a1 = this.XIntercept();
                double x = a1;
                double y = m2 * x + b2;
                return new ShapePoint(x, y);
            }
            else if (!double.IsInfinity(m1) && double.IsInfinity(m2))
            {
                double a2 = that.XIntercept();
                double x = a2;
                double y = m1 * x + b1;
                return new ShapePoint(x, y);
            }
            else
            {
                return null;
            }
        }

        public double AngleBetween(ShapeLine that)
        {
            ShapePoint common = FindCommonPoint(that);
            ShapePoint thispoint = this.End.Clone();
            if (this.Start.IsSame(common))
            {
                thispoint = this.End.Clone();
            }
            else if (this.End.IsSame(common))
            {
                thispoint = this.Start.Clone();
            }
            double thisangle = Math.Atan2(thispoint.Y - common.Y, thispoint.X - common.X);
            thisangle = (thisangle * 180) / Math.PI;

            ShapePoint thatpoint = that.End.Clone();
            if (that.Start.IsSame(common))
            {
                thatpoint = that.End.Clone();
            }
            else if (that.End.IsSame(common))
            {
                thatpoint = that.Start.Clone();
            }
            double thatangle = Math.Atan2(thatpoint.Y - common.Y, thatpoint.X - common.X);
            thatangle = (thatangle * 180) / Math.PI;

            return Math.Abs(thisangle - thatangle);
        }



        //public bool SamePoint(double[] point1, double[] point2)
        //{
        //    bool issame = false;
        //    if (point1[0] == point2[0] && point1[1] == point2[1])
        //    {
        //        issame = true;
        //    }
        //    return issame;
        //}

        public ShapePoint FindCommonPoint(ShapeLine line2)
        {
            List<ShapePoint> points = new List<ShapePoint>();
            points.Add(this.Start);
            points.Add(this.End);
            points.Add(line2.Start);
            points.Add(line2.End);

            ShapePoint same = this.Start.Clone();
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    if (points[i].IsSame(points[j]))
                    {
                        same = points[i];
                        break;
                    }
                }
            }
            return same;
        }

        public bool HasCommonPoint(ShapeLine line2)
        {
            //List<ShapePoint> points = new List<ShapePoint>();
            //points.Add(this.Start);
            //points.Add(this.End);
            //points.Add(line2.Start);
            //points.Add(line2.End);

            //bool common = false;

            //for (int i = 0; i < points.Count; i++)
            //{
            //    for (int j = i + 1; j < points.Count; j++)
            //    {
            //        if (points[i].IsSame(points[j]))
            //        {
            //            common = true;
            //            break;
            //        }
            //    }
            //}
            //return common;
            return (this.Start.IsSame(line2.Start) || this.End.IsSame(line2.Start) || this.Start.IsSame(line2.End) || this.End.IsSame(line2.End));
        }

        public bool HasCommonPoints(ShapeLine line2)
        {
            return ((this.Start.IsSame(line2.Start) && this.End.IsSame(line2.End)) || (this.Start.IsSame(line2.End) && this.End.IsSame(line2.Start)));
        }

        public List<ShapePoint> ThreePoints(ShapeLine line2)
        {
            List<ShapePoint> points = new List<ShapePoint>();
            points.Add(this.Start);
            points.Add(this.End);
            points.Add(line2.Start);
            points.Add(line2.End);

            List<ShapePoint> distinctpoints = new List<ShapePoint>();
            distinctpoints.Add(points[0]);
            for (int k = 1; k < points.Count; k++)
            {
                bool issame = false;
                ShapePoint nextpoint = points[k];
                for (int i = 0; i < distinctpoints.Count; i++)
                {
                    issame = distinctpoints[i].IsSame(nextpoint);
                    if (issame)
                    {
                        break;
                    }
                }
                if (!issame)
                {
                    distinctpoints.Add(points[k]);
                }
            }

            return distinctpoints;
        }
    }

    public class ByXLineComparer : IComparer<ShapeLine>
    {
        public int Compare(ShapeLine l1, ShapeLine l2)
        {
            int compareResult = l1.Start.X.CompareTo(l2.Start.X);
            return compareResult;
        }
    }
}
