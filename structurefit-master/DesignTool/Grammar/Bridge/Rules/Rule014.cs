using System.Collections.Generic;

namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule connects the support cables to the nearest tower top.
    /// </summary>
    public class Rule014 : BaseRule<BridgeShape>
    {
        public Rule014()
        {
            this.Name = "Rule 14";
            this.Description = "Connects each cable to the closest tower top.";
            this.LHSLabel = BridgeShapeState.ConnectSupports;
            this.RHSLabel = BridgeShapeState.End;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // remove outlines that will be duplicated
            for (int i = 0; i < s.Infill.Count; i++)
            {
                for (int j = i + 1; j < s.Infill.Count; j++)
                {
                    ShapeLine l1 = s.Infill[i];
                    ShapeLine l2 = s.Infill[j];

                    if (l1.HasCommonPoint(l2) && l1.FindCommonPoint(l2).Y == s.Deck[0].Start.Y)
                    {
                        s.Infill.Remove(l1);
                        s.Infill.Remove(l2);

                        break;
                    }
                }
            }

            // Modify existing shape object.
            List<ShapePoint> tops = s.Tops;
            foreach (ShapeLine line in s.Infill2)
            {
                ShapePoint closest = tops[0];
                double mindist = line.End.GetDistance(closest);
                foreach (ShapePoint point in tops)
                {
                    double dist = line.End.GetDistance(point);
                    if (dist < mindist)
                    {
                        mindist = dist;
                        closest = point;
                    }
                }
                line.End = closest;
            }

            //// consolidate all infill elements
            //s.Infill2.AddRange(s.Infill);
            //s.Infill2.Sort(delegate(ShapeLine l1, ShapeLine l2) { return l1.Start.X.CompareTo(l2.Start.X); });
        }
    }
}
