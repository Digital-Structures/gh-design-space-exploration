using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StructureEngine.Model;
using StormCloud;

using System.Windows.Forms;  
using System.Text;  
using System.IO;  
using System.Runtime.InteropServices;  
using Microsoft.Win32.SafeHandles;

namespace StormCloud
{
    public class ConnectivityComponent : GH_Component
    {

        //OPEN CONSOLE

        //[DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //private static extern IntPtr GetStdHandle(int nStdHandle);
        //[DllImport("kernel32.dll",
        //    EntryPoint = "AllocConsole",
        //    SetLastError = true,
        //    CharSet = CharSet.Auto,
        //    CallingConvention = CallingConvention.StdCall)]
        //private static extern int AllocConsole();
        //private const int STD_OUTPUT_HANDLE = -11;
        //private const int MY_CODE_PAGE = 437;  



        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ConnectivityComponent() : base("Assembly", "A", "Assembles the Structure", "StormCloud", "Analysis")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("Lines", "L", "Line(s) Representing the Structure", GH_ParamAccess.list);
            pManager.AddParameter(new Parameters.MaterialParameter());
            pManager.AddTextParameter("Section", "S", "Section Type for the Structure: Rod or Square",GH_ParamAccess.item);
            pManager.AddBooleanParameter("IsFrame?", "Frame?", "Type of Structure", GH_ParamAccess.item);

            //pManager.AddParameter(new CustomParameters.SectionParameter());
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // change to assembled model parameter
            /*sN = list of structural nodes (duplicates are culled) */
            pManager.AddPointParameter("Nodes", "sN", "Model Node Coordinates", GH_ParamAccess.list);
            pManager.AddParameter(new Parameters.StructureParameter());

            /*The following does not need to be an output of the component, now it is to check if the component works*/
            // pManager.AddMatrixParameter("Node Matrix", "COOR", "Node Matrix  for Structural Analysis", GH_ParamAccess.item);
            // pManager.AddNumberParameter("Element Matrix", "MMEM", "Element Matrix for Structural Analysis", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {

            /* Declare variables to contain all inputs
             * We can assign initial values (or not) that are either indicative or sensible */
            //AllocConsole();

            List<Rhino.Geometry.Line> lines = new List<Line>();
            Types.MaterialType materialtype = new Types.MaterialType();
            String sectiontype = "";

            Boolean isframe = true;

            /* Use the DA object to retrieve the data inside the input parameters
             * If the retrieval fails (e.g. there is no data), abort*/
            if (!DA.GetDataList(0,lines)) { return; }
            if (!DA.GetData(1, ref materialtype)) { return; }
            if (!DA.GetData(2, ref sectiontype)) { return; }
            if (!DA.GetData(3, ref isframe)) { return; }


            // material

            StructureEngine.Model.Material material = materialtype.Value;

            // section
            ISection SectionType = new RodSection();
            if (sectiontype == "Square")
            {
                SectionType = new SquareSection();
            }

            //if (!DA.GetData(1, ref Material)) { return; }
            //if (!DA.GetData(2, ref Section)) { return; }
            // if (!DA.GetData(1, ref tolerance)) { return; }

            /* If the tolerance is not a valid number, abort */
            // if (!Rhino.RhinoMath.IsValidDouble(tolerance)) { return; }

            /*Retrieve coordinates of start and end points*/
            List<Rhino.Geometry.Point3d> pts = new List<Rhino.Geometry.Point3d>();
            List<Node> Nodes = new List<Node>();
            List<Member> Members = new List<Member>();


            foreach (Line line in lines)
            {
                Point3d start = line.From;
                Point3d end = line.To;




                int index1 = Nodes.FindIndex(n => n.IsAtPoint(start));
                int index2 = Nodes.FindIndex(n => n.IsAtPoint(end));



                Node n1 = new Node(start, isframe);

                Node n2 = new Node(end, isframe);
 
                if (index1 ==-1 && index2==-1)
                {
                    n1.SetIndex(Nodes.Count);
                    Nodes.Add(n1);

                    n2.SetIndex(Nodes.Count+1);
                    Nodes.Add(n2);
                }

                else if (index1!=-1 && index2 == -1)
                {
                    n1 = Nodes[index1];
                    
                    n2.SetIndex(Nodes.Count);
                    Nodes.Add(n2);
                }
                
                else if (index1==-1 && index2 != -1)
                {

                    n1.SetIndex(Nodes.Count);
                    Nodes.Add(n1);
                    
                    n2 = Nodes[index2];
                }

                if (!(index1 !=-1 && index2 !=-1))
                {
                    Member m = new Member(n1, n2);

                    m.Material = material;
                    m.SectionType = SectionType;

                    //m.Material = Material.MaterialValue;
                    //m.Section = Section.SectionValue;

                    // Add member and nodes to respective list
                    Members.Add(m);                    
                }

            }





            //Console.WriteLine(5);

            //foreach (Rhino.Geometry.Line line in lines)
            //{
            //    pts.Add(line.From);
            //    pts.Add(line.To);

            //    int n1Index = pts.Count - 2;
            //    int n2Index = pts.Count - 1;

            //    // Define start node, eliminate dup if necessary

            //    for (int i = 0; i < pts.Count - 2; i++) // < should become <= (or not?)
            //    {
            //        if (Rhino.Geometry.Point3d.Equals(pts[pts.Count - 2], pts[i]))
            //        {
            //            pts.RemoveAt(pts.Count - 2);
            //            n1Index = i;
            //            n2Index = n2Index - 1; // test
            //        }
            //    }

                

            //    DOF x1 = new DOF(pts[n1Index].X);
            //    DOF y1 = new DOF(pts[n1Index].Y);
            //    DOF z1 = new DOF(pts[n1Index].Z);

            //    DOF[] coor1 = new DOF[] { x1, y1, z1, new DOF(0,isframe), new DOF(0,isframe), new DOF(0,isframe) };

            //    Node n1 = new Node(coor1);
            //    n1.Index = n1Index;

                // check that it is a new node


                // Define end node, eliminate dup if necessary

            //    for (int i = 0; i < pts.Count - 1; i++)
            //    {
            //        if (Rhino.Geometry.Point3d.Equals(pts[pts.Count - 1], pts[i]))
            //        {
            //            pts.RemoveAt(pts.Count - 1);
            //            n2Index = i;
            //        }
            //    }

            //    DOF x2 = new DOF(pts[n2Index].X);
            //    DOF y2 = new DOF(pts[n2Index].Y);
            //    DOF z2 = new DOF(pts[n2Index].Z);

            //    DOF[] coor2 = new DOF[] {x2, y2, z2, new DOF(0, isframe), new DOF(0, isframe), new DOF(0, isframe)};

            //    Node n2 = new Node(coor2);
            //    n2.Index = n2Index;

            //    // Define member

            //    Member m = new Member(n1, n2);

            //    m.Material = material;
            //    m.SectionType = SectionType;

            //    //m.Material = Material.MaterialValue;
            //    //m.Section = Section.SectionValue;

            //    // Add member and nodes to respective list

            //    Nodes.Add(n1);
            //    Nodes.Add(n2);
            //    Members.Add(m);

            //}


            Structure Structure = new Structure(Nodes, Members);

            ComputedStructure CompStructure = new ComputedStructure(Structure);

            Types.StructureType Structure_GHrep = new Types.StructureType(CompStructure);

            /*Assign the outputs via the DA object*/
            DA.SetDataList(0, pts);

            DA.SetData(1, Structure_GHrep);

        }

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
            get { return new Guid("{8a621274-9035-4973-988b-2608ac106edf}"); }
        }
    }
}
