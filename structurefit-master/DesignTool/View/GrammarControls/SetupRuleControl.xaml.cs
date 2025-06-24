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
using DesignTool.View.StaticStructure;
using DesignTool.DesignModes;
using StructureEngine.Grammar;

namespace DesignTool.View.GrammarControls
{
    public partial class SetupRuleControl : UserControl
    {
        public SetupRuleControl(RuleVM vm, IDesignMode parent)
        {
            InitializeComponent();
            this.ViewModel = vm;
            this.MyParent = parent;
            LayoutRoot.DataContext = ViewModel;
            this.DrawArrow();
            this.DrawImages();
            this.DrawParams();
        }

        public IDesignMode MyParent;

        public RuleVM ViewModel
        {
            get;
            set;
        }

        private void DrawParams()
        {
            foreach (IRuleParameter rp in ViewModel.Params)
            {
                ParameterStack.Children.Add(new SetupParamControl(ViewModel.Params.IndexOf(rp) + 1, rp));
            }
        }

        private void DrawImages()
        {
            LHSBorder.Child = new DesignControl(ViewModel.LHS, LHSBorder.Width, LHSBorder.Height, false, false, this.MyParent);
            RHSBorder.Child = new DesignControl(ViewModel.RHS, RHSBorder.Width, RHSBorder.Height, false, false, this.MyParent);
        }

        private void DrawArrow()
        {
            Canvas arrowcan = new Canvas();
            Path arrowpath = new Path();
            GeometryGroup lines = new GeometryGroup();
            Point p1 = new Point(0, 0);
            Point p2 = new Point(15, 0);
            Point p3 = new Point(10, 5);
            Point p4 = new Point(10, -5);
            LineGeometry l1 = new LineGeometry() { StartPoint = p1, EndPoint = p2 };
            LineGeometry l2 = new LineGeometry() { StartPoint = p2, EndPoint = p3 };
            LineGeometry l3 = new LineGeometry() { StartPoint = p2, EndPoint = p4 };
            lines.Children.Add(l1);
            lines.Children.Add(l2);
            lines.Children.Add(l3);
            arrowpath.Data = lines;
            //arrowpath.RenderTransform = new ScaleTransform() { ScaleX = 1.5, ScaleY = 1.5 };
            arrowpath.RenderTransform = new TranslateTransform() { X = -7.5, Y = 0 };
            arrowpath.Stroke = new SolidColorBrush(Colors.Black);
            arrowpath.StrokeThickness = 1;
            arrowcan.Children.Add(arrowpath);
            arrowcan.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            arrowcan.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            ArrowBorder.Child = arrowcan;
        }


    }
}
