using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using StructureEngine.Analysis;

namespace StructureEngine.Model
{
    public class Structure : BaseDesign
    {
        public Structure()
        {
            this.InitializeLists(null, null);
        }

        public Structure(List<Node> nodes)
        {
            this.InitializeLists(nodes, null);
        }

        public Structure(List<Node> nodes, List<Member> members)
        {
            this.InitializeLists(nodes, members);
        }

        private void InitializeLists(List<Node> nodes, List<Member> members)
        {
            if (nodes == null)
            {
                this.Nodes = new List<Node>();
            }
            else
            {
                this.Nodes = nodes;
            }

            if (members == null)
            {
                this.Members = new List<Member>();
            }
            else
            {
                this.Members = members;
            }
            

            this._Materials = new List<Material>();
            this._Sections = new List<ISection>();
            this.OrderedEnvNodes = new List<Node>();
            this.LoadCases = new List<LoadCase>();
        }

        //public int LoadCases
        //{
        //    get;
        //    private set;
        //}

        //public void AddLoadCase()
        //{
        //    foreach (Node n in Nodes)
        //    {
        //        foreach (DOF d in n.DOFs)
        //        {
        //            d.Loads.Add(0);
        //        }
        //    }
        //}

        public StructureDim Dimension
        {
            get;
            set;
        }

        public override List<IVariable> DesignVariables
        {
            get
            {
                List<IVariable> dv = new List<IVariable>();
                foreach (DOF d in this.DOFs)
                {
                    if (d is IVariable)
                    {
                        dv.Add((IVariable)d);
                    }
                }
                return dv;
            }
        }

        private List<DOF> AffectedDOFs
        {
            get
            {
                var ad = new List<DOF>();
                foreach (Node n in this.Nodes)
                {
                    for (int i = 0; i < n.DOFs.Length; i++)
                    {
                        CoordVar v = n.DOFs[i] as CoordVar;
                        // Skip variables without relations
                        if (v == null) continue;
                        // Add the source DOF
                        ad.Add(n.DOFs[i]);
                        // Add the same DOF in all listener Nodes
                        foreach (ParametricRelation r in v.Relations)
                        {
                            foreach (Node l in r.Listeners)
                            {
                                ad.Add(l.DOFs[i]);
                            }
                        }
                    }
                }
                return ad.Distinct<DOF>().ToList<DOF>();
            }
        }

        private List<ComputedMember> _AffectedMembers;
        private List<ComputedMember> _UnaffectedMembers;
        protected List<ComputedMember> AffectedMembers
        {
            get
            {
                if (_AffectedMembers == null)
                {
                    DistinguishMembers();
                }
                return _AffectedMembers;
            }
        }
        protected List<ComputedMember> UnaffectedMembers
        {
            get
            {
                if (_UnaffectedMembers == null)
                {
                    DistinguishMembers();
                }
                return _UnaffectedMembers;
            }
        }
        private void DistinguishMembers()
        {
            _AffectedMembers = new List<ComputedMember>();
            _UnaffectedMembers = new List<ComputedMember>();

            foreach (ComputedMember m in this.Members)
            {
                if (this.AffectedDOFs.Contains(m.LocalDOFs[0]) || this.AffectedDOFs.Contains(m.LocalDOFs[1]) ||
                    this.AffectedDOFs.Contains(m.LocalDOFs[2]) || this.AffectedDOFs.Contains(m.LocalDOFs[3]))
                {
                    _AffectedMembers.Add(m);
                }
                else
                {
                    _UnaffectedMembers.Add(m);
                }
            }
        }

        public List<Node> Nodes
        {
            get;
            set;
        }

        public List<Node> OrderedEnvNodes
        {
            get;
            set;
        }

        public List<Member> Members
        {
            get;
            set;
        }
        public virtual Member GetNewMember(Node i, Node j)
        {
            return new Member(i, j);
        }

        public List<LoadCase> LoadCases
        {
            get;
            set;
        }

        private List<Material> _Materials;
        public List<Material> Materials
        {
            get
            {
                foreach (Member m in this.Members)
                {
                    if (!_Materials.Contains(m.Material))
                    {
                        _Materials.Add(m.Material);
                    }
                }
                return _Materials;
            }
            set
            {
                _Materials = value;
            }
        }

