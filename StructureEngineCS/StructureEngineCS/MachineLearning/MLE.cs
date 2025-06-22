// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.MLE
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
  public class MLE : IFunction
  {
    private List<Observation> LObs;
    private Vector<double> p;
    private Matrix<double> X;
    private Vector<double> y;
    private Vector<double> one;
    private int n;

    public MLE(List<Observation> lobs, Vector<double> power)
    {
      this.LObs = lobs;
      this.p = power;
      this.X = (Matrix<double>) Regression.GetSampleMatrix(this.LObs);
      this.y = (Vector<double>) Regression.GetObsVector(this.LObs);
    }

    public double GetOutput(List<double> inputs)
    {
      this.n = this.LObs.Count;
      this.one = (Vector<double>) CreateVector.Dense<double>(this.n, 1.0);
      Matrix<double> psi = this.GetPsi(inputs);
      double output;
      if (psi.Determinant() <= 0.0)
      {
        output = Math.Pow(10.0, 4.0);
      }
      else
      {
        Tuple<double[], LU<double>> tuple = this.GetParams(psi, this.one, this.n);
        Vector<double> vector1 = ((LU<double>) tuple.Item2).U.Diagonal();
        Vector<double> vector2 = (Vector<double>) CreateVector.Dense<double>(vector1.Count);
        for (int index = 0; index < vector1.Count; ++index)
          vector2[index] = Math.Log(Math.Abs(vector1[index]));
        double num = vector2.Sum();
        output = -1.0 * ((double) -(this.n / 2) * Math.Log(tuple.Item1[1]) - 0.5 * num);
      }
      return output;
    }

    public Tuple<LU<double>, Matrix<double>, double, double> GetLUFactor(List<double> inputs)
    {
      Matrix<double> psi = this.GetPsi(inputs);
      Tuple<double[], LU<double>> tuple = this.GetParams(psi, this.one, this.n);
      return new Tuple<LU<double>, Matrix<double>, double, double>(tuple.Item2, psi, tuple.Item1[0], tuple.Item1[1]);
    }

    private Matrix<double> GetPsi(List<double> inputs)
    {
            Vector<double> parameters = new DenseVector(inputs.Count);
            for (int i = 0; i < inputs.Count; i++)
            {
                parameters[i] = inputs[i];
            }

            Vector<double> ten =  CreateVector.Dense<double>(parameters.Count, 10);
            Vector<double> theta = MathUtility.PointwisePower(ten, parameters);

            int n = LObs.Count;
            Vector<double> one =  CreateVector.Dense<double>(n, 1);

            Matrix<double> Psi = CreateMatrix.Dense<double>(n, n, 0);
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    Vector<double> v = X.Row(i) - (X.Row(j));
                    v = MathUtility.PointwisePower(v, p);
                    v = v.PointwiseMultiply(theta);
                    double sum = v.Sum();
                    Psi[i, j] = Math.Exp(-1 * sum);
                }
            }
            Matrix<double> Psit = Psi.Transpose();
            Matrix<double> eye = new DiagonalMatrix(n, n, 1);
            Matrix<double> noise = eye.Multiply(Math.Pow(2, -52));
            Psi = Psi.Add(Psit).Add(eye).Add(noise);

            return Psi;
        }

    private Tuple<double[], LU<double>> GetParams(Matrix<double> Psi, Vector<double> one, int n)
    {
            LU<double> dlu = ((DenseMatrix)Psi).LU();
            Vector<double> top = one.ToRowMatrix().Multiply(dlu.Solve(y));
            Vector<double> bottom = one.ToRowMatrix().Multiply(dlu.Solve(one));

            double mu = top[0] / bottom[0];

            Vector<double> sigsqr_v = ((y - one * mu).ToRowMatrix().Multiply(dlu.Solve((y - one * mu)))).Divide(n);
            double sigsqr = sigsqr_v[0];

            return new Tuple<double[], LU<double>>(new double[] { mu, sigsqr }, dlu);
        }
  }
}
