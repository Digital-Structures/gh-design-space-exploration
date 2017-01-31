using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DSECommon;

using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Accord.MachineLearning;

namespace Cluster
{
    public class ClusterComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ClusterComponent()
          : base("Cluster", "Cluster",
              "Clusters designs using the K-means algorithm and adjusts variable bounds based on each cluster",
              "DSE", "Simplify")
        {

            this.VarsList = new List<DSEVariable>();
            this.SlidersList = new List<GH_NumberSlider>();

        }

        // Properties specific to this component:
        public List<DSEVariable> VarsList;
        public List<List<double>> DesignMap;
        public int numClusters;
        public List<GH_NumberSlider> SlidersList;
        public bool Iterating = false;
        public bool FirstRead;


        /// <summary>
        /// Creates custom attributes for this component.
        /// </summary>
        public override void CreateAttributes()
        {
            base.m_attributes = new ClusterComponentAttributes(this);
        }



        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddNumberParameter("Variables", "Var", "Variables that define the design space", GH_ParamAccess.list);
            pManager.AddNumberParameter("Design map", "DM(+O)", "Data used to perform clustering", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Number of Clusters", "#Clust", "The objective being considered for cluster ranking", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Obj Number", "Obj#", "The objective being considered for cluster ranking", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Cluster Index", "Index", "The index of the cluster being explored", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("Cluster List", "ClusterL", "The Cluster number of each Design", GH_ParamAccess.tree);
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



            

           
            if (FirstRead)
            {
                FirstRead = false;
                
            }

            var map = new GH_Structure<GH_Number>();
            if (!DA.GetDataTree(1, out map)) return;
            this.DesignMap = StructureToListOfLists(map);

            if (!DA.GetData(2, ref numClusters)) return;

            List<List<int>> labelstree = new List<List<int>>();

            if (Iterating)
            {
   
                labelstree.Add(((ClusterComponentAttributes)this.m_attributes).labelsList);

            }

            if (!Iterating)
            {

                DA.SetDataTree(0, ListOfListsToTree<int>(labelstree));

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
            get { return new Guid("{fb314d27-3b9d-4f4a-8cbb-babb18074afd}"); }
        }
    }
}
