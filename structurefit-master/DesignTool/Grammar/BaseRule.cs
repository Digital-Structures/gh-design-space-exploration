using System.Collections.Generic;

namespace StructureEngine.Grammar
{
    /// <summary>
    /// Type inference wrapper around IRule
    /// </summary>
    public abstract class BaseRule<T> : IRule where T : class, IShape
    {
        public BaseRule()
        {
            this.Name = "(unnamed rule)";
            this.Description = "(no description)";
            this.Params = new List<IRuleParameter>();
        }

        public IGrammar MyGrammar
        {
            get;
            set;
        }

        public string Name
        {
            get;
            protected set;
        }

        public string Description
        {
            get;
            protected set;
        }

        public List<IRuleParameter> Params
        {
            get;
            protected set;
        }

        public IShapeState LHSLabel
        {
            get;
            protected set;
        }
        public IShapeState RHSLabel
        {
            get;
            protected set;
        }

        public double Weight
        {
            get;
            protected set;
        }

        public bool CanApply(IShape s)
        {
            // is shape state equal to LHSLabel?
            T ss = s as T;
            // right type of structure, right start state, and other checks pass
            return ss != null && s.ShapeState == this.LHSLabel && CanApply(ss);
        }
        public virtual bool CanApply(T s)
        {
            return true;
        }

        public void Apply(IShape s, params object[] p)
        {
            T ss = s as T;
            if (ss != null)
            {
                this.Apply(ss, p);
                ss.ShapeState = this.RHSLabel;
                ss.History.AddRule(new RuleSet(this, p));
            }
        }
        public virtual void Apply(T s, params object[] p)
        {
            // no action, unless overridden in super class
        }
    }
}
