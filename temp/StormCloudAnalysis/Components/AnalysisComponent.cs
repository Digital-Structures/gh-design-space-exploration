using System;
using System.Collections.Generic;
using StructureEngine.Analysis;
using StructureEngine.Model;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;

namespace StormCloud.Components
{
    public class AnalysisComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AnalysisComponent class.
        /// </summary>
        public AnalysisComponent(): base("Analysis", "A","Analyzes and Sizes the Structure","StormCloud", "Analysis")
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
            pManager.AddParameter(new Parameters.StructureParameter());
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

            FrameAnalysis analysis = new FrameAnalysis();
            analysis.RunAnalysis(comp);
            
            Types.StructureType Structure_GHrep = new Types.StructureType(comp);
            /*Assign the outputs via the DA object*/
            DA.SetData(0, Structure_GHrep);
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
            get { return new Guid("{2a305dd4-ff74-4156-8e77-675148e77e04}"); }
        }
    }
}