using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Stepper
{
    // Token: 0x02000006 RID: 6
    public class DesignCurve : IDesignGeometry
    {
        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600001A RID: 26 RVA: 0x00002400 File Offset: 0x00000600
        public GH_PersistentGeometryParam<GH_Curve> CrvParameter
        {
            get
            {
                return this.Parameter as GH_PersistentGeometryParam<GH_Curve>;
            }
        }

        // Token: 0x1700000B RID: 11
        // (get) Token: 0x0600001B RID: 27 RVA: 0x0000241D File Offset: 0x0000061D
        // (set) Token: 0x0600001C RID: 28 RVA: 0x00002425 File Offset: 0x00000625
        public List<GeoVariable> Variables { get; set; }

        // Token: 0x0600001D RID: 29 RVA: 0x0000242E File Offset: 0x0000062E
        public DesignCurve(IGH_Param param, NurbsCurve crv, double min = -1.0, double max = 1.0)
        {
            this.Parameter = param;
            this.Parameter.RemoveAllSources();
            this.Curve = crv;
            this.OriginalCurve = new NurbsCurve(crv);
            this.BuildVariables(min, max);
        }

        // Token: 0x1700000C RID: 12
        // (get) Token: 0x0600001E RID: 30 RVA: 0x00002469 File Offset: 0x00000669
        // (set) Token: 0x0600001F RID: 31 RVA: 0x00002471 File Offset: 0x00000671
        public IGH_Param Parameter { get; set; }

        // Token: 0x06000020 RID: 32 RVA: 0x0000247C File Offset: 0x0000067C
        public void BuildVariables(double min, double max)
        {
            this.Points = (from x in this.Curve.Points.Distinct<ControlPoint>()
                           select x.Location).ToList<Point3d>();
            this.OriginalPoints = (from x in this.OriginalCurve.Points.Distinct<ControlPoint>()
                                   select x.Location).ToList<Point3d>();
            this.Variables = new List<GeoVariable>();
            for (int i = 0; i < this.Points.Count; i++)
            {
                this.Variables.Add(new CurveVariable(min, max, i, 0, this));
                this.Variables.Add(new CurveVariable(min, max, i, 1, this));
                this.Variables.Add(new CurveVariable(min, max, i, 2, this));
            }
        }

        // Token: 0x06000021 RID: 33 RVA: 0x00002574 File Offset: 0x00000774
        public void Update()
        {
            this.Curve = NurbsCurve.Create(this.Curve.IsClosed, this.OriginalCurve.Degree, this.Points);
            this.CrvParameter.PersistentData.Clear();
            this.CrvParameter.PersistentData.Append(new GH_Curve(this.Curve));
        }

        // Token: 0x06000022 RID: 34 RVA: 0x000025D8 File Offset: 0x000007D8
        public void VarUpdate(GeoVariable geovar)
        {
            CurveVariable curveVariable = (CurveVariable)geovar;
            Point3d value = this.Points[curveVariable.u];
            Point3d point3d = this.OriginalPoints[curveVariable.u];
            switch (curveVariable.Dir)
            {
                case 0:
                    value.X = point3d.X + curveVariable.CurrentValue;
                    break;
                case 1:
                    value.Y = point3d.Y + curveVariable.CurrentValue;
                    break;
                case 2:
                    value.Z = point3d.Z + curveVariable.CurrentValue;
                    break;
            }
            this.Points[curveVariable.u] = value;
        }

        // Token: 0x04000005 RID: 5
        public NurbsCurve OriginalCurve;

        // Token: 0x04000006 RID: 6
        public List<Point3d> OriginalPoints;

        // Token: 0x04000007 RID: 7
        public NurbsCurve Curve;

        // Token: 0x04000008 RID: 8
        public List<Point3d> Points;

        // Token: 0x02000025 RID: 37
        public enum Direction
        {
            // Token: 0x040000C3 RID: 195
            X,
            // Token: 0x040000C4 RID: 196
            Y,
            // Token: 0x040000C5 RID: 197
            Z
        }
    }
}
