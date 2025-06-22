using System.Collections.Generic;
using StructureEngine.Model;

namespace StructureEngine.Grammar.Airport
{
    public class AirportShape : BaseShape
    {
        public AirportShape()
        {
            // Initialize lists of elements
            this.Roof = new List<ShapeLine>();
            this.Verticals = new List<ShapeLine>();

            // Set up initial shape
            // TODO: replace 100-ft span with something hooked up to user control
            ShapeLine span = new ShapeLine(new ShapePoint(0, 0), new ShapePoint(100, 0));
            this.Roof.Add(span);
            this.Start = span.Start;
            this.End = span.End;

            this.ShapeState = AirportShapeState.Start;

            ShapeHistory hist = new ShapeHistory();
            this.History = hist;
        }

        public override IGrammar GetGrammar()
        {
            return new AirportGrammar();
        }

        public override IEnumerator<IElement> GetEnumerator()
        {
            foreach (var e in this.Roof) yield return e;
            foreach (var e in this.Verticals) yield return e;
            // etc.
        }

        public override Structure ConvertToStructure()
        {
            List<Node> nodes = new List<Node>();
            List<Member> members = new List<Member>();
            Structure s = new Structure(nodes, members);
            return s;
        }

        public override IShape Clone()
        {
            IDesign clone = this.DesignClone();
            return clone as IShape;
        }

        public override IDesign DesignClone()
        {
            var copy = new AirportShape();
            copy.ShapeState = this.ShapeState;

            copy.Start = this.Start.Clone();
            copy.End = this.End.Clone();

            List<ShapeLine> r = new List<ShapeLine>();
            foreach (ShapeLine l in this.Roof)
            {
                r.Add(l);
            }
            copy.Roof = r;

            List<ShapeLine> v = new List<ShapeLine>();
            foreach (ShapeLine l in this.Verticals)
            {
                v.Add(l);
            }
            copy.Verticals = v;

            copy = (AirportShape)BaseClone(copy);

            return copy;
        }

        public override bool LooksSame(IShape that)
        {
            if (that is AirportShape)
            {
                // N^2 implementation since we can't figure out a good hashcode that works for fuzzy equality

                // make a list (copy)
                var list = new List<IElement>(this);

                // remove elements as we find them
                foreach (IElement elem in (AirportShape)that)
                {
                    bool found = false;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].IsSame(elem))
                        {
                            found = true;
                            list.RemoveAt(i);
                            break;
                        }
                    }
                    // not found: not equal
                    if (!found)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public override IShapeState ShapeState
        {
            get;
            set;
        }

        public override double Score
        {
            get;
            set;
        }

        public List<ShapeLine> Roof
        {
            get;
            set;
        }

        public List<ShapeLine> Verticals
        {
            get;
            set;
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

    }
}
