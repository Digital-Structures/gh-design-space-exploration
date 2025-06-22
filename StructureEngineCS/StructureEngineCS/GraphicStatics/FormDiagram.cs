// Decompiled with JetBrains decompiler
// Type: StructureEngine.GraphicStatics.FormDiagram
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Grammar;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.GraphicStatics
{
  public class FormDiagram
  {
    public ProblemSetup ProbSet;
    public List<ShapeLine> Segments;

    public FormDiagram(ProblemSetup ps, double h)
    {
      this.ProbSet = ps;
      this.Segments = new List<ShapeLine>();
      this.GenerateFD(h);
    }

    private void GenerateFD(double h)
    {
      if (this.ProbSet.ForceP == null)
        return;
      List<ShapeLine> shapeLineList = new List<ShapeLine>();
      int count = this.ProbSet.ForceP.Rays.Count;
      List<ShapePoint> shapePointList = new List<ShapePoint>();
      List<ShapeLine> collection = new List<ShapeLine>();
      shapePointList.Add(this.ProbSet.Start.Clone());
      double x1 = this.ProbSet.Start.X;
      for (int index = 0; index < count - 1; ++index)
      {
        double[] numArray = this.ProbSet.ForceP.Rays[index].SlopeIntercept();
        x1 += this.ProbSet.SegWidths[index];
        double num = x1 - shapePointList[index].X;
        double y = numArray[0] * num + shapePointList[index].Y;
        ShapePoint shapePoint = new ShapePoint(x1, y);
        shapePointList.Add(shapePoint);
      }
      double[] numArray1 = this.ProbSet.ForceP.Rays[count - 1].SlopeIntercept();
      double x2 = this.ProbSet.End.X;
      double num1 = x2 - shapePointList[shapePointList.Count - 1].X;
      double y1 = numArray1[0] * num1 + shapePointList[shapePointList.Count - 1].Y;
      ShapePoint shapePoint1 = new ShapePoint(x2, y1);
      shapePointList.Add(shapePoint1);
      for (int index = 0; index < shapePointList.Count - 1; ++index)
        collection.Add(new ShapeLine(shapePointList[index], shapePointList[index + 1])
        {
          AxialForce = h <= 0.0 ? -this.ProbSet.ForceP.Rays[index].Length : this.ProbSet.ForceP.Rays[index].Length
        });
      this.Segments.AddRange((IEnumerable<ShapeLine>) collection);
    }
  }
}
