// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule004
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule004 : BaseRule<BridgeShape>
  {
    public Rule004()
    {
      this.Name = "Rule 04";
      this.Description = "Deletes tower branches.";
      this.Params.Add((IRuleParameter) new IntParameter(0, 20, "Branch to Remove"));
      this.LHSLabel = (IShapeState) BridgeShapeState.ModifyTower;
      this.RHSLabel = (IShapeState) BridgeShapeState.ModifyTower;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      int num1 = (int) p[0];
      int num2 = s.Tower.Count - 1;
      if (num2 <= 1)
        return;
      int index = num1 % num2 + 1;
      ShapeLine shapeLine = s.Tower[index];
      s.Tower.Remove(shapeLine);
      s.Tops.Remove(shapeLine.End);
    }
  }
}
