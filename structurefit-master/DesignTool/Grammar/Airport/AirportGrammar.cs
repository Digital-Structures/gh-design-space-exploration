using System;
using System.Collections.Generic;
using StructureEngine.Analysis;
using StructureEngine.Grammar.Airport.Rules;

namespace StructureEngine.Grammar.Airport
{
    public class AirportGrammar : BaseGrammar
    {
        public AirportGrammar()
        {
            LoadRules();
            SetRuleGrammar();
        }

        public override IShape GetStartShape()
        {
            return new AirportShape();
        }

        private void LoadRules()
        {
            UniqueRules = new List<IRule>();
            UniqueRules.Add(new Rule001());
            UniqueRules.Add(new Rule002());
            //UniqueRules.Add(new Rule003());
            UniqueRules.Add(new Rule004());
            UniqueRules.Add(new Rule005());
            UniqueRules.Add(new RuleA());
            // etc.
        }

        public override IAnalysis GetAnalysis()
        {
            throw new NotImplementedException();
        }
    }
}
