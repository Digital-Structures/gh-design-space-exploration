// Decompiled with JetBrains decompiler
// Type: StructureEngine.Logging.NodeSerializer
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System.Text;

#nullable disable
namespace StructureEngine.Logging
{
  public class NodeSerializer
  {
    public string Serialize(Node n)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(nameof (n) + (object) n.Index + "\t");
      stringBuilder.Append(n.DOFs[0].Coord.ToString() + "\t");
      stringBuilder.Append(n.DOFs[1].Coord.ToString() + "\t");
      stringBuilder.Append((n.DOFs[0].Free ? 1 : 0).ToString() + "\t");
      stringBuilder.Append((n.DOFs[1].Free ? 1 : 0).ToString() + "\t");
      stringBuilder.Append(n.DOFs[0].Min.ToString() + "\t");
      stringBuilder.Append(n.DOFs[1].Min.ToString() + "\t");
      stringBuilder.Append(n.DOFs[0].Max.ToString() + "\t");
      stringBuilder.Append(n.DOFs[1].Max.ToString() + "\t");
      stringBuilder.Append((n.DOFs[0].Pinned ? 1 : 0).ToString() + "\t");
      stringBuilder.Append((n.DOFs[1].Pinned ? 1 : 0).ToString() + "\t");
      return stringBuilder.ToString();
    }
  }
}
