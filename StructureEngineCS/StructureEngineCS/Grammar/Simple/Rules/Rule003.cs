// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Simple.Rules.Rule003
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Simple.Rules
{
  public class Rule003 : BaseRule<SimpleShape>
  {
    public Rule003()
    {
      this.Name = "Rule 03";
      this.Params.Add((IRuleParameter) new DoubleParameter(40.0, 120.0, "Force per Unit Weight per Length"));
      this.Params.Add((IRuleParameter) new EnumParameter(new List<string>()
      {
        "Tension",
        "Compression"
      }, "Structural Behavior"));
      this.LHSLabel = (IShapeState) SimpleShapeState.AddFunicular;
      this.RHSLabel = (IShapeState) SimpleShapeState.End;
      this.Description = "Adds a funicular element to support deck.";
    }

    public override void Apply(SimpleShape s, params object[] p)
    {
      double force = (double) p[0];
      if ((string) p[1] == "Compression")
        force = -force;
      List<ShapeLine> shapeLineList = new List<ShapeLine>();
      s.Verticals.Sort((IComparer<ShapeLine>) new ByXLineComparer());
      SimpleAnalysis simpleAnalysis = new SimpleAnalysis();
      simpleAnalysis.SolveHorMoment(s);
      simpleAnalysis.SolveFunicular(s, force);
      for (int index = 0; index < s.Verticals.Count; ++index)
        shapeLineList.Add(new ShapeLine(s.Verticals[index].End, s.Funicular[index].End)
        {
          AxialForce = force <= 0.0 ? s.Verticals[index].AxialForce : -s.Verticals[index].AxialForce
        });
      s.Verticals = shapeLineList;
      s.Score = simpleAnalysis.Analyze((IDesign) s);
    }
  }
}
