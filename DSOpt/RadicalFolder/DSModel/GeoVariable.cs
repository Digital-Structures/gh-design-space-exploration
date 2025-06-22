using System;
using Grasshopper.Kernel;

namespace DSOptimization
{
    //GEO VARIABLE
    //Base class for manipulation of geometric control point variables
    //Used by SurfaceVariable, CurveVariable, and (potentially) MeshVariable
    public class GeoVariable:IVariable
    {
        public enum Direction { X, Y, Z, None };
        public IDesignGeometry Geometry { get; set; }
        private string pointname;

        //CONSTRUCTOR
        public GeoVariable(double min, double max, int dir, IDesignGeometry geo)
        {
            this._dir = (Direction)dir;
            this._min = min;
            this._max = max;
            this.Geometry = geo;
        }

        //System for naming points in a design
        public string PointName
        {
            get { return pointname; }
            set { pointname = value; }
        }

        //Determines whether variable should be considered in optimization
        public bool IsActive
        {
            get; set;
        }

        //Original osition of the control point
        private double reference;
        public double ReferenceValue
        {
            get { return this.reference; }
            set { this.reference = value; }
        }

        //Position of the control point relative to its starting position
        private double value;
        public double CurrentValue
        {
            get { return this.value; }
            set { this.value = value; }
        }

        //DIRECTION
        private Direction _dir;
        public int Dir
        {
            get { return (int)this._dir; }
            set { this._dir = (Direction)value; }
        }

        //Maximum negative displacement of the control point from its original state
        private double _min;
        public double Min
        {
            get { return this._min; }
            set { this._min = value; }
        }

        //Maximum positive displacement of the control point from its original state
        private double _max;
        public double Max
        {
            get { return this._max; }
            set { this._max = value; }
        }

        //Determines to what type of grasshopper object the 
        public IGH_Param Parameter
        {
            get{return Geometry.Parameter;}
        }

        //Change variable's value and update its corresponding geometry
        public void UpdateValue(double x)
        {
            this.CurrentValue = x;
            Geometry.VarUpdate(this);
        }

        public double Gradient()
        {
            throw new NotImplementedException();
        }
    }
}
