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
using System.Collections.Generic;
using StructureEngine.Grammar;
using DesignTool.View.StaticStructure;
using StructureEngine.Grammar.Airport;
using StructureEngine.Model;

namespace DesignTool.View
{
    public class AirportDrawer : IDrawer
    {
        public AirportDrawer(DesignControl p)
        {
            this.MyParent = p;
        }

        public string GetStringFormat()
        {
            return "{0:0.00}";
        }

        public bool ScoreVis
        {
            get;
            private set;
        }

        private DesignControl MyParent;

        public List<Path> DrawShape()
        {
            IDesign model = (MyParent.ViewModel.Model as AirportShape);
            if (model != null)
            {
                // set up line thicknesses
                double roofWidth = Math.Max(3 * MyParent.ScaleRef, 1);
                double vertWidth = Math.Max(6 * MyParent.ScaleRef, 2);

                // set up colors
                var myBlackBrush = new SolidColorBrush(Colors.Black);
                var myLightGrayBrush = new SolidColorBrush(Colors.LightGray);

                // set up geometry groups
                var roofGroup = new GeometryGroup();
                var vertGroup = new GeometryGroup();
                var dotGroup = new GeometryGroup();

                // draw line groups
                roofGroup = MyParent.DrawLines(roofGroup, ((AirportShape)model).Roof);
                vertGroup = MyParent.DrawLines(vertGroup, ((AirportShape)model).Verticals);

                // draw point groups
                dotGroup = MyParent.DrawPoints(dotGroup, ((AirportShape)model).Points, 2);

                // set up paths
                var RoofPath = new Path() { Stroke = myBlackBrush, StrokeThickness = roofWidth, Data = roofGroup };
                var VertPath = new Path() { Stroke = myLightGrayBrush, StrokeThickness = vertWidth, Data = vertGroup };
                var DotPath = new Path() { Stroke = myBlackBrush, StrokeThickness = 0, Data = dotGroup, Fill = myBlackBrush };

                // group paths into list
                List<Path> paths = new List<Path>() { RoofPath, VertPath };
                return paths;
            }

            return new List<Path>();
        }

        public void DoClick()
        {
        }

        public void DoHover()
        {
        }

        public void DoLeaveHover()
        {
        }
    }
}
