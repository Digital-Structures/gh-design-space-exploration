// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.BaseGrammar
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Analysis;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public abstract class BaseGrammar : IGrammar
  {
    private List<IRule> _allrules;

    public List<IRule> AllRules
    {
      get
      {
        if (this._allrules != null)
          return this._allrules;
        this._allrules = new List<IRule>();
        foreach (IRule uniqueRule in this.UniqueRules)
        {
          if (uniqueRule.Weight > 0.0)
          {
            for (int index = 0; (double) index < uniqueRule.Weight; ++index)
              this._allrules.Add(uniqueRule);
          }
          else
            this._allrules.Add(uniqueRule);
        }
        return this._allrules;
      }
    }

    public List<IRule> UniqueRules { get; set; }

    public IList<IRule> GetPossibleRules(IShape s)
    {
      List<IRule> possibleRules = new List<IRule>();
      foreach (IRule allRule in this.AllRules)
      {
        if (allRule.CanApply(s))
          possibleRules.Add(allRule);
      }
      return (IList<IRule>) possibleRules;
    }

    protected void SetRuleGrammar()
    {
      foreach (IRule uniqueRule in this.UniqueRules)
        uniqueRule.MyGrammar = (IGrammar) this;
    }

    public abstract IShape GetStartShape();

    public abstract IAnalysis GetAnalysis();

    public IShape[] RandCrossover(Random r, IShape Cross1, IShape Cross2)
    {
      int count1 = Cross1.History.Derivation.Count;
      int num = 0;
      List<int> intList = new List<int>();
      int splice1 = 0;
      while (num == 0)
      {
        splice1 = r.Next(1, count1 - 1);
        intList = this.GetSplicePoints2(splice1, Cross1, Cross2);
        if (intList.Count > 0)
          num = 1;
      }
      int count2 = Cross2.History.Derivation.Count;
      int count3 = intList.Count;
      int index = r.Next(0, count3 - 1);
      int splice2 = intList[index];
      return this.Crossover(splice1, splice2, Cross1, Cross2);
    }

    public List<IShape> AllCrossover(IShape Cross1, IShape Cross2)
    {
      int count = Cross1.History.Derivation.Count;
      List<int> intList = new List<int>();
      List<IShape> shapes = new List<IShape>();
      for (int index1 = 1; index1 < count; ++index1)
      {
        int splice1 = index1;
        List<int> splicePoints2 = this.GetSplicePoints2(splice1, Cross1, Cross2);
        for (int index2 = 0; index2 < splicePoints2.Count; ++index2)
        {
          int splice2 = splicePoints2[index2];
          IShape[] shapeArray = this.Crossover(splice1, splice2, Cross1, Cross2);
          shapes.Add(shapeArray[0]);
          shapes.Add(shapeArray[1]);
        }
      }
      return this.Unique(shapes);
    }

    private List<int> GetSplicePoints2(int splice1, IShape c1, IShape c2)
    {
      ShapeHistory history1 = c1.History;
      ShapeHistory history2 = c2.History;
      int count1 = history1.Derivation.Count;
      ShapeHistory shapeHistory1 = new ShapeHistory();
      ShapeHistory shapeHistory2 = new ShapeHistory();
      ShapeHistory shapeHistory3 = new ShapeHistory();
      shapeHistory1.Derivation = history1.Derivation.GetRange(0, splice1);
      shapeHistory2.Derivation = history1.Derivation.GetRange(splice1, history1.Derivation.Count - splice1);
      IShape s1 = shapeHistory1.ApplyHistory(this.GetStartShape());
      List<int> splicePoints2 = new List<int>();
      foreach (IRule possibleRule in (IEnumerable<IRule>) this.GetPossibleRules(s1))
      {
        foreach (RuleSet ruleSet in history2.Derivation)
        {
          IRule rule = ruleSet.Rule;
          if (string.Compare(possibleRule.Name, rule.Name) == 0)
          {
            int count2 = history2.Derivation.IndexOf(ruleSet);
            if (count2 != 0)
            {
              IShape startShape = this.GetStartShape();
              shapeHistory3.Derivation = history2.Derivation.GetRange(0, count2);
              IShape s2 = shapeHistory3.ApplyHistory(startShape);
              if (shapeHistory2.Derivation[0].Rule.CanApply(s2))
                splicePoints2.Add(count2);
            }
          }
        }
      }
      return splicePoints2;
    }

    private IShape[] Crossover(int splice1, int splice2, IShape c1, IShape c2)
    {
      ShapeHistory history1 = c1.History;
      ShapeHistory history2 = c2.History;
      int count = history1.Derivation.Count;
      ShapeHistory seg1_1;
      ShapeHistory seg2_1;
      history1.Split(splice1, out seg1_1, out seg2_1);
      ShapeHistory seg1_2;
      ShapeHistory seg2_2;
      history2.Split(splice2, out seg1_2, out seg2_2);
      ShapeHistory shapeHistory1 = new ShapeHistory();
      shapeHistory1.Derivation.AddRange((IEnumerable<RuleSet>) seg1_1.Derivation);
      shapeHistory1.Derivation.AddRange((IEnumerable<RuleSet>) seg2_2.Derivation);
      IShape shape1 = shapeHistory1.ApplyHistory((IGrammar) this);
      ShapeHistory shapeHistory2 = new ShapeHistory();
      shapeHistory2.Derivation.AddRange((IEnumerable<RuleSet>) seg1_2.Derivation);
      shapeHistory2.Derivation.AddRange((IEnumerable<RuleSet>) seg2_1.Derivation);
      IShape shape2 = shapeHistory2.ApplyHistory((IGrammar) this);
      shape1.Parent1 = c1;
      shape1.Parent2 = c2;
      shape1.SplicePoint1 = splice1;
      shape1.SplicePoint2 = splice2;
      shape2.Parent1 = c2;
      shape2.Parent2 = c1;
      shape2.SplicePoint1 = splice2;
      shape2.SplicePoint2 = splice1;
      return new IShape[2]{ shape1, shape2 };
    }

    private List<IShape> Unique(List<IShape> shapes)
    {
      List<IShape> shapeList = new List<IShape>();
      foreach (IShape shape in shapes)
      {
        bool flag = true;
        for (int index = 0; index < shapeList.Count; ++index)
        {
          if (shape.LooksSame(shapeList[index]))
          {
            flag = false;
            break;
          }
        }
        if (flag)
          shapeList.Add(shape);
      }
      return shapeList;
    }
  }
}
