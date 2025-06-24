using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Stepper
{
	// Token: 0x02000007 RID: 7
	public class DesignSurface : IDesignGeometry
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002687 File Offset: 0x00000887
		// (set) Token: 0x06000024 RID: 36 RVA: 0x0000268F File Offset: 0x0000088F
		public IGH_Param Parameter { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00002698 File Offset: 0x00000898
		public GH_PersistentGeometryParam<GH_Surface> SrfParameter
		{
			get
			{
				return this.Parameter as GH_PersistentGeometryParam<GH_Surface>;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000026 RID: 38 RVA: 0x000026B5 File Offset: 0x000008B5
		// (set) Token: 0x06000027 RID: 39 RVA: 0x000026BD File Offset: 0x000008BD
		public List<GeoVariable> Variables { get; set; }

		// Token: 0x06000028 RID: 40 RVA: 0x000026C8 File Offset: 0x000008C8
		public DesignSurface(IGH_Param param, NurbsSurface surf, double min = -1.0, double max = 1.0)
		{
			this.Parameter = param;
			this.Parameter.RemoveAllSources();
			this.Surface = surf;
			this.OriginalSurface = new NurbsSurface(surf);
			this.BuildVariables(min, max);
			List<GeoVariable> variables = this.Variables;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002715 File Offset: 0x00000915
		public DesignSurface(IGH_Param param)
		{
			this.Parameter = param;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002728 File Offset: 0x00000928
		public void BuildVariables(double min, double max)
		{
			this.Variables = new List<GeoVariable>();
			for (int i = 0; i < this.Surface.Points.CountU; i++)
			{
				for (int j = 0; j < this.Surface.Points.CountV; j++)
				{
					this.Variables.Add(new SurfaceVariable(min, max, i, j, 0, this));
					this.Variables.Add(new SurfaceVariable(min, max, i, j, 1, this));
					this.Variables.Add(new SurfaceVariable(min, max, i, j, 2, this));
				}
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000027CC File Offset: 0x000009CC
		public void Update()
		{
			this.SrfParameter.PersistentData.Clear();
			this.SrfParameter.PersistentData.Append(new GH_Surface(this.Surface));
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000027FC File Offset: 0x000009FC
		public void VarUpdate(GeoVariable geovar)
		{
			SurfaceVariable surfaceVariable = (SurfaceVariable)geovar;
			Point3d location = this.Surface.Points.GetControlPoint(surfaceVariable.u, surfaceVariable.v).Location;
			Point3d location2 = this.OriginalSurface.Points.GetControlPoint(surfaceVariable.u, surfaceVariable.v).Location;
			switch (surfaceVariable.Dir)
			{
			case 0:
				location.X = location2.X + surfaceVariable.CurrentValue;
				break;
			case 1:
				location.Y = location2.Y + surfaceVariable.CurrentValue;
				break;
			case 2:
				location.Z = location2.Z + surfaceVariable.CurrentValue;
				break;
			}
			this.Surface.Points.SetControlPoint(surfaceVariable.u, surfaceVariable.v, location);
		}

		// Token: 0x0400000B RID: 11
		public NurbsSurface OriginalSurface;

		// Token: 0x0400000C RID: 12
		public NurbsSurface Surface;

		// Token: 0x02000027 RID: 39
		public enum Direction
		{
			// Token: 0x040000CA RID: 202
			X,
			// Token: 0x040000CB RID: 203
			Y,
			// Token: 0x040000CC RID: 204
			Z
		}
	}
}
