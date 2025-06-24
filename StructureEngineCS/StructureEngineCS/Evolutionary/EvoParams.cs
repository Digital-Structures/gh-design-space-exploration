// Decompiled with JetBrains decompiler
// Type: StructureEngine.Evolutionary.EvoParams
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

#nullable disable
namespace StructureEngine.Evolutionary
{
  public class EvoParams
  {
    public int GenSize;
    public double MutRate;

    public EvoParams(int genSize, double mutRate)
    {
      this.GenSize = genSize;
      this.MutRate = mutRate;
    }
  }
}
