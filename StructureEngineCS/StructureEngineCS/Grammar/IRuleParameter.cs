// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.IRuleParameter
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.Distributions;

#nullable disable
namespace StructureEngine.Grammar
{
  public interface IRuleParameter
  {
    object GetRandomValue();

    string Name { get; }

    object Mutate(IContinuousDistribution dist, double rate, object value);
  }
}
