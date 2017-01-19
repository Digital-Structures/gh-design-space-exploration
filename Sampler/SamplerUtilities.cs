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
            double nBinsOpt = Math.Pow(MyComponent.NSamples, (1 / (double)MyComponent.VarsList.Count));
            int nBins = (int)Math.Ceiling(nBinsOpt);
            int nSamplesFinal = (int)Math.Pow(nBins, MyComponent.VarsList.Count);

            List<double> binValues = new List<double>();
            double bin = (1 / (double)(nBins))/(double)2;

            for (int i = 0; i < nBins; i++)
            {
                binValues.Add(bin);
                bin = bin + 1 / (double)(nBins);
            }

            List<List<double>> samplesList = this.PermsWithRep(binValues, nSamplesFinal);

            return Scale(samplesList);
        }

        private List<List<double>> PermsWithRep(List<double> values, int n)
        {
            double[] arr = new double[n];
            IEnumerable <double[]> perms = PermsWithRepHelper(values, arr, n, 0);
            List<List<double>> list = new List<List<double>>();
            var listArray = perms.ToList<double[]>();
            foreach (double[] array in listArray)
            {
                var l = array.ToList<double>();
                list.Add(l);
            }
            return list;
        }

        private IEnumerable<double[]> PermsWithRepHelper(List<double> values, double[] arr, int n, int i)
        {
            foreach (double val in values)
            {
                arr[i] = val;
                if (i + 1 == n)
                {
                    yield return arr;
                }
                else
                {
                    foreach (var r in PermsWithRepHelper(values, arr, n, i + 1))
                    {
                        yield return r;
                    }
                }
            }
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



        private List<int> Perms(int n)
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
