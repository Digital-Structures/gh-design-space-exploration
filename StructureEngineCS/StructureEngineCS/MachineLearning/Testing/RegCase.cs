// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.Testing.RegCase
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra;
using StructureEngine.Model;

#nullable disable
namespace StructureEngine.MachineLearning.Testing
{
  public class RegCase
  {
    public IDesign Problem;
    public int NumSamples;
    public int nUnderOne;
    public int nUnderOnePointFive;
    public string ProbName;
    public bool BuildRF;
    public bool BuildENN;
    public bool BuildKR;
    public Matrix<double> ErrorWeights;
    public SampleType SamplingPlan;

    public RegCase(
      IDesign p,
      int n,
      int n1,
      int n15,
      bool r,
      bool e,
      bool k,
      Matrix<double> w,
      SampleType s,
      string name)
    {
      this.Problem = p;
      this.NumSamples = n;
      this.nUnderOne = n1;
      this.nUnderOnePointFive = n15;
      this.BuildRF = r;
      this.BuildENN = e;
      this.BuildKR = k;
      this.ErrorWeights = w;
      this.SamplingPlan = s;
      this.ProbName = name;
    }

    public RegCase(int numSamples, bool r, bool e, bool k, Matrix<double> w, string name)
    {
      this.BuildRF = r;
      this.BuildENN = e;
      this.BuildKR = k;
      this.ErrorWeights = w;
      this.ProbName = name;
      this.NumSamples = numSamples;
    }

    public RegCase Clone()
    {
      return new RegCase(this.Problem.DesignClone(), this.NumSamples, this.nUnderOne, this.nUnderOnePointFive, this.BuildRF, this.BuildENN, this.BuildKR, this.ErrorWeights.Clone(), this.SamplingPlan, this.ProbName);
    }
  }
}
