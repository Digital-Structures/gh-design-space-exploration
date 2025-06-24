namespace StructureEngine.Grammar.Bridge
{
    public class BridgeShapeState : IShapeState
    {
        public BridgeShapeState(string name)
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
            return this == BridgeShapeState.End;
        }

        public bool Equals(IShapeState other)
        {
            BridgeShapeState sss = other as BridgeShapeState;
            return sss != null && this.Name == sss.Name;
        }

        public static BridgeShapeState MakeTower = new BridgeShapeState("MakeTower");
        public static BridgeShapeState AddBranches = new BridgeShapeState("AddBranches");
        public static BridgeShapeState ModifyTower = new BridgeShapeState("ModifyTower");
        public static BridgeShapeState MakeDeck = new BridgeShapeState("MakeDeck");
        public static BridgeShapeState MakeInfill = new BridgeShapeState("MakeInfill");
        public static BridgeShapeState Subdivide = new BridgeShapeState("Subdivide");
        public static BridgeShapeState AddSupports = new BridgeShapeState("AddSupports");
        public static BridgeShapeState ConnectSupports = new BridgeShapeState("ConnectSupports");
        public static BridgeShapeState ModifySupports = new BridgeShapeState("ModifySupports");
        public static BridgeShapeState CableShape = new BridgeShapeState("CableShape");
        public static BridgeShapeState MultipleTowers = new BridgeShapeState("MultipleTowers");
        public static BridgeShapeState End = new BridgeShapeState("End");
    }
}
