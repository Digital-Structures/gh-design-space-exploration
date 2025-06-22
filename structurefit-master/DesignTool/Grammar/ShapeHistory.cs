using System.Collections.Generic;

namespace StructureEngine.Grammar
{
    public class ShapeHistory
    {
        public ShapeHistory()
        {
            List<RuleSet> der = new List<RuleSet>();
            this.Derivation = der;
        }

        public void AddRule(RuleSet r)
        {
            this.Derivation.Add(r);
        }

        public ShapeHistory Clone()
        {
            ShapeHistory shclone = new ShapeHistory();
            List<RuleSet> rulesclone = new List<RuleSet>();
            foreach (RuleSet ruleset in this.Derivation)
            {
                rulesclone.Add(ruleset.Clone());
            }

            shclone.Derivation = rulesclone;
            return shclone;
        }

        public void GoBack()
        {
            this.Derivation.RemoveAt(this.Derivation.Count - 1);
        }

        public IShape ApplyHistory(IShape s)
        {
            if (this.Derivation.Count > 0 && this.Derivation[0].Rule.CanApply(s))
            {
                IShape s_new = s.Clone();
                foreach (RuleSet rs in this.Derivation)
                {
                    rs.Rule.Apply(s_new, rs.Param);
                }
                return s_new;
            }
            else
            {
                return s;
            }
        }

        public IShape ApplyHistory(IGrammar g)
        {
            IShape s = g.GetStartShape();
            return this.ApplyHistory(s);
        }

        public void Split(int i, out ShapeHistory seg1, out ShapeHistory seg2)
        {
            seg1 = new ShapeHistory();
            seg1.Derivation = this.Derivation.GetRange(0, i);
            seg2 = new ShapeHistory();
            seg2.Derivation = this.Derivation.GetRange(i, this.Derivation.Count - i);
        }

        //public Shape_History Condense()
        //{
        //    Shape_History cond = new Shape_History();
        //    Shape s1 = new Shape();
        //    Shape s2 = new Shape();
        //    Shape_History h1;
        //    Shape_History h2;

        //    //foreach (RuleSet rs in this.Derivation)
        //    for (int i = 0; i < this.Derivation.Count - 1; i++)
        //    {
        //        this.Split(i, out h1, out h2);
        //        s2 = h1.ApplyHistory(s1);

        //        if (!s2.IsSame(s1))
        //        {
        //            cond.Derivation.Add(this.Derivation[i+1]);
        //        }
        //    }

        //    return cond;
        //}



        public List<RuleSet> Derivation
        {
            get;
            set;
        }

    }
}
