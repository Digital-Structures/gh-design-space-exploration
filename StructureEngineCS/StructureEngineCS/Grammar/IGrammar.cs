// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.IGrammar
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Analysis;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public interface IGrammar
  {
    List<IRule> AllRules { get; }

    List<IRule> UniqueRules { get; set; }

    IShape GetStartShape();

    IList<IRule> GetPossibleRules(IShape s);

    IAnalysis GetAnalysis();

    List<IShape> AllCrossover(IShape Cross1, IShape Cross2);
  }
}
