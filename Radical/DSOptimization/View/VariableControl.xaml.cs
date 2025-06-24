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

namespace DSOptimization
{
    /// <summary>
    /// Interaction logic for DesignVariableControl.xaml
    /// </summary>

    public partial class VariableControl : BaseControl
    {
        public VariableControl():base()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR
        public VariableControl(VarVM varvm):base(varvm)
        {
            InitializeComponent();
        }
    }
}
