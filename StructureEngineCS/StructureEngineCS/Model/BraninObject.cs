// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.BraninObject
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.Distributions;
using StructureEngine.Analysis;
using StructureEngine.Evolutionary;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Model
{
  public class BraninObject : BaseDesign, IDesign
  {
    public DOF x1;
    public DOF x2;

    public BraninObject(double num1, double num2)
    {
      this.x1 = new DOF(2.5);
      this.x2 = new DOF(7.5);
      this.x1.AllowableVariation = new double?(7.5);
      this.x2.AllowableVariation = new double?(7.5);
      this.x1.Value = num1;
      this.x2.Value = num2;
    }

    public double GetOutput() => new BraninTestFunction().Analyze((IDesign) this);

    public double Score
    {
      get => this.GetOutput();
      private set => this.Score = value;
    }

    public void UpdateScore(double s) => this.Score = s;

    public IAnalysis MyAnalysis { get; set; }

    public IDesign DesignClone() => (IDesign) new BraninObject(this.x1.Value, this.x2.Value);

    public override IDesign GenerateFromVars(double[] v)
    {
      BraninObject fromVars = (BraninObject) this.DesignClone();
      for (int index = 0; index < v.Length; ++index)
        fromVars.DesignVariables[index].Project(v[index]);
      return (IDesign) fromVars;
    }

    public override List<IVariable> DesignVariables
    {
      get
      {
        return new List<IVariable>()
        {
          (IVariable) this.x1,
          (IVariable) this.x2
        };
      }
    }

    public IDesign Crossover(IList<IDesign> seeds) => throw new NotImplementedException();

    public IDesign Mutate(IContinuousDistribution dist, double rate)
    {
      BraninObject braninObject = (BraninObject) this.DesignClone();
      foreach (IVariable designVariable in braninObject.DesignVariables)
      {
        designVariable.Mutate(rate, dist);
        designVariable.FixConstraint();
      }
      return (IDesign) braninObject;
    }

    public void Setup() => throw new NotImplementedException();

    public IDivBooster GetDivBooster() => throw new NotImplementedException();

    public double[] Dimensions => throw new NotImplementedException();

    public double[] ZeroPoint => throw new NotImplementedException();

    public int GetMaxPop()
    {
      int num = 0;
      foreach (IVariable designVariable in this.DesignVariables)
        num += designVariable.GetBytes();
      return Math.Min(num * 4 * 2, 150);
    }
  }
}
