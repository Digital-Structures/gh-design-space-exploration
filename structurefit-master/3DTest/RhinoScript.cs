using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

// WPF aliases for Rhino objects
using Point3d = System.Windows.Media.Media3D.Point3D;
using GeometryBase = System.Windows.Media.Media3D.ModelVisual3D;

namespace _3DTest
{
    public class Script_Instance
    {
        /// <summary>
        /// This procedure contains the user code. Input parameters are provided as regular arguments,
        /// Output parameters as ref arguments. You don't have to assign output parameters,
        /// they will have a default value.
        /// </summary>
        public void RunScript(List<Point3d> points, int type, ref object A, double tol, bool setup)
        {
            RenderSet(0, points, false);
            switch (type)
            {
                case (int)Algorithm.BRUTE:
                    // Compare each pair of points (N^2)
                    foreach (Point3d p1 in points)
                    {
                        foreach (Point3d p2 in points)
                        {
                            if (p1 == p2) continue;  // don't compare to self
                            RenderComparison(++Iter, p1, p2, Close(p1, p2, tol));
                        }
                    }
                    break;
                case (int)Algorithm.SORTED:
                    // Sort by X (N log N)
                    var sorted = points.OrderBy(p => p,
                        Comparer<Point3d>.Create((p1, p2) => {
                            if (setup) RenderComparison(++Iter, p1, p2, false);
                            return p1.X.CompareTo(p2.X);
                        })).ToList();

                    // Compare nearby nodes (N)
                    for (int i = 0; i < sorted.Count; i++)
                    {
                        for (int j = i + 1; j < sorted.Count; j++)
                        {
                            Point3d p1 = sorted[i];
                            Point3d p2 = sorted[j];
                            RenderComparison(++Iter, p1, p2, Close(p1, p2, tol));
                            // Stop when the next point cannot be within tolerance
                            double tol_mult = p1.Y == 0 ? 1 : Math.Sqrt(2);  // 1D vs 2D
                            if (p2.X - p1.X > tol_mult * tol) break;
                        }
                    }
                    break;
                case (int)Algorithm.HASHMAP:
                    // Put values into a dictionary (N)
                    var dict = new Dictionary<Point3d, bool>();
                    foreach (Point3d p in points)
                    {
                        RenderSet(++Iter, dict.Keys, dict.ContainsKey(p));
                        dict[p] = true;
                    }
                    break;
                case (int)Algorithm.TREE:
                    // Put values into a quadtree (N log N)
                    var tree = new QuadTree(0.5, 0.5, 0.5, (qt, create) => {
                        // Always render new trees, and conditionally render tree iteration.
                        if (create) RenderArea(0, qt.Center, qt.Radius);
                        else if (setup) RenderArea(++Iter, qt.Center, qt.Radius);
                    });
                    foreach (Point3d p in points)
                    {
                        tree.Insert(p);
                    }
                    // Look for nearby nodes.
                    foreach (Point3d p in points)
                    {
                        foreach (Point3d near in tree.Query(p, tol))
                        {
                            RenderComparison(++Iter, near, p, Close(near, p, tol));
                        }
                    }
                    break;
            }

            A = Geo;
        }

        // <Custom additional code>
        private int Iter = 0;
        private List<GeometryBase> Geo = new List<GeometryBase>();
        private const double R = 0.01;
        private const double DY = -0.02;
        private const double DIAM = 0.005;

        private static bool Close(Point3d p1, Point3d p2, double tol)
        {
            //return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)) <= tol;
            return p1.DistanceTo(p2) <= tol;
        }

