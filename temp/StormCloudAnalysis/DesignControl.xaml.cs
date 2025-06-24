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
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using StormCloud.ViewModel;
//using StructureEngine.ViewModel;

namespace StormCloud
{
    /// <summary>
    /// Interaction logic for DesignControl.xaml
    /// </summary>
    public partial class DesignControl : UserControl
    {
        public DesignControl(DesignVM dvm, double Height, double Width, bool Clickable, IGH_Component comp)// DesignToolVM designtoolvm)//, bool Analyzed)
        {
            //dvm.Position = designtoolvm.Position;
            //dvm.LookDir = designtoolvm.LookDir;
            this.Component = (InterOptComponent)comp;
            this.myViewModel = dvm;
            //this.myViewModel.Position = designtoolvm.Position;
            //this.myViewModel.LookDir = designtoolvm.LookDir;
            this.DataContext = this.myViewModel;
            //this.DataContext = dvm;
            this.myViewModel.IsClickable = Clickable;
            InitializeComponent();
            ControlGrid.Height = Height;
            ControlGrid.Width = Width;
            this.ScoreText.Text = String.Format("{0:0.00}", dvm.Score);//dvm.Score.ToString();

        }

        public InterOptComponent Component;

        public DesignVM myViewModel
        {
            get;
            set;
        }


        private void ControlGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Border.BorderThickness = new Thickness(2);
        }

        private void ControlGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Border.BorderThickness = new Thickness(0);
        }

        private void ControlGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
                for (int i = 0; i < Component.Params.Input[2].Sources.Count; i++)
                {
                    GH_NumberSlider slider = Component.Params.Input[2].Sources[i] as GH_NumberSlider;
                    if (slider != null)
                    {
                        slider.SetSliderValue((decimal)this.myViewModel.Design.DesignVariables[i].Value);
                    }
                }
                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
            }
    }

    //public class StringFormatConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter,
    //        System.Globalization.CultureInfo culture)
    //    {
    //        string formatString = parameter as string;
    //        if (!string.IsNullOrEmpty(formatString))
    //        {
    //            string toreturn = string.Format(culture, formatString, value);
    //            return toreturn;

    //        }
    //        return value.ToString();
    //    }

    //    public object ConvertBack(object value, Type targetType,
    //        object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        // TODO: this is specific to the type of string being converted: make a DoubleFormatter converter
    //        return double.Parse((string)value);
    //    }
    //}

}
