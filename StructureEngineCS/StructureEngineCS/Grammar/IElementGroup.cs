// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.IElementGroup
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public interface IElementGroup
  {
    IEnumerable<ShapePoint> Points { get; }

    IEnumerable<ShapeLine> Lines { get; }

    IEnumerable<ShapeArea> Areas { get; }

    ShapePoint ZeroShapePoint { get; }

    double[] Dimensions { get; }

    void Rotate(double angle, ShapePoint center);

    void Translate(double x, double y);

    void Scale(double scalex, double scaley);
  }
}
