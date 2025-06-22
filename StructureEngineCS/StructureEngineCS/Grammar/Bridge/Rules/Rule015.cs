// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule015
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule015 : BaseRule<BridgeShape>
  {
    public Rule015()
    {
      this.Name = "Rule 15";
      this.Description = "Connects each cable to the tower top resulting in the steepest slope.";
      this.LHSLabel = (IShapeState) BridgeShapeState.ConnectSupports;
      this.RHSLabel = (IShapeState) BridgeShapeState.End;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      List<ShapePoint> tops = s.Tops;
      for (int index1 = 0; index1 < s.Infill.Count; ++index1)
      {
        for (int index2 = index1 + 1; index2 < s.Infill.Count; ++index2)
        {
          ShapeLine shapeLine = s.Infill[index1];
          ShapeLine line2 = s.Infill[index2];
          if (shapeLine.HasCommonPoint(line2) && shapeLine.FindCommonPoint(line2).Y == s.Deck[0].Start.Y)
          {
            s.Infill.Remove(shapeLine);
            s.Infill.Remove(line2);
            break;
          }
        }
      }
      foreach (ShapeLine shapeLine1 in s.Infill2)
      {
        ShapePoint shapePoint1 = tops[0];
        ShapeLine shapeLine2 = shapeLine1.Clone();
        shapeLine2.End = shapePoint1;
        double num1 = Math.Abs(shapeLine2.SlopeIntercept()[0]);
        foreach (ShapePoint shapePoint2 in tops)
        {
          shapeLine2.End = shapePoint2;
          double num2 = Math.Abs(shapeLine2.SlopeIntercept()[0]);
          if (num2 > num1)
          {
            num1 = num2;
            shapePoint1 = shapePoint2;
          }
        }
        shapeLine1.End = shapePoint1;
      }
      List<ShapePoint> shapePointList = new List<ShapePoint>();
      shapePointList.AddRange((IEnumerable<ShapePoint>) s.DeckDivs);
      shapePointList.Add(s.Deck[0].Start);
      shapePointList.Add(s.Deck[s.Deck.Count - 1].End);
      foreach (ShapePoint point in s.Points)
      {
        if (!shapePointList.Contains(point) && Math.Abs(point.Y - s.Deck[0].Start.Y) <= 0.1)
        {
          ShapePoint shapePoint3 = shapePointList[0];
          double num = double.MaxValue;
          foreach (ShapePoint shapePoint4 in shapePointList)
          {
            if (shapePoint4.GetDistance(point) < num)
            {
              num = shapePoint4.GetDistance(point);
              shapePoint3 = shapePoint4;
            }
          }
          point.X = shapePoint3.X;
        }
      }
    }
  }
}
