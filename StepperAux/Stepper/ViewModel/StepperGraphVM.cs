using System;
using System.Collections.Generic;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;

namespace Stepper
{
    // Token: 0x0200001C RID: 28
    public class StepperGraphVM : BaseVM
    {
        // Token: 0x060000E4 RID: 228 RVA: 0x00005653 File Offset: 0x00003853
        public StepperGraphVM(ChartValues<ChartValues<double>> data)
        {
            this.MyData = data;
            this.colors = new List<SolidColorBrush>();
            this.axissteps = 1;
            this.InitializeGraphSeries();
        }

        // Token: 0x1700004B RID: 75
        // (get) Token: 0x060000E5 RID: 229 RVA: 0x00005680 File Offset: 0x00003880
        public SeriesCollection ObjectiveSeries
        {
            get
            {
                return this.GraphSeries;
            }
        }

        // Token: 0x1700004C RID: 76
        // (get) Token: 0x060000E6 RID: 230 RVA: 0x00005698 File Offset: 0x00003898
        // (set) Token: 0x060000E7 RID: 231 RVA: 0x000056B0 File Offset: 0x000038B0
        public int GraphStep
        {
            get
            {
                return this.step;
            }
            set
            {
                base.CheckPropertyChanged<int>("GraphStep", ref this.step, ref value);
            }
        }

        // Token: 0x1700004D RID: 77
        // (set) Token: 0x060000E8 RID: 232 RVA: 0x000056C8 File Offset: 0x000038C8
        public List<SolidColorBrush> Colors
        {
            set
            {
                this.colors = value;
                for (int i = 0; i < this.MyData.Count; i++)
                {
                    bool flag = i == this.colors.Count;
                    if (flag)
                    {
                        i = 0;
                    }
                    else
                    {
                        ((LineSeries)this.ObjectiveSeries[i]).Stroke = this.colors[i];
                    }
                }
            }
        }

        // Token: 0x1700004E RID: 78
        // (get) Token: 0x060000E9 RID: 233 RVA: 0x00005734 File Offset: 0x00003934
        // (set) Token: 0x060000EA RID: 234 RVA: 0x0000574C File Offset: 0x0000394C
        public int XAxisSteps
        {
            get
            {
                return this.axissteps;
            }
            set
            {
                base.CheckPropertyChanged<int>("XAxisSteps", ref this.axissteps, ref value);
            }
        }

        // Token: 0x060000EB RID: 235 RVA: 0x00005764 File Offset: 0x00003964
        public void InitializeGraphSeries()
        {
            this.GraphSeries = new SeriesCollection();
            foreach (ChartValues<double> values in this.MyData)
            {
                this.GraphSeries.Add(new LineSeries
                {
                    Values = values,
                    StrokeThickness = 5.0,
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0.0,
                    PointGeometrySize = 15.0
                });
            }
        }

        // Token: 0x0400005F RID: 95
        private ChartValues<ChartValues<double>> MyData;

        // Token: 0x04000060 RID: 96
        private SeriesCollection GraphSeries;

        // Token: 0x04000061 RID: 97
        private int step;

        // Token: 0x04000062 RID: 98
        private List<SolidColorBrush> colors;

        // Token: 0x04000063 RID: 99
        private int axissteps;
    }
}
