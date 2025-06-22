// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.RodSection
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Model
{
  public class RodSection : ISection
  {
    public RodSection(double p, string n)
    {
      this.SectionParameter = p;
      this.Name = n;
    }

    public RodSection() => this.Name = "Rod";

    public SectionType Type => SectionType.Rod;

    public double GetReqEnvArea(
      Dictionary<LoadCase, double> force,
      double sigma,
      double E,
      double L)
    {
      List<double> source = new List<double>();
      foreach (double f in force.Values)
        source.Add(this.GetReqArea(f, sigma, E, L));
      return source.Max();
    }

    public double GetReqArea(double f, double sigma, double E, double L)
    {
      if (f > 0.0)
        return Math.Abs(f) / sigma;
      double x = Math.Pow(4.0 / Math.PI * (3.0 * (Math.Abs(f) * Math.Pow(L, 2.0)) / (Math.Pow(Math.PI, 2.0) * E)), 0.25);
      return Math.Max(Math.Abs(f) / sigma, Math.PI * Math.Pow(x, 2.0));
    }

    public double GetReqThickness(double reqArea) => 2.0 * Math.Sqrt(reqArea / Math.PI);

    public double GetReqMomInertia(double reqArea)
    {
      return Math.PI * Math.Pow(Math.Sqrt(reqArea / Math.PI), 4.0) / 4.0;
    }

    public double SectionParameter { get; set; }

    public string Name { get; set; }

    public ISection SectionClone() => (ISection) new RodSection(this.SectionParameter, this.Name);
  }
}
