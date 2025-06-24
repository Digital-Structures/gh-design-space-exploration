using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using StructureEngine.Model;

namespace StormCloud.Types
{
    public class StructureType : GH_Goo<ComputedStructure>
    {
        // Default Constructor
        public StructureType()
        {
            this.Value = new ComputedStructure();
        }

        // Constructor with Initial Value

        public StructureType(ComputedStructure structure)
        {
            this.Value = structure;
        }

        public override string TypeName
        {
            get { return "Structure"; }
        }

        public override string TypeDescription
        {
            get { return "Describes an assembled structure"; }
        }

        public override IGH_Goo Duplicate()
        {
            // not sure it is ok!!!
            StructureType Copy = new StructureType(this.Value);
            return Copy;
        }

        public override bool IsValid
        {
            get { return true; }
        }

        public override string ToString()
        {
            return "Structure";
        }
    }
}
