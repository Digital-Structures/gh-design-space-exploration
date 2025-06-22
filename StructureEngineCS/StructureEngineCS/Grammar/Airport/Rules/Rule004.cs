// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Airport.Rules.Rule004
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Grammar.Airport.Rules
{
  public class Rule004 : BaseRule<AirportShape>
  {
    public Rule004()
    {
      this.Name = "Rule 04";
      this.Params.Add((IRuleParameter) new DoubleParameter(10.0, 40.0, "Height Above Ground"));
      this.LHSLabel = (IShapeState) AirportShapeState.AddVerticals;
      this.LHSLabel = (IShapeState) AirportShapeState.ModifyVerticals;
    }

    public override void Apply(AirportShape s, params object[] p)
    {
      double num = (double) p[0];
      foreach (ShapePoint point in s.Points)
        point.Y += num;
      s.Verticals.Add(new ShapeLine(new ShapePoint(0.0, 0.0), s.Start));
      s.Verticals.Add(new ShapeLine(new ShapePoint(s.End.X, 0.0), s.End));
    }
  }
}
