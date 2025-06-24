// Decompiled with JetBrains decompiler
// Type: StormCloud.InterOptComponentAttributes
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using StormCloud.ViewModel;
using System;

#nullable disable
namespace StormCloud
{
  public class InterOptComponentAttributes : GH_ComponentAttributes
  {
    private InterOptComponent MyComponent;

    public InterOptComponentAttributes(IGH_Component component)
      : base(component)
    {
      this.MyComponent = (InterOptComponent) component;
    }

    [STAThread]
    public override GH_ObjectResponse RespondToMouseDoubleClick(
      GH_Canvas sender,
      GH_CanvasMouseEvent e)
    {
      this.MyComponent.DesignView.InitialDesign = new DesignVM(this.MyComponent.DesignLines, this.MyComponent.DesignCurves, this.MyComponent.DesignMeshes, this.MyComponent.DesignBreps, false, true, this.MyComponent.Score, this.MyComponent.Design);
      new StormCloudWindow(this.MyComponent.DesignView, (IGH_Component) this.MyComponent).Show();
      return base.RespondToMouseDoubleClick(sender, e);
    }
  }
}
