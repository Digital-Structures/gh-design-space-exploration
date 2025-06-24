using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Grasshopper;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using Rhino;

namespace Stepper
{
	// Token: 0x0200001D RID: 29
	public class StepperVM : BaseVM
	{
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000EC RID: 236 RVA: 0x00005810 File Offset: 0x00003A10
		// (set) Token: 0x060000ED RID: 237 RVA: 0x00005818 File Offset: 0x00003A18
		public StepperComponent Component { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000EE RID: 238 RVA: 0x00005821 File Offset: 0x00003A21
		// (set) Token: 0x060000EF RID: 239 RVA: 0x00005829 File Offset: 0x00003A29
		public List<VarVM> Variables { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000F0 RID: 240 RVA: 0x00005832 File Offset: 0x00003A32
		// (set) Token: 0x060000F1 RID: 241 RVA: 0x0000583A File Offset: 0x00003A3A
		public List<VarVM> NumVars { get; set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x00005843 File Offset: 0x00003A43
		// (set) Token: 0x060000F3 RID: 243 RVA: 0x0000584B File Offset: 0x00003A4B
		public List<List<VarVM>> GeoVars { get; set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x00005854 File Offset: 0x00003A54
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x0000585C File Offset: 0x00003A5C
		public List<GroupVarVM> GroupVars { get; set; }

		// Token: 0x060000F6 RID: 246 RVA: 0x00005865 File Offset: 0x00003A65
		public StepperVM()
		{
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00005870 File Offset: 0x00003A70
		public StepperVM(Design design, StepperComponent stepper)
		{
			this.Component = stepper;
			this.Design = design;
			this.index = 0;
			this.step = 0.05;
			this.fdstep = 0.01;
			this.trackedstep = 0;
			this.NumVars = new List<VarVM>();
			this.GeoVars = new List<List<VarVM>>();
			this.GroupVars = new List<GroupVarVM>();
			this.SortVariables();
			this._openisodialog = false;
			this.Variables = new List<VarVM>();
			this.Variables.AddRange(this.NumVars);
			this.Variables.AddRange(this.GeoVars.SelectMany((List<VarVM> x) => x).ToList<VarVM>());
			this.ObjectiveEvolution_Norm = new ChartValues<ChartValues<double>>();
			this.ObjectiveEvolution_Abs = new ChartValues<ChartValues<double>>();
			this.Objectives = new List<ObjectiveVM>();
			this.TimeEvolution = new List<TimeSpan>();
			this.TimeEvolution.Add(TimeSpan.Zero);
			this.GradientEvolution = new List<List<List<double?>>>();
			int num = 0;
			foreach (double num2 in this.Design.Objectives)
			{
				ObjectiveVM objectiveVM = new ObjectiveVM(num2, this);
				objectiveVM.Name = this.Component.Params.Input[0].Sources[num].NickName;
				this.Objectives.Add(objectiveVM);
				this.ObjectiveEvolution_Norm.Add(new ChartValues<double>
				{
					1.0
				});
				this.ObjectiveEvolution_Abs.Add(new ChartValues<double>
				{
					num2
				});
				this.GradientEvolution.Add(new List<List<double?>>());
				num++;
			}
			this.VariableEvolution = new List<List<double>>();
			foreach (VarVM varVM in this.Variables)
			{
				this.VariableEvolution.Add(new List<double>
				{
					varVM.Value
				});
				foreach (List<List<double?>> list in this.GradientEvolution)
				{
					list.Add(new List<double?>());
				}
			}
			this.ObjectiveChart_Norm = new StepperGraphVM(this.ObjectiveEvolution_Norm);
			this.ObjectiveChart_Abs = new StepperGraphVM(this.ObjectiveEvolution_Abs);
			this.ObjectiveNamesChanged();
			this._disablingNotAllowed = false;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00005B5C File Offset: 0x00003D5C
		public void Optimize(StepperOptimizer.Direction dir, List<List<double>> GradientData, StepperOptimizer optimizer)
		{
			optimizer.ConvertFromCalculatorToOptimizer(this.ObjIndex, dir, this.StepSize);
			DateTime now = DateTime.Now;
			optimizer.Optimize(GradientData);
			foreach (GroupVarVM groupVarVM in this.GroupVars)
			{
				groupVarVM.OptimizationFinished();
			}
			foreach (List<VarVM> list in this.GeoVars)
			{
				foreach (VarVM varVM in list)
				{
					varVM.OptimizationFinished();
				}
			}
			foreach (IDesignGeometry designGeometry in this.Design.Geometries)
			{
				designGeometry.Update();
			}
			Instances.ActiveCanvas.Document.NewSolution(false, (Grasshopper.Kernel.GH_SolutionMode)2);
			this.UpdateEvolutionData(GradientData, now);
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00005CC0 File Offset: 0x00003EC0
		public void Reset()
		{
			bool finished = false;
			int step = this.TrackedStep;
			Action action = delegate()
			{
				int num = 0;
				foreach (VarVM varVM in this.Variables)
				{
					this.Variables[num].Value = this.VariableEvolution[num][step];
					num++;
				}
				foreach (IDesignGeometry designGeometry in this.Design.Geometries)
				{
					designGeometry.Update();
				}
				bool flag = this.Design.Geometries.Any<IDesignGeometry>();
				if (flag)
				{
					Instances.ActiveCanvas.Document.NewSolution(true, 0);
					finished = true;
				}
				else
				{
					Instances.ActiveCanvas.Document.NewSolution(false, 0);
					finished = true;
				}
			};
			RhinoApp.MainApplicationWindow.Invoke(action);
			while (!finished)
			{
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00005D18 File Offset: 0x00003F18
		public List<string> ObjectiveNames
		{
			get
			{
				List<string> list = new List<string>();
				foreach (ObjectiveVM objectiveVM in this.Objectives)
				{
					list.Add(objectiveVM.Name);
				}
				return list;
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00005D80 File Offset: 0x00003F80
		public void ObjectiveNamesChanged()
		{
			int objIndex = this.ObjIndex;
			base.FirePropertyChanged("ObjectiveNames");
			bool flag = this.ObjectiveChart_Abs != null;
			if (flag)
			{
				int num = 0;
				foreach (ISeriesView seriesView in this.ObjectiveChart_Norm.ObjectiveSeries)
				{
					LineSeries lineSeries = (LineSeries)seriesView;
					lineSeries.SetBinding(Series.TitleProperty, new Binding
					{
						Source = this.ObjectiveNames[num]
					});
					((LineSeries)this.ObjectiveChart_Abs.ObjectiveSeries[num]).SetBinding(Series.TitleProperty, new Binding
					{
						Source = this.ObjectiveNames[num]
					});
					num++;
				}
			}
			this.ObjIndex = objIndex;
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000FC RID: 252 RVA: 0x00005E68 File Offset: 0x00004068
		// (set) Token: 0x060000FD RID: 253 RVA: 0x00005E80 File Offset: 0x00004080
		public int ObjIndex
		{
			get
			{
				return this.index;
			}
			set
			{
				base.CheckPropertyChanged<int>("ObjIndex", ref this.index, ref value);
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00005E98 File Offset: 0x00004098
		public string CurrentObjectiveName
		{
			get
			{
				return this.Objectives[this.ObjIndex].Name;
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060000FF RID: 255 RVA: 0x00005EC0 File Offset: 0x000040C0
		// (set) Token: 0x06000100 RID: 256 RVA: 0x00005ED8 File Offset: 0x000040D8
		public double StepSize
		{
			get
			{
				return this.step;
			}
			set
			{
				base.CheckPropertyChanged<double>("StepSize", ref this.step, ref value);
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000101 RID: 257 RVA: 0x00005EF0 File Offset: 0x000040F0
		// (set) Token: 0x06000102 RID: 258 RVA: 0x00005F08 File Offset: 0x00004108
		public double FDStepSize
		{
			get
			{
				return this.fdstep;
			}
			set
			{
				bool flag = value > 0.0 && value <= 0.5;
				if (flag)
				{
					base.CheckPropertyChanged<double>("FDStepSize", ref this.fdstep, ref value);
				}
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00005F50 File Offset: 0x00004150
		// (set) Token: 0x06000104 RID: 260 RVA: 0x00005F68 File Offset: 0x00004168
		public int TrackedStep
		{
			get
			{
				return this.trackedstep;
			}
			set
			{
				bool flag = base.CheckPropertyChanged<int>("TrackedStep", ref this.trackedstep, ref value);
				if (flag)
				{
					this.ObjectiveChart_Norm.GraphStep = value;
					this.ObjectiveChart_Abs.GraphStep = value;
				}
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000105 RID: 261 RVA: 0x00005FAC File Offset: 0x000041AC
		// (set) Token: 0x06000106 RID: 262 RVA: 0x00005FC4 File Offset: 0x000041C4
		public bool DisablingNotAllowed
		{
			get
			{
				return this._disablingNotAllowed;
			}
			set
			{
				bool flag = base.CheckPropertyChanged<bool>("DisablingNitAllowed", ref this._disablingNotAllowed, ref value);
				if (flag)
				{
				}
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000107 RID: 263 RVA: 0x00005FEC File Offset: 0x000041EC
		public int NumSteps
		{
			get
			{
				return this.ObjectiveEvolution_Norm[0].Count - 1;
			}
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00006014 File Offset: 0x00004214
		private void SortVariables()
		{
			int num = 1;
			foreach (IDesignGeometry designGeometry in this.Design.Geometries)
			{
				List<VarVM> list = new List<VarVM>();
				int num2 = 0;
				foreach (GeoVariable geoVariable in designGeometry.Variables)
				{
					VarVM varVM = new VarVM(geoVariable, this.Design);
					int dir = geoVariable.Dir;
					VarVM varVM2 = varVM;
					varVM2.Name += ((GeoVariable)varVM.DesignVar).PointName;
					list.Add(varVM);
					num2++;
				}
				this.GeoVars.Add(list);
				num++;
			}
			int num3 = 0;
			foreach (IVariable dvar in from numVar in this.Design.Variables
			where numVar is SliderVariable
			select numVar)
			{
				VarVM varVM3 = new VarVM(dvar, this.Design);
				bool flag = varVM3.Name == "";
				if (flag)
				{
					varVM3.Name = varVM3.DesignVar.Parameter.TypeName + " " + num3;
				}
				this.NumVars.Add(varVM3);
				num3++;
			}
		}

		// Token: 0x06000109 RID: 265 RVA: 0x000061E8 File Offset: 0x000043E8
		public void UpdateEvolutionData(List<List<double>> GradientData, DateTime start)
		{
			int num = 0;
			foreach (ChartValues<double> chartValues in this.ObjectiveEvolution_Abs)
			{
				double num2 = this.Design.Objectives[num];
				chartValues.Add(num2);
				double item = num2 / Math.Abs(chartValues[0]);
				this.ObjectiveEvolution_Norm[num].Add(item);
				num++;
			}
			num = 0;
			foreach (VarVM varVM in this.Variables)
			{
				this.VariableEvolution[num].Add(varVM.Value);
				num++;
			}
			for (int i = 0; i < this.Objectives.Count; i++)
			{
				for (int j = 0; j < this.Variables.Count<VarVM>(); j++)
				{
					List<double?> list = this.GradientEvolution[i][j];
					bool flag = GradientData.Any<List<double>>();
					if (flag)
					{
						list.Add(new double?(GradientData[i][j]));
					}
					else
					{
						list.Add(null);
					}
				}
			}
			base.FirePropertyChanged("NumSteps");
			this.ObjectiveChart_Norm.XAxisSteps = this.NumSteps / 10 + 1;
			this.ObjectiveChart_Abs.XAxisSteps = this.NumSteps / 10 + 1;
			this.TimeEvolution.Add(DateTime.Now.Subtract(start));
		}

		// Token: 0x0600010A RID: 266 RVA: 0x000063C8 File Offset: 0x000045C8
		public bool IsoperformancePossible()
		{
			return this.Design.ActiveVariables.Count > this.Design.Objectives.Count && this.Design.ActiveVariables.Count >= 2;
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x0600010B RID: 267 RVA: 0x00006420 File Offset: 0x00004620
		// (set) Token: 0x0600010C RID: 268 RVA: 0x00006438 File Offset: 0x00004638
		public virtual bool OpenIsoDialog
		{
			get
			{
				return this._openisodialog;
			}
			set
			{
				base.CheckPropertyChanged<bool>("OpenIsoDialog", ref this._openisodialog, ref value);
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600010D RID: 269 RVA: 0x00006450 File Offset: 0x00004650
		// (set) Token: 0x0600010E RID: 270 RVA: 0x00006468 File Offset: 0x00004668
		public Visibility FilePathErrorVisibility
		{
			get
			{
				return this._filepatherrorvisibility;
			}
			set
			{
				base.CheckPropertyChanged<Visibility>("FilePathErrorVisibility", ref this._filepatherrorvisibility, ref value);
			}
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00006480 File Offset: 0x00004680
		public void ExportCSV_Log(string filename)
		{
			ChartValues<ChartValues<double>> objectiveEvolution_Abs = this.ObjectiveEvolution_Abs;
			List<List<double>> variableEvolution = this.VariableEvolution;
			List<List<List<double?>>> gradientEvolution = this.GradientEvolution;
			int count = objectiveEvolution_Abs.Count;
			int count2 = variableEvolution.Count;
			string text = "";
			for (int i = 0; i < count + count2 + 2; i++)
			{
				text += ",";
			}
			text += "Gradients\r\n";
			string text2 = "Objectives,";
			for (int j = 0; j < count; j++)
			{
				text2 += ",";
			}
			text2 += "Variables,";
			for (int k = 0; k < count2; k++)
			{
				text2 += ",";
			}
			for (int l = 0; l < count; l++)
			{
				text2 = text2 + this.ObjectiveNames[l] + ",";
				for (int m = 0; m < count2; m++)
				{
					text2 += ",";
				}
			}
			text2 += "Time, ";
			text = text + text2 + "\r\n";
			text2 = "";
			for (int n = 0; n < count; n++)
			{
				text2 = text2 + this.ObjectiveNames[n] + ",";
			}
			text2 += ",";
			for (int num = 0; num < count + 1; num++)
			{
				foreach (VarVM varVM in this.Variables)
				{
					text2 = text2 + varVM.Name + ",";
				}
				text2 += ",";
			}
			text = text + text2 + "\r\n";
			for (int num2 = 0; num2 < this.NumSteps + 1; num2++)
			{
				text2 = "";
				for (int num3 = 0; num3 < count; num3++)
				{
					text2 = text2 + this.ObjectiveEvolution_Abs[num3][num2] + ",";
				}
				text2 += ",";
				for (int num4 = 0; num4 < count2; num4++)
				{
					text2 = text2 + this.VariableEvolution[num4][num2] + ",";
				}
				text2 += ",";
				bool flag = num2 < this.GradientEvolution[0][0].Count && this.GradientEvolution[0][0][num2] != null;
				if (flag)
				{
					for (int num5 = 0; num5 < count; num5++)
					{
						for (int num6 = 0; num6 < count2; num6++)
						{
							text2 = text2 + this.GradientEvolution[num5][num6][num2] + ",";
						}
						text2 += ",";
					}
				}
				bool flag2 = num2 == this.NumSteps;
				if (flag2)
				{
					for (int num7 = 0; num7 < count; num7++)
					{
						for (int num8 = 0; num8 < count2; num8++)
						{
							text2 += ",";
						}
						text2 += ",";
					}
				}
				text2 += this.TimeEvolution[num2].ToString("G");
				text2 += ",";
				text = text + text2 + "\r\n";
			}
			StreamWriter streamWriter = new StreamWriter(filename + "_log.csv");
			streamWriter.Write(text);
			streamWriter.Close();
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000068E4 File Offset: 0x00004AE4
		public void ExportCSV_Raw(string filename)
		{
			ChartValues<ChartValues<double>> objectiveEvolution_Abs = this.ObjectiveEvolution_Abs;
			List<List<double>> variableEvolution = this.VariableEvolution;
			List<List<List<double?>>> gradientEvolution = this.GradientEvolution;
			int count = objectiveEvolution_Abs.Count;
			int count2 = variableEvolution.Count;
			string text = "";
			for (int i = 0; i < this.NumSteps + 1; i++)
			{
				string text2 = "";
				for (int j = 0; j < count2; j++)
				{
					text2 = text2 + this.VariableEvolution[j][i] + ",";
				}
				for (int k = 0; k < count; k++)
				{
					text2 = text2 + this.ObjectiveEvolution_Abs[k][i] + ",";
				}
				text = text + text2 + "\r\n";
			}
			StreamWriter streamWriter = new StreamWriter(filename + "_raw.csv");
			streamWriter.Write(text);
			streamWriter.Close();
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000069FC File Offset: 0x00004BFC
		public void OnWindowClosing()
		{
			this.Component.IsWindowOpen = false;
		}

		// Token: 0x04000065 RID: 101
		public ChartValues<ChartValues<double>> ObjectiveEvolution_Norm;

		// Token: 0x04000066 RID: 102
		public ChartValues<ChartValues<double>> ObjectiveEvolution_Abs;

		// Token: 0x04000067 RID: 103
		public List<List<double>> VariableEvolution;

		// Token: 0x04000068 RID: 104
		public List<List<List<double?>>> GradientEvolution;

		// Token: 0x04000069 RID: 105
		public List<TimeSpan> TimeEvolution;

		// Token: 0x0400006A RID: 106
		public List<ObjectiveVM> Objectives;

		// Token: 0x0400006F RID: 111
		public StepperGraphVM ObjectiveChart_Norm;

		// Token: 0x04000070 RID: 112
		public StepperGraphVM ObjectiveChart_Abs;

		// Token: 0x04000071 RID: 113
		public Design Design;

		// Token: 0x04000072 RID: 114
		private int index;

		// Token: 0x04000073 RID: 115
		private double step;

		// Token: 0x04000074 RID: 116
		private double fdstep;

		// Token: 0x04000075 RID: 117
		private int trackedstep;

		// Token: 0x04000076 RID: 118
		private bool _disablingNotAllowed;

		// Token: 0x04000077 RID: 119
		private bool _openisodialog;

		// Token: 0x04000078 RID: 120
		private Visibility _filepatherrorvisibility;
	}
}
