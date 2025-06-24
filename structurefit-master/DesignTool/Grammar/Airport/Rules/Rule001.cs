using System.Collections.Generic;

namespace StructureEngine.Grammar.Airport.Rules
{
    public class Rule001 : BaseRule<AirportShape>
    {

    // This rule creates the span
        public Rule001()
        {
            this.Name = "Rule 01";
            this.Params.Add(new DoubleParameter(0, 30, "Support Offset")); // how high support may be raised
            this.Params.Add(new EnumParameter(new List<string>() {"Left Support", "Right Support"},"Which Support to Raise")); // which support is raised
            this.LHSLabel = AirportShapeState.Start;
            this.RHSLabel = AirportShapeState.ModifySpan;
        }

        public override void Apply(AirportShape s, object[] p)
        {
            // First, set parameter values.
            double deltay = (double)p[0];
            string support = (string)p[1];

            // Next, modify existing shape object.
            if (support == "Left Support")
            {
                s.Start.Y = s.Start.Y + deltay;
            }
            else if (support == "Right Support")
            {
                s.End.Y = s.End.Y + deltay;
            }
        }
    }
}
