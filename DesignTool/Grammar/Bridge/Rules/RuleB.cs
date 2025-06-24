namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule changes the State from MakeTower to ModifyTower
    /// </summary>
    public class RuleB : BaseRule<BridgeShape>
    {
        public RuleB()
        {
            this.Name = "Rule B";
            this.Description = "Changes state label.";
            this.LHSLabel = BridgeShapeState.AddBranches;
            this.RHSLabel = BridgeShapeState.ModifyTower;
        }
    }
}
