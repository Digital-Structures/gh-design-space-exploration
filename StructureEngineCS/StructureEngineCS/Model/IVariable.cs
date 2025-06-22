// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.IVariable
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using MathNet.Numerics.Distributions;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Model
{
  public interface IVariable
  {
    double Value { get; set; }

    void Mutate(double globalrate, IContinuousDistribution dist);

    void Crossover(List<IVariable> mylist);

    bool Free { get; }

    bool CheckConstraint();

    void FixConstraint();

    void SetConstraint();

    double Min { get; }

    double Max { get; }

    int GetBytes();

    void Project(double d);

    double GetPoint();
  }
}
