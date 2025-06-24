using System.Collections.Generic;

namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule subdivides the deck
    /// </summary>
    public class Rule011 : BaseRule<BridgeShape>
    {
        public Rule011()
        {
            this.Name = "Rule 11";
            this.Description = "Divides the deck.";
            this.Params.Add(new IntParameter(4, 20, "Number of Deck Subdivisions")); // number of subdivisions of deck
            this.LHSLabel = BridgeShapeState.Subdivide;
            this.RHSLabel = BridgeShapeState.AddSupports;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // First, set parameter values.
            int numdiv = (int)p[0];

            // Next, modify existing shape object.
            List<ShapePoint> allpoints = new List<ShapePoint>();
            List<ShapeLine> alldecks = new List<ShapeLine>();
            foreach (ShapeLine deck in s.Deck)
            {
                List<ShapePoint> points = deck.Subdivide(numdiv);

                if (s.Deck.IndexOf(deck) != 0)
                {
                    allpoints.Add(deck.Start);
                }
                
                allpoints.AddRange(points);

                if (numdiv > 1)
                {
                    alldecks.Add(new ShapeLine(deck.Start, points[0]));
                    for (int i = 0; i < (points.Count - 1); i++)
                    {
                        ShapeLine seg = new ShapeLine(points[i], points[i + 1]);
                        alldecks.Add(seg);
                    }
                    alldecks.Add(new ShapeLine(points[points.Count - 1], deck.End));
                }

                else if (numdiv == 1)
                {
                    points.Add(deck.Start);
                    alldecks.Add(new ShapeLine(deck.Start, deck.End));
                }

                //if (deck == s.Deck[s.Deck.Count - 1])
                //{
                //    allpoints.Add(deck.End);
                //}
                //allpoints.RemoveAt(0);
            }
            s.Deck = alldecks;
            s.DeckDivs = allpoints;
            s.DeckPoints = s.DeckDivs.GetRange(0, s.DeckDivs.Count);
        }
    }
}
