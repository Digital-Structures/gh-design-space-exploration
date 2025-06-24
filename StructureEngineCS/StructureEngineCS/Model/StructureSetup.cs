// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.StructureSetup
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Analysis;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Model
{
  public class StructureSetup : ISetup
  {
    private TrussAnalysis MyTrussAnalysis;
    public EquivFrameAnalysis MyFrameAnalysis;

    public StructureSetup()
    {
      List<IDesign> designList = new List<IDesign>();
      designList.Add((IDesign) this.simpletruss);
      designList.Add((IDesign) this.cantroof);
      designList.Add((IDesign) this.chiassoroof);
      designList.Add((IDesign) this.portalframe);
      designList.Add((IDesign) this.circulararch);
      designList.Add((IDesign) this.tower);
      designList.Add((IDesign) this.bridge);
      designList.Add((IDesign) this.Test1);
      designList.Add((IDesign) this.Test2);
      designList.Add((IDesign) this.airportroof);
      foreach (IDesign design1 in designList)
      {
        IDesign design2;
        ((Structure) (design2 = design1)).DetermineEnvelope();
        ((ComputedStructure) design2).SetStart();
      }
      this.Designs = designList;
      this.MyTrussAnalysis = new TrussAnalysis();
      this.MyFrameAnalysis = new EquivFrameAnalysis();
    }

    public StructureSetup(int probNum)
    {
      List<IDesign> designList = new List<IDesign>();
      switch (probNum)
      {
        case 1:
          designList.Add((IDesign) this.Test1);
          break;
        case 2:
          designList.Add((IDesign) this.Test2);
          break;
        case 3:
          designList.Add((IDesign) this.Test3);
          break;
      }
      this.Designs = designList;
    }

    public List<IDesign> Designs { get; set; }

    private ComputedStructure Test1
    {
      get
      {
        List<Node> nodes = new List<Node>();
        List<Member> members = new List<Member>();
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(0.0),
          new DOF(0.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(30.0),
          new DOF(-30.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(90.0),
          new DOF(-30.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(60.0),
          new DOF(0.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(120.0),
          new DOF(0.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(150.0),
          new DOF(-30.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(180.0),
          new DOF(0.0)
        }));
        int[] numArray = new int[2]{ 0, 6 };
        foreach (int index in numArray)
        {
          foreach (DOF doF in nodes[index].DOFs)
          {
            doF.Pinned = true;
            doF.Free = false;
          }
        }
        nodes[6].DOFs[0].Pinned = false;
        nodes[1].DOFs[0].Free = true;
        nodes[1].DOFs[1].Free = true;
        nodes[3].DOFs[0].Free = true;
        nodes[3].DOFs[1].Free = true;
        nodes[2].DOFs[1].Free = true;
        nodes[1].DOFs[0].AllowableVariation = new double?(20.0);
        nodes[1].DOFs[1].AllowableVariation = new double?(40.0);
        nodes[3].DOFs[0].AllowableVariation = new double?(20.0);
        nodes[3].DOFs[1].AllowableVariation = new double?(40.0);
        nodes[2].DOFs[1].AllowableVariation = new double?(40.0);
        this.ApplyRelation(nodes[1], nodes[5]);
        this.ApplyRelation(nodes[0], nodes[6]);
        this.ApplyRelation(nodes[3], nodes[4]);
        LoadCase lc = new LoadCase("LC1");
        lc.Loads.Add(new Load(-10.0, lc, nodes[3].DOFs[1]));
        lc.Loads.Add(new Load(-10.0, lc, nodes[4].DOFs[1]));
        for (int index = 0; index < nodes.Count; ++index)
          nodes[index].Index = index;
        members.Add(new Member(nodes[0], nodes[3]));
        members.Add(new Member(nodes[0], nodes[1]));
        members.Add(new Member(nodes[1], nodes[2]));
        members.Add(new Member(nodes[2], nodes[4]));
        members.Add(new Member(nodes[3], nodes[4]));
        members.Add(new Member(nodes[1], nodes[3]));
        members.Add(new Member(nodes[3], nodes[2]));
        members.Add(new Member(nodes[4], nodes[5]));
        members.Add(new Member(nodes[2], nodes[5]));
        members.Add(new Member(nodes[4], nodes[6]));
        members.Add(new Member(nodes[5], nodes[6]));
        Material material = new Material(29000.0, 0.000284, 20.0, "Steel");
        foreach (Member member in members)
        {
          member.Material = material;
          member.Area = 1.0;
        }
        double?[] nullableArray = new double?[2];
        nullableArray[0] = new double?(nodes[2].DOFs[0].Coord);
        return new ComputedStructure(new Structure(nodes, members)
        {
          SymmetryLine = nullableArray,
          StructType = Structure.StructureType.Truss,
          LoadCases = {
            lc
          }
        });
      }
    }

    private ComputedStructure Test2
    {
      get
      {
        int num = 5;
        double dim = 24.0;
        Material mat = new Material(29000.0, 0.000284, 20.0, "Steel");
        return this.GenericFrame(num, num, dim, mat, true, 20.0, -5.0, 20.0);
      }
    }

    private ComputedStructure GenericFrame(
      int numCol,
      int numBeam,
      double dim,
      Material mat,
      bool isSym,
      double latLoad,
      double gravLoad,
      double allowVar)
    {
      List<Node> nodes = new List<Node>();
      if (numBeam % 2 == 0)
        throw new Exception("This method only supports an odd number of top-beam nodes.");
      for (int index = 0; index < numCol; ++index)
      {
        Node node1 = new Node(new DOF[2]
        {
          new DOF(-(2.0 * dim) * (double) ((numBeam - 1) / 2)),
          new DOF((double) index * (2.0 * dim))
        });
        double coord1;
        double coord2;
        if (index != numCol - 1)
        {
          coord1 = node1.DOFs[0].Coord + dim;
          coord2 = node1.DOFs[1].Coord + dim;
        }
        else
        {
          coord1 = node1.DOFs[0].Coord + dim;
          coord2 = 0.0;
        }
        Node node2 = new Node(new DOF[2]
        {
          new DOF(coord1),
          new DOF(coord2)
        });
        nodes.Add(node1);
        nodes.Add(node2);
      }
      List<Node> collection = new List<Node>();
      foreach (Node node3 in nodes)
      {
        Node node4 = new Node(new DOF[2]
        {
          new DOF(-1.0 * node3.DOFs[0].Coord),
          new DOF(node3.DOFs[1].Coord)
        });
        collection.Add(node4);
      }
      nodes.AddRange((IEnumerable<Node>) collection);
      for (int index = 0; index < 1 + (numBeam - 3) * 2; ++index)
      {
        Node node = new Node(new DOF[2]
        {
          new DOF((double) ((numBeam - 1) / 2 - 1) * -(2.0 * dim) + dim * (double) index),
          new DOF((double) (numCol - 1) * (2.0 * dim) - (double) (index % 2) * dim)
        });
        nodes.Add(node);
      }
      for (int index = 0; index < nodes.Count; ++index)
        nodes[index].Index = index;
      List<Member> members = new List<Member>();
      for (int index = 0; index < 2 * numCol - 2; ++index)
      {
        Member member1 = new Member(nodes[index], nodes[index + 1]);
        Member member2 = new Member(nodes[2 * numCol + index], nodes[2 * numCol + index + 1]);
        members.Add(member1);
        members.Add(member2);
      }
      for (int index = 0; index < 2 * numCol - 3; ++index)
      {
        Member member3 = new Member(nodes[index], nodes[index + 2]);
        Member member4 = new Member(nodes[2 * numCol + index], nodes[2 * numCol + index + 2]);
        members.Add(member3);
        members.Add(member4);
      }
      int[][] numArray1 = new int[8][]
      {
        new int[2]{ 0, 2 * numCol - 1 },
        new int[2]{ 1, 2 * numCol - 1 },
        new int[2]{ 2 * numCol, 4 * numCol - 1 },
        new int[2]{ 2 * numCol + 1, 4 * numCol - 1 },
        new int[2]{ 2 * numCol - 2, 4 * numCol },
        new int[2]{ 2 * numCol - 3, 4 * numCol + 1 },
        new int[2]
        {
          4 * numCol - 3,
          4 * numCol + 2 * (numBeam - 3) - 1
        },
        new int[2]{ 4 * numCol - 2, 4 * numCol + 2 * (numBeam - 3) }
      };
      foreach (int[] numArray2 in numArray1)
      {
        Member member = new Member(nodes[numArray2[0]], nodes[numArray2[1]]);
        members.Add(member);
      }
      for (int index = 0; index < (numBeam - 3) * 2; ++index)
      {
        Member member = new Member(nodes[index + 4 * numCol], nodes[index + 4 * numCol + 1]);
        members.Add(member);
      }
      for (int index = 0; index < (numBeam - 3) * 2 - 1; ++index)
      {
        Member member = new Member(nodes[index + 4 * numCol], nodes[index + 4 * numCol + 2]);
        members.Add(member);
      }
      members.Add(new Member(nodes[2 * numCol - 3], nodes[4 * numCol]));
      members.Add(new Member(nodes[4 * numCol - 3], nodes[4 * numCol + 2 * (numBeam - 3)]));
      foreach (Member member in members)
      {
        member.Area = 1.0;
        member.Material = mat;
      }
      int[] numArray3 = new int[4]
      {
        0,
        2 * numCol - 1,
        2 * numCol,
        4 * numCol - 1
      };
      foreach (int index in numArray3)
      {
        foreach (DOF doF in nodes[index].DOFs)
          doF.Pinned = true;
      }
      foreach (Node node in nodes)
      {
        foreach (DOF doF in node.DOFs)
          doF.Free = true;
      }
      for (int index = 0; index <= 4 * numCol + 2 * (numBeam - 3); index += 2)
      {
        nodes[index].DOFs[0].Free = false;
        nodes[index].DOFs[1].Free = false;
      }
      nodes[2 * numCol - 1].DOFs[1].Free = false;
      nodes[4 * numCol - 1].DOFs[1].Free = false;
      LoadCase lc = new LoadCase("lc1");
      LoadCase loadCase = new LoadCase("lc2");
      int[] numArray4 = new int[1]{ 2 * numCol - 2 };
      foreach (int index in numArray4)
      {
        lc.Loads.Add(new Load(gravLoad, lc, nodes[index].DOFs[1]));
        loadCase.Loads.Add(new Load(gravLoad, lc, nodes[index].DOFs[1]));
      }
      for (int index = 4 * numCol - 2; index <= 4 * numCol + 2 * (numBeam - 3); index += 2)
      {
        lc.Loads.Add(new Load(gravLoad, lc, nodes[index].DOFs[1]));
        loadCase.Loads.Add(new Load(gravLoad, lc, nodes[index].DOFs[1]));
      }
      int[] numArray5 = new int[1]{ 2 * numCol - 2 };
      foreach (int index in numArray5)
        loadCase.Loads.Add(new Load(latLoad, lc, nodes[index].DOFs[0]));
      int[] numArray6 = new int[1]{ 2 * numCol - 2 };
      foreach (int index in numArray6)
        lc.Loads.Add(new Load(-latLoad, lc, nodes[index].DOFs[0]));
      Structure s = new Structure(nodes, members);
      if (isSym)
      {
        for (int index = 0; index < 2 * numCol; ++index)
          this.ApplyRelation(nodes[index], nodes[index + 2 * numCol]);
        for (int index = 0; index < numBeam - 3; ++index)
          this.ApplyRelation(nodes[4 * numCol + index], nodes[4 * numCol + 2 * (numBeam - 3) - index]);
        double?[] nullableArray1 = new double?[2];
        nullableArray1[0] = new double?(0.0);
        double?[] nullableArray2 = nullableArray1;
        s.SymmetryLine = nullableArray2;
      }
      s.StructType = Structure.StructureType.Frame;
      foreach (DOF designVariable in s.DesignVariables)
        designVariable.AllowableVariation = new double?(allowVar);
      s.LoadCases.Add(lc);
      s.LoadCases.Add(loadCase);
      return new ComputedStructure(s);
    }

    private ComputedStructure Test3
    {
      get
      {
        List<Node> nodes = new List<Node>();
        List<Member> members = new List<Member>();
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(248.0),
          new DOF(189.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(587.0),
          new DOF(306.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(56.0),
          new DOF(510.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(190.0),
          new DOF(580.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(329.0),
          new DOF(510.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(482.0),
          new DOF(580.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(593.0),
          new DOF(510.0)
        }));
        nodes[0].DOFs[0].Pinned = true;
        nodes[0].DOFs[1].Pinned = true;
        nodes[1].DOFs[0].Pinned = true;
        nodes[1].DOFs[1].Pinned = true;
        LoadCase lc = new LoadCase("lc1");
        int[] numArray = new int[5]{ 2, 3, 4, 5, 6 };
        foreach (int index in numArray)
          lc.Loads.Add(new Load(-15.0, lc, nodes[index].DOFs[1]));
        foreach (Node node in nodes)
          node.DOFs[0].PreFix = true;
        nodes[0].DOFs[1].PreFix = true;
        nodes[1].DOFs[1].PreFix = true;
        return new ComputedStructure(new Structure(nodes, members)
        {
          StructType = Structure.StructureType.Truss,
          LoadCases = {
            lc
          }
        });
      }
    }

    private ComputedStructure Test3a
    {
      get
      {
        List<Node> nodes = new List<Node>();
        List<Member> members = new List<Member>();
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(0.0),
          new DOF(0.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(50.0),
          new DOF(0.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(25.0),
          new DOF(300.0)
        }));
        nodes[0].DOFs[0].Pinned = true;
        nodes[0].DOFs[1].Pinned = true;
        nodes[1].DOFs[0].Pinned = true;
        nodes[1].DOFs[1].Pinned = true;
        LoadCase lc = new LoadCase("lc1");
        lc.Loads.Add(new Load(-15.0, lc, nodes[2].DOFs[1]));
        lc.Loads.Add(new Load(-15.0, lc, nodes[2].DOFs[0]));
        foreach (Node node in nodes)
          node.DOFs[0].PreFix = true;
        nodes[0].DOFs[1].PreFix = true;
        nodes[1].DOFs[1].PreFix = true;
        return new ComputedStructure(new Structure(nodes, members)
        {
          StructType = Structure.StructureType.Truss,
          LoadCases = {
            lc
          }
        });
      }
    }

    private ComputedStructure simpletruss
    {
      get
      {
        List<Node> nodes = new List<Node>();
        List<Member> members = new List<Member>();
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(0.0),
          new DOF(0.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(30.0),
          new DOF(-30.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(90.0),
          new DOF(-30.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(60.0),
          new DOF(0.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(120.0),
          new DOF(0.0)
        }));
        int[] numArray = new int[2]{ 0, 4 };
        foreach (int index in numArray)
        {
          foreach (DOF doF in nodes[index].DOFs)
          {
            doF.Pinned = true;
            doF.Free = false;
          }
        }
        nodes[4].DOFs[0].Pinned = false;
        nodes[1].DOFs[0].Free = true;
        nodes[1].DOFs[1].Free = true;
        nodes[3].DOFs[1].Free = true;
        nodes[1].DOFs[0].AllowableVariation = new double?(20.0);
        nodes[1].DOFs[1].AllowableVariation = new double?(40.0);
        nodes[3].DOFs[1].AllowableVariation = new double?(40.0);
        this.ApplyRelation(nodes[1], nodes[2]);
        this.ApplyRelation(nodes[0], nodes[4]);
        LoadCase lc = new LoadCase("lc1");
        lc.Loads.Add(new Load(-10.0, lc, nodes[3].DOFs[1]));
        for (int index = 0; index < nodes.Count; ++index)
          nodes[index].Index = index;
        members.Add(new Member(nodes[0], nodes[3]));
        members.Add(new Member(nodes[0], nodes[1]));
        members.Add(new Member(nodes[1], nodes[2]));
        members.Add(new Member(nodes[2], nodes[4]));
        members.Add(new Member(nodes[3], nodes[4]));
        members.Add(new Member(nodes[1], nodes[3]));
        members.Add(new Member(nodes[3], nodes[2]));
        Material material = new Material(29000.0, 0.000284, 20.0, "Steel");
        foreach (Member member in members)
        {
          member.Material = material;
          member.Area = 1.0;
        }
        double?[] nullableArray = new double?[2];
        nullableArray[0] = new double?(nodes[3].DOFs[0].Coord);
        return new ComputedStructure(new Structure(nodes, members)
        {
          SymmetryLine = nullableArray,
          StructType = Structure.StructureType.Truss,
          LoadCases = {
            lc
          }
        });
      }
    }

    private ComputedStructure surrogatetruss
    {
      get
      {
        ComputedStructure simpletruss = this.simpletruss;
        simpletruss.Nodes[1].DOFs[0].Free = false;
        simpletruss.Nodes[1].DOFs[0].AllowableVariation = new double?(0.0);
        simpletruss.Nodes[1].DOFs[1].AllowableVariation = new double?(50.0);
        simpletruss.Nodes[3].DOFs[1].AllowableVariation = new double?(50.0);
        simpletruss.StructType = Structure.StructureType.Truss;
        return new ComputedStructure((Structure) simpletruss);
      }
    }

    private ComputedStructure portalframe
    {
      get
      {
        int num = 7;
        double dim = 24.0;
        Material mat = new Material(29000.0, 0.000284, 20.0, "Steel");
        return this.GenericFrame(num, num, dim, mat, true, 20.0, -5.0, 20.0);
      }
    }

    private ComputedStructure chiassoroof
    {
      get
      {
        List<Node> nodes = new List<Node>();
        List<Member> members = new List<Member>();
        double num1 = 73.819;
        double num2 = 98.425;
        double num3 = num2 / (4.0 * num1);
        LoadCase lc = new LoadCase("lc1");
        for (int index = 0; index < 5; ++index)
        {
          double coord1 = num1 * (double) index;
          double coord2 = num3 * coord1;
          Node node1 = new Node(new DOF[2]
          {
            new DOF(coord1),
            new DOF(coord2)
          });
          lc.Loads.Add(new Load(-11.24, lc, node1.DOFs[1]));
          nodes.Add(node1);
          if (index != 0)
          {
            Node node2 = new Node(new DOF[2]
            {
              new DOF(coord1),
              new DOF(0.0)
            });
            node2.DOFs[1].Free = true;
            node2.DOFs[1].AllowableVariation = new double?(0.8 * num2);
            node2.DOFs[1].AllowableVariation = new double?(157.48);
            nodes.Add(node2);
          }
        }
        double?[] nullableArray = new double?[2];
        nullableArray[0] = new double?(nodes[7].DOFs[0].Coord);
        List<Node> collection = new List<Node>();
        for (int index = 0; index < nodes.Count - 2; ++index)
        {
          Node node3 = nodes[index];
          Node node4 = new Node(new DOF[2]
          {
            new DOF(2.0 * nullableArray[0].Value - node3.DOFs[0].Coord),
            new DOF(node3.DOFs[1].Coord)
          });
          ParametricRelation parametricRelation1 = new ParametricRelation(new List<Node>()
          {
            node4
          }, RelationType.Mirror);
          ParametricRelation parametricRelation2 = new ParametricRelation(new List<Node>()
          {
            node4
          }, RelationType.Master);
          node3.DOFs[0].Relations.Add(parametricRelation1);
          node3.DOFs[1].Relations.Add(parametricRelation2);
          lc.Loads.Add(new Load(lc.GetLoad(node3.DOFs[1]).Value, lc, node4.DOFs[1]));
          collection.Add(node4);
        }
        nodes.AddRange((IEnumerable<Node>) collection);
        nodes[0].DOFs[0].Pinned = true;
        nodes[0].DOFs[1].Pinned = true;
        nodes[9].DOFs[1].Pinned = true;
        for (int index = 0; index < nodes.Count; ++index)
          nodes[index].Index = index;
        for (int index1 = 1; index1 < 6; index1 += 2)
        {
          int index2 = index1 + 1;
          Member member1 = new Member(nodes[index1], nodes[index1 + 2]);
          Member member2 = new Member(nodes[index2], nodes[index2 + 2]);
          Member member3 = new Member(nodes[index1], nodes[index2]);
          Member member4 = new Member(nodes[index1], nodes[index1 + 3]);
          members.Add(member1);
          members.Add(member2);
          members.Add(member3);
          members.Add(member4);
        }
        members.Add(new Member(nodes[0], nodes[1]));
        members.Add(new Member(nodes[0], nodes[2]));
        for (int index3 = 10; index3 < 13; index3 += 2)
        {
          int index4 = index3 + 1;
          Member member5 = new Member(nodes[index3], nodes[index3 + 2]);
          Member member6 = new Member(nodes[index4], nodes[index4 + 2]);
          Member member7 = new Member(nodes[index3], nodes[index4]);
          Member member8 = new Member(nodes[index3], nodes[index3 + 3]);
          members.Add(member5);
          members.Add(member6);
          members.Add(member7);
          members.Add(member8);
        }
        members.Add(new Member(nodes[9], nodes[10]));
        members.Add(new Member(nodes[9], nodes[11]));
        members.Add(new Member(nodes[14], nodes[7]));
        members.Add(new Member(nodes[15], nodes[8]));
        members.Add(new Member(nodes[8], nodes[14]));
        members.Add(new Member(nodes[14], nodes[15]));
        members.Add(new Member(nodes[7], nodes[8]));
        Material material = new Material(29000.0, 0.000284, 20.0, "Steel");
        foreach (Member member in members)
        {
          member.Area = 1.0;
          member.Material = material;
        }
        return new ComputedStructure(new Structure(nodes, members)
        {
          SymmetryLine = nullableArray,
          LoadCases = {
            lc
          },
          StructType = Structure.StructureType.Truss
        });
      }
    }

    private ComputedStructure cantroof
    {
      get
      {
        List<Node> nodes = new List<Node>();
        List<Member> members = new List<Member>();
        double num1 = 120.0;
        double num2 = 240.0;
        double num3 = 0.16;
        double num4 = 24.0;
        double num5 = num2 - num4;
        LoadCase lc = new LoadCase("lc1");
        for (int index = 0; index < 10; ++index)
        {
          double coord1 = num1 * (double) index;
          double coord2 = num3 * coord1 + num2;
          Node node1 = new Node(new DOF[2]
          {
            new DOF(coord1),
            new DOF(coord2)
          });
          lc.Loads.Add(new Load(-10.0, lc, node1.DOFs[1]));
          nodes.Add(node1);
          if (index < 9)
          {
            Node node2 = new Node(new DOF[2]
            {
              new DOF(num1 * ((double) index + 0.5)),
              new DOF(num3 * coord1 + num5)
            });
            node2.DOFs[0].Free = true;
            node2.DOFs[1].Free = true;
            node2.DOFs[0].AllowableVariation = new double?(40.0);
            node2.DOFs[1].AllowableVariation = new double?(100.0);
            nodes.Add(node2);
          }
        }
        nodes[3].DOFs[0].Free = false;
        nodes[3].DOFs[0].AllowableVariation = new double?(0.0);
        nodes[13].DOFs[0].Free = false;
        nodes[13].DOFs[0].AllowableVariation = new double?(0.0);
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(156.0),
          new DOF(0.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(192.0),
          new DOF(0.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(756.0),
          new DOF(0.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(792.0),
          new DOF(0.0)
        }));
        for (int index = 19; index < 23; ++index)
        {
          nodes[index].DOFs[0].Pinned = true;
          nodes[index].DOFs[1].Pinned = true;
        }
        for (int index = 0; index < nodes.Count; ++index)
          nodes[index].Index = index;
        for (int index = 0; index < 18; ++index)
        {
          Member member1 = new Member(nodes[index], nodes[index + 1]);
          members.Add(member1);
          if (index < 17)
          {
            Member member2 = new Member(nodes[index], nodes[index + 2]);
            members.Add(member2);
          }
        }
        members.Add(new Member(nodes[3], nodes[19]));
        members.Add(new Member(nodes[3], nodes[20]));
        members.Add(new Member(nodes[13], nodes[21]));
        members.Add(new Member(nodes[13], nodes[22]));
        Material material = new Material(29000.0, 0.000284, 20.0, "Steel");
        foreach (Member member in members)
        {
          member.Area = 1.0;
          member.Material = material;
        }
        return new ComputedStructure(new Structure(nodes, members)
        {
          StructType = Structure.StructureType.Truss,
          LoadCases = {
            lc
          }
        });
      }
    }

    private ComputedStructure circulararch
    {
      get
      {
        List<Node> nodes = new List<Node>();
        List<Member> members = new List<Member>();
        LoadCase lc = new LoadCase("lc1");
        double num1 = Math.PI / 10.0;
        double num2 = 300.0;
        double num3 = 260.0;
        for (int index = 10; index > 4; --index)
        {
          double num4 = num1 * (double) index;
          double coord1 = num2 * Math.Cos(num4);
          double coord2 = num3 * Math.Cos(num4);
          double coord3 = num2 * Math.Sin(num4);
          double coord4 = num3 * Math.Sin(num4);
          Node node1 = new Node(new DOF[2]
          {
            new DOF(coord1),
            new DOF(coord3)
          });
          Node node2 = new Node(new DOF[2]
          {
            new DOF(coord2),
            new DOF(coord4)
          });
          double p1 = node1.DOFs[0].Coord - node2.DOFs[0].Coord;
          double p2 = node1.DOFs[1].Coord - node2.DOFs[1].Coord;
          ParametricRelation parametricRelation1 = new ParametricRelation(new List<Node>()
          {
            node1
          }, RelationType.Offset, (object) p1);
          ParametricRelation parametricRelation2 = new ParametricRelation(new List<Node>()
          {
            node1
          }, RelationType.Offset, (object) p2);
          node2.DOFs[0].Relations.Add(parametricRelation1);
          node2.DOFs[1].Relations.Add(parametricRelation2);
          lc.Loads.Add(new Load(-10.0, lc, node1.DOFs[1]));
          node2.DOFs[0].Free = true;
          node2.DOFs[1].Free = true;
          node2.DOFs[0].AllowableVariation = new double?(30.0);
          node2.DOFs[1].AllowableVariation = new double?(80.0);
          nodes.Add(node1);
          nodes.Add(node2);
        }
        nodes[0].DOFs[0].Pinned = true;
        nodes[0].DOFs[1].Pinned = true;
        nodes[1].DOFs[0].Pinned = true;
        nodes[1].DOFs[1].Pinned = true;
        nodes[1].DOFs[1].Free = false;
        nodes[1].DOFs[1].AllowableVariation = new double?(0.0);
        nodes[11].DOFs[0].Free = false;
        nodes[11].DOFs[0].AllowableVariation = new double?(0.0);
        double?[] nullableArray = new double?[2];
        nullableArray[0] = new double?(nodes[11].DOFs[0].Coord);
        List<Node> collection = new List<Node>();
        for (int index = 0; index < nodes.Count - 2; ++index)
        {
          Node node3 = nodes[index];
          Node node4 = new Node(new DOF[2]
          {
            new DOF(2.0 * nullableArray[0].Value - node3.DOFs[0].Coord),
            new DOF(node3.DOFs[1].Coord)
          });
          ParametricRelation parametricRelation3 = new ParametricRelation(new List<Node>()
          {
            node4
          }, RelationType.Mirror);
          ParametricRelation parametricRelation4 = new ParametricRelation(new List<Node>()
          {
            node4
          }, RelationType.Master);
          node3.DOFs[0].Relations.Add(parametricRelation3);
          node3.DOFs[1].Relations.Add(parametricRelation4);
          lc.Loads.Add(new Load(lc.GetLoad(node3.DOFs[1]).Value, lc, node4.DOFs[1]));
          node4.DOFs[0].Pinned = node3.DOFs[0].Pinned;
          node4.DOFs[1].Pinned = node3.DOFs[1].Pinned;
          collection.Add(node4);
        }
        nodes.AddRange((IEnumerable<Node>) collection);
        lc.GetLoad(nodes[0].DOFs[1]).Value = 0.0;
        lc.GetLoad(nodes[12].DOFs[1]).Value = 0.0;
        for (int index = 0; index < nodes.Count; ++index)
          nodes[index].Index = index;
        for (int index = 0; index < 9; ++index)
        {
          Member member1 = new Member(nodes[index], nodes[index + 2]);
          Member member2 = new Member(nodes[index], nodes[index + 1]);
          members.Add(member1);
          members.Add(member2);
        }
        for (int index = 12; index < 19; ++index)
        {
          Member member3 = new Member(nodes[index], nodes[index + 2]);
          Member member4 = new Member(nodes[index], nodes[index + 1]);
          members.Add(member3);
          members.Add(member4);
        }
        for (int index = 9; index < 11; ++index)
        {
          Member member5 = new Member(nodes[index], nodes[index + 1]);
          Member member6 = new Member(nodes[index + 10], nodes[index + 11]);
          members.Add(member5);
          members.Add(member6);
        }
        members.Add(new Member(nodes[21], nodes[11]));
        members.Add(new Member(nodes[20], nodes[10]));
        members.Add(new Member(nodes[21], nodes[10]));
        members.Add(new Member(nodes[9], nodes[11]));
        members.Add(new Member(nodes[19], nodes[21]));
        Material material = new Material(29000.0, 0.000284, 20.0, "Steel");
        foreach (Member member in members)
        {
          member.Area = 1.0;
          member.Material = material;
        }
        return new ComputedStructure(new Structure(nodes, members)
        {
          SymmetryLine = nullableArray,
          StructType = Structure.StructureType.Truss,
          LoadCases = {
            lc
          }
        });
      }
    }

    private ComputedStructure tower
    {
      get
      {
        List<Node> nodes = new List<Node>();
        List<Member> members = new List<Member>();
        LoadCase lc = new LoadCase("lc1");
        double num1 = 144.0;
        double coord1 = 0.0;
        double coord2 = 240.0;
        double num2 = 7.0;
        for (int index = 0; (double) index < num2; ++index)
        {
          double coord3 = num1 * (double) index;
          Node node1 = new Node(new DOF[2]
          {
            new DOF(coord1),
            new DOF(coord3)
          });
          Node node2 = new Node(new DOF[2]
          {
            new DOF(coord2),
            new DOF(coord3)
          });
          ParametricRelation parametricRelation1 = new ParametricRelation(new List<Node>()
          {
            node2
          }, RelationType.Mirror);
          ParametricRelation parametricRelation2 = new ParametricRelation(new List<Node>()
          {
            node2
          }, RelationType.Master);
          node1.DOFs[0].Relations.Add(parametricRelation1);
          node1.DOFs[1].Relations.Add(parametricRelation2);
          lc.Loads.Add(new Load(50.0, lc, node1.DOFs[0]));
          node1.DOFs[0].Free = true;
          node1.DOFs[0].AllowableVariation = new double?(100.0);
          nodes.Add(node1);
          nodes.Add(node2);
          if ((double) index < num2 - 1.0)
          {
            Node node3 = new Node(new DOF[2]
            {
              new DOF(coord2 / 2.0),
              new DOF(coord3 + num1 / 2.0)
            });
            node3.DOFs[1].Free = true;
            node3.DOFs[1].AllowableVariation = new double?(72.0);
            nodes.Add(node3);
          }
        }
        lc.GetLoad(nodes[0].DOFs[0]).Value = 0.0;
        nodes[0].DOFs[0].Pinned = true;
        nodes[0].DOFs[1].Pinned = true;
        nodes[1].DOFs[0].Pinned = true;
        nodes[1].DOFs[1].Pinned = true;
        for (int index = 0; index < nodes.Count; ++index)
          nodes[index].Index = index;
        for (int index = 2; (double) index < num2 * 3.0 - 3.0; index += 3)
        {
          members.Add(new Member(nodes[index], nodes[index - 1]));
          members.Add(new Member(nodes[index], nodes[index - 2]));
          members.Add(new Member(nodes[index], nodes[index + 1]));
          members.Add(new Member(nodes[index], nodes[index + 2]));
          members.Add(new Member(nodes[index - 2], nodes[index + 1]));
          members.Add(new Member(nodes[index - 1], nodes[index + 2]));
          members.Add(new Member(nodes[index + 1], nodes[index + 2]));
        }
        Material material = new Material(29000.0, 0.000284, 20.0, "Steel");
        foreach (Member member in members)
        {
          member.Area = 1.0;
          member.Material = material;
        }
        double?[] nullableArray1 = new double?[2];
        nullableArray1[0] = new double?(coord2 / 2.0);
        double?[] nullableArray2 = nullableArray1;
        return new ComputedStructure(new Structure(nodes, members)
        {
          SymmetryLine = nullableArray2,
          StructType = Structure.StructureType.Truss,
          LoadCases = {
            lc
          }
        });
      }
    }

    private ComputedStructure bridge
    {
      get
      {
        List<Node> nodes = new List<Node>();
        List<Member> members = new List<Member>();
        double num1 = 120.0;
        double num2 = 72.0;
        LoadCase lc = new LoadCase("lc1");
        for (int index = 0; index < 4; ++index)
        {
          double coord1 = (double) index * num1;
          double coord2 = 0.0;
          Node node1 = new Node(new DOF[2]
          {
            new DOF(coord1),
            new DOF(coord2 + num2)
          });
          Node node2 = new Node(new DOF[2]
          {
            new DOF(coord1),
            new DOF(coord2)
          });
          node1.DOFs[1].Free = true;
          node1.DOFs[1].AllowableVariation = new double?(300.0);
          node1.DOFs[0].Free = true;
          node1.DOFs[0].AllowableVariation = new double?(40.0);
          lc.Loads.Add(new Load(-40.0, lc, node2.DOFs[1]));
          nodes.Add(node1);
          nodes.Add(node2);
        }
        double?[] nullableArray1 = new double?[2];
        nullableArray1[0] = new double?(360.0);
        double?[] nullableArray2 = nullableArray1;
        List<Node> collection = new List<Node>();
        for (int index = 0; index < nodes.Count - 2; ++index)
        {
          Node node3 = nodes[index];
          Node node4 = new Node(new DOF[2]
          {
            new DOF(2.0 * nullableArray2[0].Value - node3.DOFs[0].Coord),
            new DOF(node3.DOFs[1].Coord)
          });
          ParametricRelation parametricRelation1 = new ParametricRelation(new List<Node>()
          {
            node4
          }, RelationType.Mirror);
          ParametricRelation parametricRelation2 = new ParametricRelation(new List<Node>()
          {
            node4
          }, RelationType.Master);
          node3.DOFs[0].Relations.Add(parametricRelation1);
          node3.DOFs[1].Relations.Add(parametricRelation2);
          lc.Loads.Add(new Load(lc.GetLoad(node3.DOFs[1]).Value, lc, node4.DOFs[1]));
          node4.DOFs[0].Pinned = node3.DOFs[0].Pinned;
          node4.DOFs[1].Pinned = node3.DOFs[1].Pinned;
          collection.Add(node4);
        }
        nodes.AddRange((IEnumerable<Node>) collection);
        nodes[0].DOFs[0].Free = false;
        nodes[0].DOFs[0].AllowableVariation = new double?(0.0);
        nodes[6].DOFs[0].Free = false;
        nodes[6].DOFs[0].AllowableVariation = new double?(0.0);
        nodes[0].DOFs[0].Pinned = true;
        nodes[0].DOFs[1].Pinned = true;
        nodes[8].DOFs[0].Pinned = true;
        nodes[8].DOFs[1].Pinned = true;
        nodes[1].DOFs[1].Pinned = true;
        nodes[9].DOFs[1].Pinned = true;
        for (int index = 0; index < nodes.Count; ++index)
          nodes[index].Index = index;
        for (int index1 = 0; index1 < 5; ++index1)
        {
          int index2 = index1 + 8;
          Member member1 = new Member(nodes[index1], nodes[index1 + 2]);
          members.Add(member1);
          if (index2 < 12)
          {
            Member member2 = new Member(nodes[index2], nodes[index2 + 2]);
            members.Add(member2);
          }
          if (index1 % 2 == 0)
          {
            Member member3 = new Member(nodes[index1], nodes[index1 + 3]);
            Member member4 = new Member(nodes[index1], nodes[index1 + 1]);
            members.Add(member3);
            members.Add(member4);
            if (index2 < 12)
            {
              Member member5 = new Member(nodes[index2], nodes[index2 + 3]);
              Member member6 = new Member(nodes[index2], nodes[index2 + 1]);
              members.Add(member5);
              members.Add(member6);
            }
          }
        }
        members.Add(new Member(nodes[5], nodes[7]));
        members.Add(new Member(nodes[6], nodes[7]));
        members.Add(new Member(nodes[12], nodes[6]));
        members.Add(new Member(nodes[12], nodes[7]));
        members.Add(new Member(nodes[12], nodes[13]));
        members.Add(new Member(nodes[13], nodes[7]));
        Material material = new Material(29000.0, 0.000284, 20.0, "Steel");
        foreach (Member member in members)
        {
          member.Area = 1.0;
          member.Material = material;
        }
        return new ComputedStructure(new Structure(nodes, members)
        {
          SymmetryLine = nullableArray2,
          StructType = Structure.StructureType.Truss,
          LoadCases = {
            lc
          }
        });
      }
    }

    private ComputedStructure unstable
    {
      get
      {
        List<Node> nodes = new List<Node>();
        List<Member> members = new List<Member>();
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(0.0),
          new DOF(0.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(0.0),
          new DOF(100.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(150.0),
          new DOF(0.0)
        }));
        nodes.Add(new Node(new DOF[2]
        {
          new DOF(150.0),
          new DOF(100.0)
        }));
        LoadCase lc = new LoadCase("lc1");
        lc.Loads.Add(new Load(10.0, lc, nodes[1].DOFs[0]));
        nodes[1].DOFs[0].Free = true;
        nodes[1].DOFs[0].AllowableVariation = new double?(20.0);
        nodes[0].DOFs[0].Pinned = true;
        nodes[0].DOFs[1].Pinned = true;
        nodes[2].DOFs[0].Pinned = true;
        nodes[2].DOFs[1].Pinned = true;
        members.Add(new Member(nodes[0], nodes[1]));
        members.Add(new Member(nodes[1], nodes[3]));
        members.Add(new Member(nodes[2], nodes[3]));
        Material material = new Material(29000.0, 0.000284, 20.0, "Steel");
        foreach (Member member in members)
        {
          member.Area = 1.0;
          member.Material = material;
        }
        return new ComputedStructure(new Structure(nodes, members)
        {
          StructType = Structure.StructureType.Truss,
          LoadCases = {
            lc
          }
        });
      }
    }

    private ComputedStructure airportroof
    {
      get
      {
        int numCol = 6;
        int numBeam = 9;
        double dim = 29.52756;
        double s = 2.9;
        Material mat = new Material(57.0 * Math.Sqrt(3000.0), 8.68E-05, s, "Reinforced Concrete");
        RectangularSection rectangularSection = new RectangularSection(3.937, "Rec");
        double gravLoad = -5.62;
        double latLoad = 16.86;
        ComputedStructure airportroof = this.GenericFrame(numCol, numBeam, dim, mat, false, latLoad, gravLoad, 30.0);
        airportroof.Sections.Add((ISection) rectangularSection);
        foreach (Member member in airportroof.Members)
          member.SectionType = (ISection) rectangularSection;
        List<Node> nodes = airportroof.Nodes;
        DOF[] dofArray = new DOF[24]
        {
          nodes[1].DOFs[1],
          nodes[3].DOFs[1],
          nodes[5].DOFs[1],
          nodes[7].DOFs[1],
          nodes[13].DOFs[1],
          nodes[15].DOFs[1],
          nodes[17].DOFs[1],
          nodes[19].DOFs[1],
          nodes[25].DOFs[0],
          nodes[27].DOFs[0],
          nodes[29].DOFs[0],
          nodes[31].DOFs[0],
          nodes[33].DOFs[0],
          nodes[35].DOFs[0],
          nodes[1].DOFs[0],
          nodes[3].DOFs[0],
          nodes[7].DOFs[0],
          nodes[13].DOFs[0],
          nodes[15].DOFs[0],
          nodes[19].DOFs[0],
          nodes[25].DOFs[1],
          nodes[29].DOFs[1],
          nodes[31].DOFs[1],
          nodes[35].DOFs[1]
        };
        foreach (DOF dof in dofArray)
        {
          dof.Free = false;
          dof.AllowableVariation = new double?(0.0);
        }
        nodes[5].DOFs[0].Relations.Add(new ParametricRelation(new List<Node>()
        {
          nodes[1],
          nodes[3]
        }, RelationType.Average, (object) nodes[11]));
        nodes[11].DOFs[0].Relations.Add(new ParametricRelation(new List<Node>()
        {
          nodes[1],
          nodes[3]
        }, RelationType.Average, (object) nodes[5]));
        nodes[5].DOFs[0].Relations.Add(new ParametricRelation(new List<Node>()
        {
          nodes[7]
        }, RelationType.Average, (object) nodes[9]));
        nodes[9].DOFs[0].Relations.Add(new ParametricRelation(new List<Node>()
        {
          nodes[7]
        }, RelationType.Average, (object) nodes[5]));
        nodes[17].DOFs[0].Relations.Add(new ParametricRelation(new List<Node>()
        {
          nodes[13],
          nodes[15]
        }, RelationType.Average, (object) nodes[23]));
        nodes[23].DOFs[0].Relations.Add(new ParametricRelation(new List<Node>()
        {
          nodes[13],
          nodes[15]
        }, RelationType.Average, (object) nodes[17]));
        nodes[17].DOFs[0].Relations.Add(new ParametricRelation(new List<Node>()
        {
          nodes[19]
        }, RelationType.Average, (object) nodes[21]));
        nodes[21].DOFs[0].Relations.Add(new ParametricRelation(new List<Node>()
        {
          nodes[19]
        }, RelationType.Average, (object) nodes[17]));
        nodes[27].DOFs[1].Relations.Add(new ParametricRelation(new List<Node>()
        {
          nodes[25]
        }, RelationType.Average, (object) nodes[9]));
        nodes[9].DOFs[1].Relations.Add(new ParametricRelation(new List<Node>()
        {
          nodes[25]
        }, RelationType.Average, (object) nodes[27]));
        nodes[27].DOFs[1].Relations.Add(new ParametricRelation(new List<Node>()
        {
          nodes[29],
          nodes[31]
        }, RelationType.Average, (object) nodes[33]));
        nodes[33].DOFs[1].Relations.Add(new ParametricRelation(new List<Node>()
        {
          nodes[29],
          nodes[31]
        }, RelationType.Average, (object) nodes[27]));
        nodes[33].DOFs[1].Relations.Add(new ParametricRelation(new List<Node>()
        {
          nodes[35]
        }, RelationType.Average, (object) nodes[21]));
        nodes[21].DOFs[1].Relations.Add(new ParametricRelation(new List<Node>()
        {
          nodes[35]
        }, RelationType.Average, (object) nodes[33]));
        return airportroof;
      }
    }

    public IDesign GetDesign(int i)
    {
      if (i <= this.Designs.Count)
        return this.Designs[i - 1];
      throw new Exception("List does not contain desired structure.");
    }

    private void ApplyRelation(Node leader, Node listener)
    {
      List<Node> l = new List<Node>() { listener };
      leader.DOFs[0].Relations.Add(new ParametricRelation(l, RelationType.Mirror));
      leader.DOFs[1].Relations.Add(new ParametricRelation(l, RelationType.Master));
      listener.DOFs[0].Free = false;
      listener.DOFs[1].Free = false;
    }
  }
}
