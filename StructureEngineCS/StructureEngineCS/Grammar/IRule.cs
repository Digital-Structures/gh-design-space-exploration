// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.IRule
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public interface IRule
  {
    IGrammar MyGrammar { get; set; }

    string Name { get; }

    double Weight { get; }

    string Description { get; }

    List<IRuleParameter> Params { get; }

    bool CanApply(IShape s);

    IShapeState LHSLabel { get; }

    IShapeState RHSLabel { get; }

    void Apply(IShape s, params object[] p);
  }
}
