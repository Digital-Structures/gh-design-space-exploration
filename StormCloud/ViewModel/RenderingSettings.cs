// Decompiled with JetBrains decompiler
// Type: StormCloud.ViewModel.RenderingSettings
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#nullable disable
namespace StormCloud.ViewModel
{
  public static class RenderingSettings
  {
    public static double diameter = 0.1;
    public static int resolution = 2;
    public static int resolutiontube = 5;
    public static Material mat = MaterialHelper.CreateMaterial(Colors.Black);
    public static Material matmesh = MaterialHelper.CreateMaterial(Colors.LightGray);
  }
}
