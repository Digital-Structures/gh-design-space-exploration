using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using LiveCharts;
using LiveCharts.Wpf;
using Radical;
using DSOptimization;

namespace Stepper
{
    //GRAPH VM
    public class StepperGraphVM : BaseVM
    {
        private ChartValues<ChartValues<double>> MyData;

        //CONSTRUCTOR
        public StepperGraphVM(ChartValues<ChartValues<double>> data)
        {
            this.MyData = data;
            this.colors = new List<SolidColorBrush>();
            this.axissteps = 1;

            InitializeGraphSeries();
        }

        //OBJECTIVE SERIES
        //Collection of objective evolution data series to be displayed
        private SeriesCollection GraphSeries;
        public SeriesCollection ObjectiveSeries
        {
            get { return this.GraphSeries; }
        }

        //TRACKED STEP
        private int step;
        public int GraphStep
        {
            get { return this.step; }
            set
            {
                CheckPropertyChanged<int>("GraphStep", ref step, ref value);
            }
        }

        //COLORS
        //Assign thematic colors to the various line series
        private List<SolidColorBrush> colors;
        public List<SolidColorBrush> Colors
        {
            set
            {
                this.colors = value;

                for (int i=0; i<this.MyData.Count; i++)
                {
                    if (i == this.colors.Count)
                        i = 0;
                    else
                        ((LineSeries)this.ObjectiveSeries[i]).Stroke = this.colors[i];
                }
            }
        }

        //XAxisSteps
        private int axissteps;
        public int XAxisSteps
        {
            get { return axissteps; }
            set { CheckPropertyChanged<int>("XAxisSteps", ref axissteps, ref value); }
        }

        //INITIALIZE GRAPH SERIES
        public void InitializeGraphSeries()
        {
            //Series Collection for objective graphs
            this.GraphSeries = new SeriesCollection();

            foreach (ChartValues<double> objective in this.MyData)
            {
                //Make a line series for the given objective
                GraphSeries.Add(new LineSeries
                {
                    Values = objective,
                    StrokeThickness = 5,
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0,
                    PointGeometrySize = 15
                });
            }
        }
    }
}
