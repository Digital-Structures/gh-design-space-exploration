using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using DSECommon;
using System.Threading;
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
            this.IsoPerf = new List<List<double>>();

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
        public int dir;
        public List<double> IsoPerfDirList;
        public double FDstep;

        public List<List<double>> test; //JUST TO TEST


        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            //Prevent opening of multiple windows at once
            if (!MyComponent.IsWindowOpen)
            {
                MyComponent.IsWindowOpen = true;
                StepperVM VM= new StepperVM(this.MyComponent);

                Thread viewerThread = new Thread(delegate ()
                {
                    System.Windows.Window viewer = new StepperWindow(VM);
                    viewer.Show();
                    System.Windows.Threading.Dispatcher.Run();
                });

                viewerThread.SetApartmentState(ApartmentState.STA); // needs to be STA or throws exception
                viewerThread.Start();
            }

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

            //TO TEST OUTPUTS
            test.Add(new List<double>());
            test[0].Add(248.0);

            numVars = MyComponent.numVars;
            numObjs = MyComponent.numObjs;

            //Set Finite Differences step size
            FDstep = 0.01;


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

            //var matrixGrad = MathNet.Numerics.LinearAlgebra.Double.Matrix.Abs(14.0);

            //var matrixGradT = matrixGrad.Transpose();

            var nullspace = matrixGrad.Kernel();

            // Convert array to List of nullspace vectors
            if (numVars > numObjs)
            {
                for (int i = 0; i < numVars - numObjs; i++)
                {
                    IsoPerf.Add(new List<double>());
                    double[] IsoPerfDir = nullspace[i].ToArray();
                    IsoPerf[i].AddRange(IsoPerfDir);
                }
            }

            // Randomly pick an isoperformance direction
            Random rnd = new Random();
            int dir = new int();
            int testrand = new int();
            testrand = rnd.Next(numVars - numObjs);

            dir = testrand;

            

            // Ensure that direction is "interesting"

            //for (int i = testrand; i < numVars - numObjs - 1; i++)
            //{

            //    dir = i;
            //    List<double> IsoVecAbs = IsoPerf[i].Select(x => Math.Abs(x)).ToList();
            //    IsoVecAbs.Sort();

            //    double a = IsoVecAbs[numVars - 1]; 
            //    double b = IsoVecAbs[numVars - 2];
            //    double c = a / b;

            //    if (c < 3) { break; }
            //    else { dir = dir + 1; }

            //}

            //for (int i = 0; i < numVars - numObjs; i++)
            //{
            //    for (int j = 0; j < IsoPerf[i].Count; j++)
            //    {
            //        IsoPerf.Add(new List<double>());
            //        double[] IsoPerfDir = nullspace[i].ToArray();

            //    }
            //}


            List<double> IsoPerfDirList = IsoPerf[dir];


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
                    double SteppedSlider = MyComponent.VarsVals[i] + Gradient[MyComponent.ObjNum][i] * MyComponent.StepSize * (MyComponent.MaxVals[i] - MyComponent.MinVals[i]);
                    nslider.TrySetSliderValue((decimal)SteppedSlider);
                }

                if (MyComponent.Direction < 0)
                {
                    Grasshopper.Kernel.Special.GH_NumberSlider nslider = (Grasshopper.Kernel.Special.GH_NumberSlider)sliderlist[i];
                    double SteppedSlider = MyComponent.VarsVals[i] - Gradient[MyComponent.ObjNum][i] * MyComponent.StepSize * (MyComponent.MaxVals[i] - MyComponent.MinVals[i]) ;
                    nslider.TrySetSliderValue((decimal)SteppedSlider);
                }

                // TAKE STEP IN ORTHOGONAL DIRECTION
                if (MyComponent.Direction == 0)
                {
                    Grasshopper.Kernel.Special.GH_NumberSlider nslider = (Grasshopper.Kernel.Special.GH_NumberSlider)sliderlist[i];              
                    double SteppedSlider = MyComponent.VarsVals[i] + IsoPerfDirList[i] * MyComponent.StepSize*MyComponent.numVars;                  
                    nslider.TrySetSliderValue((decimal)SteppedSlider);
                }
            }

            stepped = true;
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);

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
