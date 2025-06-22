// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.SurrogateModelBuilder
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.Distributions;
using StructureEngine.Analysis;
using StructureEngine.MachineLearning.Testing;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public class SurrogateModelBuilder
  {
    public SurrogateModelBuilder.ShouldCancel Cancel;
    public SurrogateModelBuilder.OnProgress OnProgressSimulate;
    public SurrogateModelBuilder.OnProgress OnProgressBuild;
    public SurrogateModelBuilder.OnProgress OnProgressRetest;
    private double PercentTop = 0.2;
    private IAnalysis AnalysisEngine;
    private int[] NNodes;
    private double[] RValues;

    public SurrogateModelBuilder(IAnalysis an)
    {
      this.AnalysisEngine = an;
      this.SetNuisanceParams();
    }

    public SurrogateModelBuilder() => this.SetNuisanceParams();

    private void SetNuisanceParams()
    {
      this.NNodes = new int[5]{ 4, 6, 8, 10, 12 };
      this.RValues = new double[5]
      {
        0.3,
        0.45,
        0.6,
        0.75,
        0.9
      };
    }

    public List<ValidationResult> BuildResults { get; set; }

    public RegressionReport BuildModel(
      RegCase r,
      List<Observation> trainingSet,
      List<Observation> validationSet,
      List<Observation> testSet)
    {
      long ticks = DateTime.Now.Ticks;
      RegressionReport regressionReport = new RegressionReport(r);
      Regression reg = this.Validate(trainingSet, validationSet, r);
      regressionReport.Model = reg;
      ErrorMeasures errorMeasures = new ErrorMeasures(testSet, reg, (int) Math.Round(this.PercentTop * (double) r.NumSamples), r.ErrorWeights);
      RegressionTest regressionTest = new RegressionTest()
      {
        TestSet = testSet,
        Error = errorMeasures
      };
      regressionReport.Milliseconds = new TimeSpan(DateTime.Now.Ticks - ticks).TotalMilliseconds;
      regressionReport.TestResults = regressionTest;
      return regressionReport;
    }

    public RegressionReport BuildModel(RegCase r)
    {
      ContinuousUniform dist = new ContinuousUniform();
      dist.RandomSource = Utilities.MyRandom;
      double basePct = 0.0;
      Sampling sampling = new Sampling(r.Problem, (IContinuousDistribution) dist);
      sampling.Cancel = (Sampling.IsCancel) (() => this.Cancel());
      sampling.Sample = (Sampling.OnSample) ((n, t) => this.OnProgressSimulate(basePct + (double) n / (double) t / 3.0));
      this.OnProgressSimulate(0.0);
      List<Observation> sample1 = sampling.GetSample(r.NumSamples, r.SamplingPlan, r.nUnderOne, r.nUnderOnePointFive);
      basePct = 1.0 / 3.0;
      List<Observation> sample2 = sampling.GetSample(r.NumSamples, r.SamplingPlan, r.nUnderOne, r.nUnderOnePointFive);
      basePct = 2.0 / 3.0;
      List<Observation> sample3 = sampling.GetSample(r.NumSamples, SampleType.RandomUniform, r.nUnderOne, r.nUnderOnePointFive);
      this.OnProgressSimulate(1.0);
      return this.Cancel() ? (RegressionReport) null : this.BuildModel(r, sample1, sample2, sample3);
    }

    public RegressionReport TestExistingModel(RegressionReport oldReport)
    {
      RegressionReport regressionReport = oldReport.Clone();
      regressionReport.TestResults = (RegressionTest) null;
      Regression model = regressionReport.Model;
      if (model == null)
        throw new Exception("No chosen regression model to test!!");
      RegCase properties = regressionReport.Properties;
      Sampling sampling = new Sampling(properties.Problem, (IContinuousDistribution) new ContinuousUniform()
      {
        RandomSource = Utilities.MyRandom
      });
      sampling.Cancel = (Sampling.IsCancel) (() => this.Cancel());
      sampling.Sample = (Sampling.OnSample) ((n, t) => this.OnProgressRetest((double) n / (double) t));
      this.OnProgressRetest(0.0);
      List<Observation> sample = sampling.GetSample(properties.NumSamples, SampleType.RandomUniform, properties.nUnderOne, properties.nUnderOnePointFive);
      this.OnProgressRetest(1.0);
      if (this.Cancel())
        return (RegressionReport) null;
      ErrorMeasures errorMeasures = new ErrorMeasures(sample, model, (int) Math.Round(this.PercentTop * (double) properties.NumSamples), properties.ErrorWeights);
      RegressionTest regressionTest = new RegressionTest()
      {
        TestSet = sample,
        Error = errorMeasures
      };
      regressionReport.TestResults = regressionTest;
      return regressionReport;
    }

    private Regression Validate(
      List<Observation> trainSet,
      List<Observation> valSetOrig,
      RegCase r)
    {
      Regression regression1 = (Regression) new RandomForestRegression(trainSet);
      Regression regression2 = (Regression) new EnsembleNeuralNetRegression(trainSet);
      List<ValidationResult> collection1 = new List<ValidationResult>();
      List<ValidationResult> collection2 = new List<ValidationResult>();
      List<ValidationResult> collection3 = new List<ValidationResult>();
      List<Observation> valSet = this.CloneObsSet(valSetOrig);
      Convert.ToInt32(r.BuildRF);
      int length1 = this.RValues.Length;
      Convert.ToInt32(r.BuildENN);
      int length2 = this.NNodes.Length;
      Convert.ToInt32(r.BuildKR);
      if (r.BuildRF)
      {
        foreach (double rvalue in this.RValues)
        {
          regression1.BuildModel(80, rvalue, 1, (int) Math.Round(this.PercentTop * (double) r.NumSamples));
          ErrorMeasures e = regression1.ValidateModel(valSet, (int) Math.Round(this.PercentTop * (double) r.NumSamples), r.ErrorWeights);
          collection1.Add(new ValidationResult(e, rvalue, regression1.Clone()));
          valSet.Clear();
          valSet = this.CloneObsSet(valSetOrig);
        }
      }
      if (r.BuildENN)
      {
        foreach (int nnode in this.NNodes)
        {
          regression2.BuildModel(nnode, 0.01, 1, (int) Math.Round(this.PercentTop * (double) r.NumSamples));
          ErrorMeasures e = regression2.ValidateModel(valSet, (int) Math.Round(this.PercentTop * (double) r.NumSamples), r.ErrorWeights);
          collection2.Add(new ValidationResult(e, (double) nnode, regression2.Clone()));
          valSet.Clear();
          valSet = this.CloneObsSet(valSetOrig);
        }
      }
      List<ValidationResult> candidates = new List<ValidationResult>();
      candidates.AddRange((IEnumerable<ValidationResult>) collection2);
      candidates.AddRange((IEnumerable<ValidationResult>) collection1);
      candidates.AddRange((IEnumerable<ValidationResult>) collection3);
      List<ValidationResult> combinedError = this.GetCombinedError(candidates);
      this.BuildResults = combinedError;
      return combinedError[0].Model;
    }

    private List<Observation> CloneObsSet(List<Observation> obsSet)
    {
      List<Observation> observationList = new List<Observation>();
      foreach (Observation obs in obsSet)
        observationList.Add(obs.ObservationClone());
      return observationList;
    }

    private List<ValidationResult> GetCombinedError(List<ValidationResult> candidates)
    {
      candidates.Sort((Comparison<ValidationResult>) ((first, next) => first.Error.RootMSE.CompareTo(next.Error.RootMSE)));
      candidates.ForEach((Action<ValidationResult>) (c => c.Error.Ranks[0] = (double) candidates.IndexOf(c)));
      candidates.Sort((Comparison<ValidationResult>) ((first, next) => first.Error.MeanAbsError.CompareTo(next.Error.MeanAbsError)));
      candidates.ForEach((Action<ValidationResult>) (c => c.Error.Ranks[1] = (double) candidates.IndexOf(c)));
      candidates.Sort((Comparison<ValidationResult>) ((first, next) => first.Error.MeanRankError.CompareTo(next.Error.MeanRankError)));
      candidates.ForEach((Action<ValidationResult>) (c => c.Error.Ranks[2] = (double) candidates.IndexOf(c)));
      candidates.Sort((Comparison<ValidationResult>) ((first, next) => first.Error.TopXRankError.CompareTo(next.Error.TopXRankError)));
      candidates.ForEach((Action<ValidationResult>) (c => c.Error.Ranks[3] = (double) candidates.IndexOf(c)));
      candidates.Sort((Comparison<ValidationResult>) ((first, next) => first.Error.TopXRatioError.CompareTo(next.Error.TopXRatioError)));
      candidates.ForEach((Action<ValidationResult>) (c => c.Error.Ranks[4] = (double) candidates.IndexOf(c)));
      candidates.Sort((Comparison<ValidationResult>) ((first, next) => first.Error.TopXFactorError.CompareTo(next.Error.TopXFactorError)));
      candidates.ForEach((Action<ValidationResult>) (c => c.Error.Ranks[5] = (double) candidates.IndexOf(c)));
      candidates.Sort((Comparison<ValidationResult>) ((first, next) => first.Error.CompositeError.CompareTo(next.Error.CompositeError)));
      return candidates;
    }

    public delegate bool ShouldCancel();

    public delegate void OnProgress(double pct);
  }
}
