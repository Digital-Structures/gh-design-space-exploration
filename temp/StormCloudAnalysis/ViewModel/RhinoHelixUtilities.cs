using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;


namespace StormCloud.ViewModel
{
    public static class RhinoHelixUtilities
    {


        public static Point3D RhinoToHelixPoint(Point3d pt)
        {
            return new Point3D(pt.X, pt.Y, pt.Z);
        }


        // LINES, POLYLINES, CURVES

        // simple line
        public static Model3D RhinoToHelixLine(Rhino.Geometry.Curve curve, double diameter, int resolutiontube, System.Windows.Media.Media3D.Material mat) 
        {
            var modelGroup = new Model3DGroup();
            var meshBuilder = new MeshBuilder(false, false);
            //Tube path
            List<Point3D> path = new List<Point3D>();
            path.Add(RhinoToHelixPoint(curve.PointAtStart));
            path.Add(RhinoToHelixPoint(curve.PointAtEnd));
            //Helix mesh creation
            meshBuilder.AddTube(path, diameter, resolutiontube, true);
            var meshHelix = meshBuilder.ToMesh(true);
            modelGroup.Children.Add(new GeometryModel3D { Geometry = meshHelix, Material = mat, BackMaterial = mat });

            return modelGroup;
        }

        // general curve
        public static Model3D RhinoToHelixCurve(Rhino.Geometry.Curve curve, double diameter, int resolution, int resolutiontube, System.Windows.Media.Media3D.Material mat)
        {

            int curvedegree = curve.Degree;
            var modelGroup = new Model3DGroup();

            Console.WriteLine("BS");

            if (curve.IsLinear()) // if line-like curve
            {
                Console.WriteLine("blabla");
                return RhinoToHelixLine(curve, diameter, resolutiontube, mat);
            }

            else
            {
                var meshBuilder = new MeshBuilder(false, false);

                //Point3d[] pts = new Point3d[resolution];
                List<Point3D> path = new List<Point3D>();
                //curve.DivideByCount(resolution, true, out pts);
                path.Add(RhinoToHelixPoint(curve.PointAtEnd));
                path.Add(RhinoToHelixPoint(curve.PointAtStart));
                //foreach (Point3d pt in pts)
                //{
                //    path.Add(RhinoToHelixPoint(pt));
                //}
                meshBuilder.AddTube(path, diameter, resolutiontube, false);
                var meshHelix = meshBuilder.ToMesh(true);
                modelGroup.Children.Add(new GeometryModel3D { Geometry = meshHelix, Material = mat, BackMaterial = mat });
            }
            return modelGroup;
        }

        // polyline
        public static Model3D RhinoToHelixPolyline(Rhino.Geometry.Polyline poly, double diameter, int resolutiontube, System.Windows.Media.Media3D.Material mat)
        {
            var modelGroup = new Model3DGroup();
            var meshBuilder = new MeshBuilder(false, false);
            Line[] lines = poly.GetSegments();
            List<Point3D> path = new List<Point3D>();
            foreach (Line l in lines)
            {
                path.Add(RhinoToHelixPoint(l.To));
            }
            path.Add(RhinoToHelixPoint(lines[0].From));
            
            meshBuilder.AddTube(path, diameter, resolutiontube, true);
            var meshHelix = meshBuilder.ToMesh(true);
            modelGroup.Children.Add(new GeometryModel3D { Geometry = meshHelix, Material = mat, BackMaterial = mat });
            return modelGroup;
        }

        // set of curves
        public static Model3D RhinoToHelixCurves(List<Curve> dcurves, double diameter, int resolution, int resolutiontube, System.Windows.Media.Media3D.Material mat)
        {
            var modelGroup = new Model3DGroup();
            var meshBuilder = new MeshBuilder(false, false);

            foreach(Curve c in dcurves)
            {

                int curvedegree = c.Degree;
                if (curvedegree == 1)
                {

                    modelGroup.Children.Add(RhinoToHelixLine(c,diameter,resolutiontube,mat));
                }
                else if (c.IsPolyline())
                {
                    Polyline poly = new Polyline();
                    c.TryGetPolyline(out poly);
                    modelGroup.Children.Add(RhinoToHelixPolyline(poly,diameter,resolutiontube,mat));
                }
                else
                {
                    modelGroup.Children.Add(RhinoToHelixCurve(c,diameter,resolution,resolutiontube,mat));
                }
            }
            return modelGroup;
        }
        
        // SURFACES, MESH

