using System;
using System.Collections.Generic;
using StructureEngine.Analysis;

namespace StructureEngine.Model
{
    public class StructureSetup : ISetup
    {
        public StructureSetup()
        {
            List<IDesign> list = new List<IDesign>();

            list.Add(simpletruss);
            list.Add(cantroof);
            list.Add(chiassoroof);
            list.Add(portalframe);
            list.Add(circulararch);
            list.Add(tower);
            list.Add(bridge);
            list.Add(Test1);
            list.Add(Test2);
            list.Add(airportroof);

            foreach (IDesign d in list)
            {
                ((Structure)d).DetermineEnvelope();
                ((ComputedStructure)d).SetStart();
            }

            Designs = list;

            MyTrussAnalysis = new TrussAnalysis();
            MyFrameAnalysis = new EquivFrameAnalysis();
        }

        public StructureSetup(int probNum)
        {
            List<IDesign> list = new List<IDesign>();
            if (probNum == 1)
            {
                list.Add(Test1);
            }
            else if (probNum == 2)
            {
                list.Add(Test2);
            }
            else if (probNum == 3)
            {
                list.Add(Test3);
            }
            Designs = list;
        }

        private TrussAnalysis MyTrussAnalysis;
        public EquivFrameAnalysis MyFrameAnalysis;

        public List<IDesign> Designs
        {
            get;
            set;
        }

        private ComputedStructure Test1 // 11-bar truss
        {
            get
            {
                List<Node> myNodes = new List<Node>();
                List<Member> myMembers = new List<Member>();

                // define node coordinates
                myNodes.Add(new Node(new DOF[] { new DOF(0), new DOF(0) })); // 0; inches
                myNodes.Add(new Node(new DOF[] { new DOF(30), new DOF(-30) })); // 1
                myNodes.Add(new Node(new DOF[] { new DOF(90), new DOF(-30) })); // 2
                myNodes.Add(new Node(new DOF[] { new DOF(60), new DOF(0) })); // 3
                myNodes.Add(new Node(new DOF[] { new DOF(120), new DOF(0) })); // 4
                myNodes.Add(new Node(new DOF[] { new DOF(150), new DOF(-30) })); // 5
                myNodes.Add(new Node(new DOF[] { new DOF(180), new DOF(0) })); // 6

                // set fixity and variables
                foreach (int i in new int[] { 0, 6 })
                {
                    foreach (DOF j in myNodes[i].DOFs)
                    {
                        j.Pinned = true;
                    }
                }
                myNodes[6].DOFs[0].Pinned = false;
                myNodes[1].ConvertDOFtoVar(0, 20);
                myNodes[1].ConvertDOFtoVar(1, 40);
                myNodes[3].ConvertDOFtoVar(0, 20);
                myNodes[3].ConvertDOFtoVar(1, 40);
                myNodes[2].ConvertDOFtoVar(1, 40);

                ApplyRelation(myNodes[1], myNodes[5]);
                ApplyRelation(myNodes[0], myNodes[6]);
                ApplyRelation(myNodes[3], myNodes[4]);

                LoadCase lc = new LoadCase("LC1");
                lc.Loads.Add(new Load(-10, lc, myNodes[3].DOFs[1])); // kips
                lc.Loads.Add(new Load(-10, lc, myNodes[4].DOFs[1])); // kips

                //myNodes[3].DOFs[1].Load = -10; // kips
                //myNodes[4].DOFs[1].Load = -10; // kips


                for (int i = 0; i < myNodes.Count; i++)
                {
                    myNodes[i].Index = i;
                }

                myMembers.Add(new Member(myNodes[0], myNodes[3])); // 0
                myMembers.Add(new Member(myNodes[0], myNodes[1])); // 1
                myMembers.Add(new Member(myNodes[1], myNodes[2])); // 2
                myMembers.Add(new Member(myNodes[2], myNodes[4])); // 3
                myMembers.Add(new Member(myNodes[3], myNodes[4])); // 4
                myMembers.Add(new Member(myNodes[1], myNodes[3])); // 5
                myMembers.Add(new Member(myNodes[3], myNodes[2])); // 6
                myMembers.Add(new Member(myNodes[4], myNodes[5])); // 7
                myMembers.Add(new Member(myNodes[2], myNodes[5])); // 8
                myMembers.Add(new Member(myNodes[4], myNodes[6])); // 9
                myMembers.Add(new Member(myNodes[5], myNodes[6])); // 10

                Material steel = new Material(29000, 0.000284, 20, "Steel"); // E = 29,000 ksi; rho = 0.000284 kci, 20 ksi

                foreach (Member m in myMembers)
                {
                    m.Material = steel;
                    m.Area = 1; // in^2
                }

                double?[] symmetry = new double?[] { null, null };
                symmetry[0] = myNodes[2].DOFs[0].Coord;

                Structure myStructure = new Structure(myNodes, myMembers);
                myStructure.SymmetryLine = symmetry;
                myStructure.StructType = Structure.StructureType.Truss;
                myStructure.LoadCases.Add(lc);

                return new ComputedStructure(myStructure);
            }
        }

        private ComputedStructure Test2 // smaller rigid frame
        {
            get
            {
                // generate basic geometry
                int num = 5;
                double dim = 24;
                Material steel = new Material(29000, 0.000284, 20, "Steel"); // E = 29,000 ksi; rho = 0.000284 kci, 20 ksi

                var myStructure = GenericFrame(num, num, dim, steel, true, 20, -5, 20);

                return myStructure;
            }
        }

