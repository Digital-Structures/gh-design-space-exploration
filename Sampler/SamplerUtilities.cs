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
                    MyComponent.Output = this.Grid();
                    return;
                case 2:
                    MyComponent.Output = this.LHC();
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
                List<double> thisSample = new List<double>();
                foreach (var v in MyComponent.VarsList)
                {
                    thisSample.Add(r.NextDouble());
                }
                samplesList.Add(thisSample);
            }
            return Scale(samplesList);
        }

        private List<List<double>> Grid()
        {
            double bcount = 0;
            int N = MyComponent.VarsList.Count;
            double stepsAux = Math.Pow(N, (1 / (double)MyComponent.NSamples));
            int steps = (int)Math.Ceiling(stepsAux);
            double n = Math.Pow(steps, MyComponent.NSamples);


            List<List<double>> samplesList = new List<List<double>>();
            List<double> p = new List<double>();
            List<double> basic = new List<double>();

            for (int i = 0; i < steps; i++)
            {
                basic.Add(bcount);
                bcount = bcount++ / (steps - 1);
            }
            for (int i = MyComponent.NSamples; i > 0; i--)
            {
                p.Add(Math.Pow(steps, (i - 1)));
            }
            for (int i = 0; i < n; i++)
            {
                List<double> thisSample = new List<double>();
                
                for (int j = MyComponent.NSamples - 1; j > -1; j--)
                {
                    int ind1 = (int)Math.Floor(i / (double)p[j]);
                    double ind = (ind1 % steps);
                    thisSample.Add(basic[(int)ind]);
                }
                samplesList.Add(thisSample);
            }

            return Scale(samplesList);
        }

        private List<List<double>> LHC()
        {
            int N = MyComponent.VarsList.Count;
            List<List<double>> samplesList = new List<List<double>>();
            for (int j = 0; j < MyComponent.NSamples; j++)
            {
                List<double> thisSample = new List<double>();
                samplesList.Add(thisSample);
                List<int> inds = Perms(N);

                for (int i = 0; i < N; i++)
                {
                    samplesList[j].Add(inds[i]);
                    samplesList[j][i] -= 1;
                    samplesList[j][i] /= (double)(N - 1);
                }
            }

            // TODO: What is the code doing below?  Should this replace what is above?
            //List<List<double>> b = new List<List<double>>();
            //for (int i = 0; i < N; i++)
            //{
            //    List<double> c = new List<double>();
            //    b.Add(c);
            //    for (int j = 0; j < MyComponent.NSamples; j++)
            //    {
            //        b[i].Add(samplesList[j][i]);
            //    }
            //}

            return Scale(samplesList);
        }

        public List<int> Perms(int n)
        {
            List<int> indsAscend = new List<int>();
            for (int i = 1; i < n + 1; i++)
            {
                indsAscend.Add(i);
            }

            int count = 1;
            List<int> inds = new List<int>();

            while (count <= n)
            {
                int ind = MyComponent.MyRand.Next(1, n + 1);
                if (!inds.Contains(ind))
                {
                    inds.Add(ind);
                    count += 1;
                }
            }
            return inds;
        }

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
