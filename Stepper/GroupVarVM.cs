using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel.Special;

namespace Stepper
{
	// Token: 0x0200000E RID: 14
	public class GroupVarVM : VarVM
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00002D9C File Offset: 0x00000F9C
		public GroupVarVM(StepperVM VM, int dir, int geoIndex = 0) : base(new SliderVariable(new GH_NumberSlider()))
		{
			this._dir = (VarVM.Direction)dir;
			bool flag = this._dir == VarVM.Direction.None;
			if (flag)
			{
				this.MyVars = VM.NumVars;
			}
			else
			{
				this.MyVars = (from var in VM.GeoVars[geoIndex]
				where var.Dir == this.Dir
				select var).ToList<VarVM>();
			}
			this._value = this.MyVars[0].Value;
			this._min = this.MyVars[0].Min;
			this._max = this.MyVars[0].Max;
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00002E4C File Offset: 0x0000104C
		// (set) Token: 0x06000070 RID: 112 RVA: 0x00002E64 File Offset: 0x00001064
		public override int Dir
		{
			get
			{
				return (int)this._dir;
			}
			set
			{
				this._dir = (VarVM.Direction)value;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00002E70 File Offset: 0x00001070
		// (set) Token: 0x06000072 RID: 114 RVA: 0x00002E88 File Offset: 0x00001088
		public override double Value
		{
			get
			{
				return this._value;
			}
			set
			{
				bool flag = value > this.Max || value < this.Min;
				if (flag)
				{
					this.OpenDialog = true;
				}
				else
				{
					bool flag2 = base.CheckPropertyChanged<double>("Value", ref this._value, ref value);
					if (flag2)
					{
						foreach (VarVM varVM in this.MyVars)
						{
							varVM.Value = value;
							bool openDialog = varVM.OpenDialog;
							if (openDialog)
							{
								this.OpenDialog = true;
							}
						}
						Instances.ActiveCanvas.Document.NewSolution(true, (Grasshopper.Kernel.GH_SolutionMode)2);
					}
				}
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000073 RID: 115 RVA: 0x00002F44 File Offset: 0x00001144
		// (set) Token: 0x06000074 RID: 116 RVA: 0x00002F5C File Offset: 0x0000115C
		public override double Min
		{
			get
			{
				return this._min;
			}
			set
			{
				bool flag = value > this._max;
				if (flag)
				{
					this.OpenDialog = true;
				}
				else
				{
					bool flag2 = base.CheckPropertyChanged<double>("Min", ref this._min, ref value);
					if (flag2)
					{
						foreach (VarVM varVM in this.MyVars)
						{
							varVM.Min = value;
							bool openDialog = varVM.OpenDialog;
							if (openDialog)
							{
								this.OpenDialog = true;
							}
						}
					}
				}
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00002FFC File Offset: 0x000011FC
		// (set) Token: 0x06000076 RID: 118 RVA: 0x00003014 File Offset: 0x00001214
		public override double Max
		{
			get
			{
				return this._max;
			}
			set
			{
				bool flag = value < this._min;
				if (flag)
				{
					this.OpenDialog = true;
				}
				else
				{
					bool flag2 = base.CheckPropertyChanged<double>("Max", ref this._max, ref value);
					if (flag2)
					{
						foreach (VarVM varVM in this.MyVars)
						{
							varVM.Max = value;
							bool openDialog = varVM.OpenDialog;
							if (openDialog)
							{
								this.OpenDialog = true;
							}
						}
					}
				}
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000077 RID: 119 RVA: 0x000030B4 File Offset: 0x000012B4
		// (set) Token: 0x06000078 RID: 120 RVA: 0x000030CC File Offset: 0x000012CC
		public override bool OpenDialog
		{
			get
			{
				return this.opendialog;
			}
			set
			{
				base.CheckPropertyChanged<bool>("OpenDialog", ref this.opendialog, ref value);
				bool flag = !value;
				if (flag)
				{
					foreach (VarVM varVM in this.MyVars)
					{
						varVM.OpenDialog = false;
					}
				}
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00003140 File Offset: 0x00001340
		public void OptimizationStarted()
		{
			base.ChangesEnabled = false;
			foreach (VarVM varVM in this.MyVars)
			{
				varVM.ChangesEnabled = false;
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000031A0 File Offset: 0x000013A0
		public override void OptimizationFinished()
		{
			base.ChangesEnabled = true;
			foreach (VarVM varVM in this.MyVars)
			{
				varVM.OptimizationFinished();
			}
		}

		// Token: 0x04000020 RID: 32
		public List<VarVM> MyVars;

		// Token: 0x04000021 RID: 33
		private VarVM.Direction _dir;

		// Token: 0x04000022 RID: 34
		private double _value;

		// Token: 0x04000023 RID: 35
		private double _min;

		// Token: 0x04000024 RID: 36
		private double _max;

		// Token: 0x04000025 RID: 37
		private bool opendialog;
	}
}
