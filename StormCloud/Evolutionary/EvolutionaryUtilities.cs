// Decompiled with JetBrains decompiler
// Type: StormCloud.Evolutionary.EvolutionaryUtilities
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using MathNet.Numerics.Distributions;
using StormCloud.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StormCloud.Evolutionary
{
  public static class EvolutionaryUtilities
  {
    public static Random MyRandom = new Random(1);
    public static Normal NormalGenerator = new Normal()
    {
      RandomSource = EvolutionaryUtilities.MyRandom
    };
    public static ContinuousUniform UniformGenerator = new ContinuousUniform()
    {
      RandomSource = EvolutionaryUtilities.MyRandom
    };

    public static List<Design> NewGeneration(
      Design myFirstDesign,
      List<Design> seeds,
      IContinuousDistribution dist,
      int popsize,
      double rate)
    {
      List<Design> designList = new List<Design>();
      int num = 0;
      while (num < popsize)
      {
        Design design = EvolutionaryUtilities.GenerateDesign(myFirstDesign, seeds, dist, rate);
        ++num;
        designList.Add(design.DesignClone());
      }
      return designList;
    }

    public static List<DesignVM> FindTopDesignsVM(
      List<DesignVM> DesignVMs,
      int number,
      double rate)
    {
      List<DesignVM> designVmList = DesignVMs;
      designVmList.Sort(new Comparison<DesignVM>(EvolutionaryUtilities.CompareDesigns));
      int index = 0;
      List<DesignVM> list = new List<DesignVM>();
      List<DesignVM> source = new List<DesignVM>();
      for (; list.Count < number && index < designVmList.Count; ++index)
      {
        DesignVM s = designVmList[index];
        if (EvolutionaryUtilities.CheckDiversity(s, list, rate))
          list.Add(s);
        else
          source.Add(s);
      }
      if (list.Count < number)
      {
        int count = number - list.Count;
        list.AddRange(source.Take<DesignVM>(count));
        list.Sort(new Comparison<DesignVM>(EvolutionaryUtilities.CompareDesigns));
      }
      return list;
    }

    public static Design GenerateDesign(
      Design myFirstDesign,
      List<Design> seeds,
      IContinuousDistribution dist,
      double rate)
    {
      return EvolutionaryUtilities.Mutate(EvolutionaryUtilities.Crossover(myFirstDesign.DesignClone(), seeds), dist, rate);
    }

    public static Design Mutate(Design d, IContinuousDistribution dist, double rate)
    {
      Design design = d.DesignClone();
      List<DesignVar> designVarList = new List<DesignVar>();
      foreach (DesignVar designVariable in design.DesignVariables)
      {
        DesignVar designVar = designVariable.VarClone();
        designVar.Mutate(rate, dist);
        designVar.FixConstraint();
        designVarList.Add(designVar);
      }
      design.DesignVariables = designVarList;
      return design;
    }

    public static Design Crossover(Design d, List<Design> seeds)
    {
      Design design = d.DesignClone();
      if (seeds == null || seeds.Count == 0)
        return design;
      for (int index = 0; index < d.DesignVariables.Count; ++index)
      {
        DesignVar designVariable = design.DesignVariables[index];
        List<DesignVar> mylist = new List<DesignVar>();
        foreach (Design seed in seeds)
          mylist.Add(seed.DesignVariables[index]);
        designVariable.Crossover(mylist);
      }
      return design;
    }

    public static bool IsDiverse(List<DesignVM> existing, DesignVM candidate, double rate)
    {
      double diffSize = EvolutionaryUtilities.GetDiffSize(candidate, rate);
      foreach (DesignVM d1 in existing)
      {
        if (EvolutionaryUtilities.GetDistance(d1, candidate) < diffSize)
          return false;
      }
      return true;
    }

    private static bool CheckDiversity(DesignVM s, List<DesignVM> list, double rate)
    {
      return EvolutionaryUtilities.IsDiverse(list, s, rate);
    }

    private static double GetDistance(DesignVM d1, DesignVM d2)
    {
      double d = 0.0;
      for (int index = 0; index < d1.Design.DesignVariables.Count; ++index)
      {
        double num = Math.Pow(d1.Design.DesignVariables[index].Value - d2.Design.DesignVariables[index].Value, 2.0);
        d += num;
      }
      return Math.Sqrt(d);
    }

    public static double SizeDesignSpace(DesignVM d)
    {
      double d1 = 0.0;
      foreach (DesignVar designVariable in d.Design.DesignVariables)
        d1 += Math.Pow(designVariable.Max - designVariable.Min, 2.0);
      return Math.Sqrt(d1);
    }

    private static double GetDiffSize(DesignVM d, double rate)
    {
      return 0.3 * (EvolutionaryUtilities.SizeDesignSpace(d) * rate);
    }

    public static int CompareDesigns(DesignVM d1, DesignVM d2) => d1.Score.CompareTo(d2.Score);
  }
}
