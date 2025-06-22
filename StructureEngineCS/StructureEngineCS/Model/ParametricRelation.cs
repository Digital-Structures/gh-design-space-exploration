// Decompiled with JetBrains decompiler
// Type: StructureEngine.Model.ParametricRelation
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Model
{
  public class ParametricRelation
  {
    public ParametricRelation(List<Node> l, RelationType r)
    {
      this.Listeners = l;
      this.Relation = r;
    }

    public ParametricRelation(List<Node> l, RelationType r, object p)
      : this(l, r)
    {
      this.Parameter = p;
    }

    public List<Node> Listeners { get; set; }

    public RelationType Relation { get; set; }

    public object Parameter { get; set; }
  }
}
