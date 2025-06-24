// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Simple.Rules.Rule001
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Simple.Rules
{
  public class Rule001 : BaseRule<SimpleShape>
  {
    public Rule001()
    {
      this.Name = "Rule 01";
      this.Params.Add((IRuleParameter) new DoubleParameter(0.0, 10.0, "Support Offset"));
      this.Params.Add((IRuleParameter) new EnumParameter(new List<string>()
      {
        "Left Support",
        "Right Support"
      }, "Which Support to Raise"));
      this.LHSLabel = (IShapeState) SimpleShapeState.Start;
      this.RHSLabel = (IShapeState) SimpleShapeState.SubdivideDeck;
      this.Description = "Raises right or left support.";
    }

    public override void Apply(SimpleShape s, params object[] p)
    {
      double num = (double) p[0];
      switch ((string) p[1])
      {
        case "Left Support":
          s.Start.Y += num;
          break;
        case "Right Support":
          s.End.Y += num;
          break;
      }
      s.Horizontal[0].Start = s.Start;
      s.Horizontal[0].End = s.End;
    }
  }
}
