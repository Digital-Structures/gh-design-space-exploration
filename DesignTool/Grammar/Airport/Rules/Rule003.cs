using System.Collections.Generic;

namespace StructureEngine.Grammar.Airport.Rules
{
    public class Rule003 : BaseRule<AirportShape>
    {
        // This rule mirrors the existing catenary
        public Rule003()
        {
            this.Name = "Rule 03";
            this.LHSLabel = AirportShapeState.AddVerticals;
            this.RHSLabel = AirportShapeState.End;
        }

        public override void Apply(AirportShape s, object[] p)
        {
            // Next, modify existing shape object.
            var mirrorPoints = new List<ShapePoint>();
            var myPoints = s.Points;

            foreach (ShapePoint pt in myPoints)
            {
                ShapePoint mir = pt.ReflectAcross(new ShapeLine(s.Start, s.End));
                mirrorPoints.Add(mir);
            }

            var mirrorLines = new List<ShapeLine>();
            for (int i = 0; i < mirrorPoints.Count - 1; i++)
            {
                ShapeLine l = new ShapeLine(mirrorPoints[i], mirrorPoints[i + 1]);
                mirrorLines.Add(l);
            }

            s.Roof.AddRange(mirrorLines);
        }
    }
}
