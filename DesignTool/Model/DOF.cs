using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;

namespace StructureEngine.Model
{
    public class DOF
    {
        public DOF(double coord)
        {
            this.Coord = coord;
            this.Disp = new Dictionary<LoadCase, double>();
        }

        public double Coord // this is the coordinate of the start structure
        {
            get;
            private set;
        }

        public void SetCoord(double c)
        {
            this.Coord = c;
        }

        public Dictionary<LoadCase, double> Disp;

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

        public virtual void CopyTo(DOF newDOF)
        {
            newDOF.Coord = this.Coord;
            newDOF.Pinned = this.Pinned;
            newDOF.Index = this.Index;
        }

        public bool PreFix
        {
            get;
            set;
        }
    }
}
    
