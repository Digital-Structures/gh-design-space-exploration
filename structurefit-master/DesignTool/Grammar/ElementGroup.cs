using System;
using System.Collections.Generic;
using System.Linq;

namespace StructureEngine.Grammar
{
    /// <summary>
    /// TODO: all of this stuff is stateless - convert to extension methods!?
    /// </summary>
    public abstract class ElementGroup : IEnumerable<IElement>, IElementGroup
    {
        public IEnumerable<ShapePoint> Points
        {
            //get { return this.OfType<Shape_Point>(); }
            get
            {
                // N log N or N^2 slow?
                // works on *value* comparison, once defined
                return this.Lines.SelectMany(l => new[] { l.Start, l.End })
                    .Concat(this.Areas.SelectMany(a => a.Points))
                    .Distinct();
            }
        }

        public IEnumerable<ShapeLine> Lines
        {
            get { return this.OfType<ShapeLine>(); }
        }
        public IEnumerable<ShapeArea> Areas
        {
            get { return this.OfType<ShapeArea>(); }
        }

        public void Rotate(double angle, ShapePoint center)
        {
            foreach (ShapePoint p in this.Points)
            {
                p.RotateAbout(angle, center);
            }
        }

        public void Translate(double x, double y)
        {
            foreach (ShapePoint p in this.Points)
            {
                p.X += x;
                p.Y += y;
            }
        }

        public virtual void Scale(double scalex, double scaley)
        {
            double x0 = this.ZeroShapePoint.X;
            double y0 = this.ZeroShapePoint.Y;
            ShapePoint pzero = this.ZeroShapePoint;

            foreach (ShapePoint p in this.Points)
            {
                ShapeLine temp = new ShapeLine(pzero, p);
                ShapeLine tempx = temp.Clone();
                tempx.Scale(scalex);
                ShapeLine tempy = temp.Clone();
                tempy.Scale(scaley);
                p.X = tempx.End.X;
                p.Y = tempy.End.Y;
            }

            double deltax = -this.ZeroShapePoint.X + x0;
            double deltay = -this.ZeroShapePoint.Y + y0;

            this.Translate(deltax, deltay);
        }

        public ShapePoint ZeroShapePoint
        {
            get
            {
                // TODO: reasonable defaults when there are no lines?
                double minx = 0, miny = 0;
                foreach (ShapeLine l in this.Lines)
                {
                    minx = Math.Min(minx, l.Start.X);
                    minx = Math.Min(minx, l.End.X);
                    miny = Math.Min(miny, l.Start.Y);
                    miny = Math.Min(miny, l.End.Y);
                }
                return new ShapePoint(minx, miny);
            }
        }
        
        public double[] Dimensions
        {
            get
            {
                // TODO: reasonable defaults when there are no lines?
                double maxx = 0, maxy = 0;
                foreach (ShapeLine l in this.Lines)
                {
                    maxx = Math.Max(maxx, l.Start.X);
                    maxx = Math.Max(maxx, l.End.X);
                    maxy = Math.Max(maxy, l.Start.Y);
                    maxy = Math.Max(maxy, l.End.Y);
                }
                double[] dim = new double[2];
                dim[0] = maxx - ZeroShapePoint.X;
                dim[1] = maxy - ZeroShapePoint.Y;
                return dim;
            }
        }

        #region IEnumerable

        public abstract IEnumerator<IElement> GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            var enumer = this.GetEnumerator();
            while (enumer.MoveNext())
            {
                yield return enumer.Current;
            }
        }

        #endregion
    }
}
