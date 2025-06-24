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
using StructureEngine.Grammar.Bridge;
using StructureEngine.Model;

namespace DesignTool.View
{
    public class BridgeDrawer : IDrawer
    {
        public BridgeDrawer(DesignControl p)
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
            IDesign model = (MyParent.ViewModel.Model as BridgeShape);
            if (model != null)
            {
                // set up scales
                double towerScale = Math.Max(15 * MyParent.ScaleRef, 4);
                double deckScale = Math.Max(8 * MyParent.ScaleRef, 2);
                double infillScale = Math.Max(1 * MyParent.ScaleRef, 0.5);

                // set up colors
                var myGrayBrush = new SolidColorBrush(Colors.LightGray);
                var myBlackBrush = new SolidColorBrush(Colors.Black);
                var myDarkGrayBrush = new SolidColorBrush(Colors.DarkGray);
                var myRedBrush = new SolidColorBrush(Colors.Red);
                var myBlueBrush = new SolidColorBrush(Colors.Blue);
                
                // set up geometry groups
                var deckGroup = new GeometryGroup();
                var towerGroup = new GeometryGroup();
                var infillGroup = new GeometryGroup();
                var infill2Group = new GeometryGroup();
                var fillGroup = new GeometryGroup();
                var topGroup = new GeometryGroup();
                var divGroup = new GeometryGroup();

                // draw line groups
                deckGroup = MyParent.DrawLines(deckGroup, ((BridgeShape)model).Deck);
                towerGroup = MyParent.DrawLines(towerGroup, ((BridgeShape)model).Tower);
                infillGroup = MyParent.DrawLines(infillGroup, ((BridgeShape)model).Infill);
                infill2Group = MyParent.DrawLines(infill2Group, ((BridgeShape)model).Infill2);

                // draw point groups
                topGroup = MyParent.DrawPoints(topGroup, ((BridgeShape)model).Tops, 2);
                divGroup = MyParent.DrawPoints(divGroup, ((BridgeShape)model).DeckPoints, 2);

                // draw area groups
                fillGroup = MyParent.DrawAreas(fillGroup, ((BridgeShape)model).Fill);

                // set up paths
                var DeckPath = new Path() { Stroke = myDarkGrayBrush, StrokeThickness = deckScale, Data = deckGroup };
                var TowerPath = new Path() { Stroke = myGrayBrush, StrokeThickness = towerScale, Data = towerGroup };
                var InfillPath = new Path() { Stroke = myBlackBrush, StrokeThickness = infillScale, Data = infillGroup };
                var Infill2Path = new Path() { Stroke = myBlackBrush, StrokeThickness = infillScale, Data = infill2Group };
                var FillPath = new Path() { Fill = myGrayBrush, StrokeThickness = 0, Data = fillGroup };
                var TopPath = new Path() { Stroke = new SolidColorBrush(Colors.Transparent), StrokeThickness = 0, Data = topGroup };
                var DivPath = new Path() { Stroke = new SolidColorBrush(Colors.Transparent), StrokeThickness = 0, Data = divGroup, Fill = myRedBrush };

                // group paths into list
                List<Path> paths = new List<Path>() { DeckPath, TowerPath, FillPath, InfillPath, Infill2Path, TopPath, DivPath };
                List<Path> finalPaths = new List<Path>();

                foreach (Path p in paths)
                {
                    GeometryGroup g = p.Data as GeometryGroup;
                    if (g.Children.Count > 0)
                    {
                        finalPaths.Add(p);
                    }
                }

                //List<Path> paths = new List<Path>();
                //if (deckGroup.Children.Count > 0) { paths.Add(DeckPath); }
                //if (towerGroup.Children.Count > 0) { paths.Add(TowerPath); }
                //if (infillGroup.Children.Count > 0) { paths.Add(InfillPath); }
                //if (infill2Group.Children.Count > 0) { paths.Add(Infill2Path); }
                //if (fillGroup.Children.Count > 0) { paths.Add(FillPath); }

                return finalPaths;
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
