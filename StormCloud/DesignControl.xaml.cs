// Decompiled with JetBrains decompiler
// Type: StormCloud.DesignControl
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using HelixToolkit.Wpf;
using StormCloud.ViewModel;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

#nullable disable
namespace StormCloud
{
  public partial class DesignControl : UserControl, IComponentConnector
  {
    public InterOptComponent Component;
    internal Grid ControlGrid;
    internal Border Border;
    internal HelixViewport3D Individual;
    internal TextBlock ScoreText;
    internal CheckBox Selection;
    private bool _contentLoaded;

    public DesignControl(
      DesignVM dvm,
      double Height,
      double Width,
      bool Clickable,
      IGH_Component comp)
    {
      this.Component = (InterOptComponent) comp;
      this.myViewModel = dvm;
      this.DataContext = (object) this.myViewModel;
      this.myViewModel.IsClickable = Clickable;
      this.InitializeComponent();
      this.ControlGrid.Height = Height;
      this.ControlGrid.Width = Width;
      this.ScoreText.Text = string.Format("{0:0.00}", (object) dvm.Score);
    }

    public DesignVM myViewModel { get; set; }

    private void ControlGrid_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Border.BorderThickness = new Thickness(2.0);
    }

    private void ControlGrid_MouseLeave(object sender, MouseEventArgs e)
    {
      this.Border.BorderThickness = new Thickness(0.0);
    }

    private void ControlGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      for (int index = 0; index < this.Component.Params.Input[2].Sources.Count; ++index)
      {
        if (this.Component.Params.Input[2].Sources[index] is GH_NumberSlider source)
          source.SetSliderValue((Decimal) this.myViewModel.Design.DesignVariables[index].Value);
      }
      Instances.ActiveCanvas.Document.NewSolution(true);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/StormCloud;component/designcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.ControlGrid = (Grid) target;
          break;
        case 2:
          this.Border = (Border) target;
          this.Border.MouseEnter += new MouseEventHandler(this.ControlGrid_MouseEnter);
          this.Border.MouseLeave += new MouseEventHandler(this.ControlGrid_MouseLeave);
          this.Border.MouseLeftButtonDown += new MouseButtonEventHandler(this.ControlGrid_MouseLeftButtonDown);
          break;
        case 3:
          this.Individual = (HelixViewport3D) target;
          break;
        case 4:
          this.ScoreText = (TextBlock) target;
          break;
        case 5:
          this.Selection = (CheckBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
