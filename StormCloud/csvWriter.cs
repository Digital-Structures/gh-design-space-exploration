// Decompiled with JetBrains decompiler
// Type: StormCloud.csvWriter
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using StormCloud.Evolutionary;
using StormCloud.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace StormCloud
{
  public static class csvWriter
  {
    public static void saveFile(string path, string filename, StringBuilder sb)
    {
      string path1 = path + "\\" + filename + ".csv";
      if (!File.Exists(path1))
        File.Create(path1).Close();
      try
      {
        File.WriteAllText(path1, sb.ToString());
      }
      catch (IOException ex)
      {
        int num = (int) MessageBox.Show("The exploration record file is currently open. Close the file before attempting to save the exploration record.");
      }
    }

    public static string writeDesign(DesignVM design)
    {
      string str1 = ",";
      string str2 = "";
      foreach (DesignVar designVariable in design.Design.DesignVariables)
        str2 = str2 + designVariable.Value.ToString() + str1;
      return str2 + design.Score.ToString();
    }

    public static StringBuilder InitRecord(int nVar)
    {
      string str1 = ",";
      StringBuilder stringBuilder = new StringBuilder();
      string str2 = "";
      for (int index = 0; index < nVar; ++index)
        str2 = str2 + "variable " + index.ToString() + str1;
      string str3 = str2 + "score";
      stringBuilder.AppendLine(str3);
      return stringBuilder;
    }

    public static StringBuilder writeGenerations(List<List<DesignVM>> generations)
    {
      string str1 = ",";
      StringBuilder stringBuilder = new StringBuilder();
      if (generations.Count != 0)
      {
        string str2 = "";
        for (int index = 0; index < generations[0][0].Design.DesignVariables.Count; ++index)
          str2 = str2 + "variable " + index.ToString() + str1;
        string str3 = str2 + "score";
        stringBuilder.AppendLine(str3);
        foreach (List<DesignVM> generation in generations)
        {
          foreach (DesignVM design in generation)
            stringBuilder.AppendLine(csvWriter.writeDesign(design));
        }
      }
      return stringBuilder;
    }

    public static void CreateRecordFromGen(
      string path,
      string filename,
      List<List<DesignVM>> generations)
    {
      StringBuilder sb = csvWriter.writeGenerations(generations);
      csvWriter.saveFile(path, filename, sb);
    }

    public static void CreateRecord(string path, string filename, StringBuilder sb, int nVar)
    {
      sb.Insert(0, (object) csvWriter.InitRecord(nVar));
      csvWriter.saveFile(path, filename, sb);
    }
  }
}
