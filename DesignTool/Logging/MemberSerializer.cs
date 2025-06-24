using System.Text;
using StructureEngine.Model;

namespace StructureEngine.Logging
{
    public class MemberSerializer
    {
        public MemberSerializer()
        {
        }

        public string Serialize(Member m)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("n" + m.NodeI.Index + "\t");
            sb.Append("n" + m.NodeJ.Index + "\t");
            return sb.ToString();
        }
    }
}
