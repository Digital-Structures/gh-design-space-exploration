namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule branches a tower element
    /// </summary>
    public class Rule003 : BaseRule<BridgeShape>
    {
        public Rule003()
        {
            this.Name = "Rule 03";
            this.Description = "Branches a tower element.";
            this.Params.Add(new DoubleParameter(0.2, 0.8, "Location of Branch from Bottom")); // location of branch from bottom
            this.Params.Add(new IntParameter(2, 4, "Number of Branches")); // number of branches to form
            this.Params.Add(new DoubleParameter(10, 30, "Angle Between Branches")); // angle between branches in degrees
            this.LHSLabel = BridgeShapeState.AddBranches;
            this.RHSLabel = BridgeShapeState.ModifyTower;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // First, set parameter values.
            double nodeheight = (double)p[0];
            int numbranches = (int)p[1];
            double branchangle = (double)p[2];
            double totalbranchangle = branchangle * (numbranches - 1);

            // Next, modify existing shape object.
            ShapeLine sl = s.Tower[0];
            double rotationangle = sl.Rotation;
            double origheight = sl.Length;
            sl.Scale(nodeheight);
            //s.Tower.RemoveAt(0);

            double angle = rotationangle - totalbranchangle / 2;
            for (int i = 0; i < numbranches; i++)
            {
                ShapeLine slb = new ShapeLine(sl.End, angle, origheight * (1 - nodeheight));
                s.Tower.Add(slb);
                angle = angle + branchangle;
                s.Tops.Add(slb.End);
            }

            s.Tops.Remove(s.Tower[0].End);
            s.Tops.Sort(delegate(ShapePoint p1, ShapePoint p2) { return p1.X.CompareTo(p2.X); });
        }
    }
}
