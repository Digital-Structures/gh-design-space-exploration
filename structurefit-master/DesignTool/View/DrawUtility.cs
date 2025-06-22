using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DesignTool.View
{
    public class DrawUtility
    {
        public static PathGeometry DrawArrow(Point p, double rotation, double length, double scale)
        {
            PathGeometry arrow = new PathGeometry();

            PolyLineSegment triangle = new PolyLineSegment();
            triangle.Points.Add(new Point(p.X - 3, p.Y - 10));
            triangle.Points.Add(p);
            triangle.Points.Add(new Point(p.X + 3, p.Y - 10));
            triangle.Points.Add(new Point(p.X, p.Y - 10));

            PolyLineSegment line = new PolyLineSegment();
            line.Points.Add(new Point(p.X, p.Y - 10));
            line.Points.Add(new Point(p.X, p.Y - 10 - length));

            PathFigure pathfig1 = new PathFigure();
            pathfig1.StartPoint = new Point(p.X, p.Y - 10);
            pathfig1.Segments.Add(triangle);

            PathFigure pathfig2 = new PathFigure();
            pathfig2.StartPoint = new Point(p.X, p.Y - 10);
            pathfig2.Segments.Add(line);

            arrow.Figures.Add(pathfig1);
            arrow.Figures.Add(pathfig2);

            TransformGroup transform = new TransformGroup();
            TranslateTransform trans = new TranslateTransform();
            trans.Y = -10;
            transform.Children.Add(trans);
            RotateTransform rot = new RotateTransform();
            rot.CenterX = p.X;
            rot.CenterY = p.Y;
            rot.Angle = rotation;
            transform.Children.Add(rot);
            ScaleTransform sc = new ScaleTransform();
            sc.CenterX = p.X;
            sc.CenterY = p.Y;
            sc.ScaleX = scale;
            sc.ScaleY = scale;
            transform.Children.Add(sc);
            arrow.Transform = transform;
            return arrow;
        }

        public static PathGeometry DrawVariable(Point p, double rotation, double scale)
        {
            PathGeometry variable = new PathGeometry();

            PolyLineSegment triangle = new PolyLineSegment();
            triangle.Points.Add(p);
            triangle.Points.Add(new Point(p.X + 3, p.Y + 3));

            PathFigure pathfig1 = new PathFigure();
            pathfig1.StartPoint = new Point(p.X - 3, p.Y + 3);
            pathfig1.Segments.Add(triangle);
            variable.Figures.Add(pathfig1);

            TransformGroup transform = new TransformGroup();
            TranslateTransform trans = new TranslateTransform();
            trans.Y = -10;
            transform.Children.Add(trans);
            RotateTransform rot = new RotateTransform();
            rot.CenterX = p.X;
            rot.CenterY = p.Y;
            rot.Angle = rotation;
            transform.Children.Add(rot);
            ScaleTransform sc = new ScaleTransform();
            sc.CenterX = p.X;
            sc.CenterY = p.Y;
            sc.ScaleX = scale;
            sc.ScaleY = scale;
            transform.Children.Add(sc);
            variable.Transform = transform;

            return variable;
        }

        public static PathGeometry DrawSupport(Point p, double rotation, double scale)
        {
            PathGeometry arrow = new PathGeometry();

            PolyLineSegment triangle = new PolyLineSegment();
            triangle.Points.Add(p);
            triangle.Points.Add(new Point(p.X + 4, p.Y - 6));

            PathFigure pathfig1 = new PathFigure();
            pathfig1.StartPoint = new Point(p.X - 4, p.Y - 6);
            pathfig1.Segments.Add(triangle);

            arrow.Figures.Add(pathfig1);

            TransformGroup transform = new TransformGroup();
            TranslateTransform trans = new TranslateTransform();
            trans.Y = -3;
            transform.Children.Add(trans);
            RotateTransform rot = new RotateTransform();
            rot.CenterX = p.X;
            rot.CenterY = p.Y;
            rot.Angle = rotation;
            transform.Children.Add(rot);
            ScaleTransform sc = new ScaleTransform();
            sc.CenterX = p.X;
            sc.CenterY = p.Y;
            sc.ScaleX = scale;
            sc.ScaleY = scale;
            transform.Children.Add(sc);
            arrow.Transform = transform;
            return arrow;
        }
    }
}
