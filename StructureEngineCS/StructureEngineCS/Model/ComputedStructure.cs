// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.ComputedStructure
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using StructureEngine.Analysis;
using StructureEngine.Evolutionary;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Model
{
  public class ComputedStructure : Structure, IDesign
  {
    private double _DetK;
    private double? _Score;
    public bool Analyzed;
    private Matrix<double> StiffnessConstant;
    private Matrix<double> StiffnessVariable;

    public ComputedStructure()
    {
    }

    public ComputedStructure(Structure s) => s.CopyTo((Structure) this);

    public override IDesign GenerateFromVars(double[] v)
    {
      ComputedStructure fromVars = (ComputedStructure) this.DesignClone();
      for (int index = 0; index < v.Length; ++index)
        fromVars.DesignVariables[index].Project(v[index]);
      fromVars.CheckConstraints();
      fromVars.EnforceRelationships();
      return (IDesign) fromVars;
    }

    public double DetK
    {
      get
      {
        if (!this.Analyzed)
          this.MyAnalysis.Analyze((IDesign) this);
        return this._DetK;
      }
      set => this._DetK = value;
    }

    public double Score
    {
      get => !this._Score.HasValue ? this.MyAnalysis.Analyze((IDesign) this) : this._Score.Value;
    }

    public void UpdateScore(double s) => this._Score = new double?(s);

    private IAnalysis MyAnalysis
    {
      get
      {
        if (this.StructType == Structure.StructureType.Truss)
          return (IAnalysis) new TrussAnalysis();
        if (this.StructType == Structure.StructureType.Frame)
          return (IAnalysis) new EquivFrameAnalysis();
        throw new Exception("Structural type must be defined.");
      }
    }

    public void SetStart()
    {
      if (this.StructType == Structure.StructureType.Frame)
      {
        EquivFrameAnalysis myAnalysis = (EquivFrameAnalysis) this.MyAnalysis;
        myAnalysis.SetMemberAreas(this);
        myAnalysis.OrderEnvNodes(this);
      }
      else if (this.StructType != Structure.StructureType.Truss)
        throw new Exception("Structure must have a type.");
    }

    private void MemberContribution(ComputedMember m, Matrix<double> K, int dofs)
    {
      for (int index1 = 0; index1 < 2 * dofs; ++index1)
      {
        for (int index2 = 0; index2 < 2 * dofs; ++index2)
        {
          int index3 = m.LocalDOFs[index1].Index;
          int index4 = m.LocalDOFs[index2].Index;
          double num = m.CalculateStiffness()[index1, index2];
          K[index3, index4] += num;
        }
      }
    }

    private void RoundMatrix(Matrix<double> K)
    {
      for (int index1 = 0; index1 < this.DOFs.Count; ++index1)
      {
        for (int index2 = 0; index2 < this.DOFs.Count; ++index2)
          K[index1, index2] = Math.Round(K[index1, index2], 8);
      }
    }

    private Matrix<double> CalculateStiffnessConst()
    {
      int length = this.Members[0].NodeI.DOFs.Length;
      Matrix<double> K = (Matrix<double>) new DenseMatrix(this.DOFs.Count);
      foreach (ComputedMember unaffectedMember in this.UnaffectedMembers)
        this.MemberContribution(unaffectedMember, K, length);
      return K;
    }

    private Matrix<double> CalculateStiffnessVar()
    {
      long ticks = DateTime.Now.Ticks;
      int length = this.Members[0].NodeI.DOFs.Length;
      Matrix<double> K = (Matrix<double>) new DenseMatrix(this.DOFs.Count);
      foreach (ComputedMember affectedMember in this.AffectedMembers)
        this.MemberContribution(affectedMember, K, length);
      int milliseconds = new TimeSpan(DateTime.Now.Ticks - ticks).Milliseconds;
      return K;
    }

    public Matrix<double> StiffnessMatrix
    {
      get
      {
        if (this.StiffnessConstant == null)
          this.StiffnessConstant = this.CalculateStiffnessConst();
        if (this.StiffnessVariable == null)
          this.StiffnessVariable = this.CalculateStiffnessVar();
        return StiffnessVariable + StiffnessConstant;
            }
    }

    public void ClearStiffness()
    {
      this.StiffnessConstant = (Matrix<double>) null;
      this.StiffnessVariable = (Matrix<double>) null;
    }

    public IEnumerable<ComputedMember> ComputedMembers => this.Members.Cast<ComputedMember>();

    public override Member GetNewMember(Node i, Node j) => (Member) new ComputedMember(i, j);

    public IDesign DesignClone() => (IDesign) this.CloneImpl();

    protected override Structure CloneImpl()
    {
      ComputedStructure s = new ComputedStructure();
      this.CopyTo((Structure) s);
      return (Structure) s;
    }

    internal override void CopyTo(Structure s)
    {
      base.CopyTo(s);
      if (!(s is ComputedStructure computedStructure))
        return;
      computedStructure.CompTime = this.CompTime;
      computedStructure.StiffnessConstant = this.StiffnessConstant;
      computedStructure.Analyzed = false;
    }

    public IDesign Crossover(IList<IDesign> seeds)
    {
      ComputedStructure computedStructure = (ComputedStructure) this.DesignClone();
      if (seeds == null || seeds.Count == 0)
        return (IDesign) computedStructure;
      for (int index = 0; index < this.DesignVariables.Count; ++index)
      {
        IVariable designVariable = computedStructure.DesignVariables[index];
        List<IVariable> mylist = new List<IVariable>();
        foreach (ComputedStructure seed in (IEnumerable<IDesign>) seeds)
          mylist.Add(seed.DesignVariables[index]);
        designVariable.Crossover(mylist);
      }
      return (IDesign) computedStructure;
    }

    public void Setup()
    {
      int length = this.Nodes[0].DOFs.Length;
      for (int index = 0; index < this.Nodes.Count; ++index)
        this.Nodes[index].Index = index;
    }

    public IDesign Mutate(IContinuousDistribution dist, double rate)
    {
      ComputedStructure computedStructure = (ComputedStructure) this.DesignClone();
      foreach (IVariable designVariable in computedStructure.DesignVariables)
        designVariable.Mutate(rate, dist);
      computedStructure.CheckConstraints();
      computedStructure.EnforceRelationships();
      return (IDesign) computedStructure;
    }

    public IDivBooster GetDivBooster() => (IDivBooster) new ParDivBooster();

    public double GetOutput() => this.Score;

    public int GetMaxPop()
    {
      int num = 0;
      foreach (IVariable designVariable in this.DesignVariables)
        num += designVariable.GetBytes();
      return Math.Min(num * 4 * 2, 150);
    }
  }
}
