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

namespace Stepper
{
    /// <summary>
    /// Interaction logic for GraphControl.xaml
    /// </summary>
    public partial class StepperGraphControl : UserControl
    {
        public StepperGraphControl()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR
        public StepperGraphControl(StepperGraphVM VM)
        {
            this.DataContext = VM;
            InitializeComponent();

            this.XAxis.LabelFormatter = val => val.ToString("n2");
            this.YAxis.LabelFormatter = val => val.ToString("n2");
        }

        public void ForceGraphUpdate()
        {
            Graph.Update(true, true);
        }
    }
}
