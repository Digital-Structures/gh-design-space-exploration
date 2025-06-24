// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Airport.Rules.Rule003
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Airport.Rules
{
  public class Rule003 : BaseRule<AirportShape>
  {
    public Rule003()
    {
      this.Name = "Rule 03";
      this.LHSLabel = (IShapeState) AirportShapeState.AddVerticals;
      this.RHSLabel = (IShapeState) AirportShapeState.End;
    }

    public override void Apply(AirportShape s, params object[] p)
    {
      List<ShapePoint> shapePointList = new List<ShapePoint>();
      foreach (ShapePoint point in s.Points)
      {
        ShapePoint shapePoint = point.ReflectAcross(new ShapeLine(s.Start, s.End));
        shapePointList.Add(shapePoint);
      }
      List<ShapeLine> collection = new List<ShapeLine>();
      for (int index = 0; index < shapePointList.Count - 1; ++index)
      {
        ShapeLine shapeLine = new ShapeLine(shapePointList[index], shapePointList[index + 1]);
        collection.Add(shapeLine);
      }
      s.Roof.AddRange((IEnumerable<ShapeLine>) collection);
    }
  }
}
