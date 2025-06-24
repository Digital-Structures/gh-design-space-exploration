using System;
using System.Collections.Generic;
using StructureEngine.Analysis;

namespace StructureEngine.Grammar
{
    public class RandomComputation
    {
        public RandomComputation()
        {
        }

        public RandomComputation(IGrammar g)
        {
            Gram = g;
        }

        public IGrammar Gram
        {
            get;
            set;
        }

        public List<IShape> RandShapes(int num)
        {
            List<IShape> l = new List<IShape>();
            for (int i = 0; i < num; i++)
            {
                IShape s = this.GenerateRandShape();
                l.Add(s);
            }
            return l;
        }

        public IShape GenerateRandShape()
        {
            IShape s = Gram.GetStartShape();
            while (!s.ShapeState.IsEnd())
            {
                s = this.ApplyRandRule(s);
            }

            IAnalysis a = Gram.GetAnalysis();
            s.Score = a.Analyze(s);
            return s;
        }

        public IShape GenerateShapeForRule(IRule r)
        {
            IShape s = Gram.GetStartShape();
            int count = 0;
            while (!r.CanApply(s) && count < 30)
            {
                s = this.ApplyRandRule(s);
                count++;
            }
            IAnalysis a = Gram.GetAnalysis();
            //s.Score = a.Analyze(s);
            return s;
        }

        public IShape ApplyRandRule(IShape s)
        {
            IList<IRule> rules = Gram.GetPossibleRules(s);
            int poss = rules.Count;
            if (poss != 0)
            {
                int index = Utilities.MyRandom.Next(0, poss);
                IRule rule = rules[index];
                object[] p = this.GenerateRandParams(rule);
                //Shape s_copy = s.Clone();
                rule.Apply(s, p);

                // for now, build rule history within RandomComputation.
                // this should eventually be moved somewhere more general!

                //RuleSet rs = new RuleSet(rule, p);
                //s.History.AddRule(rs);
                return s;
            }
            else
            {
                return s;
            }
        }

        public object[] GenerateRandParams(IRule rule)
        {
            int num = rule.Params.Count;
            object[] p = new object[num];

            int i = 0;
            foreach (IRuleParameter param in rule.Params)
            {
                object value = param.GetRandomValue();
                p[i] = value;
                i++;
            }

            return p;
        }

    }
}
