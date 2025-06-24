using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using StormCloud.ViewModel;
using StormCloud.Evolutionary;
using Grasshopper.Kernel.Special;

namespace StormCloud
{
    public class InterOptComponent : GH_Component
    {

        public InterOptComponent(): base("StormCloud", "StormCloud", "Interactive Evolutionary Optimization", "DSE", "StormCloud")
        {
            Score = 0;
            DesignLines = new List<Line>();
            DesignCurves = new List<Curve>();
            DesignMeshes = new List<Mesh>();
            DesignBreps = new List<Brep>();
            DesignView = new DesignToolVM();
        }

       

        // Different geometries owned by the design
        public List<Line> DesignLines;
        public List<Curve> DesignCurves;
        public List<Mesh> DesignMeshes;
        public List<Brep> DesignBreps;

        // Datacontext for the window
        public DesignToolVM DesignView;

        // Design Score
        public double Score;

        // Design Variables
        public List<DesignVar> DesignVariables
        {
            get
            {
                List<DesignVar> dvars = new List<DesignVar>();
                foreach (IGH_Param param in this.Params.Input[2].Sources) // dvar is input 2
                {                    
                    GH_NumberSlider slider = param as GH_NumberSlider;
                    if (slider != null)
                    {
                        double val = (double)slider.CurrentValue;
                        double min = (double)slider.Slider.Minimum;
                        double max = (double)slider.Slider.Maximum;
                        DesignVar dvar = new DesignVar(val, min, max);
                        dvars.Add(dvar);
                    }
                }
                return dvars;
            }
            private set { }
        }

        // Design
        public Design Design
        {
            get
            {
                return new Design(this.DesignVariables);
            }
            private set {}
        }


        public override void CreateAttributes()
        {
            base.m_attributes = new InterOptComponentAttributes(this);
        }

        // INPUT PARAMETERS
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Using Geometry Parameter allows to use any rhino as an input for the design geometry
            pManager.AddGeometryParameter("Geometry (Breps, Curves and Lines)", "G", "Design Geometry", GH_ParamAccess.list);
            // Score of the design
            pManager.AddNumberParameter("Score", "S", "Design Score", GH_ParamAccess.item);
            // Slider modifiable by the evolutionary engine
            pManager.AddNumberParameter("Design Variables", "DVar", "Design Variables To Be Considered in the Interactive Evolutionary Optimization", GH_ParamAccess.list);
        }

        // NO OUTPUT PARAMETER
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        // SOLVER
        // Retrieves data from Grasshopper and changes component properties that can be accessed in the code-behind of stormcloud.xaml
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //// GEOMETRY
            //// Use IGH_GeometricGoo to deal with different geometry types
            List<IGH_GeometricGoo> g = new List<IGH_GeometricGoo>();
            if (!DA.GetDataList<IGH_GeometricGoo>(0, g)) { return; } // retrieves list of geometries as list of IGH_GeometricGoo
            List<Curve> curves = new List<Curve>();
            List<Mesh> meshes = new List<Mesh>();
            List<Brep> breps = new List<Brep>();

            foreach (IGH_GeometricGoo goo in g)
            {
                GH_Line l = goo as GH_Line;
                if (l!=null)
                {
                    GH_Curve lc = new GH_Curve();
                    //l.CastTo<GH_Curve>(out lc);
                    
                    if (GH_Convert.ToGHCurve(l, GH_Conversion.Both, ref lc))
                    {
                        curves.Add(lc.DuplicateCurve().Value);

                    }
                    else { 

                        Console.WriteLine("Conversion failed");
                        curves.Add(lc.DuplicateCurve().Value);

                    }
                }
                GH_Curve c = goo as GH_Curve; // c = null if geometry is not a curve
                if (c != null)
                {
                    curves.Add(c.DuplicateCurve().Value); // add to curves if geometry is a curve
                    Console.WriteLine("There is a Curve");
                }
                GH_Mesh m = goo as GH_Mesh;
                if (m != null)
                {
                    GH_Brep br = new GH_Brep();
                   // m.CastTo<GH_Brep>(out br);
                   
                    if (GH_Convert.ToGHBrep(l, GH_Conversion.Both, ref br))
                    {
                        breps.Add(br.DuplicateBrep().Value);

                    }
                    else
                    {
                        Console.WriteLine("Conversion failed");
                        breps.Add(br.DuplicateBrep().Value);
                    }
                }

                //GH_Surface s = goo as GH_Surface;
                //if (s != null)
                //{
                //    GH_Brep brs = new GH_Brep();
                //    s.CastTo<GH_Brep>(out brs);
                //    breps.Add(brs.DuplicateBrep().Value);
                //}

                GH_Brep b = goo as GH_Brep;
                if (b != null)
                {
                    breps.Add(b.DuplicateBrep().Value);
                }
            }



            //// Change values of relevant component properties

            DesignCurves = curves;
            DesignMeshes = meshes;
            DesignBreps = breps;

            // DesignLines
            //List<Line> lines = new List<Line>();
            //if (!DA.GetDataList(0, lines)) { return; }
            //DesignLines = lines;
            // SCORE
            double score = 0; // instantiate score
            if (!DA.GetData(1, ref score)) { return; }
            Score = score; // Change value of component property

            // Update model of main 3D viewports in stormcloud
            if (this.DesignView.InitialDesign.Score !=0)
            {
                DesignView.UpdateCurrentScore(Score);
            }
            else { DesignView.UpdateCurrentScore(1.00); }
            //DesignView.UpdateCurrentModel(DesignLines, RenderingSettings.diameter, RenderingSettings.resolutiontube, RenderingSettings.mat);
            DesignView.UpdateCurrentModelAdvanced(DesignCurves, DesignMeshes, DesignBreps, RenderingSettings.diameter, RenderingSettings.resolution, RenderingSettings.resolutiontube, RenderingSettings.mat);

        }

        // Icon
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resources.Resources.gen_icon_small;
            }
        }

        // Guid
        public override Guid ComponentGuid
        {
            get { return new Guid("{8310F3DB-CDA7-44F9-9E6B-D84B58821CC2}"); }
        }
    }
}