        private void RenderComparison(int iter, Point3d p1, Point3d p2, bool highlight)
        {
            // Render both points.
            RenderSet(iter, new List<Point3d> { p1, p2 }, highlight);
            // Connect the points with a line.
            double z = iter * DY;
            Geo.Add(new PipeVisual3D()
            {
                Point1 = new Point3d(p1.X, p1.Y, z),
                Point2 = new Point3d(p2.X, p2.Y, z),
                Diameter = DIAM,
                Fill = new SolidColorBrush(highlight ? Colors.Red : Colors.Gray),
            });
        }
        private void RenderSet(int iter, IEnumerable<Point3d> points, bool highlight)
        {
            double z = iter * DY;
            foreach (var p in points)
            {
                //Circle c = new Circle(new Point3d(p.X, y, 0), r);
                //Geo.Add(new ArcCurve(c));
                //if (highlight)
                //{
                //    Geo.Add(Brep.CreatePlanarBreps(new ArcCurve(c))[0]);
                //}
                Geo.Add(new SphereVisual3D()
                {
                    Radius = iter == 0 ? 2*R : R,
                    Center = new Point3d(p.X, p.Y, z),
                    Fill = new SolidColorBrush(highlight ? Colors.Red : Colors.Gray),
                });
            }
        }
        private void RenderArea(int iter, Point3d p, double r)
        {
            double z = iter * DY;
            Geo.Add(new PipeVisual3D()
            {
                Point1 = new Point3d(p.X - r, p.Y - r, z),
                Point2 = new Point3d(p.X - r, p.Y + r, z),
                Diameter = DIAM,
                Fill = new SolidColorBrush(Colors.Gray),
            });
            Geo.Add(new PipeVisual3D()
            {
                Point1 = new Point3d(p.X + r, p.Y - r, z),
                Point2 = new Point3d(p.X + r, p.Y + r, z),
                Diameter = DIAM,
                Fill = new SolidColorBrush(Colors.Gray),
            });
            Geo.Add(new PipeVisual3D()
            {
                Point1 = new Point3d(p.X - r, p.Y - r, z),
                Point2 = new Point3d(p.X + r, p.Y - r, z),
                Diameter = DIAM,
                Fill = new SolidColorBrush(Colors.Gray),
            });
            Geo.Add(new PipeVisual3D()
            {
                Point1 = new Point3d(p.X - r, p.Y + r, z),
                Point2 = new Point3d(p.X + r, p.Y + r, z),
                Diameter = DIAM,
                Fill = new SolidColorBrush(Colors.Gray),
            });
        }

        private class QuadTree
        {
            private const int CAPACITY = 3;          // max points per node, MODIFY TO SUIT!
            private const bool REDISTRIBUTE = true;  // move children when splitting

            public QuadTree(double x, double y, double r, Action<QuadTree, bool> nav)
            {
                OnNav = nav;
                Radius = r;
                Center = new Point3d(x, y, 0);
                Points = new List<Point3d>();
                OnNav(this, true);  // creation callback
            }
        
            // Properties
            public Point3d Center { get; private set; } // center point
            public double Radius { get; private set; }  // center to side
            private List<Point3d> Points;               // contained points
            private Action<QuadTree, bool> OnNav;       // callbacks for rendering
            
            // Children
            private QuadTree nw, ne, sw, se;

            // Check whether a point falls in this node
            private bool Contains(Point3d p)
            {
                return
                    Center.X - Radius <= p.X && p.X < Center.X + Radius &&
                    Center.Y - Radius <= p.Y && p.Y < Center.Y + Radius;
            }
            // Check whether a rectangle intersects this one
            private bool Intersects(Point3d p, double r)
            {
                return !(
                    p.X - r > Center.X + Radius ||
                    p.X + r < Center.X - Radius ||
                    p.Y - r > Center.Y + Radius ||
                    p.Y + r < Center.Y - Radius);
            }

            // Add a point, splitting the node if necessary. Return success.
            public bool Insert(Point3d p)
            {
                // No match.
                if (!Contains(p)) return false;
                OnNav(this, false);  // insertion callback
                // Have local space, add point.
                if (nw == null && Points.Count < CAPACITY)
                {
                    Points.Add(p);
                    return true;
                }
                // Local space full, create children if necessary, then distribute point to children.
                if (nw == null)
                {
                    double r = Radius / 2;
                    nw = new QuadTree(Center.X - r, Center.Y - r, r, OnNav);
                    ne = new QuadTree(Center.X + r, Center.Y - r, r, OnNav);
                    sw = new QuadTree(Center.X - r, Center.Y + r, r, OnNav);
                    se = new QuadTree(Center.X + r, Center.Y + r, r, OnNav);

                    // Redistribute children. Not strictly necessary, but makes for cleaner visualization.
                    if (REDISTRIBUTE)
                    {
                        foreach (Point3d old in Points) this.Insert(old);
                        this.Points.Clear();
                    }
                }
                if (nw.Insert(p)) return true;
                if (ne.Insert(p)) return true;
                if (sw.Insert(p)) return true;
                if (se.Insert(p)) return true;
                return false;  // unknown error, shouldn't happen
            }

            // Query a point and its surroundings.
            public IEnumerable<Point3d> Query(Point3d p, double r)
            {
                // No intersection.
                if (!Intersects(p, r)) yield break;
                // Check points at this node.
                foreach (Point3d point in Points)
                {
                    if (point == p) continue;
                    yield return point;
                }
                // Check points in children, if any.
                if (nw != null)
                {
                    foreach (Point3d point in nw.Query(p, r)) yield return point;
                    foreach (Point3d point in ne.Query(p, r)) yield return point;
                    foreach (Point3d point in sw.Query(p, r)) yield return point;
                    foreach (Point3d point in se.Query(p, r)) yield return point;
                }
            }
        }

        /// <summary>
        /// This method will be called once every solution, before any calls to RunScript.
        /// </summary>
        //public override void BeforeRunScript()
        //{
        //    Iter = 0;
        //    Geo = new List<GeometryBase>();
        //}
    }
}
