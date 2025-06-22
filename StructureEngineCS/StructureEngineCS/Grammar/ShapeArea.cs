// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.ShapeArea
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public class ShapeArea : IElement
  {
    public ShapeArea(List<ShapePoint> p) => this.Points = p;

    public List<ShapePoint> Points { get; set; }

    IElement IElement.Clone() => (IElement) this.Clone();

    public ShapeArea Clone()
    {
      List<ShapePoint> p = new List<ShapePoint>();
      foreach (ShapePoint point in this.Points)
      {
        ShapePoint shapePoint = point.Clone();
        p.Add(shapePoint);
      }
      return new ShapeArea(p);
    }

    public void RotateCenter(double angle, ShapePoint center)
    {
      List<ShapePoint> shapePointList = new List<ShapePoint>();
      foreach (ShapePoint point in this.Points)
      {
        ShapeLine shapeLine = new ShapeLine(center, point);
        shapeLine.Rotate(angle);
        ShapePoint end = shapeLine.End;
        shapePointList.Add(end);
      }
      this.Points = shapePointList;
    }

    public bool IsSame(IElement elem)
    {
      ShapeArea shapeArea = elem as ShapeArea;
      if (elem == null || shapeArea == null || this.Points.Count != shapeArea.Points.Count)
        return false;
      for (int index = 0; index < this.Points.Count; ++index)
      {
        if (!this.Points[index].IsSame((IElement) shapeArea.Points[index]))
          return false;
      }
      return true;
    }
  }
}