        private ComputedStructure GenericFrame(int numCol, int numBeam, double dim, Material mat, bool isSym, double latLoad, double gravLoad, double allowVar)
        {
            List<Node> myNodes = new List<Node>();

            if (numBeam % 2 == 0)
            {
                throw new Exception("This method only supports an odd number of top-beam nodes.");
            }


            // generate nodes for first column
            for (int i = 0; i < numCol; i++)
            {
                DOF dx1 = new DOF(-(2 * dim) * ((numBeam - 1) / 2));
                DOF dy1 = new DOF(i * (2 * dim));
                Node n1 = new Node(new DOF[] { dx1, dy1 });

                double x2 = 0;
                double y2 = 0;
                if (i != numCol - 1)
                {
                    x2 = n1.DOFs[0].Coord + dim;
                    y2 = n1.DOFs[1].Coord + dim;
                }
                else
                {
                    x2 = n1.DOFs[0].Coord + dim;
                    y2 = 0;
                }
                DOF dx2 = new DOF(x2);
                DOF dy2 = new DOF(y2);
                Node n2 = new Node(new DOF[] { dx2, dy2 });

                myNodes.Add(n1);
                myNodes.Add(n2);
            }

            // mirror nodes for second column
            List<Node> temp = new List<Node>();
            foreach (Node i in myNodes)
            {
                Node n1 = i;
                DOF dx2 = new DOF(-1 * n1.DOFs[0].Coord);
                DOF dy2 = new DOF(n1.DOFs[1].Coord);
                Node n2 = new Node(new DOF[] { dx2, dy2 });
                temp.Add(n2);
            }
            myNodes.AddRange(temp);

            // generate nodes for top beam
            for (int i = 0; i < 1 + (numBeam - 3) * 2; i++)
            {
                DOF dx1 = new DOF(((numBeam - 1) / 2 - 1) * (-(2 * dim)) + dim * i);
                DOF dy1 = new DOF((numCol - 1) * (2 * dim) - (i % 2) * dim);
                Node n1 = new Node(new DOF[] { dx1, dy1 });
                myNodes.Add(n1);
            }

            for (int i = 0; i < myNodes.Count; i++)
            {
                myNodes[i].Index = i;
            }

            // make member list
            List<Member> myMembers = new List<Member>();

            for (int i = 0; i < 2 * numCol - 2; i++)
            {
                Member m1 = new Member(myNodes[i], myNodes[i + 1]);
                Member m2 = new Member(myNodes[2 * numCol + i], myNodes[2 * numCol + i + 1]);
                myMembers.Add(m1);
                myMembers.Add(m2);
            }

            for (int i = 0; i < 2 * numCol - 3; i++)
            {
                Member m1 = new Member(myNodes[i], myNodes[i + 2]);
                Member m2 = new Member(myNodes[2 * numCol + i], myNodes[2 * numCol + i + 2]);
                myMembers.Add(m1);
                myMembers.Add(m2);
            }

            //foreach (int[] i in new int[][] { new int[] { 0, 13 }, new int[] { 1, 13 }, new int[] { 14, 27 }, 
            //new int[] { 15, 27 }, new int[] {12, 28}, new int[] {11, 29}, 
            //new int[] { 25, 35 }, new int[] { 26, 36 }})

            foreach (int[] i in new int[][] { new int[] { 0, 2*numCol-1 }, new int[] { 1, 2*numCol-1 }, new int[] { 2*numCol, 4*numCol-1 }, 
                new int[] { 2*numCol+1, 4*numCol-1 }, new int[] {2*numCol-2, 4*numCol}, new int[] {2*numCol-3, 4*numCol+1}, 
                new int[] { 4*numCol-3, 4*numCol + 2*(numBeam-3) - 1 }, new int[] { 4*numCol-2, 4*numCol + 2*(numBeam-3) }})
            {
                Member m1 = new Member(myNodes[i[0]], myNodes[i[1]]);
                myMembers.Add(m1);
            }

            for (int i = 0; i < (numBeam - 3) * 2; i++)
            {
                Member m1 = new Member(myNodes[i + 4 * numCol], myNodes[i + 4 * numCol + 1]);
                myMembers.Add(m1);
            }
            for (int i = 0; i < (numBeam - 3) * 2 - 1; i++)
            {
                Member m1 = new Member(myNodes[i + 4 * numCol], myNodes[i + 4 * numCol + 2]);
                myMembers.Add(m1);
            }

            myMembers.Add(new Member(myNodes[2 * numCol - 3], myNodes[4 * numCol]));
            myMembers.Add(new Member(myNodes[4 * numCol - 3], myNodes[4 * numCol + 2 * (numBeam - 3)]));

            // give member properties
               foreach (Member i in myMembers)
            {
                i.Area = 1; //in^2
                i.Material = mat;
            }

            // define node fixity
            foreach (int i in new int[] { 0, 2 * numCol - 1, 2 * numCol, 4 * numCol - 1 })
            {
                foreach (DOF j in myNodes[i].DOFs)
                {
                    j.Pinned = true;
                }
            }

            // define Locked nodes
            foreach (Node n in myNodes)
            {
                for (int i = 0; i < n.DOFs.Length; i++)
                {
                    n.ConvertDOFtoVar(i, 1);
                }
            }

            //foreach (int i in new int[] { 0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36})
            for (int i = 0; i <= 4 * numCol + 2 * (numBeam - 3); i = i + 2)
            {
                myNodes[i].RevertVartoDOF(0);
                myNodes[i].RevertVartoDOF(1);
            }
            myNodes[2 * numCol - 1].RevertVartoDOF(1);
            myNodes[4 * numCol - 1].RevertVartoDOF(1);

            // assign gravity loads
            LoadCase lc1 = new LoadCase("lc1");
            LoadCase lc2 = new LoadCase("lc2");

            foreach (int i in new int[] { 2 * numCol - 2 })
            {
                lc1.Loads.Add(new Load(gravLoad, lc1, myNodes[i].DOFs[1]));
                lc2.Loads.Add(new Load(gravLoad, lc1, myNodes[i].DOFs[1]));
                //myNodes[i].DOFs[1].Load = gravLoad; // kips
            }
            for (int i = 4 * numCol - 2; i <= 4 * numCol + 2 * (numBeam - 3); i = i + 2)
            {
                lc1.Loads.Add(new Load(gravLoad, lc1, myNodes[i].DOFs[1]));
                lc2.Loads.Add(new Load(gravLoad, lc1, myNodes[i].DOFs[1]));
                //myNodes[i].DOFs[1].Load = gravLoad; // kips
            }

            // assign lateral loads
            foreach (int i in new int[] { 2 * numCol - 2 })
            {
                lc2.Loads.Add(new Load(latLoad, lc1, myNodes[i].DOFs[0]));
                
                //myNodes[i].DOFs[0].Load = latLoad; // kips
            }
            foreach (int i in new int[] { 2 * numCol - 2 })
            {
                lc1.Loads.Add(new Load(-latLoad, lc1, myNodes[i].DOFs[0]));
            }
                       

            // generate base structure
            Structure myStructure = new Structure(myNodes, myMembers);
            if (isSym)
            {
                // define mirror and master nodes
                for (int i = 0; i < 2 * numCol; i++)
                {
                    ApplyRelation(myNodes[i], myNodes[i + 2 * numCol]);
                }

                for (int i = 0; i < numBeam - 3; i++)
                {
                    ApplyRelation(myNodes[4 * numCol + i], myNodes[4 * numCol + 2 * (numBeam - 3) - i]);
                }

                double?[] symmetry = new double?[] { 0, null };
                myStructure.SymmetryLine = symmetry;
            }
            myStructure.StructType = Structure.StructureType.Frame;

            foreach (CoordVar dv in myStructure.DesignVariables)
            {
                dv.AllowableVariation = allowVar;
            }

            myStructure.LoadCases.Add(lc1);
            myStructure.LoadCases.Add(lc2);

            return new ComputedStructure(myStructure);
        }

