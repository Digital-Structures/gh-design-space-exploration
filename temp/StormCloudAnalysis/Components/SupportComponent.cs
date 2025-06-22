using Grasshopper.Kernel;
using Rhino.Geometry;
using StructureEngine.Model;
using System;
using System.Collections.Generic;

namespace StormCloud.Components
{
    public class SupportComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SupportComponent class.
        /// </summary>
        public SupportComponent(): base("Support", "S","Creates Support","StormCloud", "Analysis")
        {
        }
        // include buttons for choice

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Structure to Load
            pManager.AddParameter(new Parameters.StructureParameter());
            // Loaded Nodes
            pManager.AddPointParameter("Support Point", "Pt", "Support Point", GH_ParamAccess.list);
            // Temporary Limited Support Options
            pManager.AddBooleanParameter("Translation", "T", "True if translational movement prevented", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Rotation", "R", "True if rotational movement prevented", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.StructureParameter());
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // input
            StructureEngine.Model.ComputedStructure structure = new StructureEngine.Model.ComputedStructure(); // modify to get in solver to get output structure
            List<Point3d> points = new List<Point3d>();
            Boolean translation = true;
            Boolean rotation = true;

            Types.StructureType structuretype = new Types.StructureType();

            if (!DA.GetData(0, ref structuretype)) { return; }
            if (!DA.GetDataList(1, points)) { return; }
            if (!DA.GetData(2, ref translation)) { return; }
            if (!DA.GetData(3, ref rotation)) { return; }

            structuretype.CastTo<ComputedStructure>(ref structure);

            structure = structuretype.Value;

            foreach (Point3d point in points)
            {
                foreach (StructureEngine.Model.Node node in structure.Nodes)
                {
                    if (node.IsAtPoint(point))
                    {
                        node.DOFs[0].Fixed = translation;
                        node.DOFs[1].Fixed = translation;
                        node.DOFs[2].Fixed = translation;
                    }
                }
            }
                    //points.Remove(point); collection was modified each time -> error





            foreach (Point3d point in points)
            {
                foreach (StructureEngine.Model.Node node in structure.Nodes)
                {
                    if (node.IsAtPoint(point))
                    {
                        node.DOFs[3].Fixed = rotation;
                        node.DOFs[4].Fixed = rotation;
                        node.DOFs[5].Fixed = rotation;
                    }
                }
                //points.Remove(point); collection was modified each time -> error
            }

            /*Assign the outputs via the DA object*/
            Types.StructureType Structure_GH = new Types.StructureType(structure);
            DA.SetData(0, Structure_GH);
            //DA.SetData(1, info);

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
            get { return new Guid("{99c200e4-c426-4c25-93ae-614d52acb798}"); }
        }
    }
}