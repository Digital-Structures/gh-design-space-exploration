using System.Collections.Generic;
using StructureEngine.Model;

namespace StructureEngine.Grammar.Bridge
{
    public class BridgeShape : BaseShape
    {
        public BridgeShape()
        {
            this.Infill = new List<ShapeLine>();
            this.Infill2 = new List<ShapeLine>();
            this.Tower = new List<ShapeLine>();
            this.Deck = new List<ShapeLine>();
            this.Fill = new List<ShapeArea>();
            this.Tops = new List<ShapePoint>();
            this.DeckDivs = new List<ShapePoint>();
            this.DeckPoints = new List<ShapePoint>();

            // Set up initial shape
            ShapeLine tower1 = new ShapeLine(new ShapePoint(0, 0), new ShapePoint(0, 20));
            Tower.Add(tower1);
            Tops.Add(tower1.End);

            this.ShapeState = BridgeShapeState.MakeTower;

            ShapeHistory hist = new ShapeHistory();
            this.History = hist;
        }

        public bool IsSuspension;

        public override IGrammar GetGrammar()
        {
            return new BridgeGrammar();
        }

        public override IEnumerator<IElement> GetEnumerator()
        {
            foreach (var e in this.Infill) yield return e;
            foreach (var e in this.Infill2) yield return e;
            foreach (var e in this.Tower) yield return e;
            foreach (var e in this.Deck) yield return e;
            foreach (var e in this.Fill) yield return e;
            // duplicate pointers
            //foreach (var e in this.Tops) yield return e;
            //foreach (var e in this.DeckDivs) yield return e;
        }

        public override Structure ConvertToStructure()
        {
            //List<Node> nodes = new List<Node>();
            //List<Member> members = new List<Member>();
            //Material mat = new Material(1, 1, 1);

            //foreach (ShapeLine line in Lines)
            //{
            //    Node start = new Node(new double[] { line.Start.X, line.Start.Y });
            //    Node end = new Node(new double[] { line.End.X, line.End.Y });
            //    nodes.Add(start);
            //    nodes.Add(end);

            //    Member m = new Member(start, end);
            //    m.Material = mat;
            //    members.Add(m);
            //}

            //Structure s = new Structure(nodes, members);
            Structure s = new Structure();
            return s;
        }

        public override IShape Clone()
        {
            IDesign clone = this.DesignClone();
            return clone as IShape;
        }

        public override IDesign DesignClone()
        {
            var copy = new BridgeShape();
            copy.ShapeState = this.ShapeState;

            List<ShapePoint> top = new List<ShapePoint>();
            List<ShapeLine> tow = new List<ShapeLine>();
            foreach (ShapeLine line in Tower)
            {
                ShapeLine li = line.Clone();
                tow.Add(li);
                if (this.Tops.Contains(line.Start))
                {
                    top.Add(li.Start);
                }
                if (this.Tops.Contains(line.End))
                {
                    top.Add(li.End);
                }
            }
            copy.Tower = tow;
            copy.Tops = top;

            //List<Shape_Point> top = new List<Shape_Point>();
            //foreach (Shape_Point p in Tops)
            //{
            //    Shape_Point pcopy = p.Clone();
            //    top.Add(pcopy);
            //}
            //copy.Tops = top;

            List<ShapePoint> div = new List<ShapePoint>();
            foreach (ShapePoint p in DeckDivs)
            {
                ShapePoint pcopy = p.Clone();
                div.Add(pcopy);
            }
            copy.DeckDivs = div;

            List<ShapeLine> dec = new List<ShapeLine>();
            foreach (ShapeLine line in Deck)
            {
                ShapeLine li = line.Clone();
                dec.Add(li);
            }
            copy.Deck = dec;

            List<ShapeLine> infi = new List<ShapeLine>();
            foreach (ShapeLine line in Infill)
            {
                ShapeLine li = line.Clone();
                infi.Add(li);
            }
            copy.Infill = infi;

            List<ShapeLine> infi2 = new List<ShapeLine>();
            foreach (ShapeLine line in Infill2)
            {
                ShapeLine li = line.Clone();
                infi2.Add(li);
            }
            copy.Infill2 = infi2;

            List<ShapeArea> fi = new List<ShapeArea>();
            foreach (ShapeArea a in Fill)
            {
                ShapeArea ar = a.Clone();
                fi.Add(ar);
            }
            copy.Fill = fi;

            copy = (BridgeShape)BaseClone(copy);

            return copy;
        }

        public override double Score
        {
            get;
            set;
        }

        public override bool LooksSame(IShape that)
        {
            if (that is BridgeShape)
            {
                // N^2 implementation since we can't figure out a good hashcode that works for fuzzy equality

                // make a list (copy)
                var list = new List<IElement>(this);

                // remove elements as we find them
                foreach (IElement elem in (BridgeShape)that)
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

        public void Integrate(BridgeShape s)
        {
            this.Tower.AddRange(s.Tower);
            this.Infill.AddRange(s.Infill);
            this.Infill2.AddRange(s.Infill2);
            this.Deck.AddRange(s.Deck);
            this.Fill.AddRange(s.Areas);
            this.Tops.AddRange(s.Tops);
            this.DeckDivs.AddRange(s.DeckDivs);
            ShapePoint startdeck = this.Deck[0].Start;
            foreach (ShapeLine line in this.Deck)
            {
                if (line.Start.X < startdeck.X)
                {
                    startdeck = line.Start;
                }
                if (line.End.X < startdeck.X)
                {
                    startdeck = line.End;
                }
            }
            ShapePoint enddeck = this.Deck[0].Start;
            foreach (ShapeLine line in this.Deck)
            {
                if (line.Start.X > enddeck.X)
                {
                    enddeck = line.Start;
                }
                if (line.End.X > enddeck.X)
                {
                    enddeck = line.End;
                }
            }
            //this.Deck.Clear();
            //this.Deck.Add(new ShapeLine(startdeck, enddeck));

            List<ShapeLine> newinfill = new List<ShapeLine>();
            foreach (ShapeLine infill in this.Infill)
            {
                if (infill.Start.X > infill.End.X)
                {
                    ShapeLine newinf = new ShapeLine(infill.End, infill.Start);
                    newinfill.Add(newinf);
                }
                else
                {
                    newinfill.Add(infill);
                }
            }
            this.Infill = newinfill;
        }

        public override IShapeState ShapeState
        {
            get;
            set;
        }

        public List<ShapePoint> Tops
        {
            get;
            set;
        }

        //public List<ShapePoint> Bottoms
        //{

        //}

        public List<ShapePoint> DeckDivs
        {
            get;
            set;
        }

        public List<ShapePoint> DeckPoints
        {
            get;
            set;
        }

        public List<ShapeLine> Deck
        {
            get;
            set;
        }

        public List<ShapeLine> Tower
        {
            get;
            set;
        }

        public List<ShapeLine> Infill
        {
            get;
            set;
        }

        public List<ShapeLine> Infill2
        {
            get;
            set;
        }

        public List<ShapeArea> Fill
        {
            get;
            set;
        }

    }
}
