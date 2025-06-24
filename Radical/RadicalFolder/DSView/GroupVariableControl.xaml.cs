using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DSOptimization
{
    /// <summary>
    /// Interaction logic for GlobalVariableControl.xaml
    /// </summary>
    public partial class GroupVariableControl : BaseControl
    {
        public enum Direction { X, Y, Z };

        public GroupVariableControl()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR
        public GroupVariableControl(GroupVarVM gvarvm):base(gvarvm)
        {
            InitializeComponent();
        }

        //CHECK BOX CHECKED
        protected void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (VarVM var in ((GroupVarVM)this.MyVM).MyVars)
                var.IsActive = true;
        }

        //CHECK BOX UNCHECKED
        protected void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (VarVM var in ((GroupVarVM)this.MyVM).MyVars)
                var.IsActive = false;
        }

        //OPTIMIZATION STARTED
        public void OptimizationStarted()
        {
            ((GroupVarVM)this.MyVM).OptimizationStarted();
        }

        //OPTIMIZATION STARTED
        public void OptimizationFinished()
        {
            ((GroupVarVM)this.MyVM).OptimizationFinished();
        }
    }
}
