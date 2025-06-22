namespace StructureEngine.Grammar.Bridge.Rules
{
    /// <summary>
    /// This rule rotates a tower element
    /// </summary>
    public class Rule002 : BaseRule<BridgeShape>
    {
        public Rule002()
        {
            this.Name = "Rule 02";
            this.Description = "Rotates a tower element about its base.";
            this.Params.Add(new DoubleParameter(-10, 10, "Rotation Angle")); // rotation angle of tower in degrees
            this.LHSLabel = BridgeShapeState.MakeTower;
            this.RHSLabel = BridgeShapeState.MakeTower;
        }

        public override void Apply(BridgeShape s, object[] p)
        {
            // First, set parameter values.
            double rotationangle = (double)p[0];

            // Next, modify existing shape object.
            //s.Tower[0] = new Shape_Line(s.Tower[0].start, rotationangle, s.Tower[0].length);
            s.Rotate(rotationangle, s.Tower[0].Start);
        }
    }
}