        private List<ISection> _Sections;
        public List<ISection> Sections
        {
            get
            {
                // Default section type is round tube
                ISection defSect = new RoundTubeSection();
                foreach (Member m in this.Members)
                {
                    if (m.SectionType == null)
                    {
                        m.SectionType = defSect;
                    }

                    if (!_Sections.Contains(m.SectionType))
                    {
                        _Sections.Add(m.SectionType);
                    }
                }
                return _Sections;
            }
            set
            {
                _Sections = value;
            }
        }

        private double?[] _SymmetryLine;
        public double?[] SymmetryLine
        {
            get
            {
                if (_SymmetryLine == null)
                {
                    _SymmetryLine = new double?[] { null, null };
                }
                return _SymmetryLine;
            }
            set
            {
                _SymmetryLine = value;
            }
        }

        public List<DOF> DOFs
        {
            get
            {
                List<DOF> d = new List<DOF>();
                for (int i = 0; i < Nodes.Count; i++)
                {
                    d.AddRange(Nodes[i].DOFs);
                }
                d = d.OrderBy(i => i.Pinned).ToList();
                for (int i = 0; i < d.Count; i++)
                {
                    d[i].Index = i;
                }
                return d;
            }
        }

        public double[] ZeroPoint
        {
            get
            {
                double minx = Double.MaxValue, miny = Double.MaxValue;
                foreach (Node n in Nodes)
                {
                    minx = Math.Min(minx, n.DOFs[0].Coord);
                    miny = Math.Min(miny, n.DOFs[1].Coord);
                }
                if (Nodes.Count == 0)
                {
                    minx = 0;
                    miny = 0;
                }
                return new double[] { minx, miny };
            }
        }

        public double[] Dimensions
        {
            get
            {
                if (Nodes.Count > 0)
                {
                    double maxx = Double.MinValue, maxy = Double.MinValue;
                    foreach (Node n in Nodes)
                    {
                        maxx = Math.Max(maxx, n.DOFs[0].Coord);
                        maxy = Math.Max(maxy, n.DOFs[1].Coord);
                    }

                    double[] zero = ZeroPoint;
                    double[] dim = new double[Nodes[0].DOFs.Length];
                    dim[0] = maxx - zero[0];
                    dim[1] = maxy - zero[1];
                    return dim;
                }
                else { return new double[] { 0, 0 }; }
            }
        }

        private int dimension
        {
            get
            {
                return this.Nodes[0].DOFs.Length;
            }
        }

        //public Vector<double> Load
        //{
        //    get
        //    {
        //        //List<Vector<double>> l = new List<Vector<double>>();
        //        //for (int i = 0; i < LoadCases; i++)
        //        //{
        //        //    Vector<double> P = new DenseVector(DOFs.Count);
        //        //    for (int j = 0; j < DOFs.Count; j++)
        //        //    {
        //        //        P[j] = DOFs[j].Loads[i];
        //        //    }

        //        //    l.Add(P);
        //        //}
        //        //return l;

        //        Vector<double> P = new DenseVector(DOFs.Count);
        //        for (int j = 0; j < DOFs.Count; j++)
        //        {
        //            P[j] = DOFs[j].Load;
        //        }

        //        return P;
        //    }
        //}


        public StructureType StructType
        {
            get;
            set;
        }

        public enum StructureType
        {
            Truss,
            Frame
        }

        public void ReorderIndices()
        {
            List<DOF> d = this.DOFs;
            for (int i = 0; i < this.Nodes.Count; i++)
            {
                this.Nodes[i].Index = i;
            }
        }

        public double? BaseJitter;
        public double? BaseArea;

