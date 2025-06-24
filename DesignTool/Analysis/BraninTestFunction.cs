using System;
using StructureEngine.Model;

namespace StructureEngine.Analysis
{
    public class BraninTestFunction : IAnalysis
    {
        public BraninTestFunction()
        {
        }

        public double Analyze(IDesign d)
        {
            BraninObject b = d as BraninObject;
            if (b != null)
            {
                double x1 = b.DesignVariables[0].Value;
                double x2 = b.DesignVariables[1].Value;
                return ComputeBranin(x1, x2);
            }
            else
            {
                throw new Exception("Can only analyze a Branin Object");
            }
        }

        private double ComputeBranin(double x1, double x2)
        {
            return Math.Pow(x2 - (5.1 / (4 * Math.Pow(Math.PI, 2))) * x2 + (5 / Math.PI) * x1 - 6, 2) + 10 * ((1 - (1 / (8 * Math.PI))) * Math.Cos(x1) + 1) + 5 * x1;
        }
    }
}
