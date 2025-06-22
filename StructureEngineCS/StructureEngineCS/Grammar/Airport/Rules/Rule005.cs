// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Airport.Rules.Rule005
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Airport.Rules
{
  public class Rule005 : BaseRule<AirportShape>
  {
    public Rule005()
    {
      this.Name = "Rule 05";
      this.Params.Add((IRuleParameter) new DoubleParameter(10.0, 40.0, "Angle of Tilt"));
      this.Params.Add((IRuleParameter) new EnumParameter(new List<string>()
      {
        "Left Support",
        "Right Support"
      }, "Which Support to Tilt"));
      this.LHSLabel = (IShapeState) AirportShapeState.ModifyVerticals;
      this.RHSLabel = (IShapeState) AirportShapeState.End;
    }

    public override void Apply(AirportShape s, params object[] p)
    {
      double angle = (double) p[0];
      ShapeLine shapeLine = !((string) p[1] == "Left Support") ? s.Verticals[1] : s.Verticals[0];
      shapeLine.Start.RotateAbout(angle, shapeLine.End);
    }
  }
}
