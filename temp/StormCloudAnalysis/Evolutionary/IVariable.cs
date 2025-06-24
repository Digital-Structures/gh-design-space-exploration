using System.Collections.Generic;
using MathNet.Numerics.Distributions;

namespace StormCloud.Evolutionary
{
    public interface IVariable
    {
        double Value
        {
            get;
            set;
        }
        void Mutate(double globalrate, IContinuousDistribution dist);
        void Crossover(List<IVariable> mylist);
        bool CheckConstraint();
        void FixConstraint();
        //void SetConstraint();
        double Min
        {
            get;
        }
        double Max
        {
            get;
        }
        int GetBytes();
        //void Project(double d);
        //double GetPoint();
    }
}
