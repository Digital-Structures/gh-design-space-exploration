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
using StructureEngine.Model;

namespace DesignTool.View.StaticStructure
{
    public class SetupBehavior : IModeBehavior
    {
        public SetupBehavior(DesignControl dc)
        {
            this.MyParent = dc;
            this.ScoreVis = false;
        }

        public bool ScoreVis
        {
            get;
            set;
        }

        private DesignControl MyParent;

        public void DoClick()
        {
            if (MyParent.Clickable)
            {
                MyParent.HoverPath.Stroke = new SolidColorBrush(Colors.Gray); // 255, 127, 36
                MyParent.MyParent.SetSelectedControl(MyParent);
            }
        }

        public void DoHover()
        {
            if (MyParent.Clickable)
            {
                MyParent.HoverPath.Stroke = new SolidColorBrush(Colors.LightGray);
            }
        }

        public void DoLeaveHover()
        {
            if (MyParent.Clickable)
            {
                MyParent.HoverPath.Stroke = new SolidColorBrush(Colors.Transparent);
            }
        }

        //public List<Path> DrawShape()
        //{
        //    IDesign model = MyParent.ViewModel.Model;
        //    if (model != null)
        //    {
        //        // set up scales
        //        double lineScale = Math.Max(1 * MyParent.ScaleRef, .75);

        //        // set up colors
        //        var myBlackBrush = new SolidColorBrush(Colors.Black);
        //        var myMemberBrush = new SolidColorBrush(Colors.Black);
        //        var myLoadBrush = new SolidColorBrush(Colors.DarkGray);
        //        var myVariableBrush = new SolidColorBrush(Colors.Red);
        //        var myPinBrush = new SolidColorBrush(Colors.LightGray);
        //        var mySymBrush = new SolidColorBrush(Colors.Gray);

        //        // set up geometry groups
        //        var nodeGroup = new GeometryGroup();
        //        var memberGroup = new GeometryGroup();
        //        var loadGroup = new GeometryGroup();
        //        var variableGroup = new GeometryGroup();
        //        var pinGroup = new GeometryGroup();
        //        var symGroup = new GeometryGroup();

        //        // draw line groups
        //        memberGroup = MyParent.DrawLines(memberGroup, ((ComputedStructure)model).Members);

        //        // draw point groups
        //        nodeGroup = MyParent.DrawPoints(nodeGroup, ((ComputedStructure)model).Nodes, 1.5);

        //        // draw info groups
        //        //loadGroup = MyParent.DrawLoads(loadGroup, model.Nodes);
        //        //variableGroup = MyParent.DrawVars(variableGroup, model.Nodes);
        //        //pinGroup = MyParent.DrawPins(pinGroup, model.Nodes);
        //        //symGroup = MyParent.DrawSym(symGroup, model);

        //        // set up paths
        //        var nodePath = new Path() { Fill = myBlackBrush, StrokeThickness = 0, Data = nodeGroup };
        //        var memberPath = new Path() { Stroke = myMemberBrush, StrokeThickness = lineScale, Data = memberGroup };
        //        var loadPath = new Path() { Fill = myLoadBrush, Stroke = myLoadBrush, StrokeThickness = lineScale, Data = loadGroup };
        //        var variablePath = new Path() { Stroke = myVariableBrush, StrokeThickness = lineScale, Data = variableGroup };
        //        var pinPath = new Path() { Fill = myPinBrush, StrokeThickness = 0, Data = pinGroup };
        //        var symPath = new Path() { Stroke = mySymBrush, StrokeThickness = lineScale, Data = symGroup };

        //        // group paths into list
        //        List<Path> paths = new List<Path>() { nodePath, memberPath, loadPath, variablePath, pinPath, symPath };
        //        return paths;
        //    }
        //    return new List<Path>();
        //}
    }
}
