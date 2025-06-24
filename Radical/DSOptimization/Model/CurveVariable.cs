using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;


namespace DSOptimization
{
    //CURVE VARIABLE
    //Represents a single control point degree of freedom on a NURBS Curve
    public class CurveVariable : GeoVariable
    {
        //CONSTRUCTOR
        public CurveVariable(double min, double max, int u, int dir, DesignCurve crv):base(min,max,dir,crv)
        {
            this.u = u;

            this.PointName = String.Format(".u{0}.{1}", this.u+1, ((Direction)this.Dir).ToString());
        }
        public int u;
    }

}
