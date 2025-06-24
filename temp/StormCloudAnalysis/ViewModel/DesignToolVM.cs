using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using StormCloud.Evolutionary;

namespace StormCloud.ViewModel
{
    public class DesignToolVM : BaseVM
    {
        public DesignToolVM() 
        {
            this.CurrentGeneration = new List<DesignVM>();
            this.Generations = new List<List<DesignVM>>();
            this.Seeds = new List<Design>();
            this.InitialDesign = new DesignVM();
            this.ExplorationRec = new StringBuilder();
        }

        public InterOptComponent Component;

        public DesignVM InitialDesign;

        public List<DesignVM> CurrentGeneration;

        public List<List<DesignVM>> Generations;

        public List<Design> Seeds; // for next generation Add if selected, remove if not selected

        public StringBuilder ExplorationRec;
        
        
        public double getinitscore()
        {
            return this.InitialDesign.Score;
        }

        public void UpdateCurrentScore(double score)
        {
            if (this.getinitscore() !=0)
            {
                this.CurrentNormalizedScore = score/this.getinitscore();
            }
            else
            {
                this.CurrentNormalizedScore = 1.00;
            }            
        }

        // Updatecurrentmodel
        public void UpdateCurrentModel(List<Line> lines, double diameter, int resolutiontube, System.Windows.Media.Media3D.Material mat)
        {
            this.CurrentModel = RhinoHelixUtilities.DrawLines(lines, diameter, resolutiontube, mat);
        }

        public void UpdateCurrentModelAdvanced(List<Curve> curves, List<Mesh> meshes, List<Brep> brep, double diameter, int resolution, int resolutiontube, System.Windows.Media.Media3D.Material mat)
        {
            this.CurrentModel = RhinoHelixUtilities.Draw(curves, meshes, brep, diameter, resolution, resolutiontube, mat);
        }

        // CAMERA
        // Cam Position
        private Point3D _pos;
        public Point3D Pos
        {
            get
            {
                return _pos;
            }
            set
            {
                if (CheckPropertyChanged<Point3D>("Pos", ref _pos, ref value))
                {
                }
            }
        }

        // Cam LookDir
        private Vector3D _lookdir;
        public Vector3D LookDir
        {
            get
            {
                return _lookdir;
            }
            set
            {
                if (CheckPropertyChanged<Vector3D>("LookDir", ref _lookdir, ref value))
                {
                }
            }
        }

        // Cam UpDir

        private Vector3D _updir;
        public Vector3D UpDir
        {
            get
            {
                return _updir;
            }
            set
            {
                if (CheckPropertyChanged<Vector3D>("UpDir", ref _updir, ref value))
                {
                    Console.WriteLine("UpDir");
                    Console.WriteLine(UpDir);
                }
            }
        }

        // Cam FieldView

        private Vector3D _fieldview;
        public Vector3D FieldView
        {
            get
            {
                return _fieldview;
            }
            set
            {
                if (CheckPropertyChanged<Vector3D>("FieldView", ref _fieldview, ref value))
                {
                    Console.WriteLine("FieldView");
                    Console.WriteLine(FieldView);
                }
            }
        }

        // Current Design in Grasshopper
        private Model3D _currentmodel;
        public Model3D CurrentModel
        {
            get
            {
                return _currentmodel;
            }
            set
            {
                if (CheckPropertyChanged<Model3D>("CurrentModel", ref _currentmodel, ref value))
                {

                }
            }
        }

        // Current normalized score in Grasshopper
        private double _currentnormalizedscore;
        public double CurrentNormalizedScore
        {
            get
            {
                return _currentnormalizedscore;
            }
            set
            {
                if (CheckPropertyChanged<double>("CurrentNormalizedScore", ref _currentnormalizedscore, ref value))
                {
                }
            }
        }
    }
}
