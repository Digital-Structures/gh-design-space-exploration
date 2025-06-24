// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.RuleD
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class RuleD : BaseRule<BridgeShape>
  {
    public RuleD()
    {
      this.Name = "Rule D";
      this.Description = "Changes state label.";
      this.LHSLabel = (IShapeState) BridgeShapeState.ModifySupports;
      this.RHSLabel = (IShapeState) BridgeShapeState.ConnectSupports;
    }
  }
}
