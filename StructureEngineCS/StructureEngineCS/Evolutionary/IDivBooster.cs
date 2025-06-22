// Decompiled with JetBrains decompiler
// Type: StructureEngine.Evolutionary.IDivBooster
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Evolutionary
{
  public interface IDivBooster
  {
    bool IsDiverse(List<IDesign> existing, IDesign candidate, double rate);
  }
}
