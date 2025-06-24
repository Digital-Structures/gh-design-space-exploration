using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using System.Collections.Generic;
using System.Linq;

namespace StructureEngine.Model
{
    public class ComputedMember : Member
    {
        public ComputedMember(Node i, Node j) : base(i, j)
        {
            this.AxialForce = new Dictionary<LoadCase, double>();
        }

        public Dictionary<LoadCase, double> AxialForce;

        public double MaxAxialForce
        {
            get
            {
                return Math.Max(Math.Abs(this.AxialForce.Values.Max()), Math.Abs(this.AxialForce.Values.Min()));
            }
        }

        public double ReqArea
        {
            get
            {
                return this.SectionType.GetReqEnvArea(this.AxialForce, this.Material.StressAllow, this.Material.E, this.Length);
            }
        }

        public double GetLCArea(LoadCase lc)
        {
            return this.SectionType.GetReqArea(this.AxialForce[lc], this.Material.StressAllow, this.Material.E, this.Length);
        }

        public double Weight
        {
            get
            {
                return this.ReqArea * this.Length * this.Material.Density;
            }
        }

        public Matrix<double> CalculateStiffness()
        {
            Matrix<double> k = new DenseMatrix(4, 4);

            // Element Stiffness Matrix k:
            // [ (cos(theta))^2  sin(theta)*cos(theta)  -(cos(theta))^2  -sin(theta)*cos(theta) ]
            // [ sin(theta)*cos(theta)  (sin(theta))^2  -sin(theta)*cos(theta)  -(sin(theta))^2 ]
            // [ -(cos(theta))^2  -sin(theta)*cos(theta)  (cos(theta))^2  sin(theta)*cos(theta) ]
            // [ -sin(theta)*cos(theta)  -(sin(theta))^2  sin(theta)*cos(theta)  (sin(theta))^2 ]

            k[0, 0] = Math.Pow(Math.Cos(this.Angle), 2);
            k[0, 1] = Math.Sin(this.Angle) * Math.Cos(this.Angle);
            k[1, 1] = Math.Pow(Math.Sin(this.Angle), 2);
            k[1, 0] = k[0, 1];

            Matrix<double> submat = k.SubMatrix(0, 2, 0, 2);

            k.SetSubMatrix(2, 2, 2, 2, submat);
            k.SetSubMatrix(2, 2, 0, 2, submat.Negate());
            k.SetSubMatrix(0, 2, 2, 2, submat.Negate());

            return k * (this.Material.E * this.Area / this.Length);
        }

        public DOF[] LocalDOFs
        {
            get
            {
                DOF[] d = new DOF[12];
                d[0] = this.NodeI.DOFs[0];
                d[1] = this.NodeI.DOFs[1];
                d[2] = this.NodeJ.DOFs[0];
                d[3] = this.NodeJ.DOFs[1];

                return d;
            }
        }

        new public ComputedMember MemberClone()
        {
            return (ComputedMember)this.CloneImpl();
        }
        protected override Member CloneImpl()
        {
            var cm = new ComputedMember(this.NodeI, this.NodeJ);
            this.CopyTo(cm);
            return cm;
        }
        internal override void CopyTo(Member m)
        {
            base.CopyTo(m);
        }
    }

    public enum StructureDim
    {
        TwoD,
        ThreeD
    }
    public enum MemberType
    {
        Truss,
        Frame
    }

}
