// Decompiled with JetBrains decompiler
// Type: StructureEngine.MachineLearning.Observation
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.MachineLearning
{
  public class Observation
  {
    private double? _output;

    public Observation(IDesign s)
    {
      this.obsDesign = s;
      this.Features = this.obsDesign.GetFeatures();
    }

    public Observation(List<double> features, double output)
    {
      this.Output = output;
      this.Features = (IList<double>) features;
      this.obsDesign = (IDesign) new ComputedStructure();
    }

    public IList<double> Features { get; set; }

    public double Output
    {
      get
      {
        if (!this._output.HasValue)
          this._output = new double?(this.obsDesign.GetOutput());
        return this._output.Value;
      }
      set => this._output = new double?(value);
    }

    public int Rank { get; set; }

    public int PredictedRank { get; set; }

    public double Predicted { get; set; }

    public IDesign obsDesign { get; set; }

    public Observation ObservationClone()
    {
      Observation observation = new Observation(this.obsDesign);
      observation.Output = this.Output;
      observation.Features = (IList<double>) new List<double>();
      foreach (double feature in (IEnumerable<double>) this.Features)
        observation.Features.Add(feature);
      return observation;
    }
  }
}
