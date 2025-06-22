using System;
using System.Collections.Generic;

namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule connects the support cables to the tower top resulting in the steepest slope.
    /// </summary>
    public class Rule015 : BaseRule<BridgeShape>
    {
        public Rule015()
        {
            this.Name = "Rule 15";
            this.Description = "Connects each cable to the tower top resulting in the steepest slope.";
            this.LHSLabel = BridgeShapeState.ConnectSupports;
            this.RHSLabel = BridgeShapeState.End;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // Modify existing shape object.
            List<ShapePoint> tops = s.Tops;
            //s.Infill2.Add(new Shape_Line(s.Deck[0].Start, 0, 1, Shape_Line.Type.SecondInfill));
            //s.Infill2.Add(new Shape_Line(s.Deck[s.Deck.Count-1].End, 0, 1, Shape_Line.Type.SecondInfill));
            //s.Infill.Clear();

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

            foreach (ShapeLine line in s.Infill2)
            {
                ShapePoint closest = tops[0];
                ShapeLine templine = line.Clone();
                templine.End = closest;
                double maxslope = Math.Abs(templine.SlopeIntercept()[0]);
                foreach (ShapePoint point in tops)
                {
                    templine.End = point;
                    double slope = Math.Abs(templine.SlopeIntercept()[0]);
                    if (slope > maxslope)
                    {
                        maxslope = slope;
                        closest = point;
                    }
                }
                line.End = closest;
            }

            List<ShapePoint> deckpoints = new List<ShapePoint>();
            deckpoints.AddRange(s.DeckDivs);
            deckpoints.Add(s.Deck[0].Start);
            deckpoints.Add(s.Deck[s.Deck.Count - 1].End);
            foreach (ShapePoint point in s.Points)
            {
                if (!deckpoints.Contains(point) && Math.Abs(point.Y - s.Deck[0].Start.Y) <= 0.1)
                {
                    ShapePoint closest = deckpoints[0];
                    double min = double.MaxValue;
                    foreach (ShapePoint p2 in deckpoints)
                    {
                        if (p2.GetDistance(point) < min)
                        {
                            min = p2.GetDistance(point);
                            closest = p2;
                        }
                    }
                    point.X = closest.X;
                }
            }

            //// consolidate all infill elements
            //s.Infill2.AddRange(s.Infill);
            //s.Infill2.Sort(delegate(ShapeLine l1, ShapeLine l2) { return l1.Start.X.CompareTo(l2.Start.X); });
        }
    }
}
