// Decompiled with JetBrains decompiler
// Type: StormCloud.ViewModel.RhinoHelixUtilities
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using HelixToolkit.Wpf;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#nullable disable
namespace StormCloud.ViewModel
{
  public static class RhinoHelixUtilities
  {
    public static Point3D RhinoToHelixPoint(Point3d pt) => new Point3D(pt.X, pt.Y, pt.Z);

    public static Model3D RhinoToHelixLine(
      Curve curve,
      double diameter,
      int resolutiontube,
      Material mat)
    {
      Model3DGroup helixLine = new Model3DGroup();
      MeshBuilder meshBuilder = new MeshBuilder(false, false);
      meshBuilder.AddTube((IList<Point3D>) new List<Point3D>()
      {
        RhinoHelixUtilities.RhinoToHelixPoint(curve.PointAtStart),
        RhinoHelixUtilities.RhinoToHelixPoint(curve.PointAtEnd)
      }, diameter, resolutiontube, true);
      MeshGeometry3D mesh = meshBuilder.ToMesh(true);
      helixLine.Children.Add((Model3D) new GeometryModel3D()
      {
        Geometry = (Geometry3D) mesh,
        Material = mat,
        BackMaterial = mat
      });
      return (Model3D) helixLine;
    }

    public static Model3D RhinoToHelixCurve(
      Curve curve,
      double diameter,
      int resolution,
      int resolutiontube,
      Material mat)
    {
      int degree = curve.Degree;
      Model3DGroup helixCurve = new Model3DGroup();
      Console.WriteLine("BS");
      if (curve.IsLinear())
      {
        Console.WriteLine("blabla");
        return RhinoHelixUtilities.RhinoToHelixLine(curve, diameter, resolutiontube, mat);
      }
      MeshBuilder meshBuilder = new MeshBuilder(false, false);
      meshBuilder.AddTube((IList<Point3D>) new List<Point3D>()
      {
        RhinoHelixUtilities.RhinoToHelixPoint(curve.PointAtEnd),
        RhinoHelixUtilities.RhinoToHelixPoint(curve.PointAtStart)
      }, diameter, resolutiontube, false);
      MeshGeometry3D mesh = meshBuilder.ToMesh(true);
      helixCurve.Children.Add((Model3D) new GeometryModel3D()
      {
        Geometry = (Geometry3D) mesh,
        Material = mat,
        BackMaterial = mat
      });
      return (Model3D) helixCurve;
    }

    public static Model3D RhinoToHelixPolyline(
      Polyline poly,
      double diameter,
      int resolutiontube,
      Material mat)
    {
      Model3DGroup helixPolyline = new Model3DGroup();
      MeshBuilder meshBuilder = new MeshBuilder(false, false);
      Line[] segments = poly.GetSegments();
      List<Point3D> point3DList = new List<Point3D>();
      foreach (Line line in segments)
        point3DList.Add(RhinoHelixUtilities.RhinoToHelixPoint(line.To));
      point3DList.Add(RhinoHelixUtilities.RhinoToHelixPoint(segments[0].From));
      meshBuilder.AddTube((IList<Point3D>) point3DList, diameter, resolutiontube, true);
      MeshGeometry3D mesh = meshBuilder.ToMesh(true);
      helixPolyline.Children.Add((Model3D) new GeometryModel3D()
      {
        Geometry = (Geometry3D) mesh,
        Material = mat,
        BackMaterial = mat
      });
      return (Model3D) helixPolyline;
    }

    public static Model3D RhinoToHelixCurves(
      List<Curve> dcurves,
      double diameter,
      int resolution,
      int resolutiontube,
      Material mat)
    {
      Model3DGroup helixCurves = new Model3DGroup();
      MeshBuilder meshBuilder = new MeshBuilder(false, false);
      foreach (Curve dcurve in dcurves)
      {
        if (dcurve.Degree == 1)
          helixCurves.Children.Add(RhinoHelixUtilities.RhinoToHelixLine(dcurve, diameter, resolutiontube, mat));
        else if (dcurve.IsPolyline())
        {
          Polyline polyline = new Polyline();
          dcurve.TryGetPolyline(out polyline);
          helixCurves.Children.Add(RhinoHelixUtilities.RhinoToHelixPolyline(polyline, diameter, resolutiontube, mat));
        }
        else
          helixCurves.Children.Add(RhinoHelixUtilities.RhinoToHelixCurve(dcurve, diameter, resolution, resolutiontube, mat));
      }
      return (Model3D) helixCurves;
    }

