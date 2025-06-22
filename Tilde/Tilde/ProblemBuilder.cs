namespace Tilde
{
    using MathNet.Numerics.LinearAlgebra.Double;
    using MathNet.Numerics.LinearAlgebra;
    using StructureEngine.MachineLearning;
    using StructureEngine.MachineLearning.Testing;
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal class ProblemBuilder
    {
        private TildeComponent component;
        private SurrogateModelBuilder model;
        private RegCase r;
        //private RegressionReport rr;

        public ProblemBuilder(TildeComponent component)
        {
            this.component = component;
            component.model = this.model;
        }

        public double Predict(List<double> Features)
        {
            Observation test = new Observation(Features, 0.0);
            return this.component.rr.Model.Predict(test);
        }

        public void Start()
        {
           
            //Observation observation;
            //MessageBox.Show("Starting...");
            this.model = new SurrogateModelBuilder();
            double ratio = this.component.ratio;
            int numData = this.component.designMap.Count;
            int numTrain = (int)Math.Round(numData * ratio);
            int numVal = numData - numTrain;
            int numVars = this.component.numVariables;

            // Split data into training and validation
            List<Observation> trainSet = new List<Observation>();
            for (int i = 0; i < numTrain; i++)
            {
                List<double> features = new List<double>();
                for (int j = 0; j < numVars; j++)
                {
                    features.Add(this.component.designMap[i][j]);
                }
                Observation obs = new Observation(features, this.component.designMap[i][numVars]);
                trainSet.Add(obs);
            }

            List<Observation> valSet = new List<Observation>();
            for (int i = numTrain; i < numData; i++)
            {
                List<double> features = new List<double>();
                for (int j = 0; j < numVars; j++)
                {
                    features.Add(this.component.designMap[i][j]);
                }
                Observation obs = new Observation(features, this.component.designMap[i][numVars]);
                valSet.Add(obs);
            }

            //// NOTE: REPLACING ALL OF THE FOLLOWING CODE, WHICH DOES NOT SPLIT DATA INTO TRAINING AND VALIDATION CORRECTLY
            //int num2 = (int) Math.Round((double) (this.component.designMap.Count * ratio), 0);
            //int num3 = this.component.designMap.Count - num2;
            //List<Observation> trainSet = new List<Observation>();
            //int num4 = 0;
            //for (num5 = 0; num5 < num2; num5++)
            //{
            //    list2 = new List<double>();
            //    num6 = 0;
            //    while (num6 < this.component.numVariables)
            //    {
            //        list2.Add(this.component.designMap[num5][num6]);
            //        num4++;
            //        num6++;
            //    }
            //    observation = new Observation(list2, this.component.designMap[num5][this.component.numVariables]);
            //    trainSet.Add(observation);
            //}
            //List<Observation> valSet = new List<Observation>();
            //for (num5 = num4; num5 < this.component.designMap.Count; num5++)
            //{
            //    list2 = new List<double>();
            //    for (num6 = 0; num6 < this.component.numVariables; num6++)
            //    {
            //        list2.Add(this.component.designMap[num5][num6]);
            //    }
            //    observation = new Observation(list2, this.component.designMap[num5][this.component.numVariables]);
            //    valSet.Add(observation);
            //}

            // Choose error metrics and model types to consider
            // Currently only using RMSE
            Matrix<double> w = new DenseMatrix(1, 6, new double[] { 1.0, 0.0, 0.0, 0.0, 0.0, 0.0 });
            this.r = new RegCase(valSet.Count, true, true, true, w, null);
            MessageBox.Show("Building Model: A message will appear when finished");
            List<Observation> testSet = new List<Observation>();
            foreach(Observation o in valSet)
            {
                testSet.Add(o.ObservationClone());
            }
            this.component.rr = this.model.BuildModel(this.r, trainSet, valSet, testSet);
            MessageBox.Show("Finished: Model built");
            this.component.modelCreated = true;

            // Track model type
            if (this.component.rr.Model is EnsembleNeuralNetRegression)
            {
                this.component.modelType = "Ensemble Neural Net";
            }
            else if (this.component.rr.Model is RandomForestRegression)
            {
                this.component.modelType = "Random Forest";
            }
            else if (this.component.rr.Model is KrigingRegression)
            {
                this.component.modelType = "Kriging";
            }

            this.component.modelParam = (double) this.component.rr.Model.Parameter;

            List<ValidationResult> results = this.model.BuildResults;

            List<string> allModels = new List<string>();
            List<double> allParams = new List<double>();
            List<double> compositeErrors = new List<double>();
            foreach (ValidationResult r in results)
            {
                string modelName = "";
                Regression reg = r.Model;
                if (reg is EnsembleNeuralNetRegression)
                {
                    modelName = "Ensemble Neural Net";
                }
                else if (reg is RandomForestRegression)
                {
                    modelName = "Random Forest";
                }
                else if (reg is KrigingRegression)
                {
                    modelName = "Kriging";
                }
                allModels.Add(modelName);
                allParams.Add(r.Parameter);
                compositeErrors.Add(r.Error.RootMSE);
            }

            // Set lists of models, parameters, and errors for output
            this.component.allModels = allModels;
            this.component.allParams = allParams;
            this.component.allErrors = compositeErrors;
        }
    }
}

