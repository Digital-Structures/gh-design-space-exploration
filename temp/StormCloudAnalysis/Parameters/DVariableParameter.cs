using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using StormCloud;
using StormCloud.Evolutionary;
//using StructureEngine.Evolutionary;


namespace StormCloud.Parameters
{
    class DVariableParameter : GH_Param<GH_Number>, IVariable
    {
        public DVariableParameter() : base(new IGH_InstanceDescription()) { }

        public double Value
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {

                throw new NotImplementedException();
            }
        }

        public void Mutate(double globalrate, MathNet.Numerics.Distributions.IContinuousDistribution dist)
        {
            throw new NotImplementedException();
        }

        public void Crossover(List<IVariable> mylist)
        {
            throw new NotImplementedException();
        }

        public double Min
        {
            get { throw new NotImplementedException(); }
        }

        public double Max
        {
            get { throw new NotImplementedException(); }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{0A8A36F1-22DC-4BA8-B417-22E3B39402D0}"); }
        }


        public bool CheckConstraint()
        {
            throw new NotImplementedException();
        }

        public void FixConstraint()
        {
            throw new NotImplementedException();
        }

        public int GetBytes()
        {
            throw new NotImplementedException();
        }

        double IVariable.Value
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void IVariable.Mutate(double globalrate, MathNet.Numerics.Distributions.IContinuousDistribution dist)
        {
            throw new NotImplementedException();
        }

        void IVariable.Crossover(List<IVariable> mylist)
        {
            throw new NotImplementedException();
        }

        bool IVariable.CheckConstraint()
        {
            throw new NotImplementedException();
        }

        void IVariable.FixConstraint()
        {
            throw new NotImplementedException();
        }

        double IVariable.Min
        {
            get { throw new NotImplementedException(); }
        }

        double IVariable.Max
        {
            get { throw new NotImplementedException(); }
        }

        int IVariable.GetBytes()
        {
            throw new NotImplementedException();
        }
    }
}
