using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Stepper
{
    // Token: 0x02000013 RID: 19
    public class BaseControl : UserControl
    {
        // Token: 0x1700003B RID: 59
        // (get) Token: 0x0600009C RID: 156 RVA: 0x000036F0 File Offset: 0x000018F0
        // (set) Token: 0x0600009D RID: 157 RVA: 0x000036F8 File Offset: 0x000018F8
        public IViewModel MyVM { get; set; }

        // Token: 0x0600009E RID: 158 RVA: 0x00003701 File Offset: 0x00001901
        public BaseControl()
        {
            base.DataContext = this.MyVM;
        }

        // Token: 0x0600009F RID: 159 RVA: 0x00003718 File Offset: 0x00001918
        public BaseControl(IViewModel VM)
        {
            this.MyVM = VM;
            base.DataContext = this.MyVM;
            this.PreviousText = "";
        }

        // Token: 0x060000A0 RID: 160 RVA: 0x00003744 File Offset: 0x00001944
        protected void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            bool changesEnabled = this.MyVM.ChangesEnabled;
            if (changesEnabled)
            {
                textBox.IsReadOnly = false;
            }
            else
            {
                textBox.IsReadOnly = true;
            }
        }

        // Token: 0x060000A1 RID: 161 RVA: 0x0000377C File Offset: 0x0000197C
        protected Size MeasureString(string candidate, TextBox box)
        {
            FormattedText formattedText = new FormattedText(candidate, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(box.FontFamily, box.FontStyle, box.FontWeight, box.FontStretch), box.FontSize, Brushes.Black);
            return new Size(formattedText.Width, formattedText.Height);
        }

        // Token: 0x060000A2 RID: 162 RVA: 0x000037D4 File Offset: 0x000019D4
        protected void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            bool flag = textBox.Text != "";
            if (flag)
            {
                textBox.Width = this.MeasureString(textBox.Text, textBox).Width;
            }
        }

        // Token: 0x060000A3 RID: 163 RVA: 0x0000381C File Offset: 0x00001A1C
        protected void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            bool changesEnabled = this.MyVM.ChangesEnabled;
            if (changesEnabled)
            {
                TextBox textBox = (TextBox)sender;
                this.PreviousText = textBox.Text;
                textBox.Clear();
            }
        }

        // Token: 0x060000A4 RID: 164 RVA: 0x00003858 File Offset: 0x00001A58
        protected void TextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            bool flag = textBox.Text == "";
            if (flag)
            {
                textBox.Text = this.PreviousText;
            }
        }

        // Token: 0x060000A5 RID: 165 RVA: 0x00003890 File Offset: 0x00001A90
        protected void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool flag = e.Key == Key.Return;
            if (flag)
            {
                Keyboard.ClearFocus();
                this.TextBox_LostFocus(sender, e);
                TextBox target = (TextBox)sender;
                DependencyProperty textProperty = TextBox.TextProperty;
                BindingExpression bindingExpression = BindingOperations.GetBindingExpression(target, textProperty);
                bool flag2 = bindingExpression != null;
                if (flag2)
                {
                    bindingExpression.UpdateSource();
                }
            }
        }

        // Token: 0x060000A6 RID: 166 RVA: 0x000038E4 File Offset: 0x00001AE4
        protected void TextBox_PreviewTextInput_Float(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            bool flag = textBox.Text == "" && e.Text == "-";
            if (!flag)
            {
                e.Handled = !BaseControl.IsTextAllowedFloat(textBox.Text + e.Text);
            }
        }

        // Token: 0x060000A7 RID: 167 RVA: 0x00003948 File Offset: 0x00001B48
        protected static bool IsTextAllowedFloat(string text)
        {
            double num = 0.0;
            return double.TryParse(text, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out num);
        }

        // Token: 0x060000A8 RID: 168 RVA: 0x00003974 File Offset: 0x00001B74
        protected void TextBox_PreviewTextInput_Int(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            bool flag = textBox.Text == "" && e.Text == "-";
            if (!flag)
            {
                e.Handled = !BaseControl.IsTextAllowedInt(textBox.Text + e.Text);
            }
        }

        // Token: 0x060000A9 RID: 169 RVA: 0x000039D8 File Offset: 0x00001BD8
        protected static bool IsTextAllowedInt(string text)
        {
            int num = 0;
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out num);
        }

        // Token: 0x04000034 RID: 52
        private string PreviousText;
    }
}
