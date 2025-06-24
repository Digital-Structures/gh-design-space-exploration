using StructureEngine.Model;

namespace StructureEngine.Analysis
{
    public interface IAnalysis
    {
        double Analyze(IDesign d);
    }
}