        public static Model3D RhinotoHelixMesh(Rhino.Geometry.Mesh mesh, System.Windows.Media.Media3D.Material mat)
        {
            var modelGroup = new Model3DGroup();

            var meshBuilder = new MeshBuilder(false, false);
            var meshBuilderEdges = new MeshBuilder(false, false);
            // Triangulate to avoid planarity issues
            mesh.Faces.ConvertQuadsToTriangles();
            mesh.Faces.CullDegenerateFaces();
            Point3f[] vertices = mesh.Vertices.ToPoint3fArray();
            List<Point3D> vs = new List<Point3D>();
            //foreach (Point3d pt in vertices)
            //{
            //    vs.Add(RhinoToHelixPoint(pt));
            
            //}



            Point3f pt1 = new Point3f();
            Point3f pt2 = new Point3f();
            Point3f pt3 = new Point3f();
            Point3f pt4 = new Point3f();
            for (int i = 0; i < mesh.Faces.Count(); i++)
            {
                if (mesh.Faces.GetFaceVertices(i, out pt1, out pt2, out pt3, out pt4))
                {
                    Point3f pt1clone = new Point3f(pt1.X, pt1.Y, pt1.Z);
                    Point3f pt2clone = new Point3f(pt2.X, pt2.Y, pt2.Z);
                    Point3f pt3clone = new Point3f(pt3.X, pt3.Y, pt3.Z);
                    vs.Add(RhinoToHelixPoint(pt1clone));
                    vs.Add(RhinoToHelixPoint(pt2clone));
                    vs.Add(RhinoToHelixPoint(pt3clone));
                }
            }
            meshBuilder.AddTriangles(vs);
            var meshHelix = meshBuilder.ToMesh(true);
            var meshHelixLines = meshBuilderEdges.ToMesh(true);
            modelGroup.Children.Add(new GeometryModel3D { Geometry = meshHelix, Material = MaterialHelper.CreateMaterial(Colors.Gray), BackMaterial = MaterialHelper.CreateMaterial(Colors.Gray) });
            modelGroup.Children.Add(new GeometryModel3D { Geometry = meshHelixLines, Material =  mat , BackMaterial = mat });
            return modelGroup;
        }

        public static Model3D RhinotoHelixBrep(Brep brep, System.Windows.Media.Media3D.Material mat)
        {
            var modelGroup = new Model3DGroup();
            var meshBuilder = new MeshBuilder(false, false);

            var jaggedAndFaster = MeshingParameters.FastRenderMesh; //Coarse;
            var smoothAndSlower = MeshingParameters.QualityRenderMesh; //Smooth;
            var defaultMeshParams = MeshingParameters.Default;
            var verysmooth = MeshingParameters.QualityRenderMesh; //Smooth;
            verysmooth.MaximumEdgeLength = 5;
            var minimal = MeshingParameters.Minimal;
            
            var meshes = Mesh.CreateFromBrep(brep, verysmooth);
            if (meshes != null && meshes.Length != 0)
                foreach (Mesh m in meshes)
                {
                    modelGroup.Children.Add(RhinotoHelixMesh(m, mat));
                }
            return modelGroup;
        }

        public static Model3D DrawLines(List<Line> lines, double diameter, int resolutiontube, System.Windows.Media.Media3D.Material mat)
        {
            var modelGroup = new Model3DGroup();
            var meshBuilder = new MeshBuilder(false, false);


            foreach (Line l in lines)
            {
                meshBuilder.AddCylinder(RhinoToHelixPoint(l.From), RhinoToHelixPoint(l.To), diameter, resolutiontube); // resolution hard-coded, section parameter = radius
            }
            var mesh = meshBuilder.ToMesh(true);
            var CMaterial = MaterialHelper.CreateMaterial(Colors.Gray);
            modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh, Material = CMaterial, BackMaterial = CMaterial});

            // Assign modelGroup to model to draw
            return modelGroup;
        }

        // Draw diverse geometries in a same model3D
        public static Model3D Draw(List<Curve> dcurves, List<Mesh> dmeshes, List<Brep> dbreps, double diameter, int resolution, int resolutiontube, System.Windows.Media.Media3D.Material mat)
        {
            var modelGroup = new Model3DGroup();
            var meshBuilder = new MeshBuilder(false, false);

            if (dcurves.Count!=0)
            {
                modelGroup.Children.Add(RhinoToHelixCurves(dcurves, diameter, resolution, resolutiontube, mat));
            }


            if (dmeshes.Count!=0)
            {
                foreach (Mesh m in dmeshes)
                {
                    modelGroup.Children.Add(RhinotoHelixMesh(m, mat));
                }
            }


             if (dbreps.Count!=0)
             {
                 foreach (Brep b in dbreps)
                 {
                     modelGroup.Children.Add(RhinotoHelixBrep(b, mat));
                 }
             }


             var meshHelix = meshBuilder.ToMesh(true);

             return modelGroup;
        }
    }
}
