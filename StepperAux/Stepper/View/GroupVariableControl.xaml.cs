using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using MaterialDesignThemes.Wpf;

namespace Stepper
{
    // Token: 0x02000014 RID: 20
    public partial class GroupVariableControl : BaseControl, IComponentConnector
    {
        // Token: 0x060000AA RID: 170 RVA: 0x000039FA File Offset: 0x00001BFA
        public GroupVariableControl()
        {
            this.InitializeComponent();
        }

        // Token: 0x060000AB RID: 171 RVA: 0x00003A0B File Offset: 0x00001C0B
        public GroupVariableControl(GroupVarVM gvarvm) : base(gvarvm)
        {
            this.InitializeComponent();
        }

        // Token: 0x060000AC RID: 172 RVA: 0x00003A20 File Offset: 0x00001C20
        protected void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (VarVM varVM in ((GroupVarVM)base.MyVM).MyVars)
            {
                varVM.IsActive = true;
            }
        }

        // Token: 0x060000AD RID: 173 RVA: 0x00003A84 File Offset: 0x00001C84
        protected void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (VarVM varVM in ((GroupVarVM)base.MyVM).MyVars)
            {
                varVM.IsActive = false;
            }
        }

        // Token: 0x060000AE RID: 174 RVA: 0x00003AE8 File Offset: 0x00001CE8
        public void OptimizationStarted()
        {
            ((GroupVarVM)base.MyVM).OptimizationStarted();
        }

        // Token: 0x060000AF RID: 175 RVA: 0x00003AFC File Offset: 0x00001CFC
        public void OptimizationFinished()
        {
            ((GroupVarVM)base.MyVM).OptimizationFinished();
        }

        //// Token: 0x060000B0 RID: 176 RVA: 0x00003B10 File Offset: 0x00001D10
        //[DebuggerNonUserCode]
        //[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        //public void InitializeComponent()
        //{
        //    bool contentLoaded = this._contentLoaded;
        //    if (!contentLoaded)
        //    {
        //        this._contentLoaded = true;
        //        Uri resourceLocator = new Uri("/Stepper;component/stepperfolder/dsview/groupvariablecontrol.xaml", UriKind.Relative);
        //        Application.LoadComponent(this, resourceLocator);
        //    }
        //}

        //// Token: 0x060000B1 RID: 177 RVA: 0x00003B48 File Offset: 0x00001D48
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
        //            this.BoundsErrorBox = (DialogHost)target;
        //            break;
        //        case 2:
        //            this.GroupControlName = (TextBlock)target;
        //            break;
        //        case 3:
        //            this.ValueText = (TextBox)target;
        //            this.ValueText.TextChanged += base.TextBox_TextChanged;
        //            this.ValueText.PreviewTextInput += base.TextBox_PreviewTextInput_Float;
        //            this.ValueText.PreviewMouseDown += base.TextBox_PreviewMouseDown;
        //            this.ValueText.PreviewKeyDown += base.TextBox_PreviewKeyDown;
        //            this.ValueText.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(base.TextBox_GotFocus);
        //            this.ValueText.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(base.TextBox_LostFocus);
        //            break;
        //        case 4:
        //            this.Checkbox = (CheckBox)target;
        //            this.Checkbox.Checked += this.CheckBox_Checked;
        //            this.Checkbox.Unchecked += this.CheckBox_Unchecked;
        //            break;
        //        case 5:
        //            this.MinText = (TextBox)target;
        //            this.MinText.TextChanged += base.TextBox_TextChanged;
        //            this.MinText.PreviewTextInput += base.TextBox_PreviewTextInput_Float;
        //            this.MinText.PreviewMouseDown += base.TextBox_PreviewMouseDown;
        //            this.MinText.PreviewKeyDown += base.TextBox_PreviewKeyDown;
        //            this.MinText.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(base.TextBox_GotFocus);
        //            this.MinText.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(base.TextBox_LostFocus);
        //            break;
        //        case 6:
        //            this.Slider = (Slider)target;
        //            break;
        //        case 7:
        //            this.MaxText = (TextBox)target;
        //            this.MaxText.TextChanged += base.TextBox_TextChanged;
        //            this.MaxText.PreviewTextInput += base.TextBox_PreviewTextInput_Float;
        //            this.MaxText.PreviewMouseDown += base.TextBox_PreviewMouseDown;
        //            this.MaxText.PreviewKeyDown += base.TextBox_PreviewKeyDown;
        //            this.MaxText.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(base.TextBox_GotFocus);
        //            this.MaxText.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(base.TextBox_LostFocus);
        //            break;
        //        default:
        //            this._contentLoaded = true;
        //            break;
        //    }
        //}

       
        // Token: 0x0200002C RID: 44
        public enum Direction
        {
            // Token: 0x040000DF RID: 223
            X,
            // Token: 0x040000E0 RID: 224
            Y,
            // Token: 0x040000E1 RID: 225
            Z
        }
    }
}
