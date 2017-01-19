using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
using Rhino.Geometry;

namespace DSECommon
{
    public static class GHUtilities
    {
        public static void ChangeSliders(List<GH_NumberSlider> sliders, List<double> targetVals)
        {
            if (sliders.Count != targetVals.Count)
            {
                throw new Exception("Error: Number of sliders and number of target values must be equal.");
            }
            for (int i = 0; i < sliders.Count; i++)
            {
                GH_NumberSlider s = sliders[i];
                double d = targetVals[i];
                s.SetSliderValue((decimal)d);
            }
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
        }
    }
}
