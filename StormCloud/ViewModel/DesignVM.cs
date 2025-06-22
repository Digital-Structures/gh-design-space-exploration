// Decompiled with JetBrains decompiler
// Type: StormCloud.ViewModel.DesignVM
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using Rhino.Geometry;
using StormCloud.Evolutionary;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

#nullable disable
namespace StormCloud.ViewModel
{
  public class DesignVM : BaseVM
  {
    public List<Line> DesignLines;
    public List<Curve> DesignCurves;
    public List<Mesh> DesignMeshes;
    public List<Brep> DesignBreps;
    private bool _isselected;
    private bool _isclickable;
    public Design Design;
    private Model3D _model;

    public DesignVM(
      List<Line> lines,
      List<Curve> curves,
      List<Mesh> meshes,
      List<Brep> breps,
      bool isselected,
      bool isclickable,
      double score,
      Design design)
    {
      this.DesignLines = lines;
      this.IsClickable = isclickable;
      this.IsSelected = isselected;
      this.Score = score;
      this.Design = design;
      Console.WriteLine('1');
      this.Model = RhinoHelixUtilities.Draw(curves, meshes, breps, RenderingSettings.diameter, RenderingSettings.resolution, RenderingSettings.resolutiontube, RenderingSettings.mat);
    }

    public DesignVM()
    {
    }

    public bool IsSelected
    {
      get => this._isselected;
      set
      {
        if (!this.CheckPropertyChanged<bool>(nameof (IsSelected), ref this._isselected, ref value))
          ;
      }
    }

    public bool IsClickable
    {
      get => this._isclickable;
      set
      {
        if (!this.CheckPropertyChanged<bool>(nameof (IsClickable), ref this._isclickable, ref value))
          ;
      }
    }

    public double Score { get; set; }

    public Model3D Model
    {
      get => this._model;
      set
      {
        if (!this.CheckPropertyChanged<Model3D>(nameof (Model), ref this._model, ref value))
          ;
      }
    }
  }
}
