using System;
using System.ComponentModel;
using Rhino;

namespace Stepper
{
	// Token: 0x02000011 RID: 17
	public class VarVM : BaseVM, IStepDataElement, IViewModel, INotifyPropertyChanged
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00003210 File Offset: 0x00001410
		// (set) Token: 0x06000085 RID: 133 RVA: 0x00003218 File Offset: 0x00001418
		public double OriginalValue { get; set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000086 RID: 134 RVA: 0x00003221 File Offset: 0x00001421
		// (set) Token: 0x06000087 RID: 135 RVA: 0x00003229 File Offset: 0x00001429
		public double BestSolutionValue { get; set; }

		// Token: 0x06000088 RID: 136 RVA: 0x00003234 File Offset: 0x00001434
		public VarVM(IVariable dvar, Design d)
		{
			this.DesignVar = dvar;
			this.Design = d;
			this.OriginalValue = this.DesignVar.CurrentValue;
			this.BestSolutionValue = this.DesignVar.CurrentValue;
			this._name = this.DesignVar.Parameter.NickName;
			this._value = this.DesignVar.CurrentValue;
			this._min = this.DesignVar.Min;
			this._max = this.DesignVar.Max;
			this.IsActive = true;
			this.OptRunning = false;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000032D4 File Offset: 0x000014D4
		public VarVM(IVariable dvar)
		{
			this.DesignVar = dvar;
			this.OriginalValue = this.DesignVar.CurrentValue;
			this.BestSolutionValue = this.DesignVar.CurrentValue;
			this._name = this.DesignVar.Parameter.NickName;
			this._value = this.DesignVar.CurrentValue;
			this._min = this.DesignVar.Min;
			this._max = this.DesignVar.Max;
			this.IsActive = true;
			this.OptRunning = false;
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600008A RID: 138 RVA: 0x0000336C File Offset: 0x0000156C
		// (set) Token: 0x0600008B RID: 139 RVA: 0x00003389 File Offset: 0x00001589
		public virtual int Dir
		{
			get
			{
				return this.DesignVar.Dir;
			}
			set
			{
				this._dir = (VarVM.Direction)value;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600008C RID: 140 RVA: 0x00003394 File Offset: 0x00001594
		// (set) Token: 0x0600008D RID: 141 RVA: 0x000033AC File Offset: 0x000015AC
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
					bool flag2 = !(this.DesignVar is GeoVariable);
					if (flag2)
					{
						this.DesignVar.Parameter.NickName = this._name;
					}
				}
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600008E RID: 142 RVA: 0x00003404 File Offset: 0x00001604
		// (set) Token: 0x0600008F RID: 143 RVA: 0x0000341C File Offset: 0x0000161C
		public virtual double Value
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
						Action action = delegate()
						{
							for (int i = 0; i < this.Design.ActiveVariables.Count; i++)
							{
								bool flag3 = this.Design.ActiveVariables[i] == this.DesignVar;
								if (flag3)
								{
									this.Design.ActiveVariables[i].UpdateValue(value);
									break;
								}
							}
						};
						RhinoApp.MainApplicationWindow.Invoke(action);
					}
				}
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000090 RID: 144 RVA: 0x000034A4 File Offset: 0x000016A4
		// (set) Token: 0x06000091 RID: 145 RVA: 0x000034BC File Offset: 0x000016BC
		public virtual double Min
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
						this.DesignVar.Min = this._min;
						bool flag3 = this._min > this.DesignVar.CurrentValue;
						if (flag3)
						{
							this.DesignVar.UpdateValue(this._min);
							this.Value = this._min;
						}
					}
				}
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00003548 File Offset: 0x00001748
		// (set) Token: 0x06000093 RID: 147 RVA: 0x00003560 File Offset: 0x00001760
		public virtual double Max
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
						this.DesignVar.Max = this._max;
						bool flag3 = this._max < this.DesignVar.CurrentValue;
						if (flag3)
						{
							this.DesignVar.UpdateValue(this._max);
							this.Value = this._max;
						}
					}
				}
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000094 RID: 148 RVA: 0x000035EC File Offset: 0x000017EC
		// (set) Token: 0x06000095 RID: 149 RVA: 0x00003604 File Offset: 0x00001804
		public virtual bool OpenDialog
		{
			get
			{
				return this.opendialog;
			}
			set
			{
				base.CheckPropertyChanged<bool>("OpenDialog", ref this.opendialog, ref value);
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000096 RID: 150 RVA: 0x0000361C File Offset: 0x0000181C
		// (set) Token: 0x06000097 RID: 151 RVA: 0x00003634 File Offset: 0x00001834
		public bool IsActive
		{
			get
			{
				return this._isactive;
			}
			set
			{
				bool flag = base.CheckPropertyChanged<bool>("IsActive", ref this._isactive, ref value);
				if (flag)
				{
					this.DesignVar.IsActive = this._isactive;
				}
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000366D File Offset: 0x0000186D
		public virtual void OptimizationFinished()
		{
			base.ChangesEnabled = true;
			this.Value = this.DesignVar.CurrentValue;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000368A File Offset: 0x0000188A
		public void UpdateBestSolutionValue()
		{
			this.BestSolutionValue = this.DesignVar.CurrentValue;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000369F File Offset: 0x0000189F
		public void SetBestSolution()
		{
			this.Value = this.BestSolutionValue;
			this.DesignVar.UpdateValue(this.Value);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000036C1 File Offset: 0x000018C1
		public void ResetValue()
		{
			this.Value = this.OriginalValue;
			this.DesignVar.UpdateValue(this.Value);
			this.BestSolutionValue = this.OriginalValue;
		}

		// Token: 0x04000026 RID: 38
		public IVariable DesignVar;

		// Token: 0x04000029 RID: 41
		public Design Design;

		// Token: 0x0400002A RID: 42
		private VarVM.Direction _dir;

		// Token: 0x0400002B RID: 43
		private string _name;

		// Token: 0x0400002C RID: 44
		private double _value;

		// Token: 0x0400002D RID: 45
		private double _min;

		// Token: 0x0400002E RID: 46
		private double _max;

		// Token: 0x0400002F RID: 47
		private bool opendialog;

		// Token: 0x04000030 RID: 48
		private bool _isactive;

		// Token: 0x0200002A RID: 42
		public enum Direction
		{
			// Token: 0x040000D8 RID: 216
			X,
			// Token: 0x040000D9 RID: 217
			Y,
			// Token: 0x040000DA RID: 218
			Z,
			// Token: 0x040000DB RID: 219
			None
		}
	}
}
