using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GrasshopperImplementation;

namespace CustomComponents
{
    public class StructureParameter : GH_Param<CustomTypes.StructureType>
    {
        public StructureParameter():
            base("Structure","Struct","Represents a structure","Params","Primitives") { }
        public override Guid ComponentGuid
        {
            get { return new Guid("{F953177D-354F-48D8-9D69-C3E4A9E8F443}"); }
        }
    }
}
