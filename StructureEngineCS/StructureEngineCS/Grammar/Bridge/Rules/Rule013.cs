// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule013
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule013 : BaseRule<BridgeShape>
  {
    public Rule013()
    {
      this.Name = "Rule 13";
      this.Description = "Removes cables supporting the deck.";
      this.Params.Add((IRuleParameter) new IntParameter(0, 40, "Support to Remove"));
      this.LHSLabel = (IShapeState) BridgeShapeState.ModifySupports;
      this.RHSLabel = (IShapeState) BridgeShapeState.ModifySupports;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      int num = (int) p[0];
      if (s.Infill2.Count <= 0)
        return;
      int index1 = num % s.Infill2.Count;
      ShapeLine shapeLine1 = s.Infill2[index1];
      s.Infill2.Remove(shapeLine1);
      ShapePoint start = shapeLine1.Start;
      ShapeLine shapeLine2 = (ShapeLine) null;
      ShapeLine shapeLine3 = (ShapeLine) null;
      ShapeLine shapeLine4 = (ShapeLine) null;
      int index2 = 0;
      for (int index3 = 0; index3 < s.Deck.Count; ++index3)
      {
        if (s.Deck[index3].End.IsSame((IElement) start) && s.Deck[index3 + 1].Start.IsSame((IElement) start))
        {
          shapeLine2 = s.Deck[index3];
          shapeLine3 = s.Deck[index3 + 1];
          shapeLine4 = new ShapeLine(s.Deck[index3].Start, s.Deck[index3 + 1].End);
          index2 = index3;
          break;
        }
      }
      s.Deck.Insert(index2, shapeLine4);
      s.Deck.Remove(shapeLine2);
      s.Deck.Remove(shapeLine3);
    }
  }
}
