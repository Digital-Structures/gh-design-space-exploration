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
using StructureEngine.Grammar.Simple;
using DesignTool.View.StaticStructure;
using StructureEngine.Model;

namespace DesignTool.View.GrammarControls
{
    public class SimpleDrawer : IDrawer
    {
        public SimpleDrawer(DesignControl p)
        {
            this.MyParent = p;
        }

        private DesignControl MyParent;

        public string GetStringFormat()
        {
            return "${0:0,0}";
        }

        public List<Path> DrawShape()
        {
            IDesign model = (MyParent.ViewModel.Model as SimpleShape);
            if (model != null)
            {
                // set up line thicknesses
                double horWidth = Math.Max(6 * MyParent.ScaleRef, 2);
                double vertWidth = Math.Max(3 * MyParent.ScaleRef, 1);
                double funWidth = Math.Max(6 * MyParent.ScaleRef, 2);

                // set up colors
                var myBlackBrush = new SolidColorBrush(Colors.Black);
                var myLightGrayBrush = new SolidColorBrush(Colors.LightGray);
                var myRedBrush = new SolidColorBrush(Colors.Red);
                var myWhiteBrush = new SolidColorBrush(Colors.White);

                // set up geometry groups
                var horGroup = new GeometryGroup();
                var vertGroup = new GeometryGroup();
                var funGroup = new GeometryGroup();
                var dotGroup = new GeometryGroup();

                // draw line groups
                horGroup = MyParent.DrawLines(horGroup, ((SimpleShape)model).Horizontal);
                vertGroup = MyParent.DrawLines(vertGroup, ((SimpleShape)model).Verticals);
                funGroup = MyParent.DrawLines(funGroup, ((SimpleShape)model).Funicular);

                // draw point groups
                dotGroup = MyParent.DrawPoints(dotGroup, ((SimpleShape)model).HorPoints, 2 * vertWidth);

                // set up paths
                var HorPath = new Path() { Stroke = myLightGrayBrush, StrokeThickness = horWidth, Data = horGroup };
                var VertPath = new Path() { Stroke = myBlackBrush, StrokeThickness = vertWidth, Data = vertGroup };
                var FunPath = new Path() { Stroke = myBlackBrush, StrokeThickness = funWidth, Data = funGroup };
                var DotPath = new Path() { Stroke = myBlackBrush, StrokeThickness = 0, Data = dotGroup, Fill = myWhiteBrush };

                // group paths into list
                List<Path> paths = new List<Path>() { HorPath, VertPath, FunPath, DotPath};
                return paths;
            }

            return new List<Path>();
        }

    }
}
