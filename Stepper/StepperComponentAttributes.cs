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
using MathNet.Numerics;

namespace Stepper
{
    class StepperComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public StepperComponent MyComponent;

        public StepperComponentAttributes(IGH_Component component)
            : base(component)
        {


                MyComponent = (StepperComponent)component;

            this.Gradient = new List<List<double>>();
            this.DifOne = new List<List<double>>();
            this.DifTwo = new List<List<double>>();
            this.DesignMapStepperOne = new List<List<double>>();
            this.DesignMapStepperTwo = new List<List<double>>();
            this.DesignMapStepperCombined = new List<List<double>>();
            this.ObjValsOne = new List<List<double>>();
            this.ObjValsTwo = new List<List<double>>();
            this.IsoPerf = new List<double>();


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
        public List<double> IsoPerf;
        double stepSizeNorm;
        
       

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




            numVars = MyComponent.numVars;
                numObjs = MyComponent.numObjs;


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

                    double left = MyComponent.VarsVals[i] - 0.5 * MyComponent.StepSize * (MyComponent.MaxVals[i] - MyComponent.MinVals[i]);
                    double right = MyComponent.VarsVals[i] + 0.5 * MyComponent.StepSize * (MyComponent.MaxVals[i] - MyComponent.MinVals[i]);

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

                        double difference = (right - left) / (MyComponent.StepSize * (MyComponent.MaxVals[i] - MyComponent.MinVals[i]));

                            if (difference > maxObj) { maxObj = difference; }
                            if (difference < minObj) { minObj = difference; }

                        Gradient[j].Add((double) difference);

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

            //// FIND THE ORTHOGONAL VECTORS

            ////double[][] gradientArray = Gradient.Select(a => a.ToArray()).ToArray();

            List<List<string>> lst = new List<List<string>>();
            double[,] gradientArray = new double[Gradient.Count, Gradient[0].Count];

            for (int j = 0; j < Gradient.Count; j++)
            {
                for (int i = 0; i < Gradient[j].Count; i++)
                {
                    gradientArray[j, i] = Gradient[j][i];
                }
            }

            var matrixGrad = MathNet.Numerics.LinearAlgebra.Double.DenseMatrix.OfArray(gradientArray);

            var matrixGradT = matrixGrad.Transpose();
            var nullspace = matrixGradT.Kernel();


            // STRATEGY 1: Direct to Array

            double[] nullspaceArray;

            for (int i = 1; i < 2; i++)
            {
                nullspaceArray = nullspace[i].ToArray();
            }

            // STRATEGY 2: One Line to Array
            //double[] IsoPerfDir = nullspace.ToArray()[1].ToArray();

            double[] IsoPerfDir2 = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            //STRATEGY 3: STRING IN THE MIDDLE
            //string IsoPerfDir = nullspace.ToArray()[0].ToString();

            //List<double> IsoPerfDir3 = (List<double>)StringToDoubleList(IsoPerfDir);


            //var IsoPerfMatrix = nullspace;

            //IsoPerf.Add(IsoPerfMatrix[1]);

            //// Randomly assign coefficient, create isoperformance direction
            //Random rnd = new Random();
            //int isoDir = rnd.Next(0, numVars - 1);




            //// Convert array to List


            for (int i = 0; i < numVars; i++)
            {
                IsoPerf.Add(IsoPerfDir2[i]);
            }




            // step in the right direction based on the gradient vector

            List<IGH_Param> sliderlist = new List<IGH_Param>();

                foreach (IGH_Param src in MyComponent.Params.Input[0].Sources)
                {
                    sliderlist.Add(src);
                }

                for (int i = 0; i < numVars; i++)
                {

                    if (MyComponent.Direction > 0)
                    {
                        Grasshopper.Kernel.Special.GH_NumberSlider nslider = (Grasshopper.Kernel.Special.GH_NumberSlider)sliderlist[i];

                        double SteppedSlider = MyComponent.VarsVals[i] + Gradient[MyComponent.ObjNum][i] * MyComponent.StepSize;

                        nslider.TrySetSliderValue((decimal)SteppedSlider);
                    }

                    if (MyComponent.Direction < 0)
                    {
                        Grasshopper.Kernel.Special.GH_NumberSlider nslider = (Grasshopper.Kernel.Special.GH_NumberSlider)sliderlist[i];

                        double SteppedSlider = MyComponent.VarsVals[i] - Gradient[MyComponent.ObjNum][i] * MyComponent.StepSize;

                        nslider.TrySetSliderValue((decimal)SteppedSlider);
                    }

                     /// TAKE STEP IN ORTHOGONAL DIRECTION

                    //if (MyComponent.Direction == 0)
                    //{

                    //Grasshopper.Kernel.Special.GH_NumberSlider nslider = (Grasshopper.Kernel.Special.GH_NumberSlider)sliderlist[i];

                    //double SteppedSlider = MyComponent.VarsVals[i] - IsoPerf[i] * MyComponent.StepSize;

                    //nslider.TrySetSliderValue((decimal)SteppedSlider);

                    //}



            }

                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);

            //return base.RespondToKeyDown(sender, e);

            return base.RespondToMouseDoubleClick(sender, e);
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
