// Decompiled with JetBrains decompiler
// Type: StructureEngine.Test.DSGenerator
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using StructureEngine.MachineLearning;
using StructureEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Test
{
  public class DSGenerator
  {
    private IDesign Problem;

    public DSGenerator()
    {
    }

    public DSGenerator(IDesign problem) => this.Problem = problem;

    public Tuple<Matrix<double>, IList<IDesign>> GetData(IDesign prob, int num)
    {
      this.Problem = prob;
      int count = this.Problem.DesignVariables.Count;
      List<IDesign> list = new Sampling(this.Problem, (IContinuousDistribution) new ContinuousUniform()).GetSample(num, SampleType.RandomLatinHypercube, 0, 0).Select<Observation, IDesign>((Func<Observation, IDesign>) (o => o.obsDesign)).ToList<IDesign>();
      return new Tuple<Matrix<double>, IList<IDesign>>(this.MatfromDes((IList<IDesign>) list), (IList<IDesign>) list);
    }

    public Tuple<Matrix<double>, IList<IDesign>> GetData(
      IDesign prob,
      int num,
      double[] centers,
      double[] ranges)
    {
      int count = prob.DesignVariables.Count;
      if (prob.DesignVariables.Count != centers.Length || prob.DesignVariables.Count != ranges.Length)
        throw new Exception("Centers and ranges must be defined for each design variable.");
      double? allowableVariation;
      for (int index = 0; index < prob.DesignVariables.Count; ++index)
      {
        DOF designVariable = prob.DesignVariables[index] as DOF;
        designVariable.ShiftCenter(centers[index]);
        designVariable.Project(0.5);
        allowableVariation = designVariable.AllowableVariation;
        double range = ranges[index];
        designVariable.AllowableVariation = allowableVariation.HasValue ? new double?(allowableVariation.GetValueOrDefault() * range) : new double?();
      }
      this.Problem = prob;
      List<IDesign> list = new Sampling(this.Problem, (IContinuousDistribution) new ContinuousUniform()).GetSample(num, SampleType.RandomLatinHypercube, 0, 0).Select<Observation, IDesign>((Func<Observation, IDesign>) (o => o.obsDesign)).ToList<IDesign>();
      foreach (IDesign design in list)
      {
        for (int index = 0; index < design.DesignVariables.Count; ++index)
        {
          DOF designVariable = design.DesignVariables[index] as DOF;
          allowableVariation = designVariable.AllowableVariation;
          double range = ranges[index];
          designVariable.AllowableVariation = allowableVariation.HasValue ? new double?(allowableVariation.GetValueOrDefault() / range) : new double?();
          double point = designVariable.GetPoint();
          designVariable.ShiftCenter(1.0 - centers[index]);
          designVariable.Project(point - (0.5 - centers[index]));
        }
      }
      return new Tuple<Matrix<double>, IList<IDesign>>(this.MatfromDes((IList<IDesign>) list), (IList<IDesign>) list);
    }

    public Matrix<double> MatfromDes(IList<IDesign> designs)
    {
      int count1 = designs.Count;
      int count2 = designs[0].DesignVariables.Count;
      Matrix<double> matrix = (Matrix<double>) CreateMatrix.Dense<double>(count1, count2 + 1);
      for (int index = 0; index < count1; ++index)
      {
        IDesign design = designs[index];
        List<double> list = design.DesignVariables.Select<IVariable, double>((Func<IVariable, double>) (v => v.GetPoint())).ToList<double>();
        list.Add(design.Score);
        matrix.SetRow(index, list.ToArray());
      }
      return matrix;
    }

    public IList<Tuple<Matrix<double>, IList<IDesign>>> GetBaseData(
      IDesign prob,
      int num,
      int numFixed)
    {
      List<Tuple<Matrix<double>, IList<IDesign>>> baseData = new List<Tuple<Matrix<double>, IList<IDesign>>>();
      List<IDesign> runningList = new List<IDesign>();
      this.SetFixed((IList<IDesign>) runningList, prob.DesignClone(), 0, numFixed);
      foreach (IDesign prob1 in runningList)
      {
        Tuple<Matrix<double>, IList<IDesign>> data = this.GetData(prob1, num);
        baseData.Add(data);
      }
      return (IList<Tuple<Matrix<double>, IList<IDesign>>>) baseData;
    }

    public void SetFixed(IList<IDesign> runningList, IDesign des, int varIndex, int toFix)
    {
      if (toFix == 0)
      {
        runningList.Add(des);
      }
      else
      {
        if (varIndex >= des.DesignVariables.Count)
          return;
        IDesign des1 = des.DesignClone();
        IVariable designVariable = des1.DesignVariables[varIndex];
        DOF dof = designVariable is DOF ? (DOF) designVariable : throw new Exception("Method only works for ComputedStructure objects.");
        dof.Free = false;
        dof.AllowableVariation = new double?(0.0);
        this.SetFixed(runningList, des1, varIndex, toFix - 1);
        this.SetFixed(runningList, des, varIndex + 1, toFix);
      }
    }

    public IDesign GetDesign(double[] vars) => this.Problem.GenerateFromVars(vars);
  }
}
