namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule deletes tower branches
    /// </summary>
    public class Rule004 : BaseRule<BridgeShape>
    {
        public Rule004()
        {
            this.Name = "Rule 04";
            this.Description = "Deletes tower branches.";
            this.Params.Add(new IntParameter(0, 20, "Branch to Remove")); // which branch to remove
            this.LHSLabel = BridgeShapeState.ModifyTower;
            this.RHSLabel = BridgeShapeState.ModifyTower;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // First, set parameter values.
            int branchremove = (int)p[0];

            // Next, modify existing shape object.
            int numbranches = s.Tower.Count - 1;
            if (numbranches > 1)
            {
                int index = branchremove % numbranches + 1;
                ShapeLine removed = s.Tower[index];
                s.Tower.Remove(removed);
                s.Tops.Remove(removed.End);
            }
        }
    }
}
