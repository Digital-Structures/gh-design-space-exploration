using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;

namespace Writer
{
    public class Writer : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Writer()
          : base("Writer", "Writer",
              "Writes a nested list to a .csv file in the specified directory.",
              "DSE", "Catalog")
        {


        }


        public string CSVFilename;
        public bool WriterDone;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddNumberParameter("Data", "D", "Data to be written to a .csv", GH_ParamAccess.tree);
            pManager.AddTextParameter("Filepath", "Dir + F", "Csv filepath", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {


            var map = new GH_Structure<GH_Number>();
            if (!DA.GetDataTree(0, out map)) return;
            if (!DA.GetData(1, ref CSVFilename)) return;

            //if (WriterDone)
            //{

                WriteOutputToFile(Writer.StructureToListOfLists(map), CSVFilename);
            //}

                

        
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

        private void WriteOutputToFile(List<List<double>> output, string CSVFilename)
        {
            string a = null;
            for (int i = 0; i < output.Count; i++)
            {
                string b = null;
                for (int j = 0; j < output[i].Count; j++)
                {
                    b = b + output[i][j] + ",";
                }
                a = a + b + "\r\n";
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter(@"" + CSVFilename + ".csv");
            file.Write(a);
            file.Close();
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
                return Properties.Resources.Writer1; ;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{9a96ccab-b567-4266-8cd1-daa21abf0cf4}"); }
        }
    }
}
