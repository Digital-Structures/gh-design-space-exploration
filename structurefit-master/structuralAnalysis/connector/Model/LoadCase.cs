
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Linq;
using System;

namespace StructureEngine.Model
{
    public class LoadCase
    {
        public LoadCase(string n)
        {
            this.Name = n;
            this.Loads = new List<Load>();
        }

        //public LoadCase(string n, List<Load> l)
        //{
        //    this.Name = n;
        //    this.Loads = l;
        //}

        public string Name;

        public List<Load> Loads;

        public Vector<double> GetLoadVector(Structure s)
        {
            Vector<double> P = new DenseVector(s.DOFs.Count);

            foreach (Load l in this.Loads)
            {
                int index = s.DOFs.IndexOf(l.myDOF);
                P[index] += l.Value;
            }

            return P;
        }

        public Load GetLoad(DOF d)
        {
            List<Load> matchLoad = Loads.Where(l => l.myDOF == d).ToList();
            if (matchLoad.Count == 1)
            {
                return matchLoad[0];
            }
            else if (matchLoad.Count == 0)
            {
                Load newLoad = new Load(0, this, d);
                Loads.Add(newLoad);
                return newLoad;
            }
            else
            {
                throw new Exception("More than one load defined for one DOF.");
                //return new Load(matchLoad.Select(l => l.Value).Sum(), this, d);
            }
        }

        public LoadCase Clone()
        {
            LoadCase newLC = new LoadCase(this.Name);
            foreach (Load l in this.Loads)
            {
                newLC.Loads.Add(l.Clone(newLC));
            }

            return newLC;
        }

        //private IList<DOF> GetDOFs()
        //{
        //    IList<DOF> dofs = new List<DOF>();
        //    foreach (Load l in this.Loads)
        //    {
        //        dofs.Add(l.myDOF);
        //    }
        //    dofs = dofs.Distinct<DOF>().ToList<DOF>();

        //    return dofs;
        //}
    }
}
