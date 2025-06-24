using System;
using System.Collections.Generic;
using System.Linq;
using StructureEngine.Model;

namespace StructureEngine.Analysis
{
    public class EquivFrameAnalysis : BaseTrussAnalysis, IAnalysis
    {

        public double Analyze(Model.IDesign d)
        {
            ComputedStructure comp = (ComputedStructure)d;
            try
            {
                if (!comp.Analyzed)
                {
                    this.RunAnalysis(comp);
                    comp.Analyzed = true;
                }

                return this.GetCompositeScore(comp);
            }
            catch
            {
                return Double.NaN;
            }
        }

        private double GetCompositeScore(ComputedStructure s)
        {
            double area = this.GetPolygonArea(s);
            double stress = this.GetMaxStress(s);
            double jitter = this.GetJitter(s);

            if (s.BaseJitter != null)
            {
                jitter =  jitter / (double)s.BaseJitter;
            }
            if (s.BaseArea != null)
            {
                area = area / (double)s.BaseArea;
            }

            //return jitter + 5*area + Math.Pow(10, 8) * Math.Pow(stress, 5);
            return area + Math.Pow(10, 8) * Math.Pow(stress, 5);
            //return Math.Pow(10, 8) * Math.Pow(stress, 5);
            //return area + Math.Pow(10,8) * Math.Pow(stress, 5);
        }

        private double GetPolygonArea(ComputedStructure s)
        {
            if (s.OrderedEnvNodes.Count == 0)
            {
                s = this.OrderEnvNodes(s);
            }

            // compute enclosed area of ordered list of nodes
            int nodeCount = s.OrderedEnvNodes.Count;
            double A = 0;
            for (int i = 0; i < nodeCount - 1; i++)
            {
                // xi * yi+1 - xi+1*yi

                Node nodei = s.OrderedEnvNodes[i];
                Node nodei1 = s.OrderedEnvNodes[i + 1];
                double xi = nodei.DOFs[0].Coord;
                double yi = nodei.DOFs[1].Coord;
                double xi1 = nodei1.DOFs[0].Coord;
                double yi1 = nodei1.DOFs[1].Coord;

                A += (xi * yi1 - xi1 * yi);
            }

            A = 0.5 * Math.Abs(A);

            return A;
        }

        private double GetMaxStress(ComputedStructure s)
        {
            double maxStress = Double.MinValue;
            foreach (ComputedMember m in s.Members.Where(m => m.Envelope))
            {
                maxStress = Math.Max(maxStress, Math.Abs((m.MaxAxialForce / m.Area)) / m.Material.StressAllow);
            }

            if (maxStress > 1)
            {
                return maxStress;
            }
            else
            {
                return 0;
            }
        }

        private List<ComputedMember> GetMembers(ComputedStructure s, Node n)
        {
            var members = new List<ComputedMember>();

            foreach (ComputedMember m in s.Members)
            {
                if ((m.NodeI == n || m.NodeJ == n) && m.Envelope)
                {
                    members.Add(m);
                }
            }

            return members;
        }

        public void SetMemberAreas(ComputedStructure s)
        {
            bool isStressZero = false;
            //ComputedStructure copy = (ComputedStructure)s.DesignClone();

            s.BaseArea = this.GetPolygonArea(s);
            s.BaseJitter = this.GetJitter(s);

            while (!isStressZero)
            {
                // clear Stiffness Matrices
                s.ClearStiffness();

                this.RunAnalysis(s);
                double stress = this.GetMaxStress(s);
                if (stress == 0)
                {
                    isStressZero = true;
                    break;
                }

                //foreach (ComputedMember m in copy.Members.Where(m => m.Envelope))
                //{
                //    m.Area = m.Area * 2;
                //}

                double ratio = Double.MaxValue;
                foreach (ComputedMember m in s.Members.Where(m => m.Envelope))
                {
                    if (Math.Abs(m.Material.StressAllow / (m.MaxAxialForce / m.Area)) < ratio)
                    {
                        ratio = Math.Abs(m.Material.StressAllow / (m.MaxAxialForce / m.Area));
                    }
                }
                foreach (ComputedMember m in s.Members.Where(m => m.Envelope))
                {
                    m.Area = m.Area / ratio * 1.1;
                }
            }

            //return copy;
        }

