using System.Collections.Generic;
using StructureEngine.Grammar;

namespace StructureEngine.GraphicStatics
{
    public class ProblemSetup
    {
        public ProblemSetup(ShapePoint start, ShapePoint end, int inc, double loadperlength)
        {
            this.Start = start;
            this.End = end;
            this.Setup(inc, loadperlength);
        }

        public ProblemSetup(ShapePoint start, ShapePoint end, List<ShapeLine> loads, List<double> widths)
        {
            this.Start = start;
            this.End = end;
            this.SetupLoads(loads);
            this.SegWidths = widths;
        }

        public List<PointLoad> Loads;

        public ShapePoint Start, End;

        public double DeltaX, DeltaY, Slope, Int;

        public List<double> SegWidths;

        public double TotalLoad;

        public ForcePolygon ForceP;

        public FormDiagram FormD;

        public ShapeLine ClosingString;

        private void Setup(int inc, double loadperlength)
        {
            this.GenerateLoads(inc, loadperlength);
            this.ClosingString = new ShapeLine(this.Start, this.End);
            this.TotalLoad = (End.X - Start.X) * loadperlength;
        }

        private void SetupLoads(List<ShapeLine> loads)
        {
            List<PointLoad> p = new List<PointLoad>();
            foreach (ShapeLine s in loads)
            {
                var pl = new PointLoad(s.Start, s.End.Y - s.Start.Y);
                p.Add(pl);
            }
            this.Loads = p;

            this.DeltaX = End.X - Start.X;
            this.DeltaY = End.Y - Start.Y;
            this.Slope = DeltaY / DeltaX;
        }

        private void GenerateLoads(int inc, double loadperlength)
        {
            this.DeltaX = End.X - Start.X;
            this.DeltaY = End.Y - Start.Y;
            this.Slope = DeltaY / DeltaX;
            this.Int = Start.Y - Slope * Start.X;

            SegWidths = new List<double>();
            double segwidth = DeltaX / inc;
            for (int i = 0; i < inc; i++)
            {
                SegWidths.Add(segwidth);
            }
            double load = loadperlength * segwidth;
            List<PointLoad> loads = new List<PointLoad>();

            for (int i = 0; i < inc; i++)
            {
                double x = Start.X + SegWidths[i] * (i + 0.5);
                double y = Slope * x + Int;
                PointLoad pl = new PointLoad(new ShapePoint(x, y), load);
                loads.Add(pl);
            }

            this.Loads = loads;
        }

        public void DrawFcP(double h)
        {
            ForcePolygon fp = new ForcePolygon(this, h);
            this.ForceP = fp;
        }

        public void DrawFmD(double h)
        {
            FormDiagram fd = new FormDiagram(this, h);
            this.FormD = fd;
        }
    }
}
