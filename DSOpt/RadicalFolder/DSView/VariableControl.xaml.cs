using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
