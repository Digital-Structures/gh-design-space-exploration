using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sampler
{
    public class SamplerUtilities
    {
        public SamplerUtilities(SamplerComponent comp)
        {
            MyComponent = comp;
        }

        private SamplerComponent MyComponent;

        // This is the main method that produces the sample data based on the input scheme.
        public void Sample()
        {
            switch (MyComponent.Scheme)
            {
                case 0:
                    MyComponent.Output = this.RandomUniform();
                    return;
                case 1:
                    //MyComponent.Output = this.Grid();
                    return;
                case 2:
                    //MyComponent.Output = this.LHC();
                    return;
                default:
                    return;
            }
        }

        private List<List<double>> RandomUniform()
        {
            List<List<double>> samplesList = new List<List<double>>();

            Random r = MyComponent.MyRand;

            for (int i = 0; i < MyComponent.NSamples; i++)
            {
                //Console.WriteLine("echo");
                List<double> thisSample = new List<double>();
                for (int j = 0; j < MyComponent.VarsList.Count; j++)
                {
                    thisSample.Add(r.NextDouble());
                }
                samplesList.Add(thisSample);
            }
            //Console.Read();
            return Scale(samplesList);
        }

        //private List<List<double>> Grid()
        //{

        //    double bcount = 0;
        //    int N = this.numVar;
        //    double stepsAux = Math.Pow(N, (1 / (double)samples));
        //    int steps = (int)Math.Ceiling(stepsAux);
        //    double n = Math.Pow(steps, samples);


        //    List<List<double>> a = new List<List<double>>();
        //    List<double> p = new List<double>();
        //    List<double> basic = new List<double>();

        //    for (int i = 0; i < steps; i++)
        //    {
        //        basic.Add(bcount);
        //        bcount = bcount + 1 / (steps - 1);
        //    }
        //    for (int i = samples; i > 0; i--)
        //    {
        //        p.Add(Math.Pow(steps, (i - 1)));
        //    }
        //    for (int i = 0; i < n; i++)
        //    {
        //        List<double> c = new List<double>();
        //        a.Add(c);
        //        //MessageBox.Show("i = " + i);
        //        for (int j = samples - 1; j > -1; j--)
        //        {
        //            int ind1 = (int)Math.Floor(i / (double)p[j]);
        //            double ind = (ind1 % steps);
        //            c.Add(basic[(int)ind]);
        //        }
        //    }

        //    Scale(a);
        //    return Scale(a);
        //}

        //private List<List<double>> LHC()
        //{
        //    List<List<double>> a = new List<List<double>>();
        //    for (int j = 0; j < samples; j++)
        //    {
        //        List<double> c = new List<double>();
        //        a.Add(c);
        //        List<int> inds = Perms(numVar);

        //        for (int i = 0; i < numVar; i++)
        //        {
        //            a[j].Add(inds[i]);
        //            a[j][i] -= 1;
        //            a[j][i] /= (double)(numVar - 1);
        //        }
        //    }

        //    List<List<double>> b = new List<List<double>>();
        //    for (int i = 0; i < numVar; i++)
        //    {
        //        List<double> c = new List<double>();
        //        b.Add(c);
        //        for (int j = 0; j < samples; j++)
        //        {
        //            b[i].Add(a[j][i]);
        //        }
        //    }
        //    Scale(a);
        //    return Scale(a);
        //}

        //public List<int> Perms(int n)
        //{
        //    List<int> indsAscend = new List<int>();
        //    for (int i = 1; i < n + 1; i++)
        //    {
        //        indsAscend.Add(i);
        //    }

        //    int count = 1;
        //    List<int> inds = new List<int>();

        //    while (count <= n)
        //    {
        //        int ind = r.Next(1, n + 1);
        //        if (!inds.Contains(ind))
        //        {
        //            inds.Add(ind);
        //            count += 1;
        //        }
        //    }
        //    return inds;
        //}

        private List<List<double>> Scale(List<List<double>> samples)
        {
            List<double> rmin = new List<double>();
            List<double> rmax = new List<double>();

            foreach (DSEVariable myVar in MyComponent.VarsList)
            {
                rmin.Add(myVar.LowerBound);
                rmax.Add(myVar.UpperBound);
            }

            var scaledSamples = new List<List<double>>();

            for (int i = 0; i < MyComponent.NSamples; i++)
            {
                List<double> scaledVars = new List<double>();
                
                for (int j = 0; j < MyComponent.VarsList.Count; j++)
                {
                    scaledVars.Add((samples[i][j]) * Math.Abs(rmax[j] - rmin[j]) + rmin[j]);
                }
                scaledSamples.Add(scaledVars);
            }

            return scaledSamples;
        }

        public void WriteOutputToFile(List<List<double>> output)
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

            System.IO.StreamWriter file = new System.IO.StreamWriter(@"" + MyComponent.Path + MyComponent.Prefix + ".csv");
            file.Write(a);
            file.Close();
        }
    }
}
