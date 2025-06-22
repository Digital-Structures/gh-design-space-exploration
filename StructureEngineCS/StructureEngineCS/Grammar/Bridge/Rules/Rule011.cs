// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule011
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule011 : BaseRule<BridgeShape>
  {
    public Rule011()
    {
      this.Name = "Rule 11";
      this.Description = "Divides the deck.";
      this.Params.Add((IRuleParameter) new IntParameter(4, 20, "Number of Deck Subdivisions"));
      this.LHSLabel = (IShapeState) BridgeShapeState.Subdivide;
      this.RHSLabel = (IShapeState) BridgeShapeState.AddSupports;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      int num = (int) p[0];
      List<ShapePoint> shapePointList = new List<ShapePoint>();
      List<ShapeLine> shapeLineList = new List<ShapeLine>();
      foreach (ShapeLine shapeLine1 in s.Deck)
      {
        List<ShapePoint> collection = shapeLine1.Subdivide(num);
        if (s.Deck.IndexOf(shapeLine1) != 0)
          shapePointList.Add(shapeLine1.Start);
        shapePointList.AddRange((IEnumerable<ShapePoint>) collection);
        if (num > 1)
        {
          shapeLineList.Add(new ShapeLine(shapeLine1.Start, collection[0]));
          for (int index = 0; index < collection.Count - 1; ++index)
          {
            ShapeLine shapeLine2 = new ShapeLine(collection[index], collection[index + 1]);
            shapeLineList.Add(shapeLine2);
          }
          shapeLineList.Add(new ShapeLine(collection[collection.Count - 1], shapeLine1.End));
        }
        else if (num == 1)
        {
          collection.Add(shapeLine1.Start);
          shapeLineList.Add(new ShapeLine(shapeLine1.Start, shapeLine1.End));
        }
      }
      s.Deck = shapeLineList;
      s.DeckDivs = shapePointList;
      s.DeckPoints = s.DeckDivs.GetRange(0, s.DeckDivs.Count);
    }
  }
}
