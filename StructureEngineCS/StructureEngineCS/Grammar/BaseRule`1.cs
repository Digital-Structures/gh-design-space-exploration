// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.BaseRule`1
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public abstract class BaseRule<T> : IRule where T : class, IShape
  {
    public BaseRule()
    {
      this.Name = "(unnamed rule)";
      this.Description = "(no description)";
      this.Params = new List<IRuleParameter>();
    }

    public IGrammar MyGrammar { get; set; }

    public string Name { get; protected set; }

    public string Description { get; protected set; }

    public List<IRuleParameter> Params { get; protected set; }

    public IShapeState LHSLabel { get; protected set; }

    public IShapeState RHSLabel { get; protected set; }

    public double Weight { get; protected set; }

    public bool CanApply(IShape s)
    {
      return s is T s1 && s.ShapeState == this.LHSLabel && this.CanApply(s1);
    }

    public virtual bool CanApply(T s) => true;

    public void Apply(IShape s, params object[] p)
    {
      if (!(s is T s1))
        return;
      this.Apply(s1, p);
      s1.ShapeState = this.RHSLabel;
      s1.History.AddRule(new RuleSet((IRule) this, p));
    }

    public virtual void Apply(T s, params object[] p)
    {
    }
  }
}
