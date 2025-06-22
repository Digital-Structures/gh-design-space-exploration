// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Simple.SimpleGrammar
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Analysis;
using StructureEngine.Grammar.Simple.Rules;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Simple
{
  public class SimpleGrammar : BaseGrammar
  {
    public SimpleGrammar()
    {
      this.LoadRules();
      this.SetRuleGrammar();
    }

    public override IShape GetStartShape() => (IShape) new SimpleShape();

    private void LoadRules()
    {
      this.UniqueRules = new List<IRule>();
      this.UniqueRules.Add((IRule) new Rule001());
      this.UniqueRules.Add((IRule) new Rule002());
      this.UniqueRules.Add((IRule) new Rule003());
      this.UniqueRules.Add((IRule) new RuleA());
    }

    public override IAnalysis GetAnalysis() => (IAnalysis) new SimpleAnalysis();
  }
}
