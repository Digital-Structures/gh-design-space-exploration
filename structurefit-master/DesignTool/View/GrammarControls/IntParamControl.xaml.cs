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
using StructureEngine.Grammar;
using System.Windows.Data;
using DesignTool.DesignModes;

namespace DesignTool.View.GrammarControls
{
    public partial class IntParamControl : UserControl
    {
        public IntParamControl(SetupParamControl p, IntParameter rp)
        {
            InitializeComponent();
            this.MyParent = p;
            this.Model = rp;
            this.DrawControl();
        }

        private SetupParamControl MyParent;
        private IntParameter Model;

        private void DrawControl()
        {
            LayoutRoot.Children.Add(new TextBlock() {Text = "Minimum"});
            LayoutRoot.Children.Add(this.AddSlider(Model.Min));
            LayoutRoot.Children.Add(new TextBlock() {Text = "Maximum"});
            LayoutRoot.Children.Add(this.AddSlider(Model.Max));
        }

        private StackPanel AddSlider(int n)
        {
            Slider s = new Slider();
            s.Minimum = 0;
            s.Maximum = 2 * n;
            s.Value = n;
            s.Width = 100;
            TextBlock t = new TextBlock();
            t.Margin = new Thickness(5, 0, 0, 0);
            t.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Binding b = new Binding();
            b.Source = s;
            b.Path = new PropertyPath("Value");
            b.Converter = new StringFormatConverter();
            b.ConverterParameter = "{0:0}";
            t.SetBinding(TextBlock.TextProperty, b);

            StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
            sp.Children.Add(s);
            sp.Children.Add(t);

            return sp;
        }
    }
}
