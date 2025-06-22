// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.Rules.Rule006
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;

#nullable disable
namespace StructureEngine.Grammar.Bridge.Rules
{
  public class Rule006 : BaseRule<BridgeShape>
  {
    public Rule006()
    {
      this.Name = "Rule 06";
      this.Description = "Rotates the tower 180 degrees.";
      this.LHSLabel = (IShapeState) BridgeShapeState.ModifyTower;
      this.RHSLabel = (IShapeState) BridgeShapeState.MakeDeck;
    }

    public override void Apply(BridgeShape s, params object[] p)
    {
      double angle = 180.0;
      ShapePoint center = new ShapePoint(s.ZeroShapePoint.X + s.Dimensions[0] / 2.0, s.ZeroShapePoint.Y + s.Dimensions[1] / 2.0);
      s.Rotate(angle, center);
      s.Tops.Clear();
      s.Tops.Add(s.Tower[0].Start);
      double y = s.ZeroShapePoint.Y;
      int num1 = s.Tower.Count - 1;
      for (int index = 1; index < num1 + 1; ++index)
      {
        ShapeLine shapeLine = s.Tower[index];
        double num2 = (shapeLine.End.Y - y) / Math.Sin((shapeLine.Rotation + 180.0) * Math.PI / 180.0);
        double scale = (shapeLine.Length + num2) / shapeLine.Length;
        shapeLine.Scale(scale);
      }
    }
  }
}
