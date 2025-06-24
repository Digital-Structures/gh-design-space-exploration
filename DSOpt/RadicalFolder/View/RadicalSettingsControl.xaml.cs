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
using System.ComponentModel;
using NLoptNet;
using Radical;
using System.Diagnostics;

namespace DSOptimization
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class RadicalSettingsControl : BaseControl
    {
        public RadicalSettingsControl() : base()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR
        public RadicalSettingsControl(RadicalVM radvm) : base(radvm)
        {
            this.RadicalVM = radvm;
            InitializeComponent();
        }
        public RadicalVM RadicalVM;

        //SELECTION CHANGED
        //Determines whether a secondary algorithm is required for new selected opt. alg.
        //secondary algorithm option w/ icon disappears
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            List<NLoptAlgorithm> ReqSec = RadicalVM.DFreeAlgs_ReqSec.ToList();
            if (!RadicalVM.AvailableAlgs.Contains(RadicalVM.PrimaryAlgorithm))
            {
                this.PrimaryAlgorithm.SelectedItem = RadicalVM.AvailableAlgs.ElementAt(0);
            }

            if (this.SecondaryAlgorithm != null)
            {
                if (!ReqSec.Contains((NLoptAlgorithm)box.SelectedItem))
                {
                    this.SecondaryAlgorithm.Visibility = Visibility.Collapsed;
                    this.SecAlgIcon.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.SecondaryAlgorithm.Visibility = Visibility.Visible;
                    this.SecAlgIcon.Visibility = Visibility.Visible;
                }
            }
        }

        private void RefreshMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ComboBox box = sender as ComboBox;
            //this.RadicalVM.GraphRefreshMode(box.SelectedIndex);
        }

        //REFRESH MODE
        [TypeConverter(typeof(EnumDescriptionTypeConverter))]
        public enum RefreshMode
        {
            [Description("Live Geometry and Data")] Live = 1,
            [Description("Live Data")] Data = 2,
            [Description("Silent")] Silent = 3
        }

        private void MoreInformation_Click(object sender, RoutedEventArgs e)
        {
            MoreInformationButton.Visibility = Visibility.Collapsed;
            LessInformationButton.Visibility = Visibility.Visible;
        }

        private void LessInformation_Click(object sender, RoutedEventArgs e)
        {
            MoreInformationButton.Visibility = Visibility.Visible;
            LessInformationButton.Visibility = Visibility.Collapsed;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
