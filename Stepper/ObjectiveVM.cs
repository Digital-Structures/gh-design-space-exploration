using System;
using System.ComponentModel;

namespace Stepper
{
	// Token: 0x0200001B RID: 27
	public class ObjectiveVM : BaseVM, IStepDataElement, IViewModel, INotifyPropertyChanged
	{
		// Token: 0x060000DF RID: 223 RVA: 0x000055B3 File Offset: 0x000037B3
		public ObjectiveVM(double value, StepperVM stepper)
		{
			this._name = "Objective";
			this._val = value;
			this.Stepper = stepper;
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x000055D8 File Offset: 0x000037D8
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x000055F0 File Offset: 0x000037F0
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				bool flag = base.CheckPropertyChanged<string>("Name", ref this._name, ref value);
				if (flag)
				{
					this.Stepper.ObjectiveNamesChanged();
				}
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x00005624 File Offset: 0x00003824
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x0000563C File Offset: 0x0000383C
		public double Value
		{
			get
			{
				return this._val;
			}
			set
			{
				base.CheckPropertyChanged<double>("Value", ref this._val, ref value);
			}
		}

		// Token: 0x0400005B RID: 91
		private StepperVM Stepper;

		// Token: 0x0400005C RID: 92
		public int index;

		// Token: 0x0400005D RID: 93
		private string _name;

		// Token: 0x0400005E RID: 94
		private double _val;
	}
}