        private ComputedStructure Test3 // open ended test
        {
            get
            {
                List<Node> myNodes = new List<Node>();
                List<Member> myMembers = new List<Member>();

                myNodes.Add(new Node(new DOF[] { new DOF(248), new DOF(189) })); // 0; inches
                myNodes.Add(new Node(new DOF[] { new DOF(587), new DOF(306) })); // 1; inches
                myNodes.Add(new Node(new DOF[] { new DOF(56), new DOF(510) })); // 2; inches
                myNodes.Add(new Node(new DOF[] { new DOF(190), new DOF(580) })); // 3; inches
                myNodes.Add(new Node(new DOF[] { new DOF(329), new DOF(510) })); // 4; inches
                myNodes.Add(new Node(new DOF[] { new DOF(482), new DOF(580) })); // 5; inches
                myNodes.Add(new Node(new DOF[] { new DOF(593), new DOF(510) })); // 6; inches

                myNodes[0].DOFs[0].Pinned = true;
                myNodes[0].DOFs[1].Pinned = true;
                myNodes[1].DOFs[0].Pinned = true;
                myNodes[1].DOFs[1].Pinned = true;

                LoadCase lc = new LoadCase("lc1");
                foreach (int i in new int[] {2, 3, 4, 5, 6})
                {
                    lc.Loads.Add(new Load(-15, lc, myNodes[i].DOFs[1]));
                }

                foreach (Node n in myNodes)
                {
                    n.DOFs[0].PreFix = true;
                }
                myNodes[0].DOFs[1].PreFix = true;
                myNodes[1].DOFs[1].PreFix = true;

                Structure myStructure = new Structure(myNodes, myMembers);
                myStructure.StructType = Structure.StructureType.Truss;
                myStructure.LoadCases.Add(lc);

                return new ComputedStructure(myStructure);
            }
        }

        private ComputedStructure Test3a
        {
            get
            {
                List<Node> myNodes = new List<Node>();
                List<Member> myMembers = new List<Member>();

                myNodes.Add(new Node(new DOF[] { new DOF(0), new DOF(0) })); // 0; inches
                myNodes.Add(new Node(new DOF[] { new DOF(50), new DOF(0) })); // 1; inches
                myNodes.Add(new Node(new DOF[] { new DOF(25), new DOF(300) })); // 2; inches

                myNodes[0].DOFs[0].Pinned = true;
                myNodes[0].DOFs[1].Pinned = true;
                myNodes[1].DOFs[0].Pinned = true;
                myNodes[1].DOFs[1].Pinned = true;

                LoadCase lc = new LoadCase("lc1");
                lc.Loads.Add(new Load(-15, lc, myNodes[2].DOFs[1]));
                lc.Loads.Add(new Load(-15, lc, myNodes[2].DOFs[0]));
                //myNodes[2].DOFs[1].Load = -15;
                //myNodes[2].DOFs[0].Load = -15;

                foreach (Node n in myNodes)
                {
                    n.DOFs[0].PreFix = true;
                }
                myNodes[0].DOFs[1].PreFix = true;
                myNodes[1].DOFs[1].PreFix = true;

                Structure myStructure = new Structure(myNodes, myMembers);
                myStructure.StructType = Structure.StructureType.Truss;
                myStructure.LoadCases.Add(lc);

                return new ComputedStructure(myStructure);
            }
        }

        private ComputedStructure simpletruss // 7-bar truss
        {
            get
            {
                List<Node> myNodes = new List<Node>();
                List<Member> myMembers = new List<Member>();

                // define node coordinates
                myNodes.Add(new Node(new DOF[] { new DOF(0), new DOF(0) })); // 0; inches
                myNodes.Add(new Node(new DOF[] { new DOF(30), new DOF(-30) })); // 1
                myNodes.Add(new Node(new DOF[] { new DOF(90), new DOF(-30) })); // 2
                myNodes.Add(new Node(new DOF[] { new DOF(60), new DOF(0) })); // 3
                myNodes.Add(new Node(new DOF[] { new DOF(120), new DOF(0) })); // 4

                // set fixity and variables
                foreach (int i in new int[] { 0, 4 })
                {
                    foreach (DOF j in myNodes[i].DOFs)
                    {
                        j.Pinned = true;
                    }
                }
                myNodes[4].DOFs[0].Pinned = false;
                myNodes[1].ConvertDOFtoVar(0, 20);
                myNodes[1].ConvertDOFtoVar(1, 40);
                myNodes[3].ConvertDOFtoVar(1, 40);

                ApplyRelation(myNodes[1], myNodes[2]);
                ApplyRelation(myNodes[0], myNodes[4]);

                LoadCase lc = new LoadCase("lc1");
                lc.Loads.Add(new Load(-10, lc, myNodes[3].DOFs[1]));
                //myNodes[3].DOFs[1].Load = -10; // kips


                for (int i = 0; i < myNodes.Count; i++)
                {
                    myNodes[i].Index = i;
                }

                myMembers.Add(new Member(myNodes[0], myNodes[3])); // 0
                myMembers.Add(new Member(myNodes[0], myNodes[1])); // 1
                myMembers.Add(new Member(myNodes[1], myNodes[2])); // 2
                myMembers.Add(new Member(myNodes[2], myNodes[4])); // 3
                myMembers.Add(new Member(myNodes[3], myNodes[4])); // 4
                myMembers.Add(new Member(myNodes[1], myNodes[3])); // 5
                myMembers.Add(new Member(myNodes[3], myNodes[2])); // 6

                Material steel = new Material(29000, 0.000284, 20, "Steel"); // E = 29,000 ksi; rho = 0.000284 kci, 20 ksi

                foreach (Member m in myMembers)
                {
                    m.Material = steel;
                    m.Area = 1; // in^2
                }

                double?[] symmetry = new double?[] { null, null };
                symmetry[0] = myNodes[3].DOFs[0].Coord;

                Structure myStructure = new Structure(myNodes, myMembers);
                myStructure.SymmetryLine = symmetry;
                myStructure.StructType = Structure.StructureType.Truss;
                myStructure.LoadCases.Add(lc);

                return new ComputedStructure(myStructure);
            }
        }

