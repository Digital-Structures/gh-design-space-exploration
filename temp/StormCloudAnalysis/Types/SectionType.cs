using System;
using System.Collections.Generic;
using StructureEngine.Model;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace StormCloudAnalysis.Types
{
    public class SectionType : GH_Goo<ISection>
    {
        // Default Constructor
        public SectionType()
        {

            this.Value = null;
        }

        // Constructor with Initial Value

        public SectionType(ISection Section)
        {
            this.Value = Section;
        }

        public SectionType SectionValue
        {
            get;
            set;
        }

        public override IGH_Goo Duplicate()
        {
            // not sure it is ok!!!
            SectionType Copy = new SectionType(this.Value);
            return Copy;
        }

        public override bool IsValid
        {
            // when is it not valid???
            get { return true; }
        }

        public override string ToString()
        {
            return "Material";
        }

        public override string TypeName
        {
            get { return "Material"; }
        }

        public override string TypeDescription
        {
            get { return "Describes a material"; }
        }
    }
}
