using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Radical.Integration;
using System.Threading;
using System.Windows.Markup;
using NLoptNet;
using System.Globalization;
using System.Reflection;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using MaterialDesignThemes;
using DSOptimization;


namespace Radical
{
    /// <summary>
    /// Interaction logic for MaiWindow.xaml
    /// </summary>
    public partial class RadicalWindow : Window
    {
        private Dictionary<string, List<GraphControl>> GraphControls;
        private List<GroupVariableControl> GroupVars;
        public enum Direction { X, Y, Z, None };
        public RadicalVM RadicalVM;
        public CancellationTokenSource source;
        private bool IsAlreadyLoaded;

        public RadicalWindow()
        {
            this.DataContext = RadicalVM;
            InitializeComponent();
        }

        //CONSTRUCTOR
        public RadicalWindow(RadicalVM radicalVM)
        {
            this.RadicalVM = radicalVM;
            this.DataContext = this.RadicalVM;
            InitializeComponent();

            this.IsAlreadyLoaded = false;

            //GRAPH CONTROLS
            //Dictionary containing lists of GraphControls to be displayed, with string keys specifying graph data types.
            this.GraphControls = new Dictionary<string, List<GraphControl>>();
            this.GraphControls.Add("Main", new List<GraphControl>());
            this.GraphControls.Add("Constraints", new List<GraphControl>());

            this.GroupVars = new List<GroupVariableControl>();
            AddNumbers();
            AddGeometries();

            this.SettingsMenu.Children.Add(new RadicalSettingsControl(this.RadicalVM));

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        //MAIN WINDOW LOADED
        //Ensures main graph height is correct on loading
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsAlreadyLoaded)
            {
                AddGraphs(); //Also adds constraints
                this.IsAlreadyLoaded = true;
            }
        }

        //ACTIVE GRAPHS
        //A list of active constraint graphs to populate the window graph grid
        public List<GraphControl> ActiveGraphs
        {
            get
            {
                if (this.GraphControls["Main"].Any())
                {
                    List<GraphControl> list = new List<GraphControl>();
                    list.Add(this.GraphControls["Main"][0]);

                    foreach (GraphControl graph in GraphControls["Constraints"].Where(g => g.GraphVM.GraphVisibility == Visibility.Visible))
                        list.Add(graph);

                    return list;
                }
                return null;
            }
        }

        //GRAPH DISPLAY LISTS
        //Divides graphs into appropriate sub-lists for data display
        public List<List<GraphControl>> GraphDisplayLists
        {
            get
            {
                //Count all graphs but the main objective
                int numGraphs = this.ActiveGraphs.Count - 1;

                List<GraphControl> grid = new List<GraphControl>();
                List<GraphControl> lastRow = new List<GraphControl>();

                if ((numGraphs % 2) == 1 && numGraphs >= 3)
                {
                    for (int i = 0; i < numGraphs - 3; i++)
                        grid.Add(this.ActiveGraphs[i + 1]);
                    for (int i = numGraphs - 3; i < numGraphs; i++)
                        lastRow.Add(this.ActiveGraphs[i + 1]);
                }
                else
                    for (int i = 0; i < numGraphs; i++)
                        grid.Add(this.ActiveGraphs[i + 1]);

                return new List<List<GraphControl>> { grid, lastRow };
            }
        }

        //ADD GRAPHS
        //Creates a graph for the main objective and for all of the constraints
        private void AddGraphs()
        {
            //MAIN OBJECTIVE GRAPH
            var g = new GraphControl(this.RadicalVM.Graphs["Main"][0], this.RadicalVM, this);
            //g.GraphGrid.Height = MainGrid.ActualHeight * 0.9;
            g.UpdateHeightFullScreen();

            GraphControls["Main"].Add(g);
            MainBlock.Children.Add(GraphControls["Main"][0]);

            //CONSTRAINTS GRAPHS
            //Collapse Constraints expander if no constraints are imposed
            if (RadicalVM.Constraints.Any())
            {
                VariableHeaderControl labels = new VariableHeaderControl();
                labels.HeaderGrid.Margin = new Thickness(25, 0, 0, 0);
                this.Constraints.Children.Add(labels);

                for (int i = 0; i < RadicalVM.Constraints.Count; i++)
                {
                    GraphVM gvm = RadicalVM.Graphs["Constraints"][i];
                    g = new GraphControl(gvm, this.RadicalVM, this);
                    // g.GraphGrid.Height = MainGrid.ActualHeight * 0.45;
                    g.UpdateHeightHalfScreen();
                    GraphControls["Constraints"].Add(g);

                    ConstVM c = RadicalVM.Constraints[i];
                    this.Constraints.Children.Add(new ConstraintControl(c, this));
                }
            }
            else
                this.ConstraintsExpander.Visibility = Visibility.Collapsed;
        }

