using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using StormCloud;

namespace StormCloud.Parameters
{
    class MaterialParameter : GH_Param<Types.MaterialType> //set to public to be accessible in GH
    {
        public MaterialParameter() : base("Material", "Mat", "Represents a material", "StormCloud", "Analysis", GH_ParamAccess.item) { }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return base.Icon;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{C9C84ACB-4F91-4DC5-B6FD-D4748176507C}"); }
        }
    }
}