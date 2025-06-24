// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule017
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule017 : BaseRule<BridgeShape>
  {
    public Rule017()
    {
      this.Name = "Rule 17";
      this.Description = "Connects support cables to suspension cables.";
      this.Params.Add((IRuleParameter) new DoubleParameter(0.01, 0.03, "Cable Slack"));
      this.LHSLabel = (IShapeState) BridgeShapeState.ConnectSupports;
      this.RHSLabel = (IShapeState) BridgeShapeState.End;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      double num1 = (double) p[0];
      for (int index1 = 0; index1 < s.Infill.Count; ++index1)
      {
        for (int index2 = index1 + 1; index2 < s.Infill.Count; ++index2)
        {
          ShapeLine shapeLine1 = s.Infill[index1];
          ShapeLine line2 = s.Infill[index2];
          if (shapeLine1.HasCommonPoint(line2) && shapeLine1.FindCommonPoint(line2).Y == s.Deck[0].Start.Y)
          {
            ShapePoint commonPoint = shapeLine1.FindCommonPoint(line2);
            ShapePoint i = shapeLine1.Start;
            if (i.IsSame((IElement) commonPoint))
              i = shapeLine1.End;
            ShapePoint j = line2.End;
            if (j.IsSame((IElement) commonPoint))
              j = line2.Start;
            ShapeLine shapeLine2 = new ShapeLine(i, j);
            s.Infill.Remove(shapeLine1);
            s.Infill.Remove(line2);
            s.Infill.Add(shapeLine2);
            break;
          }
        }
      }
      foreach (ShapeLine shapeLine in s.Infill2)
      {
        ShapeLine that1 = s.Infill[0];
        double num2 = double.MaxValue;
        foreach (ShapeLine that2 in s.Infill)
        {
          ShapePoint intersection = shapeLine.FindIntersection(that2);
          if (that2.IsPointOnLine(intersection))
          {
            double distance = shapeLine.Start.GetDistance(intersection);
            if (distance < num2)
            {
              num2 = distance;
              that1 = that2;
            }
          }
        }
        ShapePoint intersection1 = shapeLine.FindIntersection(that1);
        shapeLine.End = intersection1;
        that1.Loads.Add(new ShapePointLoad(intersection1, 1.0, shapeLine.Rotation));
      }
      List<ShapeLine> shapeLineList = new List<ShapeLine>();
      foreach (ShapeLine shapeLine3 in s.Infill)
      {
        int count = shapeLine3.Loads.Count;
        shapeLine3.Loads.Sort((IComparer<ShapePointLoad>) new ByXCoordComparer());
        if (count > 0)
        {
          double direction = shapeLine3.Loads[0].Direction;
          double num3 = 90.0 + direction;
          ShapePoint start = shapeLine3.Start;
          double a = num1;
          double num4 = Math.Pow(shapeLine3.Length, 2.0) / a;
          ShapeLine shapeLine4 = shapeLine3.Clone();
          double[] numArray1 = shapeLine4.Start.RotateCoords(start, -num3);
          double[] numArray2 = shapeLine4.End.RotateCoords(start, -num3);
          shapeLine4.Start.X = numArray1[0];
          shapeLine4.Start.Y = numArray1[1];
          shapeLine4.End.X = numArray2[0];
          shapeLine4.End.Y = numArray2[1];
          double[] numArray3 = shapeLine4.Parabola_bc(a);
          double num5 = numArray3[0];
          double num6 = numArray3[1];
          foreach (ShapePointLoad load in shapeLine3.Loads)
          {
            double[] numArray4 = load.Point.RotateCoords(start, -num3);
            double x = numArray4[0];
            double num7 = a * Math.Pow(x, 2.0) + num5 * x + num6 - numArray4[1];
            double num8 = num7 * Math.Cos(Math.PI * direction / 180.0);
            double num9 = num7 * Math.Sin(Math.PI * direction / 180.0);
            double num10 = load.Point.X + num8;
            double num11 = load.Point.Y + num9;
            load.Point.X = num10;
            load.Point.Y = num11;
          }
          ShapeLine shapeLine5 = new ShapeLine(shapeLine3.Start, shapeLine3.Loads[0].Point)
          {
            HorizontalForce = num4
          };
          shapeLine5.AxialForce = shapeLine5.GetAxialForce();
          shapeLineList.Add(shapeLine5);
          for (int index = 0; index < count - 1; ++index)
            shapeLineList.Add(new ShapeLine(shapeLine3.Loads[index].Point, shapeLine3.Loads[index + 1].Point)
            {
              HorizontalForce = num4,
              AxialForce = shapeLine5.GetAxialForce()
            });
          shapeLineList.Add(new ShapeLine(shapeLine3.Loads[count - 1].Point, shapeLine3.End)
          {
            HorizontalForce = num4,
            AxialForce = shapeLine5.GetAxialForce()
          });
        }
        else
          shapeLineList.Add(shapeLine3);
      }
      s.Infill = shapeLineList;
      s.IsSuspension = true;
    }
  }
}
