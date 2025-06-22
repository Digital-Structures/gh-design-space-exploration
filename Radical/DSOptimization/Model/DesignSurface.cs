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
    public class DesignSurface : IDesignGeometry
    {
        //Legibily reference directions
        public enum Direction {X,Y,Z}
        
        public DesignSurface(IGH_Param param, NurbsSurface surf, double min=-1.0, double max =1.0)
        {
            this.Parameter = param;
            this.Parameter.RemoveAllSources();
            this.Surface = surf;
            this.OriginalSurface = new NurbsSurface(surf);
            BuildVariables(min, max);
            List<GeoVariable> myVars = Variables;
        }

        // obsolete or to be made obsolete
        public DesignSurface(IGH_Param param, List<Tuple<int, int>> fptsX, List<Tuple<int, int>> fptsY, List<Tuple<int, int>> fptsZ, double min, double max, NurbsSurface surf)
        {
            this.Parameter = param;
            this.Parameter.RemoveAllSources();
            this.Surface = surf;
            this.OriginalSurface = new NurbsSurface(surf);
            BuildVariables(fptsX, fptsY, fptsZ,min,max);
        }

        public DesignSurface(IGH_Param param)
        {
            this.Parameter = param;
        }

        public NurbsSurface OriginalSurface;

        public NurbsSurface Surface;

        public IGH_Param Parameter
        {
            get;set;
        }
        public GH_PersistentGeometryParam<GH_Surface> SrfParameter { get { return Parameter as GH_PersistentGeometryParam<GH_Surface>; } }

        public List<GeoVariable> Variables { get; set; }

        public void BuildVariables(double min, double max)
        {
            Variables = new List<GeoVariable>();
            for (int i = 0; i < Surface.Points.CountU; i++)
            {
                for (int j = 0; j < Surface.Points.CountV; j++)
                {
                    Variables.Add(new SurfaceVariable(min, max, i, j, (int)Direction.X, this));
                    Variables.Add(new SurfaceVariable(min, max, i, j, (int)Direction.Y, this));
                    Variables.Add(new SurfaceVariable(min, max, i, j, (int)Direction.Z, this));
                }
            }

        }

        // obsolete or to be made obsolete
        public void BuildVariables(List<Tuple<int, int>> fptsX, List<Tuple<int, int>> fptsY, List<Tuple<int, int>> fptsZ, double min, double max)
        {
            Variables = new List<GeoVariable>();
            for (int i=0;i<Surface.Points.CountU;i++)
            {
                for (int j=0;j<Surface.Points.CountV;j++)
                {
                    Tuple<int, int> cp = new Tuple<int, int>(i, j);
                    if (!fptsX.Contains(cp)) { Variables.Add(new SurfaceVariable(min, max, i, j, (int)Direction.X, this)); }
                    if (!fptsY.Contains(cp)) { Variables.Add(new SurfaceVariable(min, max, i, j, (int)Direction.Y, this)); }
                    if (!fptsZ.Contains(cp)) { Variables.Add(new SurfaceVariable(min, max, i, j, (int)Direction.Z, this)); }
                }
            }
        }

        public void Update()
        {
            SrfParameter.PersistentData.Clear();
            SrfParameter.PersistentData.Append(new Grasshopper.Kernel.Types.GH_Surface(this.Surface));
        }

        public void VarUpdate(GeoVariable geovar)
        {
            SurfaceVariable srfvar = (SurfaceVariable)geovar;
            // the current surface point (newPoint) tracks the previous changes applied in the iteration at other coordinates
            Point3d newPoint = this.Surface.Points.GetControlPoint(srfvar.u, srfvar.v).Location;
            // we need to grab the originalPoint because coordinate updates have to be applied with respect
            // to the original config (otherwise, the bounds are updated at every iteration and the alg. blows up)
            Point3d originalPoint = this.OriginalSurface.Points.GetControlPoint(srfvar.u, srfvar.v).Location;

            // update newPoint with respect to originalPoint
            switch (srfvar.Dir)
            {
                case (int)Direction.X:
                    newPoint.X = originalPoint.X + srfvar.CurrentValue;
                    break;
                case (int)Direction.Y:
                    newPoint.Y = originalPoint.Y + srfvar.CurrentValue;
                    break;
                case (int)Direction.Z:
                    newPoint.Z = originalPoint.Z + srfvar.CurrentValue;
                    break;
            }

            this.Surface.Points.SetControlPoint(srfvar.u, srfvar.v, newPoint);
        }
    }
}
