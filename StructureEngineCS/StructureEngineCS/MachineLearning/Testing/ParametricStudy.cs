// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.Testing.ParametricStudy
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using StructureEngine.Analysis;
using StructureEngine.Model;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.MachineLearning.Testing
{
  public class ParametricStudy
  {
    public List<RegCase> Cases;
    private int NumRuns;

    public ParametricStudy()
    {
      this.Cases = new List<RegCase>();
      this.NumRuns = 10;
      this.AddCases();
    }

    public List<RegressionReport> Run(IAnalysis MyAnalysis)
    {
      List<RegressionReport> regressionReportList = new List<RegressionReport>();
      SurrogateModelBuilder surrogateModelBuilder = new SurrogateModelBuilder(MyAnalysis);
      foreach (RegCase r in this.Cases)
        regressionReportList.Add(surrogateModelBuilder.BuildModel(r));
      return regressionReportList;
    }

    private void AddCases()
    {
      List<Tuple<IDesign, string>> tupleList = new List<Tuple<IDesign, string>>()
      {
        new Tuple<IDesign, string>(new StructureSetup().Designs[3], "RigidFrame")
      };
      List<int> intList = new List<int>()
      {
        20,
        50,
        100,
        200,
        400
      };
      List<bool[]> flagArrayList = new List<bool[]>()
      {
        new bool[3]{ true, false, false }
      };
      new List<bool>() { false };
      new List<bool>() { false };
      List<Matrix<double>> matrixList = new List<Matrix<double>>()
      {
        (Matrix<double>) CreateMatrix.Dense<double>(1, 6, new double[6]
        {
          0.0,
          0.0,
          1.0,
          1.0,
          1.0,
          1.0
        })
      };
      List<SampleType> sampleTypeList = new List<SampleType>()
      {
        SampleType.WeightedLatinHypercube
      };
      List<int[]> numArrayList = new List<int[]>()
      {
        new int[2]{ 2, 1 }
      };
      foreach (Tuple<IDesign, string> tuple in tupleList)
      {
        foreach (int n in intList)
        {
          foreach (bool[] flagArray in flagArrayList)
          {
            foreach (Matrix<double> w in matrixList)
            {
              foreach (SampleType s in sampleTypeList)
              {
                foreach (int[] numArray in numArrayList)
                {
                  RegCase regCase = new RegCase(tuple.Item1, n, numArray[0], numArray[1], flagArray[0], flagArray[1], flagArray[2], w, s, tuple.Item2);
                  for (int index = 0; index < this.NumRuns; ++index)
                    this.Cases.Add(regCase);
                }
              }
            }
          }
        }
      }
    }
  }
}
