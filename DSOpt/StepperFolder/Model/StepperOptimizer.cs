using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Radical.Integration;
using DSOptimization;

namespace Stepper
{
    //STEPPER OPTIMIZER
    public class StepperOptimizer
    {
        public enum Direction { Maximize, Minimize, Isoperformance }

        private Design Design;
        private int ObjIndex;
        private Direction Dir;
        private double StepSize;

        private int numVars;
        private int numObjs;
        private double FDstep;

        //Useful lists for value tracking
        private List<List<double>> ObjectiveData;
        private List<List<double>> IsoPerf;

        //array of objects that should not be expired until the final round
        private List<IGH_DocumentObject> Disable = new List<IGH_DocumentObject>();
        private List<IGH_ActiveObject> SetExpiredFalse;

        private bool DisablingAllowed;

        //CONSTRUCTOR for Gradient Calculation only
        public StepperOptimizer(Design design, double FDStepSize, bool disablingAllowed)
        {
            this.Design = design;

            numVars = Design.ActiveVariables.Count;
            numObjs = Design.Objectives.Count;

            ObjectiveData = new List<List<double>>();
            IsoPerf = new List<List<double>>();

            this.DisablingAllowed = disablingAllowed;
            FindWhichOnesToDisable();

            this.FDstep = FDStepSize;
        }

        public void ConvertFromCalculatorToOptimizer(int objIndex, Direction dir, double stepSize)
        {
            this.ObjIndex = objIndex;
            this.Dir = dir;
            this.StepSize = stepSize;
        }

        //CONSTRUCTOR for complete step optimization
        public StepperOptimizer(Design design, int objIndex, Direction dir, double stepSize, double FDStepSize)
        {
            this.Design = design;
            this.ObjIndex = objIndex;
            this.Dir = dir;
            this.StepSize = stepSize;

            numVars = Design.ActiveVariables.Count;
            numObjs = Design.Objectives.Count;
            this.FDstep = FDStepSize; 

            ObjectiveData = new List<List<double>>();
            IsoPerf = new List<List<double>>();
            FindWhichOnesToDisable();
        }

        public void FindWhichOnesToDisable()
        {
            //find all active objects on the board
            //find downstream of every object
            //if downstream contains DS Opt then do not expire that component 

            List<IGH_ActiveObject> disable = new List<IGH_ActiveObject>();

            //if an active object does not DSOpt in the downstream, then we can consider disabling it
            foreach(IGH_ActiveObject a in Grasshopper.Instances.ActiveCanvas.Document.ActiveObjects())
            {
                //check if the object is not already expired
                if (!Instances.ActiveCanvas.Document.DisabledObjects().Contains(a))
                {
                    List<IGH_ActiveObject> downstream = Grasshopper.Instances.ActiveCanvas.Document.FindAllDownstreamObjects(a);
                    if (!downstream.Contains(this.Design.MyComponent))
                    {
                        // Make sure it isn't the DSOpt component itself
                        if (a != this.Design.MyComponent)
                        {
                            disable.Add(a);
                        }
                    }
                }
            }

            //NEXT PART

            List<IGH_ActiveObject> actually_disable = new List<IGH_ActiveObject>();
            List<IGH_ActiveObject> expire = new List<IGH_ActiveObject>();


            //Now we consider what items are downstream of our input parameters
            List<List<IGH_ActiveObject>> sliders_downstream = new List<List<IGH_ActiveObject>>();
            List<List<IGH_ActiveObject>> curves_downstream = new List<List<IGH_ActiveObject>>();
            List<List<IGH_ActiveObject>> surfaces_downstream = new List<List<IGH_ActiveObject>>();

            foreach (IGH_Param s in this.Design.MyComponent.NumObjects)
            {
                List<IGH_ActiveObject> downstream = Grasshopper.Instances.ActiveCanvas.Document.FindAllDownstreamObjects((IGH_ActiveObject)s);
                sliders_downstream.Add(downstream);
            }
            foreach (IGH_Param c in this.Design.MyComponent.CrvObjects)
            {
                List<IGH_ActiveObject> downstream = Grasshopper.Instances.ActiveCanvas.Document.FindAllDownstreamObjects((IGH_ActiveObject)c);
                curves_downstream.Add(downstream);
            }
            foreach (IGH_Param s in this.Design.MyComponent.SrfObjects)
            {
                List<IGH_ActiveObject> downstream = Grasshopper.Instances.ActiveCanvas.Document.FindAllDownstreamObjects((IGH_ActiveObject)s);
                surfaces_downstream.Add(downstream);
            }


            //here we attempt to minimize the number of objects that we are disabling and which ones we can simply set the expire flag to false
            //if an object is downstream of an active parameter then we have to disable it so that it doesn't recompute 
            foreach (IGH_ActiveObject d in disable)
            {
                Boolean found = false;
                foreach (List<IGH_ActiveObject> dstream in sliders_downstream)
                {
                    if (dstream.Contains(d))
                    {
                        found = true;
                        break;
                    }
                }
                foreach (List<IGH_ActiveObject> dstream in curves_downstream)
                {
                    if (dstream.Contains(d))
                    {
                        found = true;
                        break;
                    }
                }
                foreach (List<IGH_ActiveObject> dstream in surfaces_downstream)
                {
                    if (dstream.Contains(d))
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    actually_disable.Add(d);
                }
                else
                {
                    expire.Add(d);
                }
            }

            if (this.DisablingAllowed)
            {
                //convert nonExpired list of Active objects to nonEnabled list of Document Objects
                foreach (IGH_ActiveObject a in actually_disable)
                {
                    Disable.Add((IGH_DocumentObject)a);
                }
            }
            SetExpiredFalse = expire;
        }

