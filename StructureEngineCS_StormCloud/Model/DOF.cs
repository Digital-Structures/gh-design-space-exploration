// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.DOF
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B2AD7AF7-B053-4B89-AC37-223D1F22213F
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\Testing\bin\Debug\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Model
{
  public class DOF
  {
    public Dictionary<LoadCase, double> Disp;

    public DOF(double coord)
    {
      this.Coord = coord;
      this.Disp = new Dictionary<LoadCase, double>();
      this.Fixed = false;
    }

    public DOF(double coord, bool IsFrame)
    {
      this.Coord = coord;
      this.Disp = new Dictionary<LoadCase, double>();
      this.Fixed = IsFrame;
    }

    public DOF(bool IsFrame)
    {
      this.Disp = new Dictionary<LoadCase, double>();
      this.Fixed = IsFrame;
    }

    public double Coord { get; private set; }

    public void SetCoord(double c) => this.Coord = c;

    public bool Fixed { get; set; }

    public int Index { get; set; }

    public virtual void CopyTo(DOF newDOF)
    {
      newDOF.Coord = this.Coord;
      newDOF.Fixed = this.Fixed;
      newDOF.Index = this.Index;
    }
  }
}
