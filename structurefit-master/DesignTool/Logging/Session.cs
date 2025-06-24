using System;
using System.Collections.Generic;
using System.Text;
using StructureEngine.Model;

namespace StructureEngine.Logging
{
    public class Session
    {
        public Session(string id, int num)
        {
            this.SessionID = id;
            this.ProbNum = num;
            this.Generations = new List<Generation>();
        }

        public bool ControlGroup
        {
            get;
            set;
        }

        public ComputedStructure FinalDesign
        {
            get;
            set;
        }


        public ComputedStructure RefineDesign
        {
            get;
            set;
        }

        public ComputedStructure InitialDesign
        {
            get;
            set;
        }

        public ComputedStructure Final1
        {
            get;
            set;
        }

        public ComputedStructure Final2
        {
            get;
            set;
        }

        public ComputedStructure Final3
        {
            get;
            set;
        }

        public DateTime TimeStamp
        {
            get;
            set;
        }

        public string SessionID
        {
            get;
            set;
        }

        public string Group
        {
            get;
            set;
        }

        public int ProbNum
        {
            get;
            set;
        }

        public List<Generation> Generations
        {
            get;
            set;
        }

        public string EvoSerialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//////////////////////");
            sb.AppendLine(" STRUCTUREFIT OUTPUT");
            sb.AppendLine("//////////////////////");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("[Initial_Structure]");
            sb.AppendLine(this.GetStructureData(this.InitialDesign, false, false));
            sb.AppendLine("[Exploration_Data]");
            sb.AppendLine(this.GetExpData(this.Generations));
            sb.AppendLine("[Appendix_Data]");
            sb.AppendLine(this.GetAppData(this.Generations));
            return sb.ToString();
        }

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//////////////////////");
            sb.AppendLine(" STRUCTUREFIT OUTPUT");
            sb.AppendLine("//////////////////////");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("[Personal_Data]");
            sb.AppendLine(this.GetPersonalData());
            sb.AppendLine("[Initial_Structure]");
            sb.AppendLine(this.GetStructureData(this.InitialDesign, false, false));
            if (this.Group != "B" || this.ProbNum != 2)
            {
                sb.AppendLine("[Exploration_Data]");
                sb.AppendLine(this.GetExpData(this.Generations));
                sb.AppendLine("[SelectedStructure_Data]");
                sb.AppendLine(this.GetStructureData(this.RefineDesign, true, true));
                sb.AppendLine(this.GetDistData(this.InitialDesign, this.RefineDesign, "Initial"));
                sb.AppendLine("");
            }
            sb.AppendLine("[FinalStructure_Data]");
            sb.AppendLine(this.GetStructureData(this.FinalDesign, true, true));
            sb.AppendLine(this.GetDistData(this.InitialDesign, this.FinalDesign, "Initial"));
            if (this.Group != "B" || this.ProbNum != 2)
            {
                sb.AppendLine(this.GetDistData(this.RefineDesign, this.FinalDesign, "Selected"));
                sb.AppendLine("");
                sb.AppendLine("[Appendix_Data]");
                sb.AppendLine(this.GetAppData(this.Generations));
            }
            return sb.ToString();
        }

        private string GetPersonalData()
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("SessionID" + "\t" + this.SessionID);
            sb.AppendLine("TimeStamp" + "\t" + this.TimeStamp.ToString());
            //sb.AppendLine("ProbNum" + "\t" + this.ProbNum.ToString());
            //sb.AppendLine("Group" + "\t" + this.Group.ToString());
            return sb.ToString();
        }

        private string GetStructureData(ComputedStructure s, bool quick, bool quickHeader)
        {
            StructureSerializer ss = new StructureSerializer();
            StringBuilder sb = new StringBuilder();
            if (quickHeader)
            {
                sb.AppendLine(ss.QuickSerializeHeader(s));
            }
            sb.AppendLine(quick ? ss.QuickSerialize(s, this.InitialDesign.Score) : ss.Serialize(s, this.InitialDesign.Score));
            return sb.ToString();
        }

        private string GetExpHeader()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb1 = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                sb1.Append("Selected" + i + "\t");
                sb1.Append("Score" + i + "\t");
            }
            sb.Append("GenNum\tMutRate\tGenSize\tAutoMode\tAutoCount\tNumSelected\tAvgScore\tAvgRank\t" + sb1.ToString());
            return sb.ToString();
        }

        private string GetExpData(List<Generation> gens)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.GetExpHeader());
            foreach (Generation g in gens)
            {
                sb.AppendLine(g.Serialize(this.InitialDesign.Score));
            }
            return sb.ToString();
        }

        private string GetDistData(ComputedStructure s1, ComputedStructure s2, string start)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DistanceFrom" + start + "\t");
            sb.Append(s1.GetDistance(s2));
            return sb.ToString();
        }

        private string GetAppData(List<Generation> gens)
        {
            StringBuilder sb = new StringBuilder();
            StructureSerializer ss = new StructureSerializer();
            sb.AppendLine("GenNum\tRank\tSelected\t" + ss.QuickSerializeHeader(this.InitialDesign));
            foreach (Generation g in gens)
            {
                sb.Append(g.FullSerialize(this.InitialDesign.Score));
            }
            return sb.ToString();
        }

        public void SetStartStructure(ComputedStructure s)
        {
            if (s != null)
            {
                this.ClearSession();
                this.InitialDesign = s;
            }
        }

        public void SetRefineStructure(ComputedStructure s)
        {
            if (s != null)
            {
                this.RefineDesign = s;
            }
        }

        public void SetFinalStructure(ComputedStructure s)
        {
            if (s != null)
            {
                this.FinalDesign = s;
                this.TimeStamp = DateTime.Now;
                this.SubmitDesign();

                if (this.ProbNum == 1)
                {
                    this.Final1 = s;
                }
                else if (this.ProbNum == 2)
                {
                    this.Final2 = s;
                }
                else if (this.ProbNum == 3)
                {
                    this.Final3 = s;
                }
            }
        }

        private void ClearSession()
        {
            this.InitialDesign = null;
            this.RefineDesign = null;
            this.FinalDesign = null;
            this.Generations.Clear();
        }

        private void SubmitDesign()
        {
            string output = this.Serialize();
            // send output somewhere?
            System.Diagnostics.Debug.WriteLine(output);

            var req = new HttpWebRequestBuilder("http://www.caitlinmueller.com/post/server.php");
            req.AddStringFile("logfile", string.Format("{0}_{1}.txt", this.SessionID, this.ProbNum), output);
            req.SendRequest();
        }

        public string SaveSession(ComputedStructure s)
        {
            if (s != null)
            {
                this.FinalDesign = s;
                return this.Serialize();
            }

            else
            {
                throw new Exception("Final design is not ComputedStructure.");
            }
        }

    }
}
