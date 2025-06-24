// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Airport.Rules.Rule002
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.GraphicStatics;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Airport.Rules
{
  public class Rule002 : BaseRule<AirportShape>
  {
    public Rule002()
    {
      this.Name = "Rule 02";
      this.Params.Add((IRuleParameter) new DoubleParameter(50.0, 150.0, "Force per Unit Weight per Length"));
      this.Params.Add((IRuleParameter) new EnumParameter(new List<string>()
      {
        "Tension",
        "Compression"
      }, "Structural Behavior"));
      this.Params.Add((IRuleParameter) new IntParameter(4, 20, "Number of Loading Points"));
      this.LHSLabel = (IShapeState) AirportShapeState.ModifySpan;
      this.RHSLabel = (IShapeState) AirportShapeState.AddVerticals;
    }

    public override void Apply(AirportShape s, params object[] p)
    {
      double h = (double) p[0];
      string str = (string) p[1];
      int inc = (int) p[2];
      if (str == "Compression")
        h = -h;
      ProblemSetup problemSetup = new ProblemSetup(s.Roof[0].Start, s.Roof[0].End, inc, 1.0);
      problemSetup.DrawFcP(h);
      problemSetup.DrawFmD(h);
      double rotation1 = s.Roof[0].Rotation;
      ShapeLine shapeLine1 = new ShapeLine(s.Roof[0].Start, 0.0, s.Roof[0].Length);
      ShapeLine shapeLine2 = new ShapeLine(s.Roof[0].Start, s.Roof[0].Rotation, s.Roof[0].Length);
      double rotation2 = shapeLine1.Rotation;
      double rotation3 = shapeLine2.Rotation;
      s.Roof.Clear();
      s.Roof.AddRange((IEnumerable<ShapeLine>) problemSetup.FormD.Segments);
      s.Start = s.Roof[0].Start;
      s.End = s.Roof[s.Roof.Count - 1].End;
    }
  }
}
