using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace Stepper
{
    // Token: 0x02000021 RID: 33
    public partial class StepperWindow : Window, IComponentConnector
    {
        // Token: 0x06000129 RID: 297 RVA: 0x00006FCB File Offset: 0x000051CB
        public StepperWindow()
        {
            this.InitializeComponent();
        }

        // Token: 0x0600012A RID: 298 RVA: 0x00006FDC File Offset: 0x000051DC
        public StepperWindow(StepperVM svm)
        {
            this.StepperVM = svm;
            base.DataContext = this.StepperVM;
            this.InitializeComponent();
            this.Objectives = new List<DataControl>();
            this.StepObjs = new List<DataControl>();
            foreach (ObjectiveVM data in this.StepperVM.Objectives)
            {
                this.Objectives.Add(new DataControl(data));
                DataControl item = new DataControl(data);
                this.StepObjs.Add(item);
            }
            this.GroupVars = new List<GroupVariableControl>();
            this.Variables = new List<VariableControl>();
            this.Gradients = new List<DataControl>();
            this.StepVars = new List<DataControl>();
            foreach (VarVM varVM in this.StepperVM.Variables)
            {
                this.Variables.Add(new VariableControl(varVM));
                this.Gradients.Add(new DataControl(varVM));
                DataControl item2 = new DataControl(varVM);
                this.StepVars.Add(item2);
            }
            this.GenerateChartColors();
            this.StepperVM.ObjectiveChart_Norm.Colors = this.ChartColors;
            this.Chart_Norm = new StepperGraphControl(this.StepperVM.ObjectiveChart_Norm);
            this.StepperVM.ObjectiveChart_Abs.Colors = this.ChartColors;
            this.Chart_Abs = new StepperGraphControl(this.StepperVM.ObjectiveChart_Abs);
            this.ConfigureDisplay();
        }

        // Token: 0x0600012B RID: 299 RVA: 0x000071A8 File Offset: 0x000053A8
        private void GenerateChartColors()
        {
            this.ChartColors = new List<SolidColorBrush>();
            SolidColorBrush item = (SolidColorBrush)base.FindResource("SecondaryAccentBrush");
            this.ChartColors.Add(item);
            SolidColorBrush item2 = (SolidColorBrush)base.FindResource("PrimaryHueDarkBrush");
            this.ChartColors.Add(item2);
            SolidColorBrush item3 = (SolidColorBrush)base.FindResource("PrimaryHueMidBrush");
            this.ChartColors.Add(item3);
            SolidColorBrush item4 = (SolidColorBrush)base.FindResource("PrimaryHueLightBrush");
            this.ChartColors.Add(item4);
            SolidColorBrush black = Brushes.Black;
            this.ChartColors.Add(black);
            SolidColorBrush gray = Brushes.Gray;
            this.ChartColors.Add(gray);
        }

        // Token: 0x0600012C RID: 300 RVA: 0x00007264 File Offset: 0x00005464
        private void ConfigureDisplay()
        {
            this.AddNumbers();
            this.AddGeometries();
            foreach (GroupVariableControl groupVariableControl in this.GroupVars)
            {
                this.StepperVM.GroupVars.Add((GroupVarVM)groupVariableControl.MyVM);
            }
            this.ObjData.SetBinding(ItemsControl.ItemsSourceProperty, new Binding
            {
                Source = this.Objectives
            });
            this.GradientData.SetBinding(ItemsControl.ItemsSourceProperty, new Binding
            {
                Source = this.Gradients
            });
            this.ChartPanel.Children.Add(this.Chart_Norm);
            this.StepObjData.SetBinding(ItemsControl.ItemsSourceProperty, new Binding
            {
                Source = this.StepObjs
            });
            this.StepVarData.SetBinding(ItemsControl.ItemsSourceProperty, new Binding
            {
                Source = this.StepVars
            });
            this.SettingsExpander.Content = new SettingsControl(this.StepperVM, this);
        }

        // Token: 0x0600012D RID: 301 RVA: 0x00007398 File Offset: 0x00005598
        private TextBlock Header1Formatting(string text)
        {
            return new TextBlock(new Run(text))
            {
                Foreground = (SolidColorBrush)base.FindResource("LightTextBrush"),
                FontSize = 16.0
            };
        }

        // Token: 0x0600012E RID: 302 RVA: 0x000073E0 File Offset: 0x000055E0
        private TextBlock Header2Formatting(string text)
        {
            return new TextBlock(new Run(text))
            {
                Foreground = (SolidColorBrush)base.FindResource("PrimaryHueDarkForegroundBrush"),
                FontSize = 16.0
            };
        }

        // Token: 0x0600012F RID: 303 RVA: 0x00007428 File Offset: 0x00005628
        private Border Separator()
        {
            return new Border
            {
                Height = 1.0,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                SnapsToDevicePixels = true,
                BorderThickness = new Thickness(0.0, 0.0, 0.0, 2.0),
                BorderBrush = (SolidColorBrush)base.FindResource("BackgroundHueMidBrush")
            };
        }

        // Token: 0x06000130 RID: 304 RVA: 0x000074A8 File Offset: 0x000056A8
        private void AddNumbers()
        {
            bool flag = !this.StepperVM.NumVars.Any<VarVM>();
            if (flag)
            {
                this.SlidersExpander.Visibility = Visibility.Collapsed;
            }
            else
            {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Background = (SolidColorBrush)base.FindResource("PrimaryHueDarkBrush");
                Expander expander = new Expander();
                expander.Background = (SolidColorBrush)base.FindResource("BackgroundHueDarkBrush");
                expander.IsExpanded = true;
                expander.Header = this.Header2Formatting("Group Variable Control");
                expander.Content = stackPanel;
                this.Sliders.Children.Add(expander);
                stackPanel.Children.Add(new VariableHeaderControl());
                GroupVariableControl groupVariableControl = new GroupVariableControl(new GroupVarVM(this.StepperVM, 3, 0));
                this.GroupVars.Add(groupVariableControl);
                groupVariableControl.GroupControlName.Text = "All Variables";
                stackPanel.Children.Add(groupVariableControl);
                this.Sliders.Children.Add(this.Separator());
                StackPanel stackPanel2 = new StackPanel();
                stackPanel2.Background = (SolidColorBrush)base.FindResource("PrimaryHueDarkBrush");
                Expander expander2 = new Expander();
                expander2.Background = (SolidColorBrush)base.FindResource("BackgroundHueDarkBrush");
                expander2.Header = this.Header2Formatting("Single Variable Control");
                expander2.Content = stackPanel2;
                this.Sliders.Children.Add(expander2);
                stackPanel2.Children.Add(new VariableHeaderControl());
                foreach (VarVM varvm in this.StepperVM.NumVars)
                {
                    stackPanel2.Children.Add(new VariableControl(varvm));
                }
            }
        }

        // Token: 0x06000131 RID: 305 RVA: 0x00007690 File Offset: 0x00005890
        private void AddGeometries()
        {
            bool flag = !this.StepperVM.GeoVars.Any<List<VarVM>>();
            if (flag)
            {
                this.GeometriesExpander.Visibility = Visibility.Collapsed;
            }
            else
            {
                int num = 0;
                foreach (List<VarVM> list in this.StepperVM.GeoVars)
                {
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Background = (SolidColorBrush)base.FindResource("PrimaryHueDarkBrush");
                    Expander expander = new Expander();
                    expander.Background = (SolidColorBrush)base.FindResource("BackgroundHueMidBrush");
                    expander.Header = this.Header1Formatting(list[num].Name.Split(new char[]
                    {
                        '.'
                    })[0]);
                    num++;
                    expander.Content = stackPanel;
                    this.Geometries.Children.Add(expander);
                    stackPanel.Children.Add(this.Separator());
                    StackPanel stackPanel2 = new StackPanel();
                    stackPanel2.Background = (SolidColorBrush)base.FindResource("PrimaryHueDarkBrush");
                    Expander expander2 = new Expander();
                    expander2.IsExpanded = true;
                    expander2.Header = this.Header2Formatting("Group Variable Control");
                    expander2.Background = (SolidColorBrush)base.FindResource("BackgroundHueDarkBrush");
                    expander2.Content = stackPanel2;
                    stackPanel.Children.Add(expander2);
                    stackPanel2.Children.Add(new VariableHeaderControl());
                    GroupVariableControl groupVariableControl = new GroupVariableControl(new GroupVarVM(this.StepperVM, 0, num - 1));
                    this.GroupVars.Add(groupVariableControl);
                    GroupVariableControl groupVariableControl2 = new GroupVariableControl(new GroupVarVM(this.StepperVM, 1, num - 1));
                    this.GroupVars.Add(groupVariableControl2);
                    GroupVariableControl groupVariableControl3 = new GroupVariableControl(new GroupVarVM(this.StepperVM, 2, num - 1));
                    this.GroupVars.Add(groupVariableControl3);
                    groupVariableControl.GroupControlName.Text = "X Variables";
                    groupVariableControl2.GroupControlName.Text = "Y Variables";
                    groupVariableControl3.GroupControlName.Text = "Z Variables";
                    stackPanel2.Children.Add(groupVariableControl);
                    stackPanel2.Children.Add(groupVariableControl2);
                    stackPanel2.Children.Add(groupVariableControl3);
                    stackPanel.Children.Add(this.Separator());
                    StackPanel stackPanel3 = new StackPanel();
                    stackPanel3.Background = (SolidColorBrush)base.FindResource("PrimaryHueDarkBrush");
                    Expander expander3 = new Expander();
                    expander3.Header = this.Header2Formatting("Single Variable Control");
                    expander3.Content = stackPanel3;
                    expander3.Background = (SolidColorBrush)base.FindResource("BackgroundHueDarkBrush");
                    stackPanel.Children.Add(expander3);
                    stackPanel3.Children.Add(new VariableHeaderControl());
                    foreach (VarVM varvm in list)
                    {
                        VariableControl variableControl = new VariableControl(varvm);
                        variableControl.Background = (SolidColorBrush)base.FindResource("PrimaryHueDarkBrush");
                        stackPanel3.Children.Add(variableControl);
                    }
                }
            }
        }

        // Token: 0x06000132 RID: 306 RVA: 0x00007A20 File Offset: 0x00005C20
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ChartCard.Height = this.MainGrid.ActualHeight;
        }

        // Token: 0x06000133 RID: 307 RVA: 0x00007A3A File Offset: 0x00005C3A
        public void Window_Closing(object sender, CancelEventArgs e)
        {
            this.StepperVM.OnWindowClosing();
        }

        // Token: 0x06000134 RID: 308 RVA: 0x00007A49 File Offset: 0x00005C49
        private void UpdateGraphSize(object sender, RoutedEventArgs e)
        {
            this.ChartCard.Height = this.MainGrid.ActualHeight;
        }

        // Token: 0x06000135 RID: 309 RVA: 0x00007A64 File Offset: 0x00005C64
        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string name = button.Name;
            StepperOptimizer stepperOptimizer = new StepperOptimizer(this.StepperVM.Design, this.StepperVM.FDStepSize, !this.StepperVM.DisablingNotAllowed);
            List<List<double>> list = stepperOptimizer.CalculateGradient();
            int objIndex = this.StepperVM.ObjIndex;
            int num = 0;
            while ((double)num < (double)list[objIndex].Count)
            {
                this.Gradients[num].Value = list[objIndex][num];
                num++;
            }
            bool flag = name != "ButtonGradient";
            if (flag)
            {
                this.GraphSlider.Visibility = Visibility.Visible;
                bool flag2 = name == "ButtonStepUp";
                StepperOptimizer.Direction direction;
                if (flag2)
                {
                    direction = StepperOptimizer.Direction.Maximize;
                }
                else
                {
                    bool flag3 = name == "ButtonStepDown";
                    if (flag3)
                    {
                        direction = StepperOptimizer.Direction.Minimize;
                    }
                    else
                    {
                        direction = StepperOptimizer.Direction.Isoperformance;
                    }
                }
                bool flag4 = !this.StepperVM.IsoperformancePossible() && direction == StepperOptimizer.Direction.Isoperformance;
                if (flag4)
                {
                    this.StepperVM.OpenIsoDialog = true;
                }
                else
                {
                    this.StepperVM.Optimize(direction, list, stepperOptimizer);
                    bool? isChecked = ((SettingsControl)this.SettingsExpander.Content).DisplayModeButton.IsChecked;
                    bool flag5 = isChecked != null && isChecked.Value;
                    for (int i = 0; i < this.Objectives.Count; i++)
                    {
                        bool flag6 = flag5;
                        if (flag6)
                        {
                            this.Objectives[i].Value = this.StepperVM.ObjectiveEvolution_Abs[i].Last<double>();
                        }
                        else
                        {
                            this.Objectives[i].Value = this.StepperVM.ObjectiveEvolution_Norm[i].Last<double>();
                        }
                    }
                    bool flag7 = flag5;
                    if (flag7)
                    {
                        this.Chart_Abs.ForceGraphUpdate();
                    }
                    else
                    {
                        this.Chart_Norm.ForceGraphUpdate();
                    }
                }
            }
            else
            {
                this.GradientsExpander.IsExpanded = true;
                this.ObjectiveData.IsExpanded = true;
            }
        }

        // Token: 0x06000136 RID: 310 RVA: 0x00007C90 File Offset: 0x00005E90
        private void GraphSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int index = (int)this.GraphSlider.Value;
            int num = 0;
            foreach (DataControl dataControl in this.StepVars)
            {
                this.StepVars[num].Value = this.StepperVM.VariableEvolution[num][index];
                num++;
            }
            num = 0;
            foreach (DataControl dataControl2 in this.StepObjs)
            {
                bool? isChecked = ((SettingsControl)this.SettingsExpander.Content).DisplayModeButton.IsChecked;
                bool flag = isChecked != null && isChecked.Value;
                bool flag2 = flag;
                if (flag2)
                {
                    this.StepVars[num].Value = this.StepperVM.ObjectiveEvolution_Abs[num][index];
                }
                else
                {
                    this.StepVars[num].Value = this.StepperVM.ObjectiveEvolution_Abs[num][index];
                }
                num++;
            }
        }

        // Token: 0x06000137 RID: 311 RVA: 0x00007DFC File Offset: 0x00005FFC
        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            int num = (int)this.GraphSlider.Value;
            bool flag = (double)num == this.GraphSlider.Maximum;
            if (!flag)
            {
                bool? isChecked = ((SettingsControl)this.SettingsExpander.Content).DisplayModeButton.IsChecked;
                bool flag2 = isChecked != null && isChecked.Value;
                for (int i = 0; i < this.Objectives.Count; i++)
                {
                    bool flag3 = flag2;
                    if (flag3)
                    {
                        this.Objectives[i].Value = this.StepperVM.ObjectiveEvolution_Abs[i][num];
                    }
                    else
                    {
                        this.Objectives[i].Value = this.StepperVM.ObjectiveEvolution_Norm[i][num];
                    }
                }
                for (int j = 0; j < this.Gradients.Count; j++)
                {
                    double? num2 = this.StepperVM.GradientEvolution[this.StepperVM.ObjIndex][j][num];
                    bool flag4 = num2 != null;
                    if (flag4)
                    {
                        this.Gradients[j].Value = num2.Value;
                    }
                }
                this.StepperVM.Reset();
                this.StepperVM.Reset();
                this.StepperVM.UpdateEvolutionData(new List<List<double>>(), now);
            }
        }

        // Token: 0x06000138 RID: 312 RVA: 0x00007F85 File Offset: 0x00006185
        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            this.ButtonOpenMenu.Visibility = Visibility.Visible;
            this.ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        // Token: 0x06000139 RID: 313 RVA: 0x00007FA2 File Offset: 0x000061A2
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            this.ButtonOpenMenu.Visibility = Visibility.Collapsed;
            this.ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        // Token: 0x0600013A RID: 314 RVA: 0x00007FC0 File Offset: 0x000061C0
        public void DisplayAbsolute()
        {
            this.ChartPanel.Children.Remove(this.Chart_Norm);
            this.ChartPanel.Children.Add(this.Chart_Abs);
            int num = 0;
            foreach (DataControl dataControl in this.Objectives)
            {
                dataControl.Value = this.StepperVM.ObjectiveEvolution_Abs[num].Last<double>();
                num++;
            }
        }

        // Token: 0x0600013B RID: 315 RVA: 0x00008064 File Offset: 0x00006264
        public void DisplayNormalized()
        {
            this.ChartPanel.Children.Remove(this.Chart_Abs);
            this.ChartPanel.Children.Add(this.Chart_Norm);
            int num = 0;
            foreach (DataControl dataControl in this.Objectives)
            {
                dataControl.Value = this.StepperVM.ObjectiveEvolution_Norm[num].Last<double>();
                num++;
            }
        }

        // Token: 0x0600013C RID: 316 RVA: 0x00008108 File Offset: 0x00006308
        private void MainGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool flag = e.Key == Key.Up;
            if (flag)
            {
                this.ButtonPlay_Click(this.ButtonStepUp, new RoutedEventArgs());
            }
            else
            {
                bool flag2 = e.Key == Key.Down;
                if (flag2)
                {
                    this.ButtonPlay_Click(this.ButtonStepDown, new RoutedEventArgs());
                }
                else
                {
                    bool flag3 = e.Key == Key.Left || e.Key == Key.Right;
                    if (flag3)
                    {
                        this.ButtonPlay_Click(this.ButtonStepIso, new RoutedEventArgs());
                    }
                }
            }
        }

        // Token: 0x0600013D RID: 317 RVA: 0x00008188 File Offset: 0x00006388
        private void ObjectiveData_Expanded(object sender, RoutedEventArgs e)
        {
            bool isExpanded = this.SettingsExpander.IsExpanded;
            if (isExpanded)
            {
                this.SettingsExpander.IsExpanded = false;
            }
            bool isExpanded2 = this.StepData.IsExpanded;
            if (isExpanded2)
            {
                this.StepData.IsExpanded = false;
            }
        }

        // Token: 0x0600013E RID: 318 RVA: 0x000081D2 File Offset: 0x000063D2
        private void ChosenObjective_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.StepperVM.FirePropertyChanged("CurrentObjectiveName");
        }

        // Token: 0x0600013F RID: 319 RVA: 0x000081E8 File Offset: 0x000063E8
        private void SettingsExpander_Expanded(object sender, RoutedEventArgs e)
        {
            bool isExpanded = this.ObjectiveData.IsExpanded;
            if (isExpanded)
            {
                this.ObjectiveData.IsExpanded = false;
            }
            bool isExpanded2 = this.StepData.IsExpanded;
            if (isExpanded2)
            {
                this.StepData.IsExpanded = false;
            }
        }

        // Token: 0x06000140 RID: 320 RVA: 0x00008234 File Offset: 0x00006434
        private void StepData_Expanded(object sender, RoutedEventArgs e)
        {
            bool isExpanded = this.ObjectiveData.IsExpanded;
            if (isExpanded)
            {
                this.ObjectiveData.IsExpanded = false;
            }
            bool isExpanded2 = this.SettingsExpander.IsExpanded;
            if (isExpanded2)
            {
                this.SettingsExpander.IsExpanded = false;
            }
        }

        // Token: 0x06000141 RID: 321 RVA: 0x0000827E File Offset: 0x0000647E
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            this.ExportCSVWindow.IsOpen = true;
        }

        // Token: 0x06000142 RID: 322 RVA: 0x00008290 File Offset: 0x00006490
        private void ExportCSV(object sender, DialogClosingEventArgs eventArgs)
        {
            bool flag = !object.Equals(eventArgs.Parameter, true);
            if (!flag)
            {
                string text = this.Filepath.Text;
                string text2 = this.Filename.Text;
                bool flag2 = string.IsNullOrWhiteSpace(text2);
                if (flag2)
                {
                    text2 = "Untitled";
                }
                string text3 = DateTime.Now.ToString("yyyyMMddHHmmss");
                string filename = string.Concat(new string[]
                {
                    text,
                    "/",
                    text2,
                    "_",
                    text3
                });
                this.StepperVM.ExportCSV_Log(filename);
                this.StepperVM.ExportCSV_Raw(filename);
            }
        }

        // Token: 0x06000143 RID: 323 RVA: 0x00008340 File Offset: 0x00006540
        private void Filepath_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = this.Filepath.Text;
            string path = text ?? "";
            bool isOpen = this.ExportCSVWindow.IsOpen;
            if (isOpen)
            {
                bool flag = Directory.Exists(path);
                if (flag)
                {
                    this.ExportWindowButton.IsEnabled = true;
                }
                else
                {
                    this.ExportWindowButton.IsEnabled = false;
                }
            }
        }

        //// Token: 0x06000144 RID: 324 RVA: 0x000083A0 File Offset: 0x000065A0
        //[DebuggerNonUserCode]
        //[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        //public void InitializeComponent()
        //{
        //    bool contentLoaded = this._contentLoaded;
        //    if (!contentLoaded)
        //    {
        //        this._contentLoaded = true;
        //        Uri resourceLocator = new Uri("/Stepper;component/stepperfolder/view/stepperwindow.xaml", UriKind.Relative);
        //        Application.LoadComponent(this, resourceLocator);
        //    }
        //}

        //// Token: 0x06000145 RID: 325 RVA: 0x000083D8 File Offset: 0x000065D8
        //[DebuggerNonUserCode]
        //[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        //[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        //[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        //void IComponentConnector.Connect(int connectionId, object target)
        //{
        //    switch (connectionId)
        //    {
        //        case 1:
        //            ((StepperWindow)target).SizeChanged += new SizeChangedEventHandler(this.UpdateGraphSize);
        //            ((StepperWindow)target).Closing += this.Window_Closing;
        //            break;
        //        case 2:
        //            this.MainGrid = (Grid)target;
        //            this.MainGrid.PreviewKeyDown += this.MainGrid_PreviewKeyDown;
        //            break;
        //        case 3:
        //            this.ConstraintsNotice = (DialogHost)target;
        //            break;
        //        case 4:
        //            this.IsoperformanceNotice = (DialogHost)target;
        //            break;
        //        case 5:
        //            this.LeftMenu = (Grid)target;
        //            break;
        //        case 6:
        //            this.ButtonCloseMenu = (Button)target;
        //            this.ButtonCloseMenu.Click += this.ButtonCloseMenu_Click;
        //            break;
        //        case 7:
        //            this.ButtonOpenMenu = (Button)target;
        //            this.ButtonOpenMenu.Click += this.ButtonOpenMenu_Click;
        //            break;
        //        case 8:
        //            this.VarTree = (ScrollViewer)target;
        //            break;
        //        case 9:
        //            this.VariablesExpander = (Expander)target;
        //            break;
        //        case 10:
        //            this.SlidersExpander = (Expander)target;
        //            break;
        //        case 11:
        //            this.Sliders = (StackPanel)target;
        //            break;
        //        case 12:
        //            this.GeometriesExpander = (Expander)target;
        //            break;
        //        case 13:
        //            this.Geometries = (StackPanel)target;
        //            break;
        //        case 14:
        //            this.ChartCard = (Card)target;
        //            break;
        //        case 15:
        //            this.ChartPanel = (DockPanel)target;
        //            break;
        //        case 16:
        //            this.GraphSlider = (Slider)target;
        //            this.GraphSlider.ValueChanged += this.GraphSlider_ValueChanged;
        //            break;
        //        case 17:
        //            ((Button)target).Click += this.ButtonReset_Click;
        //            break;
        //        case 18:
        //            this.RightMenus = (Grid)target;
        //            break;
        //        case 19:
        //            this.ExportCSVWindow = (DialogHost)target;
        //            this.ExportCSVWindow.DialogClosing += this.ExportCSV;
        //            break;
        //        case 20:
        //            this.Filepath = (TextBox)target;
        //            this.Filepath.TextChanged += this.Filepath_TextChanged;
        //            break;
        //        case 21:
        //            this.Filename = (TextBox)target;
        //            break;
        //        case 22:
        //            this.ExportWindowButton = (Button)target;
        //            break;
        //        case 23:
        //            this.InvalidPathWindow = (DialogHost)target;
        //            this.InvalidPathWindow.DialogClosing += this.ExportCSV;
        //            break;
        //        case 24:
        //            this.ExportButton = (Button)target;
        //            this.ExportButton.Click += this.ExportButton_Click;
        //            break;
        //        case 25:
        //            this.ObjectiveData = (Expander)target;
        //            this.ObjectiveData.Expanded += this.ObjectiveData_Expanded;
        //            break;
        //        case 26:
        //            this.ObjectivesExpander = (Expander)target;
        //            break;
        //        case 27:
        //            this.ObjData = (ItemsControl)target;
        //            break;
        //        case 28:
        //            this.GradientsExpander = (Expander)target;
        //            break;
        //        case 29:
        //            this.GradientData = (ItemsControl)target;
        //            break;
        //        case 30:
        //            this.StepData = (Expander)target;
        //            this.StepData.Expanded += this.StepData_Expanded;
        //            break;
        //        case 31:
        //            this.StepObjData = (ItemsControl)target;
        //            break;
        //        case 32:
        //            this.StepVarData = (ItemsControl)target;
        //            break;
        //        case 33:
        //            this.SettingsExpander = (Expander)target;
        //            this.SettingsExpander.Expanded += this.SettingsExpander_Expanded;
        //            break;
        //        case 34:
        //            this.ChosenObjective = (ComboBox)target;
        //            this.ChosenObjective.SelectionChanged += this.ChosenObjective_SelectionChanged;
        //            break;
        //        case 35:
        //            this.ButtonGrid = (Grid)target;
        //            break;
        //        case 36:
        //            this.ButtonStepUp = (Button)target;
        //            this.ButtonStepUp.Click += this.ButtonPlay_Click;
        //            break;
        //        case 37:
        //            this.ButtonStepIso = (Button)target;
        //            this.ButtonStepIso.Click += this.ButtonPlay_Click;
        //            break;
        //        case 38:
        //            this.ButtonStepDown = (Button)target;
        //            this.ButtonStepDown.Click += this.ButtonPlay_Click;
        //            break;
        //        case 39:
        //            this.ButtonGradient = (Button)target;
        //            this.ButtonGradient.Click += this.ButtonPlay_Click;
        //            break;
        //        default:
        //            this._contentLoaded = true;
        //            break;
        //    }
        //}

        // Token: 0x04000089 RID: 137
        private StepperVM StepperVM;

        // Token: 0x0400008A RID: 138
        private List<DataControl> Objectives;

        // Token: 0x0400008B RID: 139
        private List<VariableControl> Variables;

        // Token: 0x0400008C RID: 140
        private List<DataControl> Gradients;

        // Token: 0x0400008D RID: 141
        private List<GroupVariableControl> GroupVars;

        // Token: 0x0400008E RID: 142
        private List<DataControl> StepObjs;

        // Token: 0x0400008F RID: 143
        private List<DataControl> StepVars;

        // Token: 0x04000090 RID: 144
        private StepperGraphControl Chart_Norm;

        // Token: 0x04000091 RID: 145
        private StepperGraphControl Chart_Abs;

        // Token: 0x04000092 RID: 146
        private List<SolidColorBrush> ChartColors;


    }
}
