using StructureEngine.Model;
using System.Collections.Generic;

namespace StructureEngine.Grammar
{
    public interface IShape : IElementGroup, IDesign
    {
        Structure ConvertToStructure();

        IShape Clone();

        bool LooksSame(IShape that);

        ShapeHistory History
        {
            get;
            set;
        }

        IShape Parent1
        {
            get;
            set;
        }

        IShape Parent2
        {
            get;
            set;
        }

        int SplicePoint1
        {
            get;
            set;
        }

        int SplicePoint2
        {
            get;
            set;
        }

        new double Score
        {
            get;
            set;
        }

        IShapeState ShapeState
        {
            get;
            set;
        }

        IShape GoBack();

        IGrammar GetGrammar();

    }
}
