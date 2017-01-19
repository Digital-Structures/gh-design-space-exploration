using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;

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
          : base("Sampler", "Nickname",
              "Description",
              "DSE", "Main")
        {
            this.Util = new SamplerUtilities(this);
            this.MyRand = new Random();
            this.VarsList = new List<DSEVariable>();
            this.Util = new SamplerUtilities(this);
            this.Output = new List<List<double>>();
        }

        // Properties specific to this component:

        //private List<Grasshopper.Kernel.Special.GH_NumberSlider> SlidersList;
        public List<DSEVariable> VarsList;
        public string Prefix, Path;
        public int Scheme, NSamples, Seed;
        public SamplerUtilities Util;
        public List<List<double>> Output;
        //private IGH_DataAccess DAC;
        public Random MyRand;

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
            pManager.AddTextParameter("Prefix", "Pre", "Prefix for output file. Example: 'Design01'.", GH_ParamAccess.item);
            pManager.AddTextParameter("Path", "P", @"Output path. Example: 'C:\Folder\'", GH_ParamAccess.item);

            // TODO: Add feature to check whether user included \ at the end of path.  If yes, do nothing, if not, add \.

            Param_Integer param = (Param_Integer)pManager[2];
            param.AddNamedValue("Random [0]", 0);
            param.AddNamedValue("Grid [1]", 1);
            param.AddNamedValue("Latin Hypercube [2]", 2);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Output", "Out", "output data", GH_ParamAccess.tree);
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

            List<double> variables = new List<double>();
            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetDataList(0, variables)) return;
            if (!DA.GetData(1, ref NSamples)) return;
            if (!DA.GetData(2, ref Scheme)) return;
            if (!DA.GetData(3, ref Seed)) return;
            if (!DA.GetData(4, ref Prefix)) return;
            if (!DA.GetData(5, ref Path)) return;

            // We should now validate the data and warn the user if invalid data is supplied.
            
            DA.SetDataTree(0, ListOfListsToTree<double>(this.Output));
        }

        private void readSlidersList()
        {
            //this.SlidersList.Clear();
            this.VarsList.Clear();

            foreach (IGH_Param param in Params.Input[0].Sources)
            {
                GH_NumberSlider slider = param as Grasshopper.Kernel.Special.GH_NumberSlider;
                //this.SlidersList.Add(slider);

                DSEVariable newVar = new DSEVariable((double)slider.Slider.Minimum, (double)slider.Slider.Maximum, (double)slider.Slider.Value);
                this.VarsList.Add(newVar);
            }
        }

        static DataTree<T> ListOfListsToTree<T>(List<List<T>> listofLists)
        {
            DataTree<T> tree = new DataTree<T>();
            for (int i = 0; i < listofLists.Count; i++)
            {
                tree.AddRange(listofLists[i], new GH_Path(i));
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
                //return Resources.IconForThisComponent;
                return null;
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
