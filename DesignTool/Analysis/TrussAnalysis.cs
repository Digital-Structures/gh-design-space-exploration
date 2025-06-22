using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic;
using StructureEngine.Model;

namespace StructureEngine.Analysis
{
    public class TrussAnalysis : BaseTrussAnalysis, IAnalysis
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

                // Compute member member weights
                List<double> memberWeights = new List<double>();
                foreach (ComputedMember m in comp.ComputedMembers)
                {
                    memberWeights.Add(m.Weight);
                }

                return memberWeights.Sum();


                //if (comp.GetStable() == Structure.StabType.Indeterminate)
                //{
                //    bool closeEnough = CheckMemberSizes(comp);
                //    while (!closeEnough)
                //    {
                //        this.RunAnalysis(comp);
                //        closeEnough = CheckMemberSizes(comp);
                //    }
                //}
                //return comp.Result.Weight;

            }
            catch
            {
                return Double.NaN;
            }
        }

        


    }
}
