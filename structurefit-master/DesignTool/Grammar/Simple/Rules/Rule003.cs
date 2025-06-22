using System.Collections.Generic;

namespace StructureEngine.Grammar.Simple.Rules
{
    public class Rule003 : BaseRule<SimpleShape>
    {
        // this rule adds a funicular form to support the deck according to tributary area
        public Rule003()
        {
            this.Name = "Rule 03";
            this.Params.Add(new DoubleParameter(40, 120, "Force per Unit Weight per Length"));
            this.Params.Add(new EnumParameter(new List<string>() { "Tension", "Compression" }, "Structural Behavior"));
            this.LHSLabel = SimpleShapeState.AddFunicular;
            this.RHSLabel = SimpleShapeState.End;
            this.Description = "Adds a funicular element to support deck.";
        }

        public override void Apply(SimpleShape s, object[] p)
        {
            // First, set parameter values.
            double force = (double)p[0];
            string behavior = (string)p[1];
            if (behavior == "Compression")
            {
                force = -force;
            }

            // Next, modify existing shape object.
            var newverts = new List<ShapeLine>();

            // sort verticals in order of x coordinate
            s.Verticals.Sort(new ByXLineComparer());

            // solve for bending moments and axial force
            SimpleAnalysis sa = new SimpleAnalysis();
            sa.SolveHorMoment(s);
            sa.SolveFunicular(s, force);
                                
            // replace verticals
            for (int i = 0; i < s.Verticals.Count; i++ )
            {
                ShapeLine v = s.Verticals[i];
                ShapeLine f = s.Funicular[i];
                ShapeLine newv = new ShapeLine(v.End, f.End);
                if (force > 0) // tension elements, so verticals are struts
                {
                    newv.AxialForce = -s.Verticals[i].AxialForce;
                }
                else // compression elements, so vertical are hangers
                {
                    newv.AxialForce = s.Verticals[i].AxialForce;
                }
                newverts.Add(newv);
            }

            s.Verticals = newverts;

            s.Score = sa.Analyze(s);
        }
    }
}
