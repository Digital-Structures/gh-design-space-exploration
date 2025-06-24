// Decompiled with JetBrains decompiler
// Type: StructureEngine.Logging.LoadCaseSerializer
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System.Text;

#nullable disable
namespace StructureEngine.Logging
{
  public class LoadCaseSerializer
  {
    public string Serialize(LoadCase lc, Structure s)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(lc.Name);
      stringBuilder.AppendLine("Node\tValue");
      foreach (Load load in lc.Loads)
        stringBuilder.AppendLine(this.SerializeLoad(load, s));
      return stringBuilder.ToString();
    }

    private string SerializeLoad(Load l, Structure s)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(s.DOFs.IndexOf(l.myDOF));
      stringBuilder.Append(l.Value);
      return stringBuilder.ToString();
    }
  }
}
