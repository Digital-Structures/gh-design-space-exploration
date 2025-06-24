using System.Collections.Generic;

namespace StructureEngine.Model
{
    public enum RelationType
    {
        Mirror,
        Master,
        Offset,
        Average
    }

    public class ParametricRelation
    {
        public ParametricRelation(List<Node> l, RelationType r)
        {
            this.Listeners = l;
            this.Relation = r;
        }

        public ParametricRelation(List<Node> l, RelationType r, object p) : this(l,r)
        {
            this.Parameter = p;
        }

        public List<Node> Listeners
        {
            get;
            set;
        }

        public RelationType Relation
        {
            get;
            set;
        }

        public object Parameter
        {
            get;
            set;
        }
    }
}