        public Structure DesignClone()
        {
            return CloneImpl();
        }
        protected virtual Structure CloneImpl()
        {
            Structure s = new Structure();
            this.CopyTo(s);
            return s;
        }
        internal virtual void CopyTo(Structure s)
        {
            // create copy of materials
            List<Material> newMaterials = new List<Material>();
            IDictionary<Material, Material> matMap = new Dictionary<Material, Material>();
            foreach (Material mat in Materials)
            {
                Material newMaterial = mat.MaterialClone();
                matMap.Add(mat, newMaterial);
                newMaterials.Add(newMaterial);
            }

            // create copy of sections
            List<ISection> newSections = new List<ISection>();
            IDictionary<ISection, ISection> secMap = new Dictionary<ISection, ISection>();
            foreach (ISection sec in Sections)
            {
                ISection newSection = sec.SectionClone();
                secMap.Add(sec, newSection);
                newSections.Add(newSection);
            }

            // create copy of nodes
            List<Node> newNodes = new List<Node>();
            IDictionary<int, Node> orderMap = new Dictionary<int, Node>();
            List<Node> newOENodes = new List<Node>();
            IDictionary<Node, Node> nodeMap = new Dictionary<Node, Node>();
            foreach (Node nd in Nodes)
            {
                Node newNode = new Node(new DOF[] { new DOF(0), new DOF(0) }); // this is a dummy node
                nd.CopyTo(newNode); // add node properties besides parametric relationships
                nodeMap.Add(nd, newNode);
                newNodes.Add(newNode);
                if (this.OrderedEnvNodes.Contains(nd))
                {
                    orderMap.Add(new KeyValuePair<int, Node>(OrderedEnvNodes.IndexOf(nd), nd));
                }
            }

            for (int i = 0; i < orderMap.Count; i++)
            {
                newOENodes.Add(orderMap[i]);
            }

            // copy parametric relationships for nodes
            foreach (Node nd in Nodes)
            {
                for (int j = 0; j < nd.DOFs.Length; j++)
                {
                    CoordVar d = nd.DOFs[j] as CoordVar;
                    if (d == null) continue;
                    foreach (ParametricRelation p in d.Relations)
                    {
                        List<Node> newListeners = new List<Node>();
                        foreach (Node listener in p.Listeners)
                        {
                            newListeners.Add(nodeMap[listener]);
                        }

                        object param = new Double();
                        if (p.Parameter as Node != null)
                        {
                            param = nodeMap[(Node)p.Parameter];
                        }
                        else
                        {
                            param = p.Parameter;
                        }
                        ParametricRelation newp = new ParametricRelation(newListeners, p.Relation, param);
                        CoordVar cv = nodeMap[nd].DOFs[j] as CoordVar;
                        if (cv != null)
                        {
                            cv.Relations.Add(newp);
                        }
                    }
                }
            }

            // create copy of loads
            IDictionary<LoadCase, LoadCase> loadcaseMap = new Dictionary<LoadCase, LoadCase>();
            List<LoadCase> newLCs = new List<LoadCase>();
            foreach (LoadCase lc in LoadCases)
            {
                LoadCase newLC = new LoadCase(lc.Name);
                foreach (Load l in lc.Loads)
                {
                    DOF origDOF = l.myDOF;
                    Node origNode = Nodes.Where(n => n.DOFs.Contains(origDOF)).ToList()[0];
                    Node copyNode = nodeMap[origNode];
                    DOF copyDOF = copyNode.DOFs[origNode.DOFs.ToList().IndexOf(origDOF)];
                    Load newL = new Load(l.Value, newLC, copyDOF);
                    newLC.Loads.Add(newL);
                }
                newLCs.Add(newLC);
                loadcaseMap.Add(lc, newLC);
            }

            // create copy of members
            IDictionary<Member, Member> memberMap = new Dictionary<Member, Member>();
            List<Member> newMembers = new List<Member>();
            foreach (Member md in Members)
            {
                Member newMember = s.GetNewMember(nodeMap[md.NodeI], nodeMap[md.NodeJ]);
                md.CopyTo(newMember);

                newMember.Material = matMap[md.Material];
                newMember.SectionType = secMap[md.SectionType];
                newMembers.Add(newMember);

                memberMap.Add(md, newMember);
            }

            // map members to load cases if computed
            if (this as ComputedStructure != null)
            {
                ComputedStructure cs = (ComputedStructure)this;
                foreach (ComputedMember cm in cs.Members)
                {
                    ComputedMember newMember = (ComputedMember)memberMap[cm];

                    foreach (KeyValuePair<LoadCase, double> f in cm.AxialForce)
                    {
                        LoadCase oldLC = f.Key;
                        LoadCase newLC = loadcaseMap[oldLC];
                        newMember.AxialForce.Add(newLC, f.Value);
                    }
                }
            }

            s.LoadCases.AddRange(newLCs);
            s.Nodes.AddRange(newNodes);
            s.OrderedEnvNodes.AddRange(newOENodes);
            s.Members.AddRange(newMembers);
            s.SymmetryLine = this.SymmetryLine;
            s.PredictedScore = this.PredictedScore;
            s.StructType = this.StructType;
            s.BaseArea = this.BaseArea;
            s.BaseJitter = this.BaseJitter;
        }

