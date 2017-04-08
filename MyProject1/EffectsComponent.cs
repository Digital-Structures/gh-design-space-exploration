using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DSECommon;

using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;


namespace Effects
{
    public class EffectsComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public EffectsComponent()
          : base("Effects", "Effects",
              "Calculates the average effect of each variable on performance ",
              "DSE", "Simplify")
        {

            this.VarsList = new List<DSEVariable>();
            this.VarsVals = new List<double>();
            this.MinVals = new List<double>();
            this.MaxVals = new List<double>();
            this.SlidersList = new List<GH_NumberSlider>();

            this.ObjInput = new List<double>();
            this.LevelSettings = new List<double>();

        }

        // Properties specific to this component:
        public List<DSEVariable> VarsList;
        public List<double> ObjInput;
        public List<double> LevelSettings;
        public List<double> MinVals;
        public List<double> MaxVals;
        public List<double> VarsVals;
        public List<List<double>> DesignMap;
        public List<List<double>> DesignMapNoObj;
        public int numVars;
        public int numObjs;
        public int numLevels;
        public List<GH_NumberSlider> SlidersList;
        public bool EffectsDone;
        public int index;
        public List<double> newVars;
        public bool live;
        bool indexShifted;
        public double flexibility;
        public bool propCalculated;

        public override void CreateAttributes()
        {
            base.m_attributes = new EffectsComponentAttributes(this);
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddNumberParameter("Variables", "Var", "Variables to be sampled.  Must be less than 13.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Objectives", "Obj", "One or more performance objectives", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Levels", "Lev", "Number of levels.  Must be 2 or 3.", GH_ParamAccess.item, 2);
            pManager.AddNumberParameter("Level Settings", "LevSet", "Variable settings of the levels being considered", GH_ParamAccess.list);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("Average Effects", "AvgEff", "The magnitude of the average effects for each variable", GH_ParamAccess.tree);
            pManager.AddTextParameter("Raw Effects", "RawEff", "Raw effects of each variable setting", GH_ParamAccess.tree);


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
            if (!DA.GetDataList(0, variables)) return;

            numVars = variables.Count;


            if (!DA.GetDataList<double>(1, this.ObjInput)) return;

            if (!DA.GetData(2, ref this.numLevels)) return;

            if (!DA.GetDataList<double>(3, this.LevelSettings)) return;

            




            if (EffectsDone)

            {

                DA.SetDataTree(0, ListOfListsToTree<int>(((EffectsComponentAttributes)this.m_attributes).EffectIndicesList));
                DA.SetDataTree(1, ListOfListsToTree<int>(((EffectsComponentAttributes)this.m_attributes).EffectIndicesList));


            }


        }


        static List<List<double>> StructureToListOfLists(GH_Structure<GH_Number> structure)
        {
            List<List<double>> list = new List<List<double>>();
            foreach (GH_Path p in structure.Paths)
            {
                List<GH_Number> l = (List<GH_Number>)structure.get_Branch(p);
                List<double> doubles = new List<double>();
                foreach (GH_Number n in l)
                {
                    double d = 0;
                    n.CastTo<double>(out d);
                    doubles.Add(d);
                }
                list.Add(doubles);
            }
            return list;
        }

        private void readSlidersList()
        {
            this.VarsList.Clear();
            this.SlidersList = new List<GH_NumberSlider>();
            int nVars = this.Params.Input[0].Sources.Count;
            for (int i = 0; i < nVars; i++)
            {
                this.SlidersList.Add(this.Params.Input[0].Sources[i] as GH_NumberSlider);
            }

            foreach (GH_NumberSlider slider in SlidersList)
            {
                DSEVariable newVar = new DSEVariable((double)slider.Slider.Minimum, (double)slider.Slider.Maximum, (double)slider.Slider.Value);
                this.VarsList.Add(newVar);

                VarsVals.Add((double)slider.Slider.Value);
                MinVals.Add((double)slider.Slider.Minimum);
                MaxVals.Add((double)slider.Slider.Maximum);

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
            get { return new Guid("{19aec125-e640-499b-94d2-be330e05801a}"); }
        }
    }
}
