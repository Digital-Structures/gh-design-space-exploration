using System.Collections.Generic;
using System;
namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule connects the cables in a fan configuration.
    /// </summary>
    public class Rule016 : BaseRule<BridgeShape>
    {
        public Rule016()
        {
            this.Name = "Rule 16";
            this.Description = "Connects cables in a parallel configuration.";
            this.LHSLabel = BridgeShapeState.ConnectSupports;
            this.RHSLabel = BridgeShapeState.End;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // Next, Modify existing shape object.

            // determine points of intersection between tower(s) and deck
            List<ShapePoint> intPoints = new List<ShapePoint>();
            foreach (ShapeLine l in s.Deck)
            {
                foreach (ShapeLine t in s.Tower)
                {
                    if (l.IsPointOnLine(l.FindIntersection(t)) && t.IsPointOnLine(l.FindIntersection(t)))
                    {
                        ShapePoint intersect = l.FindIntersection(t);
                        bool isOnList = false;
                        foreach (ShapePoint point in intPoints)
                        {
                            if (intersect.IsSame(point))
                            {
                                isOnList = true;
                                break;
                            }
                        }
                        if (!isOnList)
                        {
                            intPoints.Add(intersect);
                        }
                    }
                }
                foreach (ShapeLine i in s.Infill)
                {
                    if (l.IsPointOnLine(l.FindIntersection(i)) && i.IsPointOnLine(l.FindIntersection(i)))
                    {
                        ShapePoint intersect = l.FindIntersection(i);
                        bool isOnList = false;
                        foreach (ShapePoint point in intPoints)
                        {
                            if (intersect.IsSame(point))
                            {
                                isOnList = true;
                                break;
                            }
                        }
                        if (!isOnList)
                        {
                            intPoints.Add(intersect);
                        }
                    }
                }
            }
            intPoints.Sort(delegate(ShapePoint p1, ShapePoint p2) { return p1.X.CompareTo(p2.X); });


            foreach (ShapeLine line in s.Infill2)
            {
                // determine outline cables corresponding to each segment of deck
                double x = line.Start.X;

                if (x < intPoints[0].X)
                {
                    break;
                }

                ShapeLine closest = null;
                if (intPoints.Count == s.Infill.Count + 1)
                {
                    for (int i = 1; i < intPoints.Count; i++)
                    {
                        if (x < intPoints[i].X)
                        {
                            closest = s.Infill[i - 1];
                            break;
                        }
                    }
                }
                else
                {
                    line.End.X = line.Start.X;
                    closest = null;
                    double mindist = double.MaxValue;
                    foreach (ShapeLine infill in s.Infill)
                    {
                        ShapePoint intersection = line.FindIntersection(infill);
                        if (infill.IsPointOnLine(intersection))
                        {
                            double dist = line.Start.GetDistance(intersection);
                            if (dist < mindist)
                            {
                                mindist = dist;
                                closest = infill;
                            }
                        }
                    }
                    
                }

                if (closest == null)
                {
                    break;
                }


                double angle = closest.Rotation;
                ShapeLine temp = new ShapeLine(line.Start, angle, line.Length);
                line.End = temp.End;

                /*
                // find closest primary infill cable and make parallel
                line.End.X = line.Start.X;
                ShapeLine closest = s.Infill[0];
                double mindist = double.MaxValue;
                foreach (ShapeLine infill in s.Infill)
                {
                    ShapePoint intersection = line.FindIntersection(infill);
                    if (infill.IsPointOnLine(intersection))
                    {
                        double dist = line.Start.GetDistance(intersection);
                        if (dist < mindist)
                        {
                            mindist = dist;
                            closest = infill;
                        }
                    }
                }
                double angle = closest.Rotation;
                ShapeLine temp = new ShapeLine(line.Start, angle, line.Length);
                if (temp.End.Y < s.Deck[0].Start.Y)
                {
                    temp.Rotate(180);
                }
                line.End = temp.End;
                */

                // find closest tower element
                closest = null;
                double mindist2 = double.MaxValue;
                foreach (ShapeLine tower in s.Tower)
                {
                    ShapePoint intersection = line.FindIntersection(tower);
                    if (tower.IsPointOnLine(intersection) && intersection.Y >= s.Deck[0].Start.Y)
                    {
                        double dist = line.Start.GetDistance(intersection);
                        if (dist < mindist2)
                        {
                            mindist2 = dist;
                            closest = tower;
                        }
                    }
                }

                if (closest == null)
                {
                    line.End = line.Start;
                }
                else
                {
                    ShapePoint end = line.FindIntersection(closest);
                    line.End = end;
                }
            }

            // remove duplicated outlines
            for (int i = 0; i < s.Infill.Count; i++)
            {
                foreach (ShapeLine l2 in s.Infill2)
                {
                    ShapeLine l1 = s.Infill[i];

                    if (l1.HasCommonPoints(l2))
                    {
                        s.Infill.Remove(l1);
                        break;
                    }
                }
            }

            //// consolidate all infill elements
            //s.Infill2.AddRange(s.Infill);
            //s.Infill2.Sort(delegate(ShapeLine l1, ShapeLine l2) { return l1.Start.X.CompareTo(l2.Start.X); });
        }
    }
}
