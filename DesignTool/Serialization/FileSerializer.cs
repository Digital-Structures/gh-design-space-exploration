using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using StructureEngine.Model;
using System.Linq;

namespace StructureEngine.Serialization
{
    public class FileSerializer
    {
        public FileSerializer()
        {
        }

        public IDesign ReadFITFile(string fs)
        {
            StringReader reader = new StringReader(fs);
            Structure opened = new Structure();

            // Read Nodes
            string line = "";

            while (!line.StartsWith("START_NODES"))
            {
                line = reader.ReadLine();
            }
            line = reader.ReadLine();
            while (!line.StartsWith("END_NODES"))
            {
                opened.Nodes.Add(ReadNode(line));
                line = reader.ReadLine();
            }

            // Read Materials
            while (!line.StartsWith("START_MATERIALS"))
            {
                line = reader.ReadLine();
            }
            line = reader.ReadLine();
            while (!line.StartsWith("END_MATERIALS"))
            {
                opened.Materials.Add(ReadMat(line));
                line = reader.ReadLine();
            }

            // Read Sections
            while (!line.StartsWith("START_SECTIONS"))
            {
                line = reader.ReadLine();
            }
            line = reader.ReadLine();
            while (!line.StartsWith("END_SECTIONS"))
            {
                opened.Sections.Add(ReadSec(line));
                line = reader.ReadLine();
            }

            // Read Members
            opened.Nodes.Sort(new NodeComparer());
            while (!line.StartsWith("START_MEMBERS"))
            {
                line = reader.ReadLine();
            }
            line = reader.ReadLine();
            while (!line.StartsWith("END_MEMBERS"))
            {
                opened.Members.Add(ReadMember(line, opened.Nodes, opened.Materials, opened.Sections));
                line = reader.ReadLine();
            }

            // Read Relations
            while (!line.StartsWith("START_RELATIONS"))
            {
                line = reader.ReadLine();
            }
            line = reader.ReadLine();
            while (!line.StartsWith("END_RELATIONS"))
            {
                this.ReadRelations(line, opened.Nodes);
                line = reader.ReadLine();
            }

            // Read SymLines
            opened.SymmetryLine = new double?[2];
            while (!line.StartsWith("START_SYMLINES"))
            {
                line = reader.ReadLine();
            }
            line = reader.ReadLine();
            while (!line.StartsWith("END_SYMLINES"))
            {
                this.ReadSym(line, opened);
                line = reader.ReadLine();
            }

            // Read Load Cases
            while (!line.StartsWith("START_LOADCASES"))
            {
                line = reader.ReadLine();
            }
            line = reader.ReadLine();
            while (!line.StartsWith("END_LOADCASES"))
            {
                this.ReadLC(opened.LoadCases, line, opened.Nodes);
                line = reader.ReadLine();
            }
                        
            opened.Nodes.Sort(new NodeComparer());

            return new ComputedStructure(opened);

            // TODO: Encode analysis method into FIT file
        }

