
namespace StructureEngine.Model
{
    public class Material
    {
        public double E
        {
            get;
            set;
        }

        public double Density
        {
            get;
            set;
        }

        public double StressAllow
        {
            get;
            set;
        }

        public string Name;

        public Material(double e, double d, double s, string n)
        {
            this.E = e;
            this.Density = d;
            this.StressAllow = s;
            this.Name = n;
        }

        public Material MaterialClone()
        {
            return new Material(this.E, this.Density, this.StressAllow, this.Name);
        }
    }
}
