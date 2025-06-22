using System.Collections.Generic;

namespace StructureEngine.Grammar
{
    public interface IElementGroup
    {
        IEnumerable<ShapePoint> Points
        {
            get;
        }

        IEnumerable<ShapeLine> Lines
        {
            get;
        }

        IEnumerable<ShapeArea> Areas
        {
            get;
        }

        ShapePoint ZeroShapePoint
        {
            get;
        }

        double[] Dimensions
        {
            get;
        }

        void Rotate(double angle, ShapePoint center);
        void Translate(double x, double y);
        void Scale(double scalex, double scaley);
    }
}
