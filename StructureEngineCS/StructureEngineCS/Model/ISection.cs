// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.ISection
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Model
{
  public interface ISection
  {
    double GetReqEnvArea(Dictionary<LoadCase, double> force, double sigma, double E, double L);

    double GetReqArea(double f, double sigma, double E, double L);

    double GetReqThickness(double reqArea);

    double GetReqMomInertia(double reqArea);

    double SectionParameter { get; set; }

    string Name { get; set; }

    SectionType Type { get; }

    ISection SectionClone();
  }
}
