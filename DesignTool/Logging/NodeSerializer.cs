using System.Text;
using StructureEngine.Model;

namespace StructureEngine.Logging
{
    public class NodeSerializer
    {
        public NodeSerializer()
        {
        }

        public string Serialize(Node n)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("n" + n.Index + "\t");
            sb.Append(n.DOFs[0].Coord + "\t");
            sb.Append(n.DOFs[1].Coord + "\t");
            sb.Append((n.DOFs[0] is CoordVar ? 1 : 0) + "\t");
            sb.Append((n.DOFs[1] is CoordVar ? 1 : 0) + "\t");
            CoordVar cvx = n.DOFs[0] as CoordVar;
            CoordVar cvy = n.DOFs[1] as CoordVar;
            sb.Append((cvx == null ? n.DOFs[0].Coord : cvx.Min) + "\t");
            sb.Append((cvy == null ? n.DOFs[1].Coord : cvy.Min) + "\t");
            sb.Append((cvx == null ? n.DOFs[0].Coord : cvx.Max) + "\t");
            sb.Append((cvy == null ? n.DOFs[1].Coord : cvy.Max) + "\t");
            sb.Append((n.DOFs[0].Pinned ? 1 : 0) + "\t");
            sb.Append((n.DOFs[1].Pinned ? 1 : 0) + "\t");
            //sb.Append(n.DOFs[0].Load + "\t");
            //sb.Append(n.DOFs[1].Load + "\t");
            return sb.ToString();
        }
    }
}
