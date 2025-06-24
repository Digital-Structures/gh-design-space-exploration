// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.Sampling
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using StructureEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public class Sampling
  {
    public Sampling.IsCancel Cancel;
    public Sampling.OnSample Sample;
    private IDesign Typ;
    private IContinuousDistribution Dist;
    private int PopSize;
    private int NumGen;

    public Sampling(IDesign typ, IContinuousDistribution dist)
    {
      this.Typ = typ;
      this.Dist = dist;
      this.PopSize = 1;
      this.NumGen = 1;
      ((IDistribution) dist).RandomSource = Utilities.MyRandom;
    }

    public List<Observation> GetSample(
      int n,
      SampleType type,
      int nUnderOne,
      int nUnderOnePointFive)
    {
      List<Observation> sample;
      switch (type)
      {
        case SampleType.RandomUniform:
          sample = this.RandUniform(n);
          break;
        case SampleType.WeightedRandomUniform:
          sample = this.WeightedRandUniform(n, nUnderOne, nUnderOnePointFive);
          break;
        case SampleType.RandomLatinHypercube:
          sample = this.LatinHypercube(n, this.Typ.DesignVariables.Count);
          break;
        case SampleType.WeightedLatinHypercube:
          sample = this.WeightedLatinHypercube(n, nUnderOne, nUnderOnePointFive);
          break;
        case SampleType.Adaptive:
          sample = this.Adaptive(n, 4);
          break;
        default:
          sample = this.RandUniform(n);
          break;
      }
      return sample;
    }

    private List<Observation> GetCornerPoints()
    {
      List<Observation> cornerPoints = new List<Observation>();
      foreach (IDesign cornerDesign in this.Typ.GetCornerDesigns())
      {
        Observation observation = new Observation(cornerDesign);
        double output = observation.Output;
        cornerPoints.Add(observation);
      }
      return cornerPoints;
    }

    private List<Observation> RandUniform(int n)
    {
      List<KeyValuePair<double, Observation>> keyValuePairList = new List<KeyValuePair<double, Observation>>();
      while (keyValuePairList.Count < n)
      {
        if (this.Cancel != null && this.Cancel())
          return (List<Observation>) null;
        if (this.Sample != null)
          this.Sample(keyValuePairList.Count, n);
        Observation observation = new Observation(this.Typ.Mutate(this.Dist, 1.0));
        double output = observation.Output;
        keyValuePairList.Add(new KeyValuePair<double, Observation>(observation.Output, observation));
      }
      List<Observation> observationList = new List<Observation>();
      for (int index = 0; index < keyValuePairList.Count; ++index)
        observationList.Add(keyValuePairList[index].Value);
      return observationList;
    }

    private List<Observation> WeightedRandUniform(int n, int nUnderOne, int nUnderOnePointFive)
    {
      List<Observation> observationList = new List<Observation>();
      int num = 0;
      while (num < n)
      {
        if (this.Cancel != null && this.Cancel())
          return (List<Observation>) null;
        if (this.Sample != null)
          this.Sample(observationList.Count, n);
        Observation observation = new Observation(this.Typ.Mutate(this.Dist, 1.0));
        double output = observation.Output;
        observationList.Add(observation);
        ++num;
        if (observation.Output <= 1.5)
        {
          for (int index = 0; index < nUnderOnePointFive; ++index)
            observationList.Add(observation);
        }
        if (observation.Output <= 1.0)
        {
          for (int index = 0; index < nUnderOne; ++index)
            observationList.Add(observation);
        }
      }
      return observationList;
    }

    private Matrix<double> RandLatinHypercube(int n, int k)
    {
      Matrix<double> matrix1 = (Matrix<double>) CreateMatrix.Dense<double>(n, k);
      for (int index1 = 0; index1 < k; ++index1)
      {
        Vector<double> v = (Vector<double>) CreateVector.Dense<double>(n);
        for (int index2 = 0; index2 < n; ++index2)
          v[index2] = (double) index2;
        Vector<double> vector = this.Shuffle(v);
        matrix1.SetColumn(index1, vector);
      }
      Matrix<double> matrix2 = matrix1.Add((Matrix<double>) CreateMatrix.Dense<double>(n, k, 0.5)).PointwiseDivide((Matrix<double>) CreateMatrix.Dense<double>(n, k, (double) n));
      for (int index3 = 0; index3 < matrix2.RowCount; ++index3)
      {
        for (int index4 = 0; index4 < matrix2.ColumnCount; ++index4)
          matrix2[index3, index4] += ((IDistribution) this.Dist).RandomSource.NextDouble() * Math.Pow(10.0, -4.0);
      }
      return matrix2;
    }

    private List<Observation> LatinHypercube(int n, int k)
    {
      return this.ProjectPoints(this.RandLatinHypercube(n, k));
    }

    private List<Observation> WeightedLatinHypercube(int n, int nUnderOne, int nUnderOnePointFive)
    {
      List<Observation> observationList1 = this.BestLatinHypercube(n, this.PopSize, this.NumGen);
      if (observationList1 == null)
        return (List<Observation>) null;
      observationList1.Sort((IComparer<Observation>) new ObsCompare());
      List<Observation> observationList2 = new List<Observation>();
      int index1 = 0;
      int num = 0;
      while (num < n)
      {
        if (index1 < observationList1.Count)
        {
          double output = observationList1[index1].Output;
          observationList2.Add(observationList1[index1]);
          ++num;
          if (output <= 1.5)
          {
            for (int index2 = 0; index2 < nUnderOnePointFive; ++index2)
              observationList2.Add(observationList1[index1]);
          }
          if (output <= 1.0)
          {
            for (int index3 = 0; index3 < nUnderOne; ++index3)
              observationList2.Add(observationList1[index1]);
          }
        }
        else
        {
          List<Observation> collection = this.BestLatinHypercube(n, this.PopSize, this.NumGen);
          if (collection == null)
            return (List<Observation>) null;
          observationList1.AddRange((IEnumerable<Observation>) collection);
        }
        ++index1;
      }
      return observationList2;
    }

    private List<Observation> Adaptive(int n, int iters)
    {
      int num1 = n / iters;
      double num2 = 1.0 / (double) iters;
      double rate = 1.0;
      List<Observation> observationList1 = new List<Observation>();
      List<Observation> observationList2 = new List<Observation>();
      observationList1.Add(new Observation(this.Typ));
      for (int index1 = 0; index1 < iters; ++index1)
      {
        List<Observation> list = observationList1.ToList<Observation>();
        observationList1.Clear();
        for (int index2 = 0; index2 < num1; ++index2)
        {
          if (this.Cancel != null && this.Cancel())
            return (List<Observation>) null;
          if (this.Sample != null)
            this.Sample(observationList2.Count, n);
          Observation observation = new Observation(list[((IDistribution) this.Dist).RandomSource.Next(list.Count)].obsDesign.DesignClone().Mutate(this.Dist, rate));
          double output = observation.Output;
          observationList1.Add(observation);
          if (output <= 1.5)
            observationList1.Add(observation);
          if (output <= 1.0)
            observationList1.Add(observation);
        }
        rate -= num2;
        observationList2.AddRange((IEnumerable<Observation>) observationList1);
      }
      return observationList2;
    }

    private Vector<double> Shuffle(Vector<double> v)
    {
      Random randomSource = ((IDistribution) this.Dist).RandomSource;
      int count = v.Count;
      while (count > 1)
      {
        --count;
        int num1 = randomSource.Next(count + 1);
        double num2 = v[num1];
        v[num1] = v[count];
        v[count] = num2;
      }
      return v;
    }

    private List<Observation> BestLatinHypercube(int n, int p, int g)
    {
      int count = this.Typ.DesignVariables.Count;
      int[] numArray = new int[4]{ 5, 10, 50, 500 };
      Matrix<double> XStart = this.RandLatinHypercube(n, count);
      List<Matrix<double>> matrixList = new List<Matrix<double>>();
      foreach (int q in numArray)
        matrixList.Add(this.GetBestPhiq(XStart, p, g, (double) q));
      matrixList.Sort((IComparer<Matrix<double>>) new MMComparer());
      return this.ProjectPoints(matrixList[0]);
    }

    private List<Observation> ProjectPoints(Matrix<double> X)
    {
      int rowCount = X.RowCount;
      int columnCount = X.ColumnCount;
      List<Observation> observationList = new List<Observation>();
      for (int index1 = 0; index1 < rowCount; ++index1)
      {
        if (this.Cancel != null && this.Cancel())
          return (List<Observation>) null;
        if (this.Sample != null)
          this.Sample(observationList.Count, rowCount);
        double[] v = new double[columnCount];
        for (int index2 = 0; index2 < columnCount; ++index2)
          v[index2] = X[index1, index2];
        Observation observation = new Observation(this.Typ.GenerateFromVars(v));
        double output = observation.Output;
        observationList.Add(observation);
      }
      return observationList;
    }

    private Matrix<double> Perturb(Matrix<double> X_old, int pertNum, Random rand)
    {
      Matrix<double> matrix = X_old.Clone();
      int rowCount = matrix.RowCount;
      int columnCount = matrix.ColumnCount;
      for (int index = 0; index < pertNum; ++index)
      {
        int num1 = (int) Math.Floor(rand.NextDouble() * (double) columnCount);
        int num2 = 1;
        int num3;
        for (num3 = 1; num2 == num3; num3 = (int) Math.Floor(rand.NextDouble() * (double) rowCount))
          num2 = (int) Math.Floor(rand.NextDouble() * (double) rowCount);
        double num4 = matrix[num2, num1];
        matrix[num2, num1] = matrix[num3, num1];
        matrix[num3, num1] = num4;
      }
      return matrix;
    }

    private Matrix<double> GetBestPhiq(Matrix<double> XStart, int p, int g, double q)
    {
      Matrix<double> X_old = XStart;
      for (int index = 0; index < g; ++index)
      {
        List<Matrix<double>> XList = new List<Matrix<double>>();
        XList.Add(X_old);
        while (XList.Count < p)
        {
          int pertNum = ((IDistribution) this.Dist).RandomSource.Next((int) Math.Floor((double) X_old.RowCount / 2.0));
          XList.Add(this.Perturb(X_old, pertNum, ((IDistribution) this.Dist).RandomSource));
        }
        X_old = this.SortPhiq(XList, q)[0];
      }
      return X_old;
    }

    private double GetPhiq(Matrix<double> X, double q)
    {
      Tuple<List<double>, List<int>> distances = MathUtility.GetDistances(X);
      Vector<double> vector = MathUtility.ListToVector(distances.Item1);
      return Math.Pow(MathUtility.ListToVector(distances.Item2).PointwiseMultiply(MathUtility.PointwisePower(vector, (Vector<double>) CreateVector.Dense<double>(vector.Count, -q))).Sum(), 1.0 / q);
    }

    private List<Matrix<double>> SortPhiq(List<Matrix<double>> XList, double q)
    {
      List<Tuple<Matrix<double>, double>> tupleList = new List<Tuple<Matrix<double>, double>>();
      for (int index = 0; index < XList.Count; ++index)
        tupleList.Add(new Tuple<Matrix<double>, double>(XList[index], this.GetPhiq(XList[index], q)));
      tupleList.Sort((IComparer<Tuple<Matrix<double>, double>>) new PhiqCompararer());
      List<Matrix<double>> matrixList = new List<Matrix<double>>();
      foreach (Tuple<Matrix<double>, double> tuple in tupleList)
        matrixList.Add(tuple.Item1);
      return matrixList;
    }

    public delegate bool IsCancel();

    public delegate void OnSample(int num, int total);
  }
}
