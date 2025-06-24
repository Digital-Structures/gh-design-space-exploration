// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.RuleSet
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Grammar
{
  public class RuleSet
  {
    public RuleSet(IRule rule, object[] par)
    {
      this.Rule = rule;
      this.Param = par;
    }

    public IRule Rule { get; set; }

    public object[] Param { get; set; }

    public RuleSet Clone()
    {
      IRule rule = this.Rule;
      object[] par = new object[this.Param.Length];
      for (int index = 0; index < this.Param.Length; ++index)
      {
        object obj = this.Param[index];
        par[index] = obj;
      }
      return new RuleSet(rule, par);
    }
  }
}
