using System.Collections.Generic;

namespace StructureEngine.Grammar.Simple.Rules
{
    public class Rule002: BaseRule<SimpleShape>
    {
        // this rule subdivides a chosen segment of the deck
        public Rule002()
        {
            this.Name = "Rule 02";
            this.Params.Add(new IntParameter(0, 30, "Deck to Subdivide")); // which segment to be subdivided based on modulus...
            this.Weight = 5;
            this.LHSLabel = SimpleShapeState.SubdivideDeck;
            this.RHSLabel = SimpleShapeState.SubdivideDeck;
            this.Description = "Subdivides deck and adds vertical elements at support points.";
        }

        public override void Apply(SimpleShape s, object[] p)
        {
            // First, set parameter values.
            int divdeck = (int)p[0];
            int numdeck = s.Horizontal.Count;
            int modnum = divdeck * (numdeck + 1);

            // Next, modify existing shape object.
            if (numdeck > 0)
            {
                int index = divdeck % numdeck;
                List<ShapeLine> subs = s.Horizontal[index].GetSubdivide(2);
                s.Horizontal.Remove(s.Horizontal[index]);
                s.Horizontal.InsertRange(index, subs);

                for (int i = 0; i < subs.Count - 1; i++)
                {
                    var l = new ShapeLine(new ShapePoint(subs[i].End.X, subs[i].End.Y + 15),
                        subs[i].End);
                    s.Verticals.Add(l);
                }
            }
        }
    }
}
