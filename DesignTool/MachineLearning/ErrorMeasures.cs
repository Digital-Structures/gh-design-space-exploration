using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace StructureEngine.MachineLearning
{
    public class ErrorMeasures
    {
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
            //this.TopXMaxRankError = this.GetTMaxRankE(lobs);
            this.Ranks = new DenseVector(6);
            this.SortedObs = this.GetRankMap(lobs);
            
        }

        public ErrorMeasures Clone()
        {
            var lobs = SortedObs.GetRange(0, SortedObs.Count);
            var clone = new ErrorMeasures(lobs, MyRegression.Clone(), this.Top, this.Weights);
            return clone;
        }

        public double RootMSE { get; set; }
        public double MeanAbsError { get; set; }
        public double MeanRankError { get; set; }
        public double TopXRankError { get; set; }
        public double TopXRatioError { get; set; }
        public double TopXFactorError { get; set; }

        public double[] GetErrors()
        {
            double[] errors = new double[]
            {
                RootMSE, MeanAbsError, MeanRankError, TopXRankError, TopXRatioError, TopXFactorError
            };
            return errors;
        }
        public List<Observation> SortedObs;
        public Matrix<double> Weights;
        public Vector<double> Ranks;
        public int Top;
        public double CompositeError
        {
            get
            {
                return Weights.Multiply(Ranks)[0];
            }
        }
        private Regression MyRegression;

        private double GetMAE(List<Observation> lobs)
        {
            double error = 0;
            foreach (Observation o in lobs)
            {
                double delta = o.Output - MyRegression.Predict(o);
                error += Math.Abs(delta);
            }
            return error / lobs.Count;
        }
        private double GetMSE(List<Observation> lobs)
        {
            double error = 0;
            foreach (Observation o in lobs)
            {
                double delta = Math.Pow(o.Output - MyRegression.Predict(o), 2);
                error += delta;
            }
            return error / lobs.Count;
        }
        private double GetRMSE(List<Observation> lobs)
        {
            double mse = this.GetMSE(lobs);
            return Math.Sqrt(mse);
        }
        private double GetMRE(List<Observation> lobs) // mean rank error
        {
            var ranks = this.GetRankMap(lobs);

            double error = 0;
            foreach (Observation k in ranks)
            {
                //double delta = Math.Abs(k.Key - k.Value) / ((double)k.Key + 1.0);
                double delta = Math.Abs(k.Rank - k.PredictedRank) / (lobs.Count / 2.0);
                error += delta;
            }
            return error / lobs.Count;
        }
        private double GetTRankE(List<Observation> lobs) // top x rank error
        {
            var ranks = this.GetRankMap(lobs);

            double error = 0;
            for (int i = 0; i < Top; i++)
            {
                var k = ranks[i];
                double delta = Math.Abs(k.Rank - k.PredictedRank) / (Top / 2.0);
                error += delta;
            }
            return error / Top;
        }
        private double GetTFactorE(List<Observation> lobs) // top x factor error
        {
            var ranks = this.GetRankMap(lobs);

            int max = 0;
            for (int i = 0; i < Top; i++)
            {
                var k = ranks[i];
                max = Math.Max(max, k.PredictedRank);
            }

            double maxrank = Convert.ToDouble(max) + 1;
            return (maxrank - Top) / (lobs.Count);
        }
        private double GetTMaxRankE(List<Observation> lobs) // top x max rank error
        {
            var ranks = this.GetRankMap(lobs);
            ranks.Sort(delegate(Observation o1, Observation o2) { return o1.Predicted.CompareTo(o2.Predicted); });
            int max = 0;
            for (int i = 0; i < Top; i++)
            {
                var k = ranks[i];
                max = Math.Max(max, k.Rank);
            }
            double maxrank = Convert.ToDouble(max) + 1;
            return (maxrank - Top) / (lobs.Count);
        }
        private double GetTRatioE(List<Observation> lobs) // top x ratio error
        {
            var ranks = this.GetRankMap(lobs);

            double underx = 0;
            for (int i = 0; i < Top; i++)
            {
                var k = ranks[i];
                if (k.PredictedRank < Top)
                {
                    underx++;
                }
            }
            return Math.Abs(Top - underx) / lobs.Count;
        }

        private List<Observation> GetRankMap(List<Observation> lobs)
        {
            lobs.Sort(new ObsCompare());
            List<Observation> ranks = new List<Observation>();
            //var comp = new List<KeyValuePair<int, double>>();
            //var ranks = new List<KeyValuePair<int, int>>();

            foreach (Observation o in lobs)
            {
                o.Rank = lobs.IndexOf(o);
                o.Predicted = MyRegression.Predict(o);
                ranks.Add(o);
                //comp.Add(new KeyValuePair<int, double>(lobs.IndexOf(o), reg.Predict(o))); // actual rank, predicted score
            }

            ranks.Sort(delegate(Observation o1, Observation o2) { return o1.Predicted.CompareTo(o2.Predicted);});
            foreach (Observation o in ranks)
            {
                o.PredictedRank = ranks.IndexOf(o);
            }

            ranks.Sort(new ObsCompare());
            return ranks;
        }
    }

    

    public class ObsCompare : IComparer<Observation>
    {
        public int Compare(Observation o1, Observation o2)
        {
            return o1.Output.CompareTo(o2.Output);
        }
    }
}
