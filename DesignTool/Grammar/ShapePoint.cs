using System;

namespace StructureEngine.Grammar
{
    public class ShapePoint : IElement
    {
        public ShapePoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X
        {
            get;
            set;
        }

        public double Y
        {
            get;
            set;
        }

        //public double[] coords
        //{
        //    get
        //    {
        //        return new double[] { X, Y };
        //    }
        //    set
        //    {

        //    }
        //}

        public double GetDistance(ShapePoint p2)
        {
            double dist = Math.Sqrt(Math.Pow(this.X - p2.X, 2) + Math.Pow(this.Y - p2.Y, 2));
            return dist;
        }

        public double GetAngle(ShapePoint start)
        {
            double height = this.Y - start.Y;
            double width = this.X - start.X;
            double angleRad = Math.Atan2(height, width);
            return angleRad * 180 / Math.PI; // returns angle in degrees;
        }

        public bool IsSame(IElement elem)
        {
            var that = elem as ShapePoint;
            if (elem == null || that == null)
            {
                return false;
            }
            double zerox = Math.Abs(this.X - that.X);
            double zeroy = Math.Abs(this.Y - that.Y);

            return (zerox < 0.001 && zeroy < 0.001);
        }

        public void RotateAbout(double angle, ShapePoint center)
        {
            double l = this.GetDistance(center);
            double theta = this.GetAngle(center);
            double totalangle = theta + angle;
            double x = l * Math.Cos(totalangle * Math.PI / 180);
            double y = l * Math.Sin(totalangle * Math.PI / 180);
            this.X = center.X + x;
            this.Y = center.Y + y;
        }

        public double[] RotateCoords(ShapePoint origin, double angle)
        {
            double l = this.GetDistance(origin);
            double x = l * Math.Cos(Math.PI * angle / 180);
            double y = l * Math.Sin(Math.PI * angle / 180);
            return new double[] { x, y };
        }

        IElement IElement.Clone()
        {
            return this.Clone();
        }

        public ShapePoint Clone()
        {
            ShapePoint copy = new ShapePoint(this.X, this.Y);
            return copy;
        }

        public void Copy(ShapePoint p)
        {
            this.X = p.X;
            this.Y = p.Y;
        }

        public bool IsBetween(ShapePoint p1, ShapePoint p2)
        {
            bool check = false;
            double minX = p1.X;
            double maxX = p2.X;
            if (maxX < minX)
            {
                minX = p2.X;
                maxX = p1.X;
            }
            double minY = p1.Y;
            double maxY = p2.Y;
            if (maxY < minY)
            {
                minY = p2.Y;
                maxY = p1.Y;
            }
            if ((this.X < maxX || Math.Abs(this.X - maxX) < 0.0001) && (this.X > minX || (Math.Abs(this.X - minX) < 0.0001)))
            {
                if (this.Y <= maxY && this.Y >= minY)
                {
                    check = true;
                }
            }
            return check;
        }

        public ShapePoint GetProjection(ShapeLine l)
        {
            double[] mb1 = l.SlopeIntercept();
            double m1 = mb1[0];
            double b1 = mb1[1];

            double x;
            double y;

            if (m1 != 0)
            {
                double m2 = -1 / m1;
                double b2 = this.Y - m2 * this.X;

                x = (b2 - b1) / (m1 - m2);
                y = m1 * x + b1;
            }
            else
            {
                x = this.X;
                y = b1;
            }

            ShapePoint that = new ShapePoint(x, y);
            return that;
        }

        public ShapeLine GetProjectionPath(ShapeLine l)
        {
            ShapePoint that = this.GetProjection(l);
            return new ShapeLine(this, that);
        }

        public ShapePoint ReflectAcross(ShapeLine l)
        {
            ShapeLine path1 = this.GetProjectionPath(l);
            ShapeLine path2 = new ShapeLine(this, path1.Rotation, 2 * path1.Length);

            ShapePoint that = path2.End;
            return that;
        }
    }
}
