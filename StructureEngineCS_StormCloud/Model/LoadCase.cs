// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.LoadCase
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B2AD7AF7-B053-4B89-AC37-223D1F22213F
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\Testing\bin\Debug\StructureEngineCS.dll

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Model
{
  public class LoadCase
  {
    public string Name;
    public List<Load> Loads;

    public LoadCase(string n)
    {
      this.Name = n;
      this.Loads = new List<Load>();
    }

    public Vector<double> GetLoadVector(Structure s)
    {
      Vector<double> loadVector = (Vector<double>) new DenseVector(s.DOFs.Count);
      foreach (Load load in this.Loads)
      {
        int num = s.DOFs.IndexOf(load.myDOF);
        Vector<double> vector;
        int index;
        (vector = loadVector)[index = num] = vector[index] + load.Value;
      }
      return loadVector;
    }

    public Load GetLoad(DOF d)
    {
      List<Load> list = this.Loads.Where<Load>((Func<Load, bool>) (l => l.myDOF == d)).ToList<Load>();
      if (list.Count == 1)
        return list[0];
      if (list.Count != 0)
        throw new Exception("More than one load defined for one DOF.");
      Load load = new Load(0.0, this, d);
      this.Loads.Add(load);
      return load;
    }

    public LoadCase Clone()
    {
      LoadCase newLC = new LoadCase(this.Name);
      foreach (Load load in this.Loads)
        newLC.Loads.Add(load.Clone(newLC));
      return newLC;
    }
  }
}
