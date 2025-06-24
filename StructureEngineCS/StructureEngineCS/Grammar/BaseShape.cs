// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.BaseShape
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.Distributions;
using StructureEngine.Analysis;
using StructureEngine.Evolutionary;
using StructureEngine.Model;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public abstract class BaseShape : ElementGroup, IShape, IElementGroup, IDesign
  {
    public abstract Structure ConvertToStructure();

    public abstract IDesign DesignClone();

    public abstract IShape Clone();

    public abstract double Score { get; set; }

    public void UpdateScore(double s) => this.Score = s;

    public IAnalysis MyAnalysis { get; set; }

    public abstract bool LooksSame(IShape that);

    public ShapeHistory History { get; set; }

    protected IShape BaseClone(IShape copy)
    {
      copy.History = this.History.Clone();
      copy.Parent1 = this.Parent1;
      copy.Parent2 = this.Parent2;
      copy.SplicePoint1 = this.SplicePoint1;
      copy.SplicePoint2 = this.SplicePoint2;
      copy.Score = this.Score;
      return copy;
    }

    public IShape GoBack()
    {
      if (this.History.Derivation.Count <= 0)
        return (IShape) this;
      IGrammar grammar = this.GetGrammar();
      ShapeHistory shapeHistory = this.History.Clone();
      shapeHistory.GoBack();
      return shapeHistory.ApplyHistory(grammar);
    }

    public IShape Parent1 { get; set; }

    public IShape Parent2 { get; set; }

    public int SplicePoint1 { get; set; }

    public int SplicePoint2 { get; set; }

    public abstract IShapeState ShapeState { get; set; }

    public abstract IGrammar GetGrammar();

    public IDesign Crossover(IList<IDesign> seeds)
    {
      IDesign design;
      if (seeds.Count == 0)
        design = (IDesign) new RandomComputation(this.GetGrammar()).GenerateRandShape();
      else if (seeds.Count == 1)
      {
        design = seeds[0].DesignClone();
      }
      else
      {
        IGrammar grammar = this.GetGrammar();
        IShape seed1 = (IShape) seeds[Utilities.MyRandom.Next(seeds.Count)];
        IShape seed2 = (IShape) seeds[Utilities.MyRandom.Next(seeds.Count)];
        IShape Cross1 = seed1;
        IShape Cross2 = seed2;
        List<IShape> shapeList = grammar.AllCrossover(Cross1, Cross2);
        design = (IDesign) shapeList[Utilities.MyRandom.Next(shapeList.Count)];
      }
      return design;
    }

    public IDesign Mutate(IContinuousDistribution dist, double rate)
    {
      int maxValue = rate != 0.0 ? Convert.ToInt32(Math.Round(Math.Pow(rate, -1.0))) + 1 : 1;
      foreach (RuleSet ruleSet in this.History.Derivation)
      {
        for (int index = 0; index < ruleSet.Rule.Params.Count; ++index)
        {
          if (((IDistribution) dist).RandomSource.Next(maxValue) == 1)
            ruleSet.Param[index] = ruleSet.Rule.Params[index].Mutate(dist, rate, ruleSet.Param[index]);
        }
      }
      return (IDesign) this.History.ApplyHistory(this.GetGrammar());
    }

    public void Setup()
    {
    }

    public IDivBooster GetDivBooster() => (IDivBooster) new GramDivBooster();

    public IList<double> GetFeatures() => throw new NotImplementedException();

    public IList<double[]> GetBounds() => throw new NotImplementedException();

    public double GetOutput() => throw new NotImplementedException();

    public double[] ZeroPoint
    {
      get
      {
        return new double[2]
        {
          this.ZeroShapePoint.X,
          this.ZeroShapePoint.Y
        };
      }
    }

    public int GetMaxPop() => ((BaseGrammar) this.GetGrammar()).UniqueRules.Count * 10;

    public IDesign GenerateFromVars(double[] v) => throw new NotImplementedException();

    public List<IVariable> DesignVariables => throw new NotImplementedException();

    public double? CompTime { get; set; }

    public double[] GetPoints() => throw new NotImplementedException();

    public List<IDesign> GetCornerDesigns() => throw new NotImplementedException();
  }
}
