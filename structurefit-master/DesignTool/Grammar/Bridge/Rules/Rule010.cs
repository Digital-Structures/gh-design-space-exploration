namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule adds a second tower based on the first.
    /// </summary>
    public class Rule010 : BaseRule<BridgeShape>
    {
        public Rule010()
        {
            this.Name = "Rule 10";
            this.Description = "Adds a second tower based on the first.";
            this.Params.Add(new IntParameter(0, 1, "Whether the Tower is Mirrored")); // whether the new tower is mirrored or not
            this.Params.Add(new DoubleParameter(.5, 1.5, "Horizontal Scale Factor")); // horizontal scaling
            this.LHSLabel = BridgeShapeState.MultipleTowers;
            this.RHSLabel = BridgeShapeState.Subdivide;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // First, set parameter values.
            int reflect = (int)p[0];
            double xscale = (double)p[1];

            // Next, modify existing shape object.
            BridgeShape copy = (BridgeShape)s.Clone();
            if (reflect == 0)
            {
                copy.Scale(xscale, 1);
            }
            else if (reflect == 1)
            {
                copy.Scale(-xscale, 1);
                copy.Infill.Reverse();
                copy.Infill2.Reverse();
                copy.Deck.Reverse();
                foreach (ShapeLine deck in copy.Deck)
                {
                    deck.ReverseLine();
                }
            }

            //List<Shape_Line> alllines = new List<Shape_Line>();
            //alllines.AddRange(s.Tower);
            //alllines.AddRange(s.Deck);

            double deckstart = copy.Deck[0].Start.X;
            foreach (ShapeLine l in copy.Deck)
            {
                if (l.Start.X < deckstart)
                {
                    deckstart = l.Start.X;
                }
                else if (l.End.X < deckstart)
                {
                    deckstart = l.End.X;
                }
            }

            double deckend = s.Deck[s.Deck.Count - 1].End.X;
            foreach (ShapeLine l in s.Deck)
            {
                if (l.Start.X > deckend)
                {
                    deckend = l.Start.X;
                }
                else if (l.End.X > deckend)
                {
                    deckend = l.End.X;
                }
            }

            double x = deckend - deckstart;
            copy.Translate(x, 0);
            //copy.Translate(s.Dimensions[0], 0);
            s.Integrate(copy);
        }
    }
}
