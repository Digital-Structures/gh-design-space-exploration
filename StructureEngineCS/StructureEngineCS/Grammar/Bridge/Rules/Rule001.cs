// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule001
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule001 : BaseRule<BridgeShape>
  {
    public Rule001()
    {
      this.Name = "Rule 01";
      this.Description = "Sets the height of the tower.";
      this.Params.Add((IRuleParameter) new IntParameter(40, 80, "Tower Height"));
      this.LHSLabel = (IShapeState) BridgeShapeState.MakeTower;
      this.RHSLabel = (IShapeState) BridgeShapeState.AddBranches;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      double scale = (double) (int) p[0] / s.Tower[0].Length;
      s.Tower[0].Scale(scale);
    }
  }
}
