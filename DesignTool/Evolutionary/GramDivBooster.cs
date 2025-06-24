using System.Collections.Generic;
using StructureEngine.Model;

namespace StructureEngine.Evolutionary
{
    public class GramDivBooster : IDivBooster
    {
        public GramDivBooster()
        {
        }

        public bool IsDiverse(List<IDesign> existing, IDesign candidate, double rate)
        {
            return true;
        }
    }
}
