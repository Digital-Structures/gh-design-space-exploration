// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.RandomComputation
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Analysis;
using StructureEngine.Model;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public class RandomComputation
  {
    public RandomComputation()
    {
    }

    public RandomComputation(IGrammar g) => this.Gram = g;

    public IGrammar Gram { get; set; }

    public List<IShape> RandShapes(int num)
    {
      List<IShape> shapeList = new List<IShape>();
      for (int index = 0; index < num; ++index)
      {
        IShape randShape = this.GenerateRandShape();
        shapeList.Add(randShape);
      }
      return shapeList;
    }

    public IShape GenerateRandShape()
    {
      IShape randShape = this.Gram.GetStartShape();
      while (!randShape.ShapeState.IsEnd())
        randShape = this.ApplyRandRule(randShape);
      IAnalysis analysis = this.Gram.GetAnalysis();
      randShape.Score = analysis.Analyze((IDesign) randShape);
      return randShape;
    }

    public IShape GenerateShapeForRule(IRule r)
    {
      IShape s = this.Gram.GetStartShape();
      for (int index = 0; !r.CanApply(s) && index < 30; ++index)
        s = this.ApplyRandRule(s);
      this.Gram.GetAnalysis();
      return s;
    }

    public IShape ApplyRandRule(IShape s)
    {
      IList<IRule> possibleRules = this.Gram.GetPossibleRules(s);
      int count = possibleRules.Count;
      if (count == 0)
        return s;
      int index = Utilities.MyRandom.Next(0, count);
      IRule rule = possibleRules[index];
      object[] randParams = this.GenerateRandParams(rule);
      rule.Apply(s, randParams);
      return s;
    }

    public object[] GenerateRandParams(IRule rule)
    {
      object[] randParams = new object[rule.Params.Count];
      int index = 0;
      foreach (IRuleParameter ruleParameter in rule.Params)
      {
        object randomValue = ruleParameter.GetRandomValue();
        randParams[index] = randomValue;
        ++index;
      }
      return randParams;
    }
  }
}
