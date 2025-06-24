// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Simple.Rules.Rule002
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Simple.Rules
{
  public class Rule002 : BaseRule<SimpleShape>
  {
    public Rule002()
    {
      this.Name = "Rule 02";
      this.Params.Add((IRuleParameter) new IntParameter(0, 30, "Deck to Subdivide"));
      this.Weight = 5.0;
      this.LHSLabel = (IShapeState) SimpleShapeState.SubdivideDeck;
      this.RHSLabel = (IShapeState) SimpleShapeState.SubdivideDeck;
      this.Description = "Subdivides deck and adds vertical elements at support points.";
    }

    public override void Apply(SimpleShape s, params object[] p)
    {
      int num = (int) p[0];
      int count = s.Horizontal.Count;
      if (count <= 0)
        return;
      int index1 = num % count;
      List<ShapeLine> subdivide = s.Horizontal[index1].GetSubdivide(2);
      s.Horizontal.Remove(s.Horizontal[index1]);
      s.Horizontal.InsertRange(index1, (IEnumerable<ShapeLine>) subdivide);
      for (int index2 = 0; index2 < subdivide.Count - 1; ++index2)
      {
        ShapeLine shapeLine = new ShapeLine(new ShapePoint(subdivide[index2].End.X, subdivide[index2].End.Y + 15.0), subdivide[index2].End);
        s.Verticals.Add(shapeLine);
      }
    }
  }
}
