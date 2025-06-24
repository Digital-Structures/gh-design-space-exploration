using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using DSOptimization;

namespace Radical
{
    public class GraphVM : BaseVM
    {
        public ChartValues<double> ChartValues { get; set; }
        public int DefaultMaxXAxis { get; set; }
        private int margin = 20;
        private int space = 5;
        public CartesianChart Graph { get; set; }

        public GraphVM(ChartValues<double> scores, string name)
        {
            ChartValues = scores;
            _linegraph_name = name;
            _y = "0";
            _graphaxislabelsvisibility = Visibility.Hidden;
            _xaxisstep = 1;
            DefaultMaxXAxis = 5;
            _maxxaxis = DefaultMaxXAxis;
            _finaloptimizedvalue = double.NaN;
        }

        private String _linegraph_name;
        public String LineGraphName
        {
            get
            { return _linegraph_name;}
            set
            {
                if (CheckPropertyChanged<String>("LineGraphName", ref _linegraph_name, ref value))
                {
                }
            }
        }

        //Tells the vertical line on the chart where it should be relative to the mouse
        private int _mouseiteration;
        public int MouseIteration
        {
            get { return _mouseiteration; }
            set
            {
                if (CheckPropertyChanged<int>("MouseIteration", ref _mouseiteration, ref value))
                {
                }
            }
        }

        private string _y;
        public string DisplayY
        {
            get { return String.Format("{0}: {1}", this.LineGraphName, _y); }
            set
            {
                CheckPropertyChanged<string>("DisplayY", ref _y, ref value);
            }
        }

        private RadicalWindow _window;
        public RadicalWindow Window
        {
            get { return _window; }
            set
            {
                _window = value;
            }
        }

        private double _graphgridheight;
        public double GraphGridHeight
        {
            get { return _graphgridheight; }
            set
            {
                if (CheckPropertyChanged<double>("GraphGridHeight", ref _graphgridheight, ref value))
                {
                }
            }
        }

        private double? _xaxisstep;
        public double? XAxisStep
        {
            get { return _xaxisstep; }
            set
            {
                if (CheckPropertyChanged<double?>("XAxisStep", ref _xaxisstep, ref value))
                {
                }
            }
        }

        private double? _maxxaxis;
        public double? MaxXAxis
        {
            get { return _maxxaxis; }
            set
            {
                if (CheckPropertyChanged<double?>("MaxXAxis", ref _maxxaxis, ref value))
                {
                }
            }
        }

        //THIS TOOL SHOULD BE IMPROVED
        private bool _optimizerdone;
        public bool OptimizerDone
        {
            get { return _optimizerdone; }
            set
            {
                _optimizerdone = value;
            }
        }
    
        public bool DisplayLine()
        {
            if (ChartValues.Any() && OptimizerDone)
            {
                return true;
            }
            return false;
        }

        private double _finaloptimizedvalue;
        public double FinalOptimizedValue
        {
            get { return _finaloptimizedvalue; }
            set
            {
                if(CheckPropertyChanged<double>("FinalOptimizedValue", ref _finaloptimizedvalue, ref value))
                {
                    FinalOptimizedValueString = String.Format("{0:0.00}", FinalOptimizedValue);
                }
            }
        }

        //There has to be a better way
        private string _finaloptimizedvaluestring;
        public string FinalOptimizedValueString
        {
            get { return _finaloptimizedvaluestring; }
            set
            {
                if (CheckPropertyChanged<string>("FinalOptimizedValueString", ref _finaloptimizedvaluestring, ref value))
                {
                }
            }
        }

        public void UpdateHeightFullScreen()
        {
            GraphGridHeight = Window.MainGrid.ActualHeight - (2 * this.margin);
        }


        public void UpdateHeightHalfScreen()
        {
            GraphGridHeight = (Window.MainGrid.ActualHeight - this.space - (2 * this.margin)) * 0.5;
        }

        //forces graph to update when user clicks in stepper multiple times
        //otherwise all the steps happen at the end and it is confusing
        public void ForceGraphUpdate()
        {
            Window.Dispatcher.BeginInvoke(
                       (Action)(() => { Graph.Update(true, true); }));


            //System.Windows.Application.Current.Dispatcher.Invoke(
            //           (Action)(() => { Graph.Update(true, true); }));
        }

        #region Visibility

        //GRAPH VISIBILITY
        //Disables graph visibility when you don't want to see it (checkbox option)
        private Visibility _graphVisibility;
        public Visibility GraphVisibility
        {
            get
            {
                return _graphVisibility;
            }
            set
            {
                CheckPropertyChanged<Visibility>("GraphVisibility", ref _graphVisibility, ref value);
            }
        }

        private Visibility _chartlinevisiblity;
        public Visibility ChartLineVisibility
        {
            get { return _chartlinevisiblity; }
            set
            {
                if (CheckPropertyChanged<Visibility>("ChartLineVisibility", ref _chartlinevisiblity, ref value))
                {
                }
            }
        }

        private Visibility _graphaxislabelsvisibility;
        public Visibility GraphAxisLabelsVisibility
        {
            get { return _graphaxislabelsvisibility; }
            set
            {
                if (CheckPropertyChanged<Visibility>("GraphAxisLabelsVisibility", ref _graphaxislabelsvisibility, ref value))
                {
                }
            }
        }

        public object DispatcherHelper { get; private set; }
        #endregion

    }
}