    public static Model3D RhinotoHelixMesh(Mesh mesh, Material mat)
    {
      Model3DGroup model3Dgroup = new Model3DGroup();
      MeshBuilder meshBuilder1 = new MeshBuilder(false, false);
      MeshBuilder meshBuilder2 = new MeshBuilder(false, false);
      mesh.Faces.ConvertQuadsToTriangles();
      mesh.Faces.CullDegenerateFaces();
      mesh.Vertices.ToPoint3fArray();
      List<Point3D> point3DList = new List<Point3D>();
      Point3f a = new Point3f();
      Point3f b = new Point3f();
      Point3f c = new Point3f();
      Point3f d = new Point3f();
      for (int faceIndex = 0; faceIndex < ((IEnumerable<MeshFace>) mesh.Faces).Count<MeshFace>(); ++faceIndex)
      {
        if (mesh.Faces.GetFaceVertices(faceIndex, out a, out b, out c, out d))
        {
          Point3f pt1 = new Point3f(a.X, a.Y, a.Z);
          Point3f pt2 = new Point3f(b.X, b.Y, b.Z);
          Point3f pt3 = new Point3f(c.X, c.Y, c.Z);
          point3DList.Add(RhinoHelixUtilities.RhinoToHelixPoint((Point3d) pt1));
          point3DList.Add(RhinoHelixUtilities.RhinoToHelixPoint((Point3d) pt2));
          point3DList.Add(RhinoHelixUtilities.RhinoToHelixPoint((Point3d) pt3));
        }
      }
      meshBuilder1.AddTriangles((IList<Point3D>) point3DList, (IList<Vector3D>) null, (IList<System.Windows.Point>) null);
      MeshGeometry3D mesh1 = meshBuilder1.ToMesh(true);
      MeshGeometry3D mesh2 = meshBuilder2.ToMesh(true);
      model3Dgroup.Children.Add((Model3D) new GeometryModel3D()
      {
        Geometry = (Geometry3D) mesh1,
        Material = MaterialHelper.CreateMaterial(Colors.Gray),
        BackMaterial = MaterialHelper.CreateMaterial(Colors.Gray)
      });
      model3Dgroup.Children.Add((Model3D) new GeometryModel3D()
      {
        Geometry = (Geometry3D) mesh2,
        Material = mat,
        BackMaterial = mat
      });
      return (Model3D) model3Dgroup;
    }

    public static Model3D RhinotoHelixBrep(Brep brep, Material mat)
    {
      Model3DGroup model3Dgroup = new Model3DGroup();
      MeshBuilder meshBuilder = new MeshBuilder(false, false);
      MeshingParameters coarse = MeshingParameters.Coarse;
      MeshingParameters smooth1 = MeshingParameters.Smooth;
      MeshingParameters meshingParameters = MeshingParameters.Default;
      MeshingParameters smooth2 = MeshingParameters.Smooth;
      smooth2.MaximumEdgeLength = 5.0;
      MeshingParameters minimal = MeshingParameters.Minimal;
      Mesh[] fromBrep = Mesh.CreateFromBrep(brep, smooth2);
      if (fromBrep != null && fromBrep.Length != 0)
      {
        foreach (Mesh mesh in fromBrep)
          model3Dgroup.Children.Add(RhinoHelixUtilities.RhinotoHelixMesh(mesh, mat));
      }
      return (Model3D) model3Dgroup;
    }

    public static Model3D DrawLines(
      List<Line> lines,
      double diameter,
      int resolutiontube,
      Material mat)
    {
      Model3DGroup model3Dgroup = new Model3DGroup();
      MeshBuilder meshBuilder = new MeshBuilder(false, false);
      foreach (Line line in lines)
        meshBuilder.AddCylinder(RhinoHelixUtilities.RhinoToHelixPoint(line.From), RhinoHelixUtilities.RhinoToHelixPoint(line.To), diameter, resolutiontube);
      MeshGeometry3D mesh = meshBuilder.ToMesh(true);
      Material material = MaterialHelper.CreateMaterial(Colors.Gray);
      model3Dgroup.Children.Add((Model3D) new GeometryModel3D()
      {
        Geometry = (Geometry3D) mesh,
        Material = material,
        BackMaterial = material
      });
      return (Model3D) model3Dgroup;
    }

    public static Model3D Draw(
      List<Curve> dcurves,
      List<Mesh> dmeshes,
      List<Brep> dbreps,
      double diameter,
      int resolution,
      int resolutiontube,
      Material mat)
    {
      Model3DGroup model3Dgroup = new Model3DGroup();
      MeshBuilder meshBuilder = new MeshBuilder(false, false);
      if (dcurves.Count != 0)
        model3Dgroup.Children.Add(RhinoHelixUtilities.RhinoToHelixCurves(dcurves, diameter, resolution, resolutiontube, mat));
      if (dmeshes.Count != 0)
      {
        foreach (Mesh dmesh in dmeshes)
          model3Dgroup.Children.Add(RhinoHelixUtilities.RhinotoHelixMesh(dmesh, mat));
      }
      if (dbreps.Count != 0)
      {
        foreach (Brep dbrep in dbreps)
          model3Dgroup.Children.Add(RhinoHelixUtilities.RhinotoHelixBrep(dbrep, mat));
      }
      meshBuilder.ToMesh(true);
      return (Model3D) model3Dgroup;
    }
  }
}
