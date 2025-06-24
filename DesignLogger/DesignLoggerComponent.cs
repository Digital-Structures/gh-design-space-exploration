using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Timers;
using DesignLogger.Properties;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;

namespace DesignLogger
{
    // Token: 0x02000002 RID: 2
    public class DesignLoggerComponent : GH_Component
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public DesignLoggerComponent() : base("DesignLogger", "DLogger", "Logs a design exploration session", "DSE", "Catalog")
        {
            this.DVecs = new List<List<double>>();
            this.OVecs = new List<List<double>>();
            this.DVec = new List<double>();
            this.OVec = new List<double>();
            this.Favorites = new List<List<double>>();
            this.timelist = new List<string>();
            this.elapsedlist = new List<double>();
            this.count = 0;
            this.favCount = new List<double>();
            this.timeListDate = new List<DateTime>();
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002150 File Offset: 0x00000350
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Record", "Rec", "Start recording", (GH_ParamAccess)0, false);
            pManager.AddNumberParameter("Variables", "Var", "Reads design vector", (GH_ParamAccess)1);
            pManager.AddNumberParameter("Objectives", "Obj", "Reads objective vector", (GH_ParamAccess)1);
            pManager.AddBooleanParameter("Favorite", "Fav", "Favorite", (GH_ParamAccess)0, false);
            pManager.AddBooleanParameter("Write", "Write", "End the session", (GH_ParamAccess)0, false);
            pManager.AddTextParameter("Directory", "Dir", "Log file path", (GH_ParamAccess)0, (string)null);
        }