        private ComputedStructure surrogatetruss
        {
            get
            {
                Structure s = this.simpletruss;
                s.Nodes[1].RevertVartoDOF(0);
                ((CoordVar)s.Nodes[1].DOFs[0]).AllowableVariation = 0;
                ((CoordVar)s.Nodes[1].DOFs[1]).AllowableVariation = 50;
                ((CoordVar)s.Nodes[3].DOFs[1]).AllowableVariation = 50;

                s.StructType = Structure.StructureType.Truss;
                return new ComputedStructure(s);
            }
        }

        private ComputedStructure portalframe // frame
        {
            get
            {
                // generate basic geometry
                int num = 7;
                double dim = 24;
                Material steel = new Material(29000, 0.000284, 20, "Steel"); // E = 29,000 ksi; rho = 0.000284 kci, 20 ksi

                var myStructure = GenericFrame(num, num, dim, steel, true, 20, -5, 20);

                return myStructure;
            }
        }

        private ComputedStructure chiassoroof // from maillart
        {
            get
            {
                List<Node> myNodes = new List<Node>();
                List<Member> myMembers = new List<Member>();

                // define nodal coordinates
                double deltax = 73.819; // inches = 1.875m;
                double deltay = 98.425; // inches = 2.5m
                double m = deltay / (4*deltax);
                LoadCase lc = new LoadCase("lc1");

                for (int i = 0; i < 5; i++)
                {
                    double x = deltax * i;
                    double y = m * x;
                    Node slopenode = new Node(new DOF[] { new DOF(x), new DOF(y) });
                    lc.Loads.Add(new Load(-11.24, lc, slopenode.DOFs[1]));
                    //slopenode.DOFs[1].Load = -11.24; // kips = 50 kN
                    myNodes.Add(slopenode);

                    if (i != 0)
                    {
                        Node flatnode = new Node(new DOF[] { new DOF(x), new DOF(0) });
                        flatnode.ConvertDOFtoVar(1, 157.48); // 4m
                        myNodes.Add(flatnode);
                    }
                }
                                
                double?[] symmetry = new double?[] { null, null };
                symmetry[0] = myNodes[7].DOFs[0].Coord;

                List<Node> addnodes = new List<Node>();
                for (int i = 0; i < myNodes.Count - 2; i++ )
                {
                    Node n = myNodes[i];
                    Node sym = new Node(new DOF[] { new DOF(2 * (double)symmetry[0] - n.DOFs[0].Coord), 
                        new DOF(n.DOFs[1].Coord) });
                    ParametricRelation mir = new ParametricRelation(new List<Node> { sym }, RelationType.Mirror);
                    ParametricRelation mas = new ParametricRelation(new List<Node> { sym }, RelationType.Master);
                    ((CoordVar)n.DOFs[0]).Relations.Add(mir);
                    ((CoordVar)n.DOFs[1]).Relations.Add(mas);

                    lc.Loads.Add(new Load(lc.GetLoad(n.DOFs[1]).Value, lc, sym.DOFs[1]));
                    //sym.DOFs[1].Load = n.DOFs[1].Load;
                    addnodes.Add(sym);
                }
                myNodes.AddRange(addnodes);

                myNodes[0].DOFs[0].Pinned = true;
                myNodes[0].DOFs[1].Pinned = true;
                myNodes[9].DOFs[1].Pinned = true;

                for (int i = 0; i < myNodes.Count; i++)
                {
                    myNodes[i].Index = i;
                }

                // add members
                for (int i = 1; i < 6; i += 2)
                {
                    int j = i + 1;
                    Member m1 = new Member(myNodes[i], myNodes[i + 2]);
                    Member m2 = new Member(myNodes[j], myNodes[j + 2]);
                    Member m3 = new Member(myNodes[i], myNodes[j]);
                    Member m4 = new Member(myNodes[i], myNodes[i + 3]);
                    myMembers.Add(m1);
                    myMembers.Add(m2);
                    myMembers.Add(m3);
                    myMembers.Add(m4);
                }
                myMembers.Add(new Member(myNodes[0], myNodes[1]));
                myMembers.Add(new Member(myNodes[0], myNodes[2]));
                for (int i = 10; i < 13; i += 2)
                {
                    int j = i + 1;
                    Member m1 = new Member(myNodes[i], myNodes[i + 2]);
                    Member m2 = new Member(myNodes[j], myNodes[j + 2]);
                    Member m3 = new Member(myNodes[i], myNodes[j]);
                    Member m4 = new Member(myNodes[i], myNodes[i + 3]);
                    myMembers.Add(m1);
                    myMembers.Add(m2);
                    myMembers.Add(m3);
                    myMembers.Add(m4);
                }
                myMembers.Add(new Member(myNodes[9], myNodes[10]));
                myMembers.Add(new Member(myNodes[9], myNodes[11]));
                myMembers.Add(new Member(myNodes[14], myNodes[7]));
                myMembers.Add(new Member(myNodes[15], myNodes[8]));
                myMembers.Add(new Member(myNodes[8], myNodes[14]));
                myMembers.Add(new Member(myNodes[14], myNodes[15]));
                myMembers.Add(new Member(myNodes[7], myNodes[8]));

                // give member properties
                Material steel = new Material(29000, 0.000284, 20, "Steel"); // E = 29,000 ksi; rho = 0.000284 kci, 20 ksi
                foreach (Member i in myMembers)
                {
                    i.Area = 1; //in^2
                    i.Material = steel;
                }

                Structure myStructure = new Structure(myNodes, myMembers);
                myStructure.SymmetryLine = symmetry;
                myStructure.LoadCases.Add(lc);

                myStructure.StructType = Structure.StructureType.Truss;
                return new ComputedStructure(myStructure);
            }
        }

