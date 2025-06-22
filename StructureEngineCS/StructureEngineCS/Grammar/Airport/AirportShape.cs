// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Airport.AirportShape
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Airport
{
  public class AirportShape : BaseShape
  {
    public AirportShape()
    {
      this.Roof = new List<ShapeLine>();
      this.Verticals = new List<ShapeLine>();
      ShapeLine shapeLine = new ShapeLine(new ShapePoint(0.0, 0.0), new ShapePoint(100.0, 0.0));
      this.Roof.Add(shapeLine);
      this.Start = shapeLine.Start;
      this.End = shapeLine.End;
      this.ShapeState = (IShapeState) AirportShapeState.Start;
      this.History = new ShapeHistory();
    }

    public override IGrammar GetGrammar() => (IGrammar) new AirportGrammar();

    public override IEnumerator<IElement> GetEnumerator()
    {
      foreach (IElement element in this.Roof)
        yield return element;
      foreach (IElement vertical in this.Verticals)
        yield return vertical;
    }

    public override Structure ConvertToStructure()
    {
      return new Structure(new List<StructureEngine.Model.Node>(), new List<Member>());
    }

    public override IShape Clone() => this.DesignClone() as IShape;

    public override IDesign DesignClone()
    {
      AirportShape copy = new AirportShape();
      copy.ShapeState = this.ShapeState;
      copy.Start = this.Start.Clone();
      copy.End = this.End.Clone();
      List<ShapeLine> shapeLineList1 = new List<ShapeLine>();
      foreach (ShapeLine shapeLine in this.Roof)
        shapeLineList1.Add(shapeLine);
      copy.Roof = shapeLineList1;
      List<ShapeLine> shapeLineList2 = new List<ShapeLine>();
      foreach (ShapeLine vertical in this.Verticals)
        shapeLineList2.Add(vertical);
      copy.Verticals = shapeLineList2;
      return (IDesign) this.BaseClone((IShape) copy);
    }

    public override bool LooksSame(IShape that)
    {
      if (!(that is AirportShape))
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

    public override IShapeState ShapeState { get; set; }

    public override double Score { get; set; }

    public List<ShapeLine> Roof { get; set; }

    public List<ShapeLine> Verticals { get; set; }

    public ShapePoint Start { get; set; }

    public ShapePoint End { get; set; }
  }
}
