using System.Collections.Generic;
using StructureEngine.Analysis;
//using StructureEngine.View.GrammarControls;
//using StructureEngine.View;
using StructureEngine.Grammar.Simple.Rules;

namespace StructureEngine.Grammar.Simple
{
    public class SimpleGrammar : BaseGrammar
    {
        public SimpleGrammar()
        {
            LoadRules();
            SetRuleGrammar();
        }

        public override IShape GetStartShape()
        {
            return new SimpleShape();
        }

        private void LoadRules()
        {
            UniqueRules = new List<IRule>();
            UniqueRules.Add(new Rule001());
            UniqueRules.Add(new Rule002());
            UniqueRules.Add(new Rule003());
            UniqueRules.Add(new RuleA());
        }

        public override IAnalysis GetAnalysis()
        {
            return new SimpleAnalysis();
        }
    }
}
