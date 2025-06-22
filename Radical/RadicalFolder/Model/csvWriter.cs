using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using DSOptimization;

namespace Radical.Integration
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
            catch (IOException)
            {
                string message = "The exploration record file is currently open. Close the file before attempting to save the exploration record.";
                MessageBox.Show(message);
            }
        }

        public static string writeSample(List<double> sample, List<double> properties)
        {
            string delimiter = ",";
            string line = "";

            foreach (double var in sample)
            {
                line += var.ToString() + delimiter;
            }
            foreach (double prop in properties)
            {
                line += prop.ToString() + delimiter;
            }
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
         
        //public static StringBuilder writeSamplingResults(Design design)
        //{
        //    string delimiter = ",";
        //    StringBuilder sb = new StringBuilder();
        //    string headings = "";
        //    for (int i = 0; i < design.Variables.Count; i++)
        //    {
        //        headings += "variable " + i.ToString() + delimiter;
        //    }
        //    for (int i = 0; i < design.Properties[0].Count; i++)
        //    {
        //        headings += "property " + i.ToString() + delimiter;
        //    }
        //    sb.AppendLine(headings);
        //    for (int i = 0; i < design.Samples.Count; i++)
        //    {
        //        sb.AppendLine( writeSample(design.Samples[i], design.Properties[i]));
        //    }

        //    return sb;

        //}
        //public static void CreateSamplingRecord(string path, string filename, Design design)
        //{
        //    StringBuilder sb = writeSamplingResults(design);
        //    saveFile(path, filename, sb);
        //}

        public static void CreateRecord(string path, string filename, StringBuilder sb, int nVar)
        {
            sb.Insert(0, InitRecord(nVar));
            saveFile(path, filename, sb);
        }
    }
}
