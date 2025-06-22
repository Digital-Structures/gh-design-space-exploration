// Decompiled with JetBrains decompiler
// Type: StructureEngine.Grammar.GrammarSetup
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using StructureEngine.Grammar.Airport;
using StructureEngine.Grammar.Bridge;
using StructureEngine.Grammar.Simple;
using StructureEngine.Model;
using System;
using System.Collections.Generic;

#nullable disable
namespace StructureEngine.Grammar
{
  public class GrammarSetup : ISetup
  {
    private Random Rand;

    public GrammarSetup()
    {
      this.Grammars = new List<IGrammar>();
      this.Grammars.Add((IGrammar) new SimpleGrammar());
      this.Grammars.Add((IGrammar) new BridgeGrammar());
      this.Grammars.Add((IGrammar) new AirportGrammar());
      this.Designs = new List<IDesign>();
      this.Rand = new Random(5);
      this.Setup(this.Designs);
    }

    public List<IGrammar> Grammars { get; set; }

    public List<IDesign> Designs { get; set; }

    private void Setup(List<IDesign> designs)
    {
      IShape randShape1 = new RandomComputation(this.Grammars[0]).GenerateRandShape();
      designs.Add((IDesign) randShape1);
      IShape randShape2 = new RandomComputation(this.Grammars[1]).GenerateRandShape();
      designs.Add((IDesign) randShape2);
    }
  }
}
