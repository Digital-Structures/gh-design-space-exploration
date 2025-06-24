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
    public partial class SetupParamControl : UserControl
    {
        public SetupParamControl(int number, IRuleParameter rp)
        {
            InitializeComponent();
            this.Model = rp;
            this.Setup(number);
            //this.AddControl();
        }

        private IRuleParameter Model
        {
            get;
            set;
        }

        private void Setup(int number)
        {
            string zero = number < 10 ? "0" : "";
            this.ParamName.Text = "[ p" + number.ToString() + " ]:";
            this.ParamDesc.Text = Model.Name;
        }

        private void AddControl()
        {
            if (this.Model as DoubleParameter != null)
            {
                ParamControl.Child = new DoubParamControl();
            }
            else if (this.Model as IntParameter != null)
            {
                ParamControl.Child = new IntParamControl(this, (IntParameter)this.Model);
            }
            else if (this.Model as EnumParameter != null)
            {
                ParamControl.Child = new EnumParamControl(this, (EnumParameter)this.Model);
            }
        }
    }
}
