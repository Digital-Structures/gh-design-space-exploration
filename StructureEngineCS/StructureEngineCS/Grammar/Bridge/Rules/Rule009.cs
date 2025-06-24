// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule009
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule009 : BaseRule<BridgeShape>
  {
    public Rule009()
    {
      this.Name = "Rule 09";
      this.Description = "Adds cable outline.";
      this.LHSLabel = (IShapeState) BridgeShapeState.MakeInfill;
      this.RHSLabel = (IShapeState) BridgeShapeState.MultipleTowers;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      ShapePoint start = s.Deck[0].Start;
      ShapePoint end = s.Deck[0].End;
      List<ShapePoint> shapePointList = new List<ShapePoint>();
      shapePointList.Add(start);
      shapePointList.AddRange((IEnumerable<ShapePoint>) s.Tops);
      shapePointList.Add(end);
      for (int index = 0; index < shapePointList.Count - 1; ++index)
      {
        ShapeLine shapeLine = new ShapeLine(shapePointList[index], shapePointList[index + 1]);
        s.Infill.Add(shapeLine);
      }
    }
  }
}
