// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Simple.SimpleShapeState
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;

#nullable disable
namespace StructureEngine.Grammar.Simple
{
  public class SimpleShapeState : IShapeState, IEquatable<IShapeState>
  {
    public static SimpleShapeState Start = new SimpleShapeState(nameof (Start));
    public static SimpleShapeState SubdivideDeck = new SimpleShapeState(nameof (SubdivideDeck));
    public static SimpleShapeState AddFunicular = new SimpleShapeState(nameof (AddFunicular));
    public static SimpleShapeState End = new SimpleShapeState(nameof (End));

    private SimpleShapeState(string name) => this.Name = name;

    public string Name { get; private set; }

    public bool IsEnd() => this == SimpleShapeState.End;

    public bool Equals(IShapeState other)
    {
      return other is SimpleShapeState simpleShapeState && this.Name == simpleShapeState.Name;
    }
  }
}
