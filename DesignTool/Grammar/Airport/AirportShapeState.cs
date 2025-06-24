
namespace StructureEngine.Grammar.Airport
{
    public class AirportShapeState : IShapeState
    {
        public AirportShapeState(string name)
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
            return this == AirportShapeState.End;
        }

        public bool Equals(IShapeState other)
        {
            AirportShapeState sss = other as AirportShapeState;
            return sss != null && this.Name == sss.Name;
        }

        public static AirportShapeState Start = new AirportShapeState("Start");
        public static AirportShapeState ModifySpan = new AirportShapeState("ModifySpan");
        public static AirportShapeState AddVerticals = new AirportShapeState("AddVerticals");
        public static AirportShapeState ModifyVerticals = new AirportShapeState("ModifyVerticals");
        public static AirportShapeState End = new AirportShapeState("End");
    }
}
