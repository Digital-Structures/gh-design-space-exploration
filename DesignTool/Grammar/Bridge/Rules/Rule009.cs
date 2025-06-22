using System.Collections.Generic;

namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule adds the outline of the infill elements.
    /// </summary>
    public class Rule009 : BaseRule<BridgeShape>
    {
        public Rule009()
        {
            this.Name = "Rule 09";
            this.Description = "Adds cable outline.";
            this.LHSLabel = BridgeShapeState.MakeInfill;
            this.RHSLabel = BridgeShapeState.MultipleTowers;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // Next, modify existing shape object.
            ShapePoint deckstart = s.Deck[0].Start;
            ShapePoint deckend = s.Deck[0].End;
            List<ShapePoint> points = new List<ShapePoint>();
            points.Add(deckstart);
            points.AddRange(s.Tops);
            points.Add(deckend);
            for (int i = 0; i < points.Count - 1; i++)
            {
                ShapeLine cable = new ShapeLine(points[i], points[i + 1]);
                s.Infill.Add(cable);
            }
        }
    }
}
