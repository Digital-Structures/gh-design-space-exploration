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
using StructureEngine.Grammar;
using DesignTool.DesignModes;
using StructureEngine.Model;

namespace DesignTool.View.StaticStructure
{
    public partial class DesignControl : UserControl
    {
        public DesignControl(IDesignVM s, double width, double height, bool clickable, bool analyze, IDesignMode parent)
        {
            this.ViewModel = s;
            this.MyParent = parent;
            this.MyDrawer = ViewModel.GetDrawer(this);
            this.MyBehavior = MyParent.GetBehavior(this);
            this.Selected = false;
            this.IsGrammar = s.IsGrammar;
            this.Clickable = clickable;

            InitializeComponent();

            // set up drawing parameters
            this.MyWidth = width;
            this.MyHeight = height;
            this.ScaleRef = Math.Min(MyWidth, MyHeight) / 500.0;
            this.MyPadding = 0.3 * Math.Min(MyWidth, MyHeight);
            this.HoverPadding = 0.2 * MyPadding;
            double modelWidth = ViewModel.Width == 0 ? 0.0001 : ViewModel.Width;
            double modelHeight = ViewModel.Height == 0 ? 0.0001 : ViewModel.Height;
            this.Scale = Math.Min((MyWidth - MyPadding) / modelWidth, (MyHeight - MyPadding) / modelHeight);
            this.WidthSlack = (MyWidth - 2 * MyPadding) - Scale * modelWidth;
            this.HeightSlack = (MyHeight - 2 * MyPadding) - Scale * modelHeight;

            // if this is an analyzed / analyzable structure, read / compute its score
            // and set this on the converter
            if (analyze)
            {
                ((ScoreConverter)Picture.Resources["ScoreConverter"]).StaticScore = ViewModel.Score;
            }
            ((ScoreConverter)Picture.Resources["ScoreConverter"]).StringFormat = MyDrawer.GetStringFormat();

            // set up xaml
            //score.Margin = new Thickness(0, 0, 0, HoverPadding/2);
            Picture.Width = width;
            Picture.Height = height;
            ScoreRec.Height = HoverPadding * 2;
            SetBaseButton.Margin = new Thickness(MyHeight * 0.1, MyHeight * 0.1, 0, 0);
            SetRefButton.Margin = new Thickness(0, MyHeight * 0.1, MyHeight * 0.1, 0);

            // draw specific shape and the hover rectangle
            this.DrawShape();
            this.DrawHoverPath();
        }

        // public properties
        public IDesignVM ViewModel
        {
            get;
            private set;
        }
        public IDrawer MyDrawer
        {
            get;
            set;
        }
        public IModeBehavior MyBehavior
        {
            get;
            set;
        }
        public Path HoverPath;
        public IDesignMode MyParent
        {
            get;
            set;
        }
        public double ScaleRef;
        public bool Selected, Clickable;

        // private properties
        private double Scale, MyPadding, WidthSlack, HeightSlack, MyWidth, MyHeight, HoverPadding;
        public bool IsGrammar;


        // private internal methods
        private Point DrawCoords(double x, double y)
        {
            double[] zero = ViewModel.Model.ZeroPoint;
            double drawx = Scale * (x - zero[0]) + this.MyPadding + WidthSlack / 2.0;
            double drawy = Scale * (-1 * (y - zero[1]) + ViewModel.Model.Dimensions[1]) + this.MyPadding + HeightSlack / 2.0;
            return new Point(drawx, drawy);
        }
        private void DrawHoverPath()
        {
            RectangleGeometry r = new RectangleGeometry();
            r.Rect = new Rect(HoverPadding, HoverPadding, MyWidth - 2 * HoverPadding, MyHeight - 2 * HoverPadding);
            r.RadiusX = 8;
            r.RadiusY = 8;
            this.HoverPath = new Path();
            HoverPath.Data = r;
            HoverPath.StrokeThickness = 4;
            HoverPath.Opacity = 1.0;
            HoverPath.Stroke = new SolidColorBrush(Colors.Transparent);
            Picture.Children.Add(HoverPath);
        }
        private void DrawShape()
        {
            List<Path> paths = MyDrawer.DrawShape();
            foreach (Path p in paths)
            {
                if (p.Data != null)
                {
                    Picture.Children.Add(p);
                }
            }
        }

        // private event handlers
        private void Picture_MouseEnter(object sender, MouseEventArgs e)
        {
            MyBehavior.DoHover();
        }
        private void Picture_MouseLeave(object sender, MouseEventArgs e)
        {
            MyBehavior.DoLeaveHover();
        }
        private void Picture_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement clickedObject = e.OriginalSource as FrameworkElement;
            if (!(clickedObject is Image))
            {
                MyBehavior.DoClick();
            }
        }
        private void SetBaseButton_Click(object sender, RoutedEventArgs e)
        {
            //if (this.MyParent is Generate)
            //{
            //    this.Selected = !this.Selected;
            //}
            //else if (this.MyParent is Refine)
            //{
            //    this.Selected = false;
            //}
            MyParent.SetBaseStructure(this);
            MyBehavior.DoLeaveHover();
        }
        private void SetRefButton_Click(object sender, RoutedEventArgs e)
        {
            //this.Selected = !this.Selected;
            MyParent.SetRefineStructure(this);
            MyParent.MyParent.refine.IsEnabled = true;
            MyParent.MyParent.refine.IsSelected = true;
            MyBehavior.DoLeaveHover();
        }

