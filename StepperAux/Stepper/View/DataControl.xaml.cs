using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Stepper
{
    // Token: 0x0200001E RID: 30
    public partial class DataControl : UserControl, IComponentConnector
    {
        // Token: 0x06000112 RID: 274 RVA: 0x00006A0C File Offset: 0x00004C0C
        public DataControl()
        {
            this.InitializeComponent();
        }

        // Token: 0x06000113 RID: 275 RVA: 0x00006A20 File Offset: 0x00004C20
        public DataControl(IStepDataElement data)
        {
            this.MyData = data;
            this.MyData.PropertyChanged += this.VarPropertyChanged;
            base.DataContext = this;
            this.InitializeComponent();
            this.Value = this.MyData.Value;
            this.VariableName = this.MyData.Name;
        }

        // Token: 0x06000114 RID: 276 RVA: 0x00006A87 File Offset: 0x00004C87
        private void VarPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.VariableName = this.MyData.Name;
        }

        // Token: 0x1700005E RID: 94
        // (get) Token: 0x06000115 RID: 277 RVA: 0x00006A9C File Offset: 0x00004C9C
        // (set) Token: 0x06000116 RID: 278 RVA: 0x00006AB4 File Offset: 0x00004CB4
        public double Value
        {
            get
            {
                return this.val;
            }
            set
            {
                bool flag = value != this.val;
                if (flag)
                {
                    this.val = value;
                    bool flag2 = this.val > 99999.0;
                    if (flag2)
                    {
                        this.ValueText.Text = string.Format("{0:0.0e0}", this.val);
                    }
                    else
                    {
                        this.ValueText.Text = string.Format("{0:0.00}", this.val);
                    }
                }
            }
        }

        // Token: 0x1700005F RID: 95
        // (get) Token: 0x06000117 RID: 279 RVA: 0x00006B38 File Offset: 0x00004D38
        // (set) Token: 0x06000118 RID: 280 RVA: 0x00006B50 File Offset: 0x00004D50
        public string VariableName
        {
            get
            {
                return this.name;
            }
            set
            {
                bool flag = value != this.name;
                if (flag)
                {
                    this.name = value;
                    this.VarName.Text = this.name;
                }
                bool flag2 = this.VariableName != this.MyData.Name;
                if (flag2)
                {
                    this.MyData.Name = this.VariableName;
                }
            }
        }

        //// Token: 0x06000119 RID: 281 RVA: 0x00006BB8 File Offset: 0x00004DB8
        //[DebuggerNonUserCode]
        //[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        //public void InitializeComponent()
        //{
        //    bool contentLoaded = this._contentLoaded;
        //    if (!contentLoaded)
        //    {
        //        this._contentLoaded = true;
        //        Uri resourceLocator = new Uri("/Stepper;component/stepperfolder/view/datacontrol.xaml", UriKind.Relative);
        //        Application.LoadComponent(this, resourceLocator);
        //    }
        //}

        //// Token: 0x0600011A RID: 282 RVA: 0x00006BF0 File Offset: 0x00004DF0
        //[DebuggerNonUserCode]
        //[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        //[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        //[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        //void IComponentConnector.Connect(int connectionId, object target)
        //{
        //    if (connectionId != 1)
        //    {
        //        if (connectionId != 2)
        //        {
        //            this._contentLoaded = true;
        //        }
        //        else
        //        {
        //            this.ValueText = (TextBlock)target;
        //        }
        //    }
        //    else
        //    {
        //        this.VarName = (TextBox)target;
        //    }
        //}

        // Token: 0x04000079 RID: 121
        private IStepDataElement MyData;

        // Token: 0x0400007A RID: 122
        private double val;

        // Token: 0x0400007B RID: 123
        private string name;
    }
}
