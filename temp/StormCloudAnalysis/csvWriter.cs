using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using StormCloud.ViewModel;
using StormCloud.Evolutionary;
using System.Windows.Forms;
using System.ComponentModel;



namespace StormCloud
{
    public static class csvWriter
    {

        public static void saveFile(string path, string filename, StringBuilder sb)
        {
            string filePath = path + "\\" + filename + ".csv";
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            try
            {
                File.WriteAllText(filePath, sb.ToString());
            }
            catch(IOException)
            {
                string message = "The exploration record file is currently open. Close the file before attempting to save the exploration record.";
                MessageBox.Show(message);
            }
        }

        public static string writeDesign(DesignVM design)
        {
            string delimiter = ",";
            string line="";
            List<DesignVar> vars = design.Design.DesignVariables;
            foreach(DesignVar var in vars)
            {
                line += var.Value.ToString() + delimiter;
            }
            line += design.Score.ToString();
            return line;
        }
 
        public static StringBuilder InitRecord(int nVar)
        {
            string delimiter = ",";
            StringBuilder sb = new StringBuilder();
            string headings = "";
            for (int i = 0; i < nVar; i++)
            {
                headings += "variable " + i.ToString() + delimiter;
            }
            headings += "score";
            sb.AppendLine(headings);
            return sb;
        }

        public static StringBuilder writeGenerations(List<List<DesignVM>> generations)
        {
            string delimiter = ",";
            StringBuilder sb = new StringBuilder();
            if (generations.Count!=0)
            {
                string headings = "";
                for (int i =0; i< generations[0][0].Design.DesignVariables.Count;i++)
                {
                    headings += "variable " + i.ToString() + delimiter;
                }
                headings += "score";
                sb.AppendLine(headings);
                foreach (List<DesignVM> gen in generations)
                {
                    foreach (DesignVM design in gen)
                    {
                        sb.AppendLine(writeDesign(design));
                    }
                }
            }
            return sb;

        }

        public static void CreateRecordFromGen(string path, string filename, List<List<DesignVM>> generations)
        {
            StringBuilder sb = writeGenerations(generations);
            saveFile(path, filename, sb);
        }

        public static void CreateRecord(string path, string filename, StringBuilder sb, int nVar)
        {
            sb.Insert(0, InitRecord(nVar));
            saveFile(path, filename, sb);
        }
    }
}
