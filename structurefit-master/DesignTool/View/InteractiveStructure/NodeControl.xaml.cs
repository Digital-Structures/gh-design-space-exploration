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
using DesignTool.View.InteractiveStructure;
using DesignTool.DesignModes;

namespace DesignTool.View
{
    public partial class NodeControl : UserControl, IControl
    {
        public NodeControl(NodeVM nvm, IDesignMode_Interactive i)
        {
            InitializeComponent();
            ((PointConverter)NodeCanvas.Resources["PointConverter"]).WorkingMode = i;
            ((ListenerColorConverter)NodeCanvas.Resources["ListenerColorConverter"]).WorkingMode = i;
            ((BoolVisModeConverter)NodeCanvas.Resources["BoolVisModeConverter"]).WorkingMode = i;
            ((NodeSizeConverter)NodeCanvas.Resources["NodeSizeConverter"]).WorkingMode = i;
            ((RangeConverter)NodeCanvas.Resources["RangeConverter"]).WorkingMode = i;
            ((MarginConverter)NodeCanvas.Resources["MarginConverter"]).WorkingMode = i;
            ((ConstraintVisConverter)NodeCanvas.Resources["ConstraintVisConverter"]).WorkingMode = i;
            ((NodeColorConverter)NodeCanvas.Resources["NodeColorConverter"]).WorkingMode = i;
            
            this.Mode = i;
            ViewModel = nvm;
            DrawLoads();
            DrawVariables();
            DrawPins();

            ControlViewModel = new ControlVM();
        }

        public ControlVM ControlViewModel
        {
            get;
            set;
        }

        public NodeVM ViewModel 
        { 
            get; 
            private set; 
        }

        private IDesignMode_Interactive Mode;

        private void DrawLoads()
        {
            Path pathx = new Path();
            PathGeometry px = DrawUtility.DrawArrow(this.myEllipse.Center, 270, 20, 1.5);
            pathx.Data = px;
            pathx.Stroke = new SolidColorBrush(Colors.DarkGray);
            pathx.Fill = new SolidColorBrush(Colors.DarkGray);
            LoadXCanvas.Children.Add(pathx);

            Path pathy = new Path();
            PathGeometry py = DrawUtility.DrawArrow(this.myEllipse.Center, 180, 20, 1.5);
            pathy.Data = py;
            pathy.Stroke = new SolidColorBrush(Colors.DarkGray);
            pathy.Fill = new SolidColorBrush(Colors.DarkGray);
            LoadYCanvas.Children.Add(pathy);
        }

        private void DrawVariables()
        {
            PathGeometry px1 = DrawUtility.DrawVariable(new Point(0, 0), 90, 1);
            Path pathx1 = new Path() { Data = px1 };
            PathGeometry px2 = DrawUtility.DrawVariable(new Point(0, 0), 270, 1);
            Path pathx2 = new Path() { Data = px2 };
            PathGeometry py1 = DrawUtility.DrawVariable(new Point(0, 0), 0, 1);
            Path pathy1 = new Path() { Data = py1 };
            PathGeometry py2 = DrawUtility.DrawVariable(new Point(0, 0), 180, 1);
            Path pathy2 = new Path() { Data = py2 };

            foreach (Path p in new Path[] { pathx1, pathx2, pathy1, pathy2 })
            {
                p.Stroke = new SolidColorBrush(new Color() { R = 0, G = 191, B = 255, A = 255 }); // FF00BFFF
                p.StrokeThickness = 1;
                p.Fill = new SolidColorBrush(Colors.Transparent);
            }

            VarXCanvas.Children.Add(pathx1);
            VarXCanvas.Children.Add(pathx2);
            VarYCanvas.Children.Add(pathy1);
            VarYCanvas.Children.Add(pathy2);
        }