        private ComputedStructure cantroof // cantilevered roof structure
        {
            get
            {
                List<Node> myNodes = new List<Node>();
                List<Member> myMembers = new List<Member>();

                // define nodes
                double deltax = 120; // inches
                double b = 240; // inches
                double slope = 8.0 / 50.0;
                double depth = 24; // inches
                double bhalf = b - depth;
                LoadCase lc = new LoadCase("lc1");

                for (int i = 0; i < 10; i++)
                {
                    double x = deltax * i;
                    double y = slope * x + b;
                    Node slopenode = new Node(new DOF[] { new DOF(x), new DOF(y) });
                    lc.Loads.Add(new Load(-10, lc, slopenode.DOFs[1])); 
                    //slopenode.DOFs[1].Load = -10; // kips
                    myNodes.Add(slopenode);

                    if (i < 9)
                    {
                        double xhalf = deltax * (i + 0.5);
                        double yhalf = slope * x + bhalf;
                        Node bottomnode = new Node(new DOF[] { new DOF(xhalf), new DOF(yhalf) });
                        bottomnode.ConvertDOFtoVar(0, 40);
                        bottomnode.ConvertDOFtoVar(1, 100);
                        myNodes.Add(bottomnode);
                    }
                }

                myNodes.Add(new Node(new DOF[] { new DOF(156), new DOF(0) }));
                myNodes.Add(new Node(new DOF[] { new DOF(192), new DOF(0) }));
                myNodes.Add(new Node(new DOF[] { new DOF(756), new DOF(0) }));
                myNodes.Add(new Node(new DOF[] { new DOF(792), new DOF(0) }));

                for (int i = 19; i < 23; i++)
                {
                    myNodes[i].DOFs[0].Pinned = true;
                    myNodes[i].DOFs[1].Pinned = true;
                }

                for (int i = 0; i < myNodes.Count; i++)
                {
                    myNodes[i].Index = i;
                }

                // define members
                for (int i = 0; i < 18; i++)
                {
                    Member m1 = new Member(myNodes[i], myNodes[i + 1]);
                    myMembers.Add(m1);
                    if (i < 17)
                    {
                        Member m2 = new Member(myNodes[i], myNodes[i + 2]);
                        myMembers.Add(m2);
                    }
                }
                myMembers.Add(new Member(myNodes[3], myNodes[19]));
                myMembers.Add(new Member(myNodes[3], myNodes[20]));
                myMembers.Add(new Member(myNodes[13], myNodes[21]));
                myMembers.Add(new Member(myNodes[13], myNodes[22]));

                // give member properties
                Material steel = new Material(29000, 0.000284, 20, "Steel"); // E = 29,000 ksi; rho = 0.000284 kci, 20 ksi
                foreach (Member i in myMembers)
                {
                    i.Area = 1; //in^2
                    i.Material = steel;
                }

                Structure myStructure = new Structure(myNodes, myMembers);
                myStructure.StructType = Structure.StructureType.Truss;
                myStructure.LoadCases.Add(lc);

                return new ComputedStructure(myStructure);
            }
        }

