using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using StructureEngine.Analysis;
//using StructureEngine.DesignModes;
using StructureEngine.MachineLearning.Testing;

namespace StructureEngine.MachineLearning
{
    public class RegressionTest
    {
        public List<Observation> TestSet { get; set; }
        public ErrorMeasures Error { get; set; }

        public RegressionTest Clone()
        {
            var set = TestSet.GetRange(0, TestSet.Count);
            return new RegressionTest() { TestSet = set, Error = this.Error.Clone() };
        }
    }

    public class RegressionReport
    {

        public RegressionReport(RegCase c)
        {
            this.Properties = c;
        }

        public Regression Model { get; set; }
        public RegressionTest TestResults { get; set; }
        public double Milliseconds { get; set; }
        public RegCase Properties { get; set; }
        public RegressionReport Clone()
        {
            var clone = new RegressionReport(this.Properties.Clone());
            if (this.Model != null)
            {
                clone.Model = this.Model.Clone();
            }
            if (this.TestResults != null)
            {
                clone.TestResults = this.TestResults.Clone();
            }
            clone.Milliseconds = this.Milliseconds;
            return clone;
        }

        //public RegressionReport(SampleType p, bool r, bool e, bool k, Matrix<double> w, int n1, int n15, string prob)
        //{
        //    this.SamplePlan = p;
        //    this.BuildRF = r;
        //    this.BuildENN = e;
        //    this.BuildKR = k;
        //    this.Weights = w;
        //    this.nUnderOne = n1;
        //    this.nUnderOnePointFive = n15;
        //    this.Problem = prob;

        //    ListWeights = new List<double>();
        //    for (int i = 0; i < Weights.ColumnCount; i++)
        //    {
        //        ListWeights.Add(Weights[0, i]);
        //    }
        //}
        //public bool BuildRF { get; set; }
        //public bool BuildENN { get; set; }
        //public bool BuildKR { get; set; }
        //public int nUnderOne { get; set; }
        //public int nUnderOnePointFive { get; set; }
        //public SampleType SamplePlan { get; set; }
        //public Matrix<double> Weights { get; set; }
        //public List<double> ListWeights { get; set; }
        //public string Problem { get; set; }

    }

    public class SurrogateModelBuilder
    {
        public SurrogateModelBuilder(IAnalysis an)
        {
            this.AnalysisEngine = an;
            this.NNodes = new int[] { 4, 6, 8, 10, 12 };
            this.RValues = new double[] { 0.30, 0.45, 0.60, 0.75, 0.90 };

            //this.MyView = r;
            //this.NumEvals = numEvals;
            //this.nUnderOnePointFive = 1;
            //this.nUnderOne = 2;
            //this.MyRand = r;
            // hook up regression builder + tester engines
            //this.HookupEngines();
        }

        // Cancel / progress callbacks
        public delegate bool ShouldCancel();
        public delegate void OnProgress(double pct);
        public ShouldCancel Cancel;
        public OnProgress OnProgressSimulate;
        public OnProgress OnProgressBuild;
        public OnProgress OnProgressRetest;

        private double PercentTop = 0.2;
        private IAnalysis AnalysisEngine;
        private int[] NNodes;
        private double[] RValues; 


