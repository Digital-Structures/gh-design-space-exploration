
namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule removes vertical support cables.
    /// </summary>
    public class Rule013 : BaseRule<BridgeShape>
    {
        public Rule013()
        {
            this.Name = "Rule 13";
            this.Description = "Removes cables supporting the deck.";
            this.Params.Add(new IntParameter(0, 40, "Support to Remove"));
            this.LHSLabel = BridgeShapeState.ModifySupports;
            this.RHSLabel = BridgeShapeState.ModifySupports;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // First, set parameter values.
            int num = (int)p[0];

            // Next, modify existing shape object.
            if (s.Infill2.Count > 0)
            {
                int index = num % s.Infill2.Count;
                ShapeLine line = s.Infill2[index];
                s.Infill2.Remove(line);

                // combine deck elements
                ShapePoint point = line.Start;
                ShapeLine remove1 = null;
                ShapeLine remove2 = null;
                ShapeLine newDeck = null;
                int deckIndex = 0;

                for (int i = 0; i < s.Deck.Count; i++)
                {
                    if (s.Deck[i].End.IsSame(point) && s.Deck[i + 1].Start.IsSame(point))
                    {
                        remove1 = s.Deck[i];
                        remove2 = s.Deck[i + 1];
                        newDeck = new ShapeLine(s.Deck[i].Start, s.Deck[i+1].End);
                        deckIndex = i;
                        break;
                    }
                }

                s.Deck.Insert(deckIndex, newDeck);
                s.Deck.Remove(remove1);
                s.Deck.Remove(remove2);
            }
        }
    }
}