        public string WriteFITFile(ComputedStructure structure, ComputedStructure initial)
        {
            StringBuilder sb = new StringBuilder();
            
            // Write Nodes
            sb.AppendLine("START_NODES");
            structure.Nodes.Sort(new NodeComparer());
            if (initial == null)
            {
                foreach (Node n in structure.Nodes)
                {
                    sb.AppendLine(WriteNode(n, null));
                }
            }
            else if (structure.Nodes.Count == initial.Nodes.Count)
            {
                for (int i = 0; i < structure.Nodes.Count; i++)
                {
                    sb.AppendLine(WriteNode(structure.Nodes[i], initial.Nodes[i]));
                }
            }
            sb.AppendLine("END_NODES");

            // Write Materials
            sb.AppendLine("START_MATERIALS");
            foreach (Material mat in structure.Materials)
            {
                sb.AppendLine(WriteMat(mat));
            }
            sb.AppendLine("END_MATERIALS");

            // Write Sections
            sb.AppendLine("START_SECTIONS");
            foreach (ISection sec in structure.Sections)
            {
                sb.AppendLine(WriteSec(sec));
            }
            sb.AppendLine("END_SECTIONS");

            // Write Members
            sb.AppendLine("START_MEMBERS");
            foreach (Member m in structure.Members)
            {
                sb.AppendLine(WriteMember(m));
            }
            sb.AppendLine("END_MEMBERS");

            // Write Relations
            sb.AppendLine("START_RELATIONS");
            foreach (Node n in structure.Nodes)
            {
                if (!String.Equals(WriteRelation(n), ""))
                {
                    sb.Append(WriteRelation(n));
                }
            }
            sb.AppendLine("END_RELATIONS");

            // Write Symmetry Lines
            sb.AppendLine("START_SYMLINES");
            for (int i = 0; i < structure.SymmetryLine.Length; i++)
            {
                double? d  = structure.SymmetryLine[i];
                if (d != null)
                {
                    sb.Append(WriteSym((double)d, i));
                }
            }
            sb.AppendLine("END_SYMLINES");

            // Write Load Cases
            sb.AppendLine("START_LOADCASES");
            foreach (LoadCase lc in structure.LoadCases)
            {
                sb.Append(WriteLC(lc, structure));
            }
            sb.AppendLine("END_LOADCASES");

            return sb.ToString();
        }

        public List<IDesign> ReadDESFile(string fs)
        {
            List<IDesign> designs = new List<IDesign>();
            StringReader reader = new StringReader(fs);

            string line = "";

            while (!line.StartsWith("START_DESIGN"))
            {
                line = reader.ReadLine();
            }
            line = reader.ReadLine();
            StringBuilder writer = new StringBuilder();
            while (!line.StartsWith("END_DESIGN"))
            {
                writer.AppendLine(line);
                line = reader.ReadLine();
            }
            designs.Add(ReadFITFile(writer.ToString()));

            line = reader.ReadLine();
            while (!line.StartsWith("START_INITIAL"))
            {
                line = reader.ReadLine();
            }
            line = reader.ReadLine();
            writer = new StringBuilder();
            while (!line.StartsWith("END_INITIAL"))
            {
                writer.AppendLine(line);
                line = reader.ReadLine();
            }
            designs.Add(ReadFITFile(writer.ToString()));

            return designs;
        }

        public string WriteDESFile(ComputedStructure design, ComputedStructure initial)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("START_DESIGN");
            sb.Append(WriteFITFile(design, initial));
            sb.AppendLine("END_DESIGN");

            sb.AppendLine("START_INITIAL");
            sb.Append(WriteFITFile(initial, null));
            sb.AppendLine("END_INITIAL");
            
            return sb.ToString();
        }

        private Node ReadNode(string nodeLine)
        {
            string[] nodeProps = nodeLine.Split('\t');

            DOF[] d;

            // Coordinates and Initial Coordinates
            if (nodeProps.Length == 13 && !String.Equals(nodeProps[11], "") && !String.Equals(nodeProps[12], ""))
            {
                CoordVar[] cv;
                cv = new CoordVar[] { new CoordVar(Convert.ToDouble(nodeProps[1]), Convert.ToDouble(nodeProps[11])),
                new CoordVar(Convert.ToDouble(nodeProps[2]), Convert.ToDouble(nodeProps[12])) };
                d = cv;
            }
            else
            {
                d = new DOF[] { new DOF(Convert.ToDouble(nodeProps[1])), new DOF(Convert.ToDouble(nodeProps[2])) };
            }

            // Variables and Ranges
            if (d[0] is CoordVar)
            {
                CoordVar cv = (CoordVar)d[0];
                cv.AllowableVariation = Convert.ToDouble(nodeProps[5]);
            }
            if (d[1] is CoordVar)
            {
                CoordVar cv = (CoordVar)d[1];
                cv.AllowableVariation = Convert.ToDouble(nodeProps[6]);
            }
            
            // Fixity
            d[0].Pinned = Convert.ToBoolean(nodeProps[7]);
            d[1].Pinned = Convert.ToBoolean(nodeProps[8]);

            //// Load
            //d[0].Load = Convert.ToDouble(nodeProps[9]);
            //d[1].Load = Convert.ToDouble(nodeProps[10]);

            Node n = new Node(d);

            // Index/Name
            n.Index = Convert.ToInt16(nodeProps[0]);
                        
            return n;
        }

