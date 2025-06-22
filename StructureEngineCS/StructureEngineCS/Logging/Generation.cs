// Decompiled with JetBrains decompiler
// Type: StructureEngine.Logging.Generation
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace StructureEngine.Logging
{
  public class Generation
  {
    private int GenNum;
    private IList<ComputedStructure> Top;
    private IList<ComputedStructure> Selected;
    private double GenSize;
    private double MutRate;
    private bool AutoMode;
    private int? AutoCount;

    public Generation(IList<ComputedStructure> top, double g, double m, bool a, int? c, int num)
    {
      this.Top = top;
      this.GenSize = g;
      this.MutRate = m;
      this.AutoMode = a;
      this.AutoCount = c;
      this.GenNum = num;
      this.Selected = (IList<ComputedStructure>) new List<ComputedStructure>();
    }

    public void UpdateSelected(IList<ComputedStructure> selected) => this.Selected = selected;

    public string Serialize(double RefScore)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.GenNum.ToString() + "\t");
      stringBuilder.Append(this.MutRate.ToString() + "\t");
      stringBuilder.Append(this.GenSize.ToString() + "\t");
      stringBuilder.Append((this.AutoMode ? 1 : 0).ToString() + "\t");
      stringBuilder.Append((this.AutoMode ? this.AutoCount : new int?(0)).ToString() + "\t");
      stringBuilder.Append(this.Selected.Count.ToString() + "\t");
      stringBuilder.Append(this.AvgScore(RefScore).ToString() + "\t");
      stringBuilder.Append(this.AvgRank().ToString() + "\t");
      foreach (ComputedStructure s in (IEnumerable<ComputedStructure>) this.Top)
        stringBuilder.Append(this.GetSelectedData(s, RefScore));
      return stringBuilder.ToString();
    }

    public string FullSerialize(double RefScore)
    {
      StringBuilder stringBuilder = new StringBuilder();
      StructureSerializer structureSerializer = new StructureSerializer();
      foreach (ComputedStructure s in (IEnumerable<ComputedStructure>) this.Top)
      {
        stringBuilder.Append(this.GenNum.ToString() + "\t");
        stringBuilder.Append(this.Top.IndexOf(s).ToString() + "\t");
        if (this.Selected.Contains(s))
          stringBuilder.Append(1.ToString() + "\t");
        else
          stringBuilder.Append(0.ToString() + "\t");
        stringBuilder.Append(structureSerializer.QuickSerialize(s, RefScore));
        stringBuilder.AppendLine();
      }
      return stringBuilder.ToString();
    }

    private double AvgRank()
    {
      double num = 0.0;
      for (int index = 0; index < this.Top.Count; ++index)
      {
        foreach (Structure structure in (IEnumerable<ComputedStructure>) this.Selected)
        {
          if (structure.IsSame((IDesign) this.Top[index]))
          {
            num += (double) index;
            break;
          }
        }
      }
      return num / (double) this.Selected.Count;
    }

    private double AvgScore(double RefScore)
    {
      double num = 0.0;
      foreach (ComputedStructure computedStructure in (IEnumerable<ComputedStructure>) this.Selected)
        num += computedStructure.Score / RefScore;
      return num / (double) this.Selected.Count;
    }

    private string GetSelectedData(ComputedStructure s, double RefScore)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num = 0;
      foreach (Structure structure in (IEnumerable<ComputedStructure>) this.Selected)
      {
        if (structure.IsSame((IDesign) s))
        {
          num = 1;
          break;
        }
      }
      stringBuilder.Append(num.ToString() + "\t");
      stringBuilder.Append((s.Score / RefScore).ToString() + "\t");
      return stringBuilder.ToString();
    }
  }
}
