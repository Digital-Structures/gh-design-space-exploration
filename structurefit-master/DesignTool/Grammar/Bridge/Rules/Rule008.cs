using System.Collections.Generic;

namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule fills in space between branches with narrow angles
    /// </summary>
    public class Rule008 : BaseRule<BridgeShape>
    {
        public Rule008()
        {
            this.Name = "Rule 08";
            this.Description = "Fills in space between tower branches with narrow angles.";
            this.Params.Add(new IntParameter(0, 20, "Which Branch to Infill From")); // which branch to infill from
            this.Params.Add(new DoubleParameter(25, 45, "Threshold Fill Angle")); // fill angle
            this.LHSLabel = BridgeShapeState.MakeInfill;
            this.RHSLabel = BridgeShapeState.MakeInfill;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // First, set parameter values.
            int branchfill = (int)p[0];
            double fillangle = (double)p[1];

            // Next, modify existing shape object.
            int numtower = s.Tower.Count;
            if (numtower > 1)
            {
                // Determine the randomly chosen tower element
                int index = branchfill % numtower;
                ShapeLine fillline = s.Tower[index];

                // Check to see which other element is closest
                double minangle = 360;
                int minindex = index;
                for (int i = index + 1; i < numtower; i++)
                {
                    double betweenangle = fillline.AngleBetween(s.Tower[i]);
                    if (betweenangle < minangle && betweenangle != 0)
                    {
                        minangle = betweenangle;
                        minindex = i;
                    }
                }

                // If closest angle has small enough angle, create infill
                if (minangle <= fillangle)
                {
                    ShapeLine line2 = s.Tower[minindex];
                    List<ShapePoint> points = fillline.ThreePoints(line2);
                    ShapeArea fill = new ShapeArea(points);
                    s.Fill.Add(fill);
                }
            }
        }
    }
}

