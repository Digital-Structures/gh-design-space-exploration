using System.Collections.Generic;

namespace StructureEngine.Grammar
{
    public class ShapeArea : IElement
    {
        public ShapeArea(List<ShapePoint> p)
        {
            Points = p;
        }

        public List<ShapePoint> Points
        {
            get;
            set;
        }

        IElement IElement.Clone()
        {
            return this.Clone();
        }

        public ShapeArea Clone()
        {
            List<ShapePoint> pcopy = new List<ShapePoint>();
            foreach (ShapePoint point in this.Points)
            {
                ShapePoint pointcopy = point.Clone();
                pcopy.Add(pointcopy);
            }
            ShapeArea copy = new ShapeArea(pcopy);
            return copy;
        }

        public void RotateCenter(double angle, ShapePoint center)
        {
            List<ShapePoint> RotPoints = new List<ShapePoint>();
            foreach (ShapePoint point in this.Points)
            {
                ShapeLine newline = new ShapeLine(center, point);
                newline.Rotate(angle);
                ShapePoint newpoint = newline.End;
                RotPoints.Add(newpoint);
            }
            this.Points = RotPoints;
        }

        public bool IsSame(IElement elem)
        {
            var that = elem as ShapeArea;
            if (elem == null || that == null)
            {
                return false;
            }

            if (this.Points.Count != that.Points.Count)
            {
                return false;
            }
            for (int i = 0; i < this.Points.Count; i++)
            {
                if (!this.Points[i].IsSame(that.Points[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
