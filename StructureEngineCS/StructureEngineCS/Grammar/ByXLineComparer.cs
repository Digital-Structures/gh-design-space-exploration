// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.ByXLineComparer
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public class ByXLineComparer : IComparer<ShapeLine>
  {
    public int Compare(ShapeLine l1, ShapeLine l2) => l1.Start.X.CompareTo(l2.Start.X);
  }
}
