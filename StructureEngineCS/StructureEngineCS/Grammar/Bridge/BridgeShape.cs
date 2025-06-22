// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.BridgeShape
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar.Bridge
{
  public class BridgeShape : BaseShape
  {
    public bool IsSuspension;

    public BridgeShape()
    {
      this.Infill = new List<ShapeLine>();
      this.Infill2 = new List<ShapeLine>();
      this.Tower = new List<ShapeLine>();
      this.Deck = new List<ShapeLine>();
      this.Fill = new List<ShapeArea>();
      this.Tops = new List<ShapePoint>();
      this.DeckDivs = new List<ShapePoint>();
      this.DeckPoints = new List<ShapePoint>();
      ShapeLine shapeLine = new ShapeLine(new ShapePoint(0.0, 0.0), new ShapePoint(0.0, 20.0));
      this.Tower.Add(shapeLine);
      this.Tops.Add(shapeLine.End);
      this.ShapeState = (IShapeState) BridgeShapeState.MakeTower;
      this.History = new ShapeHistory();
    }

    public override IGrammar GetGrammar() => (IGrammar) new BridgeGrammar();

    public override IEnumerator<IElement> GetEnumerator()
    {
      foreach (IElement element in this.Infill)
        yield return element;
      foreach (IElement element in this.Infill2)
        yield return element;
      foreach (IElement element in this.Tower)
        yield return element;
      foreach (IElement element in this.Deck)
        yield return element;
      foreach (IElement element in this.Fill)
        yield return element;
    }

    public override Structure ConvertToStructure() => new Structure();

    public override IShape Clone() => this.DesignClone() as IShape;

    public override IDesign DesignClone()
    {
      BridgeShape copy = new BridgeShape();
      copy.ShapeState = this.ShapeState;
      List<ShapePoint> shapePointList1 = new List<ShapePoint>();
      List<ShapeLine> shapeLineList1 = new List<ShapeLine>();
      foreach (ShapeLine shapeLine1 in this.Tower)
      {
        ShapeLine shapeLine2 = shapeLine1.Clone();
        shapeLineList1.Add(shapeLine2);
        if (this.Tops.Contains(shapeLine1.Start))
          shapePointList1.Add(shapeLine2.Start);
        if (this.Tops.Contains(shapeLine1.End))
          shapePointList1.Add(shapeLine2.End);
      }
      copy.Tower = shapeLineList1;
      copy.Tops = shapePointList1;
      List<ShapePoint> shapePointList2 = new List<ShapePoint>();
      foreach (ShapePoint deckDiv in this.DeckDivs)
      {
        ShapePoint shapePoint = deckDiv.Clone();
        shapePointList2.Add(shapePoint);
      }
      copy.DeckDivs = shapePointList2;
      List<ShapeLine> shapeLineList2 = new List<ShapeLine>();
      foreach (ShapeLine shapeLine3 in this.Deck)
      {
        ShapeLine shapeLine4 = shapeLine3.Clone();
        shapeLineList2.Add(shapeLine4);
      }
      copy.Deck = shapeLineList2;
      List<ShapeLine> shapeLineList3 = new List<ShapeLine>();
      foreach (ShapeLine shapeLine5 in this.Infill)
      {
        ShapeLine shapeLine6 = shapeLine5.Clone();
        shapeLineList3.Add(shapeLine6);
      }
      copy.Infill = shapeLineList3;
      List<ShapeLine> shapeLineList4 = new List<ShapeLine>();
      foreach (ShapeLine shapeLine7 in this.Infill2)
      {
        ShapeLine shapeLine8 = shapeLine7.Clone();
        shapeLineList4.Add(shapeLine8);
      }
      copy.Infill2 = shapeLineList4;
      List<ShapeArea> shapeAreaList = new List<ShapeArea>();
      foreach (ShapeArea shapeArea1 in this.Fill)
      {
        ShapeArea shapeArea2 = shapeArea1.Clone();
        shapeAreaList.Add(shapeArea2);
      }
      copy.Fill = shapeAreaList;
      return (IDesign) this.BaseClone((IShape) copy);
    }

    public override double Score { get; set; }

    public override bool LooksSame(IShape that)
    {
      if (!(that is BridgeShape))
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

    public void Integrate(BridgeShape s)
    {
      this.Tower.AddRange((IEnumerable<ShapeLine>) s.Tower);
      this.Infill.AddRange((IEnumerable<ShapeLine>) s.Infill);
      this.Infill2.AddRange((IEnumerable<ShapeLine>) s.Infill2);
      this.Deck.AddRange((IEnumerable<ShapeLine>) s.Deck);
      this.Fill.AddRange(s.Areas);
      this.Tops.AddRange((IEnumerable<ShapePoint>) s.Tops);
      this.DeckDivs.AddRange((IEnumerable<ShapePoint>) s.DeckDivs);
      ShapePoint shapePoint1 = this.Deck[0].Start;
      foreach (ShapeLine shapeLine in this.Deck)
      {
        if (shapeLine.Start.X < shapePoint1.X)
          shapePoint1 = shapeLine.Start;
        if (shapeLine.End.X < shapePoint1.X)
          shapePoint1 = shapeLine.End;
      }
      ShapePoint shapePoint2 = this.Deck[0].Start;
      foreach (ShapeLine shapeLine in this.Deck)
      {
        if (shapeLine.Start.X > shapePoint2.X)
          shapePoint2 = shapeLine.Start;
        if (shapeLine.End.X > shapePoint2.X)
          shapePoint2 = shapeLine.End;
      }
      List<ShapeLine> shapeLineList = new List<ShapeLine>();
      foreach (ShapeLine shapeLine1 in this.Infill)
      {
        if (shapeLine1.Start.X > shapeLine1.End.X)
        {
          ShapeLine shapeLine2 = new ShapeLine(shapeLine1.End, shapeLine1.Start);
          shapeLineList.Add(shapeLine2);
        }
        else
          shapeLineList.Add(shapeLine1);
      }
      this.Infill = shapeLineList;
    }

    public override IShapeState ShapeState { get; set; }

    public List<ShapePoint> Tops { get; set; }

    public List<ShapePoint> DeckDivs { get; set; }

    public List<ShapePoint> DeckPoints { get; set; }

    public List<ShapeLine> Deck { get; set; }

    public List<ShapeLine> Tower { get; set; }

    public List<ShapeLine> Infill { get; set; }

    public List<ShapeLine> Infill2 { get; set; }

    public List<ShapeArea> Fill { get; set; }
  }
}
