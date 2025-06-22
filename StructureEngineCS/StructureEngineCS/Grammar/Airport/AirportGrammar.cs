// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Airport.AirportGrammar
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Analysis;
using StructureEngine.Grammar.Airport.Rules;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Airport
{
  public class AirportGrammar : BaseGrammar
  {
    public AirportGrammar()
    {
      this.LoadRules();
      this.SetRuleGrammar();
    }

    public override IShape GetStartShape() => (IShape) new AirportShape();

    private void LoadRules()
    {
      this.UniqueRules = new List<IRule>();
      this.UniqueRules.Add((IRule) new Rule001());
      this.UniqueRules.Add((IRule) new Rule002());
      this.UniqueRules.Add((IRule) new Rule004());
      this.UniqueRules.Add((IRule) new Rule005());
      this.UniqueRules.Add((IRule) new RuleA());
    }

    public override IAnalysis GetAnalysis() => throw new NotImplementedException();
  }
}
