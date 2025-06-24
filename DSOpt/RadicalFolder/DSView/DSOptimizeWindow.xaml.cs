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
using Radical;
using Stepper;

namespace DSOptimization
{
    /// <summary>
    /// Interaction logic for DSOptimizeWindow.xaml
    /// </summary>
    public partial class DSOptimizeWindow : Window
    {
        public DSOptimizeWindow()
        {
            InitializeComponent();
        }

        DSOptimizerComponent MyComponent;
        RadicalWindow RadicalWindow;

        public DSOptimizeWindow(Design design, DSOptimizerComponent component)
        {
            MyComponent = component;
            InitializeComponent();

            this.StepperTab.Content = new StepperWindow(new StepperVM(design, component));
            this.RadicalWindow = new RadicalWindow(new RadicalVM(design, component));
            this.RadicalTab.Content = this.RadicalWindow;
        }

        public void Window_Closing(object sender, CancelEventArgs e)
        {
            this.RadicalWindow.WindowClosing();
            //if radical is running
            //throw stop
            this.MyComponent.IsWindowOpen = false;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
