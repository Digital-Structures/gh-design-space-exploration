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
using DesignTool.DesignModes;
using DesignTool.ViewModel;

namespace DesignTool.View.InteractiveStructure
{
    public partial class GhostMember : UserControl, IControl
    {
        public GhostMember(GhostMemberVM vm, IDesignMode_Interactive i)
        {
            this.ViewModel = vm;
            InitializeComponent();
            this.DataContext = ViewModel;

            ((PointConverter)LayoutRoot.Resources["PointConverter"]).WorkingMode = i;
            ControlViewModel = new ControlVM();
        }

        public ControlVM ControlViewModel
        {
            get;
            set;
        }

        public GhostMemberVM ViewModel
        {
            get;
            set;
        }
    }
}
