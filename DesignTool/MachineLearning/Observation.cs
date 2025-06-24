using System.Collections.Generic;
using StructureEngine.Model;

namespace StructureEngine.MachineLearning
{
    public class Observation
    {
        public Observation(IDesign s)
        {
            obsDesign = s;
            Features = obsDesign.GetFeatures();
        }

        public IList<double> Features
        {
            get;
            set;
        }

        private double? _output;
        public double Output
        {
            get
            {
                if (!_output.HasValue)
                {
                    _output = obsDesign.GetOutput();
                }
                return _output.Value;
            }
            set
            {
                _output = value;
            }
        }

        public int Rank
        {
            get;
            set;
        }

        public int PredictedRank
        {
            get;
            set;
        }

        public double Predicted
        {
            get;
            set;
        }

        public IDesign obsDesign
        {
            get;
            set;
        }

        public Observation ObservationClone()
        {
            Observation newObs = new Observation(this.obsDesign);
            newObs.Output = this.Output;
            return newObs;
        }
    }
}
