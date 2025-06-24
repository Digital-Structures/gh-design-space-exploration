
namespace StructureEngine.Grammar.Airport.Rules
{
    public class Rule004 : BaseRule<AirportShape>
    {
        // This rule creates vertical supports
        public Rule004()
        {
            this.Name = "Rule 04";
            this.Params.Add(new DoubleParameter(10, 40, "Height Above Ground")); // height above ground at lowest support
            this.LHSLabel = AirportShapeState.AddVerticals;
            this.LHSLabel = AirportShapeState.ModifyVerticals;
        }

        public override void Apply(AirportShape s, object[] p)
        {
            // First, set parameter values.
            double height = (double)p[0]; 

            // Next, modify existing shape object.
            foreach (ShapePoint pt in s.Points)
            {
                pt.Y = pt.Y + height;
            }

            s.Verticals.Add(new ShapeLine(new ShapePoint(0, 0), s.Start));
            s.Verticals.Add(new ShapeLine(new ShapePoint(s.End.X, 0), s.End));
        }
    }
}
