using System.Collections.Generic;

namespace StructureEngine.Grammar
{
    public class ShapePointLoad
    {
        public ShapePointLoad(ShapePoint p, double magnitude, double direction)
        {
            this.Point = p;
            this.Magnitude = magnitude;
            this.Direction = direction;
        }

        public ShapePoint Point
        {
            get;
            set;
        }

        public double Magnitude
        {
            get;
            set;
        }

        public double Direction
        {
            get;
            set;
        }

        public ShapePointLoad Clone()
        {
            ShapePoint pcopy = this.Point.Clone();
            ShapePointLoad copy = new ShapePointLoad(pcopy, this.Magnitude, this.Direction);
            return copy;
        }

    }

    public class ByXCoordComparer : IComparer<ShapePointLoad>
    {
        public int Compare(ShapePointLoad l1, ShapePointLoad l2)
        {
            int compareResult = l1.Point.X.CompareTo(l2.Point.X);
            return compareResult;
        }
    }
}
