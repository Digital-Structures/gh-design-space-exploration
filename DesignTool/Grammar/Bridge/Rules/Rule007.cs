namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule adds a deck
    /// </summary>
    public class Rule007 : BaseRule<BridgeShape> 
    {
        public Rule007()
        {
            this.Name = "Rule 07";
            this.Description = "Adds a horizontal deck.";
            this.Params.Add(new DoubleParameter(40, 80, "Length of Deck")); // length of deck
            this.Params.Add(new DoubleParameter(0.3, 0.7, "Centering of Deck")); // centering of deck
            this.Params.Add(new DoubleParameter(35, 45, "Height of Deck")); // height of deck
            this.LHSLabel = BridgeShapeState.MakeDeck;
            this.RHSLabel = BridgeShapeState.MakeInfill;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // First, set parameter values.
            double decklength = (double)p[0];
            double deckalignment = (double)p[1];
            double deckheight = (double)p[2];

            // Next, modify existing shape object.
            double centerX = s.ZeroShapePoint.X + s.Dimensions[0] / 2;
            ShapePoint start = new ShapePoint(centerX - decklength * deckalignment, deckheight);
            ShapePoint end = new ShapePoint(start.X + decklength, deckheight);
            ShapeLine deck = new ShapeLine(start, end);
            deck.DistLoad = 10; // klf
            s.Deck.Add(deck);

            //s.DeckDivs.Add(deck.Start);
            //s.DeckDivs.Add(deck.End);
        }
    }
}
