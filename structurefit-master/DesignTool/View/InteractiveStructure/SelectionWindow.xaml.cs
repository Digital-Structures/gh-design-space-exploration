using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DesignTool.ViewModel;
using System.Windows.Data;
using DesignTool.DesignModes;

namespace DesignTool.View.InteractiveStructure
{
    public partial class SelectionWindow : UserControl, IControl
    {
        public SelectionWindow(SelectionWindowVM vm, IDesignMode_Interactive i)
        {
            this.ViewModel = vm;
            InitializeComponent();
            this.DataContext = ViewModel;

            ((PointConverter)LayoutRoot.Resources["PointConverter"]).WorkingMode = i;
            ((CornerPointConverter)LayoutRoot.Resources["CornerPointConverter"]).WorkingMode = i;
            ((CornerPointConverter)LayoutRoot.Resources["CornerPointConverter"]).s = ViewModel.RecStart;

            ControlViewModel = new ControlVM();
        }

        public ControlVM ControlViewModel
        {
            get;
            set;
        }

        public SelectionWindowVM ViewModel
        {
            get;
            set;
        }
    }

    public class CornerPointConverter : IValueConverter
    {
        public Point s;
        public IDesignMode_Interactive WorkingMode;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Point e = (Point)value;
            Point EndPoint = new Point(WorkingMode.TransformtoView(e.X, false), 
                WorkingMode.TransformtoView(e.Y, true));
            Point StartPoint = new Point(WorkingMode.TransformtoView(s.X, false),
                WorkingMode.TransformtoView(s.Y, true));
            Point NewPoint = new Point();
            switch (parameter as string)
            {
                case "upperright":
                    NewPoint.X = EndPoint.X;
                    NewPoint.Y = StartPoint.Y;
                    break;
                case "lowerleft":
                    NewPoint.X = StartPoint.X;
                    NewPoint.Y = EndPoint.Y;
                    break;
                default:
                    throw new Exception("Must specify uppper left or lower right");
            }
            
            return NewPoint;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
