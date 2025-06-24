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
using System.Collections.Generic;
using StructureEngine.Model;

namespace DesignTool.View.StaticStructure
{
    public class GenerateBehavior : IModeBehavior
    {
        public GenerateBehavior(DesignControl dc)
        {
            this.MyParent = dc;
            this.ScoreVis = true;
        }

        private DesignControl MyParent;

        public bool ScoreVis
        {
            get;
            set;
        }

        public void DoClick()
        {
            if (MyParent.Clickable)
            {
                MyParent.MyParent.SetSelectedControl(MyParent);
                if (MyParent.Selected)
                {
                    MyParent.HoverPath.Stroke = new SolidColorBrush(Color.FromArgb(255, 190, 190, 190)); // 255, 127, 36
                }
                else
                {
                    MyParent.HoverPath.Stroke = new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        public void DoHover()
        {
            if (MyParent.Clickable)
            {
                MyParent.HoverPath.Stroke = new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));
                MyParent.ViewModel.SelectVis = true;
                MyParent.ViewModel.SetBaseVis = true;
            }
            
        }

        public void DoLeaveHover()
        {
            if (MyParent.Clickable && !MyParent.Selected)
            {
                MyParent.HoverPath.Stroke = new SolidColorBrush(Colors.Transparent);
            }
            else if (MyParent.Selected)
            {
                MyParent.HoverPath.Stroke = new SolidColorBrush(Color.FromArgb(255, 190, 190, 190));
            }
            MyParent.ViewModel.SelectVis = false;
            MyParent.ViewModel.SetBaseVis = false;
        }
    }
}
