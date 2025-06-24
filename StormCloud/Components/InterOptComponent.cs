// Decompiled with JetBrains decompiler
// Type: StormCloud.InterOptComponent
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using StormCloud.Evolutionary;
using StormCloud.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace StormCloud
{
  public class InterOptComponent : GH_Component
  {
    public List<Line> DesignLines;
    public List<Curve> DesignCurves;
    public List<Mesh> DesignMeshes;
    public List<Brep> DesignBreps;
    public DesignToolVM DesignView;
    public double Score;

    public InterOptComponent()
      : base("StormCloud", "StormCloud", "Interactive Evolutionary Optimization", "DSE", "StormCloud")
    {
      this.Score = 0.0;
      this.DesignLines = new List<Line>();
      this.DesignCurves = new List<Curve>();
      this.DesignMeshes = new List<Mesh>();
      this.DesignBreps = new List<Brep>();
      this.DesignView = new DesignToolVM();
    }

    public List<DesignVar> DesignVariables
    {
      get
      {
        List<DesignVar> designVariables = new List<DesignVar>();
        foreach (IGH_Param source in (IEnumerable<IGH_Param>) this.Params.Input[2].Sources)
        {
          if (source is GH_NumberSlider ghNumberSlider)
          {
            DesignVar designVar = new DesignVar((double) ghNumberSlider.CurrentValue, (double) ghNumberSlider.Slider.Minimum, (double) ghNumberSlider.Slider.Maximum);
            designVariables.Add(designVar);
          }
        }
        return designVariables;
      }
      private set
      {
      }
    }

    public Design Design
    {
      get => new Design(this.DesignVariables);
      private set
      {
      }
    }

    public override void CreateAttributes()
    {
      this.m_attributes = (IGH_Attributes) new InterOptComponentAttributes((IGH_Component) this);
    }

    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddGeometryParameter("Geometry (Breps, Curves and Lines)", "G", "Design Geometry", GH_ParamAccess.list);
      pManager.AddNumberParameter("Score", "S", "Design Score", GH_ParamAccess.item);
      pManager.AddNumberParameter("Design Variables", "DVar", "Design Variables To Be Considered in the Interactive Evolutionary Optimization", GH_ParamAccess.list);
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      List<IGH_GeometricGoo> list = new List<IGH_GeometricGoo>();
      if (!DA.GetDataList<IGH_GeometricGoo>(0, list))
        return;
      List<Curve> curveList = new List<Curve>();
      List<Mesh> meshList = new List<Mesh>();
      List<Brep> brepList = new List<Brep>();
      foreach (IGH_GeometricGoo ghGeometricGoo in list)
      {
        if (ghGeometricGoo is GH_Line ghLine)
        {
          GH_Curve target = new GH_Curve();
          ghLine.CastTo<GH_Curve>(out target);
          curveList.Add(target.DuplicateCurve().Value);
        }
        if (ghGeometricGoo is GH_Curve ghCurve)
        {
          curveList.Add(ghCurve.DuplicateCurve().Value);
          Console.WriteLine("There is a Curve");
        }
        if (ghGeometricGoo is GH_Mesh ghMesh)
        {
          GH_Brep target = new GH_Brep();
          ghMesh.CastTo<GH_Brep>(out target);
          brepList.Add(target.DuplicateBrep().Value);
        }
        if (ghGeometricGoo is GH_Brep ghBrep)
          brepList.Add(ghBrep.DuplicateBrep().Value);
      }
      this.DesignCurves = curveList;
      this.DesignMeshes = meshList;
      this.DesignBreps = brepList;
      double destination = 0.0;
      if (!DA.GetData<double>(1, ref destination))
        return;
      this.Score = destination;
      if (this.DesignView.InitialDesign.Score != 0.0)
        this.DesignView.UpdateCurrentScore(this.Score);
      else
        this.DesignView.UpdateCurrentScore(1.0);
      this.DesignView.UpdateCurrentModelAdvanced(this.DesignCurves, this.DesignMeshes, this.DesignBreps, RenderingSettings.diameter, RenderingSettings.resolution, RenderingSettings.resolutiontube, RenderingSettings.mat);
    }

    protected override Bitmap Icon => Resources.Resources.gen_icon_small;

    public override Guid ComponentGuid => new Guid("{8310F3DB-CDA7-44F9-9E6B-D84B58821CC2}");
  }
}
