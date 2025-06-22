// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.RectangularSection
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

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
      double num1 = 1.0 * (Math.Abs(f) * Math.Pow(L, 2.0)) / (Math.Pow(Math.PI, 2.0) * E);
      double num2 = Math.Min(Math.Pow(num1 * 12.0 / this.SectionParameter, 1.0 / 3.0), num1 * 12.0 / Math.Pow(this.SectionParameter, 3.0));
      return Math.Max(Math.Abs(f) / sigma, this.SectionParameter * num2);
    }

    public double GetReqThickness(double reqArea) => reqArea / this.SectionParameter;

    public double GetReqMomInertia(double reqArea)
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
