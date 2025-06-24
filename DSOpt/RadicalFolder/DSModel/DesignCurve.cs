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
    public class DesignCurve : IDesignGeometry
    {
        //Legibily reference directions
        public enum Direction { X, Y, Z }

        public NurbsCurve OriginalCurve;
        public List<Point3d> OriginalPoints;
        public NurbsCurve Curve;
        public List<Point3d> Points;

        public GH_PersistentGeometryParam<GH_Curve> CrvParameter { get { return Parameter as GH_PersistentGeometryParam<GH_Curve>; } }
        public List<GeoVariable> Variables { get; set; }

        public DesignCurve(IGH_Param param, NurbsCurve crv, double min = -1.0, double max = 1.0)
        {
            this.Parameter = param;
            this.Parameter.RemoveAllSources();
            this.Curve = crv;
            this.OriginalCurve = new NurbsCurve(crv);
            BuildVariables(min, max);
        }

        public IGH_Param Parameter
        {
            get; set;
        }

        public void BuildVariables(double min, double max)
        {
            Points = Curve.Points.Distinct().Select(x => x.Location).ToList();
            OriginalPoints = OriginalCurve.Points.Distinct().Select(x => x.Location).ToList();
            //PointsIndices = Points.Select(x => Curve.Points.ToList().IndexOf(x)).ToList();
            Variables = new List<GeoVariable>();
            for (int i = 0; i < Points.Count; i++)
            {
                Variables.Add(new CurveVariable(min, max, i, (int)Direction.X, this));
                Variables.Add(new CurveVariable(min, max, i, (int)Direction.Y, this));
                Variables.Add(new CurveVariable(min, max, i, (int)Direction.Z, this));
            }
        }

        public void Update()
        {
            Curve = NurbsCurve.Create(this.Curve.IsClosed, OriginalCurve.Degree, Points);
            CrvParameter.PersistentData.Clear();
            CrvParameter.PersistentData.Append(new Grasshopper.Kernel.Types.GH_Curve(this.Curve));
        }

        public void VarUpdate(GeoVariable geovar)
        {
            CurveVariable crvvar = (CurveVariable)geovar;
            Point3d newpoint = Points[crvvar.u];
            Point3d originalPoint = OriginalPoints[crvvar.u];

            switch (crvvar.Dir)
            {
                case (int)Direction.X:
                    newpoint.X = originalPoint.X + crvvar.CurrentValue;
                    break;
                case (int)Direction.Y:
                    newpoint.Y = originalPoint.Y + crvvar.CurrentValue;
                    break;
                case (int)Direction.Z:
                    newpoint.Z = originalPoint.Z + crvvar.CurrentValue;
                    break;
            }

            Points[crvvar.u] = newpoint;
        }

    }
}
