using System;
using System.Collections.Generic;

namespace Stepper
{
    // Token: 0x02000004 RID: 4
    public interface IOptimizeToolVM
    {
        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000011 RID: 17
        // (set) Token: 0x06000012 RID: 18
        StepperComponent Component { get; set; }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000013 RID: 19
        // (set) Token: 0x06000014 RID: 20
        List<GroupVarVM> GroupVars { get; set; }

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000015 RID: 21
        // (set) Token: 0x06000016 RID: 22
        List<VarVM> NumVars { get; set; }

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000017 RID: 23
        // (set) Token: 0x06000018 RID: 24
        List<List<VarVM>> GeoVars { get; set; }
    }
}
