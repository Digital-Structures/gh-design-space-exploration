// Decompiled with JetBrains decompiler
// Type: StructureEngine.GraphicStatics.PointLoad
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Grammar;

#nullable disable
namespace StructureEngine.GraphicStatics
{
  public class PointLoad
  {
    public ShapePoint Point;
    public double Load;
    public double Rotation;

    public PointLoad(ShapePoint p, double l)
    {
      this.Point = p;
      this.Load = l;
      this.Rotation = 270.0;
    }

    public PointLoad(ShapePoint p, double l, double r)
    {
      this.Point = p;
      this.Load = l;
      this.Rotation = r;
    }
  }
}
