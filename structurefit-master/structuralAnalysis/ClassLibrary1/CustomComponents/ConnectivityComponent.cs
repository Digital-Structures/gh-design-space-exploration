using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Rhino;

namespace GrasshopperImplementation.CustomComponents
{
    public class ConnectivityComponent : GH_Component
    {
        public ConnectivityComponent() : base("Connectivity","CONN", "Connectivity Builder","structureFIT","Assembly")
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("Lines", "L", "Line(s) Representing the Structure", GH_ParamAccess.list);
            // pManager.AddNumberParameter("Tolerance", "T", "Tolerance for Connectivity Assembly", GH_ParamAccess.item, 0.01);
            // Create a Material Parameter
            pManager.AddNumberParameter("Young", "E", "Young's Modulus", GH_ParamAccess.item);
            pManager.AddNumberParameter("Poisson", "nu", "Poisson Ratue", GH_ParamAccess.item);
            pManager.AddNumberParameter("Young", "E", "Young's Modulus", GH_ParamAccess.item);

        }
                protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
                    // change to assembled model parameter
            /*sN = list of structural nodes (duplicates are culled) */
            pManager.AddPointParameter("Nodes","sN", "Model Node Coordinates",GH_ParamAccess.list);

            /*The following does not need to be an output of the component, now it is to check if the component works*/
            pManager.AddMatrixParameter("Node Matrix", "COOR", "Node Matrix  for Structural Analysis", GH_ParamAccess.item);
            // pManager.AddNumberParameter("Element Matrix", "MMEM", "Element Matrix for Structural Analysis", GH_ParamAccess.list);
        }
            protected override void SolveInstance(IGH_DataAccess DA)
        {
            /* Declare variables to contain all inputs
             * We can assign initial values (or not) that are either indicative or sensible */
            List<Rhino.Geometry.Line> lines = new List<Rhino.Geometry.Line>() ;
            double tolerance = double.NaN ;

            /* Use the DA object to retrieve the data inside the input parameters
             * If the retrieval fails (e.g. there is no data), abort*/
            if (!DA.GetDataList(0, lines)) { return; }
            if (!DA.GetData(1, ref tolerance)) { return; }

            /* If the tolerance is not a valid number, abort */
            if (!Rhino.RhinoMath.IsValidDouble(tolerance)) { return; }

            /*Retrieve coordinates of start and end points*/
            List<Rhino.Geometry.Point3d> pts = new List<Rhino.Geometry.Point3d>();
            List<Node> Nodes = new List<Node>();
            List<Member> Members = new List<Member> (); 
            foreach (Rhino.Geometry.Line line in lines)
            {
                pts.Add(line.From);
                pts.Add(line.To);
                int nPts = pts.Count;
                Member m;
                Node n1;
                Node n2;
                int n1Index = nPts - 2;
                int n2Index = nPts - 1;

                // Define start node, eliminate dup if necessary

                for (int i = 0; i < nPts -2; i++)
                {
                    if (Rhino.Geometry.Point3d.Equals(pts[nPts-2],pts[i]))
                    {
                        pts.RemoveAt(nPts - 2);
                        n1Index = i;
                    }
                }

                DOF x1, y1, z1;
                x1.Coord = pts[n1Index].X;
                y1.Coord = pts[n1Index].Y;
                z1.Coord = pts[n1Index].Z;
                DOF[] coor1 = new DOF[] { x1, y1, z1 };
                n1.DOFs = coor1;
                n1.Index = n1Index;

                // Define start node, eliminate dup if necessary

                for (int i = 0; i < nPts - 1; i++)
                {
                    if (Rhino.Geometry.Point3d.Equals(pts[nPts - 1], pts[i]))
                    {
                        pts.RemoveAt(nPts - 1);
                        n2Index = i;
                    }
                }

                DOF x2, y2, z2;
                x2.Coord = pts[n2Index].X;
                y2.Coord = pts[n2Index].Y;
                z2.Coord = pts[n2Index].Z;
                DOF[] coor2 = new DOF[] { x2, y2, z2 };
                n2.DOFs = coor2;
                n2.Index = n2Index;

                // Define member

                m.NodeI = n1;
                m.NodeJ = n2;

                // Add member and nodes to respective lists

                Nodes.Add(n1);
                Nodes.Add(n2);
                Members.Add(m);

            }

            
            /*Assign the outputs via the DA object*/
            DA.SetDataList(0, pts);
            DA.SetDataList(1, COOR);
            
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("ACE4C05F-25EE-4F46-9466-A9E24B4577BB"); }
        }
    }
}
