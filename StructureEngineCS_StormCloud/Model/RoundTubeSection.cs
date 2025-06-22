// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.RoundTubeSection
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B2AD7AF7-B053-4B89-AC37-223D1F22213F
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\Testing\bin\Debug\StructureEngineCS.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Model
{
  public class RoundTubeSection : ISection
  {
    public RoundTubeSection(double p, string n)
    {
      this.SectionParameter = p;
      this.Name = n;
    }

    public RoundTubeSection()
    {
      this.SectionParameter = 0.05;
      this.Name = "RT0.05";
    }

    public SectionType Type => SectionType.RoundTube;

    public string Name { get; set; }

    public double GetReqEnvArea(
      Dictionary<LoadCase, List<double>> intforces,
      double sigma,
      double E,
      double L)
    {
      List<double> source = new List<double>();
      foreach (List<double> forces in intforces.Values)
        source.Add(this.GetReqArea(forces, sigma, E, L));
      return source.Max();
    }

    public double GetReqArea(List<double> forces, double sigma, double E, double L)
    {
      double force1 = forces[0];
      double force2 = forces[1];
      double force3 = forces[2];
      double force4 = forces[3];
      if (force1 > 0.0)
        return Math.Abs(force1) / sigma;
      double x1 = 1.0 - this.SectionParameter;
      double num = 3.0 * (Math.Abs(force1) * Math.Pow(L, 2.0)) / (Math.Pow(Math.PI, 2.0) * E);
      double x2 = Math.Pow(4.0 / ((1.0 - Math.Pow(x1, 4.0)) * Math.PI) * num, 0.25);
      return Math.Max(Math.Abs(force1) / sigma, Math.PI * Math.Pow(x2, 2.0) * (1.0 - Math.Pow(x1, 2.0)));
    }

    public double GetReqThickness(double reqArea)
    {
      double x = 1.0 - this.SectionParameter;
      return 2.0 * Math.Sqrt(reqArea / (Math.PI * (1.0 - Math.Pow(x, 2.0))));
    }

    public double GetReqMomInertia(double reqArea)
    {
      double x = 1.0 - this.SectionParameter;
      return Math.PI / 4.0 * Math.Pow(Math.Sqrt(reqArea / (Math.PI * (1.0 - Math.Pow(x, 2.0)))), 4.0) * (1.0 - Math.Pow(x, 4.0));
    }

    public double SectionParameter { get; set; }

    public ISection SectionClone()
    {
      return (ISection) new RoundTubeSection(this.SectionParameter, this.Name);
    }
  }
}
