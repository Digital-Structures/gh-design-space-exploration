using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using LiveCharts.Wpf;

namespace Stepper
{
    // Token: 0x02000020 RID: 32
    public partial class StepperGraphControl : UserControl, IComponentConnector
    {
        // Token: 0x06000124 RID: 292 RVA: 0x00006E9E File Offset: 0x0000509E
        public StepperGraphControl()
        {
            this.InitializeComponent();
        }

        // Token: 0x06000125 RID: 293 RVA: 0x00006EB0 File Offset: 0x000050B0
        public StepperGraphControl(StepperGraphVM VM)
        {
            base.DataContext = VM;
            this.InitializeComponent();
            this.XAxis.LabelFormatter = ((double val) => val.ToString("n2"));
            this.YAxis.LabelFormatter = ((double val) => val.ToString("n2"));
        }

        // Token: 0x06000126 RID: 294 RVA: 0x00006F2A File Offset: 0x0000512A
        public void ForceGraphUpdate()
        {
            this.Graph.Update(true, true);
        }

        //// Token: 0x06000127 RID: 295 RVA: 0x00006F3C File Offset: 0x0000513C
        //[DebuggerNonUserCode]
        //[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        //public void InitializeComponent()
        //{
        //    bool contentLoaded = this._contentLoaded;
        //    if (!contentLoaded)
        //    {
        //        this._contentLoaded = true;
        //        Uri resourceLocator = new Uri("/Stepper;component/stepperfolder/view/steppergraphcontrol.xaml", UriKind.Relative);
        //        Application.LoadComponent(this, resourceLocator);
        //    }
        //}

        //// Token: 0x06000128 RID: 296 RVA: 0x00006F74 File Offset: 0x00005174
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
        //            this.Graph = (CartesianChart)target;
        //            break;
        //        case 2:
        //            this.YAxis = (Axis)target;
        //            break;
        //        case 3:
        //            this.XAxis = (Axis)target;
        //            break;
        //        default:
        //            this._contentLoaded = true;
        //            break;
        //    }
        //}


  
    }
}
