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
using DesignTool.Model;
using DesignTool.Analysis;
using DesignTool.DesignModes;
using DesignTool.ViewModel;

namespace DesignTool.View
{
    public partial class StructureControl : UserControl
    {
        public StructureControl(Structure s, Generate gen)
        {
            InitializeComponent();
            this.Structure = s;
            this.myGen = gen;
            this.ViewModel = new StructureVM();
            ViewModel.Model = s;

            SolidColorBrush myBlackBrush = new SolidColorBrush(Colors.Black);
            SolidColorBrush myBlueBrush = new SolidColorBrush(Colors.Blue);
            SolidColorBrush myRedBrush = new SolidColorBrush(Colors.Red);
            Path memberPath = new Path();
            memberPath.Stroke = myBlackBrush;
            memberPath.StrokeThickness = 1;
            memberPath.Fill = myBlackBrush;

            GeometryGroup memberGroup = new GeometryGroup();

            double zeroX = Structure.ZeroPoint[0];
            double zeroY = Structure.ZeroPoint[1];
            double dim = Math.Max(Structure.Dimensions[0], Structure.Dimensions[1]);
            double dimX = Structure.Dimensions[0];
            double dimY = Structure.Dimensions[1];
            double scale = 65 / dim;
            //double y0 = 0; // s.Nodes[0].Y;

            double maxArea = 0;
            double minArea = 1000000;
            foreach (Member m in Structure.Members)
            {
                if (m.Area > maxArea)
                {
                    maxArea = m.Area;
                }
                if (m.Area < minArea)
                {
                    minArea = m.Area;
                }
            }

            if (minArea == 0)
            {
                minArea = 0.01;
            }

            double minWeight = 0.5;
            double maxWeight = 3.0;

            double forceScaleMax = maxWeight / Math.Abs(maxArea);
            double forceScaleMin = minWeight / Math.Abs(minArea);

            double slope = (maxArea - minArea) / (maxWeight - minWeight);
            double yint = minArea - minWeight * slope;

            foreach (Member m in Structure.Members)
            {
                if (Structure.StructType == Model.Structure.StructureType.Truss)
                {
                    //Path trussPath = new Path();
                    //if (m.AxialForce > 0)
                    //{
                    //    trussPath.Stroke = myBlueBrush;
                    //}
                    //else
                    //{
                    //    trussPath.Stroke = myRedBrush;
                    //}
                    //trussPath.StrokeThickness = (m.Area - yint) / slope;
                    LineGeometry memberGeometry = new LineGeometry();
                    memberGeometry.StartPoint = new Point(scale * (m.NodeI.DOFs[0].Coord - zeroX) + (74 - scale * dimX) / 2,
                        scale * (-1 * (m.NodeI.DOFs[1].Coord - zeroY) + Structure.Dimensions[1]) + (74 - scale * dimY) / 2);
                    memberGeometry.EndPoint = new Point(scale * (m.NodeJ.DOFs[0].Coord - zeroX) + (74 - scale * dimX) / 2,
                        scale * (-1 * (m.NodeJ.DOFs[1].Coord - zeroY) + Structure.Dimensions[1]) + (74 - scale * dimY) / 2);
                    //trussPath.Data = memberGeometry;
                    //Picture.Children.Add(trussPath);

                    memberGroup.Children.Add(memberGeometry);
                }
                else if (Structure.StructType == Model.Structure.StructureType.Frame && m.Envelope)
                {
                    //
                    //memberGeometry.StartPoint = new Point(m.NodeI.X, m.NodeI.Y);
                    //memberGeometry.EndPoint = new Point(m.NodeJ.X, m.NodeJ.Y);
                    //

                    LineGeometry memberGeometry = new LineGeometry();
                    memberGeometry.StartPoint = new Point(scale * (m.NodeI.DOFs[0].Coord - zeroX) + (74 - scale * dimX) / 2,
                        scale * (-1 * (m.NodeI.DOFs[1].Coord - zeroY) + Structure.Dimensions[1]) + (74 - scale * dimY) / 2);
                    memberGeometry.EndPoint = new Point(scale * (m.NodeJ.DOFs[0].Coord - zeroX) + (74 - scale * dimX) / 2,
                        scale * (-1 * (m.NodeJ.DOFs[1].Coord - zeroY) + Structure.Dimensions[1]) + (74 - scale * dimY) / 2);
                    memberGroup.Children.Add(memberGeometry);
                }
            }

            memberPath.Data = memberGroup;
            memberPath.Name = "myMember";

            //TranslateTransform trans1 = new TranslateTransform();
            //trans1.X = -zeroX;
            //trans1.Y = -zeroY;
            //ScaleTransform sc = new ScaleTransform();
            //sc.CenterX = zeroX;
            //sc.CenterY = zeroY;
            //sc.ScaleX = scale;
            //sc.ScaleY = -scale;
            //TranslateTransform trans = new TranslateTransform();
            //trans.X = -zeroX * scale + (74 - scale * Structure.Dimensions[0]) / 2;
            //trans.Y = -zeroY * scale + scale * Structure.Dimensions[1] + (74 - scale * Structure.Dimensions[1]) / 2;
            //TransformGroup tgroup = new TransformGroup();
            ////tgroup.Children.Add(trans1);
            //tgroup.Children.Add(sc);
            //tgroup.Children.Add(trans);

            //memberPath.RenderTransform = tgroup;
                       
            foreach (Path p in new Path[] { memberPath })
            {
                Picture.Children.Add(p);
            }

            if (s.PredictedScore != 0)
            {
                //predscore.Text = string.Format("{0:0.00}", s.PredictedScore);
            }

            //if (Structure.StructType == Model.Structure.StructureType.Truss)
            //{
                score.Text = string.Format("{0:0.00}", s.Result.Weight / MainPage.CommonData.RefTrussScore);
            //}
            //else if (Structure.StructType == Model.Structure.StructureType.Frame)
            //{
            //    score.Text = string.Format("{0:0.00}", s.Result.WeightNoBuckling / MainPage.CommonData.RefFrameScore);
            //}

            MemberPath = memberPath;
        }

