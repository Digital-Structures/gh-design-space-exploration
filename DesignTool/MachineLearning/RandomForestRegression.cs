using System;
using System.Collections.Generic;

namespace StructureEngine.MachineLearning
{
    public class RandomForestRegression : Regression
    {
        public RandomForestRegression(List<Observation> lobs)
        {
            TrainData = lobs;
            //this.NumEvals = n;
        }

        public alglib.decisionforest df;

        public int info;

        public alglib.dfreport rep;

        public override void BuildModel(int ntrees, double r, int blank, int top)
        {
            //MainPage.CommonData.RFR = this;
            alglib.dfbuildrandomdecisionforest(XY, n, p, 1, ntrees, r, out info, out df, out rep);
            this.Parameter = r;
            //var err = new ErrorMeasures(this.trainData, this, top);
            //return err;
        }

        public override double Predict(Observation test)
        {
            // start "stopwatch"
            long Before = DateTime.Now.Ticks;

            double[] x = new double[p];
            for (int i = 0; i < p; i++)
            {
                x[i] = test.Features[i];
            }
            double[] y = new double[1];
            alglib.dfprocess(df, x, ref y);
            test.Predicted = y[0];

            // stop "stopwatch"
            long After = DateTime.Now.Ticks;
            TimeSpan elapsedTime = new TimeSpan(After - Before);
            double Milliseconds = elapsedTime.TotalMilliseconds;

            return y[0];
        }

        public override Regression Clone()
        {
            RandomForestRegression copy = new RandomForestRegression(this.TrainData);
            copy.df = this.df;
            copy.info = this.info;
            copy.rep = this.rep;
            copy.Parameter = this.Parameter;
            return copy;
        }
    }
}
