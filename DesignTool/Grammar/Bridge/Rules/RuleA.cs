namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule changes the State from MultipleTowers to Subdivide
    /// </summary>
    public class RuleA : BaseRule<BridgeShape>
    {
        public RuleA()
        {
            this.Name = "Rule A";
            this.Description = "Changes state label.";
            this.LHSLabel = BridgeShapeState.MultipleTowers;
            this.RHSLabel = BridgeShapeState.Subdivide;
        }
    }
}
