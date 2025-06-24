using System;
using System.Collections.Generic;
using StructureEngine.Model;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace StormCloud.Types
{
    public class MaterialType : GH_Goo<Material>
    {
        // Default Constructor
        public MaterialType()
        {
            Material mat = new Material(29000,0.3,7500,50,"steel");
            this.Value = mat;
        }

        public MaterialType(double e, double d, double s, double p, string n)
        {
            this.Value = new Material(e, d, s, p, n);
        }

        // Constructor with Initial Value

        public MaterialType(Material material)
        {
            this.Value = material;
        }

        public Material MaterialValue
        {
            get;
            set;
        }
        public override IGH_Goo Duplicate()
        {
            // not sure it is ok!!!
            MaterialType Copy = new MaterialType(this.Value);
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
    