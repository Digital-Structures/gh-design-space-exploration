namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule changes the State from ModifyTower to MakeDeck
    /// </summary>
    public class RuleC : BaseRule<BridgeShape>
    {
        public RuleC()
        {
            this.Name = "Rule C";
            this.Description = "Changes state label.";
            this.LHSLabel = BridgeShapeState.ModifyTower;
            this.RHSLabel = BridgeShapeState.MakeDeck;
        }
    }
}
