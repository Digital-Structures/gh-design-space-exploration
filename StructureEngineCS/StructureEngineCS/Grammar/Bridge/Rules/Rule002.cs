// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule002
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule002 : BaseRule<BridgeShape>
  {
    public Rule002()
    {
      this.Name = "Rule 02";
      this.Description = "Rotates a tower element about its base.";
      this.Params.Add((IRuleParameter) new DoubleParameter(-10.0, 10.0, "Rotation Angle"));
      this.LHSLabel = (IShapeState) BridgeShapeState.MakeTower;
      this.RHSLabel = (IShapeState) BridgeShapeState.MakeTower;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      double angle = (double) p[0];
      s.Rotate(angle, s.Tower[0].Start);
    }
  }
}
