using System;
using System.Collections.Generic;
using StructureEngine.Grammar.Airport;
using StructureEngine.Grammar.Bridge;
using StructureEngine.Grammar.Simple;
using StructureEngine.Model;

namespace StructureEngine.Grammar
{
    public class GrammarSetup : ISetup
    {
        public GrammarSetup()
        {
            this.Grammars = new List<IGrammar>();
            Grammars.Add(new SimpleGrammar());
            Grammars.Add(new BridgeGrammar());
            Grammars.Add(new AirportGrammar());

            this.Designs = new List<IDesign>();
            this.Rand = new Random(5);
            this.Setup(Designs);
            
        }

        private Random Rand;

        public List<IGrammar> Grammars
        {
            get;
            set;
        }

        public List<IDesign> Designs
        {
            get;
            set;
        }

        private void Setup(List<IDesign> designs)
        {
            RandomComputation r1 = new RandomComputation(Grammars[0]);
            IShape simple = r1.GenerateRandShape();
            designs.Add(simple);

            RandomComputation r2 = new RandomComputation(Grammars[1]);
            IShape bridge = r2.GenerateRandShape();
            designs.Add(bridge);

            //RandomComputation r3 = new RandomComputation(Rand, Grammars[2]);
            //IShape airport = r3.GenerateRandShape();
            //designs.Add(airport);
        }

    }
}
