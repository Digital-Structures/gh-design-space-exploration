// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.ByXCoordComparer
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public class ByXCoordComparer : IComparer<ShapePointLoad>
  {
    public int Compare(ShapePointLoad l1, ShapePointLoad l2) => l1.Point.X.CompareTo(l2.Point.X);
  }
}
