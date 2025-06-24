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
using DesignTool.DesignModes;

namespace DesignTool.View.InteractiveStructure
{
    public partial class ComputedMemberControl : UserControl, IControl
    {
        public ComputedMemberControl(ComputedMemberVM mvm, IDesignMode_Interactive i)
        {
            InitializeComponent();
            ((CoordConverter)CompMemberCanvas.Resources["CoordConverter"]).WorkingMode = i;
            ((PointConverter)CompMemberCanvas.Resources["PointConverter"]).WorkingMode = i;
            ((ThicknessConverter)CompMemberCanvas.Resources["ThicknessConverter"]).WorkingMode = i;
            ((ColorConverter)CompMemberCanvas.Resources["ColorConverter"]).Type = mvm.StructType;
            //((ForceColorConverter)CompMemberCanvas.Resources["ForceColorConverter"]).EnvDisplay = mvm.Structure.EnvelopeDisplay;

            this.DataContext = mvm;
            ViewModel = mvm;
            ControlViewModel = new ControlVM();
        }

        public ControlVM ControlViewModel
        {
            get;
            set;
        }

        public ComputedMemberVM ViewModel
        {
            get;
            private set;
        }

        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            //LayoutRoot.Opacity = 0.6;
        }

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            //LayoutRoot.Opacity = 0.5;
        }
           
    }
}
