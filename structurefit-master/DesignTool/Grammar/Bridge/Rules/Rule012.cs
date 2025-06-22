using System.Collections.Generic;

namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule add supports for deck at subdivision points.
    /// </summary>
    public class Rule012 : BaseRule<BridgeShape>
    {
        public Rule012()
        {
            this.Name = "Rule 12";
            this.Description = "Adds support cables at deck subdivision points.";
            this.Params.Add(new DoubleParameter(80, 100, "Angle of Support Cables")); // angle from vertical of supports
            this.LHSLabel = BridgeShapeState.AddSupports;
            this.RHSLabel = BridgeShapeState.ModifySupports;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // First, set parameter values.
            double angle = (double)p[0];

            // Next, modify existing shape object.
            foreach (ShapePoint point in s.DeckDivs)
            {
                ShapeLine line = new ShapeLine(point, angle, 20);
                s.Infill2.Add(line);
            }

            s.DeckPoints.Clear();
        }
    }
}
