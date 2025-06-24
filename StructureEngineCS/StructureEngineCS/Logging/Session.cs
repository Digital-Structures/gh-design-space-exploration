// Decompiled with JetBrains decompiler
// Type: StructureEngine.Logging.Session
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace StructureEngine.Logging
{
  public class Session
  {
    public Session(string id, int num)
    {
      this.SessionID = id;
      this.ProbNum = num;
      this.Generations = new List<Generation>();
    }

    public bool ControlGroup { get; set; }

    public ComputedStructure FinalDesign { get; set; }

    public ComputedStructure RefineDesign { get; set; }

    public ComputedStructure InitialDesign { get; set; }

    public ComputedStructure Final1 { get; set; }

    public ComputedStructure Final2 { get; set; }

    public ComputedStructure Final3 { get; set; }

    public DateTime TimeStamp { get; set; }

    public string SessionID { get; set; }

    public string Group { get; set; }

    public int ProbNum { get; set; }

    public List<Generation> Generations { get; set; }

    public string EvoSerialize()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("//////////////////////");
      stringBuilder.AppendLine(" STRUCTUREFIT OUTPUT");
      stringBuilder.AppendLine("//////////////////////");
      stringBuilder.AppendLine("");
      stringBuilder.AppendLine("");
      stringBuilder.AppendLine("[Initial_Structure]");
      stringBuilder.AppendLine(this.GetStructureData(this.InitialDesign, false, false));
      stringBuilder.AppendLine("[Exploration_Data]");
      stringBuilder.AppendLine(this.GetExpData(this.Generations));
      stringBuilder.AppendLine("[Appendix_Data]");
      stringBuilder.AppendLine(this.GetAppData(this.Generations));
      return stringBuilder.ToString();
    }

    public string Serialize()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("//////////////////////");
      stringBuilder.AppendLine(" STRUCTUREFIT OUTPUT");
      stringBuilder.AppendLine("//////////////////////");
      stringBuilder.AppendLine("");
      stringBuilder.AppendLine("");
      stringBuilder.AppendLine("[Personal_Data]");
      stringBuilder.AppendLine(this.GetPersonalData());
      stringBuilder.AppendLine("[Initial_Structure]");
      stringBuilder.AppendLine(this.GetStructureData(this.InitialDesign, false, false));
      if (this.Group != "B" || this.ProbNum != 2)
      {
        stringBuilder.AppendLine("[Exploration_Data]");
        stringBuilder.AppendLine(this.GetExpData(this.Generations));
        stringBuilder.AppendLine("[SelectedStructure_Data]");
        stringBuilder.AppendLine(this.GetStructureData(this.RefineDesign, true, true));
        stringBuilder.AppendLine(this.GetDistData(this.InitialDesign, this.RefineDesign, "Initial"));
        stringBuilder.AppendLine("");
      }
      stringBuilder.AppendLine("[FinalStructure_Data]");
      stringBuilder.AppendLine(this.GetStructureData(this.FinalDesign, true, true));
      stringBuilder.AppendLine(this.GetDistData(this.InitialDesign, this.FinalDesign, "Initial"));
      if (this.Group != "B" || this.ProbNum != 2)
      {
        stringBuilder.AppendLine(this.GetDistData(this.RefineDesign, this.FinalDesign, "Selected"));
        stringBuilder.AppendLine("");
        stringBuilder.AppendLine("[Appendix_Data]");
        stringBuilder.AppendLine(this.GetAppData(this.Generations));
      }
      return stringBuilder.ToString();
    }

    private string GetPersonalData()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("TimeStamp\t" + this.TimeStamp.ToString());
      return stringBuilder.ToString();
    }

    private string GetStructureData(ComputedStructure s, bool quick, bool quickHeader)
    {
      StructureSerializer structureSerializer = new StructureSerializer();
      StringBuilder stringBuilder = new StringBuilder();
      if (quickHeader)
        stringBuilder.AppendLine(structureSerializer.QuickSerializeHeader(s));
      stringBuilder.AppendLine(quick ? structureSerializer.QuickSerialize(s, this.InitialDesign.Score) : structureSerializer.Serialize(s, this.InitialDesign.Score));
      return stringBuilder.ToString();
    }

    private string GetExpHeader()
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      for (int index = 0; index < 10; ++index)
      {
        stringBuilder2.Append("Selected" + (object) index + "\t");
        stringBuilder2.Append("Score" + (object) index + "\t");
      }
      stringBuilder1.Append("GenNum\tMutRate\tGenSize\tAutoMode\tAutoCount\tNumSelected\tAvgScore\tAvgRank\t" + stringBuilder2.ToString());
      return stringBuilder1.ToString();
    }

    private string GetExpData(List<Generation> gens)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(this.GetExpHeader());
      foreach (Generation gen in gens)
        stringBuilder.AppendLine(gen.Serialize(this.InitialDesign.Score));
      return stringBuilder.ToString();
    }

    private string GetDistData(ComputedStructure s1, ComputedStructure s2, string start)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("DistanceFrom" + start + "\t");
      stringBuilder.Append(s1.GetDistance((IDesign) s2));
      return stringBuilder.ToString();
    }

    private string GetAppData(List<Generation> gens)
    {
      StringBuilder stringBuilder = new StringBuilder();
      StructureSerializer structureSerializer = new StructureSerializer();
      stringBuilder.AppendLine("GenNum\tRank\tSelected\t" + structureSerializer.QuickSerializeHeader(this.InitialDesign));
      foreach (Generation gen in gens)
        stringBuilder.Append(gen.FullSerialize(this.InitialDesign.Score));
      return stringBuilder.ToString();
    }

    public void SetStartStructure(ComputedStructure s)
    {
      if (s == null)
        return;
      this.ClearSession();
      this.InitialDesign = s;
    }

    public void SetRefineStructure(ComputedStructure s)
    {
      if (s == null)
        return;
      this.RefineDesign = s;
    }

    public void SetFinalStructure(ComputedStructure s)
    {
      if (s == null)
        return;
      this.FinalDesign = s;
      this.TimeStamp = DateTime.Now;
      this.SubmitDesign();
      if (this.ProbNum == 1)
        this.Final1 = s;
      else if (this.ProbNum == 2)
      {
        this.Final2 = s;
      }
      else
      {
        if (this.ProbNum != 3)
          return;
        this.Final3 = s;
      }
    }

    private void ClearSession()
    {
      this.InitialDesign = (ComputedStructure) null;
      this.RefineDesign = (ComputedStructure) null;
      this.FinalDesign = (ComputedStructure) null;
      this.Generations.Clear();
    }

    private void SubmitDesign()
    {
      string contents = this.Serialize();
      HttpWebRequestBuilder webRequestBuilder = new HttpWebRequestBuilder("http://www.caitlinmueller.com/post/server.php");
      webRequestBuilder.AddStringFile("logfile", string.Format("{0}_{1}.txt", (object) this.SessionID, (object) this.ProbNum), contents);
      webRequestBuilder.SendRequest();
    }

    public string SaveSession(ComputedStructure s)
    {
      this.FinalDesign = s != null ? s : throw new Exception("Final design is not ComputedStructure.");
      return this.Serialize();
    }
  }
}
