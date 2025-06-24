
using System.Collections.Generic;
namespace StructureEngine.Model
{
    public interface ISection
    {
        double GetReqEnvArea(Dictionary<LoadCase, double> force, double sigma, double E, double L);
        double GetReqArea(double f, double sigma, double E, double L);
        double GetReqThickness(double reqArea);
        double GetReqMomInertia_yy(double reqArea);
        double GetReqMomInertia_zz(double reqArea);
        double GetReqTorInertia(double reqArea);
        double SectionParameter
        {
            get;
            set;
        }
        string Name
        {
            get;
            set;
        }
        SectionType Type
        {
            get;
        }
        ISection SectionClone();
    }

    public enum SectionType
    {
        RoundTube,
        Rectangular,
        Rod
    }
}