        //ADD NUMBERS
        //Adds a stack panel for numeric sliders
        private void AddNumbers()
        {
            //COLLAPSE SLIDERS
            //Collapse Sliders expander if no sliders are connected
            if (!RadicalVM.NumVars.Any())
            {
                this.SlidersExpander.Visibility = Visibility.Collapsed;
                return;
            }

            //GROUP VARIABLE CONTROL
            //stackpanel
            StackPanel groupControls = new StackPanel();
            groupControls.Background = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
            //expander
            Expander groupControlMenu = new Expander();
            groupControlMenu.Background = (SolidColorBrush)this.FindResource("BackgroundHueDarkBrush");
            groupControlMenu.IsExpanded = true;
            groupControlMenu.Header = Header2Formatting("Group Variable Control");
            groupControlMenu.Content = groupControls;
            this.Sliders.Children.Add(groupControlMenu);

            //Add descriptive control labels
            groupControls.Children.Add(new VariableHeaderControl());

            //Add group controls for slider direction
            GroupVariableControl groupControl = new GroupVariableControl(new GroupVarVM(this.RadicalVM, (int)(Direction.None))); this.GroupVars.Add(groupControl);
            groupControl.GroupControlName.Text = "All Variables";
            groupControls.Children.Add(groupControl);

            //Border separator
            this.Sliders.Children.Add(this.Separator());

            //INDIVIDUAL VARIBALE CONTROL
            //Stack Panel
            StackPanel individualControls = new StackPanel();
            individualControls.Background = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
            //Expander
            Expander individualControlMenu = new Expander();
            individualControlMenu.Background = (SolidColorBrush)this.FindResource("BackgroundHueDarkBrush");
            individualControlMenu.Header = Header2Formatting("Single Variable Control");
            individualControlMenu.Content = individualControls;
            this.Sliders.Children.Add(individualControlMenu);

            //Add descriptive control labels
            individualControls.Children.Add(new VariableHeaderControl());

            //Add individual slider controls
            foreach (VarVM var in RadicalVM.NumVars)
                individualControls.Children.Add(new VariableControl(var));
        }

        //ADD GEOMETRIES
        //Adds a nested stack panel for geometries and their control point variables
        private void AddGeometries()
        {
            //COLLAPSE GEOMETRIES
            //Collapse Geometries expander if no geometries are connected
            if (!RadicalVM.GeoVars.Any())
            {
                this.GeometriesExpander.Visibility = Visibility.Collapsed;
                return;
            }

            int geoIndex = 0;

            foreach (List<VarVM> geometry in RadicalVM.GeoVars)
            {

                //SINGLE GEOMETRY
                //Stack Panel
                StackPanel variableMenus = new StackPanel();
                variableMenus.Background = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
                //Expander
                Expander singleGeo = new Expander();

                singleGeo.Background = (SolidColorBrush)this.FindResource("BackgroundHueMidBrush");
                singleGeo.Header = Header1Formatting(geometry[geoIndex].Name.Split('.')[0]); geoIndex++;
                singleGeo.Content = variableMenus;
                this.Geometries.Children.Add(singleGeo);

                //Border
                variableMenus.Children.Add(this.Separator());


                //GROUP VARIABLE CONTROL
                //Stack Panel
                StackPanel groupControls = new StackPanel();
                groupControls.Background = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
                //Expander
                Expander groupControlMenu = new Expander();
                groupControlMenu.IsExpanded = true;
                groupControlMenu.Header = Header2Formatting("Group Variable Control");
                groupControlMenu.Background = (SolidColorBrush)this.FindResource("BackgroundHueDarkBrush");
                //groupControlMenu.Foreground = (SolidColorBrush)this.FindResource("BackgroundHueDarkSubtextForegroundBrush");
                groupControlMenu.Content = groupControls;
                variableMenus.Children.Add(groupControlMenu);

                //Add descriptive control labels
                groupControls.Children.Add(new VariableHeaderControl());

                //Add group controls for X, Y, and Z directions
                GroupVariableControl groupControlX = new GroupVariableControl(new GroupVarVM(this.RadicalVM, (int)(Direction.X), geoIndex - 1)); this.GroupVars.Add(groupControlX);
                GroupVariableControl groupControlY = new GroupVariableControl(new GroupVarVM(this.RadicalVM, (int)(Direction.Y), geoIndex - 1)); this.GroupVars.Add(groupControlY);
                GroupVariableControl groupControlZ = new GroupVariableControl(new GroupVarVM(this.RadicalVM, (int)(Direction.Z), geoIndex - 1)); this.GroupVars.Add(groupControlZ);

                groupControlX.GroupControlName.Text = "X Variables";
                groupControlY.GroupControlName.Text = "Y Variables";
                groupControlZ.GroupControlName.Text = "Z Variables";
                groupControls.Children.Add(groupControlX);
                groupControls.Children.Add(groupControlY);
                groupControls.Children.Add(groupControlZ);

                //Border separator
                variableMenus.Children.Add(this.Separator());

                //INDIVIDUAL VARIBALE CONTROL
                //Stack Panel
                StackPanel individualControls = new StackPanel();
                individualControls.Background = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
                //Expander
                Expander individualControlMenu = new Expander();
                individualControlMenu.Header = Header2Formatting("Single Variable Control");
                individualControlMenu.Content = individualControls;
                individualControlMenu.Background = (SolidColorBrush)this.FindResource("BackgroundHueDarkBrush");
                variableMenus.Children.Add(individualControlMenu);

                //Add descriptive control labels
                individualControls.Children.Add(new VariableHeaderControl());

                //Add individual point controls in all directions
                foreach (VarVM var in geometry)
                {
                    VariableControl vc = new VariableControl(var);
                    vc.Background = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
                    individualControls.Children.Add(vc);
                }
            }
        }

