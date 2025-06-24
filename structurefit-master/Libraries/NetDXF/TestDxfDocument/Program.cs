﻿#region netDxf, Copyright(C) 2013 Daniel Carvajal, Licensed under LGPL.

//                        netDxf library
// Copyright (C) 2013 Daniel Carvajal (haplokuon@gmail.com)
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;
using netDxf.Header;
using netDxf.Objects;
using netDxf.Tables;
using Point = netDxf.Entities.Point;

namespace TestDxfDocument
{
    /// <summary>
    /// This is just a simple test of work in progress for the netDxf Library.
    /// </summary>
    public class Program
    {
        private static void Main()
        {
            Test();
            //ExplodePolyfaceMesh();
            //ApplicationRegistries();
            //TestOCStoWCS();
            //WriteGradientPattern();
            //WriteGroup();
            //WriteMLine();
            //ObjectVisibility();
            //EntityTrueColor();
            //EntityLineWeight();
            //ReadWriteFromStream();
            //WriteNoAsciiText();
            //WriteSplineBoundaryHatch();
            //WriteNoInsertBlock();
            //WriteImage();
            //SplineDrawing();
            //AddAndRemove();
            //LoadAndSave();
            //Fixes();
            //CleanDrawing();
            //OrdinateDimensionDrawing();
            //Angular2LineDimensionDrawing();
            //Angular3PointDimensionDrawing();
            //DiametricDimensionDrawing();
            //RadialDimensionDrawing();
            //LinearDimensionDrawing();
            //AlignedDimensionDrawing();
            //WriteMText();
            //LineWidth();
            //HatchCircleBoundary();
            //ToPolyline();
            //FilesTest();
            //CustomHatchPattern();
            //LoadSaveHatchTest();
            //WriteDxfFile();
            //ReadDxfFile();
            //ExplodeTest();
            //HatchTestLinesBoundary();
            //HatchTest1();
            //HatchTest2();
            //HatchTest3();
            //BlockAttributes();
            //WriteNestedInsert();
            //WritePolyfaceMesh();
            //Ellipse();
            //Solid();
            //Face3d();
            //LwPolyline();
            //Polyline();
            //Dxf2000();
            //SpeedTest();
            //WritePolyline3d();
            //WriteInsert();
        }

        private static void Test()
        {
            // important changes in netDxf 0.4 version
            DxfVersion version = DxfDocument.CheckDxfFileVersion("sample.dxf");
            // the Load function is now static
            // sample.dxf contains all supported entities by netDxf
            DxfDocument dxf = DxfDocument.Load("sample.dxf");
            //// the dxf version is controlled by the DrawingVariables property of the dxf document,
            //// also a HeaderVariables instance or a DxfVersion can be passed to the constructor to initialize a new DxfDocument.
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2013;
            dxf.Save("sample 2013.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("sample 2010.dxf");
        }

        private static void ExplodePolyfaceMesh()
        {
            DxfDocument dxf = DxfDocument.Load("polyface mesh.dxf");
            DxfDocument dxfOut = new DxfDocument(dxf.DrawingVariables);
            foreach (PolyfaceMesh polyfaceMesh in dxf.PolyfaceMeshes)
            {
                List<EntityObject> entities = polyfaceMesh.Explode();
                dxfOut.AddEntity(entities);
            }

            dxfOut.Save("polyface mesh exploded.dxf");
        }
        private static void ApplicationRegistries()
        {
            DxfDocument dxf = new DxfDocument();
            // add a new application registry to the document (optional), if not present it will be added when the entity is passed to the document
            ApplicationRegistry newAppReg = dxf.AddApplicationRegistry(new ApplicationRegistry("NewAppReg"));

            Line line = new Line(Vector2.Zero, 100 * Vector2.UnitX);
            XData xdata = new XData(newAppReg);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "string of the new application registry"));
            line.XData = new Dictionary<string, XData>
                             {
                                 {xdata.ApplicationRegistry.Name, xdata},
                             };

            dxf.AddEntity(line);
            dxf.Save("ApplicationRegistryTest.dxf");

            // gets the complete application registries present in the document
            ReadOnlyCollection<ApplicationRegistry> appRegs = dxf.AppRegisterNames;

