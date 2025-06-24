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
using StructureEngine.Model;
using System.Collections.Generic;
using System.Linq;

namespace DesignTool.View.StaticStructure
{
    public class TrussDrawer : IDrawer
    {
        public TrussDrawer(DesignControl dc)
        {
            this.MyParent = dc;
        }

        private DesignControl MyParent;

        public string GetStringFormat()
        {
            return "{0:0.00}";
        }

        public List<Path> DrawShape()
        {
            IDesign model = MyParent.ViewModel.Model;
            if (model != null)
            {
                // set up scales
                double lineScale = Math.Max(1 * MyParent.ScaleRef, .75);

                // set up colors
                var myBlackBrush = new SolidColorBrush(Colors.Black);
                var myMemberBrush = new SolidColorBrush(Colors.Black);
                var myLoadBrush = new SolidColorBrush(Colors.DarkGray);
                var myVariableBrush = new SolidColorBrush(Colors.Red);
                var myPinBrush = new SolidColorBrush(Colors.LightGray);
                var mySymBrush = new SolidColorBrush(Colors.Gray);
                var myGrayBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));

                // set up geometry groups
                var nodeGroup = new GeometryGroup();
                var memberGroup = new GeometryGroup();
                var loadGroup = new GeometryGroup();
                var variableGroup = new GeometryGroup();
                var pinGroup = new GeometryGroup();
                var symGroup = new GeometryGroup();
                var intGroup = new GeometryGroup();

                // divide members into two groups
                List<Member> all = ((ComputedStructure)model).Members;

                IEnumerable<Member> env = new List<Member>();
                IEnumerable<Member> interior = new List<Member>();
                if (((ComputedStructure)model).StructType == Structure.StructureType.Frame)
                {
                    env = all.Where(m => m.Envelope);
                    interior = all.Where(m => !m.Envelope);
                }
                else
                {
                    env = all;
                }

                // draw line groups
                memberGroup = MyParent.DrawLines(memberGroup, env);
                intGroup = MyParent.DrawLines(intGroup, interior);

                // draw point groups
                //nodeGroup = MyParent.DrawPoints(nodeGroup, model.Nodes, 1.5);

                // draw info groups
                //loadGroup = MyParent.DrawLoads(loadGroup, model.Nodes);
                //variableGroup = MyParent.DrawVars(variableGroup, model.Nodes);
                //pinGroup = MyParent.DrawPins(pinGroup, model.Nodes);
                //symGroup = MyParent.DrawSym(symGroup, model);

                // set up paths
                //var nodePath = new Path() { Fill = myBlackBrush, StrokeThickness = 0, Data = nodeGroup };
                var memberPath = new Path() { Stroke = myMemberBrush, StrokeThickness = lineScale, Data = memberGroup };
                var intPath = new Path() { Stroke = myGrayBrush, StrokeThickness = lineScale, Data = intGroup };
                //var loadPath = new Path() { Fill = myLoadBrush, Stroke = myLoadBrush, StrokeThickness = lineScale, Data = loadGroup };
                //var variablePath = new Path() { Stroke = myVariableBrush, StrokeThickness = lineScale, Data = variableGroup };
                //var pinPath = new Path() { Fill = myPinBrush, StrokeThickness = 0, Data = pinGroup };
                //var symPath = new Path() { Stroke = mySymBrush, StrokeThickness = lineScale, Data = symGroup };

                // group paths into list
                List<Path> paths = new List<Path>() { memberPath, intPath };
                return paths;
            }
            return new List<Path>();
        }
    }
}
