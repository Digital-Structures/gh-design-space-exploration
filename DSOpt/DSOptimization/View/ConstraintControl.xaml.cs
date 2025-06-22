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
using System.Text.RegularExpressions;
using Radical;

namespace DSOptimization
{
    /// <summary>
    /// Interaction logic for ConstraintControl.xaml
    /// </summary>
    public partial class ConstraintControl : BaseControl
    {
        public ConstraintControl():base()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR
        public ConstraintControl(ConstVM const_vm, RadicalWindow window):base(const_vm)
        {
            MyWindow = window;
            InitializeComponent();
            this.MyCheckBox.IsChecked = true;
            
        }
        private RadicalWindow MyWindow;

        //CHECK BOX CHECKED
        protected void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ((ConstVM)this.MyVM).GraphVM.UpdateHeightHalfScreen();
            ((ConstVM)this.MyVM).GraphVM.GraphVisibility = Visibility.Visible;
            this.MyWindow.UpdatedGraphVisibility();
        }

        //CHECK BOX UNCHECKED
        protected void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ((ConstVM)this.MyVM).GraphVM.GraphVisibility = Visibility.Collapsed;
            this.MyWindow.UpdatedGraphVisibility();
        }
    }
}
