// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.IDesign
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.Distributions;
using StructureEngine.Evolutionary;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Model
{
  public interface IDesign
  {
    IDesign Crossover(IList<IDesign> seeds);

    IDesign Mutate(IContinuousDistribution dist, double rate);

    void Setup();

    IDivBooster GetDivBooster();

    IList<double> GetFeatures();

    IList<double[]> GetBounds();

    double GetOutput();

    double Score { get; }

    void UpdateScore(double s);

    double[] Dimensions { get; }

    double[] ZeroPoint { get; }

    int GetMaxPop();

    IDesign DesignClone();

    IDesign GenerateFromVars(double[] v);

    double[] GetPoints();

    List<IDesign> GetCornerDesigns();

    List<IVariable> DesignVariables { get; }

    double? CompTime { get; set; }
  }
}
