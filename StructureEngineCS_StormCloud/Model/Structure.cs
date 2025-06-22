// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.Structure
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B2AD7AF7-B053-4B89-AC37-223D1F22213F
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\Testing\bin\Debug\StructureEngineCS.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Model
{
  public class Structure
  {
    private List<Material> _Materials;
    private List<ISection> _Sections;

    public Structure() => this.InitializeLists((List<Node>) null, (List<Member>) null);

    public Structure(List<Node> nodes) => this.InitializeLists(nodes, (List<Member>) null);

    public Structure(List<Node> nodes, List<Member> members)
    {
      this.InitializeLists(nodes, members);
    }

    private void InitializeLists(List<Node> nodes, List<Member> members)
    {
      this.Nodes = nodes != null ? nodes : new List<Node>();
      this.Members = members != null ? members : new List<Member>();
      this._Materials = new List<Material>();
      this._Sections = new List<ISection>();
      this.LoadCases = new List<LoadCase>();
    }

    public StructureDim Dimension { get; set; }

    public List<Node> Nodes { get; set; }

    public List<Node> OrderedEnvNodes { get; set; }

    public List<Member> Members { get; set; }

    public virtual Member GetNewMember(Node i, Node j) => new Member(i, j);

    public List<LoadCase> LoadCases { get; set; }

    public List<Material> Materials
    {
      get
      {
        foreach (Member member in this.Members)
        {
          if (!this._Materials.Contains(member.Material))
            this._Materials.Add(member.Material);
        }
        return this._Materials;
      }
      set => this._Materials = value;
    }

    public List<ISection> Sections
    {
      get
      {
        ISection section = (ISection) new RoundTubeSection();
        foreach (Member member in this.Members)
        {
          if (member.SectionType == null)
            member.SectionType = section;
          if (!this._Sections.Contains(member.SectionType))
            this._Sections.Add(member.SectionType);
        }
        return this._Sections;
      }
      set => this._Sections = value;
    }

    public List<DOF> DOFs
    {
      get
      {
        List<DOF> source = new List<DOF>();
        for (int index = 0; index < this.Nodes.Count; ++index)
          source.AddRange((IEnumerable<DOF>) this.Nodes[index].DOFs);
        List<DOF> list = source.OrderBy<DOF, bool>((Func<DOF, bool>) (i => i.Fixed)).ToList<DOF>();
        for (int index = 0; index < list.Count; ++index)
          list[index].Index = index;
        return list;
      }
    }

    public double[] ZeroPoint
    {
      get
      {
        double val1_1 = double.MaxValue;
        double val1_2 = double.MaxValue;
        foreach (Node node in this.Nodes)
        {
          val1_1 = Math.Min(val1_1, node.DOFs[0].Coord);
          val1_2 = Math.Min(val1_2, node.DOFs[1].Coord);
        }
        if (this.Nodes.Count == 0)
        {
          val1_1 = 0.0;
          val1_2 = 0.0;
        }
        return new double[2]{ val1_1, val1_2 };
      }
    }

    public double[] Dimensions
    {
      get
      {
        if (this.Nodes.Count <= 0)
          return new double[2];
        double val1_1 = double.MinValue;
        double val1_2 = double.MinValue;
        foreach (Node node in this.Nodes)
        {
          val1_1 = Math.Max(val1_1, node.DOFs[0].Coord);
          val1_2 = Math.Max(val1_2, node.DOFs[1].Coord);
        }
        double[] zeroPoint = this.ZeroPoint;
        double[] dimensions = new double[this.Nodes[0].DOFs.Length];
        dimensions[0] = val1_1 - zeroPoint[0];
        dimensions[1] = val1_2 - zeroPoint[1];
        return dimensions;
      }
    }

    private int dimension => this.Nodes[0].DOFs.Length;

    public virtual Structure CloneImpl()
    {
      Structure s = new Structure();
      this.CopyTo(s);
      return s;
    }

    internal virtual void CopyTo(Structure s)
    {
      List<Material> materialList = new List<Material>();
      IDictionary<Material, Material> dictionary1 = (IDictionary<Material, Material>) new Dictionary<Material, Material>();
      foreach (Material material1 in this.Materials)
      {
        Material material2 = material1.MaterialClone();
        dictionary1.Add(material1, material2);
        materialList.Add(material2);
      }
      List<ISection> sectionList = new List<ISection>();
      IDictionary<ISection, ISection> dictionary2 = (IDictionary<ISection, ISection>) new Dictionary<ISection, ISection>();
      foreach (ISection section1 in this.Sections)
      {
        ISection section2 = section1.SectionClone();
        dictionary2.Add(section1, section2);
        sectionList.Add(section2);
      }
      List<Node> collection1 = new List<Node>();
      IDictionary<Node, Node> dictionary3 = (IDictionary<Node, Node>) new Dictionary<Node, Node>();
      foreach (Node node in this.Nodes)
      {
        Node newnode = new Node(new DOF[6]
        {
          new DOF(0.0),
          new DOF(0.0),
          new DOF(0.0),
          new DOF(0.0),
          new DOF(0.0),
          new DOF(0.0)
        });
        node.CopyTo(newnode);
        dictionary3.Add(node, newnode);
        collection1.Add(newnode);
      }
      IDictionary<LoadCase, LoadCase> dictionary4 = (IDictionary<LoadCase, LoadCase>) new Dictionary<LoadCase, LoadCase>();
      List<LoadCase> collection2 = new List<LoadCase>();
      foreach (LoadCase loadCase in this.LoadCases)
      {
        LoadCase lc = new LoadCase(loadCase.Name);
        foreach (Load load1 in loadCase.Loads)
        {
          DOF origDOF = load1.myDOF;
          Node key = this.Nodes.Where<Node>((Func<Node, bool>) (n => ((IEnumerable<DOF>) n.DOFs).Contains<DOF>(origDOF))).ToList<Node>()[0];
          DOF doF = dictionary3[key].DOFs[((IEnumerable<DOF>) key.DOFs).ToList<DOF>().IndexOf(origDOF)];
          Load load2 = new Load(load1.Value, lc, doF);
          lc.Loads.Add(load2);
        }
        collection2.Add(lc);
        dictionary4.Add(loadCase, lc);
      }
      IDictionary<Member, Member> dictionary5 = (IDictionary<Member, Member>) new Dictionary<Member, Member>();
      List<Member> collection3 = new List<Member>();
      foreach (Member member in this.Members)
      {
        Member newMember = s.GetNewMember(dictionary3[member.NodeI], dictionary3[member.NodeJ]);
        member.CopyTo(newMember);
        newMember.Material = dictionary1[member.Material];
        newMember.SectionType = dictionary2[member.SectionType];
        collection3.Add(newMember);
        dictionary5.Add(member, newMember);
      }
      if (this is ComputedStructure)
      {
        foreach (ComputedMember member in this.Members)
        {
          ComputedMember computedMember = (ComputedMember) dictionary5[(Member) member];
          foreach (KeyValuePair<LoadCase, double> keyValuePair in member.AxialForce)
          {
            LoadCase key1 = keyValuePair.Key;
            LoadCase key2 = dictionary4[key1];
            computedMember.AxialForce.Add(key2, keyValuePair.Value);
          }
        }
      }
      s.LoadCases.AddRange((IEnumerable<LoadCase>) collection2);
      s.Nodes.AddRange((IEnumerable<Node>) collection1);
      s.Members.AddRange((IEnumerable<Member>) collection3);
      s.StructType = this.StructType;
    }

    public Structure.StructureType StructType { get; set; }

    public void ReorderIndices()
    {
      List<DOF> doFs = this.DOFs;
      for (int index = 0; index < this.Nodes.Count; ++index)
        this.Nodes[index].Index = index;
    }

    public Structure.StabType GetStable()
    {
      int num = this.DOFs.Count<DOF>((Func<DOF, bool>) (d => d.Fixed));
      if (this.Members.Count == 2 * this.Nodes.Count - num)
        return Structure.StabType.Determinate;
      return this.Members.Count > 2 * this.Nodes.Count - num ? Structure.StabType.Indeterminate : Structure.StabType.Unstable;
    }

    private double Angle(Node start, Node end)
    {
      double num = Math.Atan2(end.Y - start.Y, end.X - start.X);
      if (num < 0.0)
        num += 2.0 * Math.PI;
      return num;
    }

    public enum StructureType
    {
      Truss,
      Frame,
    }

    public enum StabType
    {
      Indeterminate,
      Determinate,
      Unstable,
    }
  }
}
