using System;
using System.Collections.Generic;

namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule adjusts cable shape according to slackness.
    /// </summary>
    public class Rule017 : BaseRule<BridgeShape>
    {
        public Rule017()
        {
            this.Name = "Rule 17";
            this.Description = "Connects support cables to suspension cables.";
            this.Params.Add(new DoubleParameter(0.01, .03, "Cable Slack")); // slackness of the cables
            this.LHSLabel = BridgeShapeState.ConnectSupports;
            this.RHSLabel = BridgeShapeState.End;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // First, set parameter values.
            double slack = (double)p[0];

            // Next, Modify existing shape object.


            // Join infill outline into one line
            for (int i = 0; i < s.Infill.Count; i++)
            {
                for (int j = i + 1; j < s.Infill.Count; j++)
                {
                    ShapeLine l1 = s.Infill[i];
                    ShapeLine l2 = s.Infill[j];

                    if (l1.HasCommonPoint(l2) && l1.FindCommonPoint(l2).Y == s.Deck[0].Start.Y)
                    {
                        ShapePoint common = l1.FindCommonPoint(l2);
                        ShapePoint start = l1.Start;
                        if (start.IsSame(common))
                        {
                            start = l1.End;
                        }
                        ShapePoint end = l2.End;
                        if (end.IsSame(common))
                        {
                            end = l2.Start;
                        }
                        ShapeLine infill = new ShapeLine(start, end);
                        s.Infill.Remove(l1);
                        s.Infill.Remove(l2);
                        s.Infill.Add(infill);

                        break;
                    }
                }
            }

            // Modify existing shape object.
            foreach (ShapeLine line in s.Infill2)
            {
                ShapeLine closest = s.Infill[0];
                double mindist = double.MaxValue;
                foreach (ShapeLine infill in s.Infill) // find closest infill line
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
                ShapePoint end = line.FindIntersection(closest);
                line.End = end;
                closest.Loads.Add(new ShapePointLoad(end, 1, line.Rotation));
            }

            List<ShapeLine> newinfill = new List<ShapeLine>();
            foreach (ShapeLine line in s.Infill)
            {
                int count = line.Loads.Count;
                line.Loads.Sort(new ByXCoordComparer());

                if (count > 0)
                {
                    double theta = line.Loads[0].Direction;
                    double angle = 90 + theta;
                    ShapePoint origin = line.Start;

                    // y = ax^2 + bx + c
                    double a = slack;
                    double H = Math.Pow(line.Length, 2) / a;

                    ShapeLine newcoord = line.Clone();
                    double[] startcoords = newcoord.Start.RotateCoords(origin, -angle);
                    double[] endcoords = newcoord.End.RotateCoords(origin, -angle);
                    newcoord.Start.X = startcoords[0];
                    newcoord.Start.Y = startcoords[1];
                    newcoord.End.X = endcoords[0];
                    newcoord.End.Y = endcoords[1];

                    double[] bc = newcoord.Parabola_bc(a);
                    double b = bc[0];
                    double c = bc[1];

                    foreach (ShapePointLoad load in line.Loads)
                    {
                        double[] coords = load.Point.RotateCoords(origin, -angle);
                        double x_prime = coords[0];
                        double y_prime = a * Math.Pow(x_prime, 2) + b * x_prime + c;
                        double delta_y_prime = y_prime - coords[1];
                        double deltax = delta_y_prime * Math.Cos(Math.PI * theta / 180);
                        double deltay = delta_y_prime * Math.Sin(Math.PI * theta / 180);
                        double x = load.Point.X + deltax;
                        double y = load.Point.Y + deltay;
                        load.Point.X = x;
                        load.Point.Y = y;
                    }
                    ShapeLine l1 = new ShapeLine(line.Start, line.Loads[0].Point);
                    l1.HorizontalForce = H;
                    l1.AxialForce = l1.GetAxialForce();
                    newinfill.Add(l1);
                    for (int i = 0; i < count - 1; i++)
                    {
                        ShapePointLoad load = line.Loads[i];
                        ShapeLine l = new ShapeLine(load.Point, line.Loads[i + 1].Point);
                        l.HorizontalForce = H;
                        l.AxialForce = l1.GetAxialForce();
                        newinfill.Add(l);
                    }
                    ShapeLine ln = new ShapeLine(line.Loads[count - 1].Point, line.End);
                    ln.HorizontalForce = H;
                    ln.AxialForce = l1.GetAxialForce();
                    newinfill.Add(ln);
                }
                else
                {
                    newinfill.Add(line);
                }
            }
            s.Infill = newinfill;
            s.IsSuspension = true;
        }
    }
}