        public List<List<double>> CalculateGradient()
        {
            Boolean onEdge = false;
            foreach(IVariable v in Design.ActiveVariables)
            {
                if(v.Min == v.CurrentValue || v.Max == v.CurrentValue)
                {
                    onEdge = true;
                    break;
                }
            }
            if (onEdge)
            {
                return CalculateGradientHalfStep();
            }
            else
            {
                return CalculateGradientForwardStep();
            }
        }

        #region forward step
        public List<List<double>> GenerateDesignMapForwardStep()
        {
            var DesignMapStepperOne = new List<List<double>>();
            var DesignMapStepperCombined = new List<List<double>>();

            //Create Design Map, list of points to be tested
            for (int i = 0; i < numVars; i++)
            {
                DesignMapStepperOne.Add(new List<double>());

                for (int j = 0; j < numVars; j++)
                {
                    DesignMapStepperOne[i].Add(Design.ActiveVariables[j].CurrentValue);
                }

                IVariable var = Design.ActiveVariables[i];
                double difference = 0.5 * FDstep * (var.Max - var.Min);

                double left = var.CurrentValue - difference;

                DesignMapStepperOne[i][i] = left;
            }

            // Combine lists
            DesignMapStepperCombined.AddRange(DesignMapStepperOne);

            // Add dummy at end to resent sliders
            DesignMapStepperCombined.Add(Design.ActiveVariables.Select(var => var.CurrentValue).ToList());

            return DesignMapStepperCombined;
        }

        public List<List<double>> CalculateGradientForwardStep()
        {
            var DesignMap = GenerateDesignMapForwardStep();
            Iterate(DesignMap);

            var Gradient = new List<List<double>>();

            double maxObj = double.MinValue;
            double minObj = double.MaxValue;

            // find the gradient for each objective by taking finite differences of every variable
            for (int j = 0; j < numObjs; j++)
            {
                Gradient.Add(new List<double>());

                int active_index = 0;
                for (int i = 0; i < this.Design.Variables.Count; i++)
                {
                    if (this.Design.Variables[i].IsActive)
                    {
                        double left = ObjectiveData[active_index][j];

                        double difference = (Design.Objectives[j] - left) / (FDstep);

                        if (difference > maxObj) { maxObj = difference; }
                        if (difference < minObj) { minObj = difference; }

                        Gradient[j].Add((double)difference);
                        active_index += 1; 
                    }
                    else
                    {
                        Gradient[j].Add(0);
                    }
                }

                //Normalize by max/min difference
                double maxAbs = double.MinValue;
                double vecLength = 0;

                if (Math.Abs(maxObj) > maxAbs) { maxAbs = Math.Abs(maxObj); }
                if (Math.Abs(minObj) > maxAbs) { maxAbs = Math.Abs(minObj); }

                for (int i = 0; i < this.Design.Variables.Count; i++)
                {
                    if (maxAbs != 0)
                    {
                        Gradient[j][i] = (Gradient[j][i] / maxAbs);
                    }
                    else
                    {
                        Gradient[j][i] = 0;
                    }
                    vecLength = vecLength + (double)Gradient[j][i] * (double)Gradient[j][i];
                }

                for (int i = 0; i < this.Design.Variables.Count; i++)
                {
                    if(Gradient[j][i] != 0)
                    {
                        Gradient[j][i] = (Gradient[j][i] / Math.Sqrt(vecLength));
                    }
                }
            }

            return Gradient;
        }
        #endregion

