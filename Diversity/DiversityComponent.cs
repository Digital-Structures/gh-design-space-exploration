using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Accord.Math;


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
            this.centroid = new List<double>();
            this.distances = new List<List<double>>();
            this.diversity = new List<double>();

        }


        public List<List<double>> DesignMap;
        public List<double> centroid;
        public List<List<double>> distances;
        public List<double> diversity;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddNumberParameter("Design map", "DM", "Set of design vectors for calculating set diversity", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Mode", "M", "Diversity measurement type. Right click to choose type.", GH_ParamAccess.item, 0);

            // Add possible values for the mode input
            Param_Integer param = (Param_Integer)pManager[1];
            param.AddNamedValue("Average [0]", 0);
            param.AddNamedValue("Sparseness (avg) [1]", 1);
            param.AddNamedValue("Sparseness (med) [2]", 2);
            param.AddNamedValue("Outlier [3]", 3);
            param.AddNamedValue("Enclosing Ball [4]", 2);
            param.AddNamedValue("Convex Hull [5]", 3);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("Diversity", "Div", "Measured diversity of set using selected metric", GH_ParamAccess.tree);
            pManager.AddTextParameter("Test", "Test", "Test", GH_ParamAccess.tree);

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

            int dimension = this.DesignMap[0].Count;

            // Find centroid
            for (int i = 0; i < dimension; i++)
            {
                List<double> coordlist = new List<double>();

                for (int j = 0; j <this.DesignMap.Count; j++)
                {
                    coordlist.Add(this.DesignMap[j][i]);
                } 
            
                centroid.Add(coordlist.Average());
            }

            distances.Add(new List<double>());

            // Compare distance
            for (int i = 0; i < DesignMap.Count; i++)
            {
                double[] point1 = centroid.ToArray();
                double[] point2 = DesignMap[i].ToArray();

                distances.Add(new List<double>());

                distances[0].Add(Accord.Math.Distance.Euclidean(point1, point2));
            }



            DataTree<double> tree = new DataTree<double>();
            DataTree<double> tree2 = new DataTree<double>();

            diversity.Add(this.distances[0].Average());

            
            
            tree.Add(diversity[mode], new GH_Path());
            tree2.AddRange(distances[0], new GH_Path());


            DA.SetDataTree(0, tree);
            DA.SetDataTree(1, tree2);


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
