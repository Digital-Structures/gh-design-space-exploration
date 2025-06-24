// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.ISection
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B2AD7AF7-B053-4B89-AC37-223D1F22213F
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\Testing\bin\Debug\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Model
{
  public interface ISection
  {
    double GetReqEnvArea(
      Dictionary<LoadCase, List<double>> intforces,
      double sigma,
      double E,
      double L);

    double GetReqArea(List<double> intforces, double sigma, double E, double L);

    double GetReqThickness(double reqArea);

    double GetReqMomInertia(double reqArea);

    double SectionParameter { get; set; }

    string Name { get; set; }

    SectionType Type { get; }

    ISection SectionClone();
  }
}
