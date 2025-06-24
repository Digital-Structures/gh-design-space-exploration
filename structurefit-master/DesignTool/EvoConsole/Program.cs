using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Generic;
using netDxf;
using StructureEngine.Model;
using StructureEngine.Serialization;
using StructureEngine.Test;
using StructureEngine.Analysis;
using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace EvoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //int numC = 104 * 3;
            //int numR = numC / 4;

            //MyPlotter = new DSPlotter2D();
            //Data = GetData(numR, numC);

            //SaveData(numC, "frame");
            //GetDXFData(new double[] { 1.0, 1.1, 1.2, 1.3 }, numC);

            int num = 200;
            var MyDSGen = new DSGenerator();
            //ComputedStructure des = Set3VProblem();
            //ComputedStructure des = Set10VProblem();
            //ComputedStructure des = SetMichellProblem();
            ComputedStructure des = SetMichellProblem();
            des.SetStart();
            //Tuple<Matrix<double>, IList<IDesign>> results = MyDSGen.GetData(des, num, new double[] { 0.62, 0.68, 0.25 }, new double[] {0.5, 0.1, 0.3});
            
            //Tuple<Matrix<double>, IList<IDesign>> results = MyDSGen.GetData(des, num);
            //SaveData(results.Item1, num, "Michell");

            Matrix<double> michresult = MyDSGen.MatfromDes(new List<IDesign>() { des.GenerateFromVars(new double[] { 0.265320971160620,	0.315998052520203,
                0.323127079236871,	0.681460764463937,	0.104821746466681,	0.893464925962998,	0.364571096712965,	0.319477060839830 }) });
            SaveData(michresult, 1, "Michell");

            //IList<Tuple<Matrix<double>, IList<IDesign>>> results = MyDSGen.GetBaseData(des, num, 2);
            //Matrix<double> m = new DenseMatrix(num * results.Count, results[0].Item1.ColumnCount);
            //for (int i = 0; i < results.Count; i++)
            //{
            //    var r = results[i];
            //    Matrix<double> mat = r.Item1;
            //    m.SetSubMatrix(i * num, num, 0, results[0].Item1.ColumnCount, mat);
            //}

            //Matrix<double> m = Custom3V(des, 1, num, 4);

            //SaveData(m, num, "7bar3V_basedata1");

            //DXFMaker dm = new DXFMaker();
            //DxfDocument dxf = dm.WriteDXFSpace(designs);
            //string path = "../frame-designspace-" + num.ToString() + ".dxf";
            //StreamWriter fileStream = new StreamWriter(path, false);
            //Stream file = fileStream.BaseStream;
            //bool check = dxf.Save(file);

            //fileStream.Flush();
            //fileStream.Close();

            //SaveData(mat, num, "frame");

            //this.PrintDXFGrid();
        }

        private static void PrintDXFGrid()
        {
            Matrix<double> mat = new DenseMatrix(new double[,] {    {0.3293,0.4809,0.5887,0.6431,0.4945,0.3949,0.8664,0.7528,0.4821,0.3588},
            {0.4404,0.5871,0.3902,0.6496,0.5711,0.6125,0.7125,0.9095,0.5153,0.5795},
            {0.1476,0.4044,0.5979,0.8770,0.5792,0.6627,0.4923,0.6000,0.5588,0.3529},
            {0.3023,0.4868,0.6646,0.8221,0.6902,0.3258,0.5418,0.5836,1.0000,0.9980},
            {0.3998,0.4937,0.5376,0.7403,0.4493,0.5215,0.6627,0.6531,0.3951,0.4767},
            {0.2415,0.5681,0.5078,0.6450,0.5111,0.3564,0.5911,0.7552,0.4759,0.5581},
            {0.3885,0.5915,0.4829,0.7021,0.5750,0.3453,0.7348,0.6389,0.3134,0.1743},
            {0.2724,0.3791,0.6699,0.5396,0.4192,0.3118,0.5768,0.7917,0.5983,0.3602},
            {0.4039,0.5640,0.4530,0.6771,0.6308,0.4430,0.6362,0.6348,0.6937,0.6963},
            {0.7615,0.7335,0.5652,0.6296,0.4943,0.6585,0.6024,0.8753,0.4795,0.4526},
            {0.2202,0.6226,0.5999,0.6730,0.5364,0.6551,0.4950,0.4739,0.5859,0.3452},
            {0.3751,0.2319,0.6322,0.6253,0.3755,0.3073,0.4940,0.5877,0.1823,0.1795},
            {0.3990,0.5646,0.4554,0.5457,0.5951,0.3176,0.3121,0.9286,0.5079,0.2385},
            {0.0456,0.7576,0.9174,0.6775,0.4999,0.0605,0.5071,0.4136,0.4635,0.0000},
            {0.5332,0.3247,0.6760,0.5895,0.6682,0.4692,0.4353,0.5211,0.5911,0.5272},
            {0.1328,0.5992,0.3464,0.3291,0.3964,0.2706,0.5631,0.4933,0.3401,0.3526},
            {0.4344,0.3221,0.3541,0.5209,0.5481,0.0000,0.5791,0.5478,0.5323,0.4583},
            {0.6280,0.8965,0.6245,0.4849,0.4534,0.5331,0.7739,0.7094,0.7976,0.3714}   });

            List<double[]> vars = new List<double[]>();
            for (int i = 0; i < mat.RowCount; i++)
            {
                double[] var = mat.Row(i).ToRowMatrix().ToRowWiseArray();
                vars.Add(var);
            }

            StructureSetup setup = new StructureSetup();
            StructureSetup set = new StructureSetup();
            ComputedStructure des = (ComputedStructure)set.Designs[9];
            des.SetStart();

            DSGenerator gen = new DSGenerator(des);
            DxfDocument dxf = gen.DrawDesigns(vars, 6, 3);

            string path = "../isodesigns-frame.dxf";
            StreamWriter fileStream = new StreamWriter(path, false);
            Stream file = fileStream.BaseStream;
            bool check = dxf.Save(file);

            fileStream.Flush();
            fileStream.Close();
        }

        private static ComputedStructure SetMetricTussProblem()
        {
            StructureSetup s = new StructureSetup();
            IDesign p = s.Designs[0];
            if (p as ComputedStructure != null)
            {
                ComputedStructure c = (ComputedStructure)p;
                c.Nodes[1].DOFs[0].Coord = 39.3701; // inches = 1m
                c.Nodes[1].DOFs[1].Coord = -39.3701; // inches = -1m
                c.Nodes[2].DOFs[0].Coord = 118.11; // inches = 3m
                c.Nodes[2].DOFs[1].Coord = -39.3701; // inches = -1m
                c.Nodes[3].DOFs[0].Coord = 78.7402; // inches = 2m
                c.Nodes[4].DOFs[0].Coord = 157.48; // inches = 4m

                foreach (IVariable v in p.DesignVariables)
                {
                    v.SetConstraint();
                }

                c.Nodes[3].DOFs[1].AllowableVariation = 157.48; // inches = 4m
                c.Nodes[1].DOFs[0].AllowableVariation = 39.3; // inches =  almost 1m
                c.Nodes[1].DOFs[1].AllowableVariation = 157.48; // inches = 4m

                c.LoadCases[0].GetLoad(c.Nodes[3].DOFs[1]).Value = -22.48; // kips = 100KN
                c.SymmetryLine[0] = 78.7402; // inches = 2m

                return c;
            }

            else
            {
                throw new Exception("Problem is not of type ComputedStructure.");
            }
        }

        private static ComputedStructure Set1VProblem()
        {
            ComputedStructure c = SetMetricTussProblem();
            c.Nodes[3].DOFs[1].Free = false;
            c.Nodes[3].DOFs[1].AllowableVariation = 0;
            c.Nodes[1].DOFs[0].Free = false;
            c.Nodes[1].DOFs[0].AllowableVariation = 0;

            return c;
        }

        private static ComputedStructure Set2VProblem()
        {
            ComputedStructure c = SetMetricTussProblem();
            c.Nodes[3].DOFs[1].Free = false;
            c.Nodes[3].DOFs[1].AllowableVariation = 0;

            return c;
        }

        private static ComputedStructure Set3VProblem()
        {
            return SetMetricTussProblem();
        }

        //private static ComputedStructure Set3VOptProblem()
        //{
        //    ComputedStructure start = SetMetricTussProblem();
        //    start.GenerateFromVars(new double[] { 0.62, 0.68, 0.25 });
        //    DOF d1 = start.DesignVariables[0] as DOF;
        //    d1.AllowableVariation = d1.AllowableVariation * 0.5;
        //    DOF d2 = start.DesignVariables[1] as DOF;
        //    d2.AllowableVariation = d2.AllowableVariation * 0.1;
        //    DOF d3 = start.DesignVariables[2] as DOF;
        //    d3.AllowableVariation = d3.AllowableVariation * 0.3;

        //    return start;

        //}

        private static ComputedStructure Set4VProblem()
        {
            StructureSetup s = new StructureSetup();
            IDesign p = s.Designs[2];
            if (p as ComputedStructure != null)
            {
                ComputedStructure c = (ComputedStructure)p;
                foreach (IVariable v in c.DesignVariables)
                {
                    DOF d = v as DOF;
                    d.AllowableVariation = 157.48; // inches, = 4m
                }

                return c;
            }
            else
            {
                throw new Exception("Problem is not of type ComputedStructure.");
            }
        }

        private static ComputedStructure Set10VProblem()
        {
            StructureSetup s = new StructureSetup();
            IDesign p = s.Designs[9];
            if (p as ComputedStructure != null)
            {
                ComputedStructure c = (ComputedStructure)p;
                return c;
            }
            else
            {
                throw new Exception("Problem is not of type ComputedStructure.");
            }
        }

        private static ComputedStructure SetMichellProblem()
        {
            FileSerializer fs = new FileSerializer();
            string text = System.IO.File.ReadAllText("../../../Assets/FIT Files/MichellTruss2PanelMetric-FINAL.fit");
            IDesign michell = fs.ReadFITFile(text);
            return michell as ComputedStructure;
        }

        private static Matrix<double> Custom3V(IDesign design, int varIndex, int num, int divs)
        {
            IDesign des = design.DesignClone();
            double delta = 1.0 / divs;

            IList<IDesign> designs = new List<IDesign>();
            for (int i = 1; i < divs; i++)
            {
                double x1 = i * delta;
                for (int j = 1; j < divs; j++)
                {
                    double x3 = j * delta;
                    for (int k = 0; k < num; k++)
                    {
                        double x2 = (double)k / (double)num;
                        IDesign newDes = des.GenerateFromVars(new double[] { x1, x2, x3 });
                        designs.Add(newDes);
                    }
                }
            }

            DSGenerator ds = new DSGenerator();
            return ds.MatfromDes(designs);


            //foreach (IVariable v in des.DesignVariables)
            //{
            //    if (des.DesignVariables.IndexOf(v) != varIndex)
            //    {

            //    }

            //    else
            //    {

            //    }
            //}
        }

        private static void SaveData(Matrix<double> data, int num, string prefix)
        {
            // write data to disk;
            string path = "../" + prefix + "_" + num.ToString() + ".mat";
            StreamWriter file = new StreamWriter(path, false);
            file.Write(data.ToString());
            file.Close();
        }

        private static Matrix<double> GetData(int numR, int numC)
        {
            // get data
            var MyPlotter = new DSPlotter2D();
            Matrix<double> mat = MyPlotter.GenerateData(numR, numC);
            return mat;
        }

        private static void GetDXFData(double[] scores, int num, Matrix<double> data)
        {
            var designContours = new List<IEnumerable<ComputedStructure>>();

            // get data
            foreach (double score in scores)
            {
                var MyPlotter = new DSPlotter2D();
                var designs = MyPlotter.GetDesigns(data, score, 0.1 / (double)num);
                IEnumerable<ComputedStructure> clist = designs.Cast<ComputedStructure>();
                designContours.Add(clist);
            }

            // make DXF
            DXFMaker myDrawer = new DXFMaker();
            DxfDocument doc = myDrawer.MultiStructureDXF(designContours, scores);

            string path = "../isodesigns.dxf";
            StreamWriter fileStream = new StreamWriter(path, false);
            Stream file = fileStream.BaseStream;
            bool check = doc.Save(file);

            fileStream.Flush();
            fileStream.Close();
        }
    }
}
