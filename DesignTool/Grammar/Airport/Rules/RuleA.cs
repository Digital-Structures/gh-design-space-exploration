
namespace StructureEngine.Grammar.Airport.Rules
{
    public class RuleA : BaseRule<AirportShape>
    {
        // This rule moves shape from State Start to State ModifySpan
        public RuleA()
        {
            this.Name = "Rule A";
            this.LHSLabel = AirportShapeState.Start;
            this.RHSLabel = AirportShapeState.ModifySpan;
        }
    }
}
