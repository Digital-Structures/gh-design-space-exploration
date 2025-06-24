using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;

namespace DSOptimization
{
    public interface IDesignGeometry
    {
        List<GeoVariable> Variables { get; set; }
        void VarUpdate(GeoVariable geovar);
        void Update();
        IGH_Param Parameter { get; set;}
    }
}
