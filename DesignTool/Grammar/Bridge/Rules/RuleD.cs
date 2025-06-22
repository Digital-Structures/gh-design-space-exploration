namespace StructureEngine.Grammar.Bridge.Rules
{
    public class RuleD : BaseRule<BridgeShape>
    {
        public RuleD()
        {
            this.Name = "Rule D";
            this.Description = "Changes state label.";
            this.LHSLabel = BridgeShapeState.ModifySupports;
            this.RHSLabel = BridgeShapeState.ConnectSupports;
        }
    }
}
