// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.KrigingRegression
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public class KrigingRegression : Regression
  {
    private Vector<double> Theta;
    private Vector<double> LogTheta;
    private Matrix<double> Psi;
    private LU<double> PsiLU;
    private Matrix<double> X;
    private Vector<double> y;
    private int N;
    private double mu;
    private double sigsqr;
    private Vector<double> one;
    private Vector<double> P;

    public KrigingRegression(List<Observation> lobs)
    {
      this.TrainData = lobs;
      this.X = (Matrix<double>) Regression.GetSampleMatrix(lobs);
      this.y = (Vector<double>) Regression.GetObsVector(lobs);
      this.N = lobs.Count;
      this.one = (Vector<double>) CreateVector.Dense<double>(this.n, 1.0);
      this.P = (Vector<double>) CreateVector.Dense<double>(lobs[0].Features.Count, 2.0);
    }

    public override void BuildModel(int z1, double z2, int z3, int z4)
    {
      int count = this.TrainData[0].Features.Count;
      MLE mle = new MLE(this.TrainData, this.P);
      List<double> lowerBounds = new List<double>();
      List<double> upperBounds = new List<double>();
      for (int index = 0; index < count; ++index)
      {
        lowerBounds.Add(-3.0);
        upperBounds.Add(2.0);
      }
      Tuple<double, List<double>> best = new SimpleSearch((IFunction) mle, true, lowerBounds, upperBounds).FindBest(10, 20, 0.01);
      Vector<double> vector = (Vector<double>) CreateVector.Dense<double>(best.Item2.Count);
      for (int index = 0; index < best.Item2.Count; ++index)
        vector[index] = best.Item2[index];
      this.LogTheta = vector;
      this.Theta = MathUtility.PointwisePower((Vector<double>) CreateVector.Dense<double>(vector.Count, 10.0), this.LogTheta);
      Tuple<LU<double>, Matrix<double>, double, double> luFactor = mle.GetLUFactor(best.Item2);
      this.PsiLU = luFactor.Item1;
      this.Psi = luFactor.Item2;
      this.mu = luFactor.Item3;
      this.sigsqr = luFactor.Item4;
    }

    public override double Predict(Observation test)
    {
            Vector<double> x = GetSampleVector(test);

            Vector<double> psi = new DenseVector(n);
            for (int i = 0; i < n; i++)
            {
                Vector<double> v = X.Row(i) - x;
                v = MathUtility.PointwisePower(v, P);
                v = v.PointwiseMultiply(Theta);
                double sum = v.Sum();
                psi[i] = Math.Exp(-1 * sum);
            }

            Vector<double> summand = psi.ToRowMatrix().Multiply(PsiLU.Solve(y - one * mu));
            double f = mu + summand[0];

            return f;
        }

    public override Regression Clone()
    {
      return (Regression) new KrigingRegression(this.TrainData)
      {
        Theta = this.Theta.Clone(),
        Psi = this.Psi.Clone(),
        mu = this.mu,
        sigsqr = this.sigsqr,
        PsiLU = this.PsiLU
      };
    }
  }
}
