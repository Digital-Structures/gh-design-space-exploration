//using System;
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
using LiveCharts.Helpers;
using LiveCharts.Wpf;

namespace Radical
{
    /// <summary>
    /// Interaction logic for GraphControl.xaml
    /// </summary>
    public partial class GraphControl : UserControl
    {
        public GraphVM GraphVM;
        public RadicalWindow MyWindow;
        public RadicalVM RadicalVM;

        public GraphControl()
        {
            this.DataContext = GraphVM;
            InitializeComponent();
        }

        //CONSTRUCTOR
        public GraphControl(GraphVM graphVM, RadicalVM radicalVM, RadicalWindow window)
        {
            this.RadicalVM = radicalVM;
            this.GraphVM = graphVM;
            this.DataContext = graphVM;
            this.MyWindow = window;

            InitializeComponent();

            this.GraphVM.Window = window;
            this.GraphVM.Graph = Chart;

            this.GraphVM.ChartLineVisibility = Visibility.Collapsed;

            ChartAxisY.LabelFormatter = value => value.ToString("N2");
        }

        #region Mouse_Events
        //CHART MOUSE MOVE
        //Tracks mouse movement and sets the vertical lines on the graph to be on the points closest to the mouse
        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.GraphVM.DisplayLine())
            {
                this.GraphVM.ChartLineVisibility = Visibility.Visible;
                var mouseCoordinate = e.GetPosition(Chart);
                double chartCoordinateX = Chart.ConvertToChartValues(mouseCoordinate).X;
                int closestXPoint = (int)Chart.Series[0].ClosestPointTo(chartCoordinateX, AxisOrientation.X).X;
                this.GraphVM.MouseIteration = closestXPoint;
                this.RadicalVM.UpdateGraphLines(closestXPoint);
            }
            else
            {
                this.GraphVM.ChartLineVisibility = Visibility.Collapsed;
            }
        }

        //PREVIEW MOUSE WHEEL
        //Disable scrolling to pan and zoom the chart window
        //This enables the mouse wheel for actually scrolling through the scroll viewer
        private void Graph_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;
            MyWindow.GraphsScroller.RaiseEvent(e2);
        }

        //Shows graph axis labels when mouse is over card
        private void Chart_MouseEnter(object sender, MouseEventArgs e)
        {
            this.GraphVM.GraphAxisLabelsVisibility = Visibility.Visible;
        }

        //Hides graph axis labels when mouse is not over card
        private void Chart_MouseLeave(object sender, MouseEventArgs e)
        {
            this.GraphVM.GraphAxisLabelsVisibility = Visibility.Hidden;
        }
        #endregion

        public void UpdateHeightFullScreen()
        {
            this.GraphVM.UpdateHeightFullScreen();
        }


        public void UpdateHeightHalfScreen()
        {
            this.GraphVM.UpdateHeightHalfScreen();
        }

        //forces graph to update when user clicks in stepper multiple times
        //otherwise all the steps happen at the end and it is confusing
        public void ForceGraphUpdate()
        {
            Chart.Update(true, true);
        }

    }
}
