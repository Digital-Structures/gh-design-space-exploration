using System.Text;
using StructureEngine.Model;

namespace StructureEngine.Logging
{
    public class LoadCaseSerializer
    {
        public string Serialize(LoadCase lc, Structure s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(lc.Name);
            sb.AppendLine("Node\tValue");
            foreach (Load l in lc.Loads)
            {
                sb.AppendLine(SerializeLoad(l, s));
            }
            return sb.ToString();
        }

        private string SerializeLoad(Load l, Structure s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(s.DOFs.IndexOf(l.myDOF));
            sb.Append(l.Value);
            return sb.ToString();
        }
    }
}
