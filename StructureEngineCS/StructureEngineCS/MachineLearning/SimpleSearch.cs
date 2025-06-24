// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.SimpleSearch
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public class SimpleSearch
  {
    private IFunction MyFunction;
    private bool IsMin;
    private List<double> LowerBounds;
    private List<double> UpperBounds;
    private int NumVars;
    private double MaxMutRate;
    private double TopRatio;

    public SimpleSearch(
      IFunction function,
      bool isMin,
      List<double> lowerBounds,
      List<double> upperBounds)
    {
      this.MyFunction = function;
      this.IsMin = isMin;
      this.LowerBounds = lowerBounds;
      this.UpperBounds = upperBounds;
      this.NumVars = this.LowerBounds.Count;
      this.MaxMutRate = 0.5;
      this.TopRatio = 0.2;
    }

    public Tuple<double, List<double>> FindBest(int nIterations, int genSize, double stopCriteria)
    {
      List<Tuple<double, List<double>>> pop = this.InstantiateSearch(genSize);
      double maxValue = double.MaxValue;
      Tuple<double, List<double>> best = pop[0];
      for (int index = 0; index < nIterations; ++index)
      {
        pop.Sort((IComparer<Tuple<double, List<double>>>) new PopComparer());
        if (!this.IsMin)
          pop.Reverse();
        best = pop[0];
        Math.Abs(best.Item1 - maxValue);
        pop = this.Mutate(pop, this.MaxMutRate - Math.Pow((double) index / (double) nIterations, 2.0) * this.MaxMutRate);
        maxValue = best.Item1;
      }
      return best;
    }

    private List<Tuple<double, List<double>>> InstantiateSearch(int n)
    {
      List<Tuple<double, List<double>>> tupleList = new List<Tuple<double, List<double>>>();
      while (tupleList.Count < n)
      {
        List<double> inputs = new List<double>();
        for (int index = 0; index < this.NumVars; ++index)
        {
          double num = Utilities.MyRandom.NextDouble() * (this.UpperBounds[index] - this.LowerBounds[index]) + this.LowerBounds[index];
          inputs.Add(num);
        }
        double output = this.MyFunction.GetOutput(inputs);
        if (!double.IsNaN(output))
          tupleList.Add(new Tuple<double, List<double>>(output, inputs));
      }
      return tupleList;
    }

    private List<Tuple<double, List<double>>> Mutate(
      List<Tuple<double, List<double>>> pop,
      double mutRate)
    {
      int count = (int) Math.Round((double) pop.Count * this.TopRatio);
      int num = pop.Count - count;
      List<Tuple<double, List<double>>> range = pop.GetRange(0, count);
      pop.RemoveRange(0, count);
      List<Tuple<double, List<double>>> tupleList = new List<Tuple<double, List<double>>>();
      tupleList.AddRange((IEnumerable<Tuple<double, List<double>>>) range);
      for (int index1 = 0; index1 < num; ++index1)
      {
        int index2 = Utilities.MyRandom.Next(0, count - 1);
        int index3 = Utilities.MyRandom.Next(0, count - 1);
        Tuple<double, List<double>> tuple = this.MutateElement(range[index2], range[index3], mutRate);
        tupleList.Add(tuple);
      }
      return tupleList;
    }

    private Tuple<double, List<double>> MutateElement(
      Tuple<double, List<double>> el1,
      Tuple<double, List<double>> el2,
      double mutRate)
    {
      List<double> inputs = new List<double>();
      bool flag = false;
      double d = 0.0;
      while (!flag)
      {
        for (int index = 0; index < this.NumVars; ++index)
        {
          double lowerBound = this.LowerBounds[index];
          double upperBound = this.UpperBounds[index];
          double num1 = upperBound - lowerBound;
          double num2 = el1.Item2[index];
          double num3 = el2.Item2[index];
          double num4 = Utilities.MyRandom.NextDouble();
          double num5 = 1.0 - num4;
          double num6 = num4;
          double num7 = num2 * num6 + num3 * num5 + (Utilities.MyRandom.NextDouble() - 0.5) * mutRate * num1;
          if (num7 < lowerBound)
            num7 = lowerBound;
          else if (num7 > upperBound)
            num7 = upperBound;
          inputs.Add(num7);
        }
        d = this.MyFunction.GetOutput(inputs);
        if (!double.IsNaN(d))
          flag = true;
      }
      return new Tuple<double, List<double>>(d, inputs);
    }
  }
}
