// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.EnumParameter
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.Distributions;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public class EnumParameter : IRuleParameter
  {
    public EnumParameter(List<string> enums, string name)
    {
      this.Name = name;
      this.Enums = enums;
    }

    public List<string> Enums { get; private set; }

    public object GetRandomValue()
    {
      return (object) this.Enums[Utilities.MyRandom.Next(this.Enums.Count)];
    }

    public string Name { get; private set; }

    public object Mutate(IContinuousDistribution dist, double rate, object value)
    {
      List<bool> boolList = new List<bool>();
      int num = ((IDistribution) dist).RandomSource.NextDouble() < rate ? 1 : 0;
      List<string> stringList = new List<string>();
      stringList.AddRange((IEnumerable<string>) this.Enums);
      stringList.Remove((string) value);
      return num == 0 ? (object) (string) value : (object) stringList[((IDistribution) dist).RandomSource.Next(stringList.Count)];
    }
  }
}
