
namespace StructureEngine.Grammar.Simple.Rules
{
    public class RuleA : BaseRule<SimpleShape>
    {
        // this rule changes the state from SubdivideDeck to End
        public RuleA()
        {
            this.Name = "Rule A";
            this.LHSLabel = SimpleShapeState.SubdivideDeck;
            this.RHSLabel = SimpleShapeState.AddFunicular;
            this.Description = "Changes state label.";
        }

        public override bool CanApply(SimpleShape s)
        {
            return s.Verticals.Count != 0;
        }
    }
}
