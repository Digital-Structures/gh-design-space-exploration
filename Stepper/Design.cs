using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;

namespace Stepper
{
	// Token: 0x02000003 RID: 3
	public class Design
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000007 RID: 7 RVA: 0x000020F2 File Offset: 0x000002F2
		// (set) Token: 0x06000008 RID: 8 RVA: 0x000020FA File Offset: 0x000002FA
		public StepperComponent MyComponent { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002103 File Offset: 0x00000303
		// (set) Token: 0x0600000A RID: 10 RVA: 0x0000210B File Offset: 0x0000030B
		public List<IVariable> Variables { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002114 File Offset: 0x00000314
		// (set) Token: 0x0600000C RID: 12 RVA: 0x0000211C File Offset: 0x0000031C
		public List<IDesignGeometry> Geometries { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000D RID: 13 RVA: 0x00002128 File Offset: 0x00000328
		public List<double> Objectives
		{
			get
			{
				return this.MyComponent.Objectives;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002148 File Offset: 0x00000348
		public List<IVariable> ActiveVariables
		{
			get
			{
				return (from var in this.Variables
				where var.IsActive
				select var).ToList<IVariable>();
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000218C File Offset: 0x0000038C
		public Design(StepperComponent component)
		{
			this.MyComponent = component;
			this.Variables = new List<IVariable>();
			this.Geometries = new List<IDesignGeometry>();
			foreach (IGH_Param param in this.MyComponent.Params.Input[1].Sources)
			{
				SliderVariable sliderVariable = new SliderVariable(param);
				bool flag = sliderVariable.CurrentValue == 0.0;
				if (flag)
				{
					bool flag2 = sliderVariable.Max >= 0.001;
					if (flag2)
					{
						sliderVariable.UpdateValue(0.001);
					}
					else
					{
						sliderVariable.UpdateValue(-0.001);
					}
				}
				this.Variables.Add(new SliderVariable(param));
			}
			Instances.ActiveCanvas.Document.NewSolution(false, (Grasshopper.Kernel.GH_SolutionMode)2);
			bool flag3 = this.Geometries.Any<IDesignGeometry>();
			if (flag3)
			{
				this.Variables.AddRange((from x in this.Geometries
				select x.Variables).SelectMany((List<GeoVariable> x) => x).ToList<GeoVariable>());
			}
			this.MyComponent.numVars = (from var in this.Variables
			where var.IsActive
			select var).Count<IVariable>();
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002344 File Offset: 0x00000544
		public void UpdateComponentOutputs(List<List<double>> GradientData)
		{
			this.MyComponent.AppendToObjectives(this.Objectives);
			this.MyComponent.AppendToVariables((from var in this.Variables
			select var.CurrentValue).ToList<double>());
			this.MyComponent.AppendToGradients(GradientData);
		}
	}
}
