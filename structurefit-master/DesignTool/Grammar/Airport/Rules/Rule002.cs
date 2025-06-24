using System.Collections.Generic;

namespace StructureEngine.Grammar.Airport.Rules
{
    public class Rule002 : BaseRule<AirportShape>
    {
        // This rule gives the spanning element a catenary form
        public Rule002()
        {
            this.Name = "Rule 02";
            this.Params.Add(new DoubleParameter(50, 150, "Force per Unit Weight per Length")); // tension per unit weight per length (i.e. sag)
            this.Params.Add(new EnumParameter(new List<string>() {"Tension", "Compression"}, "Structural Behavior")); // whether it's tension or compression (i.e. cable or arch)
            this.Params.Add(new IntParameter(4, 20, "Number of Loading Points"));
            this.LHSLabel = AirportShapeState.ModifySpan;
            this.RHSLabel = AirportShapeState.AddVerticals;
        }

        public override void Apply(AirportShape s, object[] p)
        {
            // First, set parameter values.
            double H = (double)p[0];
            string behavior = (string)p[1];
            int num = (int)p[2];
            if (behavior == "Compression")
            {
                H = -H;
            }

            // Next, modify existing shape object.
            var gs = new GraphicStatics.ProblemSetup(s.Roof[0].Start, s.Roof[0].End, num, 1); // TODO: modify segments and loads as necessary
            gs.DrawFcP(H);
            gs.DrawFmD(H);

            double angle = s.Roof[0].Rotation;
            ShapeLine l1 = new ShapeLine(s.Roof[0].Start, 0, s.Roof[0].Length);
            ShapeLine l2 = new ShapeLine(s.Roof[0].Start, s.Roof[0].Rotation, s.Roof[0].Length);

            double r1 = l1.Rotation;
            double r2 = l2.Rotation;

            s.Roof.Clear();
            //s.Roof.AddRange(gs.ForceP.Rays);
            //s.Roof.AddRange(gs.ForceP.LoadLine);
            s.Roof.AddRange(gs.FormD.Segments);
            s.Start = s.Roof[0].Start;
            s.End = s.Roof[s.Roof.Count - 1].End;
        }
    }
}
