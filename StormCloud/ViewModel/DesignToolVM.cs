// Decompiled with JetBrains decompiler
// Type: StormCloud.ViewModel.DesignToolVM
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using Rhino.Geometry;
using StormCloud.Evolutionary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;

#nullable disable
namespace StormCloud.ViewModel
{
  public class DesignToolVM : BaseVM
  {
    public InterOptComponent Component;
    public DesignVM InitialDesign;
    public List<DesignVM> CurrentGeneration;
    public List<List<DesignVM>> Generations;
    public List<Design> Seeds;
    public StringBuilder ExplorationRec;
    private Point3D _pos;
    private Vector3D _lookdir;
    private Vector3D _updir;
    private Vector3D _fieldview;
    private Model3D _currentmodel;
    private double _currentnormalizedscore;

    public DesignToolVM()
    {
      this.CurrentGeneration = new List<DesignVM>();
      this.Generations = new List<List<DesignVM>>();
      this.Seeds = new List<Design>();
      this.InitialDesign = new DesignVM();
      this.ExplorationRec = new StringBuilder();
    }

    public double getinitscore() => this.InitialDesign.Score;

    public void UpdateCurrentScore(double score)
    {
      if (this.getinitscore() != 0.0)
        this.CurrentNormalizedScore = score / this.getinitscore();
      else
        this.CurrentNormalizedScore = 1.0;
    }

    public void UpdateCurrentModel(
      List<Line> lines,
      double diameter,
      int resolutiontube,
      Material mat)
    {
      this.CurrentModel = RhinoHelixUtilities.DrawLines(lines, diameter, resolutiontube, mat);
    }

    public void UpdateCurrentModelAdvanced(
      List<Curve> curves,
      List<Mesh> meshes,
      List<Brep> brep,
      double diameter,
      int resolution,
      int resolutiontube,
      Material mat)
    {
      this.CurrentModel = RhinoHelixUtilities.Draw(curves, meshes, brep, diameter, resolution, resolutiontube, mat);
    }

    public Point3D Pos
    {
      get => this._pos;
      set
      {
        if (!this.CheckPropertyChanged<Point3D>(nameof (Pos), ref this._pos, ref value))
          ;
      }
    }

    public Vector3D LookDir
    {
      get => this._lookdir;
      set
      {
        if (!this.CheckPropertyChanged<Vector3D>(nameof (LookDir), ref this._lookdir, ref value))
          ;
      }
    }

    public Vector3D UpDir
    {
      get => this._updir;
      set
      {
        if (!this.CheckPropertyChanged<Vector3D>(nameof (UpDir), ref this._updir, ref value))
          return;
        Console.WriteLine(nameof (UpDir));
        Console.WriteLine((object) this.UpDir);
      }
    }

    public Vector3D FieldView
    {
      get => this._fieldview;
      set
      {
        if (!this.CheckPropertyChanged<Vector3D>(nameof (FieldView), ref this._fieldview, ref value))
          return;
        Console.WriteLine(nameof (FieldView));
        Console.WriteLine((object) this.FieldView);
      }
    }

    public Model3D CurrentModel
    {
      get => this._currentmodel;
      set
      {
        if (!this.CheckPropertyChanged<Model3D>(nameof (CurrentModel), ref this._currentmodel, ref value))
          ;
      }
    }

    public double CurrentNormalizedScore
    {
      get => this._currentnormalizedscore;
      set
      {
        if (!this.CheckPropertyChanged<double>(nameof (CurrentNormalizedScore), ref this._currentnormalizedscore, ref value))
          ;
      }
    }
  }
}
