using System.Collections.Generic;
using StructureEngine.Model;

namespace StructureEngine.Evolutionary
{
    public interface IDivBooster
    {
        bool IsDiverse(List<IDesign> existing, IDesign candidate, double rate);
    }
}
