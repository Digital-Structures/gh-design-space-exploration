using System.Collections.Generic;
using StructureEngine.Analysis;

namespace StructureEngine.Grammar
{
    public interface IGrammar
    {
        List<IRule> AllRules
        {
            get;
        }
        List<IRule> UniqueRules
        {
            get;
            set;
        }
        IShape GetStartShape();
        IList<IRule> GetPossibleRules(IShape s);
        IAnalysis GetAnalysis();
        List<IShape> AllCrossover(IShape Cross1, IShape Cross2);
    }
}
