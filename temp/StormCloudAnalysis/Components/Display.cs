using System;
using System.Collections.Generic;
using StructureEngine.Model;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace StormCloud.Components
{
    public class Display : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Display class.
        /// </summary>
        public Display() : base("Display", "A", "Analyzes and Sizes the Structure", "StormCloud", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.StructureParameter());
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Lines", "L", "Lines", GH_ParamAccess.list);
            pManager.AddPointParameter("Points", "P", "Points", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            StructureEngine.Model.ComputedStructure comp = new StructureEngine.Model.ComputedStructure(); // modify to get in solver to get output structure

            Types.StructureType structure1 = new Types.StructureType();

            if (!DA.GetData(0, ref structure1)) { return; }

            structure1.CastTo<ComputedStructure>(ref comp);

            comp = structure1.Value;

            List<Point3d> Points = new List<Point3d>();
            List<Line> Lines = new List<Line>();

            foreach (Member m in comp.Members)
            {
                Point3d start = m.NodeI.ToRhinoPoint();
                Point3d end = m.NodeJ.ToRhinoPoint();
                //Points.Add(start);
                //Points.Add(end);
                Lines.Add(new Line(start, end));
            }

            foreach (Node n in comp.Nodes)
            {
                Point3d pt = n.ToRhinoPoint();
                Points.Add(pt);
            }

            DA.SetDataList(0, Lines);
            DA.SetDataList(1, Points);


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
            get { return new Guid("{268984e1-3662-470c-8e8b-a339f696ffb0}"); }
        }
    }
}