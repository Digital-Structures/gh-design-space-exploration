// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.EnsembleNeuralNetRegression
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public class EnsembleNeuralNetRegression : Regression
  {
    public alglib.mlpensemble enn;
    public int info;
    public alglib.mlpreport rep_mlp;
    public alglib.mlpcvreport rep_mlpcv;

    public EnsembleNeuralNetRegression(List<Observation> lobs) => this.TrainData = lobs;

    public override void BuildModel(int nnodes, double decay, int restarts, int top)
    {
      alglib.mlpecreate1(this.p, nnodes, 1, 10, out this.enn);
      alglib.mlpebagginglbfgs(this.enn, this.XY, this.n, decay, restarts, 0.01, 10, out this.info, out this.rep_mlp, out this.rep_mlpcv);
      this.Parameter = new double?((double) nnodes);
    }

    public override double Predict(Observation test)
    {
      long ticks = DateTime.Now.Ticks;
      double[] numArray1 = new double[this.p];
      for (int index = 0; index < this.p; ++index)
        numArray1[index] = test.Features[index];
      double[] numArray2 = new double[1];
      alglib.mlpeprocess(this.enn, numArray1, ref numArray2);
      test.Predicted = numArray2[0];
      double totalMilliseconds = new TimeSpan(DateTime.Now.Ticks - ticks).TotalMilliseconds;
      return numArray2[0];
    }

    public override Regression Clone()
    {
      EnsembleNeuralNetRegression neuralNetRegression = new EnsembleNeuralNetRegression(this.TrainData);
      neuralNetRegression.enn = this.enn;
      neuralNetRegression.info = this.info;
      neuralNetRegression.rep_mlp = this.rep_mlp;
      neuralNetRegression.rep_mlpcv = this.rep_mlpcv;
      neuralNetRegression.Parameter = this.Parameter;
      return (Regression) neuralNetRegression;
    }
  }
}
