// Decompiled with JetBrains decompiler
// Type: StormCloud.Evolutionary.DesignVar
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;

#nullable disable
namespace StormCloud.Evolutionary
{
  public class DesignVar
  {
    public DesignVar(double val, double min, double max)
    {
      this.Value = val;
      this.Min = min;
      this.Max = max;
    }

    public double Value { get; set; }

    public double Min { get; set; }

    public double Max { get; set; }

    public double Variation
    {
      get => (this.Max - this.Min) / 2.0;
      private set
      {
      }
    }

    public DesignVar VarClone() => new DesignVar(this.Value, this.Min, this.Max);

    public void Mutate(double globalrate, IContinuousDistribution dist)
    {
      dist = (IContinuousDistribution) new Normal(this.Value, globalrate * this.Variation);
      this.Value = dist.Sample();
    }

    public void Crossover(List<DesignVar> mylist)
    {
      double num1 = 0.0;
      double num2 = 0.0;
      foreach (DesignVar designVar in mylist)
      {
        double num3 = EvolutionaryUtilities.MyRandom.NextDouble();
        num1 += designVar.Value * num3;
        num2 += num3;
      }
      this.Value = num1 / num2;
    }

    public bool CheckConstraint() => this.Value < this.Max && this.Value > this.Min;

    public void FixConstraint()
    {
      if (this.CheckConstraint())
        return;
      if (this.Value > this.Max)
        this.Value = this.Max;
      else if (this.Value < this.Min)
        this.Value = this.Min;
    }

    public int GetBytes() => Math.Max(4, Convert.ToInt32(Math.Log(this.Variation * 2.0, 2.0)));

    public void Project(double d) => this.Value = this.Min + d * 2.0 * this.Variation;

    public void ShiftCenter(double c) => this.Value = this.Min + c * 2.0 * this.Variation;

    public double GetPoint() => (this.Value - this.Min) / (2.0 * this.Variation);

    public DesignVar Copy(DesignVar dvar) => new DesignVar(dvar.Value, dvar.Min, dvar.Max);
  }
}
