using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StormCloud.Parameters;
using StructureEngine.Model;

namespace StormCloud.Components
{
    public class LoadComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PointLoad class.
        /// </summary>
        public LoadComponent() : base("Point Load", "L","Creates Point Load Applied on Structural Node","StormCloud", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Structure to Load
            pManager.AddParameter(new Parameters.StructureParameter());
            // Loaded Nodes
            pManager.AddPointParameter("Point", "Pt", "Loaded Point", GH_ParamAccess.list);
            // Load Vector
            pManager.AddVectorParameter("Load", "L", "Load Vector [kN]", GH_ParamAccess.item);
            // LoadCase Name
            pManager.AddTextParameter("Loadcase", "LC", "Loadcase name", GH_ParamAccess.item);
            // Material
            // Section
            // StructureType
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Loaded Structure
            pManager.AddParameter(new StructureParameter());
            // Info
            //pManager.AddTextParameter("Information", "Info", "Feedback on Loading Operation", GH_ParamAccess.item);
        }

        /// <summary>
        /// Add Loads to ComputedStructure
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // input
            StructureEngine.Model.ComputedStructure structure = new StructureEngine.Model.ComputedStructure(); // modify to get in solver to get output structure
            List<Point3d> points = new List<Point3d>();
            Vector3d forcevector = new Vector3d();
            String lcname = "";
            Types.StructureType structure1 = new Types.StructureType();
            
            if (!DA.GetData(0, ref structure1)) {return;}
            if (!DA.GetDataList(1, points)) { return; }
            if (!DA.GetData(2, ref forcevector)) { return; }
            if (!DA.GetData(3, ref lcname)) { return; }

            structure1.CastTo<ComputedStructure>(ref structure);

            structure = structure1.Value;


            // output info

            //String info = "Loading Operation Successful";

            // Create new Loadcase

            LoadCase lc = new LoadCase(lcname);

            double lx = forcevector.X;
            double ly = forcevector.Y;
            double lz = forcevector.Z;

            
            foreach (Point3d point in points)
            {
                foreach (StructureEngine.Model.Node node in structure.Nodes)
                {
                    if (node.IsAtPoint(point))
                    {
                        Load loadx = new Load(lx, lc, node.DOFs[0]);
                        Console.WriteLine(loadx.Value);
                        Load loady = new Load(ly, lc, node.DOFs[1]);
                        Console.WriteLine(loady.Value);
                        Load loadz = new Load(lz, lc, node.DOFs[2]);
                        Console.WriteLine(loadz.Value);
                        lc.Loads.Add(loadx);
                        lc.Loads.Add(loady);
                        lc.Loads.Add(loadz);
                    }
                }
                //points.Remove(point); collection was modified each time -> error
            }

            // Add Loadcase to structure
            structure.LoadCases.Add(lc);

            /*Assign the outputs via the DA object*/
            Types.StructureType Structure_GH = new Types.StructureType(structure);
            DA.SetData(0, Structure_GH);
            //DA.SetData(1, info);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                //return Resources.Resources.load;
                return Resources.Resources.gen_icon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        /// 
        public override Guid ComponentGuid
        {
            get { return new Guid("{a7d699c1-9d6f-4d3e-a80a-df639edb180a}"); }
        }
    }
}