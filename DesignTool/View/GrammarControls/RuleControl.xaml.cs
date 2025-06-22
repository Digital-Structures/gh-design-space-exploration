using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using StructureEngine.Grammar;
using System.Windows.Data;
using DesignTool.DesignModes;

namespace DesignTool.View.GrammarControls
{
    public partial class RuleControl : UserControl
    {
        public RuleControl(IRule r)
        {
            InitializeComponent();

            this.MyRule = r;
            this.SetupControl();
        }

        public IRule MyRule
        {
            get;
            private set;
        }

        public object[] GetParams()
        {
            int num = MyRule.Params.Count;
            object[] p = new object[num];

            int i = 0;
            foreach (UIElement biggrid in ParamStack.Children)
            {
                if (biggrid is Grid)
                {
                    foreach (UIElement smallgrid in ((Grid)biggrid).Children)
                    {
                        if (smallgrid is Grid)
                        {
                            if (MyRule.Params[i] is DoubleParameter)
                            {
                                foreach (Slider slider in ((Grid)smallgrid).Children)
                                {
                                    if (slider is Slider)
                                    {
                                        p[i] = slider.Value;
                                        break;
                                    }
                                }
                            }
                            else if (MyRule.Params[i] is IntParameter)
                            {
                                foreach (UIElement slider in ((Grid)smallgrid).Children)
                                {
                                    if (slider is Slider)
                                    {
                                        p[i] = Convert.ToInt32(Math.Round(((Slider)slider).Value));
                                        break;
                                    }
                                }
                            }
                            else if (MyRule.Params[i] is EnumParameter)
                            {
                                foreach (UIElement rb in ((Grid)smallgrid).Children)
                                {
                                    if (rb is RadioButton)
                                    {
                                        if (((RadioButton)rb).IsChecked == true)
                                        {
                                            int row = Grid.GetRow((RadioButton)rb);
                                            UIElement text = ((Grid)smallgrid).Children.Where(w => w.GetValue(Grid.RowProperty).Equals(row) 
                                                && w.GetValue(Grid.ColumnProperty).Equals(1)).FirstOrDefault();
                                            if (text is TextBlock)
                                            {
                                                p[i] = ((TextBlock)text).Text;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            i++;
                        }
                    }
                }
            }

            return p;
        }

        private void SetupControl()
        {
            RuleName.Text = MyRule.Name;
            HookUpButton();
            AddParBoxes();
        }

        private void HookUpButton()
        {
            ApplyButton.Content = "Apply Rule";
        }

        private void AddParBoxes()
        {
            foreach (IRuleParameter p in MyRule.Params)
            {
                ParamStack.Children.Add(GetParBox(p));
            }
        }

        private Grid GetParBox(IRuleParameter p)
        {
            Grid g = new Grid();
            //g.Padding = new Thickness(0, 5, 0, 5);
            g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            g.RowDefinitions.Add(new RowDefinition());

            TextBlock name = new TextBlock();
            name.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            name.TextWrapping = TextWrapping.Wrap;
            name.Text = p.Name;
            g.Children.Add(name);
            Grid.SetRow(name, 0);

            if (p is DoubleParameter)
            {
                Grid doublegrid = this.GetDoubleSlider((DoubleParameter)p);
                g.Children.Add(doublegrid);
                Grid.SetRow(doublegrid, 1);
            }

            else if (p is IntParameter)
            {
                Grid intgrid = this.GetIntSlider((IntParameter)p);
                g.Children.Add(intgrid);
                Grid.SetRow(intgrid, 1);
            }

            else if (p is EnumParameter)
            {
                Grid enumgrid = this.GetEnumRadios((EnumParameter)p);
                g.Children.Add(enumgrid);
                Grid.SetRow(enumgrid, 1);
            }

            return g;
        }

        private Grid GetDoubleSlider(DoubleParameter d)
        {
            Grid g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });

            Slider s = new Slider();
            s.Minimum = d.Min;
            s.Maximum = d.Max;
            s.Value = s.Minimum + (s.Maximum - s.Minimum) / 2.0;
            g.Children.Add(s);
            Grid.SetColumn(s, 0);

            TextBlock t = new TextBlock();
            t.Margin = new Thickness(5, 0, 0, 0);
            t.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Binding b = new Binding();
            b.Source = s;
            b.Path = new PropertyPath("Value");
            b.Converter = new StringFormatConverter();
            b.ConverterParameter = "{0:0.0}";
            t.SetBinding(TextBlock.TextProperty, b);
            g.Children.Add(t);
            Grid.SetColumn(t, 1);

            return g;
        }

        private Grid GetIntSlider(IntParameter i)
        {
            Grid g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });

            Slider s = new Slider();
            s.Minimum = i.Min;
            s.Maximum = i.Max;
            s.Value = Math.Round(s.Minimum + (s.Maximum - s.Minimum) / 2);
            g.Children.Add(s);
            Grid.SetColumn(s, 0);

            TextBlock t = new TextBlock();
            t.Margin = new Thickness(5, 0, 0, 0);
            t.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Binding b = new Binding();
            b.Source = s;
            b.Path = new PropertyPath("Value");
            b.Converter = new StringFormatConverter();
            b.ConverterParameter = "{0:0}";
            t.SetBinding(TextBlock.TextProperty, b);
            g.Children.Add(t);
            Grid.SetColumn(t, 1);

            return g;
        }

        private Grid GetEnumRadios(EnumParameter e)
        {
            Grid g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            foreach (string s in e.Enums)
            {
                g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20) });
                
                RadioButton b = new RadioButton();
                b.GroupName = e.Name + " Group";
                b.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                if (e.Enums.IndexOf(s) == 0)
                {
                    b.IsChecked = true;
                }
                g.Children.Add(b);
                Grid.SetRow(b, e.Enums.IndexOf(s));
                Grid.SetColumn(b, 0);

                TextBlock t = new TextBlock();
                t.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                t.Text = s;
                g.Children.Add(t);
                Grid.SetRow(t, e.Enums.IndexOf(s));
                Grid.SetColumn(t, 1);
            }

            return g;
        }

        public void Disable()
        {
        }

        public void Hide()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void Show()
        {
            this.Visibility = System.Windows.Visibility.Visible;
        }

    }
}
