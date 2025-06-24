using System.Collections.Generic;
using StructureEngine.Grammar.Bridge.Rules;
using System;
using StructureEngine.Analysis;

namespace StructureEngine.Grammar.Bridge
{
    public class BridgeGrammar : BaseGrammar
    {
        public BridgeGrammar()
        {
            LoadRules();
            SetRuleGrammar();
        }

        public override IShape GetStartShape()
        {
            return new BridgeShape();
        }

        private void LoadRules()
        {
            UniqueRules = new List<IRule>();
            UniqueRules.Add(new Rule001());
            UniqueRules.Add(new Rule002());
            UniqueRules.Add(new Rule003());
            UniqueRules.Add(new Rule004());
            UniqueRules.Add(new Rule005());
            UniqueRules.Add(new Rule006());
            UniqueRules.Add(new Rule007());
            UniqueRules.Add(new Rule008());
            UniqueRules.Add(new Rule009());
            UniqueRules.Add(new Rule010());
            UniqueRules.Add(new Rule011());
            UniqueRules.Add(new Rule012());
            UniqueRules.Add(new Rule013());
            UniqueRules.Add(new Rule014());
            UniqueRules.Add(new Rule015());
            UniqueRules.Add(new Rule016());
            UniqueRules.Add(new Rule017());
            UniqueRules.Add(new RuleA());
            UniqueRules.Add(new RuleB());
            UniqueRules.Add(new RuleC());
            UniqueRules.Add(new RuleD());
            // etc.
        }

        public override IAnalysis GetAnalysis()
        {
            return new BridgeAnalysis();
        }

        public double DeckDistLoad;
        public double DeckHeight;
        public double ClearSpan;

    }
}
