// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.RectangularSection
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B2AD7AF7-B053-4B89-AC37-223D1F22213F
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\Testing\bin\Debug\StructureEngineCS.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Model
{
  public class RectangularSection : ISection
  {
    public RectangularSection(double p, string n)
    {
      this.SectionParameter = p;
      this.Name = n;
    }

    public RectangularSection()
    {
      this.SectionParameter = 0.009525;
      this.Name = "RecPC";
    }

    public SectionType Type => SectionType.Rectangular;

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
      double num1 = 1.0 * (Math.Abs(force1) * Math.Pow(L, 2.0)) / (Math.Pow(Math.PI, 2.0) * E);
      double num2 = Math.Min(Math.Pow(num1 * 12.0 / this.SectionParameter, 1.0 / 3.0), num1 * 12.0 / Math.Pow(this.SectionParameter, 3.0));
      return Math.Max(Math.Abs(force1) / sigma, this.SectionParameter * num2);
    }

    public double GetReqThickness(double reqArea) => reqArea / this.SectionParameter;

    public double GetReqMomInertia(double reqArea)
    {
      double x = reqArea / this.SectionParameter;
      return Math.Min(Math.Pow(x, 3.0) * this.SectionParameter, x * Math.Pow(this.SectionParameter, 3.0)) / 12.0;
    }

    public double GetReqTorMomInertia(double reqArea)
    {
      double x = reqArea / this.SectionParameter;
      double num = Math.Min(Math.Pow(x, 3.0) * this.SectionParameter, x * Math.Pow(this.SectionParameter, 3.0)) / 12.0;
      return 2.25 * Math.Pow(x / 2.0, 4.0);
    }

    public double GetReqMomInertiaY(double reqArea)
    {
      double x = reqArea / this.SectionParameter;
      return Math.Min(Math.Pow(x, 3.0) * this.SectionParameter, x * Math.Pow(this.SectionParameter, 3.0)) / 12.0;
    }

    public double GetReqMomInertiaZ(double reqArea)
    {
      double x = reqArea / this.SectionParameter;
      return Math.Min(Math.Pow(x, 3.0) * this.SectionParameter, x * Math.Pow(this.SectionParameter, 3.0)) / 12.0;
    }

    public double SectionParameter { get; set; }

    public string Name { get; set; }

    public ISection SectionClone()
    {
      return (ISection) new RectangularSection(this.SectionParameter, this.Name);
    }
  }
}
