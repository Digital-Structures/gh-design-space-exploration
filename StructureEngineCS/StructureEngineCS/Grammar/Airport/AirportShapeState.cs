// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.Airport.AirportShapeState
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;

#nullable disable
namespace StructureEngine.Grammar.Airport
{
  public class AirportShapeState : IShapeState, IEquatable<IShapeState>
  {
    public static AirportShapeState Start = new AirportShapeState(nameof (Start));
    public static AirportShapeState ModifySpan = new AirportShapeState(nameof (ModifySpan));
    public static AirportShapeState AddVerticals = new AirportShapeState(nameof (AddVerticals));
    public static AirportShapeState ModifyVerticals = new AirportShapeState(nameof (ModifyVerticals));
    public static AirportShapeState End = new AirportShapeState(nameof (End));

    public AirportShapeState(string name) => this.Name = name;

    public string Name { get; private set; }

    public bool IsEnd() => this == AirportShapeState.End;

    public bool Equals(IShapeState other)
    {
      return other is AirportShapeState airportShapeState && this.Name == airportShapeState.Name;
    }
  }
}
