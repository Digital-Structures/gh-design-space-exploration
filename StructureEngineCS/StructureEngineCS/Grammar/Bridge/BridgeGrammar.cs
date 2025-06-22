// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.BridgeGrammar
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Analysis;
using StructureEngine.Grammar.Bridge.Rules;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Bridge
{
  public class BridgeGrammar : BaseGrammar
  {
    public double DeckDistLoad;
    public double DeckHeight;
    public double ClearSpan;

    public BridgeGrammar()
    {
      this.LoadRules();
      this.SetRuleGrammar();
    }

    public override IShape GetStartShape() => (IShape) new BridgeShape();

    private void LoadRules()
    {
      this.UniqueRules = new List<IRule>();
      this.UniqueRules.Add((IRule) new Rule001());
      this.UniqueRules.Add((IRule) new Rule002());
      this.UniqueRules.Add((IRule) new Rule003());
      this.UniqueRules.Add((IRule) new Rule004());
      this.UniqueRules.Add((IRule) new Rule005());
      this.UniqueRules.Add((IRule) new Rule006());
      this.UniqueRules.Add((IRule) new Rule007());
      this.UniqueRules.Add((IRule) new Rule008());
      this.UniqueRules.Add((IRule) new Rule009());
      this.UniqueRules.Add((IRule) new Rule010());
      this.UniqueRules.Add((IRule) new Rule011());
      this.UniqueRules.Add((IRule) new Rule012());
      this.UniqueRules.Add((IRule) new Rule013());
      this.UniqueRules.Add((IRule) new Rule014());
      this.UniqueRules.Add((IRule) new Rule015());
      this.UniqueRules.Add((IRule) new Rule016());
      this.UniqueRules.Add((IRule) new Rule017());
      this.UniqueRules.Add((IRule) new RuleA());
      this.UniqueRules.Add((IRule) new RuleB());
      this.UniqueRules.Add((IRule) new RuleC());
      this.UniqueRules.Add((IRule) new RuleD());
    }

    public override IAnalysis GetAnalysis() => (IAnalysis) new BridgeAnalysis();
  }
}
