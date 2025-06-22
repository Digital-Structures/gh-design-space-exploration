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
using DSOptimization;

namespace Stepper
{
    //DATA CONTROL
    //Versatile UI display element for storing information about objectives or variables
    //Value is updated in StepperWindow 
    public partial class DataControl : UserControl
    {
        //Takes either an Objective VM or Variable VM
        private IStepDataElement MyData;

        public DataControl()
        {
            InitializeComponent();
        }

        public DataControl(IStepDataElement data)
        {
            this.MyData = data;

            MyData.PropertyChanged += new PropertyChangedEventHandler(VarPropertyChanged);

            this.DataContext = this;
            InitializeComponent();

            this.Value = MyData.Value;
            this.VariableName = MyData.Name;
        }

        //Property Changed event handling method
        private void VarPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.VariableName = this.MyData.Name;
        }

        //VALUE
        //Value can be a gradient, current value, or value at a previous step
        //Updated from StepperWindow.xaml.cx
        private double val;
        public double Value
        {
            get { return val; }
            set
            {
                if (value != val)
                {
                    val = value;

                    //Implement a way to do scientific notation !!! 
                    //implement it both ways

                    if(val > 99999)
                    {
                        this.ValueText.Text =  String.Format("{0:0.0e0}", val);
                    }
                    else
                    {
                        this.ValueText.Text = String.Format("{0:0.00}", val);
                    }
                }
            }
        }

        //NAME
        //Name property is tied to the name associated with the control's VM
        private string name;
        public string VariableName
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    this.VarName.Text = name;
                }

                if (this.VariableName != this.MyData.Name)
                    this.MyData.Name = this.VariableName;
            }
        }
    }
}
