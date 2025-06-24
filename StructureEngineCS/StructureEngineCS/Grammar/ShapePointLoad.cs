// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.ShapePointLoad
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Grammar
{
  public class ShapePointLoad
  {
    public ShapePointLoad(ShapePoint p, double magnitude, double direction)
    {
      this.Point = p;
      this.Magnitude = magnitude;
      this.Direction = direction;
    }

    public ShapePoint Point { get; set; }

    public double Magnitude { get; set; }

    public double Direction { get; set; }

    public ShapePointLoad Clone()
    {
      return new ShapePointLoad(this.Point.Clone(), this.Magnitude, this.Direction);
    }
  }
}
