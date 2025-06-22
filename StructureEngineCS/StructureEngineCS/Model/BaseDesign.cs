// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.BaseDesign
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Model
{
  public abstract class BaseDesign
  {
    public abstract List<IVariable> DesignVariables { get; }

    public IList<double> GetFeatures()
    {
      IList<double> features = (IList<double>) new List<double>();
      foreach (IVariable designVariable in this.DesignVariables)
        features.Add(designVariable.Value);
      return features;
    }

    public IList<double[]> GetBounds()
    {
      IList<double[]> bounds = (IList<double[]>) new List<double[]>();
      foreach (IVariable designVariable in this.DesignVariables)
        bounds.Add(new double[2]
        {
          designVariable.Min,
          designVariable.Max
        });
      return bounds;
    }

    public double? CompTime { get; set; }

    public double[] GetPoints()
    {
      double[] points = new double[this.DesignVariables.Count];
      int index = 0;
      foreach (IVariable designVariable in this.DesignVariables)
      {
        double point = designVariable.GetPoint();
        points[index] = point;
        ++index;
      }
      return points;
    }

    public List<IDesign> GetCornerDesigns()
    {
      if (this.DesignVariables.Count != 2)
        return new List<IDesign>();
      return new List<IDesign>()
      {
        this.GenerateFromVars(new double[2]
        {
          this.DesignVariables[0].Min,
          this.DesignVariables[1].Min
        }),
        this.GenerateFromVars(new double[2]
        {
          this.DesignVariables[0].Min,
          this.DesignVariables[1].Max
        }),
        this.GenerateFromVars(new double[2]
        {
          this.DesignVariables[0].Max,
          this.DesignVariables[1].Min
        }),
        this.GenerateFromVars(new double[2]
        {
          this.DesignVariables[0].Max,
          this.DesignVariables[1].Max
        })
      };
    }

    public void CheckConstraints()
    {
      foreach (IVariable designVariable in this.DesignVariables)
      {
        if (!designVariable.CheckConstraint())
          designVariable.FixConstraint();
      }
    }

    public void SetConstraints()
    {
      foreach (IVariable designVariable in this.DesignVariables)
        designVariable.SetConstraint();
    }

    public double SizeDesignSpace()
    {
      double d = 0.0;
      foreach (IVariable designVariable in this.DesignVariables)
        d += Math.Pow(designVariable.Max - designVariable.Min, 2.0);
      return Math.Sqrt(d);
    }

    public double GetDistance(IDesign that)
    {
      double d = 0.0;
      for (int index = 0; index < this.DesignVariables.Count; ++index)
      {
        double num = Math.Pow(this.DesignVariables[index].Value - that.DesignVariables[index].Value, 2.0);
        d += num;
      }
      return Math.Sqrt(d);
    }

    public virtual IDesign GenerateFromVars(double[] v) => this.DummyClone();

    private IDesign DummyClone() => (IDesign) new ComputedStructure();

    public double PredictedScore { get; set; }
  }
}
