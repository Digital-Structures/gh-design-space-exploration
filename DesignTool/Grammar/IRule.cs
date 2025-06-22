using System.Collections.Generic;

namespace StructureEngine.Grammar
{
    public interface IRule
    {
        IGrammar MyGrammar
        {
            get;
            set;
        }

        string Name
        {
            get;
        }

        double Weight
        {
            get;
        }

        string Description
        {
            get;
        }

        List<IRuleParameter> Params 
        { 
            get; 
        }

        bool CanApply(IShape s);

        IShapeState LHSLabel
        {
            get;
        }
        IShapeState RHSLabel
        {
            get;
        }

        //public void SetParams(params object[] p)
        //{
        //    int i = 0;
        //    foreach (object o in p)
        //    {
        //        Params[i].GetType()
        //        Params[i] = (t)o;
        //        i++;
        //    }
        //}

        void Apply(IShape s, params object[] p);
    }
}
