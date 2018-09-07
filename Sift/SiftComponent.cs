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
using DSECommon;

namespace Sift
{
    public class SiftComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public SiftComponent()
          : base("Sift", "Sift",
              "Selects specific designs from a design map. WORKS ON DOUBLECLICK",
              "DSE", "Catalog")
        {
            this.VarsList = new List<DSEVariable>();
            this.ObjInput = new List<double>();
            this.SlidersList = new List<GH_NumberSlider>();
            this.DesignMap = new List<List<double>>();
            this.ObjValues = new List<List<double>>();
          
        }

       
        public List<List<double>> ObjValues;
        public List<DSEVariable> VarsList;
        public List<GH_NumberSlider> SlidersList;
        public List<double> ObjInput;
        public List<List<double>> DesignMap;      
        public int NumVars, NumObjs;
        public int Index;

        public override void CreateAttributes()
        {
            base.m_attributes = new SiftComponentAttributes(this);
        }



        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            // Check that number of sliders is equal to number of variables in DM; otherwise, throw an error.
            pManager.AddNumberParameter("Variables", "Var", "Sliders representing variables", GH_ParamAccess.list);
            pManager.AddNumberParameter("Design map", "DM", "Set of design variable settings", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Index", "I", "Index in design map of desired design", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Design Vector", "DesVec", "Design vector of sifted design", GH_ParamAccess.tree);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {


            //Take in inputs
            this.readSlidersList();

            var map = new GH_Structure<GH_Number>();
            if (!DA.GetDataTree(1, out map)) return;
            this.DesignMap = StructureToListOfLists(map);

            if (!DA.GetData(2, ref Index)) return;

            // insert warnings
            int NumVars = this.Params.Input[0].Sources.Count;

            if (NumVars != this.DesignMap[0].Count)
            {
                this.AddRuntimeMessage((GH_RuntimeMessageLevel)20, "Number of sliders and design map do not match; please check");
            }

            //Set Current slider list

            //Set Outputs
            var designVector = new List<double>();
            for (int i = 0; i < this.DesignMap[Index].Count; i++)
            {
               designVector.Add(this.DesignMap[Index][i]);
            }
          
            DA.SetDataTree(0, ListToTree<double>(designVector));
            
            


        }

        private void readSlidersList()
        {
            this.VarsList.Clear();
            this.SlidersList = new List<GH_NumberSlider>();
            int nVars = this.Params.Input[0].Sources.Count;
            for (int i = 0; i < nVars; i++)
            {
                if (this.Params.Input[0].Sources[i].Name != "Number Slider")
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One or more sliders are invalid");
                    return;
                }

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


        static DataTree<T> ListToTree<T>(List<T> List)
        {
            DataTree<T> tree = new DataTree<T>();
            for (int i = 0; i < List.Count; i++)
            {
                tree.Add(List[i]);
            }
            return tree;
        }


        static List<List<T>> TreeToListOfLists<T>(DataTree<T> tree)
        {
            List<List<T>> list = new List<List<T>>();
            foreach (GH_Path p in tree.Paths)
            {
                List<T> l = tree.Branch(p);
                list.Add(l);
            }
            return list;
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
                return Sift.Properties.Resources.sift1; ;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{40e510dd-1026-4500-bbfa-764bd2053a5b}"); }
        }
    }
}
