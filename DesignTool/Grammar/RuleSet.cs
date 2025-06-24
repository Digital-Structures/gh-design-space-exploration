
namespace StructureEngine.Grammar
{
    public class RuleSet
    {
        public RuleSet(IRule rule, object[] par)
        {
            this.Rule = rule;
            this.Param = par;
        }

        public IRule Rule
        {
            get;
            set;
        }

        public object[] Param
        {
            get;
            set;
        }

        public RuleSet Clone()
        {
            IRule rulecopy = this.Rule; // for now, this is not a deep clone
            object[] pcopy = new object[Param.Length];
            for (int i = 0; i < Param.Length; i++)
            {
                object o = Param[i];
                pcopy[i] = o;
            }

            RuleSet copy = new RuleSet(rulecopy, pcopy);
            return copy;
        }
    }
}
