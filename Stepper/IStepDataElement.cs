using System;
using System.ComponentModel;

namespace Stepper
{
	// Token: 0x0200000F RID: 15
	public interface IStepDataElement : IViewModel, INotifyPropertyChanged
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600007C RID: 124
		// (set) Token: 0x0600007D RID: 125
		double Value { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600007E RID: 126
		// (set) Token: 0x0600007F RID: 127
		string Name { get; set; }
	}
}
