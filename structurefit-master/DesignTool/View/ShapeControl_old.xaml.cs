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

namespace DesignTool.View
{
    public partial class ShapeControl_old : UserControl
    {
        public ShapeControl_old(Grammar.Shape_old m, double width, double height)
        {
            InitializeComponent();
            //this.DataContext = svm;
            Model = m;

            // set up drawing parameters
            double zeroX = Model.ZeroPoint[0];
            double zeroY = Model.ZeroPoint[1];
            double scaleRef = Math.Min(width / 600, height / 400);
            double padding = 100 * scaleRef;
            double modelWidth = Model.Dimensions[0];
            double modelHeight = Model.Dimensions[1];
            double scale = Math.Min((width - padding) / modelWidth, (height - padding) / modelHeight);
            double picWidth = scale * modelWidth + padding;
            double picHeight = scale * modelHeight + padding;

            // set up scales
            double towerScale = Math.Max(15 * scaleRef, 4);
            double deckScale = Math.Max(8 * scaleRef, 3);
            double infillScale = Math.Max(1 * scaleRef, 0.5);
            
            // set up colors
            SolidColorBrush myGrayBrush = new SolidColorBrush();
            myGrayBrush.Color = Colors.LightGray;
            SolidColorBrush myBlackBrush = new SolidColorBrush();
            myBlackBrush.Color = Colors.Black;

            // set up paths
            Path DeckPath = new Path();
            DeckPath.Stroke = myGrayBrush;
            DeckPath.StrokeThickness = deckScale;

            Path TowerPath = new Path();
            TowerPath.Stroke = myGrayBrush;
            TowerPath.StrokeThickness = towerScale;

            Path InfillPath = new Path();
            InfillPath.Stroke = myBlackBrush;
            InfillPath.StrokeThickness = infillScale;

            // set up geometry groups
            GeometryGroup deckGroup = new GeometryGroup();
            GeometryGroup towerGroup = new GeometryGroup();
            GeometryGroup infillGroup = new GeometryGroup();

            // draw deck
            LineGeometry deckGeometry = new LineGeometry();
            deckGeometry.StartPoint = new Point(scale * (Model.Deck.Start.X - zeroX),
                scale * (-1 * (Model.Deck.Start.Y - zeroY) + modelHeight));
            deckGeometry.EndPoint = new Point(scale * (Model.Deck.End.X - zeroX),
                scale * (-1 * (Model.Deck.End.Y - zeroY) + modelHeight));
            deckGroup.Children.Add(deckGeometry);

            // draw towers
            LineGeometry tower1Geometry = new LineGeometry();
            tower1Geometry.StartPoint = new Point(scale * (Model.Tower1.Start.X - zeroX),
                scale * (-1 * (Model.Tower1.Start.Y - zeroY) + modelHeight));
            tower1Geometry.EndPoint = new Point(scale * (Model.Tower1.End.X - zeroX),
                scale * (-1 * (Model.Tower1.End.Y - zeroY) + modelHeight));
            towerGroup.Children.Add(tower1Geometry);

            LineGeometry tower2Geometry = new LineGeometry();
            tower2Geometry.StartPoint = new Point(scale * (Model.Tower2.Start.X - zeroX),
                scale * (-1 * (Model.Tower2.Start.Y - zeroY) + modelHeight));
            tower2Geometry.EndPoint = new Point(scale * (Model.Tower2.End.X - zeroX),
                scale * (-1 * (Model.Tower2.End.Y - zeroY) + modelHeight));
            towerGroup.Children.Add(tower2Geometry);

            // draw infill
            foreach (Shape_Line s in Model.Infill)
            {
                LineGeometry infillGeometry = new LineGeometry();
                infillGeometry.StartPoint = new Point(scale * (s.Start.X - zeroX),
                    scale * (-1 * (s.Start.Y - zeroY) + modelHeight));
                infillGeometry.EndPoint = new Point(scale * (s.End.X - zeroX),
                    scale * (-1 * (s.End.Y - zeroY) + modelHeight));
                infillGroup.Children.Add(infillGeometry);
            }

            DeckPath.Data = deckGroup;
            TowerPath.Data = towerGroup;
            InfillPath.Data = infillGroup;

            foreach (Path p in new Path[] { DeckPath, TowerPath, InfillPath})
            {
                Picture.Children.Add(p);
            }

            //Picture.Width = picWidth;
            //Picture.Height = picHeight;
            //Picture.Background = new SolidColorBrush(Colors.Blue);

            TranslateTransform t = new TranslateTransform();
            t.X = (width - picWidth + padding) / 2;
            t.Y = (height - picHeight + padding) / 2;
            this.transform = t;
            //Picture.RenderTransform = transform;
        }

        public Grammar.Shape_old Model
        {
            get;
            private set;
        }

        public Transform transform
        {
            get;
            set;
        }
    }
}
