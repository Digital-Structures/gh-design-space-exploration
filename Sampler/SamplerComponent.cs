﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Sampler
{
    public class SamplerComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public SamplerComponent()
          : base("Sampler", "Nickname",
              "Description",
              "DSE", "Main")
        {
            this.Util = new SamplerUtilities(this);
            this.MyRand = new Random();
        }

        // Properties specific to this component:

        private List<Grasshopper.Kernel.Special.GH_NumberSlider> SlidersList;
        public List<DSEVariable> VarsList;
        public string Prefix, Path;
        public int Scheme, NSamples, Seed;
        public SamplerUtilities Util;
        public List<List<double>> Output;
        private IGH_DataAccess DAC;
        public Random MyRand;

        /// <summary>
        /// Creates custom attributes for this component.
        /// </summary>
        public override void CreateAttributes()
        {
            base.m_attributes = new SamplerComponentAttributes(this);
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
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
            get { return new Guid("{3fb887e3-6af7-4c40-9e49-a2cc46a88cb9}"); }
        }
    }
}
