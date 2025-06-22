using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DesignTool.ViewModel;
using DesignTool.View.InteractiveStructure;
using DesignTool.DesignModes;

namespace DesignTool.View
{
    public partial class MemberControl : UserControl, IControl
    {
        public MemberControl(MemberVM mvm, IDesignMode_Interactive i)
        {
            InitializeComponent();
            ViewModel = mvm;

            ((CoordConverter)MemberCanvas.Resources["CoordConverter"]).WorkingMode = i;
            ((ColorConverter)MemberCanvas.Resources["ColorConverter"]).Type = ViewModel.StructType;

            ControlViewModel = new ControlVM();
        }

        public ControlVM ControlViewModel
        {
            get;
            set;
        }

        public IControlVM IViewModel
        {
            get
            {
                return ViewModel;
            }
        }

        public MemberVM ViewModel 
        { 
            get; 
            private set; 
        }

        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            LayoutRoot.StrokeThickness = 3;
        }

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            LayoutRoot.StrokeThickness = 2;
        }
            
    }
}
