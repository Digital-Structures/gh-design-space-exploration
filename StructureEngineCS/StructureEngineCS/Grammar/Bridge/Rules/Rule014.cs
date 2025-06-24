// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule014
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule014 : BaseRule<BridgeShape>
  {
    public Rule014()
    {
      this.Name = "Rule 14";
      this.Description = "Connects each cable to the closest tower top.";
      this.LHSLabel = (IShapeState) BridgeShapeState.ConnectSupports;
      this.RHSLabel = (IShapeState) BridgeShapeState.End;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
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
      List<ShapePoint> tops = s.Tops;
      foreach (ShapeLine shapeLine in s.Infill2)
      {
        ShapePoint p2_1 = tops[0];
        double num = shapeLine.End.GetDistance(p2_1);
        foreach (ShapePoint p2_2 in tops)
        {
          double distance = shapeLine.End.GetDistance(p2_2);
          if (distance < num)
          {
            num = distance;
            p2_1 = p2_2;
          }
        }
        shapeLine.End = p2_1;
      }
    }
  }
}
