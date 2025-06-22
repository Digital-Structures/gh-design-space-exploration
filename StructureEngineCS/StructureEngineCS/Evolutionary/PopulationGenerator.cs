// Decompiled with JetBrains decompiler
// Type: StructureEngine.Evolutionary.PopulationGenerator
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.Distributions;
using StructureEngine.Analysis;
using StructureEngine.MachineLearning;
using StructureEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Evolutionary
{
  public class PopulationGenerator
  {
    public PopulationGenerator(EvoParams p, IList<IDesign> newSeeds)
    {
      this.Seeds = newSeeds;
      this.Initialize(p);
    }

    public PopulationGenerator(EvoParams p) => this.Initialize(p);

    private void Initialize(EvoParams p)
    {
      this.Params = p;
      this.NormalGenerator = new Normal();
      this.NormalGenerator.RandomSource = Utilities.MyRandom;
      this.UniformGenerator = new ContinuousUniform();
      this.UniformGenerator.RandomSource = Utilities.MyRandom;
    }

    private EvoParams Params { get; set; }

    public IList<IDesign> Seeds { get; private set; }

    private Normal NormalGenerator { get; set; }

    private ContinuousUniform UniformGenerator { get; set; }

    public IList<IDesign> GenerateAndSelectTop(int number, IDesign myStructure)
    {
      return (IList<IDesign>) this.FindTopDesigns(this.NewGeneration(myStructure, (IContinuousDistribution) this.NormalGenerator, (IAnalysis) null), number);
    }

    public IList<IDesign> MultiGenerateAndSelectTop(
      int number,
      IDesign myStructure,
      int generations)
    {
      IList<IDesign> andSelectTop = this.GenerateAndSelectTop(number, myStructure);
      for (int index = 1; index < generations; ++index)
      {
        List<IDesign> newSeeds = new List<IDesign>();
        newSeeds.AddRange(andSelectTop.Take<IDesign>(2));
        andSelectTop = new PopulationGenerator(this.Params, (IList<IDesign>) newSeeds).GenerateAndSelectTop(number, myStructure);
      }
      return andSelectTop;
    }

    public IList<IDesign> GenerateAndSelectTop(int number, IDesign myStructure, Regression reg)
    {
      RegAnalysis a = new RegAnalysis(reg);
      List<IDesign> topDesigns = this.FindTopDesigns(this.NewGeneration(myStructure, (IContinuousDistribution) this.NormalGenerator, (IAnalysis) a), 2 * number);
      foreach (IDesign comp in topDesigns)
        this.EvaluateDesign(comp, (IAnalysis) a);
      topDesigns.Sort(new Comparison<IDesign>(PopulationGenerator.CompareStructures));
      topDesigns.RemoveRange(number, topDesigns.Count - number);
      return (IList<IDesign>) topDesigns;
    }

    private IList<IDesign> NewGeneration(IDesign myStructure, IContinuousDistribution dist, IAnalysis a)
    {
      IDesign typ = this.Seeds == null || this.Seeds.Count == 0 ? myStructure : this.Seeds[0];
      typ.Setup();
      List<IDesign> designList = new List<IDesign>();
      DateTime now = DateTime.Now;
      long ticks = now.Ticks;
      int num = 0;
      while (num < this.Params.GenSize)
      {
        IDesign design = this.GenerateDesign(typ, dist);
        if (!double.IsNaN(this.EvaluateDesign(design, a)))
        {
          designList.Add(design);
          ++num;
        }
      }
      now = DateTime.Now;
      double totalMilliseconds = new TimeSpan(now.Ticks - ticks).TotalMilliseconds;
      return (IList<IDesign>) designList;
    }

    private List<IDesign> FindTopDesigns(IList<IDesign> designs, int number)
    {
      List<IDesign> designList = (List<IDesign>) designs;
      designList.Sort(new Comparison<IDesign>(PopulationGenerator.CompareStructures));
      int index = 0;
      List<IDesign> list = new List<IDesign>();
      List<IDesign> source = new List<IDesign>();
      for (; list.Count < number && index < designList.Count; ++index)
      {
        IDesign s = designList[index];
        if (this.CheckDiversity(s, list))
          list.Add(s);
        else
          source.Add(s);
      }
      if (list.Count < number)
      {
        int count = number - list.Count;
        list.AddRange(source.Take<IDesign>(count));
        list.Sort(new Comparison<IDesign>(PopulationGenerator.CompareStructures));
      }
      return list;
    }

    private double EvaluateDesign(IDesign comp, IAnalysis a)
    {
      return a == null ? comp.Score : a.Analyze(comp);
    }

    private bool CheckDiversity(IDesign s, List<IDesign> list)
    {
      return s.GetDivBooster().IsDiverse(list, s, this.Params.MutRate);
    }

    private IDesign GenerateDesign(IDesign typ, IContinuousDistribution dist)
    {
      return typ.Crossover(this.Seeds).Mutate(dist, this.Params.MutRate);
    }

    public static int CompareStructures(IDesign s1, IDesign s2) => s1.Score.CompareTo(s2.Score);
  }
}
