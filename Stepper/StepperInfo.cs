using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Stepper
{
	// Token: 0x02000017 RID: 23
	public class StepperInfo : GH_AssemblyInfo
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00004114 File Offset: 0x00002314
		public override string Name
		{
			get
			{
				return "Stepper";
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000BA RID: 186 RVA: 0x0000412C File Offset: 0x0000232C
		public override Bitmap Icon
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000BB RID: 187 RVA: 0x00004140 File Offset: 0x00002340
		public override string Description
		{
			get
			{
				return "";
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00004158 File Offset: 0x00002358
		public override Guid Id
		{
			get
			{
				return new Guid("05e5ad17-b9bc-445d-a0b8-d7267490c4a9");
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000BD RID: 189 RVA: 0x00004174 File Offset: 0x00002374
		public override string AuthorName
		{
			get
			{
				return "";
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000BE RID: 190 RVA: 0x0000418C File Offset: 0x0000238C
		public override string AuthorContact
		{
			get
			{
				return "";
			}
		}
	}
}
