// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.DoubleParameter
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.Distributions;
using System;

#nullable disable
namespace StructureEngine.Grammar
{
  public class DoubleParameter : IRuleParameter
  {
    public DoubleParameter(double mi, double ma, string name)
    {
      this.Min = mi;
      this.Max = ma;
      this.Name = name;
    }

    public string Name { get; private set; }

    public double Min { get; private set; }

    public double Max { get; private set; }

    public object GetRandomValue()
    {
      return (object) (Utilities.MyRandom.NextDouble() * (this.Max - this.Min) + this.Min);
    }

    public object Mutate(IContinuousDistribution dist, double rate, object value)
    {
      double num = rate * Math.Abs(this.Max - this.Min) / 2.0;
      //dist.Mean = (double) value;
      //dist.StdDev = num;
       dist = new Normal((double)value, num);
            return (object) Math.Min(Math.Max(((IContinuousDistribution) dist).Sample(), this.Min), this.Max);
    }
  }
}
