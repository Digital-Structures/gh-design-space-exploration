
using System.Collections.Generic;
using System;
namespace StructureEngine.Model
{
    public class Node
        // Base class for the definition of structural nodes
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

        public int Index
        {
            get;
            set;
        }

        public double X { get { return DOFs[0].Coord; } }
        public double Y { get { return DOFs[1].Coord; } }
        public double Z { get { return DOFs[2].Coord;  } }

       /* public void CopyTo(Node newnode)
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
                foreach (ParametricRelation p in d.Relations)
                {
                    // enforce master relationships
                    if (p.Relation == RelationType.Master)
                    {
                        foreach (Node lis in p.Listeners)
                        {
                            if (i == 0)
                            {
                                lis.DOFs[0].Coord = DOFs[0].Coord;
                            }
                            else
                            {
                                lis.DOFs[1].Coord = DOFs[1].Coord;
                            }
                        }
                    }

                    // enforce mirror relationships
                    else if (p.Relation == RelationType.Mirror)
                    {
                        foreach (Node lis in p.Listeners)
                        {
                            if (i == 0)
                            {
                                lis.DOFs[0].Coord = 2 * (double)s.SymmetryLine[i] - DOFs[0].Coord;
                            }
                            else
                            {
                                lis.DOFs[1].Coord = 2 * (double)s.SymmetryLine[i] - DOFs[1].Coord;
                            }
                        }
                    }

                    // enforce offset relationships
                    else if (p.Relation == RelationType.Offset)
                    {
                        foreach (Node lis in p.Listeners)
                        {
                            if (i == 0)
                            {
                                lis.DOFs[0].Coord = DOFs[0].Coord + (Double)p.Parameter;
                            }
                            else
                            {
                                lis.DOFs[1].Coord = DOFs[1].Coord + (Double)p.Parameter;
                            }
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

                            if (i == 0)
                            {
                                lis.DOFs[0].Coord = this.DOFs[0].Coord * (1 - ratio) + other.DOFs[0].Coord * (ratio);
                            }
                            else
                            {
                                lis.DOFs[1].Coord = this.DOFs[1].Coord * (1 - ratio) + other.DOFs[1].Coord * (ratio);
                            }
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
                sum += Math.Pow(this.DOFs[i].Value - that.DOFs[i].Value, 2);
            }

            return Math.Sqrt(sum);
        }
    }*/
}