        #region public drawing utility methods
        public GeometryGroup DrawLines(GeometryGroup group, IEnumerable<Member> members)
        {
            foreach (Member s in members)
            {
                LineGeometry lg = new LineGeometry();
                lg.StartPoint = DrawCoords(s.NodeI.DOFs[0].Coord, s.NodeI.DOFs[1].Coord);
                lg.EndPoint = DrawCoords(s.NodeJ.DOFs[0].Coord, s.NodeJ.DOFs[1].Coord);
                group.Children.Add(lg);
            }
            return group;
        }
        public GeometryGroup DrawPoints(GeometryGroup group, IEnumerable<Node> nodes, double radius)
        {
            foreach (Node p in nodes)
            {
                EllipseGeometry e = new EllipseGeometry();
                e.Center = DrawCoords(p.DOFs[0].Coord, p.DOFs[1].Coord);
                e.RadiusX = radius;
                e.RadiusY = radius;
                group.Children.Add(e);
            }
            return group;
        }
        public GeometryGroup DrawLines(GeometryGroup group, IEnumerable<ShapeLine> lines)
        {
            foreach (ShapeLine s in lines)
            {
                LineGeometry lg = new LineGeometry();
                lg.StartPoint = DrawCoords(s.Start.X, s.Start.Y);
                lg.EndPoint = DrawCoords(s.End.X, s.End.Y);
                group.Children.Add(lg);
            }
            return group;
        }
        public GeometryGroup DrawPoints(GeometryGroup group, IEnumerable<ShapePoint> points, double radius)
        {
            foreach (ShapePoint p in points)
            {
                EllipseGeometry e = new EllipseGeometry();
                e.Center = DrawCoords(p.X, p.Y);
                e.RadiusX = radius;
                e.RadiusY = radius;
                group.Children.Add(e);
            }
            return group;
        }
        public GeometryGroup DrawAreas(GeometryGroup group, IEnumerable<ShapeArea> areas)
        {
            foreach (ShapeArea a in areas)
            {
                PathGeometry pg = new PathGeometry();
                PathFigure p = new PathFigure();
                int numpoints = a.Points.Count;
                p.StartPoint = DrawCoords(a.Points[0].X, a.Points[0].Y);
                for (int i = 1; i < numpoints; i++)
                {
                    LineSegment line = new LineSegment();
                    line.Point = DrawCoords(a.Points[i].X, a.Points[i].Y);
                    p.Segments.Add(line);
                }
                LineSegment lastline = new LineSegment() { Point = DrawCoords(a.Points[0].X, a.Points[0].Y) };
                p.Segments.Add(lastline);
                pg.Figures.Add(p);
                group.Children.Add(pg);
            }
            return group;
        }
        public GeometryGroup DrawLoads(Geometry goup, IEnumerable<Node> nodes)
        {
            GeometryGroup loads = new GeometryGroup();
            double length = 2;
            double scale = 1;
            double rot;
            foreach (Node n in nodes)
            {
                Point p = DrawCoords(n.DOFs[0].Coord, n.DOFs[1].Coord);
                for (int i = 0; i < n.DOFs.Length; i++)
                {
                    DOF d = n.DOFs[i];
                    StructureVM vm = (StructureVM)ViewModel;
                    double load = vm.CurrentLoadCase.GetLoad(d).Value;
                    if (load != 0)
                    {
                        if (load > 0 && i == 0)
                        {
                            rot = 270;
                        }
                        else if (load < 0 && i == 0)
                        {
                            rot = 90;
                        }
                        else if (load > 0 && i == 1)
                        {
                            rot = 180;
                        }
                        else
                        {
                            rot = 0;
                        }
                        PathGeometry path = DrawUtility.DrawArrow(p, rot, length, scale);
                        loads.Children.Add(path);
                    }
                }
            }

            return loads;
        }
        public GeometryGroup DrawVars(Geometry goup, IEnumerable<Node> nodes)
        {
            GeometryGroup vars = new GeometryGroup();
            double scale = 1;
            double rot;
            foreach (Node n in nodes)
            {
                Point p = DrawCoords(n.DOFs[0].Coord, n.DOFs[1].Coord);
                for (int i = 0; i < n.DOFs.Length; i++)
                {
                    DOF d = n.DOFs[i];
                    if (d.Free)
                    {
                        if (i == 0)
                        {
                            rot = 90;
                        }
                        else
                        {
                            rot = 0;
                        }
                        PathGeometry path1 = DrawUtility.DrawVariable(p, rot, scale);
                        PathGeometry path2 = DrawUtility.DrawVariable(p, rot + 180, scale);
                        vars.Children.Add(path1);
                        vars.Children.Add(path2);
                    }
                }
            }

            return vars;
        }
        public GeometryGroup DrawPins(Geometry goup, IEnumerable<Node> nodes)
        {
            GeometryGroup vars = new GeometryGroup();
            double scale = 1;
            double rot;
            foreach (Node n in nodes)
            {
                Point p = DrawCoords(n.DOFs[0].Coord, n.DOFs[1].Coord);
                for (int i = 0; i < n.DOFs.Length; i++)
                {
                    DOF d = n.DOFs[i];
                    if (d.Pinned)
                    {
                        if (i == 0)
                        {
                            rot = 270;
                        }
                        else
                        {
                            rot = 180;
                        }
                        PathGeometry path = DrawUtility.DrawSupport(p, rot, scale);
                        vars.Children.Add(path);
                    }
                }
            }

            return vars;
        }
        #endregion

        private void SetBaseButton_MouseEnter(object sender, MouseEventArgs e)
        {
            SetBaseButton.Opacity = 0.9;
        }

        private void SetRefButton_MouseEnter(object sender, MouseEventArgs e)
        {
            SetRefButton.Opacity = 0.9;
        }

        private void SetBaseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            SetBaseButton.Opacity = 0.6;
            SetRefButton.Opacity = 0.6;
        }




    }
}
