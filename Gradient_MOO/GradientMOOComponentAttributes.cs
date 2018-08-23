using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using DSECommon;
using System.Drawing;
using Grasshopper.GUI.Canvas;
using System.Windows.Forms;


namespace Gradient_MOO
{
    class GradientMOOComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public GradientMOOComponent MyComponent;

        public GradientMOOComponentAttributes(IGH_Component component)
            : base(component)
        {

            MyComponent = (GradientMOOComponent)component;

            this.Gradient = new List<List<double>>();
            this.DifOne = new List<List<double>>();
            this.DifTwo = new List<List<double>>();
            this.DesignMapStepperOne = new List<List<double>>();
            this.DesignMapStepperTwo = new List<List<double>>();
            this.DesignMapStepperCombined = new List<List<double>>();
            this.ObjValsOne = new List<List<double>>();
            this.ObjValsTwo = new List<List<double>>();
            this.IsoPerf = new List<List<double>>();
            this.GradLengths = new List<double>();

            this.test = new List<List<double>>(); //JUST TO TEST
        }

        // Create variables

        public List<List<double>> Gradient;
        int numVars;
        int numObjs;
        public List<List<double>> DifOne;
        public List<List<double>> DifTwo;
        public List<List<double>> DesignMapStepperOne;
        public List<List<double>> DesignMapStepperTwo;
        public List<List<double>> DesignMapStepperCombined;
        public List<List<double>> ObjValsOne;
        public List<List<double>> ObjValsTwo;
        public List<List<double>> IsoPerf;
        double stepSizeNorm;
        public Boolean stepped;
        public double FDstep;
        public List<double> GradLengths;
        public List<double> deltaSlider;
        public List<List<double>> test; //JUST TO TEST
        public double angle;
        public double bisectLength;