        private string WriteNode(Node n, Node startNode)
        {
            int dim = 3;
            StringBuilder sb = new StringBuilder();

            sb.Append(n.Index);

            for (int i = 0; i < dim; i++)
            {
                sb.Append("\t" + n.DOFs[i].Coord);
            }

            for (int i = 0; i < dim; i++)
            {
                CoordVar cv = n.DOFs[i] as CoordVar;
                if (cv == null)
                {
                    sb.Append("\t" + "0");
                }
                else
                {
                    sb.Append("\t" + cv.AllowableVariation);
                }
            }

            for (int i = 0; i < dim; i++)
            {
                sb.Append("\t" + n.DOFs[i].Pinned.ToString());
            }

            if (startNode != null)
            {
                for (int i = 0; i < dim; i++)
                {
                    sb.Append("\t" + startNode.DOFs[i].Coord);
                }
            }

            return sb.ToString();
            
        }

        private Member ReadMember(string memberLine, List<Node> nodes, List<Material> mats, List<ISection> secs)
        {
            string[] memProps = memberLine.Split('\t');

            // Nodes
            Node i = nodes[Convert.ToInt16(memProps[0])];
            Node j = nodes[Convert.ToInt16(memProps[1])];
            Member m = new Member(i, j);

            // Material
            string matName = memProps[2];
            foreach (Material mat in mats)
            {
                if (String.Equals(matName, mat.Name))
                {
                    m.Material = mat;
                    break;
                }
            }
            if (m.Material == null)
            {
                throw new Exception("Invalid Material Error");
            }

            // Area
            m.Area = Convert.ToDouble(memProps[3]);

            // Section
            string secName = memProps[4];
            foreach (ISection sec in secs)
            {
                if (String.Equals(secName, sec.Name))
                {
                    m.SectionType = sec;
                    break;
                }
            }
            if (m.SectionType == null)
            {
                throw new Exception("Invalid Section Error");
            }

            return m;

        }

        private string WriteMember(Member m)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(m.NodeI.Index + "\t" + m.NodeJ.Index + "\t" + m.Material.Name + "\t" + m.Area + "\t" + m.SectionType.Name);

