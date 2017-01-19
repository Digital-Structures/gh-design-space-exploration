using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using DSECommon;


namespace Capture
{
    public class CaptureComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public CaptureComponent()
          : base("Capture", "Capture",
              "Description",
              "DSE", "Main")
        {
            this.VarsList = new List<DSEVariable>();
            this.ObjInput = new List<List<double>>();
            this.PropInput = new List<List<double>>();
            this.SlidersList = new List<GH_NumberSlider>();
            this.DesignMap = new List<List<double>>();
        }

        public bool Iterating;
        public List<List<double>> ObjValues;
        public List<DSEVariable> VarsList;
        public List<GH_NumberSlider> SlidersList;
        public List<List<double>> ObjInput;
        public List<List<double>> DesignMap;
        public enum CaptureMode {Screenshot, Evaluation, Both };
        public CaptureMode Mode;
        public List<List<double>> PropInput;
        public List<List<double>> PropertyValues;
        public string Dir;
        public string Filename;
        public int NumVars, NumObjs;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Check that number of sliders is equal to number of variables in DM; otherwise, throw an error.
            pManager.AddNumberParameter("Variables", "Vars", "Sliders representing variables", GH_ParamAccess.list);
            pManager.AddNumberParameter("Objectives", "Obj", "One or more performance objectives", GH_ParamAccess.list);
            pManager.AddNumberParameter("Mode", "M", "Sampling Type. Right click to choose type.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Properties", "P", "One or more numerical properties to record", GH_ParamAccess.list);
            pManager.AddNumberParameter("Design Map", "DM", "Set of design variable settings to capture", GH_ParamAccess.tree);
            pManager.AddTextParameter("Filename", "F", "Prefix for output files. Example: 'Design'", GH_ParamAccess.item);
            pManager.AddTextParameter("Directory", "Dir", @"Output path. Example: 'C:\Folder\'", GH_ParamAccess.item);

            // Add possible values for the mode input
            Param_Integer param = (Param_Integer)pManager[2];
            param.AddNamedValue("Evaluation [0]", 0);
            param.AddNamedValue("Screenshot [1]", 1);
            param.AddNamedValue("Both [2]", 2);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            this.readSlidersList();

            var obj = new DataTree<double>();
            if (!DA.GetData(1, ref obj)) return;
            this.ObjInput = TreeToListOfLists<double>(obj);

            if (!DA.GetData(2, ref Mode)) return;

            var prop = new DataTree<double>();
            if (!DA.GetData(3, ref prop)) return;
            this.PropInput = TreeToListOfLists<double>(prop);

            var map = new DataTree<double>();
            if (!DA.GetData(4, ref map)) return;
            this.DesignMap = TreeToListOfLists<double>(map);

            if (!DA.GetData(5, ref Filename)) return;
            if (!DA.GetData(6, ref Dir)) return;

            if (Iterating)
            {
                this.ObjValues = new List<List<double>>();
                this.PropertyValues = new List<List<double>>();
                List<double> o = new List<double>();
                List<double> p = new List<double>();
                if (!DA.GetData(1, ref o)) { return; }
                if (!DA.GetData(3, ref p)) { return; }

                ObjValues.Add(o);
                PropertyValues.Add(p);
            }

            if (!Iterating)
            {
                //DA.SetDataList(0, results);
            }
        }

        private void readSlidersList()
        {
            this.VarsList.Clear();
            this.SlidersList.Clear();
            this.SlidersList = this.Params.Input[0].Sources[0] as List<GH_NumberSlider>;

            foreach (GH_NumberSlider slider in SlidersList)
            {
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

        static List<List<T>> TreeToListOfLists<T>(DataTree<T> tree)
        {
            List<List<T>> list = new List<List<T>>();
            foreach (GH_Path p in tree.Paths)
            {
                List<T> l = tree.Branch(p);
                list.Add(l);
            }
            return list;
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
            get { return new Guid("{33db086d-a308-4678-893f-6ab54420dc62}"); }
        }
    }
}
