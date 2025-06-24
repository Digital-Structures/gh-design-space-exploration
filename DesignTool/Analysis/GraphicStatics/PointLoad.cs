using StructureEngine.Grammar;

namespace StructureEngine.GraphicStatics
{
    public class PointLoad
    {
        public PointLoad(ShapePoint p, double l)
        {
            this.Point = p;
            this.Load = l;
            this.Rotation = 270;
        }

        public PointLoad(ShapePoint p, double l, double r)
        {
            this.Point = p;
            this.Load = l;
            this.Rotation = r;
        }

        public ShapePoint Point;

        public double Load;

        public double Rotation;
    }
}
