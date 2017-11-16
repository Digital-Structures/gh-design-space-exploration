using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using DSECommon;

namespace Sampler
{
    public class SamplerComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public SamplerComponent()
          : base("Sampler", "Sampler",
              "Generates a list of parametric design vectors, called a “Design Map”, based on user-defined variable properties. WORKS ON DOUBLECLICK",
              "DSE", "Catalog")
        {
            this.Util = new SamplerUtilities(this);
            this.MyRand = new Random();
            this.VarsList = new List<DSEVariable>();
            this.Util = new SamplerUtilities(this);
            this.Output = new List<List<double>>();
        }

        // Properties specific to this component:
        public List<DSEVariable> VarsList;
        public string Filename, Dir;
        public int Scheme, NSamples, Seed;
        public SamplerUtilities Util;
        public List<List<double>> Output;
        public Random MyRand;
        public string FilesWritten;

        /// <summary>
        /// Creates custom attributes for this component.
        /// </summary>
        public override void CreateAttributes()
        {
            base.m_attributes = new SamplerComponentAttributes(this);
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Variables", "Var", "Variables to be sampled", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Samples", "N", "Number of samples", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Type", "T", "Sampling Type. Right click to choose type.", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("Seed", "S", "Random Seed. Integer 0 will leave the seed unspecified.", GH_ParamAccess.item);
            if (this.Seed != 0) { this.MyRand = new Random(this.Seed); }
            pManager.AddTextParameter("Filename", "F", "Filename for output .csv file. Example: 'Samples'", GH_ParamAccess.item);
            pManager.AddTextParameter("Directory", "Dir", @"Output directory for .csv file. Example: 'C:\Folder or C:\Folder\", GH_ParamAccess.item, "None");

            // TODO: Add feature to check whether user included \ at the end of path.  If yes, do nothing, if not, add \.
            // or change to just path with filename

            Param_Integer param = (Param_Integer)pManager[2];
            param.AddNamedValue("Random [0]", 0);
            param.AddNamedValue("Grid [1]", 1);
            param.AddNamedValue("Latin Hypercube [2]", 2);

            pManager[4].Optional = true;
            pManager[5].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Design Map", "DM", "Set of design variables", GH_ParamAccess.tree);
            pManager.AddTextParameter("Data", "CSV?", "Data written?", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            readSlidersList();
            // First, we need to retrieve all data from the input parameters.
            // We'll start by declaring variables and assigning them starting values.

            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from required parameters, abort this method 
            if (!DA.GetData(1, ref NSamples)) return;
            if (!DA.GetData(2, ref Scheme)) return;
            if (!DA.GetData(3, ref Seed)) return;
            // if (!DA.GetData(4, ref Filename)) return;
            // if (!DA.GetData(5, ref Dir)) return;
            DA.GetData(4, ref Filename);
            DA.GetData(5, ref Dir);

            // Make sure there is backslash on directory
            char last = Dir[Dir.Length - 1];

            if (!last.Equals('\\') && Dir != "None")
            {
                Dir = @Dir + @"\";
            }


            // We should now validate the data and warn the user if invalid data is supplied.

            DA.SetDataTree(0, GHUtilities.ListOfListsToTree<double>(this.Output));

            DA.SetData(1, FilesWritten);

        }

        private void readSlidersList()
        {
            this.VarsList.Clear();

            foreach (IGH_Param param in Params.Input[0].Sources)
            {
                GH_NumberSlider slider = param as Grasshopper.Kernel.Special.GH_NumberSlider;

                DSEVariable newVar = new DSEVariable((double)slider.Slider.Minimum, (double)slider.Slider.Maximum, (double)slider.Slider.Value);
                this.VarsList.Add(newVar);
            }
        }

        static DataTree<T> ListToTree<T>(List<T> List)
        {
            DataTree<T> tree = new DataTree<T>();
            for (int i = 0; i < List.Count; i++)
            {
                tree.Add(List[i]);
            }
            return tree;
        }


        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                return Sampler.Properties.Resources.Sampler1;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{3fb887e3-6af7-4c40-9e49-a2cc46a88cb9}"); }
        }
    }
}