            return sb.ToString();
        }

        private void ReadLC(List<LoadCase> cases, string lcLine, List<Node> nodes)
        {
            string[] lcProps = lcLine.Split('\t');
            string lcName = lcProps[0];
            Node n = nodes[Convert.ToInt16(lcProps[1])];
            DOF d = n.DOFs[Convert.ToInt16(lcProps[2])];
            double value = Convert.ToDouble(lcProps[3]);

            List<LoadCase> thisCase = cases.Where(c => c.Name == lcName).ToList();

            // if the case doesn't exist yet, create a new case.
            if (thisCase.Count == 0)
            {
                LoadCase myCase = new LoadCase(lcName);
                Load myLoad = new Load(value, myCase, d);
                myCase.Loads.Add(myLoad);
                cases.Add(myCase);
            }

            // if the load case does exist, add the new load to the existing case.
            else if (thisCase.Count == 1)
            {
                LoadCase myCase = thisCase[0];
                Load myLoad = new Load(value, myCase, d);
                myCase.Loads.Add(myLoad);
            }

            // otherwise, two load cases must have the same name, which is a problem.
            else
            {
                throw new Exception("Load cases must have distinct names.");
            }
        }

        private string WriteLC(LoadCase lc, Structure s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Node n in s.Nodes)
            {
                for (int i = 0; i < n.DOFs.Length; i++)
                {
                    Load l = lc.GetLoad(n.DOFs[i]);
                    if (l.Value != 0)
                    {
                        sb.AppendLine(l.Case.Name + "\t" + n.Index + "\t" + i + "\t" + l.Value);
                    }
                }

            }

            return sb.ToString();
        }

        //private ComputedMember ReadComputedMember(string compMemberLine, List<Node> nodes, List<Material> mats)
        //{
        //    string[] memProps = compMemberLine.Split('\t');

        //    // Nodes
        //    Node i = nodes[Convert.ToInt16(memProps[0])];
        //    Node j = nodes[Convert.ToInt16(memProps[1])];
        //    ComputedMember m = new ComputedMember(i, j);

        //    // Material
        //    string matName = memProps[2];
        //    foreach (Material mat in mats)
        //    {
        //        if (String.Equals(matName, mat.Name))
        //        {
        //            m.Material = mat;
        //            break;
        //        }
        //    }
        //    if (m.Material == null)
        //    {
        //        throw new Exception("Invalid Material Error");
        //    }

        //    // Force
        //    m.AxialForce = Convert.ToDouble(memProps[3]);
            

        //    //// Requred Area
        //    //m.ReqArea = Convert.ToDouble(memProps[4]);

        //    //// Requred Area
        //    //m.ReqArea = Convert.ToDouble(memProps[4]);

        //    return m;
        //}

        private Material ReadMat(string matLine)
        {
            string[] matProps = matLine.Split('\t');
            
            string name = matProps[0];
            double e = Convert.ToDouble(matProps[1]);
            double sigma = Convert.ToDouble(matProps[2]);
            double rho = Convert.ToDouble(matProps[3]);
            Material mat = new Material(e, rho, sigma, name);
            return mat;
        }

        private string WriteMat(Material mat)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(mat.Name + "\t" + mat.E + "\t" + mat.StressAllow + "\t" + mat.Density);

            return sb.ToString();
        }

        private ISection ReadSec(string secLine)
        {
            string[] secProps = secLine.Split('\t');

            string name = secProps[0];
            string type = secProps[1];
            double param = Convert.ToDouble(secProps[2]);

            if (type == "Rectangular")
            {
                ISection sec = new RectangularSection(param, name);
                return sec;
            }
            else if (type == "RoundTube")
            {
                ISection sec = new RoundTubeSection(param, name);
                return sec;
            }
            else if (type == "Rod")
            {
                ISection sec = new RodSection(param, name);
                return sec;
            }
            else
            {
                throw new Exception("Not a recognized section type.");
            }
        }

        private string WriteSec(ISection sec)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(sec.Name + "\t" + sec.Type.ToString() + "\t" + sec.SectionParameter.ToString());

            return sb.ToString();
        }

        private void ReadRelations(string relLine, List<Node> nodes)
        {
            string[] relProps = relLine.Split('\t');

            List<Node> l = new List<Node>();
            l.Add(nodes[Convert.ToInt16(relProps[4])]);

            RelationType r = (RelationType)Enum.Parse(typeof(RelationType), relProps[2], true);

            double p = Convert.ToDouble(relProps[3]);
            
            ParametricRelation pr = new ParametricRelation(l, r, p);

            Node relNode = nodes[Convert.ToInt16(relProps[0])];
            int dof = Convert.ToInt16(relProps[1]);
            CoordVar cv = (CoordVar)relNode.DOFs[dof];
            cv.Relations.Add(pr);
        }

        private string WriteRelation(Node n)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < n.DOFs.Length; i++)
            {
                CoordVar cv = n.DOFs[i] as CoordVar;
                if (cv != null)
                {
                    foreach (ParametricRelation p in cv.Relations)
                    {
                        foreach (Node lis in p.Listeners)
                        {
                            string param = (p.Parameter == null) ? 0.ToString() : p.Parameter.ToString();
                            sb.AppendLine(n.Index + "\t" + i.ToString() + "\t" + p.Relation.ToString() + "\t"
                                + param + "\t" + lis.Index);
                        }
                    }
                }
            }

            return sb.ToString();
        }

        private void ReadSym(string symLine, Structure s)
        {
            string[] symProps = symLine.Split('\t');
            s.SymmetryLine[Convert.ToInt16(symProps[0])] = Convert.ToDouble(symProps[1]);
        }

        private string WriteSym(double d, int coord)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(coord + "\t" + d);
            return sb.ToString();
        }
    }



    public class NodeComparer : IComparer<Node>
    {
        public int Compare(Node n1, Node n2)
        {
            int compareResult = n1.Index.CompareTo(n2.Index);
            return compareResult;
        }
    }
}
