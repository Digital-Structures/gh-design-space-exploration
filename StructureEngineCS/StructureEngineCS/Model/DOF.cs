// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.DOF
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Model
{
  public class DOF : IVariable
  {
    public Node MyNode;
    public bool IsX;
    private double _coord;
    public double Coord;
    public Dictionary<LoadCase, double> Disp;
    public double? AllowableVariation;

    public DOF(double coord)
    {
      this.Coord = coord;
      this._coord = coord;
      this.Relations = new List<ParametricRelation>();
      this.Disp = new Dictionary<LoadCase, double>();
    }

    public DOF(double coord, double _coord)
    {
      this.Coord = coord;
      this._coord = _coord;
      this.Relations = new List<ParametricRelation>();
      this.Disp = new Dictionary<LoadCase, double>();
    }

    public double Min
    {
      get => this.AllowableVariation.HasValue ? this._coord - this.AllowableVariation.Value : 0.0;
    }

    public double Max
    {
      get => this.AllowableVariation.HasValue ? this._coord + this.AllowableVariation.Value : 0.0;
    }

    public bool Pinned { get; set; }

    public int Index { get; set; }

    public bool Free { get; set; }

    public bool PreFix { get; set; }

    public double Value
    {
      get => this.Coord;
      set => this.Coord = value;
    }

    public List<ParametricRelation> Relations { get; set; }

    public void CopyTo(DOF newDOF)
    {
      newDOF.Coord = this.Coord;
      newDOF._coord = this._coord;
      newDOF.Pinned = this.Pinned;
      newDOF.Index = this.Index;
      newDOF.Free = this.Free;
      newDOF.PreFix = this.PreFix;
      newDOF.AllowableVariation = this.AllowableVariation;
    }

    public void Mutate(double globalrate, IContinuousDistribution dist)
    {
      double num = !this.AllowableVariation.HasValue ? 0.0 : globalrate * this.AllowableVariation.Value;
      //dist.Mean = this.Coord;
      //dist.StdDev = num;
            dist = new Normal(this.Coord, num);
            this.Coord = ((IContinuousDistribution) dist).Sample();
    }

    public void Crossover(List<IVariable> mylist)
    {
      double num1 = 0.0;
      double num2 = 0.0;
      foreach (IVariable variable in mylist)
      {
        double num3 = Utilities.MyRandom.NextDouble();
        DOF dof = (DOF) variable;
        num1 += dof.Coord * num3;
        num2 += num3;
      }
      this.Coord = num1 / num2;
    }

    public bool CheckConstraint() => this.Coord < this.Max && this.Coord > this.Min;

    public void FixConstraint()
    {
      if (this.CheckConstraint())
        return;
      if (this.Coord > this.Max)
      {
        this.Coord = this.Max;
      }
      else
      {
        if (this.Coord >= this.Min)
          return;
        this.Coord = this.Min;
      }
    }

    public void SetConstraint() => this._coord = this.Coord;

    public int GetBytes()
    {
      return Math.Max(4, Convert.ToInt32(Math.Log(this.AllowableVariation.Value * 2.0, 2.0)));
    }

    public void Project(double d)
    {
      this.Value = this.Min + d * 2.0 * this.AllowableVariation.Value;
    }

    public void ShiftCenter(double c)
    {
      this._coord = this.Min + c * 2.0 * this.AllowableVariation.Value;
    }

    public double GetPoint() => (this.Value - this.Min) / (2.0 * this.AllowableVariation.Value);
  }
}
