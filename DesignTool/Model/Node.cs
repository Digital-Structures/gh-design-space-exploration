
using System.Collections.Generic;
using System;
namespace StructureEngine.Model
{
    public class Node
    {
        public Node(DOF[] dofs)
        {
            this.DOFs = dofs;
        }

        public DOF[] DOFs
        {
            get;
            set;
        }

        public void ConvertDOFtoVar(int index, double variation)
        {
            DOF d = this.DOFs[index];
            CoordVar cv = new CoordVar(d.Coord, variation);
            d.CopyTo(cv);
            this.DOFs[index] = cv;
        }

        public void RevertVartoDOF(int index)
        {
            DOF d = this.DOFs[index];
            DOF nonvar = new DOF(d.Coord);
            d.CopyTo(nonvar);
            this.DOFs[index] = nonvar;
        }


        public int Index
        {
            get;
            set;
        }

        public double X { get { return DOFs[0].Coord; } }
        public double Y { get { return DOFs[1].Coord; } }
        public double Z { get { return DOFs[2].Coord; } }
        public double RotX { get { return DOFs[3].Coord; } }
        public double RotY { get { return DOFs[4].Coord; } }
        public double RotZ { get { return DOFs[5].Coord; } }

        public void CopyTo(Node newnode)
        {
            // copy properties at DOF level
            for (int i = 0; i < this.DOFs.Length; i++)
            {
                var src = this.DOFs[i];
                var dst = newnode.DOFs[i];
                src.CopyTo(dst);
            }

            // copy Node-level properties
            newnode.Index = this.Index;
        }

        public void EnforceRelationships(Structure s)
        {
            for (int i = 0; i < DOFs.Length; i++)
            {
                DOF d = DOFs[i];
                CoordVar cv = d as CoordVar;
                if (cv == null) continue;
                foreach (ParametricRelation p in cv.Relations)
                {
                    // enforce master relationships
                    if (p.Relation == RelationType.Master)
                    {
                        foreach (Node lis in p.Listeners)
                        {
                            lis.DOFs[i].SetCoord(DOFs[i].Coord);
                        }
                    }

                    // enforce mirror relationships
                    else if (p.Relation == RelationType.Mirror)
                    {
                        foreach (Node lis in p.Listeners)
                        {
                            lis.DOFs[i].SetCoord(2 * (double)s.SymmetryLine[i] - DOFs[i].Coord);
                        }
                    }

                    // enforce offset relationships
                    else if (p.Relation == RelationType.Offset)
                    {
                        foreach (Node lis in p.Listeners)
                        {
                            lis.DOFs[i].SetCoord(DOFs[i].Coord + (Double)p.Parameter);
                        }
                    }

                    // enforce average relationships
                    else if (p.Relation == RelationType.Average)
                    {
                        Node other = (Node)p.Parameter;

                        foreach (Node lis in p.Listeners)
                        {
                            double lisdist = DistanceFrom(lis);
                            double otherdist = DistanceFrom(other);
                            double ratio = lisdist / otherdist;

                            lis.DOFs[i].SetCoord(this.DOFs[i].Coord * (1 - ratio) + other.DOFs[i].Coord * (ratio));
                        }
                    }
                }
            }
        }

        public double DistanceFrom(Node that)
        {
            double sum = 0;
            for (int i = 0; i < this.DOFs.Length; i++)
            {
                sum += Math.Pow(this.DOFs[i].Coord - that.DOFs[i].Coord, 2);
            }

            return Math.Sqrt(sum);
        }
    }
}
