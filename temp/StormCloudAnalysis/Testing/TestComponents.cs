using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StructureEngine.Model;
using StructureEngine.Analysis;
using StormCloud;
//using StormCloudAnalysis.Types;

namespace Testing
{
    public class TestComponents
    {
        public static void Main()
        {
            /* Declare variables to contain all inputs
* We can assign initial values (or not) that are either indicative or sensible */
            List<Rhino.Geometry.Line> lines = new List<Line>();
            StormCloud.Types.MaterialType materialtype = new StormCloud.Types.MaterialType(); //PROBLEM HERE???
            String sectiontype = "";

            Line line1 = new Line(new Point3d(0, 0, 0), new Point3d(0, 1, 0));
            Line line2 = new Line(new Point3d(0, 0, 0), new Point3d(0, 0, 1));
            Line line3 = new Line(new Point3d(0, 0, 1), new Point3d(0, 0, 2));

            lines.Add(line1);
            lines.Add(line2);
            lines.Add(line3);





            // material

            StructureEngine.Model.Material material = new Material(29, 29, 29, 29, ""); // modify to get in solver to get output structure
            materialtype.CastTo<Material>(ref material);

            //if (!DA.GetData(1, ref Material)) { return; }
            //if (!DA.GetData(2, ref Section)) { return; }
            // if (!DA.GetData(1, ref tolerance)) { return; }

            /* If the tolerance is not a valid number, abort */
            // if (!Rhino.RhinoMath.IsValidDouble(tolerance)) { return; }

            /*Retrieve coordinates of start and end points*/
            List<Rhino.Geometry.Point3d> pts = new List<Rhino.Geometry.Point3d>();
            List<Node> Nodes = new List<Node>();
            List<Member> Members = new List<Member>();

            ISection SectionType = new RodSection();
            if (sectiontype == "Square")
            {
                SectionType = new SquareSection();
            }



            foreach (Rhino.Geometry.Line line in lines)
            {
                pts.Add(line.From);
                pts.Add(line.To);

                int n1Index = pts.Count - 2;
                int n2Index = pts.Count - 1;

                // Define start node, eliminate dup if necessary

                for (int i = 0; i < pts.Count - 2; i++) // < should become <= (or not?)
                {
                    if (Rhino.Geometry.Point3d.Equals(pts[pts.Count - 2], pts[i]))
                    {
                        pts.RemoveAt(pts.Count - 2);
                        n1Index = i;
                        n2Index = n2Index - 1; // test
                    }
                }

                DOF x1 = new DOF(pts[n1Index].X);
                DOF y1 = new DOF(pts[n1Index].Y);
                DOF z1 = new DOF(pts[n1Index].Z);

                DOF[] coor1 = new DOF[] { x1, y1, z1, new DOF(0), new DOF(0), new DOF(0) };

                Node n1 = new Node(coor1);
                n1.Index = n1Index;

                // Define end node, eliminate dup if necessary

                for (int i = 0; i < pts.Count - 1; i++)
                {
                    if (Rhino.Geometry.Point3d.Equals(pts[pts.Count - 1], pts[i]))
                    {
                        pts.RemoveAt(pts.Count - 1);
                        n2Index = i;
                    }
                }

                DOF x2 = new DOF(pts[n2Index].X);
                DOF y2 = new DOF(pts[n2Index].Y);
                DOF z2 = new DOF(pts[n2Index].Z);

                DOF[] coor2 = new DOF[] { x2, y2, z2, new DOF(0), new DOF(0), new DOF(0) };

                Node n2 = new Node(coor2);
                n2.Index = n2Index;

                // Define member

                Member m = new Member(n1, n2);

                m.Material = material;
                m.SectionType = SectionType;

                //m.Material = Material.MaterialValue;
                //m.Section = Section.SectionValue;

                // Add member and nodes to respective lists

                Nodes.Add(n1);
                Nodes.Add(n2);
                Members.Add(m);

            }

            Structure Structure = new Structure(Nodes, Members);
            ComputedStructure CompStructure = new ComputedStructure(Structure);
            Console.ReadLine();
            StormCloud.Types.StructureType Structure_GHrep = new StormCloud.Types.StructureType(CompStructure);  //PROBLEM HERE?????
            /*Assign the outputs via the DA object*/
            //DA.SetDataList(0, pts);
            //DA.SetData(1, Structure_GHrep);

 //           // CONNECTIVITY

 //           /* Declare variables to contain all inputs
 //* We can assign initial values (or not) that are either indicative or sensible */
 //           List<Rhino.Geometry.Line> lines = new List<Line>();
 //           Line line1 = new Line(new Point3d(0, 0, 0), new Point3d(0, 1, 0));
 //           Line line2 = new Line(new Point3d(0, 0, 0), new Point3d(0, 0, 1));
 //           Line line3 = new Line(new Point3d(0, 0, 1), new Point3d(0, 0, 2));

 //           lines.Add(line1);
 //           lines.Add(line2);
 //           lines.Add(line3);


 //           //CustomTypes.MaterialType Material;
 //           //CustomTypes.SectionType Section;
 //           // double tolerance = double.NaN ;



 //           /*Retrieve coordinates of start and end points*/
 //           List<Rhino.Geometry.Point3d> pts = new List<Rhino.Geometry.Point3d>();
 //           List<Node> Nodes = new List<Node>();
 //           List<Member> Members = new List<Member>();

 //           //////////////

 //           Material mat = new Material(29000, 0.3, 7500, 50, "steel");
 //           ISection SectionType = new RodSection();


 //           foreach (Rhino.Geometry.Line line in lines)
 //           {
 //               pts.Add(line.From);
 //               pts.Add(line.To);

 //               int n1Index = pts.Count - 2;
 //               int n2Index = pts.Count - 1;

 //               // Define start node, eliminate dup if necessary

 //               for (int i = 0; i < pts.Count - 2; i++) // < should become <= (or not?)
 //               {
 //                   if (Rhino.Geometry.Point3d.Equals(pts[pts.Count - 2], pts[i]))
 //                   {
 //                       pts.RemoveAt(pts.Count - 2);
 //                       n1Index = i;
 //                       n2Index = n2Index - 1; // test
 //                   }
 //               }

 //               DOF x1 = new DOF(pts[n1Index].X);
 //               DOF y1 = new DOF(pts[n1Index].Y);
 //               DOF z1 = new DOF(pts[n1Index].Z);

 //               DOF[] coor1 = new DOF[] { x1, y1, z1, new DOF(0), new DOF(0), new DOF(0) };

 //               Node n1 = new Node(coor1);
 //               n1.Index = n1Index;

 //               // Define end node, eliminate dup if necessary

 //               for (int i = 0; i < pts.Count - 1; i++)
 //               {
 //                   if (Rhino.Geometry.Point3d.Equals(pts[pts.Count - 1], pts[i]))
 //                   {
 //                       pts.RemoveAt(pts.Count - 1);
 //                       n2Index = i;
 //                   }
 //               }

 //               DOF x2 = new DOF(pts[n2Index].X);
 //               DOF y2 = new DOF(pts[n2Index].Y);
 //               DOF z2 = new DOF(pts[n2Index].Z);

 //               DOF[] coor2 = new DOF[] { x2, y2, z2, new DOF(0), new DOF(0), new DOF(0) };

 //               Node n2 = new Node(coor2);
 //               n2.Index = n2Index;

 //               // Define member

 //               Member m = new Member(n1, n2);

 //               m.Material = mat;
 //               m.SectionType = SectionType;

 //               //m.Material = Material.MaterialValue;
 //               //m.Section = Section.SectionValue;

 //               // Add member and nodes to respective lists

 //               Nodes.Add(n1);
 //               Nodes.Add(n2);
 //               Members.Add(m);

 //           }

 //           ////////////

 //           // Material


 //           //foreach (Rhino.Geometry.Line line in lines)
 //           //{
 //           //    pts.Add(line.From);
 //           //    pts.Add(line.To);

 //           //    int n1Index = pts.Count - 2;
 //           //    int n2Index = pts.Count - 1;

 //           //    // Define start node, eliminate dup if necessary

 //           //    for (int i = 0; i < pts.Count - 2; i++) // < should become <= (or not?)
 //           //    {
 //           //        if (Rhino.Geometry.Point3d.Equals(pts[pts.Count - 2], pts[i]))
 //           //        {
 //           //            pts.RemoveAt(pts.Count - 2);
 //           //            n1Index = i;
 //           //            n2Index = n2Index - 1; // test
 //           //        }
 //           //    }

 //           //    DOF x1 = new DOF(pts[n1Index].X);
 //           //    DOF y1 = new DOF(pts[n1Index].Y);
 //           //    DOF z1 = new DOF(pts[n1Index].Z);

 //           //    DOF[] coor1 = new DOF[] { x1, y1, z1 };

 //           //    Node n1 = new Node(coor1);
 //           //    n1.Index = n1Index;

 //           //    // Define end node, eliminate dup if necessary

 //           //    for (int i = 0; i < pts.Count - 1; i++)
 //           //    {
 //           //        if (Rhino.Geometry.Point3d.Equals(pts[pts.Count - 1], pts[i]))
 //           //        {
 //           //            pts.RemoveAt(pts.Count - 1);
 //           //            n2Index = i;
 //           //        }
 //           //    }

 //           //    DOF x2 = new DOF(pts[n2Index].X);
 //           //    DOF y2 = new DOF(pts[n2Index].Y);
 //           //    DOF z2 = new DOF(pts[n2Index].Z);

 //           //    DOF[] coor2 = new DOF[] { x2, y2, z2, new DOF(0), new DOF(0), new DOF(0) };

 //           //    Node n2 = new Node(coor2);
 //           //    n2.Index = n2Index;

 //           //    // Define member

 //           //    Member m = new Member(n1, n2);
 //           //    m.Material = mat;

 //           //    //m.Material = Material.MaterialValue;
 //           //    //m.Section = Section.SectionValue;

 //           //    // Add member and nodes to respective lists

 //           //    Nodes.Add(n1);
 //           //    Nodes.Add(n2);
 //           //    Members.Add(m);

 //           //}
            
 //           foreach (DOF d in Nodes[0].DOFs)
 //           {
 //               d.Fixed = true;
 //           }

 //           foreach (DOF d in Nodes[2].DOFs)
 //           {
 //               d.Fixed = true;
 //           }

 //           Structure structure = new Structure(Nodes, Members);

 //           LoadCase lc = new LoadCase("lc1");

 //           double lx = -1.00;

 //           foreach (Member m in structure.Members)
 //           {
 //               Console.WriteLine(1);
 //               Console.WriteLine(Members[0].NodeI.DOFs.Length);
 //           }

                          
 //           foreach (StructureEngine.Model.Node node in structure.Nodes)
 //               {

 //                       Load loadx = new Load(lx, lc, node.DOFs[0]);
 //                       lc.Loads.Add(loadx);
 //               }
 //               //points.Remove(point); collection was modified each time -> error
 //           structure.LoadCases.Add(lc);
 //           ComputedStructure comp = new ComputedStructure(structure);

 //           Structure Structure = new Structure(Nodes, Members);
 //           ComputedStructure CompStructure = new ComputedStructure(Structure);

 //           // Add Loadcase to structure

 //           FrameAnalysis analysis = new FrameAnalysis();
 //           analysis.RunAnalysis(comp);

            //StormCloudAnalysis.Types.StructureType Structure_GHrep = new StormCloudAnalysis.Types.StructureType(CompStructure);
            /*Assign the outputs via the DA object*/
            //DA.SetDataList(0, pts);
            //DA.SetData(1, Structure_GHrep);

            // LOAD 


        }

    }
}