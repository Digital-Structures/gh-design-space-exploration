using System;
using Grasshopper.Kernel;

namespace Stepper
{
	// Token: 0x0200000A RID: 10
	public interface IVariable
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000047 RID: 71
		// (set) Token: 0x06000048 RID: 72
		double ReferenceValue { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000049 RID: 73
		// (set) Token: 0x0600004A RID: 74
		double CurrentValue { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600004B RID: 75
		// (set) Token: 0x0600004C RID: 76
		double Max { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600004D RID: 77
		// (set) Token: 0x0600004E RID: 78
		double Min { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600004F RID: 79
		// (set) Token: 0x06000050 RID: 80
		int Dir { get; set; }

		// Token: 0x06000051 RID: 81
		void UpdateValue(double x);

		// Token: 0x06000052 RID: 82
		double Gradient();

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000053 RID: 83
		// (set) Token: 0x06000054 RID: 84
		bool IsActive { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000055 RID: 85
		IGH_Param Parameter { get; }
	}
}
