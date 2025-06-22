using System;
using System.Collections.Generic;

namespace StructureEngine.Model
{
    public abstract class BaseDesign
    {
        public abstract List<IVariable> DesignVariables
        {
            get;
        }
        public IList<double> GetFeatures()
        {
            IList<double> vars = new List<double>();
            foreach (IVariable v in this.DesignVariables)
            {
                vars.Add(v.Value);
            }
            return vars;
        }
        public IList<double[]> GetBounds()
        {
            IList<double[]> bounds = new List<double[]>();
            foreach (IVariable v in this.DesignVariables)
            {
                bounds.Add(new double[] { v.Min, v.Max });
            }
            return bounds;
        }

        public double? CompTime
        {
            get;
            set;
        }

        public double[] GetPoints()
        {
            double[] points = new double[DesignVariables.Count];
            int i = 0;
            foreach (IVariable dv in this.DesignVariables)
            {
                double p = dv.GetPoint();
                points[i] = p;
                i++;
            }
            return points;

        }

        public List<IDesign> GetCornerDesigns()
        {
            if (this.DesignVariables.Count == 2)
            {
                var corners = new List<IDesign>();
                corners.Add(GenerateFromVars(new double[] { DesignVariables[0].Min, DesignVariables[1].Min }));
                corners.Add(GenerateFromVars(new double[] { DesignVariables[0].Min, DesignVariables[1].Max }));
                corners.Add(GenerateFromVars(new double[] { DesignVariables[0].Max, DesignVariables[1].Min }));
                corners.Add(GenerateFromVars(new double[] { DesignVariables[0].Max, DesignVariables[1].Max }));

                return corners;
            }

            else
            {
                return new List<IDesign>();
            }
        }

        public void CheckConstraints()
        {
            foreach (IVariable var in this.DesignVariables)
            {
                if (!var.CheckConstraint())
                {
                    var.FixConstraint();
                }
            }
        }

        public void SetConstraints()
        {
            foreach (IVariable var in this.DesignVariables)
            {
                var.SetConstraint();
            }
        }

        public double SizeDesignSpace()
        {
            double size = 0;
            foreach (IVariable var in this.DesignVariables)
            {
                size += Math.Pow(var.Max - var.Min, 2);
            }
            size = Math.Sqrt(size);
            return size;
        }

        public double GetDistance(IDesign that)
        {
            double dist = 0;
            for (int i = 0; i < this.DesignVariables.Count; i++)
            {
                double var1 = this.DesignVariables[i].Value;
                double var2 = that.DesignVariables[i].Value;
                double sq = Math.Pow((var1 - var2), 2);
                dist += sq;
            }
            dist = Math.Sqrt(dist);
            return dist;
        }

        public virtual IDesign GenerateFromVars(double[] v)
        {
            return this.DummyClone();
        }

        private IDesign DummyClone()
        {
            return new ComputedStructure();
        }

        public Double PredictedScore
        {
            get;
            set;
        }
    }
}
