using System;
using Grasshopper.Kernel;

namespace Stepper
{
	// Token: 0x02000008 RID: 8
	public class GeoVariable : IVariable
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600002D RID: 45 RVA: 0x000028DE File Offset: 0x00000ADE
		// (set) Token: 0x0600002E RID: 46 RVA: 0x000028E6 File Offset: 0x00000AE6
		public IDesignGeometry Geometry { get; set; }

		// Token: 0x0600002F RID: 47 RVA: 0x000028EF File Offset: 0x00000AEF
		public GeoVariable(double min, double max, int dir, IDesignGeometry geo)
		{
			this._dir = (GeoVariable.Direction)dir;
			this._min = min;
			this._max = max;
			this.Geometry = geo;
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002918 File Offset: 0x00000B18
		// (set) Token: 0x06000031 RID: 49 RVA: 0x00002930 File Offset: 0x00000B30
		public string PointName
		{
			get
			{
				return this.pointname;
			}
			set
			{
				this.pointname = value;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000032 RID: 50 RVA: 0x0000293A File Offset: 0x00000B3A
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00002942 File Offset: 0x00000B42
		public bool IsActive { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000034 RID: 52 RVA: 0x0000294C File Offset: 0x00000B4C
		// (set) Token: 0x06000035 RID: 53 RVA: 0x00002964 File Offset: 0x00000B64
		public double ReferenceValue
		{
			get
			{
				return this.reference;
			}
			set
			{
				this.reference = value;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002970 File Offset: 0x00000B70
		// (set) Token: 0x06000037 RID: 55 RVA: 0x00002988 File Offset: 0x00000B88
		public double CurrentValue
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00002994 File Offset: 0x00000B94
		// (set) Token: 0x06000039 RID: 57 RVA: 0x000029AC File Offset: 0x00000BAC
		public int Dir
		{
			get
			{
				return (int)this._dir;
			}
			set
			{
				this._dir = (GeoVariable.Direction)value;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600003A RID: 58 RVA: 0x000029B8 File Offset: 0x00000BB8
		// (set) Token: 0x0600003B RID: 59 RVA: 0x000029D0 File Offset: 0x00000BD0
		public double Min
		{
			get
			{
				return this._min;
			}
			set
			{
				this._min = value;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600003C RID: 60 RVA: 0x000029DC File Offset: 0x00000BDC
		// (set) Token: 0x0600003D RID: 61 RVA: 0x000029F4 File Offset: 0x00000BF4
		public double Max
		{
			get
			{
				return this._max;
			}
			set
			{
				this._max = value;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00002A00 File Offset: 0x00000C00
		public IGH_Param Parameter
		{
			get
			{
				return this.Geometry.Parameter;
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002A1D File Offset: 0x00000C1D
		public void UpdateValue(double x)
		{
			this.CurrentValue = x;
			this.Geometry.VarUpdate(this);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002A35 File Offset: 0x00000C35
		public double Gradient()
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000010 RID: 16
		private string pointname;

		// Token: 0x04000012 RID: 18
		private double reference;

		// Token: 0x04000013 RID: 19
		private double value;

		// Token: 0x04000014 RID: 20
		private GeoVariable.Direction _dir;

		// Token: 0x04000015 RID: 21
		private double _min;

		// Token: 0x04000016 RID: 22
		private double _max;

		// Token: 0x02000028 RID: 40
		public enum Direction
		{
			// Token: 0x040000CE RID: 206
			X,
			// Token: 0x040000CF RID: 207
			Y,
			// Token: 0x040000D0 RID: 208
			Z,
			// Token: 0x040000D1 RID: 209
			None
		}
	}
}