        private ComputedStructure circulararch // semicircular arch
        {
            get
            {
                List<Node> myNodes = new List<Node>();
                List<Member> myMembers = new List<Member>();
                LoadCase lc = new LoadCase("lc1");

                // define nodal coordinates
                double inc = 18 * Math.PI / 180; // 18 degrees in radians
                double r_outer = 300; // inches
                double r_inner = 260; // inches
                for (int i = 10; i > 4; i--)
                {
                    double angle = inc * i;
                    double xout = r_outer * Math.Cos(angle);
                    double xin = r_inner * Math.Cos(angle);
                    double yout = r_outer * Math.Sin(angle);
                    double yin = r_inner * Math.Sin(angle);

                    Node outer = new Node(new DOF[] { new DOF(xout), new DOF(yout) }); // listener
                    Node inner = new Node(new DOF[] { new DOF(xin), new DOF(yin) }); // leader

                    // apply loads
                    lc.Loads.Add(new Load(-10, lc, outer.DOFs[1])); 
                    //outer.DOFs[1].Load = -10; // kips

                    // define variables
                    inner.ConvertDOFtoVar(0, 30);
                    inner.ConvertDOFtoVar(1, 80);
                    myNodes.Add(outer);
                    myNodes.Add(inner);

                    // parameter = listener - leader, listener = leader + parameter
                    double px = outer.DOFs[0].Coord - inner.DOFs[0].Coord;
                    double py = outer.DOFs[1].Coord - inner.DOFs[1].Coord;
                    ParametricRelation prx = new ParametricRelation(new List<Node> { outer }, RelationType.Offset, px);
                    ParametricRelation pry = new ParametricRelation(new List<Node> { outer }, RelationType.Offset, py);
                    ((CoordVar)inner.DOFs[0]).Relations.Add(prx);
                    ((CoordVar)inner.DOFs[1]).Relations.Add(pry);
                }

                myNodes[0].DOFs[0].Pinned = true;
                myNodes[0].DOFs[1].Pinned = true;
                myNodes[1].DOFs[0].Pinned = true;
                myNodes[1].DOFs[1].Pinned = true;
                myNodes[1].RevertVartoDOF(1);
                myNodes[11].RevertVartoDOF(0);

                double?[] symmetry = new double?[] { null, null };
                symmetry[0] = myNodes[11].DOFs[0].Coord;

                List<Node> addnodes = new List<Node>();
                for (int i = 0; i < myNodes.Count - 2; i++)
                {
                    Node n = myNodes[i];
                    Node sym = new Node(new DOF[] { new DOF(2 * (double)symmetry[0] - n.DOFs[0].Coord), 
                        new DOF(n.DOFs[1].Coord) });
                    ParametricRelation mir = new ParametricRelation(new List<Node> { sym }, RelationType.Mirror);
                    ParametricRelation mas = new ParametricRelation(new List<Node> { sym }, RelationType.Master);
                    ((CoordVar)n.DOFs[0]).Relations.Add(mir);
                    ((CoordVar)n.DOFs[1]).Relations.Add(mas);

                    lc.Loads.Add(new Load(lc.GetLoad(n.DOFs[1]).Value, lc, sym.DOFs[1])); 
                    //sym.DOFs[1].Load = n.DOFs[1].Load;
                    sym.DOFs[0].Pinned = n.DOFs[0].Pinned;
                    sym.DOFs[1].Pinned = n.DOFs[1].Pinned;

                    addnodes.Add(sym);
                }
                myNodes.AddRange(addnodes);

                lc.GetLoad(myNodes[0].DOFs[1]).Value = 0;
                lc.GetLoad(myNodes[12].DOFs[1]).Value = 0;
                //myNodes[0].DOFs[1].Load = 0;
                //myNodes[12].DOFs[1].Load = 0;

                for (int i = 0; i < myNodes.Count; i++)
                {
                    myNodes[i].Index = i;
                }

                // add members
                for (int i = 0; i < 9; i++)
                {
                    Member m1 = new Member(myNodes[i], myNodes[i + 2]);
                    Member m3 = new Member(myNodes[i], myNodes[i + 1]);
                    myMembers.Add(m1);
                    myMembers.Add(m3);
                }
                for (int i = 12; i < 19; i++)
                {
                    Member m1 = new Member(myNodes[i], myNodes[i + 2]);
                    Member m3 = new Member(myNodes[i], myNodes[i + 1]);
                    myMembers.Add(m1);
                    myMembers.Add(m3);
                }
                for (int i = 9; i < 11; i++)
                {
                    Member m1 = new Member(myNodes[i], myNodes[i + 1]);
                    Member m2 = new Member(myNodes[i + 10], myNodes[i + 11]);
                    myMembers.Add(m1);
                    myMembers.Add(m2);
                }
                myMembers.Add(new Member(myNodes[21], myNodes[11]));
                myMembers.Add(new Member(myNodes[20], myNodes[10]));
                myMembers.Add(new Member(myNodes[21], myNodes[10]));
                myMembers.Add(new Member(myNodes[9], myNodes[11]));
                myMembers.Add(new Member(myNodes[19], myNodes[21]));

                // give member properties
                Material steel = new Material(29000, 0.000284, 20, "Steel"); // E = 29,000 ksi; rho = 0.000284 kci, 20 ksi
                foreach (Member i in myMembers)
                {
                    i.Area = 1; //in^2
                    i.Material = steel;
                }

                Structure myStructure = new Structure(myNodes, myMembers);
                myStructure.SymmetryLine = symmetry;
                myStructure.StructType = Structure.StructureType.Truss;
                myStructure.LoadCases.Add(lc);

                return new ComputedStructure(myStructure);
            }
        }

        private ComputedStructure tower // 10-story tower
        {
            get
            {
                List<Node> myNodes = new List<Node>();
                List<Member> myMembers = new List<Member>();
                LoadCase lc = new LoadCase("lc1");

                // define nodal coordinates
                double ftf = 144; // inches
                double x1 = 0;
                double x2 = 240; // inches
                double stories = 7;
                for (int i = 0; i < stories; i++)
                {
                    double y = ftf * i;
                    Node left = new Node(new DOF[] { new DOF(x1), new DOF(y) });
                    Node right = new Node(new DOF[] { new DOF(x2), new DOF(y) });
                    
                    lc.Loads.Add(new Load(50, lc, left.DOFs[0])); 
                    //left.DOFs[0].Load = 50;
                    left.ConvertDOFtoVar(0, 100);

                    ParametricRelation mir = new ParametricRelation(new List<Node> { right }, RelationType.Mirror);
                    ParametricRelation mas = new ParametricRelation(new List<Node> { right }, RelationType.Master);
                    ((CoordVar)left.DOFs[0]).Relations.Add(mir);
                    ((CoordVar)left.DOFs[1]).Relations.Add(mas);

                    myNodes.Add(left);
                    myNodes.Add(right);

                    if (i < stories - 1)
                    {
                        Node diag = new Node(new DOF[] { new DOF(x2 / 2), new DOF(y + ftf / 2) });
                        diag.ConvertDOFtoVar(1, 72);
                        myNodes.Add(diag);
                    }
                }

                lc.GetLoad(myNodes[0].DOFs[0]).Value = 0;
                //myNodes[0].DOFs[0].Load = 0;
                myNodes[0].DOFs[0].Pinned = true;
                myNodes[0].DOFs[1].Pinned = true;
                myNodes[1].DOFs[0].Pinned = true;
                myNodes[1].DOFs[1].Pinned = true;

                for (int i = 0; i < myNodes.Count; i++)
                {
                    myNodes[i].Index = i;
                }

                // add members
                for (int i = 2; i < stories * 3 - 3; i += 3)
                {
                    myMembers.Add(new Member(myNodes[i], myNodes[i - 1]));
                    myMembers.Add(new Member(myNodes[i], myNodes[i - 2]));
                    myMembers.Add(new Member(myNodes[i], myNodes[i + 1]));
                    myMembers.Add(new Member(myNodes[i], myNodes[i + 2]));
                    myMembers.Add(new Member(myNodes[i - 2], myNodes[i + 1]));
                    myMembers.Add(new Member(myNodes[i - 1], myNodes[i + 2]));
                    myMembers.Add(new Member(myNodes[i + 1], myNodes[i + 2]));
                }


                // give member properties
                Material steel = new Material(29000, 0.000284, 20, "Steel"); // E = 29,000 ksi; rho = 0.000284 kci, 20 ksi
                foreach (Member i in myMembers)
                {
                    i.Area = 1; //in^2
                    i.Material = steel;
                }


                double?[] symmetry = new double?[] { x2 / 2, null };
                Structure myStructure = new Structure(myNodes, myMembers);
                myStructure.SymmetryLine = symmetry;
                myStructure.StructType = Structure.StructureType.Truss;
                myStructure.LoadCases.Add(lc);

                return new ComputedStructure(myStructure);
            }
        }

