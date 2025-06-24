// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Bridge.BridgeShapeState
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;

#nullable disable
namespace StructureEngine.Grammar.Bridge
{
  public class BridgeShapeState : IShapeState, IEquatable<IShapeState>
  {
    public static BridgeShapeState MakeTower = new BridgeShapeState(nameof (MakeTower));
    public static BridgeShapeState AddBranches = new BridgeShapeState(nameof (AddBranches));
    public static BridgeShapeState ModifyTower = new BridgeShapeState(nameof (ModifyTower));
    public static BridgeShapeState MakeDeck = new BridgeShapeState(nameof (MakeDeck));
    public static BridgeShapeState MakeInfill = new BridgeShapeState(nameof (MakeInfill));
    public static BridgeShapeState Subdivide = new BridgeShapeState(nameof (Subdivide));
    public static BridgeShapeState AddSupports = new BridgeShapeState(nameof (AddSupports));
    public static BridgeShapeState ConnectSupports = new BridgeShapeState(nameof (ConnectSupports));
    public static BridgeShapeState ModifySupports = new BridgeShapeState(nameof (ModifySupports));
    public static BridgeShapeState CableShape = new BridgeShapeState(nameof (CableShape));
    public static BridgeShapeState MultipleTowers = new BridgeShapeState(nameof (MultipleTowers));
    public static BridgeShapeState End = new BridgeShapeState(nameof (End));

    public BridgeShapeState(string name) => this.Name = name;

    public string Name { get; private set; }

    public bool IsEnd() => this == BridgeShapeState.End;

    public bool Equals(IShapeState other)
    {
      return other is BridgeShapeState bridgeShapeState && this.Name == bridgeShapeState.Name;
    }
  }
}
