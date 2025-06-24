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

namespace DesignTool.View
{
    public interface IModeBehavior
    {
        void DoClick();
        void DoHover();
        void DoLeaveHover();
        bool ScoreVis
        {
            get;
        }
    }
}
