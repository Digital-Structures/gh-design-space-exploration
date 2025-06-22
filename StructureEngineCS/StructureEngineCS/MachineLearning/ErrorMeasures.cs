// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.ErrorMeasures
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public class ErrorMeasures
  {
    public List<Observation> SortedObs;
    public Matrix<double> Weights;
    public Vector<double> Ranks;
    public int Top;
    private Regression MyRegression;

    public ErrorMeasures(List<Observation> lobs, Regression reg, int top, Matrix<double> w)
    {
      this.MyRegression = reg;
      this.Weights = w;
      this.Top = top;
      this.MeanAbsError = this.GetMAE(lobs);
      this.RootMSE = this.GetRMSE(lobs);
      this.MeanRankError = this.GetMRE(lobs);
      this.TopXRankError = this.GetTRankE(lobs);
      this.TopXRatioError = this.GetTRatioE(lobs);
      this.TopXFactorError = this.GetTFactorE(lobs);
      this.Ranks = (Vector<double>) CreateVector.Dense<double>(6);
      this.SortedObs = this.GetRankMap(lobs);
    }

    public ErrorMeasures Clone()
    {
      return new ErrorMeasures(this.SortedObs.GetRange(0, this.SortedObs.Count), this.MyRegression.Clone(), this.Top, this.Weights);
    }

    public double RootMSE { get; set; }

    public double RootMSENorm { get; set; }

    public double MeanAbsError { get; set; }

    public double MeanRankError { get; set; }

    public double TopXRankError { get; set; }

    public double TopXRatioError { get; set; }

    public double TopXFactorError { get; set; }

    public double[] GetErrors()
    {
      return new double[6]
      {
        this.RootMSE,
        this.MeanAbsError,
        this.MeanRankError,
        this.TopXRankError,
        this.TopXRatioError,
        this.TopXFactorError
      };
    }

    public double CompositeError => this.Weights.Multiply(this.Ranks)[0];

    public double CompositeAbsoluteError
    {
      get => this.Weights.Multiply((Vector<double>) CreateVector.Dense<double>(this.GetErrors()))[0];
    }

    private double GetMAE(List<Observation> lobs)
    {
      double num1 = 0.0;
      foreach (Observation lob in lobs)
      {
        double num2 = lob.Output - this.MyRegression.Predict(lob);
        num1 += Math.Abs(num2);
      }
      return num1 / (double) lobs.Count;
    }

    private double GetMSE(List<Observation> lobs)
    {
      double num1 = 0.0;
      foreach (Observation lob in lobs)
      {
        double num2 = Math.Pow(lob.Output - this.MyRegression.Predict(lob), 2.0);
        num1 += num2;
      }
      return num1 / (double) lobs.Count;
    }

    private double GetRMSE(List<Observation> lobs) => Math.Sqrt(this.GetMSE(lobs));

    private double GetMRE(List<Observation> lobs)
    {
      List<Observation> rankMap = this.GetRankMap(lobs);
      double num1 = 0.0;
      foreach (Observation observation in rankMap)
      {
        double num2 = (double) Math.Abs(observation.Rank - observation.PredictedRank) / ((double) lobs.Count / 2.0);
        num1 += num2;
      }
      return num1 / (double) lobs.Count;
    }

    private double GetTRankE(List<Observation> lobs)
    {
      List<Observation> rankMap = this.GetRankMap(lobs);
      double num1 = 0.0;
      for (int index = 0; index < this.Top; ++index)
      {
        Observation observation = rankMap[index];
        double num2 = (double) Math.Abs(observation.Rank - observation.PredictedRank) / ((double) this.Top / 2.0);
        num1 += num2;
      }
      return num1 / (double) this.Top;
    }

    private double GetTFactorE(List<Observation> lobs)
    {
      List<Observation> rankMap = this.GetRankMap(lobs);
      int val1 = 0;
      for (int index = 0; index < this.Top; ++index)
      {
        Observation observation = rankMap[index];
        val1 = Math.Max(val1, observation.PredictedRank);
      }
      return (Convert.ToDouble(val1) + 1.0 - (double) this.Top) / (double) lobs.Count;
    }

    private double GetTMaxRankE(List<Observation> lobs)
    {
      List<Observation> rankMap = this.GetRankMap(lobs);
      rankMap.Sort((Comparison<Observation>) ((o1, o2) => o1.Predicted.CompareTo(o2.Predicted)));
      int val1 = 0;
      for (int index = 0; index < this.Top; ++index)
      {
        Observation observation = rankMap[index];
        val1 = Math.Max(val1, observation.Rank);
      }
      return (Convert.ToDouble(val1) + 1.0 - (double) this.Top) / (double) lobs.Count;
    }

    private double GetTRatioE(List<Observation> lobs)
    {
      List<Observation> rankMap = this.GetRankMap(lobs);
      double num = 0.0;
      for (int index = 0; index < this.Top; ++index)
      {
        if (rankMap[index].PredictedRank < this.Top)
          ++num;
      }
      return Math.Abs((double) this.Top - num) / (double) lobs.Count;
    }

    private List<Observation> GetRankMap(List<Observation> lobs)
    {
      lobs.Sort((IComparer<Observation>) new ObsCompare());
      List<Observation> rankMap = new List<Observation>();
      foreach (Observation lob in lobs)
      {
        lob.Rank = lobs.IndexOf(lob);
        lob.Predicted = this.MyRegression.Predict(lob);
        rankMap.Add(lob);
      }
      rankMap.Sort((Comparison<Observation>) ((o1, o2) => o1.Predicted.CompareTo(o2.Predicted)));
      foreach (Observation observation in rankMap)
        observation.PredictedRank = rankMap.IndexOf(observation);
      rankMap.Sort((IComparer<Observation>) new ObsCompare());
      return rankMap;
    }
  }
}