        public bool[] CheckReady(IAnalysis AnalysisEngine)
        {
            bool[] check = new bool[3];

            check[0] = true;
            check[1] = false;
            check[2] = false;

            if (this.GetStable() == StabType.Unstable)
            {
                check[0] = false;
            }

            else
            {
                try
                {
                    //TrussAnalysis ta = new TrussAnalysis();
                    ComputedStructure comp = new ComputedStructure(this);
                    //ta.Analyze(comp);
                    if (comp.DetK <= 0)
                    {
                        check[0] = false;
                    }
                }
                catch
                {
                    check[0] = false;
                }
            }

            foreach (LoadCase lc in this.LoadCases)
            {
                foreach (Load l in lc.Loads)
                {
                    if (l.Value != 0)
                    {
                        check[1] = true;
                        break;
                    }
                }
                if (check[1])
                {
                    break;
                }
            }

            foreach (DOF d in this.DOFs)
            {
                if (d is CoordVar)
                {
                    check[2] = true;
                    if (((CoordVar)d).AllowableVariation == 0)
                    {
                        check[2] = false;
                    }
                }
            }

            foreach (LoadCase lc in this.LoadCases)
            {
                foreach (Load l in lc.Loads)
                {
                    if (l.Value != 0)
                    {
                        check[1] = true;
                        break;
                    }
                }
            }

            return check;
        }

        public StabType GetStable()
        {
            int DOFs_pinned = DOFs.Count(d => d.Pinned);

            if (Members.Count == 2 * Nodes.Count - DOFs_pinned)
            {
                return StabType.Determinate;
            }
            else if (Members.Count > 2 * Nodes.Count - DOFs_pinned)
            {
                return StabType.Indeterminate;
            }
            else
            {
                return StabType.Unstable;
            }
        }

        public enum StabType
        {
            Indeterminate,
            Determinate,
            Unstable
        }

        public bool IsSame(IDesign that)
        {
            return Math.Abs(this.GetDistance(that)) < 0.00001;
        }

        public void EnforceRelationships()
        {
            foreach (Node n in this.Nodes)
            {
                n.EnforceRelationships(this);
            }
        }

        public void DetermineEnvelope()
        {
            if (this.StructType != StructureType.Frame)
            {
                return;
            }

            // build a map of nodes to members
            var nodeMemberMap = new Dictionary<Node, ICollection<Member>>();
            foreach (Node n in this.Nodes)
            {
                nodeMemberMap[n] = new List<Member>();
            }
            foreach (Member m in this.Members)
            {
                nodeMemberMap[m.NodeI].Add(m);
                nodeMemberMap[m.NodeJ].Add(m);

                m.Envelope = false; // reset all members
            }

            // find all connected subgraphs (BFS)
            var graphs = new List<List<Node>>();
            var found = new HashSet<Node>();
            foreach (Node n in this.Nodes)
            {
                if (found.Contains(n)) continue;
                var graph = new List<Node>();
                var queue = new Queue<Node>();
                queue.Enqueue(n);
                while (queue.Count > 0)
                {
                    Node c = queue.Dequeue();
                    if (found.Contains(c)) continue;
                    found.Add(c);
                    graph.Add(c);
                    foreach (Member m in nodeMemberMap[c])
                    {
                        queue.Enqueue(c == m.NodeI ? m.NodeJ : m.NodeI);
                    }
                }
                graphs.Add(graph);
                //System.Diagnostics.Debug.WriteLine("SUBGRAPH: nodes " + graph.Count);
            }

            // find envelope for each subgraph
            foreach (var graph in graphs)
            {
                // find an outer node (start with top-most, right-most)
                Node top = null;
                foreach (Node n in graph)
                {
                    if (top == null || top.Y < n.Y || (top.Y == n.Y && top.X < n.X))
                    {
                        top = n;
                    }
                }

                // find envelope, starting at one node and traveling counter-clockwise
                Node node = top;
                var edges = new List<Member>();
                var tried = new HashSet<Member>();
                while (true)
                {
                    // find angle from this node to last node (whence we came)
                    double diffAngle = 2 * Math.PI;
                    double lastAngle = 0; // default to 0 radians; starting at the top node, this means go left
                    if (edges.Count > 0) // if we have a previous edge, start with its angle
                    {
                        var lastEdge = edges[edges.Count - 1];
                        var lastNode = node == lastEdge.NodeI ? lastEdge.NodeJ : lastEdge.NodeI;
                        lastAngle = Angle(node, lastNode); // note: angle FROM current node, BACK to last node
                    }

                    // find edge with minimum angle greater than last angle (radians increase counter-clockwise)
                    Member nextEdge = null;
                    Node nextNode = null;
                    //System.Diagnostics.Debug.WriteLine("CONSIDER: node " + node.Index);
                    foreach (Member m in nodeMemberMap[node])
                    {
                        if (tried.Contains(m)) continue; // prevent cycles
                        double a = node == m.NodeI ? Angle(m.NodeI, m.NodeJ) : Angle(m.NodeJ, m.NodeI);
                        double adiff = (2 * Math.PI + a - lastAngle) % (2 * Math.PI);

                        //System.Diagnostics.Debug.WriteLine("CONSIDER: angle " + a + " adiff " + adiff + " edge.I " + m.NodeI.Index + " edge.J " + m.NodeJ.Index);
                        if (adiff < diffAngle)
                        {
                            nextEdge = m;
                            nextNode = node == m.NodeI ? m.NodeJ : m.NodeI;
                            diffAngle = adiff;
                        }
                    }

                    // act on the next node/edge
                    if (nextNode == top)
                    {
                        // made it back to the start node, envelope complete!
                        edges.Add(nextEdge);
                        foreach (Member m in edges)
                        {
                            m.Envelope = true;
                        }
                        //System.Diagnostics.Debug.WriteLine("DONE: envelope edges " + edges.Count);
                        break;
                    }
                    else if (nextNode == null)
                    {
                        // dead end, back up or stop if we ran out of edges
                        if (edges.Count == 0) break;
                        var prevEdge = edges[edges.Count - 1];
                        var prevNode = node == prevEdge.NodeI ? prevEdge.NodeJ : prevEdge.NodeI;
                        //System.Diagnostics.Debug.WriteLine("BACK UP: dead node " + node.Index + " edge.I " + prevEdge.NodeI.Index + " edge.J " + prevEdge.NodeJ.Index);
                        // consider isolated lines to be part of the envelope
                        prevEdge.Envelope = true;
                        node = prevNode;
                        edges.RemoveAt(edges.Count - 1);
                    }
                    else
                    {
                        // march on to the next node, keeping track of edges we've tried
                        //System.Diagnostics.Debug.WriteLine("PICKING: next node " + nextNode.Index + " edge.I " + nextEdge.NodeI.Index + " edge.J " + nextEdge.NodeJ.Index);
                        node = nextNode;
                        edges.Add(nextEdge);
                        tried.Add(nextEdge);
                    }
                }
            }
        }

