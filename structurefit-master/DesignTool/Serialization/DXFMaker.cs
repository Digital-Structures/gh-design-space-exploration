using System;
using System.Collections.Generic;
using netDxf;
using StructureEngine.Model;
using netDxf.Entities;

namespace StructureEngine.Serialization
{
    public class DXFMaker
    {
        public DXFMaker()
        {
        }

        public DxfDocument WriteDXF(ComputedStructure cs)
        {
            DxfDocument newDXF = new DxfDocument();
            newDXF.AddLayer(new netDxf.Tables.Layer("Centerlines") { Color = new AciColor(255, 0, 0)});
            newDXF.AddLayer(new netDxf.Tables.Layer("Thicknesses") { Color = new AciColor(255, 255, 255)});

            foreach (ComputedMember m in cs.Members)
            {
                Line newLine = new Line(new Vector2(m.NodeI.DOFs[0].Coord, m.NodeI.DOFs[1].Coord),
                    new Vector2(m.NodeJ.DOFs[0].Coord, m.NodeJ.DOFs[1].Coord));
                newLine.Layer = newDXF.Layers[1];
                newDXF.AddEntity(newLine);

                double reqThickness = m.SectionType.GetReqThickness(m.ReqArea);
                IEnumerable<Line> list = Offset(newLine, reqThickness / 2.0);
                foreach (Line l in list)
                {
                    l.Layer = newDXF.Layers[2];
                }
                newDXF.AddEntity(list);
            }

            return newDXF;
        }

        public DxfDocument WriteDXFSpace(IList<ComputedStructure> designs)
        {
            double cutoff = 1.5;
            DxfDocument newDXF = new DxfDocument();
            foreach (ComputedStructure cs in designs)
            {
                double score = cs.Score;
                if (score < cutoff)
                {
                    byte color = (byte)Math.Min(Math.Round((Math.Pow(score,2) / cutoff) * 254), 254);
                    netDxf.Objects.Group group = new netDxf.Objects.Group();
                    foreach (ComputedMember m in cs.Members)
                    {
                        if (m.Envelope)
                        {
                            Line newLine = new Line(new Vector2(m.NodeI.DOFs[0].Coord, m.NodeI.DOFs[1].Coord),
                                        new Vector2(m.NodeJ.DOFs[0].Coord, m.NodeJ.DOFs[1].Coord));
                            newLine.Color = new AciColor(254, color, color);
                            
                            group.Entities.Add(newLine);
                        }
                    }
                    newDXF.AddGroup(group);
                }
            }

            return newDXF;
        }

        private IEnumerable<Line> DrawStructure(ComputedStructure s)
        {
            IList<Line> list = new List<Line>();
            
            foreach (ComputedMember m in s.Members)
            {
                if (s.StructType == Structure.StructureType.Truss || m.Envelope)
                {
                    Line newLine = new Line(new Vector2(m.NodeI.DOFs[0].Coord, m.NodeI.DOFs[1].Coord),
                        new Vector2(m.NodeJ.DOFs[0].Coord, m.NodeJ.DOFs[1].Coord));
                    list.Add(newLine);
                }
            }

            return list;
        }

        public DxfDocument MultiStructureDXF(IEnumerable<IEnumerable<ComputedStructure>> structureContours, double[] vals)
        {
            DxfDocument newDXF = new DxfDocument();
            int i = 0;

            if (vals.Length != ((List<IEnumerable<ComputedStructure>>)structureContours).Count)
            {
                throw new Exception("Number of contours must match number of structure groups.");
            }

            foreach (IEnumerable<ComputedStructure> structures in structureContours)
            {
                double val = vals[i];
                int col = (int)(255 - Math.Round((1.0 / val) * 255));
                newDXF.AddLayer(new netDxf.Tables.Layer("designs-" + val.ToString()) { Color = new AciColor(col, col, col) });

                foreach (ComputedStructure s in structures)
                {
                    //var structLines = this.Offset(this.DrawStructure(s), 0, -1 * s.ZeroPoint[1]);
                    var structLines = this.Offset(this.DrawStructure(s), s.DesignVariables[0].Value * 10, s.DesignVariables[1].Value * 10);
                    foreach (Line l in structLines)
                    {
                        l.Layer = newDXF.Layers[i + 1];
                    }
                    newDXF.AddEntity(structLines);
                }

                i++;
            }

            return newDXF;
        }

        public DxfDocument TableStructureDXF(List<ComputedStructure> designs, int nrows, int ncols)
        {
            DxfDocument newDXF = new DxfDocument();
            double deltax = designs[0].Dimensions[0] * 1.2;
            double deltay = designs[0].Dimensions[1] * 1.2;

            for (int i = 0; i < nrows; i++)
            {
                for (int j = 0; j < ncols; j++)
                {
                    int count = i * ncols + j;
                    IEnumerable<Line> drawing = DrawStructure(designs[count]);
                    drawing = this.Offset(drawing, deltax * j, deltay * i);
                    newDXF.AddEntity(drawing);
                }
            }


            return newDXF;
        }

        private IEnumerable<Line> Offset(Line cl, double offset)
        {
            List<Line> olines = new List<Line>();

            Line l1 = new Line(TransformPoint(new Vector2(cl.StartPoint.X, cl.StartPoint.Y), -1 / GetSlope(cl), offset),
                TransformPoint(new Vector2(cl.EndPoint.X, cl.EndPoint.Y), -1 / GetSlope(cl), offset));
            Line l2 = new Line(TransformPoint(new Vector2(cl.StartPoint.X, cl.StartPoint.Y), -1 / GetSlope(cl), -1 * offset),
                TransformPoint(new Vector2(cl.EndPoint.X, cl.EndPoint.Y), -1 / GetSlope(cl), -1 * offset));

            olines.Add(l1);
            olines.Add(l2);

            return olines;
        }

        private IEnumerable<Line> Offset(IEnumerable<Line> lines, double offsetX, double offsetY)
        {
            var OffsetLines = new List<Line>();
            foreach (Line l in lines)
            {
                Line ol = new Line(new Vector2(l.StartPoint.X + offsetX, l.StartPoint.Y + offsetY), new Vector2(l.EndPoint.X + offsetX, l.EndPoint.Y + offsetY));
                OffsetLines.Add(ol);
            }
            return OffsetLines;
        }

        private double GetSlope(Line l)
        {
            return (l.EndPoint.Y - l.StartPoint.Y) / (l.EndPoint.X - l.StartPoint.X);
        }

        private Vector2 TransformPoint(Vector2 p1, double slope, double distance)
        {
            Vector2 p2 = new Vector2();
            double deltaX, deltaY;
            if (Double.IsInfinity(slope))
            {
                deltaX = 0;
                deltaY = distance;
            }
            else
            {
                deltaX = distance / Math.Sqrt(1 + Math.Pow(slope, 2));
                deltaY = deltaX * slope;
            }
            p2.X = p1.X + deltaX;
            p2.Y = p1.Y + deltaY;
            return p2;
        }
    }
}
