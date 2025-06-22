// Decompiled with JetBrains decompiler
// Type: StormCloud.StormCloudWindow
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using HelixToolkit.Wpf;
using MathNet.Numerics.Distributions;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using StormCloud.Evolutionary;
using StormCloud.ViewModel;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Environment = System.Environment;

#nullable disable
namespace StormCloud
{
  public partial class StormCloudWindow : Window, IComponentConnector
  {
    public InterOptComponent Component;
    public DesignToolVM myDesignToolVM;
    public const int NCOL = 8;
    private double _RowHeight;
    internal StormCloudWindow SCWindow;
    internal HelixViewport3D CurrentModel;
    internal CheckBox autoGenerateCheck;
    internal Slider autogenSlider;
    internal Slider Pop;
    internal Slider Mut;
    internal ScrollViewer myScroll;
    internal Grid DesignGrid;
    private bool _contentLoaded;

    public StormCloudWindow(DesignToolVM designtoolvm, IGH_Component comp)
    {
      this.InitializeComponent();
      this.Component = (InterOptComponent) comp;
      this.myDesignToolVM = designtoolvm;
      this.myDesignToolVM.Pos = new Point3D(0.0, 0.0, 20.0);
      this.myDesignToolVM.LookDir = new Vector3D(0.0, 0.0, -1.0);
      this.Pop.Minimum = 8.0;
      this.DataContext = (object) designtoolvm;
    }

    public List<Design> GetSeeds(int rowIndex)
    {
      List<Design> seeds = new List<Design>();
      foreach (DesignVM designVm in this.myDesignToolVM.Generations[rowIndex])
      {
        if (designVm.IsSelected)
          seeds.Add(designVm.Design.DesignClone());
      }
      return seeds;
    }

    public List<DesignVM> GenerateAndSelectTop(int number, int popsize, double rate)
    {
      DesignVM initialDesign = this.myDesignToolVM.InitialDesign;
      int rowIndex = this.DesignGrid.RowDefinitions.Count - 1;
      List<Design> seeds = new List<Design>();
      if (rowIndex >= 0)
        seeds = this.GetSeeds(rowIndex);
      List<Design> designList = EvolutionaryUtilities.NewGeneration(initialDesign.Design, seeds, (IContinuousDistribution) EvolutionaryUtilities.NormalGenerator, popsize, rate);
      List<DesignVM> DesignVMs = new List<DesignVM>();
      foreach (Design design1 in designList)
      {
        for (int index = 0; index < this.Component.Params.Input[2].Sources.Count; ++index)
        {
          if (this.Component.Params.Input[2].Sources[index] is GH_NumberSlider source)
            source.SetSliderValue((Decimal) design1.DesignVariables[index].Value);
        }
        Instances.ActiveCanvas.Document.NewSolution(true);
        DesignVM design2 = new DesignVM(this.Component.DesignLines, this.Component.DesignCurves, this.Component.DesignMeshes, this.Component.DesignBreps, false, true, this.Component.Score / this.myDesignToolVM.getinitscore(), design1.DesignClone());
        this.myDesignToolVM.ExplorationRec.AppendLine(csvWriter.writeDesign(design2));
        DesignVMs.Add(design2);
      }
      List<DesignVM> topDesignsVm = EvolutionaryUtilities.FindTopDesignsVM(DesignVMs, number, rate);
      DesignVM designVm = topDesignsVm[0];
      for (int index = 0; index < this.Component.Params.Input[2].Sources.Count; ++index)
      {
        if (this.Component.Params.Input[2].Sources[index] is GH_NumberSlider source)
          source.SetSliderValue((Decimal) designVm.Design.DesignVariables[index].Value);
      }
      Instances.ActiveCanvas.Document.NewSolution(true);
      return topDesignsVm;
    }

    public List<DesignVM> GenerateAndSelectTopFromSeeds(
      int number,
      int popsize,
      double rate,
      List<Design> seeds)
    {
      DesignVM initialDesign = this.myDesignToolVM.InitialDesign;
      int num = this.DesignGrid.RowDefinitions.Count - 1;
      List<Design> designList = EvolutionaryUtilities.NewGeneration(initialDesign.Design, seeds, (IContinuousDistribution) EvolutionaryUtilities.NormalGenerator, popsize, rate);
      List<DesignVM> DesignVMs = new List<DesignVM>();
      foreach (Design design1 in designList)
      {
        for (int index = 0; index < this.Component.Params.Input[2].Sources.Count; ++index)
        {
          if (this.Component.Params.Input[2].Sources[index] is GH_NumberSlider source)
            source.SetSliderValue((Decimal) design1.DesignVariables[index].Value);
        }
        Instances.ActiveCanvas.Document.NewSolution(true);
        DesignVM design2 = new DesignVM(this.Component.DesignLines, this.Component.DesignCurves, this.Component.DesignMeshes, this.Component.DesignBreps, false, true, this.Component.Score / this.myDesignToolVM.getinitscore(), design1.DesignClone());
        this.myDesignToolVM.ExplorationRec.AppendLine(csvWriter.writeDesign(design2));
        DesignVMs.Add(design2);
      }
      List<DesignVM> topDesignsVm = EvolutionaryUtilities.FindTopDesignsVM(DesignVMs, number, rate);
      DesignVM designVm = topDesignsVm[0];
      for (int index = 0; index < this.Component.Params.Input[2].Sources.Count; ++index)
      {
        if (this.Component.Params.Input[2].Sources[index] is GH_NumberSlider source)
          source.SetSliderValue((Decimal) designVm.Design.DesignVariables[index].Value);
      }
      Instances.ActiveCanvas.Document.NewSolution(true);
      return topDesignsVm;
    }

