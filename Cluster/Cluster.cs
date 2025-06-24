using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using DSECommon;

using Grasshopper;
using Grasshopper.Kernel.Data;
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
            
            // Disable caching to force refresh
            this.Message = "Click Run to update";
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

        // Lists for keeping track of design data
        public List<List<List<double>>> DesignMapSorted; 
        public List<List<double>> ClusterAves;
        public List<List<double>> ClusterMaxs;
        public List<List<double>> ClusterMins;
        public List<List<double>> ClusterObjs;
        public List<List<int>> labelstree;
        public List<int> ClusterLabelsList;
        public List<int> LabelsList;


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
            pManager.AddBooleanParameter("Run", "Run", "Seperate DM into clusters", GH_ParamAccess.item);


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
            // Clear previous data to ensure fresh calculations
            this.DesignMapSorted.Clear();
            this.ClusterAves.Clear();
            this.ClusterMaxs.Clear();
            this.ClusterMins.Clear();
            this.ClusterObjs.Clear();
            this.labelstree.Clear();
            this.propCalculated = false;
            
            // Read in slider properties - reinitialize lists to ensure fresh data
            this.VarsVals.Clear();
            this.MinVals.Clear();
            this.MaxVals.Clear();
            readSlidersList();
            
            List<double> variables = new List<double>();
            if (!DA.GetDataList(0, variables)) return;
            numVars = variables.Count;

            // Read in design map
            var map = new GH_Structure<GH_Number>();
            if (!DA.GetDataTree(1, out map)) return;
            this.DesignMap = StructureToListOfLists(map);

            numObjs = DesignMap[0].Count - numVars;

            // Take in other inputs
            if (!DA.GetData(2, ref numClusters)) return;
            if (!DA.GetData(3, ref flexibility)) return;
            if (!DA.GetData(4, ref index)) return;

            // If button is clicked, run clustering
            if (Run(DA,5))
            {
                // Initialize the lists for each cluster
                for (int i = 0; i < numClusters; i++)
                {
                    ClusterObjs.Add(new List<double>());
                    labelstree.Add(new List<int>());
                }

                //run clustering process
                KMeans kmeans = new KMeans(numClusters);

                double[][] data = DesignMap.Select(a => a.ToArray()).ToArray();

                // Make sure we have data to cluster
                if (data.Length == 0 || data[0].Length < numVars)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not enough data to cluster");
                    return;
                }

                // Extract just the variables (not objectives) for clustering
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i] = data[i].Take(data[i].Count() - numObjs).ToArray();
                }

                // Debug output
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, 
                    $"Clustering {data.Length} designs with {numVars} variables into {numClusters} clusters");

                // Compute clusters
                int[] labels = kmeans.Compute(data);
                
                LabelsList = labels.OfType<int>().ToList();

                // Initialize labelstree for output
                for (int i = 0; i < LabelsList.Count; i++)
                {
                    int clusterIndex = LabelsList[i];
                    if (clusterIndex >= 0 && clusterIndex < labelstree.Count)
                    {
                        labelstree[clusterIndex].Add(i);
                    }
                }

                // create Sorted list
                DesignMapSorted.Clear();
                for (int i = 0; i < numClusters; i++)
                {
                    DesignMapSorted.Add(new List<List<double>>());
                }

                for (int j = 0; j < DesignMap.Count; j++)
                {
                    if (j < LabelsList.Count)
                    {
                        int clusterIndex = LabelsList[j];
                        if (clusterIndex >= 0 && clusterIndex < DesignMapSorted.Count)
                        {
                            DesignMapSorted[clusterIndex].Add(DesignMap[j]);
                        }
                    }
                }

                // Calculate min/max/average for each cluster
                ClusterAves.Clear();
                ClusterMaxs.Clear();
                ClusterMins.Clear();
                ClusterObjs.Clear();

                // Initialize with the slider values
                ClusterAves.Add(new List<double>(VarsVals));
                ClusterMaxs.Add(new List<double>(MaxVals));
                ClusterMins.Add(new List<double>(MinVals));

                for (int i = 0; i < numClusters; i++)
                {
                    ClusterAves.Add(new List<double>());
                    ClusterMaxs.Add(new List<double>());
                    ClusterMins.Add(new List<double>());
                    ClusterObjs.Add(new List<double>());

                    // Skip empty clusters
                    if (DesignMapSorted[i].Count == 0)
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Cluster {i} is empty");
                        
                        // Add placeholder values
                        for (int k = 0; k < numVars; k++)
                        {
                            ClusterAves[i+1].Add(0);
                            ClusterMaxs[i+1].Add(0);
                            ClusterMins[i+1].Add(0);
                        }
                        
                        for (int k = 0; k < numObjs; k++)
                        {
                            ClusterObjs[i].Add(0);
                        }
                        
                        continue;
                    }

                    double[] sum = new double[numVars];
                    double[] average = new double[numVars];
                    double[] max = new double[numVars];
                    double[] min = new double[numVars];
                    
                    double[] sumObj = new double[numObjs];
                    double[] averageObj = new double[numObjs];
                    double[] maxObj = new double[numObjs];
                    double[] minObj = new double[numObjs];

                    // Initialize values
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
                    
                    // Calculate statistics for this cluster
                    for (int j = 0; j < DesignMapSorted[i].Count; j++)
                    {
                        if (DesignMapSorted[i][j].Count < numVars + numObjs)
                        {
                            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, 
                                $"Design {j} in cluster {i} has insufficient data");
                            continue;
                        }
                        
                        // Process variables
                        for (int k = 0; k < numVars; k++)
                        {
                            sum[k] += DesignMapSorted[i][j][k];
                            max[k] = Math.Max(max[k], DesignMapSorted[i][j][k]);
                            min[k] = Math.Min(min[k], DesignMapSorted[i][j][k]);
                        }
                        
                        // Process objectives
                        for (int k = 0; k < numObjs; k++)
                        {
                            sumObj[k] += DesignMapSorted[i][j][k+numVars];
                            maxObj[k] = Math.Max(maxObj[k], DesignMapSorted[i][j][k+numVars]);
                            minObj[k] = Math.Min(minObj[k], DesignMapSorted[i][j][k+numVars]);
                        }
                    }
                    
                    // Calculate averages
                    for (int k = 0; k < numVars; k++)
                    {
                        average[k] = sum[k] / DesignMapSorted[i].Count;
                        ClusterAves[i+1].Add(average[k]);
                        ClusterMaxs[i+1].Add(max[k]);
                        ClusterMins[i+1].Add(min[k]);
                    }
                    
                    for (int k = 0; k < numObjs; k++)
                    {
                        averageObj[k] = sumObj[k] / DesignMapSorted[i].Count;
                        ClusterObjs[i].Add(averageObj[k]);
                    }
                }

                propCalculated = true;
                ClusterDone = true;

                // Debug output
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, 
                    $"Calculated statistics for {numClusters} clusters");
            }

            // Create list of cluster labels
            List<List<double>> averageTree = new List<List<Double>>();

            ClusterLabelsList = LabelsList;

            if (ClusterDone & !propCalculated)
            {

                labelstree.Add(LabelsList);

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

                // Shift indices of labels from 0-numbering to 1-numbering (so that "0" will be original inputs)
                if (ClusterDone && !indexShifted)

                {

                    for (int i = 0; i < labelstree[0].Count; i++)
                    {
                        labelstree[0][i] = labelstree[0][i] + 1;
                    }

                    indexShifted = true;
                    //DA.SetDataTree(0, ListOfListsToTree<int>(labelstree));

                }

                ClusterAves.Clear();
                ClusterMaxs.Clear();
                ClusterMins.Clear();
                ClusterObjs.Clear();

                // Add original inputs of variable sliders to cluster lists, so that "0" will reset to original design space
                ClusterAves.Add(new List<double>());
                ClusterMaxs.Add(new List<double>());
                ClusterMins.Add(new List<double>());

                for (int k = 0; k < numVars; k++)
                {
                    ClusterAves[0].Add(VarsVals[k]);
                    ClusterMaxs[0].Add(MaxVals[k]);
                    ClusterMins[0].Add(MinVals[k]);
                }

                // Add cluster properties to lists for outputs and cycling through clusters (in Component Attributes)
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
                        ClusterAves[i+1].Add(average[k]);
                        ClusterMaxs[i+1].Add(max[k]);
                        ClusterMins[i+1].Add(min[k]);
                    }

                    for (int k = 0; k < numObjs; k++)
                    {
                        ClusterObjs[i].Add(averageObj[k]);
                    }

                }

                propCalculated = true;
            }

            // Set all outputs
            if (ClusterDone & propCalculated)
            {

                DA.SetDataTree(0, ListOfListsToTree<int>(labelstree));
                DA.SetDataTree(1, ListOfListsToTree<Double>(ClusterAves));
                DA.SetDataTree(2, ListOfListsToTree<Double>(ClusterMins));
                DA.SetDataTree(3, ListOfListsToTree<Double>(ClusterMaxs));
                DA.SetDataTree(4, ListOfListsToTree<Double>(ClusterObjs));

            }

            

        }

        public static bool Run(Grasshopper.Kernel.IGH_DataAccess DA, int index)
        {
            bool run = false;
            DA.GetData<bool>(index, ref run);
            return run;
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
                    // There is signature mismatch in IGH_GOO and GH_GOO CastTo method that GH_NUMBER modifies. 
                    //cn.CastTo<double>(out d);
                    // doubles.Add(d);

                    if (GH_Convert.ToDouble(n, out double d, GH_Conversion.Both))
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
            this.SlidersList.Clear();
            
            // Add debug message to help troubleshoot
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, $"Reading {this.Params.Input[0].Sources.Count} sliders");
            
            int nVars = this.Params.Input[0].Sources.Count;
            for (int i = 0; i < nVars; i++)
            {
                var sliderParam = this.Params.Input[0].Sources[i] as GH_NumberSlider;
                if (sliderParam != null)
                {
                    this.SlidersList.Add(sliderParam);
                    
                    // Add debug for slider values
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, 
                        $"Slider {i}: Min={sliderParam.Slider.Minimum}, Max={sliderParam.Slider.Maximum}, Value={sliderParam.Slider.Value}");
                }
                else
                {
                    // If not a slider, get the value directly
                    double value = 0.0;
                    if (this.Params.Input[0].Sources[i].VolatileData.AllData(true).Count() > 0)
                    {
                        GH_Convert.ToDouble(this.Params.Input[0].Sources[i].VolatileData.AllData(true).ElementAt(0), 
                            out value, GH_Conversion.Both);
                        
                        // Create a variable with default min/max if not a slider
                        DSEVariable newVar = new DSEVariable(value - 10.0, value + 10.0, value);
                        this.VarsList.Add(newVar);
                        
                        VarsVals.Add(value);
                        MinVals.Add(value - 10.0);
                        MaxVals.Add(value + 10.0);
                        
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, $"Non-slider input {i}: Value={value}");
                    }
                }
            }

            foreach (GH_NumberSlider slider in SlidersList)
            {
                DSEVariable newVar = new DSEVariable(
                    (double)slider.Slider.Minimum, 
                    (double)slider.Slider.Maximum, 
                    (double)slider.Slider.Value);
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

        // Override this method to force recompute when inputs change
        public override void AddedToDocument(GH_Document document)
        {
            base.AddedToDocument(document);
            this.ExpireSolution(true);
        }

        // Ensure component refreshes when there's a change
        public override void ExpireSolution(bool recompute)
        {
            // Add debug message
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Component solution expired - will recompute");
            base.ExpireSolution(recompute);
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
