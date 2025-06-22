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

namespace DesignTool.View.GrammarControls
{
    public partial class EnumParamControl : UserControl
    {
        public EnumParamControl(SetupParamControl p, EnumParameter rp)
        {
            InitializeComponent();
            this.MyParent = p;
            this.Model = rp;
            this.DrawControls();
        }

        private SetupParamControl MyParent;
        private EnumParameter Model;

        private void DrawControls()
        {
            foreach (string s in Model.Enums)
            {
                this.AddControl(s);
            }
        }

        private void AddControl(string s)
        {
            StackPanel p = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 2), 
                VerticalAlignment = System.Windows.VerticalAlignment.Center };
            CheckBox c = new CheckBox() { Name = s, IsChecked = true };
            c.Checked += new RoutedEventHandler(c_Checked);
            c.Unchecked += new RoutedEventHandler(c_Unchecked);
            TextBlock t = new TextBlock() { Text = s, Margin = new Thickness(5, 0, 0, 0), 
                VerticalAlignment = System.Windows.VerticalAlignment.Center, };
            p.Children.Add(c);
            p.Children.Add(t);

            LayoutRoot.Children.Add(p);
        }

        private void c_Checked(object sender, RoutedEventArgs e)
        {
            object n = this.GetName(sender);
            if (this.GetName(sender) != null)
            {
                string name = (string)n;
                if (!Model.Enums.Contains(name))
                {
                    Model.Enums.Add(name);
                }
            }
        }

        private void c_Unchecked(object sender, RoutedEventArgs e)
        {
            object n = this.GetName(sender);
            if (this.GetName(sender) != null)
            {
                string name = (string)n;
                if (Model.Enums.Contains(name))
                {
                    Model.Enums.Remove(name);
                }
            }
        }

        private string GetName(object sender)
        {
            CheckBox c = sender as CheckBox;
            if (c != null)
            {
                StackPanel p = c.Parent as StackPanel;
                if (p != null)
                {
                    StackPanel lr = p.Parent as StackPanel;
                    if (lr != null)
                    {
                        TextBlock n = p.Children[1] as TextBlock;
                        if (n != null)
                        {
                            string name = n.Text;
                            return name;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
