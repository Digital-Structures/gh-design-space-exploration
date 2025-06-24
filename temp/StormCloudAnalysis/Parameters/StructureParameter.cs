using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using StormCloud;

namespace StormCloud.Parameters
{
    public class StructureParameter : GH_Param<Types.StructureType> // Removed IGH_Param
    {
        public StructureParameter() : base("Structure", "Struct", "Represents a structure", "StormCloud", "Analysis", GH_ParamAccess.item) { }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return base.Icon;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{E1CD5876-F2E3-4E34-90CA-689D8C92C609}"); }
        }
    }
}
