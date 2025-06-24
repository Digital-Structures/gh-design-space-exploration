using System;

namespace StructureEngine.Model
{
    public class Member
    {
        public Member(Node i, Node j)
        {
            NodeI = i;
            NodeJ = j;
        }

        public bool Envelope
        {
            get;
            set;
        }

        public Node NodeI
        {
            get;
            set;
        }

        public Node NodeJ
        {
            get;
            set;
        }

        public ISection SectionType
        {
            get;
            set;
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(Math.Pow(NodeI.DOFs[0].Coord - NodeJ.DOFs[0].Coord, 2)
                    + Math.Pow(NodeI.DOFs[1].Coord - NodeJ.DOFs[1].Coord, 2));
            }
        }

        public double Angle
        {
            get
            {
                double deltaX = NodeJ.DOFs[0].Coord - NodeI.DOFs[0].Coord;
                double deltaY = NodeJ.DOFs[1].Coord - NodeI.DOFs[1].Coord;
                double a = Math.Atan(deltaY / deltaX);
                double angle;
                if (deltaX < 0)
                {
                    angle = Math.PI + a;
                }
                else
                {
                    angle = a;
                }

                return angle;
            }
        }

        public double Area
        {
            get;
            set;
        }

        public double MomentofInertia
        {
            get
            {
                return this.SectionType.GetReqMomInertia(this.Area);
            }
        }

        public Material Material
        {
            get;
            set;
        }

        public Member MemberClone()
        {
            return CloneImpl();
        }
        protected virtual Member CloneImpl()
        {
            Member m = new Member(this.NodeI, this.NodeJ);
            this.CopyTo(m);
            return m;
        }
        internal virtual void CopyTo(Member m)
        {
            // Set envelope
            m.Envelope = this.Envelope;

            // Set Area
            m.Area = this.Area;

        }
    }
}
