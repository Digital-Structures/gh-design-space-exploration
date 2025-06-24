// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule016
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule016 : BaseRule<BridgeShape>
  {
    public Rule016()
    {
      this.Name = "Rule 16";
      this.Description = "Connects cables in a parallel configuration.";
      this.LHSLabel = (IShapeState) BridgeShapeState.ConnectSupports;
      this.RHSLabel = (IShapeState) BridgeShapeState.End;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      List<ShapePoint> shapePointList = new List<ShapePoint>();
      foreach (ShapeLine shapeLine in s.Deck)
      {
        foreach (ShapeLine that in s.Tower)
        {
          if (shapeLine.IsPointOnLine(shapeLine.FindIntersection(that)) && that.IsPointOnLine(shapeLine.FindIntersection(that)))
          {
            ShapePoint intersection = shapeLine.FindIntersection(that);
            bool flag = false;
            foreach (ShapePoint elem in shapePointList)
            {
              if (intersection.IsSame((IElement) elem))
              {
                flag = true;
                break;
              }
            }
            if (!flag)
              shapePointList.Add(intersection);
          }
        }
        foreach (ShapeLine that in s.Infill)
        {
          if (shapeLine.IsPointOnLine(shapeLine.FindIntersection(that)) && that.IsPointOnLine(shapeLine.FindIntersection(that)))
          {
            ShapePoint intersection = shapeLine.FindIntersection(that);
            bool flag = false;
            foreach (ShapePoint elem in shapePointList)
            {
              if (intersection.IsSame((IElement) elem))
              {
                flag = true;
                break;
              }
            }
            if (!flag)
              shapePointList.Add(intersection);
          }
        }
      }
      shapePointList.Sort((Comparison<ShapePoint>) ((p1, p2) => p1.X.CompareTo(p2.X)));
      foreach (ShapeLine shapeLine1 in s.Infill2)
      {
        double x = shapeLine1.Start.X;
        if (x >= shapePointList[0].X)
        {
          ShapeLine shapeLine2 = (ShapeLine) null;
          if (shapePointList.Count == s.Infill.Count + 1)
          {
            for (int index = 1; index < shapePointList.Count; ++index)
            {
              if (x < shapePointList[index].X)
              {
                shapeLine2 = s.Infill[index - 1];
                break;
              }
            }
          }
          else
          {
            shapeLine1.End.X = shapeLine1.Start.X;
            shapeLine2 = (ShapeLine) null;
            double num = double.MaxValue;
            foreach (ShapeLine that in s.Infill)
            {
              ShapePoint intersection = shapeLine1.FindIntersection(that);
              if (that.IsPointOnLine(intersection))
              {
                double distance = shapeLine1.Start.GetDistance(intersection);
                if (distance < num)
                {
                  num = distance;
                  shapeLine2 = that;
                }
              }
            }
          }
          if (shapeLine2 != null)
          {
            double rotation = shapeLine2.Rotation;
            ShapeLine shapeLine3 = new ShapeLine(shapeLine1.Start, rotation, shapeLine1.Length);
            shapeLine1.End = shapeLine3.End;
            ShapeLine that1 = (ShapeLine) null;
            double num = double.MaxValue;
            foreach (ShapeLine that2 in s.Tower)
            {
              ShapePoint intersection = shapeLine1.FindIntersection(that2);
              if (that2.IsPointOnLine(intersection) && intersection.Y >= s.Deck[0].Start.Y)
              {
                double distance = shapeLine1.Start.GetDistance(intersection);
                if (distance < num)
                {
                  num = distance;
                  that1 = that2;
                }
              }
            }
            if (that1 == null)
            {
              shapeLine1.End = shapeLine1.Start;
            }
            else
            {
              ShapePoint intersection = shapeLine1.FindIntersection(that1);
              shapeLine1.End = intersection;
            }
          }
          else
            break;
        }
        else
          break;
      }
      for (int index = 0; index < s.Infill.Count; ++index)
      {
        foreach (ShapeLine line2 in s.Infill2)
        {
          ShapeLine shapeLine = s.Infill[index];
          if (shapeLine.HasCommonPoints(line2))
          {
            s.Infill.Remove(shapeLine);
            break;
          }
        }
      }
    }
  }
}