        public StructureVM ViewModel
        {
            get;
            private set;
        }

        public Path MemberPath
        {
            get;
            set;
        }
        
        public Structure Structure
        {
            get;
            set;
        }

        public bool Selected
        {
            get;
            private set;
        }

        public bool Disabled
        {
            get;
            set;
        }

        public Generate myGen
        {
            get;
            set;
        }

        private void myStructure_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.Disabled)
            {
                this.Selected = !Selected;
                if (this.Selected)
                {
                    SolidColorBrush selectBrush = new SolidColorBrush();
                    selectBrush.Color = Colors.Orange;
                    MemberPath.Stroke = selectBrush;
                    MemberPath.StrokeThickness = 2;
                }
                else
                {
                    SolidColorBrush myBlackBrush = new SolidColorBrush(Colors.Black);
                    MemberPath.Stroke = myBlackBrush;
                    MemberPath.StrokeThickness = 1;
                }
            }
            
        }

        private void refCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.SetRefineStructure();
            myGen.SetRefStructControl(this);
            refCheckBox.Visibility = Visibility.Visible;
        }

        private void refCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel.ResetRefineStructure();
            this.refCheckBox.IsChecked = false;
            myGen.ResetRefStructControl();
            refCheckBox.Visibility = Visibility.Collapsed;
        }

        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            SolidColorBrush hoverBrush = new SolidColorBrush();
            hoverBrush.Color = Colors.Orange;
            MemberPath.Stroke = hoverBrush;
            refCheckBox.Visibility = Visibility.Visible;
        }

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.Selected)
            {
                SolidColorBrush myBlackBrush = new SolidColorBrush();
                myBlackBrush.Color = Colors.Black;
                MemberPath.Stroke = myBlackBrush;
                MemberPath.StrokeThickness = 1;
            }
            if (!(bool)refCheckBox.IsChecked)
            {
                refCheckBox.Visibility = Visibility.Collapsed;
            }
        }
    }
}
