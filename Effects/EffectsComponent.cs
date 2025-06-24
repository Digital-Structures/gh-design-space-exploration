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
              "Calculates the average effect of each variable on performance. WORKS ON DOUBLECLICK ",
              "DSE", "Simplify")
        {

            this.VarsList = new List<DSEVariable>();
            this.VarsVals = new List<double>();
            this.MinVals = new List<double>();
            this.MaxVals = new List<double>();
            this.SlidersList = new List<GH_NumberSlider>();

            this.ObjInput = new List<double>();
            this.LevelSettings = new List<double>();
            this.ObjValues = new List<List<double>>();
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
        public List<List<double>> ObjValues;
        public bool Iterating;
        public bool FirstRead;
        public bool ObjCalculated;
        public int numVars;
        public int numObjs;
        public int numLevels;
        public List<GH_NumberSlider> SlidersList;
        public bool EffectsDone;
        public int index;
        public List<double> newVars;
        public bool DebugLogging;



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
            pManager.AddIntegerParameter("Number of Objectives", "#Obj", "Number of objectives.  Must match length of 'Obj' input.", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("Debug Logging", "Debug", "Enable console debug logging for troubleshooting", GH_ParamAccess.item, false);
            
            // Make debug parameter optional
            pManager[5].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this compzonent.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("Average Effects", "AvgEff", "The magnitude of the average effects for each variable", GH_ParamAccess.tree);
            pManager.AddTextParameter("Raw Effects", "RawEff", "Raw effects of each variable setting", GH_ParamAccess.tree);
            pManager.AddTextParameter("Design Map + Objectives", "DM+O", "Set of design variables plus objective(s) recorded during calculations", GH_ParamAccess.tree);


        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            readSlidersList();

            // Get input data
            List<double> variables = new List<double>();
            if (!DA.GetDataList(0, variables)) return;
            numVars = variables.Count;

            if (!DA.GetDataList<double>(1, this.ObjInput)) return;
            numObjs = ObjInput.Count;

            if (!DA.GetData(2, ref this.numLevels)) return;
            if (!DA.GetDataList<double>(3, this.LevelSettings)) return;
            if (!DA.GetData(4, ref this.numObjs)) return;
            
            // Read optional debug flag, default to false if not provided
            this.DebugLogging = false;
            DA.GetData(5, ref this.DebugLogging);

            // During iteration, collect objective values
            if (Iterating)
            {
                List<double> objectiveValues = new List<double>();
                if (DA.GetDataList(1, objectiveValues))
                {
                    ObjValues.Add(objectiveValues);
                }
                return;
            }

            // Output results after effects calculation is done
            if (EffectsDone && !Iterating)
            {
                var attrs = (EffectsComponentAttributes)this.m_attributes;
                
                DA.SetDataTree(0, ListOfListsToTree<double>(attrs.OverallEff));
                
                // Output raw effects for each objective
                if (attrs.IndEffectsAvg.Count > 0)
                {
                    DA.SetDataTree(1, ListOfListsToTree<double>(attrs.IndEffectsAvg[0]));
                }
                
                DA.SetDataTree(2, AssembleDMOTree(attrs.DesignMapEffects, this.ObjValues));
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
                    //There is signature mismatch in IGH_GOO and GH_GOO CastTo method that GH_NUMBER modifies. 
                    // n.CastTo<double>(out d);
                    // doubles.Add(d);

                    if (GH_Convert.ToDouble(n, out d, GH_Conversion.Both))
                    {
                        doubles.Add(d);
                    }
                }
                list.Add(doubles);
            }
            return list;
        }

        private void readSlidersList()
        {
            this.VarsList.Clear();
            this.VarsVals.Clear();  // Clear existing values
            this.MinVals.Clear();   // Clear existing values
            this.MaxVals.Clear();   // Clear existing values
            
            this.SlidersList = new List<GH_NumberSlider>();
            int nVars = this.Params.Input[0].Sources.Count;
            for (int i = 0; i < nVars; i++)
            {
                this.SlidersList.Add(this.Params.Input[0].Sources[i] as GH_NumberSlider);
            }

            foreach (GH_NumberSlider slider in SlidersList)
            {
                if (slider != null)  // Add null check
                {
                    DSEVariable newVar = new DSEVariable((double)slider.Slider.Minimum, (double)slider.Slider.Maximum, (double)slider.Slider.Value);
                    this.VarsList.Add(newVar);

                    VarsVals.Add((double)slider.Slider.Value);
                    MinVals.Add((double)slider.Slider.Minimum);
                    MaxVals.Add((double)slider.Slider.Maximum);
                }
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

            int nSamples = ((EffectsComponentAttributes)this.m_attributes).numRows;
            
            // Add detailed logging only if debug is enabled
            if (DebugLogging)
            {
                System.Console.WriteLine($"AssembleDMO Debug Info:");
                System.Console.WriteLine($"  nSamples (numRows): {nSamples}");
                System.Console.WriteLine($"  vars.Count: {vars.Count}");
                System.Console.WriteLine($"  obj.Count: {obj.Count}");
                System.Console.WriteLine($"  varsCopy.Count: {varsCopy.Count}");
            }
            
            for (int i = 0; i < nSamples; i++)
            {
                try
                {
                    if (i < varsCopy.Count && i < obj.Count)
                    {
                        List<double> l = varsCopy[i];
                        l.AddRange(obj[i]);
                    }
                    else
                    {
                        if (DebugLogging)
                        {
                            System.Console.WriteLine($"  Warning: Skipping index {i} - varsCopy.Count: {varsCopy.Count}, obj.Count: {obj.Count}");
                        }
                        
                        // If we have vars but no objectives, just keep the vars
                        if (i < varsCopy.Count)
                        {
                            if (DebugLogging)
                            {
                                System.Console.WriteLine($"  Adding empty objectives for variable set {i}");
                            }
                            // Add placeholder objective values (zeros)
                            for (int objIdx = 0; objIdx < numObjs; objIdx++)
                            {
                                varsCopy[i].Add(0.0);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (DebugLogging)
                    {
                        System.Console.WriteLine($"  Error at index {i}: {ex.GetType().Name} - {ex.Message}");
                        System.Console.WriteLine($"  Stack trace: {ex.StackTrace}");
                    }
                    
                    // Continue with next iteration instead of crashing
                    continue;
                }
            }

            return varsCopy;
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
                return MyProject1.Properties.Resources.Effects1;
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