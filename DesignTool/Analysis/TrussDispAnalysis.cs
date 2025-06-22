using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic;
using StructureEngine.Model;

namespace StructureEngine.Analysis
{
    public class TrussDispAnalysis : BaseTrussAnalysis, IAnalysis
    {
        public double Analyze(IDesign d)
        {
            ComputedStructure comp = (ComputedStructure)d;
            try
            {
                if (!comp.Analyzed)
                {
                    this.RunAnalysis(comp);
                    comp.Analyzed = true;
                }

                // Compute maximum displacement
                List<double> nodeDisp = new List<double>();
                foreach (DOF dof in comp.DOFs)
                {
                    double disp = dof.Disp.Values.Select(x => Math.Abs(x)).Max();
                    nodeDisp.Add(disp);
                }
                return nodeDisp.Max();
            }
            catch
            {
                return Double.NaN;
            }
        }
    }
}
