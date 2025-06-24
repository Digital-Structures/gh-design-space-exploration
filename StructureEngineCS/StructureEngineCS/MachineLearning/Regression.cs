// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.Regression
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using StructureEngine.Model;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public abstract class Regression
  {
    public double? Parameter;

    public List<Observation> TrainData { get; set; }

    public double[,] XY
    {
      get
      {
        double[,] xy = new double[this.n, this.p + 1];
        for (int index1 = 0; index1 < this.n; ++index1)
        {
          xy[index1, this.p] = this.TrainData[index1].Output;
          for (int index2 = 0; index2 < this.p; ++index2)
            xy[index1, index2] = this.TrainData[index1].Features[index2];
        }
        return xy;
      }
    }

    public int n => this.TrainData.Count;

    public int p => this.TrainData[0].Features.Count;

    public abstract Regression Clone();

    public abstract void BuildModel(int n, double d, int b, int top);

    public ErrorMeasures ValidateModel(List<Observation> valSet, int top, Matrix<double> weights)
    {
      return new ErrorMeasures(valSet, this, top, weights);
    }

    public abstract double Predict(Observation test);

    private static Observation ScaleData(Observation o)
    {
      List<double> doubleList = new List<double>();
      foreach (IVariable designVariable in o.obsDesign.DesignVariables)
      {
        double point = designVariable.GetPoint();
        doubleList.Add(point);
      }
      Observation observation = o.ObservationClone();
      observation.Features = (IList<double>) doubleList;
      return observation;
    }

    public static DenseMatrix GetSampleMatrix(List<Observation> lobs)
    {
      int count1 = lobs.Count;
      int count2 = lobs[0].Features.Count;
      DenseMatrix sampleMatrix = new DenseMatrix(count1, count2);
      for (int index1 = 0; index1 < count1; ++index1)
      {
        for (int index2 = 0; index2 < count2; ++index2)
          ((Matrix<double>) sampleMatrix)[index1, index2] = Regression.ScaleData(lobs[index1]).Features[index2];
      }
      return sampleMatrix;
    }

    public static DenseVector GetSampleVector(Observation o)
    {
      int count = o.Features.Count;
      Observation observation = Regression.ScaleData(o);
      DenseVector sampleVector = new DenseVector(count);
      for (int index = 0; index < count; ++index)
        ((Vector<double>) sampleVector)[index] = observation.Features[index];
      return sampleVector;
    }

    public static DenseVector GetObsVector(List<Observation> lobs)
    {
      int count = lobs.Count;
      DenseVector obsVector = new DenseVector(count);
      for (int index = 0; index < count; ++index)
        ((Vector<double>) obsVector)[index] = lobs[index].Output;
      return obsVector;
    }
  }
}
