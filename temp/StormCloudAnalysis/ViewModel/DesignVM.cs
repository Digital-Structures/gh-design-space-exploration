using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;
using Rhino.Geometry;
using StormCloud.ViewModel;
using HelixToolkit.Wpf;
using StormCloud.Evolutionary;

namespace StormCloud.ViewModel
{
    public class DesignVM : BaseVM
    {

        public DesignVM(List<Line> lines, List<Curve> curves, List<Mesh> meshes, List<Brep> breps, bool isselected, bool isclickable, double score, Design design)
        {
            this.DesignLines = lines;
            this.IsClickable = isclickable;
            this.IsSelected = isselected;
            this.Score = score;
            this.Design = design;
            Console.WriteLine('1');
            //this.Model = RhinoHelixUtilities.DrawLines(this.DesignLines,RenderingSettings.diameter, RenderingSettings.resolutiontube,RenderingSettings.mat);
            this.Model = RhinoHelixUtilities.Draw(curves, meshes, breps, RenderingSettings.diameter, RenderingSettings.resolution, RenderingSettings.resolutiontube, RenderingSettings.mat);
        }

        public DesignVM() {}

        public List<Line> DesignLines;

        public List<Curve> DesignCurves;
        public List<Mesh> DesignMeshes;
        public List<Brep> DesignBreps;


        private bool _isselected;
        public bool IsSelected
        {
            get
            {
                return _isselected;
            }
            set
            {
                if (CheckPropertyChanged<bool>("IsSelected", ref _isselected, ref value))
                {
                }
            }
        }


        private bool _isclickable;
        public bool IsClickable
        {
                        get
            {
                return _isclickable;
            }
            set
            {
                if (CheckPropertyChanged<bool>("IsClickable", ref _isclickable, ref value))
                {

                }
            }
        }

        public double Score
        {
            get;
            set;
        }

        //public List<DesignVar> DesignVariables;

        public Design Design;


         //Cam Position
        //private Vector3D _position;
        //public Vector3D Position
        //{
        //    get
        //    {
        //        return _position;
        //    }
        //    set
        //    {
        //        if (CheckPropertyChanged<Vector3D>("Position", ref _position, ref value))
        //        {

        //        }
        //    }
        //}

        //// Cam LookDir
        //private Vector3D _lookdir;
        //public Vector3D LookDir
        //{
        //    get
        //    {
        //        return _lookdir;
        //    }
        //    set
        //    {
        //        if (CheckPropertyChanged<Vector3D>("LookDir", ref _lookdir, ref value))
        //        {

        //        }
        //    }
        //}
        
        private Model3D _model;
        public Model3D Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (CheckPropertyChanged<Model3D>("Model", ref _model, ref value))
                {

                }
            }
        }
    }
}
