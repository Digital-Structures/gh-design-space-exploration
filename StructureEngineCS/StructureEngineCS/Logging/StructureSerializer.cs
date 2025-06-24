// Decompiled with JetBrains decompiler
// Type: StructureEngine.Logging.StructureSerializer
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System.Text;

#nullable disable
namespace StructureEngine.Logging
{
  public class StructureSerializer
  {
    public string Serialize(ComputedStructure s, double RefScore)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("// Score //");
      stringBuilder.AppendLine(s.Score.ToString());
      stringBuilder.AppendLine("");
      stringBuilder.AppendLine("// Nodes //");
      stringBuilder.AppendLine("Name\tXCoord\tYCoord\tXVar\tYVar\tXMin\tYMin\tXMax\tYMax\tXPin\tYPin\tXLoad\tYLoad");
      NodeSerializer nodeSerializer = new NodeSerializer();
      foreach (Node node in s.Nodes)
        stringBuilder.AppendLine(nodeSerializer.Serialize(node));
      stringBuilder.AppendLine("");
      stringBuilder.AppendLine("// Load Cases //");
      stringBuilder.AppendLine("Name");
      LoadCaseSerializer loadCaseSerializer = new LoadCaseSerializer();
      foreach (LoadCase loadCase in s.LoadCases)
        stringBuilder.AppendLine(loadCaseSerializer.Serialize(loadCase, (Structure) s));
      stringBuilder.AppendLine("");
      stringBuilder.AppendLine("// Members //");
      stringBuilder.AppendLine("NodeI\tNodeJ");
      MemberSerializer memberSerializer = new MemberSerializer();
      foreach (Member member in s.Members)
        stringBuilder.AppendLine(memberSerializer.Serialize(member));
      stringBuilder.AppendLine("");
      stringBuilder.AppendLine("// Variable Map //");
      stringBuilder.AppendLine(this.SerializeVars(s));
      stringBuilder.AppendLine("// Vector Format //");
      stringBuilder.AppendLine(this.QuickSerializeHeader(s));
      stringBuilder.AppendLine(this.QuickSerialize(s, RefScore));
      return stringBuilder.ToString();
    }

    public string QuickSerialize(ComputedStructure s, double RefScore)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append((s.Score / RefScore).ToString() + "\t");
      foreach (IVariable designVariable in s.DesignVariables)
        stringBuilder.Append(designVariable.GetPoint().ToString() + "\t");
      return stringBuilder.ToString();
    }

    public string QuickSerializeHeader(ComputedStructure s)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Score\t");
      int num = 0;
      foreach (IVariable designVariable in s.DesignVariables)
      {
        stringBuilder.Append("v" + (object) num + "\t");
        ++num;
      }
      return stringBuilder.ToString();
    }

    private string SerializeVars(ComputedStructure s)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("Name\tNode\tDOF");
      int num = 0;
      foreach (Node node in s.Nodes)
      {
        for (int index = 0; index < node.DOFs.Length; ++index)
        {
          if (node.DOFs[index].Free)
          {
            string str = index == 0 ? "X" : "Y";
            stringBuilder.AppendLine("v" + (object) num + "\tn" + (object) node.Index + "\t" + str);
            ++num;
          }
        }
      }
      return stringBuilder.ToString();
    }
  }
}
