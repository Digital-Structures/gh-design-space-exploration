using System;
using System.Collections.Generic;
using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace StormCloud.ViewModel
{
    public static class RenderingSettings
    {
        public static double diameter = 0.1;
        public static int resolution = 2;
        public static int resolutiontube = 5;
        public static System.Windows.Media.Media3D.Material mat = MaterialHelper.CreateMaterial(Colors.Black);
        public static System.Windows.Media.Media3D.Material matmesh = MaterialHelper.CreateMaterial(Colors.LightGray);
    }
}