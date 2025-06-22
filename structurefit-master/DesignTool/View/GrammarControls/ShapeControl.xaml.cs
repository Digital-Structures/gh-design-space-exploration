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
using DesignTool.Model;
using DesignTool.Grammar;
using DesignTool.DesignModes.GrammarMode;

namespace DesignTool.View
{
    public partial class ShapeControl : UserControl
    {
        public ShapeControl()
        {
        }

        public ShapeControl(IShape m, double width, double height, bool clickable, IGrammarMode parent)
        {
            InitializeComponent();
            this.Model = m;
            this.Selected = false;
            this.Clickable = clickable;
            this.MyParent = parent;

            // set up drawing parameters
            this.MyWidth = width;
            this.MyHeight = height;
            this.ScaleRef = Math.Min(MyWidth, MyHeight) / 500.0;
            this.MyPadding = 0.3 * Math.Min(MyWidth, MyHeight);
            double HoverPadding = 0.2 * MyPadding;
            double modelWidth = m.Dimensions[0] == 0 ? 5 : m.Dimensions[0];
            double modelHeight = m.Dimensions[1] == 0 ? 5 : m.Dimensions[1];
            this.Scale = Math.Min((MyWidth - MyPadding) / modelWidth, (MyHeight - MyPadding) / modelHeight);
            this.WidthSlack = (MyWidth - MyPadding) - Scale * modelWidth;
            this.HeightSlack = (MyHeight - MyPadding) - Scale * modelHeight;

            Picture.Width = width;
            Picture.Height = height;
            ScoreRec.Height = HoverPadding * 2;
            score.Text = "$" + Model.Score.ToString("0");

            // draw specific shape and the hover rectangle
            this.DrawShape();
            this.DrawHoverPath();
        }

        public IGrammarMode MyParent;

        public double ScaleRef
        {
            get;
            set;
        }

        public Grammar.IShape Model
        {
            get;
            private set;
        }

        public Transform transform
        {
            get;
            set;
        }

        public double Scale
        {
            get;
            set;
        }

        public double MyWidth, MyHeight;

        public double MyPadding
        {
            get;
            set;
        }

        public double WidthSlack;

        public double HeightSlack;

        public bool Selected
        {
            get;
            set;
        }

        private bool Clickable;

        public Point DrawCoords(double x, double y)
        {
            double drawx = Scale * (x - Model.ZeroShapePoint.X) + this.MyPadding / 2.0 + WidthSlack / 2.0;
            double drawy = Scale * (-1 * (y - Model.ZeroShapePoint.Y) + Model.Dimensions[1]) + this.MyPadding / 2.0 + HeightSlack / 2.0;
            return new Point(drawx, drawy);
        }

        private void Picture_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.Clickable)
            {
                if (!this.Selected)
                {
                    HoverPath.Stroke = new SolidColorBrush(Color.FromArgb(100, 0, 191, 255)); // 0, 191, 255
                }
                else
                {
                    HoverPath.Stroke = new SolidColorBrush(Color.FromArgb(170, 0, 191, 255)); // 255, 127, 36
                }
            }
        }

        private void Picture_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Clickable)
            {
                if (!this.Selected)
                {
                    HoverPath.Stroke = new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    HoverPath.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 191, 255)); // 255, 127, 36
                }
            }
        }

        private void Picture_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.Clickable)
            {
                this.Selected = !Selected;
                if (this.Selected)
                {
                    HoverPath.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 191, 255)); // 255, 127, 36
                    this.MyParent.SetSelectedControl(this);
                }
                else
                {
                    HoverPath.Stroke = new SolidColorBrush(Colors.Transparent); // 255, 127, 36
                }
            }
        }

        public Path HoverPath;

        private void DrawHoverPath()
        {
            RectangleGeometry r = new RectangleGeometry();
            r.Rect = new Rect(this.MyPadding / 4.0, this.MyPadding / 4.0, MyWidth - this.MyPadding / 2.0, MyHeight - this.MyPadding / 2.0);
            //r.Rect = new Rect(0, 0,  width, height);
            r.RadiusX = 8;
            r.RadiusY = 8;
            this.HoverPath = new Path();
            HoverPath.Data = r;
            HoverPath.StrokeThickness = 4;
            HoverPath.Stroke = new SolidColorBrush(Colors.Transparent);
            Picture.Children.Add(HoverPath);
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

        private void DrawShape()
        {
            //IDrawer drawer = this.MyParent.MyParent.Gram.GetDrawer(this);
            //List<Path> paths = drawer.DrawShape();
            //foreach (Path p in paths)
            //{
            //    Picture.Children.Add(p);
            //}
        }
    }
}