            // get an application registry by name
            ApplicationRegistry netDxfAppReg = dxf.GetApplicationRegistry(appRegs[appRegs.Count - 1].Name);
        }
        private static void TestOCStoWCS()
        {
            // vertexes of the light weight polyline, they are defined in OCS (Object Coordinate System)
            LwPolylineVertex v1 = new LwPolylineVertex(1, -5);
            LwPolylineVertex v2 = new LwPolylineVertex(-3, 2);
            LwPolylineVertex v3 = new LwPolylineVertex(8, 15);

            LwPolyline lwp = new LwPolyline(new List<LwPolylineVertex> {v1, v2, v3});
            // the normal will define the plane where the lwpolyline is defined
            lwp.Normal = new Vector3(1, 1, 0);
            // the entity elevation defines the z vector of the vertexes along the entity normal
            lwp.Elevation = 2.5;

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(lwp);
            dxf.Save("OCStoWCS.dxf");

            // if you want to convert the vertexes of the polyline to WCS (World Coordinate System), you can
            Vector3 v1OCS = new Vector3(v1.Location.X, v1.Location.Y, lwp.Elevation);
            Vector3 v2OCS = new Vector3(v2.Location.X, v2.Location.Y, lwp.Elevation);
            Vector3 v3OCS = new Vector3(v3.Location.X, v3.Location.Y, lwp.Elevation);
            List<Vector3> vertexesWCS = MathHelper.Transform(new List<Vector3> { v1OCS, v2OCS, v3OCS }, lwp.Normal, MathHelper.CoordinateSystem.Object, MathHelper.CoordinateSystem.World);


        }
        private static void WriteGradientPattern()
        {
            List<LwPolylineVertex> vertexes = new List<LwPolylineVertex>
                                         {
                                             new LwPolylineVertex(new Vector2(0, 0)),
                                             new LwPolylineVertex(new Vector2(0, 150)),
                                             new LwPolylineVertex(new Vector2(150, 150)),
                                             new LwPolylineVertex(new Vector2(150, 0))
                                         };
            LwPolyline pol = new LwPolyline(vertexes, true);


            Line line1 = new Line(new Vector2(0, 0), new Vector2(0, 150));
            Line line2 = new Line(new Vector2(0, 150), new Vector2(150, 150));
            Line line3 = new Line(new Vector2(150, 150), new Vector2(150, 0));
            Line line4 = new Line(new Vector2(150, 0), new Vector2(0, 0));


            HatchGradientPattern gradient = new HatchGradientPattern(AciColor.Red, AciColor.Blue, HatchGradientPatternType.Linear);
            //HatchGradientPattern gradient = new HatchGradientPattern(AciColor.Red, 0.75, HatchGradientPatternType.Linear);
            gradient.Angle = 30;

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>
                                                   {
                                                       new HatchBoundaryPath(new List<EntityObject> {pol})
                                                   };
            Hatch hatch = new Hatch(gradient, boundary);
            
            // gradients are only supported for AutoCad2004 and later
            DxfDocument dxf = new DxfDocument(DxfVersion.AutoCad2004);
            dxf.AddEntity(hatch);
            dxf.Save("gradient test.dxf");

            //DxfDocument dxf2 = DxfDocument.Load("gradient test.dxf");

            //dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;

            //AciColor color = new AciColor(63, 79, 127);

            //dxf.Save("gradient test 2000.dxf");

        }
        private static void WriteGroup()
        {
            Line line1 = new Line(new Vector2(0, 0), new Vector2(100, 100));
            Line line2 = new Line(new Vector2(100, 0), new Vector2(200, 100));
            Line line3 = new Line(new Vector2(200, 0), new Vector2(300, 100));

            Group group = new Group("MyGroup")
                              {
                                  Entities = new List<EntityObject> {line1, line2}
                              };

            Group group2 = new Group()
            {
                Entities = new List<EntityObject> { line1, line3 }
            };

            DxfDocument dxf = new DxfDocument();
            // the AddGroup method will also add the entities contained in a group to the document.
            dxf.AddGroup(group);
            dxf.AddGroup(group2);
            dxf.Save("group.dxf");

            DxfDocument dxf2 = DxfDocument.Load("group.dxf");
            Console.WriteLine("Dxf name: " + dxf2.Name + " loaded.");
        }
        private static void WriteMLine()
        {
            DxfDocument dxf = new DxfDocument();
            //MLineStyle style = MLineStyle.Default;
            //dxf.AddMLineStyle(style);

            List<Vector2> vertexes = new List<Vector2>
                                         {
                                             new Vector2(0, 0),
                                             new Vector2(0, 150),
                                             new Vector2(150, 150),
                                             new Vector2(150, 0)
                                         };

            MLine mline = new MLine(vertexes);
            mline.Scale = 20;
            mline.Justification = MLineJustification.Zero;
            //mline.IsClosed = true;

            MLineStyle style = new MLineStyle("MyStyle", "Personalized style.");
            style.Elements.Add(new MLineStyleElement(0.25));
            style.Elements.Add(new MLineStyleElement(-0.25));
            // if we add new elements directly to the list we need to sort the list,
            style.Elements.Sort();
            style.Flags = MLineStyleFlags.EndInnerArcsCap | MLineStyleFlags.EndRoundCap | MLineStyleFlags.StartInnerArcsCap | MLineStyleFlags.StartRoundCap;
            //style.StartAngle = 25.0;
            //style.EndAngle = 160.0;
            // AutoCad2000 dxf version does not support true colors for MLineStyle elements
            style.Elements[0].Color = new AciColor(180, 230, 147);
            mline.Style = style;
            // we have modified the mline after setting its vertexes so we need to manually call this method.
            // also when manually editting the vertex distances
            mline.CalculateVertexesInfo();

            // we can manually create cuts or gaps in the individual elements that made the multiline.
            // the cuts are defined as distances from the start point of the element along its direction.
            mline.Vertexes[0].Distances[0].Add(50);
            mline.Vertexes[0].Distances[0].Add(100);
            mline.Vertexes[0].Distances[mline.Style.Elements.Count-1].Add(50);
            mline.Vertexes[0].Distances[mline.Style.Elements.Count-1].Add(100);
            dxf.AddEntity(mline);

            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            dxf.Save("MLine.dxf");

            //dxf = DxfDocument.Load("Drawing1.dxf");
            //dxf.Save("Drawing1 copy.dxf");

            //dxf = DxfDocument.Load("Drawing3.dxf");
            //dxf.Save("Drawing3 copy.dxf");

            //dxf = DxfDocument.Load("Drawing2.dxf");
            //dxf.Save("Drawing2 copy.dxf");

            // empty mline
            //List<Vector2> vertexes2 = new List<Vector2>
            //                             {
            //                                 new Vector2(0, 0),
            //                                 new Vector2(100, 100),
            //                                 new Vector2(100, 100),
            //                                 new Vector2(200, 0)
            //                             };

            //MLine mline2 = new MLine(vertexes2){Scale = 20};
            //mline2.CalculateVertexesInfo();

            //DxfDocument dxf2 = new DxfDocument();
            //dxf2.AddEntity(mline2);
            ////dxf2.Save("void mline.dxf");

            //MLine mline3 = new MLine();
            //dxf2.AddEntity(mline3);
            ////dxf2.Save("void mline.dxf");

            //Polyline pol = new Polyline();
            //LwPolyline lwPol = new LwPolyline();
            //dxf2.AddEntity(pol);
            //dxf2.AddEntity(lwPol);
            //dxf2.Save("void mline.dxf");
            //dxf2 = DxfDocument.Load("void mline.dxf");
            
        }
        private static void ObjectVisibility()
        {
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0))
                            {
                                Color = AciColor.Yellow
                            };

            Line line2 = new Line(new Vector3(0, 100, 0), new Vector3(100, 0, 0))
                             {
                                 IsVisible = false
                             };

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line);
            dxf.AddEntity(line2);
            dxf.Save("object visibility.dxf");
            dxf = DxfDocument.Load("object visibility.dxf");
            dxf.Save("object visibility 2.dxf");

        }
        private static void WriteInsert()
        {
            // nested blocks
            DxfDocument dxf = new DxfDocument();

            Block nestedBlock = new Block("Nested block");
            nestedBlock.Entities.Add(new Line(new Vector3(-5, -5, 0), new Vector3(5, 5, 0)));
            nestedBlock.Entities.Add(new Line(new Vector3(5, -5, 0), new Vector3(-5, 5, 0)));

            Insert nestedInsert = new Insert(nestedBlock, new Vector3(0, 0, 0)); // the position will be relative to the position of the insert that nest it

            Circle circle = new Circle(Vector3.Zero, 5);
            circle.Layer = new Layer("circle");
            circle.Layer.Color.Index = 2;
            Block block = new Block("MyBlock");
            block.Entities.Add(circle);
            block.Entities.Add(nestedInsert);

            Insert insert = new Insert(block, new Vector3(5, 5, 5));
            insert.Layer = new Layer("insert");

            dxf.AddEntity(insert);

            dxf.Save("insert.dxf");
            dxf = DxfDocument.Load("insert.dxf");

        }
        private static void EntityTrueColor()
        {
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0));
            line.Color = new AciColor(152, 103, 136);
            // by default a color initialized with rgb components will be exported as true color
            // you can override this behaviour with
            // line.Color.UseTrueColor = false;

            Layer layer = new Layer("MyLayer");
            layer.Color = new AciColor(157, 238, 17);
            Line line2 = new Line(new Vector3(0, 100, 0), new Vector3(100, 0, 0));
            line2.Layer = layer;
            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line);
            dxf.AddEntity(line2);
            dxf.Save("line true color.dxf");
            dxf = DxfDocument.Load("line true color.dxf");
        }
        private static void EntityLineWeight()
        {
            // the lineweight is always defined as 1/100 mm, this property is the equivalent of stroke width, outline width in other programs. Do not confuse with line.Thickness
            // it follow the AutoCAD naming style, check the documentation in case of doubt
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0));
            line.Lineweight.Value = 100; // 1.0 mm
            Text text = new Text("Text with lineweight", Vector3.Zero, 10);
            text.Lineweight.Value = 50; // 0.5 mm

            Layer layer = new Layer("MyLayer");
            layer.Lineweight.Value = 200; // 2 mm all entities in the layer with Color.ByLayer will inherit this value
            layer.Color = AciColor.Green;
            Line line2 = new Line(new Vector3(0, 100, 0), new Vector3(100, 0, 0));
            line2.Layer = layer;

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line);
            dxf.AddEntity(line2);
            dxf.AddEntity(text);
            dxf.Save("line weight.dxf");
            dxf = DxfDocument.Load("line weight.dxf");
        }
        private static void ReadWriteFromStream()
        {
            // Load and Save methods are now able to work directly with a stream.
            // They will return true or false if the operation has been carried out successfully or not.
            // The Save(string file, DxfVersion dxfVersion) and Load(string file) methods will still raise an exception if they are unable to create the FileStream.
            // On Debug mode they will raise any exception that migh occurr during the whole process.
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0));
            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line);
            dxf.Save("test.dxf");
            
            // saving to memory stream always use the default constructor, a fixed size stream will not work.
            MemoryStream memoryStream = new MemoryStream();
            dxf.Save(memoryStream);

            // loading from memory stream
            DxfDocument dxf2 = DxfDocument.Load(memoryStream);

            // saving to file stream
            FileStream fileStream = new FileStream("test fileStream.dxf", FileMode.Create);
            dxf2.Save(fileStream);
            fileStream.Close(); // you will need to close the stream manually to avoid file already open conflicts

            FileStream fileStreamLoad = new FileStream("test fileStream.dxf", FileMode.Open, FileAccess.Read);
            DxfDocument dxf3 = DxfDocument.Load(fileStreamLoad);

        }
        private static void WriteNoAsciiText()
        {
            TextStyle textStyle = new TextStyle("Arial.ttf");
            DxfDocument dxf = new DxfDocument();
            Text text = new Text("ÁÉÍÓÚ áéíóú Ññ àèìòù âêîôû", Vector2.Zero,10);
            text.Style = textStyle;
            dxf.AddEntity(text);
            dxf.Save("text.dxf");

            dxf = DxfDocument.Load("text1.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            dxf.Save("text2.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2007;
            dxf.Save("text3.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("text4.dxf");

        }
        private static void WriteSplineBoundaryHatch()
        {

            List<SplineVertex> ctrlPoints = new List<SplineVertex>
                                                {
                                                    new SplineVertex(new Vector3(0, 0, 0)),
                                                    new SplineVertex(new Vector3(25, 50, 0)),
                                                    new SplineVertex(new Vector3(50, 0, 0)),
                                                    new SplineVertex(new Vector3(75, 50, 0)),
                                                    new SplineVertex(new Vector3(100, 0, 0))
                                                };

            // hatch with single closed spline boundary path
            Spline spline = new Spline(ctrlPoints, 3, true); // closed periodic

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>();

            HatchBoundaryPath path = new HatchBoundaryPath(new List<EntityObject> {spline});
            boundary.Add(path);
            Hatch hatch = new Hatch(HatchPattern.Line, boundary);
            hatch.Pattern.Angle = 45;
            hatch.Pattern.Scale = 10;

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(hatch);
            dxf.AddEntity(spline);
            dxf.Save("hatch closed spline.dxf");
            dxf = DxfDocument.Load("hatch closed spline.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("hatch closed spline 2010.dxf");

            // hatch boundary path with spline and line
            Spline openSpline = new Spline(ctrlPoints, 3);
            Line line = new Line(ctrlPoints[0].Location, ctrlPoints[ctrlPoints.Count - 1].Location);

            List<HatchBoundaryPath> boundary2 = new List<HatchBoundaryPath>();
            HatchBoundaryPath path2 = new HatchBoundaryPath(new List<EntityObject> { openSpline, line });
            boundary2.Add(path2);
            Hatch hatch2 = new Hatch(HatchPattern.Line, boundary2);
            hatch2.Pattern.Angle = 45;
            hatch2.Pattern.Scale = 10;

            DxfDocument dxf2 = new DxfDocument();
            dxf2.AddEntity(hatch2);
            dxf2.AddEntity(openSpline);
            dxf2.AddEntity(line);
            dxf2.Save("hatch open spline.dxf");
            dxf2 = DxfDocument.Load("hatch open spline.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf2.Save("hatch open spline 2010.dxf");

        }
        private static void WriteNoInsertBlock()
        {
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0));
            Line line2 = new Line(new Vector3(0, 100, 0), new Vector3(100, 0, 0));
            // create the block definition
            Block block = new Block("MyBlock");
            // add the entities that you want in the block
            block.Entities.Add(line);
            block.Entities.Add(line2);
            
            
            // create the document
            DxfDocument dxf = new DxfDocument();
            // add the block definition to the block table list (this is the function that was private in earlier versions, check the changelog.txt)
            dxf.AddBlock(block);

            // and save file, no visible entities will appear if you try to open the drawing but the block will be there
            dxf.Save("Block definiton.dxf");

        }
        private static void WriteImage()
        {
            ImageDef imageDef = new ImageDef("image01.png");
            Image image = new Image(imageDef, Vector3.Zero);
            image.SetScale(2);

            XData xdata1 = new XData(new ApplicationRegistry("netDxf"));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.String, "xData image position"));
            xdata1.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionX, image.Position.X));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionY, image.Position.Y));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionZ, image.Position.Z));
            xdata1.XDataRecord.Add(XDataRecord.CloseControlString);
            image.XData = new Dictionary<string, XData>
                             {
                                 {xdata1.ApplicationRegistry.Name, xdata1},
                             };

            //image.Normal = new Vector3(1, 1, 1);
            //image.Rotation = 30;

            // you can pass a name that must be unique for the image definiton, by default it will use the file name without the extension
            ImageDef imageDef2 = new ImageDef("img\\image02.jpg", "MyImage");
            Image image2 = new Image(imageDef2, new Vector3(0, 150, 0));
            Image image3 = new Image(imageDef2, new Vector3(150, 150, 0));

            // clipping boundary definition in local coordinates
            ImageClippingBoundary clip = new ImageClippingBoundary(100, 100, 500, 300);
            image.ClippingBoundary = clip;
            // set to null to restore the default clipping boundary (full image)
            image.ClippingBoundary = null;

            // images can be part of a block definition
            Block block = new Block("ImageBlock");
            block.Entities.Add(image2);
            block.Entities.Add(image3);
            Insert insert = new Insert(block, new Vector3(0, 100, 0));

            DxfDocument dxf = new DxfDocument();

            dxf.AddEntity(image);
            //dxf.AddEntity(image2);
            //dxf.AddEntity(image3);
            dxf.AddEntity(insert);

            dxf.Save("image.dxf");
            dxf = DxfDocument.Load("image.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("image.dxf");

            //dxf.RemoveEntity(image2);
            //dxf.Save("image2.dxf");
            //dxf.RemoveEntity(image3);
            //dxf.RemoveEntity(image);
            //dxf.Save("image3.dxf");
        }
        private static void SplineDrawing()
        {
            List<SplineVertex> ctrlPoints = new List<SplineVertex>
                                                {
                                                    new SplineVertex(new Vector3(0, 0, 0), 1),
                                                    new SplineVertex(new Vector3(25, 50, 0), 2),
                                                    new SplineVertex(new Vector3(50, 0, 0), 3),
                                                    new SplineVertex(new Vector3(75, 50, 0), 4),
                                                    new SplineVertex(new Vector3(100, 0, 0), 5)
                                                };

            // the constructor will generate a uniform knot vector 
            Spline openSpline = new Spline(ctrlPoints, 3);

            List<SplineVertex> ctrlPointsClosed = new List<SplineVertex>
                                                {
                                                    new SplineVertex(new Vector3(0, 0, 0), 1),
                                                    new SplineVertex(new Vector3(25, 50, 0), 2),
                                                    new SplineVertex(new Vector3(50, 0, 0), 3),
                                                    new SplineVertex(new Vector3(75, 50, 0), 4),
                                                    new SplineVertex(new Vector3(100, 0, 0), 5),
                                                    new SplineVertex(new Vector3(0, 0, 0), 1) // closed spline non periodic we repeat the last control point
                                                };
            Spline closedNonPeriodicSpline = new Spline(ctrlPointsClosed, 3);

            // the periodic spline will generate a periodic (unclamped) closed curve,
            // as far as my tests have gone not all programs handle them correctly, most of them only handle clamped splines
            // seems that AutoCAD imports periodic closed splines correclty if all control point weights are equal to one.
            // seems that internally AutoCAD converts periodic closed splines to nonperiodic (clamped) closed splines.
            // my knowledge on nurbs is limited
            Spline closedPeriodicSpline = new Spline(ctrlPoints, 3, true);
            closedPeriodicSpline.SetUniformWeights(1.0);

            // manually defining the control points and the knot vector (example a circle created with nurbs)
            List<SplineVertex> circle = new List<SplineVertex>
                                                {
                                                    new SplineVertex(new Vector3(50, 0, 0), 1.0),
                                                    new SplineVertex(new Vector3(100, 0, 0), 1.0/2.0),
                                                    new SplineVertex(new Vector3(100, 100, 0), 1.0/2.0),
                                                    new SplineVertex(new Vector3(50, 100, 0), 1.0),
                                                    new SplineVertex(new Vector3(0, 100, 0), 1.0/2.0),
                                                    new SplineVertex(new Vector3(0, 0, 0), 1.0/2.0),
                                                    new SplineVertex(new Vector3(50, 0, 0), 1.0) // repeat the first point to close the circle
                                                };

            // the number of knots must be control points number + degree + 1
            // Conics are 2nd degree curves
            double[] knots = new[] {0, 0, 0, 1.0/4.0, 1.0/2.0, 1.0/2.0, 3.0/4.0, 1.0, 1.0, 1.0};
            Spline splineCircle = new Spline(circle, knots, 2);

            DxfDocument dxf = new DxfDocument();
            //dxf.AddEntity(openSpline);
            //dxf.AddEntity(closedNonPeriodicSpline);
            dxf.AddEntity(closedPeriodicSpline);
            //dxf.AddEntity(splineCircle);
            dxf.Save("spline.dxf");

        }
        private static void AddAndRemove()
        {
            Layer layer1 = new Layer("layer1") { Color = AciColor.Blue };
            Layer layer2 = new Layer("layer2") { Color = AciColor.Green };

            Line line = new Line(new Vector2(0, 0), new Vector2(10, 10));
            line.Layer = layer1;
            Circle circle = new Circle(new Vector2(0, 0), 10);
            circle.Layer = layer2;

            double offset = -0.9;
            Vector3 p1 = new Vector3(1, 2, 0);
            Vector3 p2 = new Vector3(2, 6, 0);
            Line line1 = new Line(p1, p2);
            Vector3 l1;
            Vector3 l2;
            MathHelper.OffsetLine(line1.StartPoint, line1.EndPoint, line1.Normal, offset, out l1, out l2);

            DimensionStyle myStyle = new DimensionStyle("MyDimStyle");
            myStyle.DIMPOST = "<>mm";
            AlignedDimension dim1 = new AlignedDimension(p1, p2, offset, myStyle);

            //text
            TextStyle style = new TextStyle("MyTextStyle", "Arial.ttf");
            Text text = new Text("Hello world!", Vector3.Zero, 10.0f, style)
                            {
                                Layer = new Layer("text")
                                            {
                                                Color = {Index = 8}
                                            }
                            };
            text.Alignment = TextAlignment.TopRight;

            HeaderVariables variables = new HeaderVariables
                                            {
                                                AcadVer = DxfVersion.AutoCad2004
                                            };
            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(new EntityObject[] {line, circle, dim1, text});
            dxf.Save("before remove.dxf");

            dxf.RemoveEntity(circle);
            dxf.Save("after remove.dxf");

            dxf.AddEntity(circle);
            dxf.Save("after remove and add.dxf");

            dxf.RemoveEntity(dim1);
            dxf.Save("remove dim.dxf");

            dxf.AddEntity(dim1);
            dxf.Save("add dim.dxf");

            DxfDocument dxf2 = DxfDocument.Load("dim block names.dxf");
            dxf2.AddEntity(dim1);
            dxf2.Save("dim block names2.dxf");
        }
        private static void LoadAndSave()
        {
            DxfDocument dxf = DxfDocument.Load("block sample.dxf");
            dxf.Save("block sample1.dxf");

            DxfDocument dxf2 = new DxfDocument();
            dxf2.AddEntity(dxf.Inserts[0]);
            dxf2.Save("block sample2.dxf");

            dxf.Save("clean2.dxf");
            dxf = DxfDocument.Load("clean.dxf");
            dxf.Save("clean1.dxf");

            // open a dxf saved with autocad
            dxf = DxfDocument.Load("sample.dxf");
            dxf.Save("sample4.dxf");

            Line cadLine = dxf.Lines[0];
            Layer layer = new Layer("netLayer");
            layer.Color = AciColor.Yellow;

            Line line = new Line(new Vector2(20, 40), new Vector2(100, 200));
            line.Layer = layer;
            // add a new entity to the document
            dxf.AddEntity(line);

            dxf.Save("sample2.dxf");

            DxfDocument dxf3 = new DxfDocument();
            dxf3.AddEntity(cadLine);
            dxf3.AddEntity(line);
            dxf3.Save("sample3.dxf");
        }
        private static void CleanDrawing()
        {
            DxfDocument dxf = new DxfDocument();
            dxf.Save("clean drawing.dxf");
        }
        private static void OrdinateDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 origin = new Vector3(2, 1, 0);
            Vector2 refX = new Vector2(1, 0);
            Vector2 refY = new Vector2(0, 2);
            double length = 3;
            double angle = 30;
            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            myStyle.DIMDEC = 2;

            OrdinateDimension dimX1 = new OrdinateDimension(origin, refX, length, OrdinateDimensionAxis.X, 0, myStyle);
            OrdinateDimension dimX2 = new OrdinateDimension(origin, refX, length, OrdinateDimensionAxis.X, angle, myStyle);
            OrdinateDimension dimY1 = new OrdinateDimension(origin, refY, length, OrdinateDimensionAxis.Y, 0, myStyle);
            OrdinateDimension dimY2 = new OrdinateDimension(origin, refY, length, OrdinateDimensionAxis.Y, angle, myStyle);

            dxf.AddEntity(dimX1);
            dxf.AddEntity(dimY1);
            dxf.AddEntity(dimX2);
            dxf.AddEntity(dimY2);

            Line lineX = new Line(origin, origin+5 * Vector3.UnitX);
            Line lineY = new Line(origin, origin+5 * Vector3.UnitY);

            Vector2 point = Vector2.Polar(new Vector2(origin.X, origin.Y), 5, angle * MathHelper.DegToRad);
            Line lineXRotate = new Line(origin, new Vector3(point.X, point.Y, 0));

            point = Vector2.Polar(new Vector2(origin.X, origin.Y), 5, angle * MathHelper.DegToRad + MathHelper.HalfPI);
            Line lineYRotate = new Line(origin, new Vector3(point.X, point.Y, 0));

            dxf.AddEntity(lineX);
            dxf.AddEntity(lineY);
            dxf.AddEntity(lineXRotate);
            dxf.AddEntity(lineYRotate);

            dxf.Save("ordinate dimension.dxf");

            dxf = DxfDocument.Load("ordinate dimension.dxf");
        }
        private static void Angular2LineDimensionDrawing()
        {
            double offset = 7.5;
            
            Line line1 = new Line(new Vector2(1, 2), new Vector2(6, 0));
            Line line2 = new Line(new Vector2(2, 1), new Vector2(4,5));

            Angular2LineDimension dim = new Angular2LineDimension(line1, line2, offset);
            
            DxfDocument dxf = new DxfDocument();
            //dxf.AddEntity(line1);
            //dxf.AddEntity(line2);
            //dxf.AddEntity(dim);

            Block block = new Block("DimensionBlock");
            block.Entities.Add(line1);
            block.Entities.Add(line2);
            block.Entities.Add(dim);
            Insert insert = new Insert(block);

            dxf.AddEntity(insert);

            dxf.Save("angular 2 line dimension.dxf");
            dxf = DxfDocument.Load("angular 2 line dimension.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("angular 2 line dimension.dxf");
        }
        private static void Angular3PointDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 center = new Vector3(1, 2, 0);
            double radius = 2.42548;
            Arc arc = new Arc(center, radius, -30, 60);
            //arc.Normal = new Vector3(1, 1, 1);
            DimensionStyle myStyle = new DimensionStyle("MyStyle");

            Angular3PointDimension dim = new Angular3PointDimension(arc, 5, myStyle);
            dxf.AddEntity(arc);
            dxf.AddEntity(dim);
            dxf.Save("angular 3 point dimension.dxf");

            dxf = DxfDocument.Load("angular 3 point dimension.dxf");
        }
        private static void DiametricDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 center = new Vector3(1, 2, 0);
            double radius = 2.42548;
            Circle circle = new Circle(center, radius);
            //circle.Normal = new Vector3(1, 1, 1);
            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            myStyle.DIMDEC = 2;
            myStyle.DIMDSEP = ",";

            DiametricDimension dim = new DiametricDimension(circle, 30, myStyle);
            dxf.AddEntity(circle);
            dxf.AddEntity(dim);
            dxf.Save("diametric dimension.dxf");

            dxf.RemoveEntity(dim);
            dxf.Save("diametric dimension removed.dxf");

            dxf = DxfDocument.Load("diametric dimension.dxf");
            // remove entitiy with a handle
            Dimension dimLoaded = (Dimension) dxf.GetEntityByHandle(dim.Handle);
            dxf.RemoveEntity(dimLoaded);
            dxf.Save("diametric dimension removed 2.dxf");

        }
        private static void RadialDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 center = new Vector3(1, 2, 0);
            double radius = 2.42548;
            Circle circle = new Circle(center, radius);
            circle.Normal = new Vector3(1, 1, 1);
            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            myStyle.DIMDEC = 2;
            myStyle.DIMDSEP = ",";
            
            RadialDimension dim = new RadialDimension(circle, 30, myStyle);
            dxf.AddEntity(circle);
            dxf.AddEntity(dim);
            dxf.Save("radial dimension.dxf");

            dxf = DxfDocument.Load("radial dimension.dxf");
        }
        private static void LinearDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();
            
            Vector3 p1 = new Vector3(0, 0, 0);
            Vector3 p2 = new Vector3(5, 5, 0);
            Line line = new Line(p1, p2);

            dxf.AddEntity(line);

            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            myStyle.DIMDEC = 2;
            double offset = 7;
            LinearDimension dimX = new LinearDimension(line, offset,0.0, myStyle);
            dimX.Rotation += 30.0;
            LinearDimension dimY = new LinearDimension(line, offset, 90.0, myStyle);
            dimY.Rotation += 30.0;

            XData xdata = new XData(new ApplicationRegistry("other application"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "Linear Dimension"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Long, 350));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);
            dimX.XData = new Dictionary<string, XData>
                             {
                                 {xdata.ApplicationRegistry.Name, xdata}
                             };
            dimY.XData = new Dictionary<string, XData>
                             {
                                 {xdata.ApplicationRegistry.Name, xdata}
                             };
            dxf.AddEntity(dimX);
            dxf.AddEntity(dimY);
            dxf.Save("linear dimension.dxf");
            dxf = DxfDocument.Load("linear dimension.dxf");
        }
        private static void AlignedDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();
            double offset = -0.9;
            Vector3 p1 = new Vector3(1, 2, 0);
            Vector3 p2 = new Vector3(2, 6, 0);
            Line line1 = new Line(p1, p2);
            Vector3 l1;
            Vector3 l2;
            MathHelper.OffsetLine(line1.StartPoint, line1.EndPoint, line1.Normal, offset, out l1, out l2);

            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            AlignedDimension dim1 = new AlignedDimension(p1, p2, offset, myStyle);

            Vector3 p3 = p1 + new Vector3(4, 0, 0);
            Vector3 p4 = p2 + new Vector3(4, 0, 0);
            Line line2 = new Line(p3, p4);
            AlignedDimension dim2 = new AlignedDimension(p3, p4, -offset, myStyle);


            Vector2 perp = Vector2.Perpendicular(new Vector2(p2.X, p2.Y) - new Vector2(p1.X, p1.Y));
            //dim.Normal = -new Vector3(perp.X, perp.Y, 0.0) ;

            XData xdata = new XData(new ApplicationRegistry("other application"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "Aligned Dimension"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Long, 350));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);
            dim1.XData = new Dictionary<string, XData>
                             {
                                 {xdata.ApplicationRegistry.Name, xdata}
                             };

            //dxf.AddEntity(line1);
            //dxf.AddEntity(line2);
            //dxf.AddEntity(dim1);
            //dxf.AddEntity(dim2);



            Block block = new Block("DimensionBlock");
            block.Entities.Add(line1);
            block.Entities.Add(line2);
            block.Entities.Add(dim1);
            block.Entities.Add(dim2);
            Insert insert = new Insert(block);
            dxf.AddEntity(insert);

            dxf.Save("aligned dimension.dxf");

            dxf = DxfDocument.Load("aligned dimension.dxf");

        }
        private static void WriteMText()
        {
            DxfDocument dxf = new DxfDocument();

            //xData sample
            XData xdata = new XData(new ApplicationRegistry("netDxf"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionX, 0));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionY, 0));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionZ, 0));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);

            //text
            TextStyle style = new TextStyle("Times.ttf");
            //TextStyle style = TextStyle.Default;
            MText mText = new MText(new Vector3(3,2,0), 1.0f, 100.0f, style);
            mText.Layer = new Layer("Multiline Text");
            //mText.Layer.Color.Index = 8;
            mText.Rotation = 0;
            mText.LineSpacingFactor = 1.0;
            mText.ParagraphHeightFactor = 1.0;

            //mText.AttachmentPoint = MTextAttachmentPoint.TopCenter;
            //mText.Write("Hello World!");
            //mText.Write(" we keep writting on the same line.");
            //mText.WriteLine("This text is in a new line");

            //mText.Write("Hello World! ");
            //for (int i = 0; i < 50; i++)
            //{
            //    mText.Write("1234567890");
            //}
            //mText.Write(" This text is over the limit of the 250 character chunk");
            //mText.NewParagraph();
            //mText.Write("This is a text in a new paragraph");
            //mText.Write(" and we continue writing in the previous paragraph");
            //mText.NewParagraph();
            MTextFormattingOptions options = new MTextFormattingOptions(mText.Style);
            options.Bold = true;
            mText.Write("Bold text in mText.Style", options);
            mText.EndParagraph();
            options.Italic = true;
            mText.Write("Bold and italic text in mText.Style", options);
            mText.EndParagraph();
            options.Bold = false;
            options.FontName = "Arial";
            options.Color = AciColor.Blue;
            mText.ParagraphHeightFactor = 2;
            mText.Write("Italic text in Arial", options);
            mText.EndParagraph();
            options.Italic = false;
            options.Color = null; // back to the default text color
            mText.Write("Normal text in Arial with the default paragraph height factor", options);
            mText.EndParagraph();
            mText.ParagraphHeightFactor = 1;
            mText.Write("No formatted text uses mText.Style");
            mText.Write(" and the text continues in the same paragraph.");
            mText.EndParagraph();

            //options.HeightPercentage = 2.5;
            //options.Color = AciColor.Red;
            //options.Overstrike = true;
            //options.Underline = true;
            //options.FontFile = "times.ttf";
            //options.ObliqueAngle = 15;
            //options.CharacterSpacePercentage = 2.35;
            //options.WidthFactor = 1.8;
            
            //for unknown reasons the aligment doesn't seem to change anything
            //mText.Write("Formatted text", options);
            //options.Aligment = MTextFormattingOptions.TextAligment.Center;
            //mText.Write("Center", options);
            //options.Aligment = MTextFormattingOptions.TextAligment.Top;
            //mText.Write("Top", options);
            //options.Aligment = MTextFormattingOptions.TextAligment.Bottom;
            //mText.Write("Bottom", options);

            mText.XData = new Dictionary<string, XData>
                             {
                                 {xdata.ApplicationRegistry.Name, xdata}
                             };
            
            dxf.AddEntity(mText);

            dxf.Save("MText sample.dxf");

        }
        private static void HatchCircleBoundary()
        {
            DxfDocument dxf = new DxfDocument();

            // create a circle that will be our hatch boundary in this case it is a circle with center (5.5, -5.5, 0.0) and a radius 10.0
            Circle circle = new Circle(new Vector3(5.5, -5.5, 0), 10);

            // create the hatch boundary path with only the circle (a circle is already a closed loop it is all we need to define a valid boundary path)
            // a hatch can have many boundaries (closed loops) and every boundary path can be made of several entities (lines, polylines, arcs, circles and ellipses)
            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>
                                                        {
                                                            new HatchBoundaryPath(new List<EntityObject>{circle})
                                                        };  

            // create the hatch in this case we will use the predefined Solid hatch pattern and our circle as the boundary path
            Hatch hatch = new Hatch(HatchPattern.Solid, boundary);

            // to give a color to the hatch, we have to options:

            // create a new layer with a color for the hatch (in this case by default the hatch will have a ByLayer color)
            //Layer hatchLayer = new Layer("HathLayer") {Color = AciColor.Green};
            //hatch.Layer = hatchLayer;

            // or give the hatch a color just for it
            // old AutoCAD versions only had 255 colors (indexed color), now in AutoCAD you can use true colors (8 bits per channel) but at the moment this is not supported.
            // if you try to give r, g, b values to define a color it will be converted to an indexed color
            // (I haven't tested this code a lot, so errors might appear and the result might not be what you expected).
            hatch.Color = AciColor.Red;

            // the hatch by itself will not show the boundary, but we can use the same entity to show the limits of the hatch, adding it to the document 
            dxf.AddEntity(circle);

            // add the hatch to the document
            dxf.AddEntity(hatch);

            dxf.Save("circle solid fill.dxf");
        }
        private static void LineWidth()
        {
            // the line thickness works as expected, according to the AutoCAD way of doing things
            Line thickLine = new Line(new Vector3(0,10,0),  new Vector3(10,20,0));

            // when you assign a thickness to a line, the result is like a wall, it is like a 3d face whose vertexes are defined by the
            // start and end points of the line and the thickness along the normal of the line.
            thickLine.Thickness = 5;

            // maybe what you are trying to do is create a line with a width (something that we can read it as a line with thickness), the only way to do this is to create a polyline
            // the kind of result you will get if you give a width to a 2d polyline 
            // you can only give a width to a vertex of a Polyline or a LightweigthPolyline
            // I am planning to drop support to AutoCAD 12 dxf files, so to define a bidimensional polyline the only way will be to use lightweight polyline
            // (the Polyline class and the LightWeightPolyline are basically the same).
            LwPolyline widthLine = new LwPolyline();
            LwPolylineVertex startVertex = new LwPolylineVertex(new Vector2(0, 0));
            LwPolylineVertex endVertex = new LwPolylineVertex(new Vector2(10, 10));
            widthLine.Vertexes = new List<LwPolylineVertex> { startVertex, endVertex };

            // the easy way to give a constant width to a polyline, but you can also give a polyline width by vertex
            // there is a mistake on my part, following the AutoCAD documentation I should have called the PolylineVertex.StartThickness and PolylineVertex.EndThickness as
            // PolylineVertex.StartWidth and PolylineVertex.EndWidth
            // SetConstantWidth is a sort cut that will asign the given value to every start width and end width of every vertex of the polyline
            widthLine.SetConstantWidth(0.5);

            DxfDocument dxf = new DxfDocument();

            // add the entities to the document (both of them to see the difference)
            dxf.AddEntity(thickLine);
            dxf.AddEntity(widthLine);

            dxf.Save("line width.dxf");

        }
        private static void ToPolyline()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 center = new Vector3(1, 8, -7);
            Vector3 normal = new Vector3(1, 1, 1);

            Circle circle = new Circle(center, 7.5);
            circle.Normal = normal;

            Arc arc = new Arc(center, 5, 30, 215);
            arc.Normal = normal;


            Ellipse ellipse = new Ellipse(center, 15, 7.5);
            ellipse.Rotation = 35;
            ellipse.Normal = normal;

            Ellipse ellipseArc = new Ellipse(center, 10, 5);
            ellipseArc.StartAngle = 30;
            ellipseArc.EndAngle = 325;
            ellipseArc.Rotation = 35;
            ellipseArc.Normal = normal;

            dxf.AddEntity(circle);
            dxf.AddEntity(circle.ToPolyline(10));

            dxf.AddEntity(arc);
            dxf.AddEntity(arc.ToPolyline(10));

            dxf.AddEntity(ellipse);
            dxf.AddEntity(ellipse.ToPolyline(10));

            dxf.AddEntity(ellipseArc);
            dxf.AddEntity(ellipseArc.ToPolyline(10));

            dxf.Save("to polyline.dxf");

            dxf = DxfDocument.Load("to polyline.dxf");

            dxf.Save("to polyline2.dxf");
        }
        private static void CustomHatchPattern()
        {
            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.Vertexes[2].Bulge = 1;
            poly.IsClosed = true;

            LwPolyline poly2 = new LwPolyline();
            poly2.Vertexes.Add(new LwPolylineVertex(-5, -5));
            poly2.Vertexes.Add(new LwPolylineVertex(5, -5));
            poly2.Vertexes.Add(new LwPolylineVertex(5, 5));
            poly2.Vertexes.Add(new LwPolylineVertex(-5, 5));
            poly2.Vertexes[1].Bulge = -0.25;
            poly2.IsClosed = true;

            LwPolyline poly3 = new LwPolyline();
            poly3.Vertexes.Add(new LwPolylineVertex(-8, -8));
            poly3.Vertexes.Add(new LwPolylineVertex(-6, -8));
            poly3.Vertexes.Add(new LwPolylineVertex(-6, -6));
            poly3.Vertexes.Add(new LwPolylineVertex(-8, -6));
            poly3.IsClosed = true;

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly2}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly3}),
                                                                          };

            HatchPattern pattern = new HatchPattern("MyPattern", "A custom hatch pattern");

            HatchPatternLineDefinition line1 = new HatchPatternLineDefinition();
            line1.Angle = 45;
            line1.Origin = Vector2.Zero;
            line1.Delta=new Vector2(4,4);
            line1.DashPattern.Add(12);
            line1.DashPattern.Add(-4);
            pattern.LineDefinitions.Add(line1);

            HatchPatternLineDefinition line2 = new HatchPatternLineDefinition();
            line2.Angle = 135;
            line2.Origin = new Vector2(2.828427125, 2.828427125);
            line2.Delta = new Vector2(4,-4);
            line2.DashPattern.Add(12);
            line2.DashPattern.Add(-4);
            pattern.LineDefinitions.Add(line2);

            Hatch hatch = new Hatch(pattern, boundary);
            hatch.Layer = new Layer("hatch")
            {
                Color = AciColor.Red,
                LineType = LineType.Continuous
            };
            hatch.Pattern.Angle = 0;
            hatch.Pattern.Scale = 1;
            dxf.AddEntity(poly);
            dxf.AddEntity(poly2);
            dxf.AddEntity(poly3);
            dxf.AddEntity(hatch);

            dxf.Save("hatchTest.dxf");
        }
        private static void FilesTest()
        {
            LineType lineType = LineType.FromFile("acad.lin", "ACAD_ISO15W100");
            HatchPattern hatch = HatchPattern.FromFile("acad.pat", "zigzag");

        }
        private static void LoadSaveHatchTest()
        {
            DxfDocument dxf = DxfDocument.Load("Hatch2.dxf");
            dxf.Save("HatchTest.dxf");
        }
        private static void ExplodeTest()
        {
            DxfDocument dxf = new DxfDocument();
            //polyline
            LwPolylineVertex polyVertex;
            List<LwPolylineVertex> polyVertexes = new List<LwPolylineVertex>();
            polyVertex = new LwPolylineVertex(new Vector2(-50, -23.5));
            polyVertex.Bulge = 1.33;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(34.8, -42.7));
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(65.3, 54.7));
            polyVertex.Bulge = -0.47;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(-48.2, 42.5));
            polyVertexes.Add(polyVertex);
            LwPolyline polyline2d = new LwPolyline(polyVertexes);
            polyline2d.Layer = new Layer("polyline2d");
            polyline2d.Layer.Color.Index = 5;
            polyline2d.Normal = new Vector3(1, 1, 1);
            polyline2d.Elevation = 100.0f;

            dxf.AddEntity(polyline2d);
            dxf.AddEntity(polyline2d.Explode());

            dxf.Save("explode.dxf");
        }
        private static void HatchTestLinesBoundary()
        {
            DxfDocument dxf = new DxfDocument();

            Line line1 = new Line(new Vector3(-10,-10,0),new Vector3(10,-10,0));
            Line line2 = new Line(new Vector3(10, -10, 0), new Vector3(10, 10, 0));
            Line line3 = new Line(new Vector3(10, 10, 0), new Vector3(-10, 10, 0));
            Line line4 = new Line(new Vector3(-10, 10, 0), new Vector3(-10, -10, 0));


            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{line1, line2, line3, line4})
                                                                          };
            Hatch hatch = new Hatch(HatchPattern.Line, boundary);
            hatch.Layer = new Layer("hatch")
            {
                Color = AciColor.Red,
                LineType = LineType.Dashed
            };
            hatch.Pattern.Angle = 45;

            XData xdata = new XData(new ApplicationRegistry("netDxf"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "netDxf hatch"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Distance, hatch.Pattern.Scale));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Real, hatch.Pattern.Angle));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);

            hatch.XData = new Dictionary<string, XData>
                             {
                                 {xdata.ApplicationRegistry.Name, xdata},
                             };

            dxf.AddEntity(line1);
            dxf.AddEntity(line2);
            dxf.AddEntity(line3);
            dxf.AddEntity(line4);
            dxf.AddEntity(hatch);

            dxf.Save("hatchTest.dxf");
        }
        private static void HatchTest1()
        {
            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.Vertexes[2].Bulge = 1;
            poly.IsClosed = true;

            LwPolyline poly2 = new LwPolyline();
            poly2.Vertexes.Add(new LwPolylineVertex(-5, -5));
            poly2.Vertexes.Add(new LwPolylineVertex(5, -5));
            poly2.Vertexes.Add(new LwPolylineVertex(5, 5));
            poly2.Vertexes.Add(new LwPolylineVertex(-5, 5));
            poly2.Vertexes[1].Bulge = -0.25;
            poly2.IsClosed = true;

            LwPolyline poly3 = new LwPolyline();
            poly3.Vertexes.Add(new LwPolylineVertex(-8, -8));
            poly3.Vertexes.Add(new LwPolylineVertex(-6, -8));
            poly3.Vertexes.Add(new LwPolylineVertex(-6, -6));
            poly3.Vertexes.Add(new LwPolylineVertex(-8, -6));
            poly3.IsClosed = true;

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly2}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly3}),
                                                                          };
            Hatch hatch = new Hatch(HatchPattern.Net, boundary);
            hatch.Layer = new Layer("hatch")
                              {
                                  Color = AciColor.Red,
                                  LineType = LineType.Continuous
                              };
            hatch.Pattern.Angle = 30;

            hatch.Pattern.Scale = 1 / hatch.Pattern.LineDefinitions[0].Delta.Y;
            dxf.AddEntity(poly);
            dxf.AddEntity(poly2);
            dxf.AddEntity(poly3);
            dxf.AddEntity(hatch);

            dxf.Save("hatchTest1.dxf");
            dxf = DxfDocument.Load("hatchTest1.dxf");
        }
        private static void HatchTest2()
        {
            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.Vertexes[2].Bulge = 1;
            poly.IsClosed = true;

            Circle circle = new Circle(Vector3.Zero, 5);

            Ellipse ellipse = new Ellipse(Vector3.Zero,16,10);
            ellipse.Rotation = 30;
            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{circle}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{ellipse})
                                                                          };

            Hatch hatch = new Hatch(HatchPattern.Line, boundary);
            hatch.Pattern.Angle = 150;
            hatch.Pattern.Scale = 5;

            dxf.AddEntity(poly);
            dxf.AddEntity(circle);
            dxf.AddEntity(ellipse);
            dxf.AddEntity(hatch);

            dxf.Save("hatchTest2.dxf");
            dxf = DxfDocument.Load("hatchTest2.dxf");
            dxf.Save("hatchTest2 copy.dxf");
        }
        private static void HatchTest3()
        {
            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.Vertexes[2].Bulge = 1;
            poly.IsClosed = true;

            Ellipse ellipse = new Ellipse(Vector3.Zero, 16, 10);
            ellipse.Rotation = 0;
            ellipse.StartAngle = 0;
            ellipse.EndAngle = 180;

            LwPolyline poly2 = new LwPolyline();
            poly2.Vertexes.Add(new LwPolylineVertex(-8, 0));
            poly2.Vertexes.Add(new LwPolylineVertex(0, -4));
            poly2.Vertexes.Add(new LwPolylineVertex(8, 0));

            Arc arc = new Arc(Vector3.Zero,8,180,0);
            Line line =new Line(new Vector3(8,0,0), new Vector3(-8,0,0));

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly2, ellipse})
                                                                          };

            Hatch hatch = new Hatch(HatchPattern.Line, boundary);
            hatch.Pattern.Angle = 45;
            dxf.AddEntity(poly);
            dxf.AddEntity(ellipse);
            //dxf.AddEntity(arc);
            //dxf.AddEntity(line);
            dxf.AddEntity(poly2);
            dxf.AddEntity(hatch);

            dxf.Save("hatchTest3.dxf");
            dxf = DxfDocument.Load("hatchTest3.dxf");
        }
        private static void Dxf2000()
        {
            DxfDocument dxf = new DxfDocument();
           //line
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(5, 5, 5));
            line.Layer = new Layer("line");
            line.Layer.Color.Index = 6;

            dxf.AddEntity(line);

            dxf.Save("test2000.dxf");
        }
        private static void LwPolyline()
        {

            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(0, 0));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(20, 0));
            poly.Vertexes.Add(new LwPolylineVertex(30, 10));
            poly.SetConstantWidth(2);
            //poly.IsClosed = true;
            dxf.AddEntity(poly);

            dxf.Save("lwpolyline.dxf");

            dxf = DxfDocument.Load("lwpolyline.dxf");
        }
        private static void Polyline()
        {

            DxfDocument dxf = new DxfDocument();
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            Polyline poly = new Polyline();
            poly.Vertexes.Add(new PolylineVertex(0, 0, 0));
            poly.Vertexes.Add(new PolylineVertex(10, 10, 0));
            poly.Vertexes.Add(new PolylineVertex(20, 0, 0));
            poly.Vertexes.Add(new PolylineVertex(30, 10, 0));
            dxf.AddEntity(poly);

            dxf.Save("polyline.dxf");

        }
        private static void Solid()
        {

            DxfDocument dxf = new DxfDocument();

            Solid solid = new Solid();
            solid.FirstVertex=new Vector3(0,0,0);
            solid.SecondVertex  = new Vector3(1, 0, 0);
            solid.ThirdVertex  = new Vector3(0, 1, 0);
            solid.FourthVertex  = new Vector3(1, 1, 0);
            dxf.AddEntity(solid);

            dxf.Save("solid.dxf");
            //dxf = DxfDocument.Load("solid.dxf");
            //dxf.Save("solid.dxf");

        }
        private static void Face3d()
        {

            DxfDocument dxf = new DxfDocument();

            Face3d face3d = new Face3d();
            face3d.FirstVertex = new Vector3(0, 0, 0);
            face3d.SecondVertex = new Vector3(1, 0, 0);
            face3d.ThirdVertex = new Vector3(1, 1, 0);
            face3d.FourthVertex = new Vector3(0, 1, 0);
            dxf.AddEntity(face3d);

            dxf.Save("face.dxf");
            dxf = DxfDocument.Load("face.dxf");
            dxf.Save("face return.dxf");

        }
        private static void Ellipse()
        {
           
            DxfDocument dxf = new DxfDocument();

            Line line = new Line(new Vector3(0, 0, 0), new Vector3(2 * Math.Cos(Math.PI / 4),2 * Math.Cos(Math.PI / 4), 0));

            dxf.AddEntity(line);

            Line line2 = new Line(new Vector3(0, 0, 0), new Vector3(0, -2, 0));
            dxf.AddEntity(line2);

            Arc arc=new Arc(Vector3.Zero,2,45,270);
            dxf.AddEntity(arc);

            // ellipses are saved as polylines
            Ellipse ellipse = new Ellipse(new Vector3(2,2,0), 5,3);
            ellipse.Rotation = 30;
            ellipse.Normal=new Vector3(1,1,1);
            ellipse.Thickness = 2;
            dxf.AddEntity(ellipse);


            dxf.Save("ellipse.dxf");
            dxf = new DxfDocument();
            dxf = DxfDocument.Load("ellipse.dxf");
           
        }
        private static void SpeedTest()
        {
            Stopwatch crono = new Stopwatch();
            const int numLines = 100000; // create # lines
            float totalTime=0;
            
            List<EntityObject> lines = new List<EntityObject>(numLines);
            DxfDocument dxf = new DxfDocument();

            crono.Start();
            for (int i = 0; i < numLines; i++)
            {
                 //line
                Line line = new Line(new Vector3(0, i, 0), new Vector3(5, i, 0));
                line.Layer = new Layer("line");
                line.Layer.Color.Index = 6;
                lines.Add(line);
            }
            Console.WriteLine("Time creating entities : " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Reset();

            crono.Start();
            dxf.AddEntity(lines);
            Console.WriteLine("Time adding entities to document : " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Reset();

            crono.Start();
            dxf.Save("speedtest.dxf");
            Console.WriteLine("Time saving file : " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Reset();

            crono.Start();
            dxf = DxfDocument.Load("speedtest.dxf");
            Console.WriteLine("Time loading file : " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Stop();

            Console.WriteLine("Total time : " + totalTime / 1000.0f);
            Console.ReadLine();

        }
        private static void BlockAttributes()
        {
            DxfDocument dxf = new DxfDocument( );
            Block block = new Block("BlockWithAttributes");
            block.Layer = new Layer("BlockSample");
            block.Position = new Vector3(10, 5, 0);

            AttributeDefinition attdef = new AttributeDefinition("NewAttribute");
            attdef.Text = "InfoText";
            // the attribute position is in local coordinates to the Insert entity to which it belongs
            attdef.Position = new Vector3(1, 1, 0);
            // modifying directly the text style might not get the desired results. Create one or get one from the text style table, modify it and assign it to the attribute text style.
            // one thing to note, if there is already a text style with the assigned name, the existing one in the text style table will override the new one.
            //attdef.Style.IsVertical = true;

            TextStyle txt = new TextStyle("MyStyle", "complex.shx");
            txt.IsVertical = true;
            attdef.Style = txt;
            
            attdef.WidthFactor = 2;
            // not all alignment options are avaliable for ttf fonts 
            attdef.Alignment = TextAlignment.MiddleCenter;
            attdef.Rotation = 0;

            // the attribute normal will use the one applied to the Insert entity to which it belongs
            // this is subject to change if I find a way to get predictable results even when inserting new blocks in AutoCAD
            //attdef.Normal = new Vector3(1, 0, 0);

            block.Attributes.Add(attdef.Id, attdef);
            block.Entities.Add(new Line(new Vector3(-5, -5, 0), new Vector3(5, 5, 0)));
            block.Entities.Add(new Line(new Vector3(5, -5, 0), new Vector3(-5, 5, 0)));

            Insert insert = new Insert(block, new Vector3(5, 5, 5))
                                {
                                    Layer = new Layer("insert"),
                                    Normal = new Vector3(1, 1, 1),
                                    Rotation = 45
                                };
            // since we have changes the insert position, rotation, normal and/or scale we need to transform the insert attributes
            insert.TransformAttributes();

            // the insert rotation will also affect the attributes.
            insert.Layer.Color.Index = 4;
            insert.Attributes[0].Value = 24;

            Insert insert2 = new Insert(block, new Vector3(0, 0, 0)) //new Vector3(-5, -5, -5))
                                 {
                                     Rotation = 45,
                                     //Scale = new Vector3(1, 2, 1)
                                 };
            //insert2.Normal = new Vector3(1, 1, 1);
            // since we have changes the insert position, rotation, normal and/or scale we need to transform the insert attributes
            insert2.TransformAttributes();
            
            insert2.Attributes[0].Value = 34;
            

            XData xdata1 = new XData(new ApplicationRegistry("netDxf"));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata1.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionX, 0));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionY, 0));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionZ, 0));
            xdata1.XDataRecord.Add(XDataRecord.CloseControlString);

            XData xdata2 = new XData(new ApplicationRegistry("other application"));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata2.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "string record"));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Long, 350));
            xdata2.XDataRecord.Add(XDataRecord.CloseControlString);

            insert.XData = new Dictionary<string, XData>
                             {
                                 {xdata1.ApplicationRegistry.Name, xdata1},
                             };
            dxf.AddEntity(insert);
            dxf.AddEntity(insert2);

            Circle circle = new Circle(Vector3.Zero, 5);
            circle.Layer = new Layer("circle");
            circle.Layer.Color.Index = 2;
            circle.XData = new Dictionary<string, XData>
                             {
                                 {xdata2.ApplicationRegistry.Name, xdata2},
                             };
            dxf.AddEntity(circle);

            dxf.Save("Block with attributes.dxf");
            dxf = DxfDocument.Load("Block with attributes.dxf");
            dxf.Save("Block with attributes 2.dxf");
        }
        private static void WriteNestedInsert()
        {
            // nested blocks
            DxfDocument dxf = new DxfDocument();
            
            Block nestedBlock = new Block("Nested block");
            Circle circle = new Circle(Vector3.Zero, 5);
            circle.Layer = new Layer("circle");
            circle.Layer.Color.Index = 2;
            nestedBlock.Entities.Add(circle);
            
            AttributeDefinition attdef = new AttributeDefinition("NewAttribute");
            attdef.Text = "InfoText";
            attdef.Alignment = TextAlignment.MiddleCenter;
            nestedBlock.Attributes.Add(attdef.Id, attdef);
            Insert nestedInsert = new Insert(nestedBlock, new Vector3(0, 0, 0)); // the position will be relative to the position of the insert that nest it
            nestedInsert.Attributes[0].Value = 24;

            Insert nestedInsert2 = new Insert(nestedBlock, new Vector3(-20, 0, 0)); // the position will be relative to the position of the insert that nest it
            nestedInsert2.Attributes[0].Value = -20;

            Block block = new Block("MyBlock");
            block.Entities.Add(new Line(new Vector3(-5, -5, 0), new Vector3(5, 5, 0)));
            block.Entities.Add(new Line(new Vector3(5, -5, 0), new Vector3(-5, 5, 0)));
            block.Entities.Add(nestedInsert);
            block.Entities.Add(nestedInsert2);

            Insert insert = new Insert(block, new Vector3(5, 5, 5));
            insert.Layer = new Layer("insert");

            dxf.AddEntity(insert);
            //dxf.AddEntity(circle); // this is not allowed the circle is already part of a block

            dxf.Save("nested insert.dxf");
            dxf = DxfDocument.Load("nested insert.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("nested insert copy.dxf");

        }
        private static void WritePolyfaceMesh()
        {
            DxfDocument dxf = new DxfDocument();


            List<PolyfaceMeshVertex> vertexes = new List<PolyfaceMeshVertex>
                                                    {
                                                        new PolyfaceMeshVertex(0, 0, 0),
                                                        new PolyfaceMeshVertex(10, 0, 0),
                                                        new PolyfaceMeshVertex(10, 10, 0),
                                                        new PolyfaceMeshVertex(5, 15, 0),
                                                        new PolyfaceMeshVertex(0, 10, 0)
                                                    };
            List<PolyfaceMeshFace> faces = new List<PolyfaceMeshFace>
                                               {
                                                   new PolyfaceMeshFace(new[] {1, 2, -3}),
                                                   new PolyfaceMeshFace(new[] {-1, 3, -4}),
                                                   new PolyfaceMeshFace(new[] {-1, 4, 5})
                                               };

            PolyfaceMesh mesh = new PolyfaceMesh(vertexes, faces);
            dxf.AddEntity(mesh);

            dxf.Save("mesh.dxf");
        }
        private static void WriteDxfFile()
        {
            DxfDocument dxf = new DxfDocument();

            //arc
            Arc arc = new Arc(new Vector3(10, 10, 0), 10, 45, 135);
            arc.Layer = new Layer("arc");
            arc.Layer.Color.Index = 1;
            dxf.AddEntity(arc);

            //xData sample
            XData xdata = new XData(new ApplicationRegistry("netDxf"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionX, 0));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionY, 0));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionZ, 0));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);

            XData xdata2 = new XData(new ApplicationRegistry("other application"));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata2.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "string record"));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Long, 350));
            xdata2.XDataRecord.Add(XDataRecord.CloseControlString);

            //circle
            Vector3 extrusion = new Vector3(1, 1, 1);
            Vector3 centerWCS = new Vector3(1, 1, 1);
            Vector3 centerOCS = MathHelper.Transform(centerWCS,
                                                      extrusion,
                                                      MathHelper.CoordinateSystem.World,
                                                      MathHelper.CoordinateSystem.Object);

            Circle circle = new Circle(centerOCS, 5);
            circle.Layer = new Layer("circle with spaces");
            circle.Layer.Color=AciColor.Yellow;
            circle.LineType = LineType.Dashed;
            circle.Normal = extrusion;
            circle.XData=new Dictionary<string, XData>
                             {
                                 {xdata.ApplicationRegistry.Name, xdata},
                                 {xdata2.ApplicationRegistry.Name, xdata2}
                             };

            dxf.AddEntity(circle);

            //points
            Point point1 = new Point(new Vector3(-3, -3, 0));
            point1.Layer = new Layer("point");
            point1.Color = new AciColor(30);
            Point point2 = new Point(new Vector3(1, 1, 1));
            point2.Layer = point1.Layer;
            point2.Layer.Color.Index = 9;
            point2.Normal = new Vector3(1, 1, 1);
            dxf.AddEntity(point1);
            dxf.AddEntity(point2);

            //3dface
            Face3d face3D = new Face3d(new Vector3(-5, -5, 5),
                                       new Vector3(5, -5, 5),
                                       new Vector3(5, 5, 5),
                                       new Vector3(-5, 5, 5));
            face3D.Layer = new Layer("3dface");
            face3D.Layer.Color.Index = 3;
            dxf.AddEntity(face3D);
            
            //polyline
            LwPolylineVertex polyVertex;
            List<LwPolylineVertex> polyVertexes = new List<LwPolylineVertex>();
            polyVertex = new LwPolylineVertex(new Vector2(-50, -50));
            polyVertex.BeginWidth = 2;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(50, -50));
            polyVertex.BeginWidth = 1;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(50, 50));
            polyVertex.Bulge = 1;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(-50, 50));
            polyVertexes.Add(polyVertex);
            LwPolyline polyline2d = new LwPolyline(polyVertexes, true);
            polyline2d.Layer = new Layer("polyline2d");
            polyline2d.Layer.Color.Index = 5;
            polyline2d.Normal = new Vector3(1, 1, 1);
            polyline2d.Elevation = 100.0f;
            dxf.AddEntity(polyline2d);

            //lightweight polyline
            LwPolylineVertex lwVertex;
            List<LwPolylineVertex> lwVertexes = new List<LwPolylineVertex>();
            lwVertex = new LwPolylineVertex(new Vector2(-25, -25));
            lwVertex.BeginWidth = 2;
            lwVertexes.Add(lwVertex);
            lwVertex = new LwPolylineVertex(new Vector2(25, -25));
            lwVertex.BeginWidth = 1;
            lwVertexes.Add(lwVertex);
            lwVertex = new LwPolylineVertex(new Vector2(25, 25));
            lwVertex.Bulge = 1;
            lwVertexes.Add(lwVertex);
            lwVertex = new LwPolylineVertex(new Vector2(-25, 25));
            lwVertexes.Add(lwVertex);
            LwPolyline lwPolyline = new LwPolyline(lwVertexes, true);
            lwPolyline.Layer = new Layer("lwpolyline");
            lwPolyline.Layer.Color.Index = 5;
            lwPolyline.Normal = new Vector3(1, 1, 1);
            lwPolyline.Elevation = 100.0f;
            dxf.AddEntity(lwPolyline);

            // polyfaceMesh
            List<PolyfaceMeshVertex> meshVertexes = new List<PolyfaceMeshVertex>
                                                    {
                                                        new PolyfaceMeshVertex(0, 0, 0),
                                                        new PolyfaceMeshVertex(10, 0, 0),
                                                        new PolyfaceMeshVertex(10, 10, 0),
                                                        new PolyfaceMeshVertex(5, 15, 0),
                                                        new PolyfaceMeshVertex(0, 10, 0)
                                                    };
            List<PolyfaceMeshFace> faces = new List<PolyfaceMeshFace>
                                               {
                                                   new PolyfaceMeshFace(new[] {1, 2, -3}),
                                                   new PolyfaceMeshFace(new[] {-1, 3, -4}),
                                                   new PolyfaceMeshFace(new[] {-1, 4, 5})
                                               };

            PolyfaceMesh mesh = new PolyfaceMesh(meshVertexes, faces);
            mesh.Layer = new Layer("polyfacemesh");
            mesh.Layer.Color.Index = 104;
            dxf.AddEntity(mesh);

            //line
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(10, 10, 10));
            line.Layer = new Layer("line");
            line.Layer.Color.Index = 6;
            dxf.AddEntity(line);

            //3d polyline
            PolylineVertex vertex;
            List<PolylineVertex> vertexes = new List<PolylineVertex>();
            vertex = new PolylineVertex(new Vector3(-50, -50, 0));
            vertexes.Add(vertex);
            vertex = new PolylineVertex(new Vector3(50, -50, 10));
            vertexes.Add(vertex);
            vertex = new PolylineVertex(new Vector3(50, 50, 25));
            vertexes.Add(vertex);
            vertex = new PolylineVertex(new Vector3(-50, 50, 50));
            vertexes.Add(vertex);
            Polyline polyline = new Polyline(vertexes, true);
            polyline.Layer = new Layer("polyline3d");
            polyline.Layer.Color.Index = 24;
            dxf.AddEntity(polyline);

            //block definition
            Block block = new Block("TestBlock");
            block.Entities.Add(new Line(new Vector3(-5, -5, 5), new Vector3(5, 5, 5)));
            block.Entities.Add(new Line(new Vector3(5, -5, 5), new Vector3(-5, 5, 5)));
           
            //insert
            Insert insert = new Insert(block, new Vector3(5, 5, 5));
            insert.Layer = new Layer("insert");
            insert.Layer.Color.Index = 4;
            dxf.AddEntity(insert);

            //text
            TextStyle style=new TextStyle("True type font","Arial.ttf");
            Text text = new Text("Hello world!", Vector3.Zero, 10.0f,style);
            text.Layer = new Layer("text");
            text.Layer.Color.Index = 8;
            text.Alignment = TextAlignment.TopRight;
            dxf.AddEntity(text);

            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("AutoCad2010.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2007;
            dxf.Save("AutoCad2007.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            dxf.Save("AutoCad2004.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;
            dxf.Save("AutoCad2000.dxf");
            dxf = DxfDocument.Load("AutoCad2000.dxf");
            dxf.Save("AutoCad2000 result.dxf");
        }
        private static void WritePolyline3d()
        {
            DxfDocument dxf = new DxfDocument();

            List<PolylineVertex> vertexes = new List<PolylineVertex>{
                                                                        new PolylineVertex(0, 0, 0), 
                                                                        new PolylineVertex(10, 0, 10), 
                                                                        new PolylineVertex(10, 10, 20), 
                                                                        new PolylineVertex(0, 10, 30)
                                                                        };

            Polyline poly = new Polyline(vertexes, true);

            XData xdata = new XData(new ApplicationRegistry("netDxf"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "netDxf polyline3d"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Integer, poly.Vertexes.Count));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);

            poly.XData = new Dictionary<string, XData>
                             {
                                 {xdata.ApplicationRegistry.Name, xdata},
                             }; 
            dxf.AddEntity(poly);

            dxf.Save("polyline.dxf");

            
        }
    }
}