// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule003
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule003 : BaseRule<BridgeShape>
  {
    public Rule003()
    {
      this.Name = "Rule 03";
      this.Description = "Branches a tower element.";
      this.Params.Add((IRuleParameter) new DoubleParameter(0.2, 0.8, "Location of Branch from Bottom"));
      this.Params.Add((IRuleParameter) new IntParameter(2, 4, "Number of Branches"));
      this.Params.Add((IRuleParameter) new DoubleParameter(10.0, 30.0, "Angle Between Branches"));
      this.LHSLabel = (IShapeState) BridgeShapeState.AddBranches;
      this.RHSLabel = (IShapeState) BridgeShapeState.ModifyTower;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      double scale = (double) p[0];
      int num1 = (int) p[1];
      double num2 = (double) p[2];
      double num3 = num2 * (double) (num1 - 1);
      ShapeLine shapeLine1 = s.Tower[0];
      double rotation = shapeLine1.Rotation;
      double length = shapeLine1.Length;
      shapeLine1.Scale(scale);
      double angle = rotation - num3 / 2.0;
      for (int index = 0; index < num1; ++index)
      {
        ShapeLine shapeLine2 = new ShapeLine(shapeLine1.End, angle, length * (1.0 - scale));
        s.Tower.Add(shapeLine2);
        angle += num2;
        s.Tops.Add(shapeLine2.End);
      }
      s.Tops.Remove(s.Tower[0].End);
      s.Tops.Sort((Comparison<ShapePoint>) ((p1, p2) => p1.X.CompareTo(p2.X)));
    }
  }
}