        private void DrawPins()
        {
            PathGeometry px = DrawUtility.DrawSupport(new Point(0, 0), 270, 2);
            Path pathx = new Path() { Data = px };
            PathGeometry py = DrawUtility.DrawSupport(new Point(0, 0), 180, 2);
            Path pathy = new Path() { Data = py };

            foreach (Path p in new Path[] { pathx, pathy })
            {
                p.Fill = new SolidColorBrush(Colors.LightGray);
                p.StrokeThickness = 1;
                p.Stroke = new SolidColorBrush(Colors.Transparent);
            }

            PinXCanvas.Children.Add(pathx);
            PinYCanvas.Children.Add(pathy);
        }

        private void NodeEllipse_MouseEnter(object sender, MouseEventArgs e)
        {
            Mode.OnNodeHover(this);
        }

        private void NodeEllipse_MouseLeave(object sender, MouseEventArgs e)
        {
            Mode.OnNodeLeave(this);
        }
    }

    #region value converters

    public class NodeColorConverter : IValueConverter
    {
        public IDesignMode_Interactive WorkingMode;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //NodeVM node = (NodeVM)value;
            bool[] boolArray = (bool[])value;
            if (WorkingMode is Setup)
            {
                if (boolArray[0])
                {
                    return new SolidColorBrush(Colors.LightGray);
                }
                else if (boolArray[1] || boolArray[2])
                {
                    return new SolidColorBrush(Color.FromArgb(255, 247, 114, 96));
                    //return new SolidColorBrush(Colors.Green);
                }
                else
                {
                    return new SolidColorBrush(Colors.Black);
                }
            }
            else if (WorkingMode is Refine)
            {
                return boolArray[0] ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.Black);
                //return (node.IsListener || (!node.FreeY && !node.FreeX)) ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.Black);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ListenerColorConverter : IValueConverter
    {
        public IDesignMode_Interactive WorkingMode;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //NodeVM node = (NodeVM)value;
            bool isListener = (bool)value;
            if (WorkingMode is Setup)
            {
                return isListener ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.Black);
            }
            else if (WorkingMode is Refine)
            {
                return isListener ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.Black);
                //return (node.IsListener || (!node.FreeY && !node.FreeX)) ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.Black);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ZListenerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? -1 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class NodeSizeConverter : IValueConverter
    {
        public IDesignMode_Interactive WorkingMode;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            NodeVM node = (NodeVM)value;
            if (WorkingMode is Setup)
            {
                return 4;
            }
            else if (WorkingMode is Refine)
            {
                return (node.IsListener || (!node.FreeY && !node.FreeX)) ? 4 : 6;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class RangeConverter : IValueConverter
    {
        public IDesignMode_Interactive WorkingMode;

        /// <summary>
        /// Convert model coordinate to view coordinate
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                double model = (double)value;
                double multiple;
                if (Double.TryParse((string)parameter, out multiple))
                {
                    if (model != 0)
                    {
                        return multiple * model * WorkingMode.Zoom;
                    }
                    return .25;
                }
            }
            return .25;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolVisModeConverter : IValueConverter
    {
        public IDesignMode_Interactive WorkingMode;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (WorkingMode is Setup)
            {
                bool istrue = (bool)value;
                return istrue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class LoadDrawConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double load = (double)value;
            return load != 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class LoadTransformConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double load = (double)value;
            return load > 0 ? 0 : 180;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MarginConverter : IValueConverter
    {
        public IDesignMode_Interactive WorkingMode;

        /// <summary>
        /// Convert model coordinate to view coordinate
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double?[] range = (double?[])value;
            double marginx, marginy;
            marginx = (range[0] == null || range[0] == 0) ? -0.125 : (double)range[0] * WorkingMode.Zoom * (-1);
            marginy = (range[1] == null || range[1] == 0) ? -0.125 : (double)range[1] * WorkingMode.Zoom * (-1);
            return new Thickness(marginx, marginy, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConstraintVisConverter : IValueConverter
    {
        public IDesignMode_Interactive WorkingMode;

        /// <summary>
        /// Convert model coordinate to view coordinate
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (WorkingMode is Setup)
            {
                double?[] range = (double?[])value;
                bool isvar = false;
                if (range[0] > 0 || range[1] > 0)
                {
                    isvar = true;
                }
                return isvar ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
