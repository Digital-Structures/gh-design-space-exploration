using System.Collections.Generic;
using StructureEngine.Model;

namespace StormCloud.Evolutionary
{
    public interface IDivBooster
    {
        bool IsDiverse(List<Design> existing, Design candidate, double rate);
    }
}
