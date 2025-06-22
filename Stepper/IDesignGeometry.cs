using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace Stepper
{
	// Token: 0x02000009 RID: 9
	public interface IDesignGeometry
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000041 RID: 65
		// (set) Token: 0x06000042 RID: 66
		List<GeoVariable> Variables { get; set; }

		// Token: 0x06000043 RID: 67
		void VarUpdate(GeoVariable geovar);

		// Token: 0x06000044 RID: 68
		void Update();

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000045 RID: 69
		// (set) Token: 0x06000046 RID: 70
		IGH_Param Parameter { get; set; }
	}
}
