using System;

namespace Stepper
{
    // Token: 0x02000005 RID: 5
    public class CurveVariable : GeoVariable
    {
        // Token: 0x06000019 RID: 25 RVA: 0x000023AC File Offset: 0x000005AC
        public CurveVariable(double min, double max, int u, int dir, DesignCurve crv) : base(min, max, dir, crv)
        {
            this.u = u;
            base.PointName = string.Format(".u{0}.{1}", this.u + 1, ((GeoVariable.Direction)base.Dir).ToString());
        }

        // Token: 0x04000004 RID: 4
        public int u;
    }
}