        private ComputedStructure bridge // arch/cable/truss bridge
        {
            get
            {
                List<Node> myNodes = new List<Node>();
                List<Member> myMembers = new List<Member>();

                // define nodal coordinates
                double deltax = 120; // inches
                double deltay = 72; // inches
                LoadCase lc = new LoadCase("lc1");

                for (int i = 0; i < 4; i++)
                {
                    double x = i * deltax;
                    double y = 0;
                    Node top = new Node(new DOF[] { new DOF(x), new DOF(y + deltay) });
                    Node bottom = new Node(new DOF[] { new DOF(x), new DOF(y) });
                    top.ConvertDOFtoVar(1, 300);
                    top.ConvertDOFtoVar(0, 40);
                    lc.Loads.Add(new Load(-40, lc, bottom.DOFs[1]));
                    //bottom.DOFs[1].Load = -40; // kips
                    myNodes.Add(top);
                    myNodes.Add(bottom);
                }

                double?[] symmetry = new double?[] { 360, null };
                List<Node> addnodes = new List<Node>();
                for (int i = 0; i < myNodes.Count - 2; i++)
                {
                    Node n = myNodes[i];
                    Node sym = new Node(new DOF[] { new DOF(2 * (double)symmetry[0] - n.DOFs[0].Coord), 
                        new DOF(n.DOFs[1].Coord) });
                    ParametricRelation mir = new ParametricRelation(new List<Node> { sym }, RelationType.Mirror);
                    ParametricRelation mas = new ParametricRelation(new List<Node> { sym }, RelationType.Master);
                    ((CoordVar)n.DOFs[0]).Relations.Add(mir);
                    ((CoordVar)n.DOFs[1]).Relations.Add(mas);

                    lc.Loads.Add(new Load(lc.GetLoad(n.DOFs[1]).Value, lc, sym.DOFs[1]));
                    //sym.DOFs[1].Load = n.DOFs[1].Load;
                    sym.DOFs[0].Pinned = n.DOFs[0].Pinned;
                    sym.DOFs[1].Pinned = n.DOFs[1].Pinned;

                    addnodes.Add(sym);
                }
                myNodes.AddRange(addnodes);

                myNodes[0].RevertVartoDOF(0);
                myNodes[6].RevertVartoDOF(0);
                myNodes[0].DOFs[0].Pinned = true;
                myNodes[0].DOFs[1].Pinned = true;
                myNodes[8].DOFs[0].Pinned = true;
                myNodes[8].DOFs[1].Pinned = true;
                myNodes[1].DOFs[1].Pinned = true;
                myNodes[9].DOFs[1].Pinned = true;

                for (int i = 0; i < myNodes.Count; i++)
                {
                    myNodes[i].Index = i;
                }

                // add members
                for (int i = 0; i < 5; i++)
                {
                    int j = i + 8;
                    Member m1 = new Member(myNodes[i], myNodes[i + 2]);
                    myMembers.Add(m1);
                    
                    if (j < 12)
                    {
                        Member m2 = new Member(myNodes[j], myNodes[j + 2]);
                        myMembers.Add(m2);
                    }
                    
                    if (i % 2 == 0)
                    {
                        Member m3 = new Member(myNodes[i], myNodes[i + 3]);
                        Member m5 = new Member(myNodes[i], myNodes[i + 1]);
                        myMembers.Add(m3);
                        myMembers.Add(m5);
                        if (j < 12)
                        {
                            Member m4 = new Member(myNodes[j], myNodes[j + 3]);
                            Member m6 = new Member(myNodes[j], myNodes[j + 1]);
                            myMembers.Add(m4);
                            myMembers.Add(m6);
                        }
                    }
                }
                myMembers.Add(new Member(myNodes[5], myNodes[7]));
                myMembers.Add(new Member(myNodes[6], myNodes[7]));
                myMembers.Add(new Member(myNodes[12], myNodes[6]));
                myMembers.Add(new Member(myNodes[12], myNodes[7]));
                myMembers.Add(new Member(myNodes[12], myNodes[13]));
                myMembers.Add(new Member(myNodes[13], myNodes[7]));


                // give member properties
                Material steel = new Material(29000, 0.000284, 20, "Steel"); // E = 29,000 ksi; rho = 0.000284 kci, 20 ksi
                foreach (Member i in myMembers)
                {
                    i.Area = 1; //in^2
                    i.Material = steel;
                }

                Structure myStructure = new Structure(myNodes, myMembers);
                myStructure.SymmetryLine = symmetry;
                myStructure.StructType = Structure.StructureType.Truss;
                myStructure.LoadCases.Add(lc);

                return new ComputedStructure(myStructure);
            }
        }

        private ComputedStructure unstable // test
        {
            get
            {
                List<Node> myNodes = new List<Node>();
                List<Member> myMembers = new List<Member>();

                myNodes.Add(new Node(new DOF[] { new DOF(0), new DOF(0) }));
                myNodes.Add(new Node(new DOF[] { new DOF(0), new DOF(100) }));
                myNodes.Add(new Node(new DOF[] { new DOF(150), new DOF(0) }));
                myNodes.Add(new Node(new DOF[] { new DOF(150), new DOF(100) }));

                LoadCase lc = new LoadCase("lc1");
                lc.Loads.Add(new Load(10, lc, myNodes[1].DOFs[0]));
                //myNodes[1].DOFs[0].Load = 10;

                myNodes[1].ConvertDOFtoVar(0, 20);

                myNodes[0].DOFs[0].Pinned = true;
                myNodes[0].DOFs[1].Pinned = true;
                myNodes[2].DOFs[0].Pinned = true;
                myNodes[2].DOFs[1].Pinned = true;
                
                myMembers.Add(new Member(myNodes[0], myNodes[1]));
                myMembers.Add(new Member(myNodes[1], myNodes[3]));
                myMembers.Add(new Member(myNodes[2], myNodes[3]));

                Material steel = new Material(29000, 0.000284, 20, "Steel"); // E = 29,000 ksi; rho = 0.000284 kci, 20 ksi
                foreach (Member i in myMembers)
                {
                    i.Area = 1; //in^2
                    i.Material = steel;
                }

                Structure myStructure = new Structure(myNodes, myMembers);
                myStructure.StructType = Structure.StructureType.Truss;
                myStructure.LoadCases.Add(lc);

                return new ComputedStructure(myStructure);
            }
        }

