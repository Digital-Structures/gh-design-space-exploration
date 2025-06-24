// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule010
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule010 : BaseRule<BridgeShape>
  {
    public Rule010()
    {
      this.Name = "Rule 10";
      this.Description = "Adds a second tower based on the first.";
      this.Params.Add((IRuleParameter) new IntParameter(0, 1, "Whether the Tower is Mirrored"));
      this.Params.Add((IRuleParameter) new DoubleParameter(0.5, 1.5, "Horizontal Scale Factor"));
      this.LHSLabel = (IShapeState) BridgeShapeState.MultipleTowers;
      this.RHSLabel = (IShapeState) BridgeShapeState.Subdivide;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      int num = (int) p[0];
      double scalex1 = (double) p[1];
      double scalex2 = 1.0 / (1.0 + scalex1);
      BridgeShape s1 = (BridgeShape) s.Clone();
      switch (num)
      {
        case 0:
          s1.Scale(scalex1, 1.0);
          break;
        case 1:
          s1.Scale(-scalex1, 1.0);
          s1.Infill.Reverse();
          s1.Infill2.Reverse();
          s1.Deck.Reverse();
          using (List<ShapeLine>.Enumerator enumerator = s1.Deck.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.ReverseLine();
            break;
          }
      }
      double x1 = s1.Deck[0].Start.X;
      foreach (ShapeLine shapeLine in s1.Deck)
      {
        if (shapeLine.Start.X < x1)
          x1 = shapeLine.Start.X;
        else if (shapeLine.End.X < x1)
          x1 = shapeLine.End.X;
      }
      double x2 = s.Deck[s.Deck.Count - 1].End.X;
      foreach (ShapeLine shapeLine in s.Deck)
      {
        if (shapeLine.Start.X > x2)
          x2 = shapeLine.Start.X;
        else if (shapeLine.End.X > x2)
          x2 = shapeLine.End.X;
      }
      double x3 = x2 - x1;
      s1.Translate(x3, 0.0);
      s.Integrate(s1);
      s.Scale(scalex2, 1.0);
    }
  }
}
