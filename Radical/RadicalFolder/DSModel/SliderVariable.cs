using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.GUI;

namespace DSOptimization
{
    //SLIDER VARIABLE
    //Class for manipulation of numeric slider variables
    public class SliderVariable : IVariable
    {
        public enum Direction { X, Y, Z, None };

        //CONSTRUCTOR
        public SliderVariable(IGH_Param param)
        {
            this.param = param;
            this.Slider = this.Parameter as GH_NumberSlider;
            this._dir = Direction.None;
        }
        public GH_NumberSlider Slider;

        //IS ACTIVE
        //Determines whether variable should be considered in optimization
        public bool IsActive
        {
            get; set;
        }

        //DIRECTION
        private Direction _dir;
        public int Dir
        {
            get { return (int)this._dir; }
            set { this._dir = (Direction)value; }
        }

        //MAX
        //Maximum displacement of the control point from its original state
        public double Min
        {
            get { return (double)Slider.Slider.Minimum; }
            set { this.Slider.Slider.Minimum = (decimal)value; }
        }

        //MIN
        //Minimum displacement of the control point from its original state
        public double Max
        {
            get { return (double)Slider.Slider.Maximum; }
            set { this.Slider.Slider.Maximum = (decimal)value; }
        }

        //CURRENT VALUE
        //Position of the control point relative to its starting position
        public double ReferenceValue
        {
            get;
            set;
        }

        //CURRENT VALUE
        //Position of the control point relative to its starting position
        public double CurrentValue
        {
            get { return (double)Slider.CurrentValue; }
            set { this.UpdateValue(value); }
        }

        //PARAMETER
        //Determines to what type of grasshopper object the variable belongs
        private IGH_Param param;
        public IGH_Param Parameter
        {
            get { return this.param; }
        }

        //UPDATE VALUE
        //Change variable's value and update its corresponding slider
        public void UpdateValue(double x)
        {
            try
            {
                Slider.SetSliderValue((decimal)x);
            }
            catch
            {
            }
        }

        public double Gradient()
        {
            throw new NotImplementedException();
        }
    }
}