        public RegressionReport BuildModel(RegCase r)
        {
            // start "stopwatch"
            long before = DateTime.Now.Ticks;

            // set up regression report
            var BuildReport = new RegressionReport(r);

            // set up sampler
            var dist = new ContinuousUniform();
            dist.RandomSource = Utilities.MyRandom;

            double basePct = 0;
            var sampler = new Sampling(r.Problem, dist);
            sampler.Cancel = () => { return Cancel(); };
            sampler.Sample = (n, t) =>
            {
                double pct = basePct + ((double)n / t) / 3.0;
                this.OnProgressSimulate(pct);
            };

            // simulate three data sets for training, validation, and testing
            this.OnProgressSimulate(0);
            List<Observation> trainSet = sampler.GetSample(r.NumSamples, r.SamplingPlan, r.nUnderOne, r.nUnderOnePointFive);
            basePct = 1/3.0;
            List<Observation> valSet = sampler.GetSample(r.NumSamples, r.SamplingPlan, r.nUnderOne, r.nUnderOnePointFive);
            basePct = 2/3.0;
            List<Observation> testSet = sampler.GetSample(r.NumSamples, SampleType.RandomUniform, r.nUnderOne, r.nUnderOnePointFive);
            this.OnProgressSimulate(1);
            if (Cancel()) return null;

            // build candidate surrogate models to compare via validation
            Regression chosen = this.Validate(trainSet, valSet, r);
            BuildReport.Model = chosen;
            if (Cancel()) return null;

            // test chosen surrogate model with independent data set
            ErrorMeasures error = new ErrorMeasures(testSet, chosen, (int)Math.Round(PercentTop * r.NumSamples), r.ErrorWeights);
            RegressionTest results = new RegressionTest() { TestSet = testSet, Error = error };

            // stop "stopwatch"
            long after = DateTime.Now.Ticks;
            TimeSpan elapsedTime = new TimeSpan(after - before);
            BuildReport.Milliseconds = elapsedTime.TotalMilliseconds;

            BuildReport.TestResults = results;
            return BuildReport;
            //MyView.AddRegResults(MyBuildReport);
        }

        public RegressionReport TestExistingModel(RegressionReport oldReport)
        {
            var newReport = oldReport.Clone();
            newReport.TestResults = null;

            Regression chosen = newReport.Model;
            if (chosen != null)
            {
                // simulate one new data set for testing
                RegCase r = newReport.Properties;
                var dist = new ContinuousUniform();
                dist.RandomSource = Utilities.MyRandom;
                var sampler = new Sampling(r.Problem, dist);

                sampler.Cancel = () => { return Cancel(); };
                sampler.Sample = (n, t) =>
                {
                    this.OnProgressRetest((double)n / t);
                };
                this.OnProgressRetest(0);
                List<Observation> testSet = sampler.GetSample(r.NumSamples, SampleType.RandomUniform, r.nUnderOne, r.nUnderOnePointFive);
                this.OnProgressRetest(1);
                if (Cancel()) return null;

                // test chosen surrogate model with independent data set
                ErrorMeasures error = new ErrorMeasures(testSet, chosen, (int)Math.Round(PercentTop * r.NumSamples), r.ErrorWeights);
                RegressionTest results = new RegressionTest() { TestSet = testSet, Error = error };
                newReport.TestResults = results;

                //MyView.AddRegResults(MyTestReport);
                return newReport;
            }
            else
            {
                throw new Exception("No chosen regression model to test!!");
            }            
        }
        private Regression Validate(List<Observation> trainSet, List<Observation> valSetOrig, RegCase r)
        {
            // set up models
            Regression rfr = new RandomForestRegression(trainSet);
            Regression enn = new EnsembleNeuralNetRegression(trainSet);
            Regression kr = new KrigingRegression(trainSet);

            // set up results
            List<ValidationResult> RFResults = new List<ValidationResult>();
            List<ValidationResult> ENNResults = new List<ValidationResult>();
            List<ValidationResult> KRResults = new List<ValidationResult>();

            // make a fresh copy of the validation set
            List<Observation> valset = this.CloneObsSet(valSetOrig);

            // set up progress tracking
            int total = Convert.ToInt32(r.BuildRF) * this.RValues.Length + Convert.ToInt32(r.BuildENN) * this.NNodes.Length + Convert.ToInt32(r.BuildKR) * 1;
            int complete = 0;
            this.OnProgressBuild(0);

            // build random forest models
            if (r.BuildRF)
            {
                foreach (double rvalue in this.RValues)
                {
                    rfr.BuildModel(80, rvalue, 1, (int)Math.Round(PercentTop * r.NumSamples));
                    ErrorMeasures rferror = rfr.ValidateModel(valset, (int)Math.Round(PercentTop * r.NumSamples), r.ErrorWeights);
                    RFResults.Add(new ValidationResult(rferror, rvalue, rfr.Clone()));
                    valset.Clear();
                    valset = this.CloneObsSet(valSetOrig);
                    
                    this.OnProgressBuild(++complete / (double)total);
                    if (Cancel()) return null;
                }
            }

            // build ensemble neural network models
            if (r.BuildENN)
            {
                foreach (int nnodes in this.NNodes)
                {
                    enn.BuildModel(nnodes, 0.01, 1, (int)Math.Round(PercentTop * r.NumSamples));
                    ErrorMeasures nnerror = enn.ValidateModel(valset, (int)Math.Round(PercentTop * r.NumSamples), r.ErrorWeights);
                    ENNResults.Add(new ValidationResult(nnerror, nnodes, enn.Clone()));
                    valset.Clear();
                    valset = this.CloneObsSet(valSetOrig);
                    
                    this.OnProgressBuild(++complete / (double)total);
                    if (Cancel()) return null;
                }
            }

            // build Kriging model
            if (r.BuildKR)
            {
                kr.BuildModel(0, 0, 0, 0);
                ErrorMeasures krerror = kr.ValidateModel(valset, (int)Math.Round(PercentTop * r.NumSamples), r.ErrorWeights);
                KRResults.Add(new ValidationResult(krerror, 0, kr.Clone()));

                this.OnProgressBuild(++complete / (double)total);
                if (Cancel()) return null;
            }

            this.OnProgressBuild(1);

            // choose best model and return it
            List<ValidationResult> AllResults = new List<ValidationResult>();
            AllResults.AddRange(ENNResults);
            AllResults.AddRange(RFResults);
            AllResults.AddRange(KRResults);
            AllResults = GetCombinedError(AllResults);

            return AllResults[0].Model;
        }

