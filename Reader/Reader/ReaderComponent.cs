using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel.Data;
using System.Windows.Forms;


namespace Reader
{
    public class ReaderComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ReaderComponent()
          : base("Reader", "Reader",
              "Reads in .CSV files as nested lists",
              "DSE", "Main")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddTextParameter("Filepath", "Dir + F", "Csv filepath", GH_ParamAccess.item);
            pManager.AddTextParameter("Separator", "S", "Character used to separate data", GH_ParamAccess.item, " ");

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Data", "D", "Output data", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            // First, we need to retrieve all data from the input parameters.
            // We'll start by declaring variables and assigning them starting values.
            string path = null;
            string separator = null;
            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetData(0, ref path)) return;
            if (!DA.GetData(1, ref separator)) return;

            // We're set to create the spiral now. To keep the size of the SolveInstance() method small, 
            // The actual functionality will be in a different method:

            char sep = separator.ToCharArray()[0];

            System.IO.StreamReader file = new System.IO.StreamReader(path);
            List<List<string>> tree = new List<List<string>>();

            while (!file.EndOfStream)
            {
                List<string> currentLine = new List<string>();

                string line = file.ReadLine().Trim();
                string[] dataLine = line.Split(sep);

                for (int i = 0; i < dataLine.Length; i++)
                {
                    currentLine.Add(dataLine[i].Trim());
                }

                tree.Add(currentLine);
            }
            // Finally assign the spiral to the output parameter.
            DA.SetDataTree(0, ListOfListsToTree<string>(tree));
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
            get { return new Guid("{641f736c-7655-44ca-b060-b1546b714cc9}"); }
        }
    }
}
