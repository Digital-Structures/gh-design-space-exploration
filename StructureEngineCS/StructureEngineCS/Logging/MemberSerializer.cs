// Decompiled with JetBrains decompiler
// Type: StructureEngine.Logging.MemberSerializer
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System.Text;

#nullable disable
namespace StructureEngine.Logging
{
  public class MemberSerializer
  {
    public string Serialize(Member m)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("n" + (object) m.NodeI.Index + "\t");
      stringBuilder.Append("n" + (object) m.NodeJ.Index + "\t");
      return stringBuilder.ToString();
    }
  }
}
