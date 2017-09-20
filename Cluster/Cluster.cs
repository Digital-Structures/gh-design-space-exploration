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
              "Clusters designs using the K-means algorithm and adjusts variable bounds based on each cluster. WORKS ON DOUBLECLICK",
              "DSE", "Simplify")
        {

            this.VarsList = new List<DSEVariable>();
            this.VarsVals = new List<double>();
            this.MinVals = new List<double>();
            this.MaxVals = new List<double>();
            this.SlidersList = new List<GH_NumberSlider>();
            this.DesignMapSorted = new List<List<List<double>>>();
            this.ClusterAves = new List<List<double>>();
            this.DesignMap = new List<List<double>>();
            this.ClusterMaxs = new List<List<double>>();
            this.ClusterMins = new List<List<double>>();
            this.ClusterObjs = new List<List<double>>();
            this.labelstree = new List<List<int>>();
            this.newVars = new List<double>();
            
        }

        // Properties specific to this component:
        public List<DSEVariable> VarsList;
        public List<double> VarsVals;
        public List<double> MinVals;
        public List<double> MaxVals;
        public List<List<double>> DesignMap;
        public List<List<double>> DesignMapNoObj;
        public int numVars;
        public int numObjs;
        public int numClusters;
        public List<GH_NumberSlider> SlidersList;
        public bool ClusterDone;
        public int index;
        public List<double> newVars;
        public bool live;
        bool indexShifted;
        public double flexibility;
        public bool propCalculated;

        public List<List<List<double>>> DesignMapSorted; 
        public List<List<double>> ClusterAves;
        public List<List<double>> ClusterMaxs;
        public List<List<double>> ClusterMins;
        public List<List<double>> ClusterObjs;
        public List<List<int>> labelstree;
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
            pManager.AddNumberParameter("Flexibility", "Flex", "Number between 0-1 that sets the flexibility of each cluster's design space", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Cluster Index", "Index", "The index of the cluster being explored; 0 is entire dataset", GH_ParamAccess.item);


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("Cluster List", "ClusterL", "The Cluster number of each Design", GH_ParamAccess.tree);
            pManager.AddTextParameter("ClusterAvs", "ClusterAv", "Average variable values of each cluster", GH_ParamAccess.tree);
            pManager.AddTextParameter("ClusterMins", "ClusterMins", "Minimum values for each cluster", GH_ParamAccess.tree);
            pManager.AddTextParameter("ClusterMaxs", "ClusterMaxs", "Maximum values for each cluster", GH_ParamAccess.tree);
            pManager.AddTextParameter("ClusterObjs", "ClusterObj", "Average objective values for each cluster", GH_ParamAccess.tree);

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

            numObjs = DesignMap[0].Count - numVars;

            if (!DA.GetData(2, ref numClusters)) return;

            if (!DA.GetData(3, ref flexibility)) return;

            if (!DA.GetData(4, ref index)) return;


                List<List<double>> averageTree = new List<List<Double>>();

                ClusterLabelsList = ((ClusterComponentAttributes)this.m_attributes).LabelsList;

            if (ClusterDone & !propCalculated)
            {

                labelstree.Add(((ClusterComponentAttributes)this.m_attributes).LabelsList);

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
                ClusterObjs.Clear();

                for (int i = 0; i < numClusters; i++)
                {

                    ClusterAves.Add(new List<double>());
                    ClusterMaxs.Add(new List<double>());
                    ClusterMins.Add(new List<double>());
                    ClusterObjs.Add(new List<double>());

                    double[] sum = new double[numVars];
                    double[] average = new double[numVars];
                    double[] max = new double[numVars];
                    double[] min = new double[numVars];

                    double[] sumObj = new double[numObjs];
                    double[] averageObj = new double[numObjs];
                    double[] maxObj = new double[numObjs];
                    double[] minObj = new double[numObjs];
                    

                    // Capture average, max, min for variables
                    for (int l = 0; l < numVars; l++)

                    {
                        sum[l] = 0;
                        max[l] = double.MinValue;
                        min[l] = double.MaxValue;
                    }

                    for (int l = 0; l < numObjs; l++)

                    {
                        sumObj[l] = 0;
                        maxObj[l] = double.MinValue;
                        minObj[l] = double.MaxValue;
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

                    // Capture objective value averages

                    for (int j = 0; j < DesignMapSorted[i].Count; j++)

                    {
                        
                        for (int k = 0; k < numObjs; k++)

                        {
                            sumObj[k] = sumObj[k] + DesignMapSorted[i][j][k+numVars];

                            if (DesignMapSorted[i][j][k+numVars] > maxObj[k])

                            {
                                maxObj[k] = DesignMapSorted[i][j][k+numVars];
                            }
                            else if (DesignMapSorted[i][j][k+numVars] < minObj[k])

                            {

                                minObj[k] = DesignMapSorted[i][j][k+numVars];
                            }

                            averageObj[k] = sumObj[k] / DesignMapSorted[i].Count;

                        }


                    }

                    for (int k = 0; k < numVars; k++)
                    {
                        ClusterAves[i].Add(average[k]);
                        ClusterMaxs[i].Add(max[k]);
                        ClusterMins[i].Add(min[k]);
                    }

                    for (int k = 0; k < numObjs; k++)
                    {
                        ClusterObjs[i].Add(averageObj[k]);
                    }

                }


                // Change sliders
                if (ClusterDone && !indexShifted)

                {

                    for (int i = 0; i < labelstree[0].Count; i++)
                    {
                        labelstree[0][i] = labelstree[0][i] + 1;
                    }

                    indexShifted = true;
                    //DA.SetDataTree(0, ListOfListsToTree<int>(labelstree));

                }

                propCalculated = true;
            }


            if (ClusterDone & propCalculated)
            {

                DA.SetDataTree(0, ListOfListsToTree<int>(labelstree));
                DA.SetDataTree(1, ListOfListsToTree<Double>(ClusterAves));
                DA.SetDataTree(2, ListOfListsToTree<Double>(ClusterMins));
                DA.SetDataTree(3, ListOfListsToTree<Double>(ClusterMaxs));
                DA.SetDataTree(4, ListOfListsToTree<Double>(ClusterObjs));

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
