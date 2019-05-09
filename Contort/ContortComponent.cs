using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Rhino.Geometry;

using Grasshopper.Kernel.Data;

using DSECommon;
using GH_IO;
using Grasshopper;

using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino.Collections;

using GH_IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System.Drawing;


// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace Contort
{
    public class ContortComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ContortComponent()
          : base("Contort", "Contort",
              "Explore design space using synthetic variables mapped back to the original variables",
              "DSE", "Optimize")
        {

            this.SlidersList = new List<GH_NumberSlider>();
            this.Coeff = new List<List<double>>();
            this.SynthVals = new List<double>();
        }


        public int NumVars, NumSynths;
        public double Scale;
        public bool MakeSliders;
        public bool run;
        public List<GH_NumberSlider> SlidersList;
        public List<List<double>> Coeff;
        public List<double> SynthVals;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            // Check that number of sliders is equal to number of variables in DM; otherwise, throw an error.
            pManager.AddNumberParameter("Variables", "Var", "Sliders representing variables", GH_ParamAccess.list);
            pManager.AddNumberParameter("Coefficients", "Coeff", "Coefficents for mapping to synthetic variables; each (nested) list of coefficients must equal the number of variables", GH_ParamAccess.tree);            
            pManager.AddNumberParameter("Synthetic Sliders", "SynthSlide", "Synthetic sliders to control your overall design; must match number of coefficient lists", GH_ParamAccess.list, 0);
            pManager.AddNumberParameter("Scale", "Scale", "Scale of how far to move in the design space", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Create Sliders", "CreateSliders", "Click button to create sliders", GH_ParamAccess.item,false);

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
            //Get Inputs

            this.Coeff.Clear();
            this.SynthVals.Clear();

            run = false;

            readSlidersList();

            var RawCoeff = new GH_Structure<GH_Number>();
            if (!DA.GetDataTree(1, out RawCoeff)) return;
            this.Coeff = StructureToListOfLists(RawCoeff);

            //DA.GetDataList<List<double>>(1, this.Coeff);

            DA.GetDataList<double>(2, this.SynthVals);
            DA.GetData(3, ref Scale);
            DA.GetData(4, ref MakeSliders);


            if (MakeSliders)
            {

                for (int i = 0; i < NumSynths; i++)
                {

                    //instantiate objective sliders
                    Grasshopper.Kernel.Special.GH_NumberSlider slid = new Grasshopper.Kernel.Special.GH_NumberSlider();
                    slid.CreateAttributes(); //sets up default values, and makes sure your sli


                    //customise slider (position, ranges etc)
                    slid.Attributes.Pivot = new PointF((float)this.Attributes.DocObject.Attributes.Bounds.Left - slid.Attributes.Bounds.Width - 30, (float)this.Params.Input[2].Attributes.Bounds.Y + i * 30);
                    slid.Slider.Maximum = 1;
                    slid.Slider.Minimum = -1;
                    slid.Slider.DecimalPlaces = 3;
                    slid.SetSliderValue((decimal)0);

                    // This command makes it 'real' and adds it to the canvas.
                    Grasshopper.Instances.ActiveCanvas.Document.AddObject(slid, false);
                    //Connect the new slider to this component
                    this.Params.Input[2].AddSource(slid);

                }
            }


            NumVars = SlidersList.Count;
            NumSynths = Coeff.Count;

            for (int i = 0; i < NumVars; i++)
            {

                var slider = SlidersList[i] as Grasshopper.Kernel.Special.GH_NumberSlider; //try to cast that thing as a slider
                decimal mid = (slider.Slider.Maximum + slider.Slider.Minimum) / 2;
                decimal range = slider.Slider.Maximum - slider.Slider.Minimum;

                decimal value = mid;

                for (int j = 0; j < NumSynths; j++)
                {

                    value = value + (decimal)SynthVals[j] * (decimal) Coeff[j][i] * range/2 * (decimal) Scale;

                }

                if (slider != null) //if the component was successfully cast as a slider
                {
                    slider.SetSliderValue(value);
                }
            }

            run = true;

            if(run)
            {
                RefreshSliders();
            }

            

            int test = 0;

        }


        private void readSlidersList()
        {
            
            this.SlidersList = new List<GH_NumberSlider>();
            int nVars = this.Params.Input[0].Sources.Count;

            NumVars = nVars;

            for (int i = 0; i < nVars; i++)
            {
                this.SlidersList.Add(this.Params.Input[0].Sources[i] as GH_NumberSlider);
            }

            foreach (GH_NumberSlider slider in SlidersList)
            {
                DSEVariable newVar = new DSEVariable((double)slider.Slider.Minimum, (double)slider.Slider.Maximum, (double)slider.Slider.Value);
          
            }
        }

        private void RefreshSliders()
        {
            Grasshopper.Instances.ActiveCanvas.Document.ScheduleSolution(1);
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
                return Contort.Properties.Resources.Contort1; 
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5e040c37-bab0-4c27-a693-c0e63e8e65e7"); }
        }

        public object GH_Document { get; private set; }
    }
}


