using System;

namespace Stepper
{
    // Token: 0x0200000C RID: 12
    public class SurfaceVariable : GeoVariable
    {
        // Token: 0x06000066 RID: 102 RVA: 0x00002BC0 File Offset: 0x00000DC0
        public SurfaceVariable(double min, double max, int u, int v, int dir, DesignSurface surf) : base(min, max, dir, surf)
        {
            this.u = u;
            this.v = v;
            base.PointName = string.Format(".u{0}v{1}.{2}", this.u + 1, this.v + 1, ((GeoVariable.Direction)base.Dir).ToString());
        }

        // Token: 0x0400001C RID: 28
        public int u;

        // Token: 0x0400001D RID: 29
        public int v;
    }
}
