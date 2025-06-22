// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule007
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule007 : BaseRule<BridgeShape>
  {
    public Rule007()
    {
      this.Name = "Rule 07";
      this.Description = "Adds a horizontal deck.";
      this.Params.Add((IRuleParameter) new IntParameter(60, 60, "Length of Deck"));
      this.Params.Add((IRuleParameter) new DoubleParameter(0.3, 0.7, "Centering of Deck"));
      this.Params.Add((IRuleParameter) new IntParameter(30, 30, "Height of Deck"));
      this.LHSLabel = (IShapeState) BridgeShapeState.MakeDeck;
      this.RHSLabel = (IShapeState) BridgeShapeState.MakeInfill;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      int num1 = (int) p[0];
      double num2 = (double) p[1];
      int y = (int) p[2];
      ShapePoint i = new ShapePoint(s.ZeroShapePoint.X + s.Dimensions[0] / 2.0 - (double) num1 * num2, (double) y);
      s.Deck.Add(new ShapeLine(i, new ShapePoint(i.X + (double) num1, (double) y))
      {
        DistLoad = 10.0
      });
    }
  }
}
