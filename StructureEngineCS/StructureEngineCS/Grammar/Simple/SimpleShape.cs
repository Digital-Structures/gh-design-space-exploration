// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Simple.SimpleShape
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Simple
{
  public class SimpleShape : BaseShape
  {
    public SimpleShape()
    {
      this.Horizontal = new List<ShapeLine>();
      this.Verticals = new List<ShapeLine>();
      this.Funicular = new List<ShapeLine>();
      ShapeLine shapeLine = new ShapeLine(new ShapePoint(0.0, 0.0), new ShapePoint(100.0, 0.0));
      shapeLine.DistLoad = 1.0;
      this.Horizontal.Add(shapeLine);
      this.Start = shapeLine.Start;
      this.End = shapeLine.End;
      this.ShapeState = (IShapeState) SimpleShapeState.Start;
      this.History = new ShapeHistory();
    }

    public override IGrammar GetGrammar() => (IGrammar) new SimpleGrammar();

    public override bool LooksSame(IShape that)
    {
      if (!(that is SimpleShape))
        return false;
      List<IElement> elementList = new List<IElement>((IEnumerable<IElement>) this);
      foreach (IElement elem in (ElementGroup) that)
      {
        bool flag = false;
        for (int index = 0; index < elementList.Count; ++index)
        {
          if (elementList[index].IsSame(elem))
          {
            flag = true;
            elementList.RemoveAt(index);
            break;
          }
        }
        if (!flag)
          return false;
      }
      return true;
    }

    public override IEnumerator<IElement> GetEnumerator()
    {
      foreach (IElement element in this.Horizontal)
        yield return element;
      foreach (IElement vertical in this.Verticals)
        yield return vertical;
      foreach (IElement element in this.Funicular)
        yield return element;
    }

    public override Structure ConvertToStructure()
    {
      return new Structure(new List<StructureEngine.Model.Node>(), new List<Member>());
    }

    public override IShape Clone() => this.DesignClone() as IShape;

    public override IDesign DesignClone()
    {
      SimpleShape copy = new SimpleShape();
      copy.ShapeState = this.ShapeState;
      copy.Start = this.Start.Clone();
      copy.End = this.End.Clone();
      List<ShapeLine> shapeLineList1 = new List<ShapeLine>();
      foreach (ShapeLine shapeLine in this.Horizontal)
        shapeLineList1.Add(shapeLine.Clone());
      copy.Horizontal = shapeLineList1;
      List<ShapeLine> shapeLineList2 = new List<ShapeLine>();
      foreach (ShapeLine vertical in this.Verticals)
        shapeLineList2.Add(vertical.Clone());
      copy.Verticals = shapeLineList2;
      List<ShapeLine> shapeLineList3 = new List<ShapeLine>();
      foreach (ShapeLine shapeLine in this.Funicular)
        shapeLineList3.Add(shapeLine.Clone());
      copy.Funicular = shapeLineList3;
      return (IDesign) this.BaseClone((IShape) copy);
    }

    public override IShapeState ShapeState { get; set; }

    public List<ShapeLine> Funicular { get; set; }

    public List<ShapeLine> Verticals { get; set; }

    public List<ShapeLine> Horizontal { get; set; }

    public List<ShapePoint> HorPoints
    {
      get
      {
        List<ShapePoint> horPoints = new List<ShapePoint>();
        foreach (ShapeLine shapeLine in this.Horizontal)
        {
          if (!horPoints.Contains(shapeLine.Start))
            horPoints.Add(shapeLine.Start);
          if (!horPoints.Contains(shapeLine.End))
            horPoints.Add(shapeLine.End);
        }
        return horPoints;
      }
    }

    public ShapePoint Start { get; set; }

    public ShapePoint End { get; set; }

    public override double Score { get; set; }
  }
}
