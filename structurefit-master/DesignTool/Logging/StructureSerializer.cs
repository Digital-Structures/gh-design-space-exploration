using System.Text;
using StructureEngine.Model;

namespace StructureEngine.Logging
{
    public class StructureSerializer
    {
        public StructureSerializer()
        {
        }

        public string Serialize(ComputedStructure s, double RefScore)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("// Score //");
            sb.AppendLine(s.Score.ToString());
            sb.AppendLine("");
 
            sb.AppendLine("// Nodes //");
            sb.AppendLine("Name\tXCoord\tYCoord\tXVar\tYVar\tXMin\tYMin\tXMax\tYMax\tXPin\tYPin\tXLoad\tYLoad");
            NodeSerializer ns = new NodeSerializer();
            foreach (Node n in s.Nodes)
            {
                sb.AppendLine(ns.Serialize(n));
            }
            sb.AppendLine("");

            sb.AppendLine("// Load Cases //");
            sb.AppendLine("Name");
            LoadCaseSerializer lcs = new LoadCaseSerializer();
            foreach (LoadCase lc in s.LoadCases)
            {
                sb.AppendLine(lcs.Serialize(lc, s));
            }
            sb.AppendLine("");

            sb.AppendLine("// Members //");
            sb.AppendLine("NodeI\tNodeJ");
            MemberSerializer ms = new MemberSerializer();
            foreach (Member m in s.Members)
            {
                sb.AppendLine(ms.Serialize(m));
            }
            sb.AppendLine("");

            sb.AppendLine("// Variable Map //");
            sb.AppendLine(this.SerializeVars(s));

            sb.AppendLine("// Vector Format //");
            sb.AppendLine(this.QuickSerializeHeader(s));
            sb.AppendLine(this.QuickSerialize(s, RefScore));

            return sb.ToString();
        }

        public string QuickSerialize(ComputedStructure s, double RefScore)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(s.Score/RefScore + "\t");
            foreach (IVariable v in s.DesignVariables)
            {
                sb.Append(v.GetPoint() + "\t");
            }
            return sb.ToString();
        }

        public string QuickSerializeHeader(ComputedStructure s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Score" + "\t");
            int i = 0;
            foreach (IVariable v in s.DesignVariables)
            {
                sb.Append("v" + i + "\t");
                i++;
            }
            return sb.ToString();
        }

        private string SerializeVars(ComputedStructure s)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Name\tNode\tDOF");
            int index = 0;
            foreach (Node n in s.Nodes)
            {
                for (int i = 0; i < n.DOFs.Length; i++)
                {
                    DOF d = n.DOFs[i];
                    if (d is CoordVar)
                    {
                        string XorY = i == 0 ? "X" : "Y";
                        sb.AppendLine("v" + index + "\t" + "n" + n.Index + "\t" + XorY);
                        index++;
                    }
                }
            }
            return sb.ToString();
        }
    }
}
