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
using DesignTool.DesignModes;
using System.Windows.Data;

namespace DesignTool.View.InteractiveStructure
{
    public partial class SymmetryControl : UserControl, IControl
    {
        public SymmetryControl(StructureVM vm, IDesignMode_Interactive i, bool ver)
        {
            this.DataContext = vm;
            this.ViewModel = vm;
            this.IsVer = ver;
            InitializeComponent();

            ((PointConverter)SymGrid.Resources["PointConverter"]).WorkingMode = i;
            ((SymPointConverter)SymGrid.Resources["SymPointConverter"]).WorkingMode = i;
            ((SymPointConverter)SymGrid.Resources["SymPointConverter"]).IsVer = ver;
            this.Refresh();

            ControlViewModel = new ControlVM();
        }

        public bool IsVer
        {
            get;
            set;
        }

        public ControlVM ControlViewModel
        {
            get;
            set;
        }

        public StructureVM ViewModel
        {
            get;
            set;
        }

        public void Refresh()
        {
            ((SymPointConverter)SymGrid.Resources["SymPointConverter"]).Height = ViewModel.Height;
            ((SymPointConverter)SymGrid.Resources["SymPointConverter"]).Width = ViewModel.Width;
            ((SymPointConverter)SymGrid.Resources["SymPointConverter"]).Zero = ViewModel.Zero;
        }
    }

    public class SymPointConverter : IValueConverter
    {
        public IDesignMode_Interactive WorkingMode;
        public bool IsVer;
        public double Height;
        public double Width;
        public double[] Zero;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double?[] sym = (double?[])value;
            double x, y;
            double padding = 40;

            switch (parameter as string)
            {
                case "start":
                    if (IsVer)
                    {
                        x = WorkingMode.TransformtoView((double)sym[0], false);
                        y = WorkingMode.TransformtoView(Zero[1], true) + padding;
                    }
                    else
                    {
                        x = WorkingMode.TransformtoView(Zero[0], false) - padding;
                        y = WorkingMode.TransformtoView((double)sym[1], true);
                    }
                    break;
                case "end":
                    if (IsVer)
                    {
                        x = WorkingMode.TransformtoView((double)sym[0], false);
                        y = WorkingMode.TransformtoView(Zero[1] + Height, true) - padding;
                    }
                    else
                    {
                        x = WorkingMode.TransformtoView(Zero[0] + Width, false) + padding;
                        y = WorkingMode.TransformtoView((double)sym[1], true);
                    }
                    break;
                default:
                    throw new Exception("Must specify start or end");
            }


            return new Point(x, y);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
