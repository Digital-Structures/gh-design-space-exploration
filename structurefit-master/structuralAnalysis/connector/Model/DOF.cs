using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;

namespace StructureEngine.Model
{
    public class DOF : IVariable
    {
        public DOF(double coord)
        {
            this.Coord = coord;
            this._coord = coord;
            this.Relations = new List<ParametricRelation>();
            this.Disp = new Dictionary<LoadCase, double>();
            //this.Loads = new List<double>();
        }

        public DOF(double coord, double _coord)
        {
            this.Coord = coord;
            this._coord = _coord;
            this.Relations = new List<ParametricRelation>();
            this.Disp = new Dictionary<LoadCase, double>();
            //this.Loads = new List<double>();
        }

        public Node MyNode;
        public bool IsX;

        private double _coord; // this is the coordinate of the start structure
        public double Coord; // this coordinate can vary in evolutionary search

        public Dictionary<LoadCase, double> Disp;

        public double? AllowableVariation;

        public double Min
        {
            get
            {
                return AllowableVariation == null ? 0 : _coord - (double)AllowableVariation;
            }
        }

        public double Max
        {
            get
            {
                return AllowableVariation == null ? 0 : _coord + (double)AllowableVariation;
            }
        }

        public bool Pinned
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        // TODO: Fix
        //public List<double> Loads
        //{
        //    get;
        //    set;
        //}

        //public double Load
        //{
        //    get;
        //    set;
        //}

        public bool Free
        {
            get;
            set;
        }

        public bool PreFix
        {
            get;
            set;
        }

        public double Value
        {
            get
            {
                return this.Coord;
            }
            set
            {
                this.Coord = value;
            }
        }

        public List<ParametricRelation> Relations
        {
            get;
            set;
        }

        public void CopyTo(DOF newDOF)
        {
            newDOF.Coord = this.Coord;
            newDOF._coord = this._coord;
            newDOF.Pinned = this.Pinned;
            newDOF.Index = this.Index;
            //newDOF.Load = this.Load;
            newDOF.Free = this.Free;
            newDOF.PreFix = this.PreFix;
            newDOF.AllowableVariation = this.AllowableVariation;
        }

        public void Mutate(double globalrate, ISetDistribution dist)
        {
            double sigma = AllowableVariation == null ? 0 : globalrate * (double)AllowableVariation;
            dist.Mean = this.Coord;
            dist.StdDev = sigma;

            double newcoord = dist.Sample();
            this.Coord = newcoord;
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
            Coord = newcoord;
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
                    Coord = Max;
                }
                else if (Coord < Min)
                {
                    Coord = Min;
                }
            }
        }

        public void SetConstraint()
        {
            _coord = this.Coord;
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
            this._coord = this.Min + c * 2 * (double)this.AllowableVariation;
            //this.Coord = this._coord;
        }

        public double GetPoint()
        {
            return (this.Value - this.Min) / (2 * (double)this.AllowableVariation);
        }

    }
}
    
