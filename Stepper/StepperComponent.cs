using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Stepper.Properties;
 // Decompiled using dnSpy

namespace Stepper
{
	// Token: 0x02000019 RID: 25
	public class StepperComponent : GH_Component
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000CA RID: 202 RVA: 0x0000504B File Offset: 0x0000324B
		// (set) Token: 0x060000CB RID: 203 RVA: 0x00005053 File Offset: 0x00003253
		public List<double> Objectives { get; set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000CC RID: 204 RVA: 0x0000505C File Offset: 0x0000325C
		// (set) Token: 0x060000CD RID: 205 RVA: 0x00005064 File Offset: 0x00003264
		public List<double> NumVariables { get; set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00005070 File Offset: 0x00003270
		public IList<IGH_Param> NumObjects
		{
			get
			{
				return base.Params.Input[1].Sources;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000CF RID: 207 RVA: 0x00005098 File Offset: 0x00003298
		// (set) Token: 0x060000D0 RID: 208 RVA: 0x000050A0 File Offset: 0x000032A0
		public bool InputsSatisfied { get; set; }

		// Token: 0x060000D1 RID: 209 RVA: 0x000050AC File Offset: 0x000032AC
		public StepperComponent() : base("Stepper", "Stepper", "Optimization component featuring Radical and Stepper", "DSE", "Optimize")
		{
			this.Objectives = new List<double>();
			this.NumVariables = new List<double>();
			this.ObjectiveHistory = new DataTree<double>();
			this.VariableHistory = new DataTree<double>();
			this.GradientHistory = new DataTree<double?>();
			this.open = false;
			this.InputsSatisfied = false;
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x00005124 File Offset: 0x00003324
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x0000513C File Offset: 0x0000333C
		public bool IsWindowOpen
		{
			get
			{
				return this.open;
			}
			set
			{
				this.open = value;
			}
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00005146 File Offset: 0x00003346
		public override void CreateAttributes()
		{
			this.m_attributes = new StepperComponentAttributes(this);
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00005155 File Offset: 0x00003355
		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			// stepper .gha has been decompiled using dnSpy so enum are not destructured propely -> add explicit cast
			pManager.AddNumberParameter("Objective", "O", "Objective to Minimize", (GH_ParamAccess)1);
			pManager.AddNumberParameter("Numerical Variables", "numVar", "Numerical Optimization Variables", (GH_ParamAccess)1);
			pManager[1].Optional = true;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00005194 File Offset: 0x00003394
		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			pManager.AddNumberParameter("Objective Evolution", "Obj", "Objective value history", (GH_ParamAccess)2);

			pManager.AddNumberParameter("Variable Evolution", "Var", "Variable value history", (GH_ParamAccess)2);
			pManager.AddNumberParameter("Gradient Evolution", "Grad", "Gradient value history", (GH_ParamAccess)2);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x000051E8 File Offset: 0x000033E8
		protected override void SolveInstance(IGH_DataAccess DA)
		{
			List<double> list = new List<double>();
			bool flag = !DA.GetDataList<double>(0, list);
			if (flag)
			{
				this.InputsSatisfied = false;
			}
			else
			{
				this.Objectives = list;
				bool flag2 = base.Params.Input[0].Sources.Count > 0 && this.Objectives.Count == 0;
				if (flag2)
				{
					this.InputsSatisfied = false;
					this.AddRuntimeMessage((GH_RuntimeMessageLevel)10, "One or more objectives are invalid");
				}
				else
				{
					List<double> list2 = new List<double>();
					DA.GetDataList<double>(1, list2);
					this.NumVariables = list2;
					foreach (IGH_Param igh_Param in base.Params.Input[1].Sources)
					{
						bool flag3 = igh_Param.Name != "Number Slider";
						if (flag3)
						{
							this.InputsSatisfied = false;
							this.AddRuntimeMessage((GH_RuntimeMessageLevel)10, "One or more NumVars are invalid");
							return;
						}
					}
					this.InputsSatisfied = true;
					DA.SetDataTree(0, this.ObjectiveHistory);
					DA.SetDataTree(1, this.VariableHistory);
					DA.SetDataTree(2, this.GradientHistory);
				}
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000533C File Offset: 0x0000353C
		public void AppendToObjectives(List<double> values)
		{
			int num = 0;
			foreach (double num2 in values)
			{
				GH_Path gh_Path = new GH_Path(num);
				this.ObjectiveHistory.Add(num2, gh_Path);
				num++;
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x000053A4 File Offset: 0x000035A4
		public void AppendToVariables(List<double> values)
		{
			int num = 0;
			foreach (double num2 in values)
			{
				GH_Path gh_Path = new GH_Path(num);
				this.VariableHistory.Add(num2, gh_Path);
				num++;
			}
		}

		// Token: 0x060000DA RID: 218 RVA: 0x0000540C File Offset: 0x0000360C
		public void AppendToGradients(List<List<double>> values)
		{
			int num = 0;
			foreach (double num2 in this.Objectives)
			{
				for (int i = 0; i < this.numVars; i++)
				{
					GH_Path gh_Path = new GH_Path(new int[]
					{
						num,
						i
					});
					bool flag = values.Any<List<double>>();
					if (flag)
					{
						this.GradientHistory.Add(new double?(values[num][i]), gh_Path);
					}
					else
					{
						this.GradientHistory.Add(null, gh_Path);
					}
				}
				num++;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000DB RID: 219 RVA: 0x000054E0 File Offset: 0x000036E0
		protected override Bitmap Icon
		{
			get
			{
				return Resources.Stepper;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000DC RID: 220 RVA: 0x000054F8 File Offset: 0x000036F8
		public override Guid ComponentGuid
		{
			get
			{
				return new Guid("17e61555-19a4-4cd2-856d-7964da95f3df");
			}
		}

		// Token: 0x04000054 RID: 84
		private DataTree<double> ObjectiveHistory;

		// Token: 0x04000055 RID: 85
		private DataTree<double> VariableHistory;

		// Token: 0x04000056 RID: 86
		private DataTree<double?> GradientHistory;

		// Token: 0x04000057 RID: 87
		public int numVars;

		// Token: 0x04000058 RID: 88
		private bool open;
	}
}
