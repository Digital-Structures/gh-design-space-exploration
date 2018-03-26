using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;



namespace Diversity
{
    public class DiversityComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public DiversityComponent()
          : base("Diversity", "Diversity",
              "Calculates the diversity of a set of design vectors",
              "DSE", "Simplify")
        {

            this.DesignMap = new List<List<double>>();
            this.DesignMapThreshold = new List<List<double>>();
            this.Objectives = new List<List<double>>();
            this.Targets = new List<double>();
            this.DesignMapDiv = new List<List<double>>();



        }


        public List<List<double>> DesignMap;
        public List<List<double>> DesignMapThreshold;
        public List<List<double>> DesignMapDiv;
        public List<List<double>> Objectives;
        public List<double> Targets;
        public List<double> centroid;
        public List<double> median;
        public List<List<double>> distances;
        public List<double> diversity;
        public List<double> diversityfin;
        public double threshold;
        public int Iterations;
        public int cullTo;
        public List<double> diversityfinCull;
        public List<List<double>> DesignMapDivFinal;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddNumberParameter("Design map", "DM", "Set of design vectors for calculating set diversity", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Mode", "Mode", "Diversity measurement type. Right click to choose type.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Objective Scores", "Obj (DM)", "List of objective scores for designs in DM", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Objective Targets", "Obj Targets", "Targets for each objective", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Objective Threshold", "Threshold", "If < 1, fraction that solutions can differ from target", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Cull to", "Cull to", "Desired number of solutions in final set", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Iterations", "Iter", "Iterations for diversity culling - recommended at least 1000 if not too slow", GH_ParamAccess.item, 10000);
            pManager.AddBooleanParameter("Run", "Run", "Run diversity filtering", GH_ParamAccess.item);

            // Add possible values for the mode input
            Param_Integer param = (Param_Integer)pManager[1];
            param.AddNamedValue("Average [0]", 0);
            param.AddNamedValue("Sparseness (avg) [1]", 1);
            param.AddNamedValue("Sparseness (med) [2]", 2);
            param.AddNamedValue("Outlier [3]", 3);
           // param.AddNamedValue("Enclosing Ball [4]", 2);
           // param.AddNamedValue("Convex Hull [5]", 3);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("Diversity of Culled", "DivCulled", "Measured diversity of culled set using selected metric", GH_ParamAccess.tree);
            pManager.AddTextParameter("Diversity of All Qualified", "DivQual", "Measured diversity of qualified set using selected metric", GH_ParamAccess.tree);
            pManager.AddTextParameter("Culled Set", "CulledSet", "Diverse set of designs for consideration", GH_ParamAccess.tree);
            pManager.AddTextParameter("Qualified Set", "QualSet", "Full set of qualifying designs", GH_ParamAccess.tree);


        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            // Read in list of design vectors
            var map = new GH_Structure<GH_Number>();
            if (!DA.GetDataTree(0, out map)) return;
            this.DesignMap = StructureToListOfLists(map);

            int mode = 0;
            if (!DA.GetData(1, ref mode)) return;

            var map2 = new GH_Structure<GH_Number>();
            DA.GetDataTree(2, out map2);
            this.Objectives = StructureToListOfLists(map2);


            int dimension = this.DesignMap[0].Count;
            int numObj = this.Objectives[0].Count;

            var map3 = new GH_Structure<GH_Number>();
            DA.GetDataTree(3, out map3);
            this.Targets = StructureToList(map3);

            DA.GetData(4, ref threshold);
            DA.GetData(5, ref cullTo);
            DA.GetData(6, ref Iterations);


            // Do filtering based on threshold first

            DesignMapThreshold.Clear();

            // Add all designs that qualify
            for (int i = 0; i < DesignMap.Count(); i++)
            {

                List<Boolean> qualified = new List<Boolean>();

                for (int j = 0; j < Targets.Count(); j++)
                {

                    if (Objectives[i][j] > (Targets[j] - threshold * Targets[j]) && Objectives[i][j] < (Targets[j] + threshold * Targets[j]))
                    {
                        qualified.Add(true);
                    }
                    else { qualified.Add(false); }
                }

                if (!qualified.Any(c => c == false))
                {
                    DesignMapThreshold.Add(DesignMap[i]);
                }

            }


            double divFull = Diversity(mode, DesignMap);

            List<double> diversityfinselected = new List<double>();
            diversityfinselected.Add(divFull);


            //if (Run(DA, 5))

            //{

            // Do Diversity Calculations




            //tree.Add(diversity[mode], new GH_Path());
            //tree2.AddRange(diversityfin, new GH_Path());

            DA.SetDataTree(1, ListToTree(diversityfinselected));
            

            //}

            DA.SetDataTree(3, ListOfListsToTree(DesignMapThreshold));

            // ---------------------------------------------------------------------------------------------

            if (Run(DA, 7))

            {

                double maxDiv = 0;
                DesignMapDivFinal = new List<List<double>>();

                for (int j = 0; j < Iterations; j++)
                {

                    // Had random here
                    Random MyRand = new Random();
                    int Range = DesignMapThreshold.Count();

                    DesignMapDiv.Clear();


                    for (int i = 0; i < cullTo; i++)
                    {

                        List<int> ind = new List<int>();
                        
                        int Rand = MyRand.Next(0, Range); //for ints
                        ind.Add(Rand);
                        if (ind.Any(c => c == Rand)) { Rand = MyRand.Next(0, Range); }

                            DesignMapDiv.Add(DesignMapThreshold[Rand]);
                    }

                    double divCull = Diversity(mode, DesignMapDiv);

                    if (divCull > maxDiv)
                    {
                        maxDiv = divCull;
                        DesignMapDivFinal = DesignMapDiv;
                    }


                }



                List<double> diversityfinCull = new List<double>();
                diversityfinCull.Add(maxDiv);

                DA.SetDataTree(0, ListToTree(diversityfinCull));
                DA.SetDataTree(2, ListOfListsToTree(DesignMapDivFinal));

            }

            


        }