    public List<DesignVM> MultiGenerateAndSelectTop(int number, int popsize, double rate)
    {
      List<DesignVM> andSelectTop = this.GenerateAndSelectTop(number, popsize, rate);
      for (int index = 1; (double) index < this.autogenSlider.Value; ++index)
        andSelectTop = this.GenerateAndSelectTopFromSeeds(number, popsize, rate, new List<Design>()
        {
          andSelectTop[0].Design,
          andSelectTop[0].Design
        });
      return andSelectTop;
    }

    public List<DesignVM> GenDesignVMs(List<Design> listDesigns)
    {
      List<DesignVM> designVmList = new List<DesignVM>();
      foreach (Design listDesign in listDesigns)
      {
        for (int index = 0; index < this.Component.Params.Input[2].Sources.Count; ++index)
        {
          if (this.Component.Params.Input[2].Sources[index] is GH_NumberSlider source)
            source.SetSliderValue((Decimal) listDesign.DesignVariables[index].Value);
        }
        Instances.ActiveCanvas.Document.NewSolution(true);
        List<Line> designLines = this.Component.DesignLines;
        List<Curve> designCurves = this.Component.DesignCurves;
        List<Mesh> designMeshes = this.Component.DesignMeshes;
        List<Brep> designBreps = this.Component.DesignBreps;
        designVmList.Add(new DesignVM(designLines, designCurves, designMeshes, designBreps, false, true, this.Component.Score, listDesign));
      }
      return designVmList;
    }