        //Formatting individual geometry headers
        private TextBlock Header1Formatting(string text)
        {
            TextBlock header = new TextBlock(new Run(text));
            header.Foreground = (SolidColorBrush)this.FindResource("BackgroundHueDarkForegroundBrush");
            header.FontSize = 16;

            return header;
        }

        //Formatting control group headers
        private TextBlock Header2Formatting(string text)
        {
            TextBlock header = new TextBlock(new Run(text));
            header.Foreground = (SolidColorBrush)this.FindResource("PrimaryHueDarkForegroundBrush");
            header.FontSize = 16;

            return header;
        }

        //Formatting border separators
        private Border Separator()
        {
            Border b = new Border();
            b.Height = 1;
            b.HorizontalAlignment = HorizontalAlignment.Stretch;
            b.SnapsToDevicePixels = true;
            b.BorderThickness = new Thickness(0, 0, 0, 2);
            b.BorderBrush = (SolidColorBrush)this.FindResource("BackgroundHueMidBrush");
            return b;
        }

        //UpdatedGraphVisibility
        public void UpdatedGraphVisibility()
        {
            List<List<GraphControl>> displayData = this.GraphDisplayLists;
            ConstraintsGraphs.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = displayData[0] });
            ConstraintsGraphs2.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = displayData[1] });

            if (this.ActiveGraphs.Count == 1)
            {
                //this.ActiveGraphs[0].GraphGrid.Height = 0.9 * this.MainGrid.ActualHeight;
                this.ActiveGraphs[0].UpdateHeightFullScreen();
            }
            else
            {
                //this.ActiveGraphs[0].GraphGrid.Height = 0.45 * this.MainGrid.ActualHeight;
                this.ActiveGraphs[0].UpdateHeightHalfScreen();

                if (this.ActiveGraphs.Count == 2)
                {
                    this.RadicalVM.Cols = 1; //two columns
                }
                else
                {
                    this.RadicalVM.Cols = 2; //three columns
                }

                foreach (GraphControl g in this.ActiveGraphs)
                {
                    //g.GraphGrid.Height = 0.45 * this.MainGrid.ActualHeight;
                    g.UpdateHeightHalfScreen();
                }
            }
        }

        //OPTIMIZE
        private void Optimize()
        {
            this.RadicalVM.Optimize(this);
        }

        //OPTIMIZATION STARTED
        public void OptimizationStarted()
        {
            this.RadicalVM.OptimizationStarted();
            foreach (GroupVariableControl group in this.GroupVars)
                group.OptimizationStarted();
        }

        //OPTIMIZATION FINISHED
        public void OptimizationFinished()
        {
            this.RadicalVM.OptimizationFinished();
            foreach (GroupVariableControl group in this.GroupVars)
                group.OptimizationFinished();
        }

        //IDEALLY WHAT WILL BE USED
        public void WindowClosing()
        {
            if (ButtonPause.Visibility == Visibility.Visible)
            {
                source.Cancel();
            }
        }

        public void Window_Closing(object sender, CancelEventArgs e)
        {
            this.WindowClosing();
            //if radical is running
            //throw stop
            this.RadicalVM.OnWindowClosing();
        }

        //WINDOW CLOSING - I believe this is no longer in use because of DSOpt closing function
        //Alert the component that the window has been closed
        //(and therefore a new window can open on double click)
        //public void RadicalWindow_Closing(object sender, CancelEventArgs e)
        //{
        //    if(ButtonPause.Visibility == Visibility.Visible)
        //    {
        //        source.Cancel();
        //    }
        //    this.RadicalVM.OnWindowClosing();
        //}

        //ANIMATION BEGAN
        private void AnimationBegan()
        {
            foreach (GraphControl g in this.ActiveGraphs)
                g.ChartRow.Visibility = Visibility.Collapsed;
        }

        //ANIMATION COMPLETED
        private void Animation_Completed(object sender, EventArgs e)
        {
            foreach (GraphControl g in this.ActiveGraphs)
                g.ChartRow.Visibility = Visibility.Visible;
        }

        private void UpdateGraphSize(object sender, RoutedEventArgs e)
        {
            if (this.ActiveGraphs != null)
            {
                if (this.ActiveGraphs.Count == 1)
                {
                    //this.ActiveGraphs[0].GraphGrid.Height = 0.9 * this.MainGrid.ActualHeight;
                    this.ActiveGraphs[0].UpdateHeightFullScreen();
                }
                else
                {
                    foreach (GraphControl g in this.ActiveGraphs)
                    {
                        //g.GraphGrid.Height = 0.45 * this.MainGrid.ActualHeight;
                        g.UpdateHeightHalfScreen();
                    }
                }
            }
        }

        #region Button_Events
        //OPEN MENU CLICK
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
            GridMenu.Background = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
            this.AnimationBegan();
        }

        //CLOSE MENU CLICK
        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            // GridMenu.Background = (SolidColorBrush)this.FindResource("BackgroundHueDarkBrush");
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            GridMenu.Background = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
            this.AnimationBegan();
        }

        //BUTTON PLAY CLICK
        private async void ButtonPlay_Click(object sender, RoutedEventArgs e) // make async
        {
            if (this.RadicalVM.Design.ActiveVariables.Any())
            {
                ButtonPause.Visibility = Visibility.Visible;
                ButtonPlay.Visibility = Visibility.Collapsed;
                source = new CancellationTokenSource();

                try
                {
                    await Task.Run(() => Optimize(), source.Token);

                }
                catch (OperationCanceledException)
                {
                    //System.Windows.MessageBox.Show("Cancelled");
                }
                // UpdateAllGraphs();
                ButtonPause.Visibility = Visibility.Collapsed;
                ButtonPlay.Visibility = Visibility.Visible;
            }
            else
                System.Windows.MessageBox.Show("No variables selected!");
        }

        //BUTTON PAUSE CLICK
        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            this.OptimizationFinished(); //Enable variable changes when paused
            source.Cancel();
            //UpdateAllGraphs();
            ButtonPause.Visibility = Visibility.Collapsed;
            ButtonPlay.Visibility = Visibility.Visible;
        }

        //BUTTON SETTINGS OPEN CLICK
        private void ButtonSettingsOpen_Click(object sender, RoutedEventArgs e)
        {
            ButtonSettingsOpen.Visibility = Visibility.Collapsed;
            SettingsClose.Visibility = Visibility.Visible;
            this.AnimationBegan();
        }

        //BUTTON SETTINGS CLOSE CLICK
        private void ButtonSettingsClose_Click(object sender, RoutedEventArgs e)
        {
            ButtonSettingsOpen.Visibility = Visibility.Visible;
            SettingsClose.Visibility = Visibility.Collapsed;
            this.AnimationBegan();
        }

        //BUTTON RESTART (reset) CLICK
        private void ButtonRestart_Click(object sender, RoutedEventArgs e)
        {
            this.RadicalVM.ResetObjective();
            foreach (VarVM v in this.RadicalVM.NumVars)
            {
                v.ResetValue();
            }
            foreach (List<VarVM> lvm in this.RadicalVM.GeoVars)
            {
                foreach (VarVM v in lvm)
                {
                    v.ResetValue();
                }
            }

            foreach (IDesignGeometry geo in this.RadicalVM.Design.Geometries)
            {
                geo.Update();
            }

            UpdateRhino();
            this.RadicalVM.UpdateCurrentScoreDisplay();
            this.RadicalVM.ClearGraphs();
        }

        //BUTTON OPTIMAL RESULT CLICK
        private void ButtonOptimalResult_Click(object sender, RoutedEventArgs e)
        {
            foreach (VarVM v in this.RadicalVM.NumVars)
            {
                v.SetBestSolution();
            }
            foreach (List<VarVM> lvm in this.RadicalVM.GeoVars)
            {
                foreach (VarVM v in lvm)
                {
                    v.SetBestSolution();
                }
            }

            foreach (IDesignGeometry geo in this.RadicalVM.Design.Geometries)
            {
                geo.Update();
            }

            UpdateRhino();
            this.RadicalVM.UpdateCurrentScoreDisplay();
            this.RadicalVM.ClearGraphs();
        }
        #endregion

        //Updates variables in Rhino
        public void UpdateRhino()
        {
            bool finished = false;
            System.Action run = delegate ()
            {
                Grasshopper.Kernel.GH_SolutionMode refresh = Grasshopper.Kernel.GH_SolutionMode.Default;
                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false, refresh);
                finished = true;
            };
            Rhino.RhinoApp.MainApplicationWindow.Invoke(run);
            while (!finished) { }
        }

        #region Export
        //EXPORT BUTTON
        //Launches dialog prompting user for a file path
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            this.ExportCSVWindow.IsOpen = true;
        }

        //EXPORT to .CSV
        //Exports all ObjectiveEvolution, VariableEvolution, and GradientEvolution data
        private void ExportCSV(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            //CANCEL
            //Don't export file if user clicked cancel
            if (!Equals(eventArgs.Parameter, true)) return;

            var CSVFilepath = this.Filepath.Text;
            var CSVFilename = this.Filename.Text;
            if (string.IsNullOrWhiteSpace(CSVFilename))
            {
                CSVFilename = "Untitled";
            }

            String timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            String filepath = @"" + CSVFilepath + "/" + CSVFilename + "_" + timestamp;

            this.RadicalVM.ExportCSV_Log(filepath);
            this.RadicalVM.ExportCSV_Raw(filepath);
        }

        private void Filepath_TextChanged(object sender, TextChangedEventArgs e)
        {
            var CSVFilepath = this.Filepath.Text;
            String filepath = @"" + CSVFilepath;

            if (this.ExportCSVWindow.IsOpen)
            {
                if (System.IO.Directory.Exists(filepath))
                {
                    this.ExportWindowButton.IsEnabled = true;
                }
                else
                {
                    this.ExportWindowButton.IsEnabled = false;
                }
            }
        }
        #endregion

    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum RefreshMode
    {
        [Description("Live Geometry and Data")] Live = 1,
        [Description("Live Data")] Data = 2,
        [Description("Silent")] Silent = 3
    }

    #region Converters
    public class EnumDescriptionTypeConverter : EnumConverter
    {
        public EnumDescriptionTypeConverter(Type type)
            : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
                    FieldInfo fi = value.GetType().GetField(value.ToString());
                    if (fi != null)
                    {
                        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        return ((attributes.Length > 0) && (!String.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
                    }
                }
                return string.Empty;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type _enumType;
        public Type EnumType
        {
            get { return this._enumType; }
            set
            {
                if (value != this._enumType)
                {
                    if (null != value)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;

                        if (!enumType.IsEnum)
                            throw new ArgumentException("Type must be for an Enum.");
                    }
                    this._enumType = value;
                }
            }
        }
        public EnumBindingSourceExtension() { }
        public EnumBindingSourceExtension(Type enumType)
        {
            this.EnumType = enumType;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == this._enumType)
                throw new InvalidOperationException("The EnumType must be specified.");
            Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;
            Array enumValues = Enum.GetValues(actualEnumType);
            if (actualEnumType == this._enumType)
                return enumValues;
            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
    }

    public class VisibilityToCheckedConverter : IValueConverter

    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)

        {

            return ((Visibility)value) == Visibility.Visible;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)

        {

            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;

        }

    }
    #endregion
}