// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule008
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule008 : BaseRule<BridgeShape>
  {
    public Rule008()
    {
      this.Name = "Rule 08";
      this.Description = "Fills in space between tower branches with narrow angles.";
      this.Params.Add((IRuleParameter) new IntParameter(0, 20, "Which Branch to Infill From"));
      this.Params.Add((IRuleParameter) new DoubleParameter(25.0, 45.0, "Threshold Fill Angle"));
      this.LHSLabel = (IShapeState) BridgeShapeState.MakeInfill;
      this.RHSLabel = (IShapeState) BridgeShapeState.MakeInfill;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      int num1 = (int) p[0];
      double num2 = (double) p[1];
      int count = s.Tower.Count;
      if (count <= 1)
        return;
      int index1 = num1 % count;
      ShapeLine shapeLine = s.Tower[index1];
      double num3 = 360.0;
      int index2 = index1;
      for (int index3 = index1 + 1; index3 < count; ++index3)
      {
        double num4 = shapeLine.AngleBetween(s.Tower[index3]);
        if (num4 < num3 && num4 != 0.0)
        {
          num3 = num4;
          index2 = index3;
        }
      }
      if (num3 > num2)
        return;
      ShapeLine line2 = s.Tower[index2];
      ShapeArea shapeArea = new ShapeArea(shapeLine.ThreePoints(line2));
      s.Fill.Add(shapeArea);
    }
  }
}
