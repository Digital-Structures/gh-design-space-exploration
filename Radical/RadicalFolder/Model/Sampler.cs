using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radical.Components;
using DSOptimization;

namespace Radical.Integration
{
    public class Sampler
    {
        public Sampler(Design design)
        {
            Design = design;
        }

        public Sampler(Design design, ISamplingAlg alg, int nsamples)
        {
            nSamples = nsamples;
            Design = design;
            Alg = alg;
            SetBounds();
        }

        public void SetBounds()
        {
            Bounds = new List<Tuple<double, double>>();
            foreach (IVariable var in Design.Variables)
            {
                Bounds.Add(new Tuple<double, double>(var.Min, var.Max));
            }
        }

        internal void RunSampling()
        {
            List<List<double>> samples = Alg.Run(nSamples, NVars, Bounds);

            foreach (List<double> newvars in samples)
            {
                for (int i = 0; i < NVars; i++)
                {
                    IVariable var = Design.Variables[i];
                    var.UpdateValue(newvars[i]);
                }
                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true, Grasshopper.Kernel.GH_SolutionMode.Silent);
                //Design.Samples.Add(((Design)Design).ExpComponent.dVars);
                //Design.Properties.Add(((Design)Design).ExpComponent.dProp);
            }
        }

        public Design Design;
        public ISamplingAlg Alg;
        private int NVars { get { return this.Design.Variables.Count; } }
        private int nSamples;
        private List<Tuple<double, double>> Bounds;

        public interface ISamplingAlg
        {
            List<List<double>> Run(int nsamples, int ndimensions, List<Tuple<double, double>> bounds);
        }


        #region SamplingAlgorithms
        public class RUSampling : ISamplingAlg
        {
            public RUSampling() { }
            public List<List<double>> Run(int ns, int nvar, List<Tuple<double, double>> bounds)
            {
                List<List<double>> samples = new List<List<double>>();
                for (int i = 0; i < nvar; i++)
                {
                    List<double> samples_i = new List<double>();
                    Random rd = new Random();
                    double min = bounds[i].Item1;
                    double max = bounds[i].Item2;

                    for (int j = 0; j < ns; j++)
                    {
                        double s = rd.NextDouble() * (max - min) + min;
                        samples_i.Add(s);
                    }
                    samples.Add(samples_i);
                }
                return samples;
            }
        }
        public class LHSampling : ISamplingAlg
        {
            public LHSampling() { }
            public List<List<double>> Run(int ns, int nvar, List<Tuple<double, double>> bounds)
            {
                List<List<double>> samples = new List<List<double>>();
                Random rd = new Random();
                double nstep = 1.0 / ns;
                for (int i = 0; i < nvar; i++)
                {
                    List<int> cells = Enumerable.Range(0, ns).ToList();
                    List<double> samples_i = new List<double>();
                    int count = ns;
                    double min = bounds[i].Item1;
                    double max = bounds[i].Item2;
                    for (int j = 0; j < ns; j++)
                    {
                        if (i == 0) { samples.Add(new List<double>()); }
                        int k = rd.Next(count);
                        int cell = cells[k];
                        cells.RemoveAt(k);
                        double s = (cell + rd.NextDouble()) * nstep;
                        samples[j].Add(s * (max - min) + min);
                        count = count - 1;
                    }
                }
                return samples;

            }
        }
        public class GSampling : ISamplingAlg
        {
            public GSampling() { }

            public List<List<double>> Run(int ns, int nvar, List<Tuple<double, double>> bounds)
            {
                List<List<double>> samples = new List<List<double>>();
                double step = 1.0 / (ns - 1.0);
                for (int i = 0; i < nvar; i++)
                {
                    List<double> samples_i = new List<double>();
                    double min = bounds[i].Item1;
                    double max = bounds[i].Item2;
                    for (int j = 0; j < ns; j++)
                    {
                        double s = j * step * (max - min) + min;
                        samples_i.Add(s);
                    }
                    samples.Add(samples_i);
                }
                return samples;

            }
        }
        #endregion
    }
}
