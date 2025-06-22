using System;

namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule rotates the tour 180 degrees and makes branches go to zero
    /// </summary>
    public class Rule006 : BaseRule<BridgeShape>
    {
        public Rule006()
        {
            this.Name = "Rule 06";
            this.Description = "Rotates the tower 180 degrees.";
            this.LHSLabel = BridgeShapeState.ModifyTower;
            this.RHSLabel = BridgeShapeState.MakeDeck;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // Rotate 180 degrees
            double angle = 180;
            ShapePoint center = new ShapePoint(s.ZeroShapePoint.X + s.Dimensions[0] / 2, s.ZeroShapePoint.Y + s.Dimensions[1] / 2);
            s.Rotate(angle, center);
            s.Tops.Clear();
            s.Tops.Add(s.Tower[0].Start);
            //s.Tops.Add(center);

            // Extend all branches to ground
            double zero = s.ZeroShapePoint.Y;
            int numbranches = s.Tower.Count - 1;
            for (int i = 1; i < numbranches + 1; i++) // for each branch
            {
                ShapeLine line = s.Tower[i];
                double end_y = line.End.Y - zero;
                double angleRad = ((line.Rotation + 180) * Math.PI / 180);
                double stretch = end_y / Math.Sin(angleRad);
                double scale = (line.Length + stretch) / line.Length;
                line.Scale(scale);
            }

            // Remove branches that are too long
            //double meanlength = 0;

            //foreach (Shape_Line line in s.Tower)
            //{
            //    meanlength += line.length;
            //}
            //meanlength = meanlength / s.Tower.Count;

            //for (int i = s.Tower.Count - 1; i >= 0; i--)
            //{
            //    Shape_Line line = s.Tower[i];
            //    if (line.length > 1.2 * meanlength)
            //    {
            //        s.Tower.Remove(line);
            //    }
            //}
        }
    }
}

