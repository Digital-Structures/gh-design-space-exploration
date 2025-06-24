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
    public partial class ComputedMemberSimple : UserControl, IControl
    {
        public ComputedMemberSimple(ComputedMemberVM mvm, IDesignMode_Interactive i)
        {
            InitializeComponent();
            ((CoordConverter)CompMemberCanvas.Resources["CoordConverter"]).WorkingMode = i;
            ((PointConverter)CompMemberCanvas.Resources["PointConverter"]).WorkingMode = i;
            ((ThicknessConverter)CompMemberCanvas.Resources["ThicknessConverter"]).WorkingMode = i;

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
    }
}
