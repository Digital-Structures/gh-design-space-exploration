using System.Collections.Generic;
using StructureEngine.Model;

namespace StructureEngine.Grammar.Simple
{
    public class SimpleShape : BaseShape
    {
        public SimpleShape()
        {
            // Initialize lists of elements
            this.Horizontal = new List<ShapeLine>();
            this.Verticals = new List<ShapeLine>();
            this.Funicular = new List<ShapeLine>();

            // Set up initial shape
            // TODO: replace 100-ft span with something hooked up to user control
            ShapeLine span = new ShapeLine(new ShapePoint(0, 0), new ShapePoint(100, 0));
            span.DistLoad = 1; // klf
            this.Horizontal.Add(span);
            this.Start = span.Start;
            this.End = span.End;

            this.ShapeState = SimpleShapeState.Start;

            ShapeHistory hist = new ShapeHistory();
            this.History = hist;
        }

        public override IGrammar GetGrammar()
        {
            return new SimpleGrammar();
        }

        public override bool LooksSame(IShape that)
        {
            if (that is SimpleShape)
            {
                // N^2 implementation since we can't figure out a good hashcode that works for fuzzy equality

                // make a list (copy)
                var list = new List<IElement>(this);

                // remove elements as we find them
                foreach (IElement elem in (SimpleShape)that)
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

        public override IEnumerator<IElement> GetEnumerator()
        {
            foreach (var e in this.Horizontal) yield return e;
            foreach (var e in this.Verticals) yield return e;
            foreach (var e in this.Funicular) yield return e;
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
            var copy = new SimpleShape();
            copy.ShapeState = this.ShapeState;

            copy.Start = this.Start.Clone();
            copy.End = this.End.Clone();

            List<ShapeLine> h = new List<ShapeLine>();
            foreach (ShapeLine l in this.Horizontal)
            {
                h.Add(l.Clone());
            }
            copy.Horizontal = h;

            List<ShapeLine> v = new List<ShapeLine>();
            foreach (ShapeLine l in this.Verticals)
            {
                v.Add(l.Clone());
            }
            copy.Verticals = v;

            List<ShapeLine> f = new List<ShapeLine>();
            foreach (ShapeLine l in this.Funicular)
            {
                f.Add(l.Clone());
            }
            copy.Funicular = f;

            copy = (SimpleShape)BaseClone(copy);

            return copy;
        }

        public override IShapeState ShapeState
        {
            get;
            set;
        }

        public List<ShapeLine> Funicular
        {
            get;
            set;
        }

        public List<ShapeLine> Verticals
        {
            get;
            set;
        }

        public List<ShapeLine> Horizontal
        {
            get;
            set;
        }

        public List<ShapePoint> HorPoints
        {
            get
            {
                List<ShapePoint> d = new List<ShapePoint>();
                foreach (ShapeLine l in this.Horizontal)
                {
                    if (!d.Contains(l.Start))
                    {
                        d.Add(l.Start);
                    }
                    if (!d.Contains(l.End))
                    {
                        d.Add(l.End);
                    }
                }

                return d;
            }
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

        //private double _Score;
        public override double Score
        {
            get;
            set;
        }

    }
}
