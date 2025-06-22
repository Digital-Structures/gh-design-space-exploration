// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.ShapeHistory
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public class ShapeHistory
  {
    public ShapeHistory() => this.Derivation = new List<RuleSet>();

    public void AddRule(RuleSet r) => this.Derivation.Add(r);

    public ShapeHistory Clone()
    {
      ShapeHistory shapeHistory = new ShapeHistory();
      List<RuleSet> ruleSetList = new List<RuleSet>();
      foreach (RuleSet ruleSet in this.Derivation)
        ruleSetList.Add(ruleSet.Clone());
      shapeHistory.Derivation = ruleSetList;
      return shapeHistory;
    }

    public void GoBack() => this.Derivation.RemoveAt(this.Derivation.Count - 1);

    public IShape ApplyHistory(IShape s)
    {
      if (this.Derivation.Count <= 0 || !this.Derivation[0].Rule.CanApply(s))
        return s;
      IShape s1 = s.Clone();
      foreach (RuleSet ruleSet in this.Derivation)
        ruleSet.Rule.Apply(s1, ruleSet.Param);
      return s1;
    }

    public IShape ApplyHistory(IGrammar g) => this.ApplyHistory(g.GetStartShape());

    public void Split(int i, out ShapeHistory seg1, out ShapeHistory seg2)
    {
      seg1 = new ShapeHistory();
      seg1.Derivation = this.Derivation.GetRange(0, i);
      seg2 = new ShapeHistory();
      seg2.Derivation = this.Derivation.GetRange(i, this.Derivation.Count - i);
    }

    public List<RuleSet> Derivation { get; set; }
  }
}
