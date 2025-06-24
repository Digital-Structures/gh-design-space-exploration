// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule012
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule012 : BaseRule<BridgeShape>
  {
    public Rule012()
    {
      this.Name = "Rule 12";
      this.Description = "Adds support cables at deck subdivision points.";
      this.Params.Add((IRuleParameter) new DoubleParameter(80.0, 100.0, "Angle of Support Cables"));
      this.LHSLabel = (IShapeState) BridgeShapeState.AddSupports;
      this.RHSLabel = (IShapeState) BridgeShapeState.ModifySupports;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      double angle = (double) p[0];
      foreach (ShapePoint deckDiv in s.DeckDivs)
      {
        ShapeLine shapeLine = new ShapeLine(deckDiv, angle, 20.0);
        s.Infill2.Add(shapeLine);
      }
      s.DeckPoints.Clear();
    }
  }
}
