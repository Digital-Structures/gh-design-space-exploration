using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Stepper
{
    // Token: 0x02000015 RID: 21
    public partial class VariableControl : BaseControl, IComponentConnector
    {
        // Token: 0x060000B2 RID: 178 RVA: 0x00003DDC File Offset: 0x00001FDC
        public VariableControl()
        {
            this.InitializeComponent();
        }

        // Token: 0x060000B3 RID: 179 RVA: 0x00003DED File Offset: 0x00001FED
        public VariableControl(VarVM varvm) : base(varvm)
        {
            this.InitializeComponent();
        }

        //// Token: 0x060000B4 RID: 180 RVA: 0x00003E00 File Offset: 0x00002000
        //[DebuggerNonUserCode]
        //[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        //public void InitializeComponent()
        //{
        //    bool contentLoaded = this._contentLoaded;
        //    if (!contentLoaded)
        //    {
        //        this._contentLoaded = true;
        //        Uri resourceLocator = new Uri("/Stepper;component/stepperfolder/dsview/variablecontrol.xaml", UriKind.Relative);
        //        Application.LoadComponent(this, resourceLocator);
        //    }
        //}

        // Token: 0x060000B5 RID: 181 RVA: 0x00003E38 File Offset: 0x00002038
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
        //            this.VarName = (TextBox)target;
        //            this.VarName.PreviewKeyDown += base.TextBox_PreviewKeyDown;
        //            break;
        //        case 2:
        //            this.ValueText = (TextBox)target;
        //            this.ValueText.TextChanged += base.TextBox_TextChanged;
        //            this.ValueText.PreviewTextInput += base.TextBox_PreviewTextInput_Float;
        //            this.ValueText.PreviewMouseDown += base.TextBox_PreviewMouseDown;
        //            this.ValueText.PreviewKeyDown += base.TextBox_PreviewKeyDown;
        //            this.ValueText.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(base.TextBox_GotFocus);
        //            this.ValueText.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(base.TextBox_LostFocus);
        //            break;
        //        case 3:
        //            this.Checkbox = (CheckBox)target;
        //            break;
        //        case 4:
        //            this.MinText = (TextBox)target;
        //            this.MinText.TextChanged += base.TextBox_TextChanged;
        //            this.MinText.PreviewTextInput += base.TextBox_PreviewTextInput_Float;
        //            this.MinText.PreviewMouseDown += base.TextBox_PreviewMouseDown;
        //            this.MinText.PreviewKeyDown += base.TextBox_PreviewKeyDown;
        //            this.MinText.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(base.TextBox_GotFocus);
        //            this.MinText.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(base.TextBox_LostFocus);
        //            break;
        //        case 5:
        //            this.Slider = (Slider)target;
        //            break;
        //        case 6:
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
    }
}
