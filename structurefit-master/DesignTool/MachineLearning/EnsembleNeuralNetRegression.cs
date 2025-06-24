using System;
using System.Collections.Generic;

namespace StructureEngine.MachineLearning
{
    public class EnsembleNeuralNetRegression : Regression
    {
        public EnsembleNeuralNetRegression(List<Observation> lobs)
        {
            TrainData = lobs;
            //this.NumEvals = n;
        }



        //public alglib.multilayerperceptron enn;
        public alglib.mlpensemble enn;

        public int info;

        public alglib.mlpreport rep_mlp;

        public alglib.mlpcvreport rep_mlpcv;

        //public override void SetModel()
        //{
        //    MainPage.CommonData.ENN = this;
        //}

        public override void BuildModel(int nnodes, double decay, int restarts, int top)
        {
            //MainPage.CommonData.ENN = this;
            // alglib.mlpcreate2(p, 20, 20, 1, out enn);
            alglib.mlpecreate1(p, nnodes, 1, 10, out enn);
            alglib.mlpebagginglbfgs(enn, XY, n, decay, restarts, 0.01, 10, out info, out rep_mlp, out rep_mlpcv);
            this.Parameter = nnodes;

            // return training error
            //return new ErrorMeasures(this.trainData, this, top);
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
            alglib.mlpeprocess(enn, x, ref y);
            test.Predicted = y[0];

            // stop "stopwatch"
            long After = DateTime.Now.Ticks;
            TimeSpan elapsedTime = new TimeSpan(After - Before);
            double Milliseconds = elapsedTime.TotalMilliseconds;

            return y[0];
        }

        public override Regression Clone()
        {
            EnsembleNeuralNetRegression copy = new EnsembleNeuralNetRegression(this.TrainData);
            copy.enn = this.enn;
            copy.info = this.info;
            copy.rep_mlp = this.rep_mlp;
            copy.rep_mlpcv = this.rep_mlpcv;
            copy.Parameter = this.Parameter;
            return copy;
        }
    }
}
