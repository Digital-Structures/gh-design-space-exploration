﻿namespace DSECommon
{
    public class DSEVariable
    {
        public DSEVariable(double lb, double ub, double current)
        {
            this.LowerBound = lb;
            this.UpperBound = ub;
            this.CurrentVal = current;
        }

        public double LowerBound, UpperBound, CurrentVal;
    }
}
