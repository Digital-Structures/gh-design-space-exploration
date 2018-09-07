using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
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
              "A general iterator that automatically generates many different design options and records an image, performance score, and/or other properties of each design. WORKS ON DOUBLECLICK",
              "DSE", "Catalog")
        {
            this.VarsList = new List<DSEVariable>();
            this.ObjInput = new List<double>();
            this.PropInput = new List<double>();
            this.SlidersList = new List<GH_NumberSlider>();
            this.DesignMap = new List<List<double>>();
            this.ObjValues = new List<List<double>>();   
            this.PropertyValues = new List<List<double>>();
            this.FirstRead = true;
        }

        public bool Iterating;
        public bool FirstRead;
        public List<List<double>> ObjValues;
        public List<DSEVariable> VarsList;
        public List<GH_NumberSlider> SlidersList;
        public List<double> ObjInput;
        public List<List<double>> DesignMap;
        public enum CaptureMode {SaveCSV, SaveScreenshot, Both, Neither };
        public CaptureMode Mode;
        public List<double> PropInput;
        public List<List<double>> PropertyValues;
        public string SSDir;
        public string SSFilename;
        public string CSVDir;
        public string CSVFilename;
        public int NumVars, NumObjs;
        public string DataWritten;
        public string ImagesWritten;
        public int Index;

        public override void CreateAttributes()
        {
            base.m_attributes = new CaptureComponentAttributes(this);
        }



        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Check that number of sliders is equal to number of variables in DM; otherwise, throw an error.
            pManager.AddNumberParameter("Variables", "Var", "Sliders representing variables", GH_ParamAccess.list);
            pManager.AddNumberParameter("Objectives", "Obj", "One or more performance objectives", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Mode", "M", "Sampling Type. Right click to choose type.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Properties", "Prop", "One or more numerical properties to record", GH_ParamAccess.list, 0.0);
            pManager.AddNumberParameter("Design map", "DM", "Set of design variable settings to capture", GH_ParamAccess.tree);
            pManager.AddTextParameter(".csv filename", ".csv F", "Prefix for output files. Example: 'all-data'", GH_ParamAccess.item);
            pManager.AddTextParameter(".csv directory", ".csv Dir", @"Output path. Example: 'C:\Folder or C:\Folder\", GH_ParamAccess.item, "None");
            pManager.AddTextParameter("Screenshot filename", "SS F", "Prefix for output files. Example: 'design'", GH_ParamAccess.item);
            pManager.AddTextParameter("Screenshot directory", "SS Dir", @"Output path. Example: 'C:\Folder or C:\Folder\", GH_ParamAccess.item, "None");

            // Add possible values for the mode input
            Param_Integer param = (Param_Integer)pManager[2];
            param.AddNamedValue("Save .csv [0]", 0);
            param.AddNamedValue("Save screenshot [1]", 1);
            param.AddNamedValue("Save both [2]", 2);
            param.AddNamedValue("Save neither [3]", 3);

            pManager[1].Optional = true;
            pManager[3].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Design Map + Objectives", "DM+O", "Set of design variables plus recorded objective(s)", GH_ParamAccess.tree);
            pManager.AddTextParameter("Captured Properties", "Prop", "Additional numerical properties (not objectives) recorded during capture", GH_ParamAccess.tree);
            pManager.AddTextParameter("Data", "CSV?", "Data written?", GH_ParamAccess.item);
            pManager.AddTextParameter("Images", "PNGs?", "Images written?", GH_ParamAccess.item);
            pManager.AddTextParameter("I", "Index", "Index of design for external use", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            this.readSlidersList();

            DA.GetDataList<double>(1, this.ObjInput);
            int mode = 0;
            if (!DA.GetData(2, ref mode)) return;
            this.Mode = (CaptureMode)mode;
            //if (!DA.GetDataList<double>(3, this.PropInput)) return;

            DA.GetDataList(3, this.PropInput);

            var map = new GH_Structure<GH_Number>();
            if (!DA.GetDataTree(4, out map)) return;
            this.DesignMap = StructureToListOfLists(map);
            if (FirstRead)
            {
                this.populateObjOutput();
                FirstRead = false;
            }

            // Activate if file writing should be required; currently, possible to capture without directory
            //if (!DA.GetData(5, ref CSVFilename)) return;
            //if (!DA.GetData(6, ref CSVDir)) return;
            //if (!DA.GetData(7, ref SSFilename)) return;
            //if (!DA.GetData(8, ref SSDir)) return;

            DA.GetData(5, ref CSVFilename);
            DA.GetData(6, ref CSVDir);
            DA.GetData(7, ref SSFilename);
            DA.GetData(8, ref SSDir);


            // Make sure directories can be accepted with or without slashes

            // Make sure there is backslash on directory
            char lastc = CSVDir[CSVDir.Length - 1];
            char lasts = SSDir[SSDir.Length - 1];

            if (!lastc.Equals('\\') && CSVDir != "None")
            {
                CSVDir = @CSVDir + @"\";
            }

            if (!lasts.Equals('\\') && SSDir != "None")
            {
                SSDir = @SSDir + @"\";
            }

            // Give warning messages for illegal inputs
            if (Mode == CaptureComponent.CaptureMode.SaveScreenshot || Mode == CaptureComponent.CaptureMode.Both)
            {

                if (SSDir == "None")
                {
                    this.AddRuntimeMessage((GH_RuntimeMessageLevel)20, "No screenshot directory given! Please add valid directory");
                }
            }

            if (Mode == CaptureComponent.CaptureMode.SaveCSV || Mode == CaptureComponent.CaptureMode.Both)
            {

                if (CSVDir == "None")
                {
                    this.AddRuntimeMessage((GH_RuntimeMessageLevel)20, "No CSV directory given! Please add valid directory");
                }
            }
        
            if (NumVars != this.DesignMap[0].Count)
            {
                    this.AddRuntimeMessage((GH_RuntimeMessageLevel)20, "Number of sliders and design map do not match; please check");
            }
            




            if (Iterating)
            {
                List<double> o = new List<double>();
                List<double> p = new List<double>();
                if (!DA.GetDataList(1, o)) { return; }
                if (!DA.GetDataList(3, p)) { return; }

                for (int i = 0; i < ObjValues.Count; i++)
                {
                    if (ObjValues[i].Count == 0)
                    {
                        ObjValues[i].AddRange(o);
                        break;
                    }
                }
                PropertyValues.Add(p);

                DA.SetData(4, Index);
            }

            if (!Iterating)
            {
                DA.SetDataTree(0, AssembleDMOTree(this.DesignMap, this.ObjValues));
                DA.SetDataTree(1, ListOfListsToTree<double>(this.PropertyValues));
                DA.SetData(2, DataWritten);
                DA.SetData(3, ImagesWritten);
            }
        }

        private DataTree<double> AssembleDMOTree(List<List<double>> vars, List<List<double>> obj)
        {
            return ListOfListsToTree<double>(AssembleDMO(vars, obj));
        }

        public List<List<double>> AssembleDMO(List<List<double>> vars, List<List<double>> obj)
        {
            List<List<double>> varsCopy = new List<List<double>>();
            foreach (List<double> list in vars)
            {
                List<double> newList = new List<double>();
                newList.AddRange(list);
                varsCopy.Add(newList);
            }

            int nSamples = vars.Count;
            for (int i = 0; i < nSamples; i++)
            {
                List<double> l = varsCopy[i];
                l.AddRange(obj[i]);
            }

            return varsCopy;
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

        private void populateObjOutput()
        {
            foreach (List<double> l in this.DesignMap)
            {
                this.ObjValues.Add(new List<double>());
            }
        }

        private void readSlidersList()
        {
            this.VarsList.Clear();
            this.SlidersList = new List<GH_NumberSlider>();
            int nVars = this.Params.Input[0].Sources.Count;

            NumVars = nVars;

            for (int i = 0; i < nVars; i++)
            {
                this.SlidersList.Add(this.Params.Input[0].Sources[i] as GH_NumberSlider);
            }

            foreach (GH_NumberSlider slider in SlidersList)
            {
                DSEVariable newVar = new DSEVariable((double)slider.Slider.Minimum, (double)slider.Slider.Maximum, (double)slider.Slider.Value);
                this.VarsList.Add(newVar);
            }
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
                return Properties.Resources.Capture1;
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
