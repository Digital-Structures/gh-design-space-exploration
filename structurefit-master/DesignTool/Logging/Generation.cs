using System.Collections.Generic;
using System.Text;
using StructureEngine.Model;

namespace StructureEngine.Logging
{
    public class Generation
    {
        public Generation(IList<ComputedStructure> top, double g, double m, bool a, int? c, int num)
        {
            this.Top = top;
            this.GenSize = g;
            this.MutRate = m;
            this.AutoMode = a;
            this.AutoCount = c;
            this.GenNum = num;
            this.Selected = new List<ComputedStructure>();
        }

        public void UpdateSelected(IList<ComputedStructure> selected)
        {
            this.Selected = selected;
        }

        private int GenNum;
        private IList<ComputedStructure> Top;
        private IList<ComputedStructure> Selected;
        private double GenSize;
        private double MutRate;
        private bool AutoMode;
        private int? AutoCount;

        public string Serialize(double RefScore)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GenNum + "\t");
            sb.Append(MutRate + "\t");
            sb.Append(GenSize + "\t");
            sb.Append((AutoMode ? 1 : 0) + "\t");
            sb.Append((AutoMode ? AutoCount : 0) + "\t");
            sb.Append(Selected.Count + "\t");
            sb.Append(this.AvgScore(RefScore) + "\t");
            sb.Append(this.AvgRank() + "\t");

            foreach (ComputedStructure s in this.Top)
            {
                sb.Append(this.GetSelectedData(s, RefScore));
            }

            return sb.ToString();
        }

        public string FullSerialize(double RefScore)
        {
            StringBuilder sb = new StringBuilder();
            StructureSerializer ss = new StructureSerializer();
            foreach (ComputedStructure s in this.Top)
            {
                sb.Append(this.GenNum + "\t");
                sb.Append(this.Top.IndexOf(s) + "\t");
                if (this.Selected.Contains(s))
                {
                    sb.Append(1 + "\t");
                }
                else
                {
                    sb.Append(0 + "\t");
                }
                sb.Append(ss.QuickSerialize(s, RefScore));
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private double AvgRank()
        {
            double sum = 0;
            for (int i = 0; i < Top.Count; i++)
            {
                foreach (ComputedStructure s_total in Selected)
                {
                    if (s_total.IsSame(Top[i]))
                    {
                        sum += i;
                        break;
                    }
                }
            }
            return sum / Selected.Count;
        }

        private double AvgScore(double RefScore)
        {
            double sum = 0;
            foreach (ComputedStructure s in this.Selected)
            {
                sum += s.Score / RefScore;
            }
            return sum / Selected.Count;
        }

        private string GetSelectedData(ComputedStructure s, double RefScore)
        {
            StringBuilder sb = new StringBuilder();
            int selected = 0;
            foreach (ComputedStructure s_total in Selected)
            {
                if (s_total.IsSame(s))
                {
                    selected = 1;   
                    break;
                }
            }
            sb.Append(selected + "\t");
            sb.Append(s.Score/RefScore + "\t");
            return sb.ToString();
        }
    }
}
