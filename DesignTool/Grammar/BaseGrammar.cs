using System.Collections.Generic;
using StructureEngine.Analysis;
using System;

namespace StructureEngine.Grammar
{
    public abstract class BaseGrammar : IGrammar
    {
        private List<IRule> _allrules;
        public List<IRule> AllRules
        {
            get
            {
                if (_allrules != null)
                {
                    return _allrules;
                }
                else
                {
                    _allrules = new List<IRule>();
                    foreach (IRule r in UniqueRules)
                    {
                        if (r.Weight > 0)
                        {
                            for (int i = 0; i < r.Weight; i++)
                            {
                                _allrules.Add(r);
                            }
                        }
                        else
                        {
                            _allrules.Add(r);
                        }
                    }
                    return _allrules;
                }
            }
        }

        public List<IRule> UniqueRules
        {
            get;
            set;
        }

        public IList<IRule> GetPossibleRules(IShape s)
        {
            var rules = new List<IRule>();

            foreach (IRule rule in AllRules)
            {
                if (rule.CanApply(s))
                {
                    rules.Add(rule);
                }
            }
            return rules;
        }

        protected void SetRuleGrammar()
        {
            foreach (IRule r in UniqueRules)
            {
                r.MyGrammar = this;
            }
        }

        public abstract IShape GetStartShape();

        public abstract IAnalysis GetAnalysis();

        public IShape[] RandCrossover(Random r, IShape Cross1, IShape Cross2)
        {
            // randomly select a splice point in Cross1's history
            int num1 = Cross1.History.Derivation.Count;

            int possrules = 0;
            List<int> splicepoints2 = new List<int>();
            int splice1 = new int();

            while (possrules == 0)
            {
                splice1 = r.Next(1, num1 - 1);
                splicepoints2 = this.GetSplicePoints2(splice1, Cross1, Cross2);

                if (splicepoints2.Count > 0)
                {
                    possrules = 1;
                }
            }

            // randomly select a splice point for Cross2
            int num2 = Cross2.History.Derivation.Count;
            int count2 = splicepoints2.Count;
            int rand2 = r.Next(0, count2 - 1);
            int splice2 = splicepoints2[rand2];

            // generate new crossover shapes
            Grammar.IShape[] crossed = Crossover(splice1, splice2, Cross1, Cross2);

            return crossed;
        }

        public List<IShape> AllCrossover(IShape Cross1, IShape Cross2)
        {
            int num1 = Cross1.History.Derivation.Count;

            int splice1 = new int();
            int splice2 = new int();
            List<int> splicepoints2 = new List<int>();

            List<IShape> allcrossed = new List<IShape>();

            // loop through all possible splice points for Cross 1
            for (int i = 1; i < num1; i++)
            {
                splice1 = i;
                splicepoints2 = this.GetSplicePoints2(splice1, Cross1, Cross2);

                // loop through all possible splice points for Cross 2
                for (int j = 0; j < splicepoints2.Count; j++)
                {
                    splice2 = splicepoints2[j];
                    IShape[] crossed = Crossover(splice1, splice2, Cross1, Cross2);
                    allcrossed.Add(crossed[0]);
                    allcrossed.Add(crossed[1]);
                }
            }

            List<IShape> unique = this.Unique(allcrossed);

            return unique;
        }

        private List<int> GetSplicePoints2(int splice1, IShape c1, IShape c2)
        {
            ShapeHistory hist1 = c1.History;
            ShapeHistory hist2 = c2.History;
            int num1 = hist1.Derivation.Count;

            ShapeHistory hist1a = new ShapeHistory();
            ShapeHistory hist1b = new ShapeHistory();
            ShapeHistory hist2a = new ShapeHistory();

            hist1a.Derivation = hist1.Derivation.GetRange(0, splice1);
            hist1b.Derivation = hist1.Derivation.GetRange(splice1, hist1.Derivation.Count - splice1);

            IShape s1a = this.GetStartShape();
            s1a = hist1a.ApplyHistory(s1a);

            List<int> splicepoints2 = new List<int>();

            // determine all possible rules that can be applied at splice point
            IList<IRule> rules = this.GetPossibleRules(s1a);

            // determine feasible splice points in Cross2
            foreach (IRule r1 in rules)
            {
                foreach (RuleSet rs in hist2.Derivation)
                {
                    IRule r2 = rs.Rule;
                    if (String.Compare(r1.Name, r2.Name) == 0) // i.e. if true
                    {
                        int splice2 = hist2.Derivation.IndexOf(rs); // make sure rule isn't first
                        if (splice2 != 0)
                        {
                            // get head of second crossover shape
                            IShape s2a = this.GetStartShape();
                            hist2a.Derivation = hist2.Derivation.GetRange(0, splice2);
                            s2a = hist2a.ApplyHistory(s2a);

                            // check whether tail of first shape can apply to head of second
                            if (hist1b.Derivation[0].Rule.CanApply(s2a))
                            {
                                splicepoints2.Add(splice2);
                            }
                        }
                    }
                }
            }

            return splicepoints2;
        }

        private IShape[] Crossover(int splice1, int splice2, IShape c1, IShape c2)
        {
            ShapeHistory hist1 = c1.History;
            ShapeHistory hist2 = c2.History;
            int num1 = hist1.Derivation.Count;

            ShapeHistory seg1a;
            ShapeHistory seg1b;
            hist1.Split(splice1, out seg1a, out seg1b);

            ShapeHistory seg2a;
            ShapeHistory seg2b;
            hist2.Split(splice2, out seg2a, out seg2b);

            ShapeHistory shx1 = new ShapeHistory();
            shx1.Derivation.AddRange(seg1a.Derivation);
            shx1.Derivation.AddRange(seg2b.Derivation);
            Grammar.IShape crossed1 = shx1.ApplyHistory(this);

            ShapeHistory shx2 = new ShapeHistory();
            shx2.Derivation.AddRange(seg2a.Derivation);
            shx2.Derivation.AddRange(seg1b.Derivation);
            Grammar.IShape crossed2 = shx2.ApplyHistory(this);

            // generate new crossover shapes
            crossed1.Parent1 = c1;
            crossed1.Parent2 = c2;
            crossed1.SplicePoint1 = splice1;
            crossed1.SplicePoint2 = splice2;
            crossed2.Parent1 = c2;
            crossed2.Parent2 = c1;
            crossed2.SplicePoint1 = splice2;
            crossed2.SplicePoint2 = splice1;

            IShape[] crossed = new IShape[] { crossed1, crossed2 };

            return crossed;
        }

        private List<IShape> Unique(List<IShape> shapes)
        {
            List<IShape> unique = new List<IShape>();

            foreach (IShape s in shapes)
            {
                bool u = true;
                for (int i = 0; i < unique.Count; i++)
                {
                    if (s.LooksSame(unique[i]))
                    {
                        u = false;
                        break;
                    }
                }
                if (u)
                {
                    unique.Add(s);
                }
            }

            return unique;
        }
    }
}
