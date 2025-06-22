// Decompiled with JetBrains decompiler
// Type: StructureEngine.Analysis.EquivFrameAnalysis
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StructureEngine.Analysis
{
  public class EquivFrameAnalysis : BaseTrussAnalysis, IAnalysis
  {
    public double Analyze(IDesign d)
    {
      ComputedStructure computedStructure = (ComputedStructure) d;
      try
      {
        if (!computedStructure.Analyzed)
        {
          this.RunAnalysis(computedStructure);
          computedStructure.Analyzed = true;
        }
        return this.GetCompositeScore(computedStructure);
      }
      catch
      {
        return double.NaN;
      }
    }

    private double GetCompositeScore(ComputedStructure s)
    {
      double polygonArea = this.GetPolygonArea(s);
      double maxStress = this.GetMaxStress(s);
      double jitter = this.GetJitter(s);
      if (s.BaseJitter.HasValue)
      {
        double num = jitter / s.BaseJitter.Value;
      }
      if (s.BaseArea.HasValue)
        polygonArea /= s.BaseArea.Value;
      return polygonArea + Math.Pow(10.0, 8.0) * Math.Pow(maxStress, 5.0);
    }

    private double GetPolygonArea(ComputedStructure s)
    {
      if (s.OrderedEnvNodes.Count == 0)
        s = this.OrderEnvNodes(s);
      int count = s.OrderedEnvNodes.Count;
      double num = 0.0;
      for (int index = 0; index < count - 1; ++index)
      {
        StructureEngine.Model.Node orderedEnvNode1 = s.OrderedEnvNodes[index];
        StructureEngine.Model.Node orderedEnvNode2 = s.OrderedEnvNodes[index + 1];
        double coord1 = orderedEnvNode1.DOFs[0].Coord;
        double coord2 = orderedEnvNode1.DOFs[1].Coord;
        double coord3 = orderedEnvNode2.DOFs[0].Coord;
        double coord4 = orderedEnvNode2.DOFs[1].Coord;
        num += coord1 * coord4 - coord3 * coord2;
      }
      return 0.5 * Math.Abs(num);
    }

    private double GetMaxStress(ComputedStructure s)
    {
      double val1 = double.MinValue;
      foreach (ComputedMember computedMember in s.Members.Where<Member>((Func<Member, bool>) (m => m.Envelope)))
        val1 = Math.Max(val1, Math.Abs(computedMember.MaxAxialForce / computedMember.Area) / computedMember.Material.StressAllow);
      return val1 > 1.0 ? val1 : 0.0;
    }

    private List<ComputedMember> GetMembers(ComputedStructure s, StructureEngine.Model.Node n)
    {
      List<ComputedMember> members = new List<ComputedMember>();
      foreach (ComputedMember member in s.Members)
      {
        if ((member.NodeI == n || member.NodeJ == n) && member.Envelope)
          members.Add(member);
      }
      return members;
    }

    public void SetMemberAreas(ComputedStructure s)
    {
      bool flag = false;
      s.BaseArea = new double?(this.GetPolygonArea(s));
      s.BaseJitter = new double?(this.GetJitter(s));
      while (!flag)
      {
        s.ClearStiffness();
        this.RunAnalysis(s);
        if (this.GetMaxStress(s) == 0.0)
          break;
        double num = double.MaxValue;
        foreach (ComputedMember computedMember in s.Members.Where<Member>((Func<Member, bool>) (m => m.Envelope)))
        {
          if (Math.Abs(computedMember.Material.StressAllow / (computedMember.MaxAxialForce / computedMember.Area)) < num)
            num = Math.Abs(computedMember.Material.StressAllow / (computedMember.MaxAxialForce / computedMember.Area));
        }
        foreach (ComputedMember computedMember in s.Members.Where<Member>((Func<Member, bool>) (m => m.Envelope)))
          computedMember.Area = computedMember.Area / num * 1.1;
      }
    }

    public ComputedStructure OrderEnvNodes(ComputedStructure s)
    {
      ComputedStructure s1 = (ComputedStructure) s.DesignClone();
      List<StructureEngine.Model.Node> source = new List<StructureEngine.Model.Node>();
      foreach (ComputedMember member in s1.Members)
      {
        if (member.Envelope)
        {
          source.Add(member.NodeI);
          source.Add(member.NodeJ);
        }
      }
      List<StructureEngine.Model.Node> list = source.Select<StructureEngine.Model.Node, StructureEngine.Model.Node>((Func<StructureEngine.Model.Node, StructureEngine.Model.Node>) (n => n)).Distinct<StructureEngine.Model.Node>().ToList<StructureEngine.Model.Node>();
      List<StructureEngine.Model.Node> collection = new List<StructureEngine.Model.Node>();
      StructureEngine.Model.Node n1 = list[0];
      bool flag = true;
      while (list.Count > 0)
      {
        if (!flag)
          throw new Exception("Error with node ordering.");
        collection.Add(n1);
        list.Remove(n1);
        List<ComputedMember> members = this.GetMembers(s1, n1);
        int index = 0;
        flag = false;
        for (; index < members.Count; ++index)
        {
          ComputedMember computedMember = members[index];
          if (computedMember.NodeI == n1 && list.Contains(computedMember.NodeJ))
          {
            n1 = computedMember.NodeJ;
            flag = true;
            break;
          }
          if (computedMember.NodeJ == n1 && list.Contains(computedMember.NodeI))
          {
            n1 = computedMember.NodeI;
            flag = true;
            break;
          }
        }
      }
      s1.OrderedEnvNodes.Clear();
      s1.OrderedEnvNodes.AddRange((IEnumerable<StructureEngine.Model.Node>) collection);
      return s1;
    }

    private double GetJitter(ComputedStructure s)
    {
      if (s.OrderedEnvNodes.Count == 0)
        s = this.OrderEnvNodes(s);
      List<StructureEngine.Model.Node> nodeList = new List<StructureEngine.Model.Node>();
      foreach (StructureEngine.Model.Node orderedEnvNode in s.OrderedEnvNodes)
      {
        if (orderedEnvNode.DOFs[0].Free || orderedEnvNode.DOFs[1].Free)
          nodeList.Add(orderedEnvNode);
      }
      double x1 = 0.0;
      foreach (StructureEngine.Model.Node n in nodeList)
      {
        List<ComputedMember> list = this.GetMembers(s, n).Where<ComputedMember>((Func<ComputedMember, bool>) (m => m.Envelope)).ToList<ComputedMember>();
        if (list.Count != 2)
          throw new Exception("Node must be connected to two envelope members.");
        if (nodeList.Contains(list[0].NodeI) && nodeList.Contains(list[0].NodeJ) && nodeList.Contains(list[1].NodeI) && nodeList.Contains(list[1].NodeJ))
        {
          double x2 = (Math.PI - this.GetAngleBetween((Member) list[0], (Member) list[1])) / Math.PI;
          x1 += Math.Pow(x2, 1.0);
        }
      }
      return Math.Pow(x1, 2.0);
    }

    private double GetAngleBetween(Member m1, Member m2)
    {
      double num1 = m1.NodeJ.DOFs[0].Coord - m1.NodeI.DOFs[0].Coord;
      double num2 = m1.NodeJ.DOFs[1].Coord - m1.NodeI.DOFs[1].Coord;
      double num3 = m2.NodeJ.DOFs[0].Coord - m2.NodeI.DOFs[0].Coord;
      double num4 = m2.NodeJ.DOFs[1].Coord - m2.NodeI.DOFs[1].Coord;
      double angleBetween = Math.Atan2(Math.Abs(num1 * num4 - num3 * num2), num1 * num3 + num2 * num4);
      if (m1.NodeI == m2.NodeI || m1.NodeJ == m2.NodeJ)
        return angleBetween;
      if (m1.NodeI == m2.NodeJ || m1.NodeJ == m2.NodeI)
        return Math.PI - angleBetween;
      throw new Exception("Members must share a node.");
    }
  }
}
