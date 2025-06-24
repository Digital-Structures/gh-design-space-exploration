using System.Collections.Generic;

namespace StructureEngine.Grammar.Simple.Rules
{
    public class Rule001 : BaseRule<SimpleShape>
    {
        public Rule001()
        {
            this.Name = "Rule 01";
            this.Params.Add(new DoubleParameter(0, 10, "Support Offset")); // how high support may be raised
            this.Params.Add(new EnumParameter(new List<string>() { "Left Support", "Right Support" }, "Which Support to Raise")); // which support is raised
            this.LHSLabel = SimpleShapeState.Start;
            this.RHSLabel = SimpleShapeState.SubdivideDeck;
            this.Description = "Raises right or left support.";
        }

        public override void Apply(SimpleShape s, object[] p)
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
            s.Horizontal[0].Start = s.Start;
            s.Horizontal[0].End = s.End;
        }
    }
}