        public List<List<double>> HistoryBiSteps = new List<List<double>>();
        public List<List<double>> HistoryGradSteps = new List<List<double>>();

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)

        //public override GH_ObjectResponse RespondToKeyDown(GH_Canvas sender, KeyEventArgs e)
        {

            //reset relevant lists
            this.Gradient = new List<List<double>>();
            this.DifOne = new List<List<double>>();
            this.DifTwo = new List<List<double>>();
            this.DesignMapStepperOne = new List<List<double>>();
            this.DesignMapStepperTwo = new List<List<double>>();
            this.DesignMapStepperCombined = new List<List<double>>();
            this.ObjValsOne = new List<List<double>>();
            this.ObjValsTwo = new List<List<double>>();
            this.IsoPerf = new List<List<double>>();
            this.GradLengths = new List<double>();
            this.deltaSlider = new List<double>();

            numVars = MyComponent.numVars;
            numObjs = MyComponent.numObjs;

            //Set Finite Differences step size
            FDstep = 0.01;

            angle = 0;

            double iterationlimit = 0;
            while (iterationlimit < 1000)
            {
                TakeStep();

                iterationlimit++;

                if (angle > MyComponent.threshold)
                {
                    stepped = true;
                    break;
                }
                    
            }

            return base.RespondToMouseDoubleClick(sender, e);
        }


        private void TakeStep()
        {

            // create design map stepper, which is the list of new points to be tested
            for (int i = 0; i < numVars; i++)
            {
                DesignMapStepperOne.Add(new List<double>());
                DesignMapStepperTwo.Add(new List<double>());
            }

            for (int i = 0; i < numVars; i++)
            {
                for (int j = 0; j < numVars; j++)
                {
                    DesignMapStepperOne[i].Add(MyComponent.VarsVals[j]);
                    DesignMapStepperTwo[i].Add(MyComponent.VarsVals[j]);
                }
            }

            for (int i = 0; i < numVars; i++)
            {

                double left = MyComponent.VarsVals[i] - 0.5 * FDstep * (MyComponent.MaxVals[i] - MyComponent.MinVals[i]);
                double right = MyComponent.VarsVals[i] + 0.5 * FDstep * (MyComponent.MaxVals[i] - MyComponent.MinVals[i]);

                DesignMapStepperOne[i][i] = left;
                DesignMapStepperTwo[i][i] = right;

            }

            // combine lists
            DesignMapStepperCombined.AddRange(DesignMapStepperOne);
            DesignMapStepperCombined.AddRange(DesignMapStepperTwo);

            // Add dummy at end to resent sliders
            DesignMapStepperCombined.Add(MyComponent.VarsVals);


            // run through both design maps, gather objective values on left and right for each variable
            MyComponent.ObjValues = new List<List<double>>();

            MyComponent.Iterating = true;
            this.Iterate();
            MyComponent.Iterating = false;

            for (int j = 0; j < numObjs; j++)
            {
                ObjValsOne.AddRange(MyComponent.ObjValues);
            }

            double maxObj = double.MinValue;
            double minObj = double.MaxValue;

            // find the gradient for each objective by taking finite differences of every variable

            for (int j = 0; j < numObjs; j++)
            {

                Gradient.Add(new List<double>());


                for (int i = 0; i < numVars; i++)
                {

                    double left = ObjValsOne[i][j];
                    double right = ObjValsOne[numVars + i][j];

                    double difference = (right - left) / (FDstep); //* (MyComponent.MaxVals[i] - MyComponent.MinVals[i]));

                    if (difference > maxObj) { maxObj = difference; }
                    if (difference < minObj) { minObj = difference; }

                    Gradient[j].Add((double)difference);

                    //Gradient[j].Add((double) maxObj);
                }

                //Normalize by max/min difference

                double maxAbs = double.MinValue;
                double vecLength = 0;

                if (Math.Abs(maxObj) > maxAbs) { maxAbs = Math.Abs(maxObj); }
                if (Math.Abs(minObj) > maxAbs) { maxAbs = Math.Abs(minObj); }

                for (int i = 0; i < numVars; i++)
                {
                    Gradient[j][i] = (Gradient[j][i] / maxAbs);
                    vecLength = vecLength + Gradient[j][i] * Gradient[j][i];
                }

                for (int i = 0; i < numVars; i++)
                {
                    Gradient[j][i] = (Gradient[j][i] / Math.Sqrt(vecLength));
                }

            }

            // Find length of both gradient vectors
            double sum1 = 0;
            double sum2 = 0;
            double dotProduct = 0;

            for (int i = 0; i < numVars; i++)
            {
                sum1 = sum1 + Gradient[0][i] * Gradient[0][i];
                sum2 = sum2 + Gradient[1][i] * Gradient[1][i];
                dotProduct = dotProduct + Gradient[0][i] * Gradient[1][i];
            }

            GradLengths.Add(Math.Sqrt(sum1));
            GradLengths.Add(Math.Sqrt(sum2));

            //Find angle between
            double angleRad = Math.Acos(dotProduct / (GradLengths[0] * GradLengths[1]));
            angle = angleRad * 57.2958;

            // step in the right direction based on the gradient vector

            List<IGH_Param> sliderlist = new List<IGH_Param>();

            foreach (IGH_Param src in MyComponent.Params.Input[0].Sources)
            {
                sliderlist.Add(src);
            }

            for (int i = 0; i < numVars; i++)
            {

                // Take dot product to find bisector
                double change = GradLengths[1] * Gradient[0][i] + GradLengths[0] * Gradient[1][i];

                deltaSlider.Add(change);
                double SteppedSlider = 0;

                Grasshopper.Kernel.Special.GH_NumberSlider nslider = (Grasshopper.Kernel.Special.GH_NumberSlider)sliderlist[i];


                // First two modes keep advancing both; second two modes, follow one gradient or the other
                if (MyComponent.Direction > 0 && !MyComponent.Mode)
                {
                    SteppedSlider = MyComponent.VarsVals[i] + deltaSlider[i] * MyComponent.StepSize * (MyComponent.MaxVals[i] - MyComponent.MinVals[i]);
                }

                if (MyComponent.Direction < 0 && !MyComponent.Mode)
                {
                    SteppedSlider = MyComponent.VarsVals[i] - deltaSlider[i] * MyComponent.StepSize * (MyComponent.MaxVals[i] - MyComponent.MinVals[i]);
                }

                if (MyComponent.Direction > 0 && MyComponent.Mode)
                {
                    SteppedSlider = MyComponent.VarsVals[i] + Gradient[0][i] * MyComponent.StepSize * (MyComponent.MaxVals[i] - MyComponent.MinVals[i]);
                }

                if (MyComponent.Direction < 0 && MyComponent.Mode)
                {
                    SteppedSlider = MyComponent.VarsVals[i] + Gradient[1][i] * MyComponent.StepSize * (MyComponent.MaxVals[i] - MyComponent.MinVals[i]);
                }

                nslider.TrySetSliderValue((decimal)SteppedSlider);

            }

            // Send to history, depending on mode

            List<double> VarsObjsCurrent = new List<double>();
            VarsObjsCurrent.AddRange(MyComponent.VarsVals);
            VarsObjsCurrent.AddRange(MyComponent.ObjInput);

            if (!MyComponent.Mode)
            {
                HistoryBiSteps.Add(VarsObjsCurrent);
            } else { HistoryGradSteps.Add(VarsObjsCurrent); }

            double sum3 = 0;
            //Calculate length of bisector
            for (int i = 0; i < numVars; i++)
            {
                sum3 += deltaSlider[i] * deltaSlider[i];
            }

            bisectLength = Math.Sqrt(sum3);

            //stepped = true;

            // Do not recompute UI--------------------------
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
            
            //

            //return base.RespondToKeyDown(sender, e);


        }

        public static IEnumerable<double> StringToDoubleList(string str)
        {
            if (String.IsNullOrEmpty(str))
                yield break;

            foreach (var s in str.Split(' '))
            {
                double num;
                if (double.TryParse(s, out num))
                    yield return num;
            }
        }

        private void Iterate()
        {
            int i = 1;

            foreach (List<double> sample in this.DesignMapStepperCombined)
            {
                GHUtilities.ChangeSliders(MyComponent.SlidersList, sample);

                // Reset last slider
                if (i == this.DesignMapStepperCombined.Count)
                {

                }

                i++;
            }

        }

    }

}
