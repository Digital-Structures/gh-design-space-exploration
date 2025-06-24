namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule changes the length of tower branches
    /// </summary>
    public class Rule005 : BaseRule<BridgeShape>
    {
        public Rule005()
        {
            this.Name = "Rule 05";
            this.Description = "Changes the length of tower branches.";
            this.Params.Add(new DoubleParameter(0.5, 1.5, "Extension or Contraction")); // decimal amount to increase or decrease
            this.Params.Add(new IntParameter(0,20, "Which Branch to Adjust")); // which branch to adjust
            this.LHSLabel = BridgeShapeState.ModifyTower;
            this.RHSLabel = BridgeShapeState.ModifyTower;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // First, set parameter values.
            double branchlength = (double)p[0];
            int branchchange = (int)p[1];

            // Next, modify existing shape object.
            int numbranches = s.Tower.Count - 1;
            if (numbranches > 1)
            {
                int index = branchchange % numbranches + 1;
                s.Tower[index].Scale(branchlength);
            }
        }
    }
}