        #region half steps
        public List<List<double>> GenerateDesignMapHalfStep()
        {
            var DesignMapStepperOne = new List<List<double>>();
            var DesignMapStepperTwo = new List<List<double>>();
            var DesignMapStepperCombined = new List<List<double>>();

            //Create Design Map, list of points to be tested
            for (int i = 0; i < numVars; i++)
            {
                DesignMapStepperOne.Add(new List<double>());
                DesignMapStepperTwo.Add(new List<double>());

                for (int j = 0; j < numVars; j++)
                {
                    DesignMapStepperOne[i].Add(Design.ActiveVariables[j].CurrentValue);
                    DesignMapStepperTwo[i].Add(Design.ActiveVariables[j].CurrentValue);
                }

                IVariable var = Design.ActiveVariables[i];
                double difference = 0.5 * FDstep * (var.Max - var.Min);

                double left = var.CurrentValue - difference;
                double right = var.CurrentValue + difference;

                DesignMapStepperOne[i][i] = left;
                DesignMapStepperTwo[i][i] = right;
            }

            // Combine lists
            DesignMapStepperCombined.AddRange(DesignMapStepperOne);
            DesignMapStepperCombined.AddRange(DesignMapStepperTwo);

            // Add dummy at end to resent sliders
            DesignMapStepperCombined.Add(Design.ActiveVariables.Select(var => var.CurrentValue).ToList());

            return DesignMapStepperCombined;
        }

        public List<List<double>> CalculateGradientHalfStep()
        {
            var DesignMap = GenerateDesignMapHalfStep();
            Iterate(DesignMap);

             var Gradient = new List<List<double>>();

            double maxObj = double.MinValue;
            double minObj = double.MaxValue;

            // find the gradient for each objective by taking finite differences of every variable
            for (int j = 0; j < numObjs; j++)
            {
                Gradient.Add(new List<double>());

                int active_index = 0;
                for (int i = 0; i < this.Design.Variables.Count; i++)
                {
                    if (this.Design.Variables[i].IsActive)
                    {
                        double left = ObjectiveData[active_index][j];
                        double right = ObjectiveData[numVars + active_index][j];

                        double difference = (right - left) / (FDstep);

                        if (difference > maxObj) { maxObj = difference; }
                        if (difference < minObj) { minObj = difference; }

                        Gradient[j].Add((double)difference);

                        active_index += 1;
                    }
                    else
                    {
                        Gradient[j].Add(0);
                    }
                    
                }

                //Normalize by max/min difference
                double maxAbs = double.MinValue;
                double vecLength = 0;

                if (Math.Abs(maxObj) > maxAbs) { maxAbs = Math.Abs(maxObj); }
                if (Math.Abs(minObj) > maxAbs) { maxAbs = Math.Abs(minObj); }

                for (int i = 0; i < numVars; i++)
                {
                    if (maxAbs != 0)
                    {
                        Gradient[j][i] = (Gradient[j][i] / maxAbs);
                    }
                    else
                    {
                        Gradient[j][i] = 0;
                    }
                    vecLength = vecLength + (double)Gradient[j][i] * (double)Gradient[j][i];
                }

                for (int i = 0; i < numVars; i++)
                {
                    if (Gradient[j][i] != 0)
                    {
                        Gradient[j][i] = (Gradient[j][i] / Math.Sqrt(vecLength));
                    }
                }
            }

            return Gradient;
        }
        #endregion

        public void Iterate(List<List<double>> DesignMap)
        {
            bool finished = false;

            //Invoke a delegate to solve threading issue
            System.Action run = delegate ()
            {

                //Turn off all components not needed for gradient calculation and stepping 
                Grasshopper.Instances.ActiveCanvas.Document.SetEnabledFlags(Disable, false);

                //set other inactive objects expired flags to false
                foreach (IGH_ActiveObject a in this.SetExpiredFalse)
                {
                    a.ExpireSolution(false);
                }

                foreach (List<double> sample in DesignMap)
                {
                    int i = 0;
                    foreach (double val in sample)
                    {
                        Design.ActiveVariables[i].UpdateValue(val);
                        i++;
                    }

                    if (this.Design.Geometries.Any())
                    {
                        foreach (IDesignGeometry geo in this.Design.Geometries)
                        {
                            geo.Update();
                        }
                    }

                    Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false, Grasshopper.Kernel.GH_SolutionMode.Silent);
                    this.ObjectiveData.Add(Design.Objectives);
                }

                //Turn back on components and recalculate after final step
                Grasshopper.Instances.ActiveCanvas.Document.SetEnabledFlags(Disable, true);
                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false, Grasshopper.Kernel.GH_SolutionMode.Default);

