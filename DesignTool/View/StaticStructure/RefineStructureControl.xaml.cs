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

namespace DesignTool.View
{
    public partial class RefineStructureControl : UserControl
    {
        public RefineStructureControl(Structure s)
        {
            InitializeComponent();
            this.Structure = s;

            // set up drawing parameters
            double zeroX = Structure.ZeroPoint[0];
            double zeroY = Structure.ZeroPoint[1];
            double scale = Math.Min(600 / Structure.Dimensions[0], 400 / Structure.Dimensions[1]);
            double zeroForce = 0.001;

            // set up colors
            SolidColorBrush myBlueBrush = new SolidColorBrush();
            SolidColorBrush myRedBrush = new SolidColorBrush();
            SolidColorBrush myGrayBrush = new SolidColorBrush();
            SolidColorBrush myBlackBrush = new SolidColorBrush();
            myBlueBrush.Color = Colors.Blue;
            myRedBrush.Color = Colors.Red;
            myGrayBrush.Color = Colors.Gray;
            myBlackBrush.Color = Colors.Black;
            
            Path tensionStructurePath = new Path();
            tensionStructurePath.Stroke = myBlackBrush;
            tensionStructurePath.StrokeThickness = 2;
            Path compressionStructurePath = new Path();
            compressionStructurePath.Stroke = myBlackBrush;
            compressionStructurePath.StrokeThickness = 2;
            Path zeroStructurePath = new Path();
            zeroStructurePath.Stroke = myBlackBrush;
            zeroStructurePath.StrokeThickness = 2;

            Path NodePath = new Path();
            NodePath.Stroke = myBlackBrush;
            NodePath.StrokeThickness = 0.5;
            NodePath.Fill = myBlackBrush;

            // set up geometry groups
            GeometryGroup tensionGeometryGroup = new GeometryGroup();
            GeometryGroup compressionGeometryGroup = new GeometryGroup();
            GeometryGroup zeroGeometryGroup = new GeometryGroup();
            GeometryGroup NodeGroup = new GeometryGroup();

            // draw members
            foreach (Member m in Structure.Members)
            {
                LineGeometry memberGeometry = new LineGeometry();
                memberGeometry.StartPoint = new Point(scale * (m.NodeI.DOFs[0].Coord - zeroX),
                    scale * (-1 * (m.NodeI.DOFs[1].Coord - zeroY) + Structure.Dimensions[1]));
                memberGeometry.EndPoint = new Point(scale * (m.NodeJ.DOFs[0].Coord - zeroX),
                    scale * (-1 * (m.NodeJ.DOFs[1].Coord - zeroY) + Structure.Dimensions[1]));
                if (Structure.StructType == Model.Structure.StructureType.Frame)
                {
                    if (m.Envelope)
                    {
                        if (m.AxialForce > zeroForce)
                        {
                            tensionGeometryGroup.Children.Add(memberGeometry);
                        }
                        else if (m.AxialForce < -1 * zeroForce)
                        {
                            compressionGeometryGroup.Children.Add(memberGeometry);
                        }
                        else
                        {
                            zeroGeometryGroup.Children.Add(memberGeometry);
                        }
                    }
                }
                else
                {
                    if (m.AxialForce > zeroForce)
                    {
                        tensionGeometryGroup.Children.Add(memberGeometry);
                    }
                    else if (m.AxialForce < -1 * zeroForce)
                    {
                        compressionGeometryGroup.Children.Add(memberGeometry);
                    }
                    else
                    {
                        zeroGeometryGroup.Children.Add(memberGeometry);
                    }
                }
            }

            // draw nodes
            foreach (Node n in Structure.Nodes)
            {
                EllipseGeometry nodeCircle = new EllipseGeometry();
                nodeCircle.Center = new Point(scale * (n.DOFs[0].Coord - zeroX),
                    scale * (-1 * (n.DOFs[1].Coord - zeroY) + Structure.Dimensions[1]));
                
                nodeCircle.RadiusX = 4;
                nodeCircle.RadiusY = 4;
                NodeGroup.Children.Add(nodeCircle);
            }

            tensionStructurePath.Data = tensionGeometryGroup;
            tensionStructurePath.Name = "myTensionStructure";
            compressionStructurePath.Data = compressionGeometryGroup;
            compressionStructurePath.Name = "myCompressionStructure";
            zeroStructurePath.Data = zeroGeometryGroup;
            zeroStructurePath.Name = "myZeroStructure";
            NodePath.Data = NodeGroup;

            foreach (Path p in new Path[] { tensionStructurePath, compressionStructurePath, zeroStructurePath, NodePath })
            {
                Picture.Children.Add(p);
            }

            Picture.Width = scale * Structure.Dimensions[0] + 200;
            Picture.Height = scale * Structure.Dimensions[1] + 200;

            TranslateTransform transform = new TranslateTransform();
            transform.X = 100;
            transform.Y = 100;
            //transform.X = scale * Structure.Dimensions[0] / 2;
            //transform.DOFs[1].Coord = scale * Structure.Dimensions[1] / 2;
            Picture.RenderTransform = transform;

            //if (Structure.StructType == Model.Structure.StructureType.Truss)
            //{
               Score.Text = "score: " + string.Format("{0:0.00}", s.Result.Weight);
            //}
            //else if (Structure.StructType == Model.Structure.StructureType.Frame)
            //{
            //    Score.Text = "score: " + string.Format("{0:0.00}", s.Result.WeightNoBuckling / MainPage.CommonData.RefFrameScore);
            //}
        }

        public Structure Structure
        {
            get;
            set;
        }
    }
}
