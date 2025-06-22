
namespace StructureEngine.Model
{
    public class Load
    {
        public Load(double l, LoadCase lc, DOF d)
        {
            this.Value = l;
            this.Case = lc;
            this.myDOF = d;
        }

        public double Value;

        public LoadCase Case;

        public DOF myDOF;

        public Load Clone(LoadCase newLC)
        {
            return new Load(this.Value, newLC, this.myDOF);
        }
    }
}
