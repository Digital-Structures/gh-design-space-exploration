// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.Structure
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Model
{
  public class Structure : BaseDesign
  {
    private List<ComputedMember> _AffectedMembers;
    private List<ComputedMember> _UnaffectedMembers;
    private List<Material> _Materials;
    private List<ISection> _Sections;
    private double?[] _SymmetryLine;
    public double? BaseJitter;
    public double? BaseArea;

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
      this.OrderedEnvNodes = new List<Node>();
      this.LoadCases = new List<LoadCase>();
    }

    public override List<IVariable> DesignVariables
    {
      get
      {
        List<IVariable> designVariables = new List<IVariable>();
        foreach (IVariable doF in this.DOFs)
        {
          if (doF.Free)
            designVariables.Add(doF);
        }
        return designVariables;
      }
    }

    private List<DOF> AffectedDOFs
    {
      get
      {
        List<DOF> source = new List<DOF>();
        foreach (DOF doF in this.DOFs)
        {
          if (doF.Free)
          {
            source.Add(doF);
            foreach (ParametricRelation relation in doF.Relations)
            {
              foreach (Node listener in relation.Listeners)
              {
                if (doF.IsX)
                  source.Add(listener.DOFs[0]);
                else
                  source.Add(listener.DOFs[1]);
              }
            }
          }
        }
        return source.Distinct<DOF>().ToList<DOF>();
      }
    }

    protected List<ComputedMember> AffectedMembers
    {
      get
      {
        if (this._AffectedMembers == null)
          this.DistinguishMembers();
        return this._AffectedMembers;
      }
    }

    protected List<ComputedMember> UnaffectedMembers
    {
      get
      {
        if (this._UnaffectedMembers == null)
          this.DistinguishMembers();
        return this._UnaffectedMembers;
      }
    }

    private void DistinguishMembers()
    {
      this._AffectedMembers = new List<ComputedMember>();
      this._UnaffectedMembers = new List<ComputedMember>();
      foreach (ComputedMember member in this.Members)
      {
        if (this.AffectedDOFs.Contains(member.LocalDOFs[0]) || this.AffectedDOFs.Contains(member.LocalDOFs[1]) || this.AffectedDOFs.Contains(member.LocalDOFs[2]) || this.AffectedDOFs.Contains(member.LocalDOFs[3]))
          this._AffectedMembers.Add(member);
        else
          this._UnaffectedMembers.Add(member);
      }
    }

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

    public double?[] SymmetryLine
    {
      get
      {
        if (this._SymmetryLine == null)
          this._SymmetryLine = new double?[2];
        return this._SymmetryLine;
      }
      set => this._SymmetryLine = value;
    }

    public List<DOF> DOFs
    {
      get
      {
        List<DOF> source = new List<DOF>();
        for (int index = 0; index < this.Nodes.Count; ++index)
          source.AddRange((IEnumerable<DOF>) this.Nodes[index].DOFs);
        List<DOF> list = source.OrderBy<DOF, bool>((Func<DOF, bool>) (i => i.Pinned)).ToList<DOF>();
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

    public Structure.StructureType StructType { get; set; }

    public void ReorderIndices()
    {
      List<DOF> doFs = this.DOFs;
      for (int index = 0; index < this.Nodes.Count; ++index)
        this.Nodes[index].Index = index;
    }

    public Structure DesignClone() => this.CloneImpl();

    protected virtual Structure CloneImpl()
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
      IDictionary<int, Node> dictionary3 = (IDictionary<int, Node>) new Dictionary<int, Node>();
      List<Node> collection2 = new List<Node>();
      IDictionary<Node, Node> dictionary4 = (IDictionary<Node, Node>) new Dictionary<Node, Node>();
      foreach (Node node in this.Nodes)
      {
        Node newnode = new Node(new DOF[2]
        {
          new DOF(0.0),
          new DOF(0.0)
        });
        node.CopyTo(newnode);
        dictionary4.Add(node, newnode);
        collection1.Add(newnode);
        if (this.OrderedEnvNodes.Contains(node))
          dictionary3.Add(new KeyValuePair<int, Node>(this.OrderedEnvNodes.IndexOf(node), node));
      }
      for (int key = 0; key < dictionary3.Count; ++key)
        collection2.Add(dictionary3[key]);
      foreach (Node node in this.Nodes)
      {
        for (int index = 0; index < node.DOFs.Length; ++index)
        {
          foreach (ParametricRelation relation in node.DOFs[index].Relations)
          {
            List<Node> l = new List<Node>();
            foreach (Node listener in relation.Listeners)
              l.Add(dictionary4[listener]);
            object obj = (object) 0.0;
            object p = !(relation.Parameter is Node) ? relation.Parameter : (object) dictionary4[(Node) relation.Parameter];
            ParametricRelation parametricRelation = new ParametricRelation(l, relation.Relation, p);
            dictionary4[node].DOFs[index].Relations.Add(parametricRelation);
          }
        }
      }
      IDictionary<LoadCase, LoadCase> dictionary5 = (IDictionary<LoadCase, LoadCase>) new Dictionary<LoadCase, LoadCase>();
      List<LoadCase> collection3 = new List<LoadCase>();
      foreach (LoadCase loadCase in this.LoadCases)
      {
        LoadCase lc = new LoadCase(loadCase.Name);
        foreach (Load load1 in loadCase.Loads)
        {
          DOF origDOF = load1.myDOF;
          Node key = this.Nodes.Where<Node>((Func<Node, bool>) (n => ((IEnumerable<DOF>) n.DOFs).Contains<DOF>(origDOF))).ToList<Node>()[0];
          DOF doF = dictionary4[key].DOFs[((IEnumerable<DOF>) key.DOFs).ToList<DOF>().IndexOf(origDOF)];
          Load load2 = new Load(load1.Value, lc, doF);
          lc.Loads.Add(load2);
        }
        collection3.Add(lc);
        dictionary5.Add(loadCase, lc);
      }
      IDictionary<Member, Member> dictionary6 = (IDictionary<Member, Member>) new Dictionary<Member, Member>();
      List<Member> collection4 = new List<Member>();
      foreach (Member member in this.Members)
      {
        Member newMember = s.GetNewMember(dictionary4[member.NodeI], dictionary4[member.NodeJ]);
        member.CopyTo(newMember);
        newMember.Material = dictionary1[member.Material];
        newMember.SectionType = dictionary2[member.SectionType];
        collection4.Add(newMember);
        dictionary6.Add(member, newMember);
      }
      if (this is ComputedStructure)
      {
        foreach (ComputedMember member in this.Members)
        {
          ComputedMember computedMember = (ComputedMember) dictionary6[(Member) member];
          foreach (KeyValuePair<LoadCase, double> keyValuePair in member.AxialForce)
          {
            LoadCase key1 = keyValuePair.Key;
            LoadCase key2 = dictionary5[key1];
            computedMember.AxialForce.Add(key2, keyValuePair.Value);
          }
        }
      }
      s.LoadCases.AddRange((IEnumerable<LoadCase>) collection3);
      s.Nodes.AddRange((IEnumerable<Node>) collection1);
      s.OrderedEnvNodes.AddRange((IEnumerable<Node>) collection2);
      s.Members.AddRange((IEnumerable<Member>) collection4);
      s.SymmetryLine = this.SymmetryLine;
      s.PredictedScore = this.PredictedScore;
      s.StructType = this.StructType;
      s.BaseArea = this.BaseArea;
      s.BaseJitter = this.BaseJitter;
    }

    public bool[] CheckReady(IAnalysis AnalysisEngine)
    {
      bool[] flagArray = new bool[3]{ true, false, false };
      if (this.GetStable() == Structure.StabType.Unstable)
      {
        flagArray[0] = false;
      }
      else
      {
        try
        {
          if (new ComputedStructure(this).DetK <= 0.0)
            flagArray[0] = false;
        }
        catch
        {
          flagArray[0] = false;
        }
      }
      foreach (LoadCase loadCase in this.LoadCases)
      {
        foreach (Load load in loadCase.Loads)
        {
          if (load.Value != 0.0)
          {
            flagArray[1] = true;
            break;
          }
        }
        if (flagArray[1])
          break;
      }
      foreach (DOF doF in this.DOFs)
      {
        if (doF.Free)
        {
          flagArray[2] = true;
          double? allowableVariation = doF.AllowableVariation;
          double num = 0.0;
          if ((allowableVariation.GetValueOrDefault() == num ? (allowableVariation.HasValue ? 1 : 0) : 0) != 0)
            flagArray[2] = false;
        }
      }
      foreach (LoadCase loadCase in this.LoadCases)
      {
        foreach (Load load in loadCase.Loads)
        {
          if (load.Value != 0.0)
          {
            flagArray[1] = true;
            break;
          }
        }
      }
      return flagArray;
    }

    public Structure.StabType GetStable()
    {
      int num = this.DOFs.Count<DOF>((Func<DOF, bool>) (d => d.Pinned));
      if (this.Members.Count == 2 * this.Nodes.Count - num)
        return Structure.StabType.Determinate;
      return this.Members.Count > 2 * this.Nodes.Count - num ? Structure.StabType.Indeterminate : Structure.StabType.Unstable;
    }

    public bool IsSame(IDesign that) => Math.Abs(this.GetDistance(that)) < 1E-05;

    public void EnforceRelationships()
    {
      foreach (Node node in this.Nodes)
        node.EnforceRelationships(this);
    }

    public void DetermineEnvelope()
    {
      if (this.StructType != Structure.StructureType.Frame)
        return;
      Dictionary<Node, ICollection<Member>> dictionary = new Dictionary<Node, ICollection<Member>>();
      foreach (Node node in this.Nodes)
        dictionary[node] = (ICollection<Member>) new List<Member>();
      foreach (Member member in this.Members)
      {
        dictionary[member.NodeI].Add(member);
        dictionary[member.NodeJ].Add(member);
        member.Envelope = false;
      }
      List<List<Node>> nodeListList = new List<List<Node>>();
      HashSet<Node> nodeSet = new HashSet<Node>();
      foreach (Node node in this.Nodes)
      {
        if (!nodeSet.Contains(node))
        {
          List<Node> nodeList = new List<Node>();
          Queue<Node> nodeQueue = new Queue<Node>();
          nodeQueue.Enqueue(node);
          while (nodeQueue.Count > 0)
          {
            Node key = nodeQueue.Dequeue();
            if (!nodeSet.Contains(key))
            {
              nodeSet.Add(key);
              nodeList.Add(key);
              foreach (Member member in (IEnumerable<Member>) dictionary[key])
                nodeQueue.Enqueue(key == member.NodeI ? member.NodeJ : member.NodeI);
            }
          }
          nodeListList.Add(nodeList);
        }
      }
      using (List<List<Node>>.Enumerator enumerator = nodeListList.GetEnumerator())
      {
label_58:
        while (enumerator.MoveNext())
        {
          List<Node> current = enumerator.Current;
          Node node1 = (Node) null;
          foreach (Node node2 in current)
          {
            if (node1 == null || node1.Y < node2.Y || node1.Y == node2.Y && node1.X < node2.X)
              node1 = node2;
          }
          Node node3 = node1;
          List<Member> memberList = new List<Member>();
          HashSet<Member> memberSet = new HashSet<Member>();
          Member member1;
          while (true)
          {
            double num1 = 2.0 * Math.PI;
            double num2 = 0.0;
            if (memberList.Count > 0)
            {
              Member member2 = memberList[memberList.Count - 1];
              Node end = node3 == member2.NodeI ? member2.NodeJ : member2.NodeI;
              num2 = this.Angle(node3, end);
            }
            member1 = (Member) null;
            Node node4 = (Node) null;
            foreach (Member member3 in (IEnumerable<Member>) dictionary[node3])
            {
              if (!memberSet.Contains(member3))
              {
                double num3 = (2.0 * Math.PI + (node3 == member3.NodeI ? this.Angle(member3.NodeI, member3.NodeJ) : this.Angle(member3.NodeJ, member3.NodeI)) - num2) % (2.0 * Math.PI);
                if (num3 < num1)
                {
                  member1 = member3;
                  node4 = node3 == member3.NodeI ? member3.NodeJ : member3.NodeI;
                  num1 = num3;
                }
              }
            }
            if (node4 != node1)
            {
              if (node4 == null)
              {
                if (memberList.Count != 0)
                {
                  Member member4 = memberList[memberList.Count - 1];
                  Node node5 = node3 == member4.NodeI ? member4.NodeJ : member4.NodeI;
                  member4.Envelope = true;
                  node3 = node5;
                  memberList.RemoveAt(memberList.Count - 1);
                }
                else
                  goto label_58;
              }
              else
              {
                node3 = node4;
                memberList.Add(member1);
                memberSet.Add(member1);
              }
            }
            else
              break;
          }
          memberList.Add(member1);
          foreach (Member member5 in memberList)
            member5.Envelope = true;
        }
      }
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
