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
using MaterialDesignThemes;

namespace Stepper
{
    /// <summary>
    /// Interaction logic for ObjectiveControl.xaml
    /// </summary>
    public partial class ObjectiveControl : UserControl
    {
        private int test;

        public ObjectiveControl()
        {
            test = 4; //this is a debug test
            InitializeComponent();
        }
    }
}
