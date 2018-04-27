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

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace Gradient_MOO
{
    public class GradientMOOComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public GradientMOOComponent()
          : base("Gradient_MOO", "GradMOO",
              "Iteratively finds areas of direct tradeoff (Pareto fronts) for multi-objective problems",
              "DSE", "Navigate")
        {

            this.VarsList = new List<DSEVariable>();
            this.VarsVals = new List<double>();
            this.MinVals = new List<double>();
            this.MaxVals = new List<double>();
            this.SlidersList = new List<GH_NumberSlider>();
            this.ObjInput = new List<double>();

            this.ObjNum = new int();
            this.StepSize = new double();
            this.Direction = new int();

        }

        // Properties specific to this component:
        public List<DSEVariable> VarsList;
        public List<double> ObjInput;

        public List<double> MinVals;
        public List<double> MaxVals;
        public List<double> VarsVals;
        public List<List<double>> DesignMap;
        public List<List<double>> ObjValues;
        public bool Iterating = false;
        public int numVars;
        public int numObjs;
        public List<GH_NumberSlider> SlidersList;
        public int index;
        public List<double> newVars;
        public int ObjNum;
        public double StepSize;
        public int Direction;
        public bool Step;
        public bool Mode;
        public double threshold;

        public override void CreateAttributes()
        {
            base.m_attributes = new GradientMOOComponentAttributes(this);
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddNumberParameter("Variables", "Var", "Variables to be sampled.  Must be less than 13.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Objectives", "Obj", "One or more performance objectives", GH_ParamAccess.list);
            pManager.AddNumberParameter("Step size", "Step Size", "Step size", GH_ParamAccess.item, 0.05);
            pManager.AddIntegerParameter("Direction", "Direction", "Direction to step in; 1 = better, 2 = worse", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("Pareto?", "Pareto?", "Switch modes to move 'in' the gradient direction", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Threshold", "Threshold", "Switch modes to move 'in' the gradient direction", GH_ParamAccess.item, 175);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddNumberParameter("Tested Design Map", "DM", "Map of tested designs during finite differences", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Gradient", "Gradient", "The Gradient at the previous point", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Bisector", "Bisector", "Direction between two gradients", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Angle", "Angle", "Angle between gradients", GH_ParamAccess.item);
            pManager.AddNumberParameter("Bisector Length", "Bisector Length", "I mean come on, the name basically says it", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Reset lists
            this.VarsList = new List<DSEVariable>();
            this.VarsVals = new List<double>();
            this.MinVals = new List<double>();
            this.MaxVals = new List<double>();
            this.SlidersList = new List<GH_NumberSlider>();
            this.ObjInput = new List<double>();


            readSlidersList();
            // First, we need to retrieve all data from the input parameters.
            // We'll start by declaring variables and assigning them starting values.


            List<double> variables = new List<double>();
            if (!DA.GetDataList(0, variables)) return;

            numVars = variables.Count;


            if (!DA.GetDataList<double>(1, this.ObjInput)) return;

            //HARDCODE
            numObjs = ObjInput.Count;

            if (!DA.GetData(2, ref this.StepSize)) return;
            if (!DA.GetData(3, ref this.Direction)) return;
            if (!DA.GetData(4, ref this.Mode)) return;
            if (!DA.GetData(5, ref this.threshold)) return;

            if (Iterating)
            {
                List<double> o = new List<double>();

                if (!DA.GetDataList(1, o)) { return; }

                ObjValues.Add(o);

            }

            //Do the step


            DA.SetDataTree(0, ListOfListsToTree<double>(((GradientMOOComponentAttributes)this.m_attributes).DesignMapStepperCombined));
            DA.SetDataTree(1, ListOfListsToTree<double>(((GradientMOOComponentAttributes)this.m_attributes).Gradient));
            DA.SetDataTree(2, ListToTree<double>(((GradientMOOComponentAttributes)this.m_attributes).deltaSlider));
            if (((GradientMOOComponentAttributes)this.m_attributes).stepped)
            {
                //DA.SetDataTree(2, ListToTree<double>(((StepperComponentAttributes)this.m_attributes).IsoPerfDirList));
            }
            DA.SetData(3, ((GradientMOOComponentAttributes)this.m_attributes).angle);
            DA.SetData(4, ((GradientMOOComponentAttributes)this.m_attributes).bisectLength);

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

        static DataTree<T> ListToTree<T>(List<T> List)
        {
            DataTree<T> tree = new DataTree<T>();
            for (int i = 0; i < List.Count; i++)
            {
                tree.Add(List[i], new GH_Path(i));
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
            get { return new Guid("f99366ca-ed47-4882-aa13-d99dd84567db"); }
        }
    }
}
