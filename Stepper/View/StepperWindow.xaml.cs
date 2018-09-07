using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
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
using System.Threading;
using System.Windows.Markup;
using System.Globalization;
using System.Reflection;
using LiveCharts;
using LiveCharts.Wpf;
using MaterialDesignThemes;

namespace Stepper
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class StepperWindow : Window
    {
        //Variables and properties
        private StepperVM StepperVM;
        private List<ObjectiveControl> Objectives; 

        public StepperWindow():base()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR
        public StepperWindow(StepperVM svm) : base()
        {
            //Bind property data through the StepperVM
            this.StepperVM = svm;
            this.DataContext = svm;
            InitializeComponent();

            //Create a list of ObjectiveControl UI objects for each objective
            this.Objectives = new List<ObjectiveControl>();
            foreach (List<double> objective in this.StepperVM.Objectives)
                this.Objectives.Add(new ObjectiveControl());

            ConfigureDisplay();
        }

        //CONFIGURE DISPLAY
        private void ConfigureDisplay()
        {
            foreach (ObjectiveControl objective in this.Objectives)
                this.ObjectivesPanel.Children.Add(objective);
        }

        //WINDOW CLOSING
        //Alert the VM that the window has closed, another window may open
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.StepperVM.OnWindowClosing();
        }
    }
}