        public ComputedStructure OrderEnvNodes(ComputedStructure s)
        {
            ComputedStructure copy = (ComputedStructure)s.DesignClone();

            // identify envelope nodes
            var envNodes = new List<Node>();
            foreach (ComputedMember m in copy.Members)
            {
                if (m.Envelope)
                {
                    envNodes.Add(m.NodeI);
                    envNodes.Add(m.NodeJ);
                }
            }
            envNodes = (from n in envNodes select n).Distinct().ToList();

            // order envelope nodes
            var envOrdered = new List<Node>();
            Node current = envNodes[0];
            bool foundNext = true;

            while (envNodes.Count > 0)
            {
                if (!foundNext)
                {
                    throw new Exception("Error with node ordering.");
                }
                envOrdered.Add(current);
                envNodes.Remove(current);
                var mems = this.GetMembers(copy, current);
                int i = 0;
                foundNext = false;

                while (i < mems.Count)
                {
                    ComputedMember mem = mems[i];

                    if (mem.NodeI == current && envNodes.Contains(mem.NodeJ))
                    {
                        current = mem.NodeJ;
                        foundNext = true;
                        break;
                    }
                    else if (mem.NodeJ == current && envNodes.Contains(mem.NodeI))
                    {
                        current = mem.NodeI;
                        foundNext = true;
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            copy.OrderedEnvNodes.Clear();
            copy.OrderedEnvNodes.AddRange(envOrdered);

            return copy;
        }

        private double GetJitter(ComputedStructure s)
        {
            if (s.OrderedEnvNodes.Count == 0)
            {
                s = this.OrderEnvNodes(s);
            }

            List<Node> VarEnvNodes = new List<Node>();
            foreach (Node n in s.OrderedEnvNodes)
            {
                if (n.DOFs[0] is IVariable || n.DOFs[1] is IVariable)
                {
                    VarEnvNodes.Add(n);
                }
            }

            double j = 0;

            foreach (Node n in VarEnvNodes)
            {
                var contigMems = this.GetMembers(s, n);
                contigMems = contigMems.Where(m => m.Envelope).ToList();
                if (contigMems.Count != 2) { throw new Exception("Node must be connected to two envelope members."); }
                if ((VarEnvNodes.Contains(contigMems[0].NodeI) && VarEnvNodes.Contains(contigMems[0].NodeJ)) && 
                    (VarEnvNodes.Contains(contigMems[1].NodeI) && VarEnvNodes.Contains(contigMems[1].NodeJ)))
                {
                    double jitter = (Math.PI - this.GetAngleBetween(contigMems[0], contigMems[1])) / Math.PI;
                    //if (jitter == 1) { jitter = 0; }
                    j += Math.Pow(jitter,1);
                }
            }

            return Math.Pow(j,2);
        }

        private double GetAngleBetween(Member m1, Member m2)
        {
            double dx1 = m1.NodeJ.DOFs[0].Coord - m1.NodeI.DOFs[0].Coord;
            double dy1 = m1.NodeJ.DOFs[1].Coord - m1.NodeI.DOFs[1].Coord;
            double dx2 = m2.NodeJ.DOFs[0].Coord - m2.NodeI.DOFs[0].Coord;
            double dy2 = m2.NodeJ.DOFs[1].Coord - m2.NodeI.DOFs[1].Coord;

            double cross = Math.Abs(dx1 * dy2 - dx2 * dy1);
            double dot = dx1 * dx2 + dy1 * dy2;

            double angle = Math.Atan2(cross, dot);

            if (m1.NodeI == m2.NodeI || m1.NodeJ == m2.NodeJ)
            {
                return angle;
            }
            else if (m1.NodeI == m2.NodeJ || m1.NodeJ == m2.NodeI)
            {
                return Math.PI - angle;
            }
            else
            {
                throw new Exception("Members must share a node.");
            }
        }
    }
}
