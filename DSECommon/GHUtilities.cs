using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
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
            // Modify for silent behavior
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true,GH_SolutionMode.Silent);
        }

        public static void WriteOutputToFile(List<List<double>> output, string path, string filename, string extension)
        {
            string a = null;
            for (int i = 0; i < output.Count; i++)
            {
                string b = null;
                for (int j = 0; j < output[i].Count; j++)
                {
                    b = b + output[i][j] + " ";
                }
                a = a + b + "\r\n";
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter(@"" + path + filename + extension);
            file.Write(a);
            file.Close();
        }

        public static DataTree<T> ListOfListsToTree<T>(List<List<T>> listofLists)
        {
            DataTree<T> tree = new DataTree<T>();
            for (int i = 0; i < listofLists.Count; i++)
            {
                tree.AddRange(listofLists[i], new GH_Path(i));
            }
            return tree;
        }

        public static List<List<double>> StructureToListOfLists(GH_Structure<GH_Number> structure)
        {
            List<List<double>> list = new List<List<double>>();
            foreach (GH_Path p in structure.Paths)
            {
                List<GH_Number> l = (List<GH_Number>)structure.get_Branch(p);
                List<double> doubles = new List<double>();
                foreach (GH_Number n in l)
                {
                    double d = 0;
                    n.CastTo<double>(out d);
                    doubles.Add(d);
                }
                list.Add(doubles);
            }
            return list;
        }

        public static List<List<T>> TreeToListOfLists<T>(DataTree<T> tree)
        {
            List<List<T>> list = new List<List<T>>();
            foreach (GH_Path p in tree.Paths)
            {
                List<T> l = tree.Branch(p);
                list.Add(l);
            }
            return list;
        }
    }
}