        // Token: 0x06000003 RID: 3 RVA: 0x000021EC File Offset: 0x000003EC
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Favorites", "Favs", "List of 'Favorite' designs", (GH_ParamAccess)1);
            pManager.AddNumberParameter("Variable History", "Var Hist", "List of recorded variables", (GH_ParamAccess)1);
            pManager.AddNumberParameter("Objective History", "Obj Hist", "List of recorded objectives", (GH_ParamAccess)1);
            pManager.AddNumberParameter("Elapsed Time", "Time Hist", "List of time spent on each design", (GH_ParamAccess)1);
            pManager.AddNumberParameter("Total Time", "Total Time", "Total elapsed time from beginning of recording", (GH_ParamAccess)0);
        }

        // Token: 0x06000004 RID: 4 RVA: 0x00002270 File Offset: 0x00000470
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool flag = this.file == null;
            if (flag)
            {
                bool flag2 = false;
                DA.GetData<bool>(0, ref flag2);
                bool flag3 = !flag2;
                if (flag3)
                {
                    this.DVecs.Clear();
                    this.OVecs.Clear();
                    this.Favorites.Clear();
                    this.timelist.Clear();
                    this.timeListDate.Clear();
                    this.elapsedlist.Clear();
                    this.favCount.Clear();
                    this.count = 0;
                    this.favCount.Add(0.0);
                    this.timelist.Add("0");
                    this.first = true;
                    return;
                }
                bool flag4 = this.first;
                if (flag4)
                {
                    DA.GetData<string>(5, ref this.path);
                    this.first = false;
                    this.StartFormat = DateTime.Now;
                    string value = DateTime.Now.ToString("yyyyMMddHHmmss");
                    this.StartTime = Convert.ToDouble(value);
                    List<double> list = new List<double>();
                    List<double> list2 = new List<double>();
                    DA.GetDataList<double>(1, list);
                    DA.GetDataList<double>(2, list2);
                }
            }
            this.DVec.Clear();
            this.OVec.Clear();
            List<double> list3 = new List<double>();
            List<double> list4 = new List<double>();
            DA.GetDataList<double>(1, list3);
            DA.GetDataList<double>(2, list4);
            DA.GetData<bool>(3, ref this.favorite);
            DA.GetData<bool>(4, ref this.End);
            DateTime now = DateTime.Now;
            string value2 = DateTime.Now.ToString("yyyyMMddHHmmss");
            double num = Convert.ToDouble(value2);
            double totalSeconds = now.Subtract(this.StartFormat).TotalSeconds;
            this.DVecs.Add(list3);
            this.OVecs.Add(list4);
            this.timelist.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
            this.timeListDate.Add(DateTime.Now);
            this.elapsedlist.Add(totalSeconds);
            this.count++;
            bool end = this.End;
            if (end)
            {
                this.PrintAllSolutions();
                this.saveTimer.Stop();
                this.endSession();
            }
            this.favCount.Add(0.0);
            bool flag5 = this.favorite;
            if (flag5)
            {
                this.MakeFavorite(list3);
            }
            DA.SetDataTree(0, DesignLoggerComponent.ListOfListsToTree<double>(this.Favorites));
            DA.SetDataTree(1, DesignLoggerComponent.ListOfListsToTree<double>(this.DVecs));
            DA.SetDataTree(2, DesignLoggerComponent.ListOfListsToTree<double>(this.OVecs));
            DA.SetDataTree(3, DesignLoggerComponent.ListToTree<double>(this.elapsedlist));
            DA.SetData(4, totalSeconds);
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002550 File Offset: 0x00000750
        public void PrintAllSolutions()
        {
            bool flag = !this.path[this.path.Length - 1].Equals('\\');
            if (flag)
            {
                this.path += "\\";
            }
            StreamWriter streamWriter = new StreamWriter(this.path + "DLogger_" + DateTime.Now.ToString("HHmmss") + ".csv");
            for (int i = 0; i < this.DVecs.Count; i++)
            {
                string text = "";
                List<double> list = this.DVecs[i];
                for (int j = 0; j < list.Count; j++)
                {
                    text = text + list[j] + ",";
                }
                List<double> list2 = this.OVecs[i];
                for (int k = 0; k < list2.Count; k++)
                {
                    text = text + list2[k] + ",";
                }
                text = text + this.timelist[i] + ",";
                text = text + this.elapsedlist[i] + ",";
                text = text + this.favCount[i] + ",";
                streamWriter.WriteLine(text);
            }
            streamWriter.Close();
        }

        // Token: 0x06000006 RID: 6 RVA: 0x000026F8 File Offset: 0x000008F8
        public void endSession()
        {
        }

        // Token: 0x06000007 RID: 7 RVA: 0x000026FB File Offset: 0x000008FB
        public void MakeFavorite(List<double> Temp)
        {
            this.favCount[this.count] = 1.0;
            this.Favorites.Add(Temp);
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002728 File Offset: 0x00000928
        protected void log(string message)
        {
            bool flag = this.file != null;
            if (flag)
            {
                this.file.WriteLine("{0}: {1}", DateTime.Now.ToString(), message);
            }
        }

        // Token: 0x06000009 RID: 9 RVA: 0x00002764 File Offset: 0x00000964
        private static DataTree<T> ListOfListsToTree<T>(List<List<T>> listofLists)
        {
            DataTree<T> dataTree = new DataTree<T>();
            for (int i = 0; i < listofLists.Count; i++)
            {
                dataTree.AddRange(listofLists[i], new GH_Path(i));
            }
            return dataTree;
        }

        // Token: 0x0600000A RID: 10 RVA: 0x000027A8 File Offset: 0x000009A8
        private static DataTree<T> ListToTree<T>(List<T> List)
        {
            DataTree<T> dataTree = new DataTree<T>();
            for (int i = 0; i < List.Count; i++)
            {
                dataTree.Add(List[i], new GH_Path(i));
            }
            return dataTree;
        }

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x0600000B RID: 11 RVA: 0x000027EC File Offset: 0x000009EC
        protected override Bitmap Icon
        {
            get
            {
                return Resources.Logger;
            }
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x0600000C RID: 12 RVA: 0x00002804 File Offset: 0x00000A04
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("8edb7005-f3b2-4a1e-aa48-b84c2df3e204");
            }
        }

        // Token: 0x04000001 RID: 1
        protected StreamWriter file = null;

        // Token: 0x04000002 RID: 2
        protected int oldMonth = -1;

        // Token: 0x04000003 RID: 3
        protected int oldDay = -1;

        // Token: 0x04000004 RID: 4
        protected double oldHour = -1.0;

        // Token: 0x04000005 RID: 5
        public List<List<double>> DVecs;

        // Token: 0x04000006 RID: 6
        public List<List<double>> OVecs;

        // Token: 0x04000007 RID: 7
        public List<List<double>> Favorites;

        // Token: 0x04000008 RID: 8
        public List<double> DVec;

        // Token: 0x04000009 RID: 9
        public List<double> OVec;

        // Token: 0x0400000A RID: 10
        public List<string> timelist;

        // Token: 0x0400000B RID: 11
        public List<DateTime> timeListDate;

        // Token: 0x0400000C RID: 12
        public List<double> elapsedlist;

        // Token: 0x0400000D RID: 13
        public bool favorite = false;

        // Token: 0x0400000E RID: 14
        public bool End = false;

        // Token: 0x0400000F RID: 15
        public string path;

        // Token: 0x04000010 RID: 16
        public int participant;

        // Token: 0x04000011 RID: 17
        public bool first = true;

        // Token: 0x04000012 RID: 18
        public int count;

        // Token: 0x04000013 RID: 19
        public List<double> favCount;

        // Token: 0x04000014 RID: 20
        public double lastTime = 0.0;

        // Token: 0x04000015 RID: 21
        public double goTime = 0.0;

        // Token: 0x04000016 RID: 22
        public double StartTime = 0.0;

        // Token: 0x04000017 RID: 23
        public DateTime StartFormat;

        // Token: 0x04000018 RID: 24
        private Timer sectionTimer;

        // Token: 0x04000019 RID: 25
        private Timer saveTimer;
    }
}
