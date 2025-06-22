using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DesignTool.ViewModel;

namespace DesignTool.View.InteractiveStructure
{
    public interface IControl
    {
        ControlVM ControlViewModel
        {
            get;
        }
    }
}
