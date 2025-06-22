using System.Collections.Generic;
using StructureEngine.Grammar;

namespace StructureEngine.GraphicStatics
{
    public class FormDiagram
    {
        public FormDiagram(ProblemSetup ps, double h)
        {
            this.ProbSet = ps;
            this.Segments = new List<ShapeLine>();
            this.GenerateFD(h);
        }

        public ProblemSetup ProbSet;

        public List<ShapeLine> Segments;

        private void GenerateFD(double h)
        {
            if (ProbSet.ForceP != null)
            {
                var segs = new List<ShapeLine>();
                int count = ProbSet.ForceP.Rays.Count;
                
                // segments
                var lp = new List<ShapePoint>();
                var ll = new List<ShapeLine>();

                lp.Add(ProbSet.Start.Clone());
                double x = ProbSet.Start.X;
                for (int i = 0; i < count - 1; i++)
                {
                    ShapeLine ray = ProbSet.ForceP.Rays[i];
                    double[] si = ray.SlopeIntercept();
                    x += ProbSet.SegWidths[i];
                    double deltax = x - lp[i].X;
                    double y = si[0] * deltax + lp[i].Y;
                    ShapePoint p = new ShapePoint(x, y);
                    lp.Add(p);
                }

                ShapeLine ray1 = ProbSet.ForceP.Rays[count - 1];
                double[] si1 = ray1.SlopeIntercept();
                double x1 = ProbSet.End.X;
                double deltax1 = x1 - lp[lp.Count - 1].X;
                double y1 = si1[0] * deltax1 + lp[lp.Count - 1].Y;
                ShapePoint p1 = new ShapePoint(x1, y1);
                lp.Add(p1);

                //if (!lp[lp.Count - 1].IsSame(ProbSet.End))
                //{
                //    throw new Exception("Form Diagram does not close");
                //}

                //lp.Add(ProbSet.End.Clone());

                for (int i = 0; i < lp.Count - 1; i++)
                {
                    ShapeLine l = new ShapeLine(lp[i], lp[i + 1]);
                    if (h > 0) // tension element
                    {
                        l.AxialForce = ProbSet.ForceP.Rays[i].Length;
                    }
                    else // compression element
                    {
                        l.AxialForce = -ProbSet.ForceP.Rays[i].Length;
                    }
                    ll.Add(l);
                }

                this.Segments.AddRange(ll);
            }
        }
    }
}
