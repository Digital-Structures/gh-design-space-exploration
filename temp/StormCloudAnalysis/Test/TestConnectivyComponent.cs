using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StructureEngine.Model;

namespace StormCloud.Test
{
    public class TestConnectivyComponent
    {
        public static void Main()
        {
            /* Declare variables to contain all inputs
 * We can assign initial values (or not) that are either indicative or sensible */
            List<Rhino.Geometry.Line> lines = new List<Line>();
            Line line1 = new Line(new Point3d(0,0,0), new Point3d(0,1,0));
            Line line2 = new Line(new Point3d(0,0,0), new Point3d(0,0,1));
            lines.Add(line1);
            lines.Add(line2);


            //CustomTypes.MaterialType Material;
            //CustomTypes.SectionType Section;
            // double tolerance = double.NaN ;



            /*Retrieve coordinates of start and end points*/
            List<Rhino.Geometry.Point3d> pts = new List<Rhino.Geometry.Point3d>();
            List<Node> Nodes = new List<Node>();
            List<Member> Members = new List<Member>();

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
                    }
                }

                DOF x1 = new DOF(pts[n1Index].X);
                DOF y1 = new DOF(pts[n1Index].Y);
                DOF z1 = new DOF(pts[n1Index].Z);

                DOF[] coor1 = new DOF[] { x1, y1, z1 };

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

                DOF[] coor2 = new DOF[] { x2, y2, z2 };

                Node n2 = new Node(coor2);
                n2.Index = n2Index;

                // Define member

                Member m = new Member(n1, n2);

                //m.Material = Material.MaterialValue;
                //m.Section = Section.SectionValue;

                // Add member and nodes to respective lists

                Nodes.Add(n1);
                Nodes.Add(n2);
                Members.Add(m);

            }

            Structure Structure = new Structure(Nodes, Members);
            ComputedStructure CompStructure = new ComputedStructure(Structure);
            Types.StructureType Structure_GHrep = new Types.StructureType(CompStructure);
            /*Assign the outputs via the DA object*/
            //DA.SetDataList(0, pts);
            //DA.SetData(1, Structure_GHrep);
        }

    }
}