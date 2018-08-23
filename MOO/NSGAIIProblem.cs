using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Wrapper;
using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;


namespace MOO
{
    /// <summary>
    /// 
    /// </summary>
    public class NSGAIIProblem : Problem
    {
        MOO component = null;
        List<double> var1Value = new List<double>();
        List<double> var2Value = new List<double>();
        List<double> objectiveValue = new List<double>();
        List<GH_NumberSlider> variablesSliders = new List<GH_NumberSlider>();
        List<double> objectives = new List<double>();

        // All Solutions
        public List<List<double>> allSolutions = new List<List<double>>();

        #region Constructors

        /// <summary>
        /// Constructor.
        /// Creates a new multiobjective problem instance.
        /// </summary>
        /// <param name="solutionType">The solution type must "Real" or "BinaryReal", and "ArrayReal".</param>
        /// <param name="numberOfVariables">Number of variables</param>
        public NSGAIIProblem(string solutionType, MOO comp, int solutionsCounter)
        {
            this.component = comp;
            NumberOfVariables = comp.readSlidersList().Count;
            NumberOfObjectives = comp.objectives.Count;
            NumberOfConstraints = 0;
            ProblemName = "Multiobjective";

            // Log
            comp.LogAddMessage("Number of Variables = " + NumberOfVariables);
            comp.LogAddMessage("Number of Objectives = " + NumberOfObjectives);
            comp.LogAddMessage("Number of Constraints = " + NumberOfConstraints);


            UpperLimit = new double[NumberOfVariables];
            LowerLimit = new double[NumberOfVariables];

            for (int i = 0; i < NumberOfVariables; i++)
            {
                GH_NumberSlider curSlider = comp.readSlidersList()[i];

                LowerLimit[i] = (double)curSlider.Slider.Minimum;
                UpperLimit[i] = (double)curSlider.Slider.Maximum;
            }

            if (solutionType == "BinaryReal")
            {
                SolutionType = new BinaryRealSolutionType(this);
            }
            else if (solutionType == "Real")
            {
                SolutionType = new RealSolutionType(this);
            }
            else if (solutionType == "ArrayReal")
            {
                SolutionType = new ArrayRealSolutionType(this);
            }
            else
            {
                Console.WriteLine("Error: solution type " + solutionType + " is invalid");
                //Logger.Log.Error("Solution type " + solutionType + " is invalid");
                return;
            }

            // Log
            comp.LogAddMessage("Solution Type = " + solutionType);
        }

        #endregion

        public override void Evaluate(Solution solution)
        {
            // Current Solution
            List<double> currentSolution = new List<double>();

            double[] storeVar = new double[NumberOfVariables];
            double[] storeObj = new double[NumberOfObjectives];
            XReal x = new XReal(solution);

            // Reading x values
            double[] xValues = new double[NumberOfVariables];
            for (int i = 0; i < NumberOfVariables; i++)
            {
                xValues[i] = x.GetValue(i);
                var1Value.Add(x.GetValue(0));
                var2Value.Add(x.GetValue(1));

                currentSolution.Add(x.GetValue(i)); // add current variable value to current solution

            }
            GH_NumberSlider currentSlider = null;
            for (int i = 0; i < component.readSlidersList().Count; i++)
            {
                currentSlider = component.readSlidersList()[i];
                currentSlider.SetSliderValue((decimal)x.GetValue(i));
                 //Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true); //<- NOTE: Is this the part that's re-evaluating after every slider change??
            }

            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);

            for (int i = 0; i < component.objectives.Count; i++)
            {
                solution.Objective[i] = component.objectives[i];

                currentSolution.Add(component.objectives[i]); //adding current i-objective to current solution 
            }
            //component.allSolutions = component.allSolutions +"" + component.objectives[i] + " ";

            allSolutions.Add(currentSolution);

        }

        public void PrintAllSolutions()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"" + component.directory + "allSolutions-" + component.fileName);
            for (int i = 0; i < allSolutions.Count; i++)
            {
                string design = "";
                List<double> currentDesign = allSolutions[i];
                for (int j = 0; j < currentDesign.Count - 1; j++)
                {
                    design = design + currentDesign[j] + ",";
                }

                design = design + currentDesign[currentDesign.Count - 1];

                file.WriteLine(design);
            }

            file.Close();
        }

        public void PrintLogFile()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"" + component.directory + "LogFile-" + component.fileName);
            string design = component.log;
            file.WriteLine(design);
            file.Close();
        }
    }
}
