using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using StructureEngine.Model;

namespace StructureEngine.MachineLearning
{
    public abstract class Regression
    {
        public List<Observation> TrainData
        {
            get;
            set;
        }

        public double? Parameter;

        public double[,] XY
        {
            get
            {
                double[,] xy = new double[n, p + 1];
                for (int i = 0; i < n; i++)
                {
                    xy[i, p] = TrainData[i].Output;
                    for (int j = 0; j < p; j++)
                    {
                        xy[i, j] = TrainData[i].Features[j];
                    }
                }
                return xy;
            }
        }

        //public int NumEvals { get; set; }

        public int n
        {
            get
            {
                return TrainData.Count;
            }
        }

        public int p
        {
            get
            {
                return TrainData[0].Features.Count;
            }
        }

        public abstract Regression Clone();

        //public abstract void SetModel();

        public abstract void BuildModel(int n, double d, int b, int top);

        public ErrorMeasures ValidateModel(List<Observation> valSet, int top, Matrix<double> weights)
        {
            return new ErrorMeasures(valSet, this, top, weights);
        }

        public abstract double Predict(Observation test);

        private static Observation ScaleData(Observation o)
        {
            List<double> scaledFeatures = new List<double>();
            foreach (IVariable d in o.obsDesign.DesignVariables)
            {
                double scaledValue = d.GetPoint();
                scaledFeatures.Add(scaledValue);
            }
            Observation oScale = o.ObservationClone();
            oScale.Features = scaledFeatures;
            return oScale;
        }

        public static DenseMatrix GetSampleMatrix(List<Observation> lobs)
        {
            int n = lobs.Count;
            int k = lobs[0].Features.Count;

            DenseMatrix X = new DenseMatrix(n, k);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    X[i, j] = ScaleData(lobs[i]).Features[j];
                    //X[i, j] = lobs[i].Features[j];
                }
            }

            return X;
        }

        public static DenseVector GetSampleVector(Observation o)
        {
            int k = o.Features.Count;
            Observation scaleO = ScaleData(o);
            DenseVector x = new DenseVector(k);
            for (int i = 0; i < k; i++)
            {
                x[i] = scaleO.Features[i];
            }
            return x;
        }

        public static DenseVector GetObsVector(List<Observation> lobs)
        {
            int n = lobs.Count;

            DenseVector y = new DenseVector(n);
            for (int i = 0; i < n; i++)
            {
                y[i] = lobs[i].Output;
            }

            return y;
        }

    }
}
