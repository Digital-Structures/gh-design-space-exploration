// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.IntParameter
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.Distributions;
using System;

#nullable disable
namespace StructureEngine.Grammar
{
  public class IntParameter : IRuleParameter
  {
    public IntParameter(int mi, int ma, string name)
    {
      this.Min = mi;
      this.Max = ma;
      this.Name = name;
    }

    public string Name { get; private set; }

    public int Min { get; private set; }

    public int Max { get; private set; }

    public object GetRandomValue() => (object) Utilities.MyRandom.Next(this.Min, this.Max + 1);

    public object Mutate(IContinuousDistribution dist, double rate, object value)
    {
      double num = rate * (double) Math.Abs(this.Max - this.Min) / 2.0;
      //dist.Mean = Convert.ToDouble((int) value);
      //dist.StdDev = num;
            dist = new Normal((double)value, num);
            return (object) Convert.ToInt32(Math.Min(Math.Max(((IContinuousDistribution) dist).Sample(), (double) this.Min), (double) this.Max));
    }
  }
}
