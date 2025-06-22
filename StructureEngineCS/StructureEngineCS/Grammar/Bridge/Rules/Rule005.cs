// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule005
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule005 : BaseRule<BridgeShape>
  {
    public Rule005()
    {
      this.Name = "Rule 05";
      this.Description = "Changes the length of tower branches.";
      this.Params.Add((IRuleParameter) new DoubleParameter(0.5, 1.5, "Extension or Contraction"));
      this.Params.Add((IRuleParameter) new IntParameter(0, 20, "Which Branch to Adjust"));
      this.LHSLabel = (IShapeState) BridgeShapeState.ModifyTower;
      this.RHSLabel = (IShapeState) BridgeShapeState.ModifyTower;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      double scale = (double) p[0];
      int num1 = (int) p[1];
      int num2 = s.Tower.Count - 1;
      if (num2 <= 1)
        return;
      int index = num1 % num2 + 1;
      s.Tower[index].Scale(scale);
    }
  }
}
