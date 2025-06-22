using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace Stepper
{
    // Token: 0x0200000B RID: 11
    public class SliderVariable : IVariable
    {
        // Token: 0x06000056 RID: 86 RVA: 0x00002A3D File Offset: 0x00000C3D
        public SliderVariable(IGH_Param param)
        {
            this.param = param;
            this.Slider = (this.Parameter as GH_NumberSlider);
            this._dir = SliderVariable.Direction.None;
        }

        // Token: 0x17000022 RID: 34
        // (get) Token: 0x06000057 RID: 87 RVA: 0x00002A66 File Offset: 0x00000C66
        // (set) Token: 0x06000058 RID: 88 RVA: 0x00002A6E File Offset: 0x00000C6E
        public bool IsActive { get; set; }

        // Token: 0x17000023 RID: 35
        // (get) Token: 0x06000059 RID: 89 RVA: 0x00002A78 File Offset: 0x00000C78
        // (set) Token: 0x0600005A RID: 90 RVA: 0x00002A90 File Offset: 0x00000C90
        public int Dir
        {
            get
            {
                return (int)this._dir;
            }
            set
            {
                this._dir = (SliderVariable.Direction)value;
            }
        }

        // Token: 0x17000024 RID: 36
        // (get) Token: 0x0600005B RID: 91 RVA: 0x00002A9C File Offset: 0x00000C9C
        // (set) Token: 0x0600005C RID: 92 RVA: 0x00002AC4 File Offset: 0x00000CC4
        public double Min
        {
            get
            {
                return (double)this.Slider.Slider.Minimum;
            }
            set
            {
                this.Slider.Slider.Minimum = (decimal)value;
            }
        }

        // Token: 0x17000025 RID: 37
        // (get) Token: 0x0600005D RID: 93 RVA: 0x00002AE0 File Offset: 0x00000CE0
        // (set) Token: 0x0600005E RID: 94 RVA: 0x00002B08 File Offset: 0x00000D08
        public double Max
        {
            get
            {
                return (double)this.Slider.Slider.Maximum;
            }
            set
            {
                this.Slider.Slider.Maximum = (decimal)value;
            }
        }

        // Token: 0x17000026 RID: 38
        // (get) Token: 0x0600005F RID: 95 RVA: 0x00002B22 File Offset: 0x00000D22
        // (set) Token: 0x06000060 RID: 96 RVA: 0x00002B2A File Offset: 0x00000D2A
        public double ReferenceValue { get; set; }

        // Token: 0x17000027 RID: 39
        // (get) Token: 0x06000061 RID: 97 RVA: 0x00002B34 File Offset: 0x00000D34
        // (set) Token: 0x06000062 RID: 98 RVA: 0x00002B57 File Offset: 0x00000D57
        public double CurrentValue
        {
            get
            {
                return (double)this.Slider.CurrentValue;
            }
            set
            {
                this.UpdateValue(value);
            }
        }

        // Token: 0x17000028 RID: 40
        // (get) Token: 0x06000063 RID: 99 RVA: 0x00002B64 File Offset: 0x00000D64
        public IGH_Param Parameter
        {
            get
            {
                return this.param;
            }
        }

        // Token: 0x06000064 RID: 100 RVA: 0x00002B7C File Offset: 0x00000D7C
        public void UpdateValue(double x)
        {
            try
            {
                this.Slider.SetSliderValue((decimal)x);
            }
            catch
            {
            }
        }

        // Token: 0x06000065 RID: 101 RVA: 0x00002BB8 File Offset: 0x00000DB8
        public double Gradient()
        {
            throw new NotImplementedException();
        }

        // Token: 0x04000017 RID: 23
        public GH_NumberSlider Slider;

        // Token: 0x04000019 RID: 25
        private SliderVariable.Direction _dir;

        // Token: 0x0400001B RID: 27
        private IGH_Param param;

        // Token: 0x02000029 RID: 41
        public enum Direction
        {
            // Token: 0x040000D3 RID: 211
            X,
            // Token: 0x040000D4 RID: 212
            Y,
            // Token: 0x040000D5 RID: 213
            Z,
            // Token: 0x040000D6 RID: 214
            None
        }
    }
}
