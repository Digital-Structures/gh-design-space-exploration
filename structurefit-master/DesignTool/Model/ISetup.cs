using System.Collections.Generic;

namespace StructureEngine.Model
{
    public interface ISetup
    {
        List<IDesign> Designs
        {
            get;
            set;
        }
    }
}