    public void NewRow()
    {
      bool? isChecked = this.autoGenerateCheck.IsChecked;
      bool flag = true;
      if (isChecked.GetValueOrDefault() == flag && isChecked.HasValue)
        this.myDesignToolVM.Generations.Add(this.MultiGenerateAndSelectTop(8, (int) this.Pop.Value, this.Mut.Value));
      else
        this.myDesignToolVM.Generations.Add(this.GenerateAndSelectTop(8, (int) this.Pop.Value, this.Mut.Value));
      this.DesignGrid.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(this.RowHeight)
      });
      int index1 = this.DesignGrid.RowDefinitions.Count - 1;
      List<DesignVM> generation = this.myDesignToolVM.Generations[index1];
      TextBlock element1 = new TextBlock();
      element1.Text = index1.ToString();
      element1.Style = (Style) this.Resources[(object) "RowLabel"];
      element1.Padding = new Thickness(0.0, 0.0, 0.0, 10.0);
      TextBlock element2 = new TextBlock();
      element2.HorizontalAlignment = HorizontalAlignment.Center;
      element2.Text = "n: " + this.Pop.Value.ToString();
      TextBlock element3 = new TextBlock();
      element3.HorizontalAlignment = HorizontalAlignment.Center;
      element3.Text = "r: " + this.Mut.Value.ToString("0.00");
      StackPanel element4 = new StackPanel();
      element4.Width = this.RowHeight;
      element4.Children.Add((UIElement) element1);
      element4.Children.Add((UIElement) element2);
      element4.Children.Add((UIElement) element3);
      element4.VerticalAlignment = VerticalAlignment.Center;
      element4.HorizontalAlignment = HorizontalAlignment.Center;
      this.DesignGrid.Children.Add((UIElement) element4);
      Grid.SetColumn((UIElement) element4, 0);
      Grid.SetRow((UIElement) element4, index1);
      for (int index2 = 0; index2 < 8; ++index2)
      {
        Console.WriteLine(index2);
        DesignControl element5 = new DesignControl(generation[index2], this.RowHeight, this.RowHeight, true, (IGH_Component) this.Component);
        this.DesignGrid.Children.Add((UIElement) element5);
        Grid.SetColumn((UIElement) element5, index2 + 1);
        Grid.SetRow((UIElement) element5, index1);
      }
      TextBlock element6 = new TextBlock();
      element6.Text = "<";
      element6.Width = this.RowHeight;
      element6.Style = (Style) this.Resources[(object) "RowLabel"];
      element6.MouseLeftButtonDown += new MouseButtonEventHandler(this.GoBack_Click);
      element6.MouseEnter += new MouseEventHandler(this.GoBack_Hover);
      element6.MouseLeave += new MouseEventHandler(this.GoBack_Leave);
      this.DesignGrid.Children.Add((UIElement) element6);
      Grid.SetColumn((UIElement) element6, 10);
      Grid.SetRow((UIElement) element6, index1);
      this.myScroll.UpdateLayout();
      this.myScroll.ScrollToVerticalOffset(this.RowHeight);
    }

    private double RowHeight
    {
      get
      {
        if (this._RowHeight == 0.0)
          this._RowHeight = this.DesignGrid.ActualWidth / 10.0;
        return this._RowHeight;
      }
      set => this._RowHeight = value;
    }

    public void Initialize()
    {
      for (int index = 0; index < 11; ++index)
        this.DesignGrid.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(this.RowHeight)
        });
    }

    private IList<UIElement> GetRow(int rowIndex)
    {
      IList<UIElement> row = (IList<UIElement>) new List<UIElement>();
      int num = 10 * rowIndex + 1;
      for (int index = num; index < num + 8; ++index)
        row.Add(this.DesignGrid.Children[index]);
      return row;
    }

    private void Button_Click_Generate(object sender, RoutedEventArgs e)
    {
      this.NewRow();
      this.myDesignToolVM.Seeds.Clear();
    }

    private void GoBack_Hover(object sender, RoutedEventArgs e)
    {
      if (!(sender is TextBlock textBlock))
        return;
      textBlock.Foreground = (Brush) new SolidColorBrush(Colors.Gray);
    }

    private void GoBack_Leave(object sender, RoutedEventArgs e)
    {
      if (!(sender is TextBlock textBlock))
        return;
      textBlock.Foreground = (Brush) new SolidColorBrush(Colors.Black);
    }

    private void GoBack_Click(object sender, RoutedEventArgs e)
    {
      int row = Grid.GetRow((UIElement) (e.OriginalSource as TextBlock));
      int num = 10 * (row + 1);
      while (this.DesignGrid.Children.Count > num)
        this.DesignGrid.Children.RemoveAt(this.DesignGrid.Children.Count - 1);
      int count = this.myDesignToolVM.Generations.Count - row - 1;
      this.myDesignToolVM.Generations.RemoveRange(row + 1, count);
      while (this.DesignGrid.RowDefinitions.Count > row + 1)
        this.DesignGrid.RowDefinitions.RemoveAt(this.DesignGrid.RowDefinitions.Count - 1);
      foreach (DesignControl designControl in (IEnumerable<UIElement>) this.GetRow(row))
        designControl.myViewModel.IsClickable = true;
    }

    private void Button_Click_Build(object sender, RoutedEventArgs e)
    {
      RhinoDoc activeDoc = RhinoDoc.ActiveDoc;
      int index = activeDoc.Layers.Find("Exploration", true);
      if (index < 0)
      {
        activeDoc.Layers.Add(new Layer()
        {
          Name = "Exploration"
        });
        index = activeDoc.Layers.Find("Exploration", true);
      }
      int num1 = 0;
      if (activeDoc.Layers[index].GetChildren() != null)
        num1 = activeDoc.Layers[index].GetChildren().Length;
      Layer layer = new Layer();
      layer.Name = "Exploration" + num1.ToString();
      layer.ParentLayerId = activeDoc.Layers[index].Id;
      activeDoc.Layers.Add(layer);
      int num2 = activeDoc.Layers.Find(layer.Name, true);
      ObjectAttributes objectAttributes = new ObjectAttributes();
      objectAttributes.LayerIndex = num2;
      foreach (Line designLine in this.Component.DesignLines)
        activeDoc.Objects.AddLine(designLine, objectAttributes);
      foreach (Curve designCurve in this.Component.DesignCurves)
        activeDoc.Objects.AddCurve(designCurve, objectAttributes);
      foreach (Brep designBrep in this.Component.DesignBreps)
        activeDoc.Objects.AddBrep(designBrep, objectAttributes);
      foreach (Mesh designMesh in this.Component.DesignMeshes)
        activeDoc.Objects.AddMesh(designMesh, objectAttributes);
    }

    private void Button_MouseRightButtonDown_Build(object sender, MouseButtonEventArgs e)
    {
      string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
      Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      csvWriter.CreateRecord(folderPath.Replace("\\", "\\\\"), "ExplorationRecord", this.myDesignToolVM.ExplorationRec, this.Component.Params.Input[2].Sources.Count);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/StormCloud;component/stormcloudwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.SCWindow = (StormCloudWindow) target;
          break;
        case 2:
          this.CurrentModel = (HelixViewport3D) target;
          break;
        case 3:
          this.autoGenerateCheck = (CheckBox) target;
          break;
        case 4:
          this.autogenSlider = (Slider) target;
          break;
        case 5:
          this.Pop = (Slider) target;
          break;
        case 6:
          this.Mut = (Slider) target;
          break;
        case 7:
          this.myScroll = (ScrollViewer) target;
          break;
        case 8:
          this.DesignGrid = (Grid) target;
          break;
        case 9:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.Button_Click_Generate);
          break;
        case 10:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.Button_Click_Build);
          ((UIElement) target).MouseRightButtonDown += new MouseButtonEventHandler(this.Button_MouseRightButtonDown_Build);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
