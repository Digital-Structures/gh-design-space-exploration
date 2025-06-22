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
    //SURFACE VARIABLE
    //Represents a single control point degree of freedom on a NURBS Surface
    public class SurfaceVariable : GeoVariable
    {
        //CONSTRUCTOR
        public SurfaceVariable(double min, double max, int u, int v, int dir, DesignSurface surf):base(min,max,dir,surf)
        {
            this.u = u;
            this.v = v;

            this.PointName = String.Format(".u{0}v{1}.{2}", this.u+1, this.v+1, ((Direction)this.Dir).ToString());
        }
        public int u;
        public int v;

    }
} 
