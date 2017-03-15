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
            this.DesignMapSorted = new List<List<List<double>>>();
            this.ClusterAves = new List<List<double>>();
            this.DesignMap = new List<List<double>>();
            this.ClusterMaxs = new List<List<double>>();
            this.ClusterMins = new List<List<double>>();
            this.newVars = new List<double>();
            
        }

        // Properties specific to this component:
        public List<DSEVariable> VarsList;
        public List<List<double>> DesignMap;
        public int numVars;
        public int numClusters;
        public List<GH_NumberSlider> SlidersList;
        public bool ClusterDone;
        public int index;
        public List<double> newVars;
        public bool live;

        public List<List<List<double>>> DesignMapSorted; 
        public List<List<double>> ClusterAves;
        public List<List<double>> ClusterMaxs;
        public List<List<double>> ClusterMins;
        public List<int> ClusterLabelsList;
        


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
            pManager.AddNumberParameter("New Variables", "NewVar", "Variables for the specific cluster", GH_ParamAccess.list, 0);

            pManager[5].Optional = true;

            //double trick = 0;
             
            Param_Number param5 = (Param_Number)pManager[5];
            
         


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("Cluster List", "ClusterL", "The Cluster number of each Design", GH_ParamAccess.tree);
            pManager.AddTextParameter("ClusterAvs", "ClusterAv", "Average variable values of each cluster", GH_ParamAccess.tree);
            pManager.AddTextParameter("ClusterMins", "ClusterMins", "Average variable values of each cluster", GH_ParamAccess.tree);

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

            var map = new GH_Structure<GH_Number>();
            if (!DA.GetDataTree(1, out map)) return;
            this.DesignMap = StructureToListOfLists(map);

            if (!DA.GetData(2, ref numClusters)) return;

            if (!DA.GetData(4, ref index)) return;


            List<List<int>> labelstree = new List<List<int>>();
            List<List<double>> averageTree = new List<List<Double>>();

            ClusterLabelsList = ((ClusterComponentAttributes)this.m_attributes).LabelsList;



            if (ClusterDone)
            {

                labelstree.Add(((ClusterComponentAttributes)this.m_attributes).LabelsList);
                DA.SetDataTree(0, ListOfListsToTree<int>(labelstree));


                /// <summary>
                /// 
                /// </summary>


                for (int i = 0; i < numClusters; i++)
                {

                    DesignMapSorted.Add(new List<List<double>>());
                    for (int j = 0; j < DesignMap.Count; j++)
                    {

                        if (ClusterLabelsList[j] == i)
                        {

                            DesignMapSorted[i].Add(DesignMap[j]);

                        }
                    }
                }


                ClusterAves.Clear();
                ClusterMaxs.Clear();
                ClusterMins.Clear();

                for (int i = 0; i < numClusters; i++)
                {
                    

                    ClusterAves.Add(new List<double>());
                    ClusterMaxs.Add(new List<double>());
                    ClusterMins.Add(new List<double>());

                    double[] sum = new double[numVars];
                    double[] average = new double[numVars];
                    double[] max = new double[numVars];
                    double[] min = new double[numVars];

                    for (int l = 0; l < numVars; l++)

                    {
                        sum[l] = 0;
                        max[l] = double.MinValue;
                        min[l] = double.MaxValue;
                    }

                    for (int j = 0; j < DesignMapSorted[i].Count; j++)

                    {

                        
                        for (int k = 0; k < numVars; k++)

                        {
                            sum[k] = sum[k] + DesignMapSorted[i][j][k];

                            if (DesignMapSorted[i][j][k] > max[k])

                            {
                                max[k] = DesignMapSorted[i][j][k];
                            }
                            else if (DesignMapSorted[i][j][k] < min[k])

                            {

                                min[k] = DesignMapSorted[i][j][k];
                            }

                            average[k] = sum[k] / DesignMapSorted[i].Count;
    
                        }
    

                    }

                    for (int k = 0; k < numVars; k++)
                    {
                        ClusterAves[i].Add(average[k]);
                        ClusterMaxs[i].Add(max[k]);
                        ClusterMins[i].Add(min[k]);
                    }
                }



                // Change sliders
                if (ClusterDone)
                {

                    if (!DA.GetDataList<double>(5, this.newVars)) return;


                    //                    this.newVars.Clear();
                    //            for (int i = 0; i < numVars; i++)
                    //            {
                    //                this.newVars.Add(this.Params.Input[5].Sources[i]);
                    //            }



                    List<IGH_Param> sliderList = new List<IGH_Param>();

                        foreach (IGH_Param src in Params.Input[0].Sources)
                        {
                            sliderList.Add(src);
                        }

                    for (int i = 0; i < numVars; i++)
                        {
                                Grasshopper.Kernel.Special.GH_NumberSlider nslider = (Grasshopper.Kernel.Special.GH_NumberSlider) sliderList[i];

                                nslider.TrySetSliderValue((decimal) newVars[i]);
                            }
                        
                    }

                if (this.newVars.Count > 1)
                {

                    Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);

                    List<IGH_Param> sliderList = new List<IGH_Param>();

                    foreach (IGH_Param src in Params.Input[0].Sources)
                    {
                        sliderList.Add(src);
                    }

                    for (int i = 0; i < numVars; i++)
                    {
                        Grasshopper.Kernel.Special.GH_NumberSlider nslider = (Grasshopper.Kernel.Special.GH_NumberSlider)sliderList[i];

                        nslider.TrySetSliderValue((decimal)newVars[i]);
                    }


                }



              


                    //Params.Input[5].Sources.ElementAt(1)

                    ////////////


                DA.SetDataTree(1, ListOfListsToTree<Double>(ClusterAves));
                DA.SetDataTree(2, ListOfListsToTree<Double>(ClusterMins));


            }


            if (live)

            {



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
                return Properties.Resources.cluster1;
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
