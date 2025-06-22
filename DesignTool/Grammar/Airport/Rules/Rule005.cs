using System.Collections.Generic;

namespace StructureEngine.Grammar.Airport.Rules
{
    public class Rule005 : BaseRule<AirportShape>
    {
        // This rule tilts supports
        public Rule005()
        {
            this.Name = "Rule 05";
            this.Params.Add(new DoubleParameter(10, 40, "Angle of Tilt")); // angle of support
            this.Params.Add(new EnumParameter(new List<string>() {"Left Support", "Right Support"}, "Which Support to Tilt")); // which support to tilt
            this.LHSLabel = AirportShapeState.ModifyVerticals;
            this.RHSLabel = AirportShapeState.End;
        }

        public override void Apply(AirportShape s, object[] p)
        {
            // First, set parameter values.
            double angle = (double)p[0];
            string support = (string)p[1];

            // Next, modify existing shape object.
            ShapeLine vert;
            if (support == "Left Support")
            {
                vert = s.Verticals[0];
            }
            else
            {
                vert = s.Verticals[1];
            }
            vert.Start.RotateAbout(angle, vert.End);

            // TODO: scale structure to have same overall span
        }
    }
}
