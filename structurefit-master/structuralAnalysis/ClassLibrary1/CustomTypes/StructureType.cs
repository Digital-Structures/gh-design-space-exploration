using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using StructureEngine.Model;

namespace CustomTypes
{
    public class StructureType : GH_Goo<Structure>
    {
        // Default Constructor
        public StructureType()
        {
            this.Value = new Structure();
        }

        // Constructor with Initial Value

        public StructureType(Structure strucValue)
        {
            this.Value = strucValue;
        }

        public override string TypeName
        {
            get { return "Structure"; }
        }

        public override string TypeDescription
        {
            get { return "Describes an assembled structure" }
        }
    }
}