        // --------------------------------------------------------------------------------------------------------



        static double Diversity(int mode, List<List<double>> DesignMap)

        {

            List<double> centroid = new List<double>();
            List<double> median = new List<double>();
            List<List<double>> distances = new List<List<double>>();
            List<double> diversity = new List<double>();
            List<double> diversityfin = new List<double>();

            distances.Add(new List<double>());

            int dimension = DesignMap[0].Count;

            diversityfin.Clear();
            diversityfin.Add(0.0);
            diversityfin.Add(0.0);
            diversityfin.Add(0.0);
            diversityfin.Add(0.0);

            // Find centroid
            for (int i = 0; i < dimension; i++)
            {
                List<double> coordlist = new List<double>();

                for (int j = 0; j < DesignMap.Count; j++)
                {
                    coordlist.Add(DesignMap[j][i]);
                }

                centroid.Add(coordlist.Average());
                coordlist.Sort();
                median.Add(coordlist[coordlist.Count / 2]);
            }



            // Compare distance
            for (int i = 0; i < DesignMap.Count; i++)
            {

                distances.Add(new List<double>());

                List<double> coorddif = new List<double>();
                List<double> coorddif2 = new List<double>();

                for (int j = 0; j < dimension; j++)
                {
                    coorddif.Add((DesignMap[i][j] - centroid[j]) * (DesignMap[i][j] - centroid[j]));
                    coorddif2.Add((DesignMap[i][j] - median[j]) * (DesignMap[i][j] - median[j]));
                }

                double sum = coorddif.Sum(x => Convert.ToDouble(x));
                double sumcent = coorddif2.Sum(x => Convert.ToDouble(x));

                distances[0].Add(Math.Sqrt(sum));
                distances[1].Add(Math.Sqrt(sumcent));

            }


            // DataTree<double> tree = new DataTree<double>();
            // DataTree<double> tree2 = new DataTree<double>();


            diversity.Add(distances[0].Average());

            // add distance calculations to diversity list
            diversityfin[1] = distances[0].Max();
            diversityfin[2] = distances[0].Average();
            diversityfin[3] = distances[1].Average();

            double divsum = 0;
            for (int i = 1; i < 4; i++)
            { divsum = divsum + diversityfin[i]; }

            diversityfin[0] = divsum / 3;

            return diversityfin[mode];

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

        static List<double> StructureToList(GH_Structure<GH_Number> structure)
        {
            List<double> list = new List<double>();
            foreach (GH_Path p in structure.Paths)
            {
                List<GH_Number> l = (List<GH_Number>)structure.get_Branch(p);
                
                foreach (GH_Number n in l)
                {
                    double d = 0;
                    n.CastTo<double>(out d);
                    list.Add(d);
                }
                
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

        static DataTree<T> ListToTree<T>(List<T> List)
        {
            DataTree<T> tree = new DataTree<T>();
            for (int i = 0; i < List.Count; i++)
            {
                tree.Add(List[i], new GH_Path(i));
            }
            return tree;
        }

        public static bool Run(Grasshopper.Kernel.IGH_DataAccess DA, int index)
        {
            bool run = false;
            DA.GetData<bool>(index, ref run);
            return run;
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
                return Properties.Resources.Diversity;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{e9d4496e-8582-442f-b0c0-3dca864ce83e}"); }
        }
    }
}
