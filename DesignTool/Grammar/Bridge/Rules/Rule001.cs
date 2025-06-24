namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule determines the height of a tower element.
    /// </summary>
    public class Rule001 : BaseRule<BridgeShape>
    {
        public Rule001()
        {
            this.Name = "Rule 01";
            this.Description = "Sets the height of the tower.";
            this.Params.Add(new IntParameter(45, 80, "Tower Height")); // height of tower
            this.LHSLabel = BridgeShapeState.MakeTower;
            this.RHSLabel = BridgeShapeState.AddBranches;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // First, set parameter values.
            int towerHeight = (int)p[0];

            // Next, modify existing shape object.
            double scale = (double)towerHeight / (double)s.Tower[0].Length;
            s.Tower[0].Scale(scale);
        }
    }
}
