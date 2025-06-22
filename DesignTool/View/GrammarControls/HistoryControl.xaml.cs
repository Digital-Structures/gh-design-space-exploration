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
using DesignTool.View.StaticStructure;

namespace DesignTool.View
{
    public partial class HistoryControl : UserControl
    {
        public HistoryControl(DesignControl sc)
        {
            InitializeComponent();
            HistGrid.Children.Add(sc);
            Grid.SetColumn(sc, 1);
            Grid.SetRow(sc, 2);
            this.ShapePic = sc;
        }

        public DesignControl ShapePic
        {
            get;
            private set;
        }

    }
}
