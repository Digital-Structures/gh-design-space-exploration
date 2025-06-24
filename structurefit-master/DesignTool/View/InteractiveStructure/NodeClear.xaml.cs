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
using System.Windows.Data;
using DesignTool.View.InteractiveStructure;
using DesignTool.DesignModes;

namespace DesignTool.View
{
    public partial class NodeClear : UserControl, IControl
    {
        public NodeClear(NodeVM nvm, IDesignMode_Interactive i)
        {
            InitializeComponent();
            ((PointConverter)NodeCanvas.Resources["PointConverter"]).WorkingMode = i;

            this.Mode = i;
            ViewModel = nvm;

            ControlViewModel = new ControlVM();
        }

        public ControlVM ControlViewModel
        {
            get;
            set;
        }

        public NodeVM ViewModel
        {
            get;
            private set;
        }

        private IDesignMode_Interactive Mode;
    }
}

