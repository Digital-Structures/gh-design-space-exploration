// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.IShape
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;

#nullable disable
namespace StructureEngine.Grammar
{
  public interface IShape : IElementGroup, IDesign
  {
    Structure ConvertToStructure();

    IShape Clone();

    bool LooksSame(IShape that);

    ShapeHistory History { get; set; }

    IShape Parent1 { get; set; }

    IShape Parent2 { get; set; }

    int SplicePoint1 { get; set; }

    int SplicePoint2 { get; set; }

    new double Score { get; set; }

    IShapeState ShapeState { get; set; }

    IShape GoBack();

    IGrammar GetGrammar();
  }
}
