using StructureEngine.MachineLearning;
using StructureEngine.Model;

namespace StructureEngine.Analysis
{
    public class RegAnalysis : IAnalysis
    {
        public RegAnalysis(Regression r)
        {
            Reg = r;
        }

        private Regression Reg;

        public double Analyze(IDesign comp)
        {
            ComputedStructure cs = (ComputedStructure)comp;
            Observation obs = new Observation(cs); // TODO: take a computed structure?
            double score = Reg.Predict(obs);

            //Results r = new Results();
            //r.Weight = score;

            //cs.Result = r;

            return score;
        }
    }
}
