using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StructureEngine.Model
{
    public class CoordVar : DOF, IVariable
    {

        public CoordVar(double coord, double varcoord, double variation) : base(coord)
        {
            this.VarCoord = varcoord;
            this.AllowableVariation = variation;
            this.Disp = new Dictionary<LoadCase, double>();
        }

        public CoordVar(double coord, double variation) : this(coord, coord, variation) { }
        
        public double VarCoord; // this coordinate can vary in evolutionary search

        public double AllowableVariation;

        public double Min
        {
            get
            {
                return Coord - AllowableVariation;
            }
        }

        public double Max
        {
            get
            {
                return Coord + AllowableVariation;
            }
        }

        public List<ParametricRelation> Relations
        {
            get;
            set;
        }
       
        public void Mutate(double globalrate, ISetDistribution dist)
        {
            double sigma = globalrate * AllowableVariation;
            dist.Mean = this.VarCoord;
            dist.StdDev = sigma;

            double newcoord = dist.Sample();
            this.VarCoord = newcoord;
        }

        public void Crossover(System.Collections.Generic.List<IVariable> mylist)
        {
            double newcoord = 0;
            double normalize = 0;
            foreach (IVariable dof in mylist)
            {
                double weight = Utilities.MyRandom.NextDouble();
                DOF d = (DOF)dof;
                newcoord += d.Coord * weight;
                normalize += weight;
            }

            newcoord = newcoord / normalize;
            this.VarCoord = newcoord;
        }

        public bool CheckConstraint()
        {
            return (Coord < Max && Coord > Min);
        }

        public void FixConstraint()
        {
            if (!CheckConstraint())
            {
                if (Coord > Max)
                {
                    this.VarCoord = Max;
                }
                else if (Coord < Min)
                {
                    this.VarCoord = Min;
                }
            }
        }
        
        public void SetConstraint()
        {
            this.SetCoord(VarCoord);
        }

        public int GetBytes()
        {
            double range = (double)this.AllowableVariation * 2;
            double log = Math.Log(range, 2);
            int bytes = Convert.ToInt32(log);
            return Math.Max(4, bytes);
        }

        public void Project(double d)
        {
            this.Value = this.Min + d * 2 * (double)this.AllowableVariation;
        }

        public void ShiftCenter(double c)
        {
            this.VarCoord = this.Min + c * 2 * (double)this.AllowableVariation;
        }

        public double GetPoint()
        {
            return (this.Value - this.Min) / (2 * (double)this.AllowableVariation);
        }

        

        public double Value
        {
            get
            {
                return this.VarCoord;
            }
            set
            {
                this.VarCoord = value;
            }
        }

        public override void CopyTo(DOF newDOF)
        {
            base.CopyTo(newDOF);
            CoordVar newCV = newDOF as CoordVar;
            if (newCV != null)
            {
                newCV.PreFix = this.PreFix;
                newCV.AllowableVariation = this.AllowableVariation;
            }
        }
    }
}
