
namespace StructureEngine.Grammar.Simple
{
    public class SimpleShapeState : IShapeState
    {
        private SimpleShapeState(string name)
        {
            this.Name = name;
        }

        public string Name
        {
            get;
            private set;
        }

        public bool IsEnd()
        {
            return this == SimpleShapeState.End;
        }

        public bool Equals(IShapeState other)
        {
            SimpleShapeState sss = other as SimpleShapeState;
            return sss != null && this.Name == sss.Name;
        }

        public static SimpleShapeState Start = new SimpleShapeState("Start");
        public static SimpleShapeState SubdivideDeck = new SimpleShapeState("SubdivideDeck");
        public static SimpleShapeState AddFunicular = new SimpleShapeState("AddFunicular");
        public static SimpleShapeState End = new SimpleShapeState("End");
    }
}