        private List<Observation> CloneObsSet(List<Observation> obsSet)
        {
            List<Observation> newObsSet = new List<Observation>();
            foreach (Observation o in obsSet)
            {
                newObsSet.Add(o.ObservationClone());
            }
            return newObsSet;
        }
        private List<ValidationResult> GetCombinedError(List<ValidationResult> candidates)
        {
            // sort by RMSE
            candidates.Sort(
                        delegate(ValidationResult first, ValidationResult next)
                        {
                            return first.Error.RootMSE.CompareTo(next.Error.RootMSE);
                        });
            candidates.ForEach(c => c.Error.Ranks[0] = candidates.IndexOf(c));

            // sort by MAE
            candidates.Sort(
            delegate(ValidationResult first, ValidationResult next)
            {
                return first.Error.MeanAbsError.CompareTo(next.Error.MeanAbsError);
            });
            candidates.ForEach(c => c.Error.Ranks[1] = candidates.IndexOf(c));

            // sort by MRE
            candidates.Sort(
            delegate(ValidationResult first, ValidationResult next)
            {
                return first.Error.MeanRankError.CompareTo(next.Error.MeanRankError);
            });
            candidates.ForEach(c => c.Error.Ranks[2] = candidates.IndexOf(c));

            // sort by MTRE
            candidates.Sort(
            delegate(ValidationResult first, ValidationResult next)
            {
                return first.Error.TopXRankError.CompareTo(next.Error.TopXRankError);
            });
            candidates.ForEach(c => c.Error.Ranks[3] = candidates.IndexOf(c));

            // sort by TRE
            candidates.Sort(
            delegate(ValidationResult first, ValidationResult next)
            {
                return first.Error.TopXRatioError.CompareTo(next.Error.TopXRatioError);
            });
            candidates.ForEach(c => c.Error.Ranks[4] = candidates.IndexOf(c));

            // sort by TFE
            candidates.Sort(
            delegate(ValidationResult first, ValidationResult next)
            {
                return first.Error.TopXFactorError.CompareTo(next.Error.TopXFactorError);
            });
            candidates.ForEach(c => c.Error.Ranks[5] = candidates.IndexOf(c));

            // sort by composite error
            candidates.Sort(
           delegate(ValidationResult first, ValidationResult next)
           {
               return first.Error.CompositeError.CompareTo(next.Error.CompositeError);
           });

            return candidates;
        }
    }
}
