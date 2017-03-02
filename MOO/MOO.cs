using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;


using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
using Grasshopper;

namespace MOO
{
    public class MOO : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public MOO()
          : base("MOO", "MOO",
              "Executes the NSGA-II multi-objective optimization algorithm.",
              "DSE", "Catalog")
        {
            this.ObjValues = new List<List<double>>();
            this.VarValues = new List<List<double>>();
            this.comparer = new ObjectiveComparer();
            this.MyRand = new Random();
        }

        public override void CreateAttributes()
        {
            base.m_attributes = new NSGASolutionComponentAttributes(this);
        }


        public bool MooDone;

        public List<List<double>> ObjValues;
        public List<List<double>> VarValues;
        public List<double> objectives;
        public List<GH_NumberSlider> slidersList = new List<GH_NumberSlider>();
        public int popSize = 0, maxEvals = 0;
        public string directory = null, fileName = null;
        public string log = null;
        private ObjectiveComparer comparer;
        public Random MyRand;
        public int Seed;




        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddNumberParameter("Variables", "Var", "Design Variables", GH_ParamAccess.list); // Variables
            pManager.AddNumberParameter("Objectives", "Obj", "Design Objectives", GH_ParamAccess.list); // Objectives
            pManager.AddIntegerParameter("Population Size", "Pop", "Population Size: number of solutions for each interation", GH_ParamAccess.item); // Population size
            pManager.AddIntegerParameter("Max Evaluations", "MaxEvals", "Max number of function evaluations", GH_ParamAccess.item); // Max number of iterations
            pManager.AddTextParameter("Filename", "F", "File name + extension ('output.csv')", GH_ParamAccess.item); // FIle name
            pManager.AddTextParameter("Directory", "Dir", "Address of output file", GH_ParamAccess.item); // Output destination
            pManager.AddIntegerParameter("Seed", "S", "Random Seed. Integer 0 will leave the seed unspecified.", GH_ParamAccess.item);
            if (this.Seed != 0) { this.MyRand = new Random(this.Seed); }

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("Pareto Front", "Pareto", "The last NSGA-II generation, approximating the Pareto Front", GH_ParamAccess.item);
            pManager.AddTextParameter("All Solutions", "All Solutions", "Record of all designs and their performance recorded while the algorithm was running", GH_ParamAccess.tree);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            // We'll start by declaring variables and assigning them starting values.
            List<double> variables = new List<double>();
            objectives = new List<double>();

            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetDataList(0, variables)) return;
            if (!DA.GetDataList(1, objectives)) return;
            if (!DA.GetData(2, ref popSize)) return;
            if (!DA.GetData(3, ref maxEvals)) return;
            if (!DA.GetData(4, ref fileName)) return;
            if (!DA.GetData(5, ref directory)) return;
            if (!DA.GetData(6, ref Seed)) return;




            if (MooDone)
            {


                var solutions = ((NSGASolutionComponentAttributes)this.m_attributes).allSolutionsTrack;
                var paretoSolutions = solutions.GetRange(solutions.Count - 1 - this.popSize, this.popSize);

                this.comparer.NumVars = variables.Count;
                paretoSolutions.Sort(this.comparer);

                DA.SetDataTree(0, ListOfListsToTree<double>(paretoSolutions));

                DA.SetDataTree(1, ListOfListsToTree<double>(solutions));


            }


        }

        public List<double> getObjectives()
        {
            return objectives;
        }

        public void LogAddMessage(string message)
        {
            log = log + message + "\r\n";
        }

        private DataTree<double> AssembleDMOTree(List<List<double>> vars, List<List<double>> obj)
        {
            return ListOfListsToTree<double>(AssembleDMO(vars, obj));
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

        private void populateObjOutput()
        {
            for (int i = 0; i < popSize; i++)
            {
                this.ObjValues.Add(new List<double>());
                this.VarValues.Add(new List<double>());
            }
        }

        public List<GH_NumberSlider> readSlidersList()
        {
            slidersList.Clear();

            foreach (IGH_Param param in Params.Input[0].Sources)
            {
                GH_NumberSlider slider = param as Grasshopper.Kernel.Special.GH_NumberSlider;
                slidersList.Add(slider);

            }
            return slidersList;
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
                //return this.res;
                return Properties.Resources.MOO1;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{bb604608-8f85-4c53-b8b0-cbc9dd4654ee}"); }
        }
    }

    public class ObjectiveComparer : IComparer<List<double>>
    {

        public int NumVars;
        public int Compare(List<double> x, List<double> y)
        {
            if(x[NumVars] >= y[NumVars])
            {
                return 1;
            } else
            {
                return -1;
            }
        }

    }

}
