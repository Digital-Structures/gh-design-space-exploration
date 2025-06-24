using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Stepper
{
    // Token: 0x0200001F RID: 31
    public partial class SettingsControl : UserControl, IComponentConnector
    {
        // Token: 0x0600011B RID: 283 RVA: 0x00006C2F File Offset: 0x00004E2F
        public SettingsControl()
        {
            this.InitializeComponent();
        }

        // Token: 0x0600011C RID: 284 RVA: 0x00006C40 File Offset: 0x00004E40
        public SettingsControl(StepperVM stepper, StepperWindow window)
        {
            this.MyWindow = window;
            this.Stepper = stepper;
            this.PreviousText = "";
            base.DataContext = this.Stepper;
            this.InitializeComponent();
        }

        // Token: 0x0600011D RID: 285 RVA: 0x00006C77 File Offset: 0x00004E77
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            this.MyWindow.DisplayAbsolute();
        }

        // Token: 0x0600011E RID: 286 RVA: 0x00006C86 File Offset: 0x00004E86
        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.MyWindow.DisplayNormalized();
        }

        // Token: 0x0600011F RID: 287 RVA: 0x00006C98 File Offset: 0x00004E98
        protected void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            this.PreviousText = textBox.Text;
            textBox.Clear();
        }

        // Token: 0x06000120 RID: 288 RVA: 0x00006CC0 File Offset: 0x00004EC0
        protected void TextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            bool flag = textBox.Text == "";
            if (flag)
            {
                textBox.Text = this.PreviousText;
            }
        }

        // Token: 0x06000121 RID: 289 RVA: 0x00006CF8 File Offset: 0x00004EF8
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

        //// Token: 0x06000122 RID: 290 RVA: 0x00006D4C File Offset: 0x00004F4C
        //[DebuggerNonUserCode]
        //[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        //public void InitializeComponent()
        //{
        //    bool contentLoaded = this._contentLoaded;
        //    if (!contentLoaded)
        //    {
        //        this._contentLoaded = true;
        //        Uri resourceLocator = new Uri("/Stepper;component/stepperfolder/view/settingscontrol.xaml", UriKind.Relative);
        //        Application.LoadComponent(this, resourceLocator);
        //    }
        //}

        //// Token: 0x06000123 RID: 291 RVA: 0x00006D84 File Offset: 0x00004F84
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
        //            this.SettingsGrid = (Grid)target;
        //            break;
        //        case 2:
        //            ((TextBox)target).PreviewKeyDown += this.TextBox_PreviewKeyDown;
        //            ((TextBox)target).PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(this.TextBox_GotFocus);
        //            ((TextBox)target).PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(this.TextBox_LostFocus);
        //            break;
        //        case 3:
        //            ((TextBox)target).PreviewKeyDown += this.TextBox_PreviewKeyDown;
        //            ((TextBox)target).PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(this.TextBox_GotFocus);
        //            ((TextBox)target).PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(this.TextBox_LostFocus);
        //            break;
        //        case 4:
        //            this.DisplayModeButton = (ToggleButton)target;
        //            this.DisplayModeButton.Checked += this.ToggleButton_Checked;
        //            this.DisplayModeButton.Unchecked += this.ToggleButton_Unchecked;
        //            break;
        //        default:
        //            this._contentLoaded = true;
        //            break;
        //    }
        //}

        // Token: 0x0400007F RID: 127
        private StepperWindow MyWindow;

        // Token: 0x04000080 RID: 128
        private StepperVM Stepper;

        // Token: 0x04000081 RID: 129
        private string PreviousText;

    }
}
