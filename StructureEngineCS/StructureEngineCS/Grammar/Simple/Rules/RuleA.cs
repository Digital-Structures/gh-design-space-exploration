// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Simple.Rules.RuleA
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Grammar.Simple.Rules
{
  public class RuleA : BaseRule<SimpleShape>
  {
    public RuleA()
    {
      this.Name = "Rule A";
      this.LHSLabel = (IShapeState) SimpleShapeState.SubdivideDeck;
      this.RHSLabel = (IShapeState) SimpleShapeState.AddFunicular;
      this.Description = "Changes state label.";
    }

    public override bool CanApply(SimpleShape s) => s.Verticals.Count != 0;
  }
}
