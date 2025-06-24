using System;
using System.Collections.Generic;
using StructureEngine.Grammar;

namespace StructureEngine.GraphicStatics
{
    public class ForcePolygon
    {
        public ForcePolygon(ProblemSetup ps, double h)
        {
            this.ProbSet = ps;
            this.H = h;

            this.LoadLine = new List<ShapeLine>();
            this.Rays = new List<ShapeLine>();
            this.GenerateFP();
        }

        public ProblemSetup ProbSet;

        public double H;

        public List<ShapeLine> LoadLine;

        public List<ShapeLine> Rays;

        public ShapePoint ReactionPoint;

        public ShapePoint Pole;

        private void GenerateFP()
        {
            // draw load line
            var lp = new List<ShapePoint>();
            var ll = new List<ShapeLine>();

            var start = new ShapePoint(0, 0);
            lp.Add(start);

            // points on load line
            for (int i = 1; i < ProbSet.Loads.Count + 1; i++)
            {
                ShapePoint p = new ShapePoint(0, lp[i - 1].Y + ProbSet.Loads[i - 1].Load);
                lp.Add(p);
            }

            // lines on load line
            for (int i = 0; i < lp.Count - 1; i++)
            {
                ShapeLine l = new ShapeLine(lp[i], lp[i + 1]);
                ll.Add(l);
            }

            this.LoadLine = ll;
            ShapePoint end = ll[ll.Count - 1].End;

            // reaction point
            //double reaction = ProbSet.TotalLoad / 2;
            this.ReactionPoint = new ShapePoint(0, end.Y + GetReaction());

            // pole
            this.Pole = new ShapePoint(0 + this.H, ProbSet.Slope * (0 + this.H) + ReactionPoint.Y);

            // draw rays
            Rays.Add(new ShapeLine(LoadLine[0].Start, Pole));
            foreach (ShapeLine l in this.LoadLine)
            {
                Rays.Add(new ShapeLine(l.End, Pole));
            }

        }

        private double GetReaction()
        {
            double moment = 0;
            foreach (PointLoad p in this.ProbSet.Loads)
            {
                double arm = Math.Abs(p.Point.X - ProbSet.Start.X);
                double load = p.Load;
                moment += load * arm;
            }

            double reaction = Math.Abs(moment) / (Math.Abs(ProbSet.End.X - ProbSet.Start.X));
            return reaction;
        }
    }
}