        private double Angle(Node start, Node end)
        {
            double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
            if (angle < 0) angle += 2 * Math.PI;
            return angle;
        }
                
        #region old code

        //public void ApplyMirror(Node n, bool vert) // true means vertical plane, false means horizontal plane
        //{
        //    SymmetryLine[0] = n.DOFs[0].Coord;
        //    SymmetryLine[1] = n.DOFs[1].Coord;

        //    foreach (Node nd in this.Nodes)
        //    {
        //        if (vert)
        //        {
        //            Node newnode = new Node(new DOF[2]);
        //            newnode.Master[1] = nd;
        //            newnode.Mirror[0] = nd;
        //            this.Nodes.Add(newnode);
        //        }
        //        else
        //        {
        //            Node newnode = new Node(new DOF[2]);
        //            newnode.Master[0] = nd;
        //            newnode.Mirror[1] = nd;
        //            this.Nodes.Add(newnode);
        //        }
        //    }

        //    this.MasterMirror();
        //}
        //public void MasterMirror()
        //{
        //    foreach (Node n in this.Nodes)
        //    {
        //        for (int k = 0; k < dimension; k++)
        //        {
        //            // Master
        //            if (n.Master[k] != null)
        //            {
        //                n.DOFs[k].Coord = n.Master[k].DOFs[k].Coord;
        //            }

        //            // Mirror
        //            if (n.Mirror[k] != null)
        //            {
        //                if (this.SymmetryLine[k] != null)
        //                {
        //                    double slavecoord = n.DOFs[k].Coord;
        //                    double mastercoord = n.Mirror[k].DOFs[k].Coord;

        //                    //if (slavecoord != 2 * (double)this.SymmetryLine[k] - mastercoord) // if mirrored nodes aren't co-located
        //                    //{
        //                        n.DOFs[k].Coord = 2 * (double)this.SymmetryLine[k] - n.Mirror[k].DOFs[k].Coord;
        //                    //}
        //                    //else
        //                    //{
        //                    //    n.Mirror[k].DOFs[k].Coord
        //                    //}
        //                }
        //            }
        //        }
        //    }
        #endregion
    }
}
