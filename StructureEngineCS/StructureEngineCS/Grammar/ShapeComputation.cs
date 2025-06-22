// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.ShapeComputation
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public class ShapeComputation
  {
    private IGrammar Grammar;

    public ShapeComputation(IGrammar g)
    {
      this.Grammar = g;
      List<IShape> shapeList = new List<IShape>();
      IShape startShape = this.Grammar.GetStartShape();
      shapeList.Add(startShape);
      this.Current = startShape;
      this.History = shapeList;
    }

    public void ApplyRule(IShape s)
    {
      IShape shape = s.Clone();
      int num1 = this.History.Count - 1;
      int num2 = this.History.IndexOf(this.Current);
      if (num2 < num1)
      {
        for (int index = 0; index < num1 - num2; ++index)
          this.History.RemoveAt(num2 + 1);
      }
      this.History.Add(shape);
      this.Current = shape;
    }

    public void GoBack()
    {
      int index = this.History.IndexOf(this.Current);
      if (index > 0)
        this.Current = this.History[index - 1];
      else
        this.Current = this.History[index];
    }

    public void GoForward()
    {
      int num = this.History.Count - 1;
      int index = this.History.IndexOf(this.Current);
      if (index < num)
        this.Current = this.History[index + 1];
      else
        this.Current = this.History[index];
    }

    public ShapeComputation Clone()
    {
      ShapeComputation shapeComputation = new ShapeComputation(this.Grammar);
      shapeComputation.History.Clear();
      foreach (IShape shape1 in this.History)
      {
        IShape shape2 = shape1.Clone();
        shapeComputation.History.Add(shape2);
      }
      shapeComputation.Current = shapeComputation.History[this.History.IndexOf(this.Current)];
      return shapeComputation;
    }

    public List<IShape> History { get; set; }

    public IShape Current { get; set; }
  }
}