                finished = true;
            };
            Rhino.RhinoApp.MainApplicationWindow.Invoke(run);

            //Wait for iteration thread to finish
            while (!finished)
            {
            }
        }

        public void DownStreamExpire()
        {
            List<IGH_ActiveObject> active = Grasshopper.Instances.ActiveCanvas.Document.ActiveObjects();
            List<IGH_ActiveObject> downstream = Grasshopper.Instances.ActiveCanvas.Document.FindAllDownstreamObjects(active);
            foreach (IGH_ActiveObject d in downstream)
            {
                //Grasshopper.Instances.ActiveCanvas.Document.Ex
                d.ExpireSolution(true);
            }
            

            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false, Grasshopper.Kernel.GH_SolutionMode.Silent);
        }

        public void Optimize(List<List<double>> Gradient)
        {
            //// FIND THE ORTHOGONAL VECTORS
            ////double[][] gradientArray = Gradient.Select(a => a.ToArray()).ToArray();
            List<List<string>> lst = new List<List<string>>();
            double[,] gradientArray = new double[Gradient.Count, Gradient[0].Count];

            for (int j = 0; j < Gradient.Count; j++)
            {
                for (int i = 0; i < Gradient[j].Count; i++)
                {
                    //Temporary check for null gradients, usually pop up for surface objectives which are not implemented 
                    if (double.IsNaN((double)Gradient[j][i]))
                    {
                        return; 
                    }
                    gradientArray[j, i] = (double)Gradient[j][i];
                }
            }

            var matrixGrad = MathNet.Numerics.LinearAlgebra.Double.DenseMatrix.OfArray(gradientArray);
            var nullspace = matrixGrad.Kernel();

            int dir = new int();
            dir = 0; //dummy value or else it won't compile
            if (this.Dir == Direction.Isoperformance)
            {
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
                int testrand = new int();
                testrand = rnd.Next(numVars - numObjs);

                dir = testrand;
            }
            

            #region commented out code
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
            #endregion

            // step in the right direction based on the gradient vector

            //Set all sliders to their optimized values
            int active_index = 0; 
            for(int i = 0; i < Design.Variables.Count; i++)
            {
                if (Design.Variables[i].IsActive)
                {
                    IVariable var = Design.ActiveVariables[active_index];
                    double SteppedValue;

                    switch (this.Dir)
                    {
                        case Direction.Maximize:
                            SteppedValue = var.CurrentValue + (double)Gradient[this.ObjIndex][i] * this.StepSize * (var.Max - var.Min);
                            var.CurrentValue = SteppedValue;
                            break;

                        case Direction.Minimize:
                            SteppedValue = var.CurrentValue - (double)Gradient[this.ObjIndex][i] * this.StepSize * (var.Max - var.Min);
                            var.CurrentValue = SteppedValue;
                            break;

                        case Direction.Isoperformance:
                            List<double> IsoPerfDirList = IsoPerf[dir];
                            SteppedValue = var.CurrentValue + IsoPerfDirList[i] * this.StepSize * numVars;
                            var.CurrentValue = SteppedValue;
                            break;
                    }

                    active_index += 1;
                }
            }

            #region Commented out code
            //for (int i = 0; i < numVars; i++)
            //{
            //    IVariable var = Design.ActiveVariables[i];
            //    double SteppedValue;

            //    switch(this.Dir)
            //    {
            //        case Direction.Maximize:
            //            SteppedValue = var.CurrentValue + (double)Gradient[this.ObjIndex][i] * this.StepSize * (var.Max - var.Min);
            //            var.CurrentValue = SteppedValue;
            //            break;

            //        case Direction.Minimize:
            //            SteppedValue = var.CurrentValue - (double)Gradient[this.ObjIndex][i] * this.StepSize * (var.Max - var.Min);
            //            var.CurrentValue = SteppedValue;
            //            break;

            //        case Direction.Isoperformance:
            //            List<double> IsoPerfDirList = IsoPerf[dir];
            //            SteppedValue = var.CurrentValue + IsoPerfDirList[i] * this.StepSize * numVars;
            //            var.CurrentValue = SteppedValue;
            //            break;
            //    }
            //}
            #endregion

            //Append data to the end of component output lists
            this.Design.UpdateComponentOutputs(Gradient);
        }
    }
}
