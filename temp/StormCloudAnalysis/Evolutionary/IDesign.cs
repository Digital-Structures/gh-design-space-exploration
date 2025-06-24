using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace StormCloud.Evolutionary
{
    public interface IDesign
    {
        IDesign Crossover(IList<IDesign> seeds);
        IDesign Mutate(IContinuousDistribution dist, double rate);
        void Setup();
        IDivBooster GetDivBooster();
        IList<double> GetFeatures();
        IList<double[]> GetBounds();
        double GetOutput();
        double Score
        {
            get;
        }
        void UpdateScore(double s);

        double[] Dimensions
        {
            get;
        }
        double[] ZeroPoint
        {
            get;
        }
        //IDesignVM GetVM();
        int GetMaxPop();
        IDesign DesignClone();
        IDesign GenerateFromVars(double[] v);
        double[] GetPoints();
        List<IDesign> GetCornerDesigns();
        List<IVariable> DesignVariables
        {
            get;
        }
        double? CompTime
        {
            get;
            set;
        }
    }
}