        private ComputedStructure airportroof // 40' x 25' frame with shaped inner profile
        {
            get
            {
                // generate basic geometry
                int numCol = 6;
                int numBeam = 9;
                //double dim = 30; // 30in = 2.5ft
                double dim = 29.52756; // = 0.75m = 2.46ft

                //double fc = 3;
                double fc = 2.9; // = 20 MPa;
                Material concrete = new Material(57*Math.Sqrt(3*1000), 0.0000868, fc, "Reinforced Concrete"); // E = 29,000 ksi; rho = 0.000284 kci, 20 ksi

                double thickness = 3.937; // = 0.1m
                RectangularSection rec = new RectangularSection(thickness, "Rec");

                double gravload = -5.620; // 25 kN
                double latload = 16.860; // 75 kN;
                var myStructure = GenericFrame(numCol, numBeam, dim, concrete, false, latload, gravload, 30);
                myStructure.Sections.Add(rec);
                foreach (Member m in myStructure.Members)
                {
                    m.SectionType = rec;
                }

                List<Node> allNodes = myStructure.Nodes;
                var nodes0 = new Node[] { allNodes[25], allNodes[27], allNodes[29], allNodes[31], allNodes[33], allNodes[35],
                                        allNodes[1], allNodes[3], allNodes[7], allNodes[13], allNodes[15], allNodes[19]        };

                var nodes1 = new Node[] { allNodes[1], allNodes[3], allNodes[5], allNodes[7], allNodes[13], allNodes[15], allNodes[17], allNodes[19],
                                         allNodes[25], allNodes[29], allNodes[31], allNodes[35]};

                foreach (Node n in nodes0)
                {
                    n.RevertVartoDOF(0);
                }

                foreach (Node n in nodes1)
                {
                    n.RevertVartoDOF(1);
                }

                ((CoordVar)allNodes[5].DOFs[0]).Relations.Add(new ParametricRelation(new List<Node>() {allNodes[1], allNodes[3]} , RelationType.Average, allNodes[11]));
                ((CoordVar)allNodes[11].DOFs[0]).Relations.Add(new ParametricRelation(new List<Node>() { allNodes[1], allNodes[3] }, RelationType.Average, allNodes[5]));

                ((CoordVar)allNodes[5].DOFs[0]).Relations.Add(new ParametricRelation(new List<Node>() { allNodes[7] }, RelationType.Average, allNodes[9]));
                ((CoordVar)allNodes[9].DOFs[0]).Relations.Add(new ParametricRelation(new List<Node>() { allNodes[7] }, RelationType.Average, allNodes[5]));

                ((CoordVar)allNodes[17].DOFs[0]).Relations.Add(new ParametricRelation(new List<Node>() { allNodes[13], allNodes[15] }, RelationType.Average, allNodes[23]));
                ((CoordVar)allNodes[23].DOFs[0]).Relations.Add(new ParametricRelation(new List<Node>() { allNodes[13], allNodes[15] }, RelationType.Average, allNodes[17]));

                ((CoordVar)allNodes[17].DOFs[0]).Relations.Add(new ParametricRelation(new List<Node>() { allNodes[19] }, RelationType.Average, allNodes[21]));
                ((CoordVar)allNodes[21].DOFs[0]).Relations.Add(new ParametricRelation(new List<Node>() { allNodes[19] }, RelationType.Average, allNodes[17]));

                ((CoordVar)allNodes[27].DOFs[1]).Relations.Add(new ParametricRelation(new List<Node>() { allNodes[25] }, RelationType.Average, allNodes[9]));
                ((CoordVar)allNodes[9].DOFs[1]).Relations.Add(new ParametricRelation(new List<Node>() { allNodes[25] }, RelationType.Average, allNodes[27]));

                ((CoordVar)allNodes[27].DOFs[1]).Relations.Add(new ParametricRelation(new List<Node>() { allNodes[29], allNodes[31] }, RelationType.Average, allNodes[33]));
                ((CoordVar)allNodes[33].DOFs[1]).Relations.Add(new ParametricRelation(new List<Node>() { allNodes[29], allNodes[31] }, RelationType.Average, allNodes[27]));

                ((CoordVar)allNodes[33].DOFs[1]).Relations.Add(new ParametricRelation(new List<Node>() { allNodes[35] }, RelationType.Average, allNodes[21]));
                ((CoordVar)allNodes[21].DOFs[1]).Relations.Add(new ParametricRelation(new List<Node>() { allNodes[35] }, RelationType.Average, allNodes[33]));


                return myStructure;
            }
        }

        public IDesign GetDesign(int i)
        {
            if (i <= Designs.Count)
            {
                return Designs[i-1];
            }

            else
            {
                //// todo: assemble structure from user input in setup tab
                //List<Node> n = new List<Node>();
                //ComputedStructure s = new ComputedStructure(new Structure(n));
                //return s;
                throw new Exception("List does not contain desired structure.");
            }
        }

        private void ApplyRelation(Node leader, Node listener)
        {
            List<Node> listeners = new List<Node>() { listener };
            ((CoordVar)leader.DOFs[0]).Relations.Add(new ParametricRelation(listeners, RelationType.Mirror));
            ((CoordVar)leader.DOFs[1]).Relations.Add(new ParametricRelation(listeners, RelationType.Master));
            listener.RevertVartoDOF(0);
            listener.RevertVartoDOF(1);
        }

    }
}
