// Decompiled with JetBrains decompiler
// Type: StormCloud.Evolutionary.Design
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;

#nullable disable
namespace StormCloud.Evolutionary
{
  public class Design
  {
    public Design()
    {
    }

    public Design(List<DesignVar> dvar) => this.DesignVariables = dvar;

    public List<DesignVar> DesignVariables { get; set; }

    public void CheckConstraints()
    {
      foreach (DesignVar designVariable in this.DesignVariables)
      {
        if (!designVariable.CheckConstraint())
          designVariable.FixConstraint();
      }
    }

    public Design DesignClone()
    {
      List<DesignVar> designVarList = new List<DesignVar>();
      foreach (DesignVar designVariable in this.DesignVariables)
      {
        DesignVar designVar = new DesignVar(designVariable.Value, designVariable.Min, designVariable.Max);
        designVarList.Add(designVar);
        Console.WriteLine("SAME OBJECT");
        Console.Write(designVar == designVariable);
        Console.WriteLine(100000);
      }
      return new Design(this.DesignVariables);
    }

    public Design Crossover(List<Design> seeds)
    {
      Design design = this.DesignClone();
      if (seeds == null || seeds.Count == 0)
        return design;
      for (int index = 0; index < this.DesignVariables.Count; ++index)
      {
        DesignVar designVariable = design.DesignVariables[index];
        List<DesignVar> mylist = new List<DesignVar>();
        foreach (Design seed in seeds)
          mylist.Add(seed.DesignVariables[index]);
        designVariable.Crossover(mylist);
      }
      return design;
    }

    public Design Mutate(IContinuousDistribution dist, double rate)
    {
      Design design = this.DesignClone();
      foreach (DesignVar designVariable in design.DesignVariables)
        designVariable.Mutate(rate, dist);
      design.CheckConstraints();
      return design;
    }
  }
}
