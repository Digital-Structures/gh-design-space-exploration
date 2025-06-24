// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.IShapeState
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;

#nullable disable
namespace StructureEngine.Grammar
{
  public interface IShapeState : IEquatable<IShapeState>
  {
    string Name { get; }

    bool IsEnd();
  }
}
