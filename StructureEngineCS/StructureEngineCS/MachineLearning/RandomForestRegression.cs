// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.RandomForestRegression
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public class RandomForestRegression : Regression
  {
    public alglib.decisionforest df;
    public int info;
    public alglib.dfreport rep;

    public RandomForestRegression(List<Observation> lobs) => this.TrainData = lobs;

    public override void BuildModel(int ntrees, double r, int blank, int top)
    {
      alglib.dfbuildrandomdecisionforest(this.XY, this.n, this.p, 1, ntrees, r, out this.info, out this.df, out this.rep);
      this.Parameter = new double?(r);
    }

    public override double Predict(Observation test)
    {
      long ticks = DateTime.Now.Ticks;
      double[] numArray1 = new double[this.p];
      for (int index = 0; index < this.p; ++index)
        numArray1[index] = test.Features[index];
     
            double[] numArray2 = new double[1];
      alglib.dfprocess(this.df, numArray1, ref numArray2);
      test.Predicted = numArray2[0];
      double totalMilliseconds = new TimeSpan(DateTime.Now.Ticks - ticks).TotalMilliseconds;
      return numArray2[0];
    }

    public override Regression Clone()
    {
      RandomForestRegression forestRegression = new RandomForestRegression(this.TrainData);
      forestRegression.df = this.df;
      forestRegression.info = this.info;
      forestRegression.rep = this.rep;
      forestRegression.Parameter = this.Parameter;
      return (Regression) forestRegression;
    }
  }
}
