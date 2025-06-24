using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StructureEngine.Model;
using StormCloud;

namespace StormCloud.Components
{
    public class MaterialComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MaterialComponent class.
        /// </summary>
        public MaterialComponent(): base("Material", "Mat","Material Properties","StormCloud", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Young", "E", "Young's Modulus [GPa]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Density", "d", "Material Density [lkg/m3]", GH_ParamAccess.item);
            pManager.AddNumberParameter("StressAllow", "s", "Maximum Allowable Stress [MPa]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Poisson", "p", "Poisson Ratio [/]", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Material Name", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.MaterialParameter());
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double e = 0;
            double d = 0;
            double s = 0;
            double p = 0;
            string name = null;

            // Retrieve Data
            if (!DA.GetData(0, ref e)) { return; }
            if (!DA.GetData(1, ref d)) { return; }
            if (!DA.GetData(2, ref s)) { return; }
            if (!DA.GetData(3, ref p)) { return; }
            if (!DA.GetData(4, ref name)) { return; }

            // Material

            Material material = new Material(e, p, d, s, name);

            Types.MaterialType Material_GH = new Types.MaterialType(material);
            /*Assign the outputs via the DA object*/
            DA.SetData(0, Material_GH);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{3b379949-0f68-4693-ac2c-9f5ceeec2373}"); }
        }
    }
}