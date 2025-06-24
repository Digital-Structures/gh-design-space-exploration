using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLoptNet;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using DSOptimization;
using Grasshopper.Kernel;
using Grasshopper;
using System.Diagnostics;

namespace Radical
{
    //RADICAL OPTIMIZER 
    public class RadicalOptimizer

    {
        public Design Design;
        public uint nVars { get { return (uint)this.Design.ActiveVariables.Count; } }

        public NLoptAlgorithm MainAlg;
        public NLoptAlgorithm SecondaryAlg; // Optional
        public NLoptSolver Solver;
        public RadicalVM RadicalVM;
        public RadicalWindow RadicalWindow; 

        public ChartValues<double> StoredMainValues;
        public ChartValues<ChartValues<double>> StoredConstraintValues;

        //public Boolean SolutionInProcess;

        //array of objects that should not be expired until the final round
        private List<IGH_DocumentObject> Disable = new List<IGH_DocumentObject>();
        private List<IGH_ActiveObject> SetExpiredFalse = new List<IGH_ActiveObject>();

        //static bool verbose = true;
        public double? MinValue; //init objective

        public List<Boolean> Recomputed;

        public bool DisablingAllowed; //if user want us to disable components that are not necessary in recomputation

        // CONSTRUCTOR FOR RADICAL
        public RadicalOptimizer(Design design, RadicalWindow radwindow)
        {
            this.Design = design;
            this.RadicalWindow = radwindow;
            this.RadicalVM = this.RadicalWindow.RadicalVM;
            this.MainAlg = this.RadicalVM.PrimaryAlgorithm;
            this.DisablingAllowed = !this.RadicalVM.DisablingNotAllowed;

            //this.SecondaryAlg = NLoptAlgorithm.LN_COBYLA;
            BuildWrapper();
            SetBounds();

            StoredMainValues = new ChartValues<double>();
            StoredConstraintValues = new ChartValues<ChartValues<double>>();

            if (this.DisablingAllowed)
            {
                FindWhichOnesToDisable();
            }

            if (Design.Constraints != null)
            {
                foreach (Constraint c in Design.Constraints)
                {
                    if (c.IsActive)
                    {
                        StoredConstraintValues.Add(new ChartValues<double>());
                        if (c.MyType == Constraint.ConstraintType.lessthan)
                            Solver.AddLessOrEqualZeroConstraint((x) => constraint(x, c));
                        else if (c.MyType == Constraint.ConstraintType.morethan)
                            Solver.AddLessOrEqualZeroConstraint((x) => -constraint(x, c));
                        else
                            Solver.AddEqualZeroConstraint((x) => constraint(x, c));
                    }
                }
            }

            Solver.SetMinObjective((x) => Objective(x));
        }

        public void BuildWrapper()
        {
            this.Solver = new NLoptSolver(MainAlg, nVars, this.RadicalVM.ConvCrit, this.RadicalVM.Niterations);
        }

        public void BuildWrapper(double relStopTol, int niter)
        {
            this.Solver = new NLoptSolver(MainAlg, nVars, relStopTol, niter);
        } // relStopTO, neither should be made optional arguments

        public void SetBounds()
        {
            Solver.SetLowerBounds(Design.ActiveVariables.Select(x => x.Min).ToArray()); // should we do it only for active variables?
            Solver.SetUpperBounds(Design.ActiveVariables.Select(x => x.Max).ToArray());
        }


        public void FindWhichOnesToDisable()
        {
            //find all active objects on the board
            //find downstream of every object
            //if downstream contains DS Opt then do not expire that component 

            List<IGH_ActiveObject> disable = new List<IGH_ActiveObject>();

            //if an active object does not DSOpt in the downstream, then we can consider disabling it
            foreach (IGH_ActiveObject a in Grasshopper.Instances.ActiveCanvas.Document.ActiveObjects())
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
            //Now we consider what items are downstream of our input parameters
            //If they are downstream our input parameters then we have disable them or else they will automatically recompute

            List<IGH_ActiveObject> actually_disable = new List<IGH_ActiveObject>();
            List<IGH_ActiveObject> expire = new List<IGH_ActiveObject>();

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

            if (!this.RadicalVM.DisablingNotAllowed)
            {
                //convert nonExpired list of Active objects to nonEnabled list of Document Objects
                foreach (IGH_ActiveObject a in actually_disable)
                {
                    Disable.Add((IGH_DocumentObject)a);
                }
            }
        
            SetExpiredFalse = expire;
        }

        public double Objective(double[] x)
        {
            //Debug.WriteLine("Obj");
            //x.ToList().ForEach(i => Debug.WriteLine(i.ToString()));

            DateTime start_iteration = DateTime.Now; 

            bool finished = false;

            Grasshopper.Kernel.GH_SolutionMode refresh = Grasshopper.Kernel.GH_SolutionMode.Silent;

            if (this.RadicalVM.Mode == RefreshMode.Live) { refresh = Grasshopper.Kernel.GH_SolutionMode.Default; }

            System.Action run = delegate ()
            {
                //Turn off all components not needed for gradient calculation and stepping 
                Grasshopper.Instances.ActiveCanvas.Document.SetEnabledFlags(Disable, false);

                //set other inactive objects expired flags to false
                foreach (IGH_ActiveObject a in this.SetExpiredFalse)
                {
                    a.ExpireSolution(false);
                }

                for (int i = 0; i < nVars; i++)
                {
                    IVariable var = Design.ActiveVariables[i]; // ONLY INCL. VARS
                    var.UpdateValue(x[i]);
                }
                // Once all points have been updated, we can update the geometries
                foreach (IDesignGeometry geo in this.Design.Geometries)
                {
                    geo.Update();
                }

                if (this.Design.Geometries.Any())
                {
                    Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true, refresh);
                }
                else
                {
                    Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false, refresh);
                }

               // this.SolutionInProcess = true;
                finished = true;
            };
            Rhino.RhinoApp.MainApplicationWindow.Invoke(run);

            // inelegant, temporary
            while (!finished)
            {
                Thread.Sleep(1);
            }

            double objective = Design.Objectives[0];

            //When a new global objective minimum is reached all variable values at that point are recorded
            //if constraints are computed after the objective then this might be why our return to optimal solution guy does not work ...
            if (objective < this.RadicalVM.SmallestObjectiveValue && AllConstraintsSatisfied())
            {
                this.RadicalVM.SmallestObjectiveValue = objective; 
                foreach (VarVM v in this.RadicalVM.NumVars)
                {
                    v.UpdateBestSolutionValue();
                }
                foreach (List<VarVM> lvm in this.RadicalVM.GeoVars)
                {
                    foreach (VarVM v in lvm)
                    {
                        v.UpdateBestSolutionValue();
                    }
                }
            }

            //store evolution of number variables 
            for(int i = 0; i < this.RadicalVM.NumVarEvolution.Count; i++)
            {
                this.RadicalVM.NumVarEvolution[i].Add(Design.ActiveVariables[i].CurrentValue);
            }

            //If RefreshMode == Silent then values are stored in a local list so that the graph is not updated
            if (this.RadicalVM.Mode == RefreshMode.Silent)
            {
                this.StoredMainValues.Add(objective);

                for (int i = 0; i < this.RadicalVM.ConstraintsEvolution.Count; i++)
                {
                    double score = Design.Constraints[i].CurrentValue;
                    this.StoredConstraintValues[i].Add(score);
                }

            }
            else
            {
                this.RadicalVM.UpdateCurrentScoreDisplay();

                //If refresh mode is switched from Silent to Live 
                if (StoredMainValues.Any())
                {
                    AppendStoredValues();
                    this.RadicalVM.AutomateStepSize(true);
                }

                //Checks to see if # of iterations is = number of original steps in graph, if it is it will make it auto
                this.RadicalVM.AutomateStepSize(false);

                //Adds main objective values to list and draws
                this.RadicalVM.ObjectiveEvolution.Add(objective);

                for (int i = 0; i < this.RadicalVM.ConstraintsEvolution.Count; i++)
                {
                    double score = Design.Constraints[i].CurrentValue;
                    this.RadicalVM.ConstraintsEvolution[i].Add(score);
                }

                foreach(GraphVM gvm in this.RadicalVM.ActiveGraphs)
                {
                    gvm.ForceGraphUpdate();
                }
            }

            try
            {
                this.RadicalWindow.source.Token.ThrowIfCancellationRequested();
            }
            catch
            {
                bool final_refresh_done = false;
                System.Action lastrun = delegate ()
                {
                    //turn disabled guys back on 
                    Grasshopper.Instances.ActiveCanvas.Document.SetEnabledFlags(Disable, true);
                    Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false);
                    final_refresh_done = true;
                };
                Rhino.RhinoApp.MainApplicationWindow.Invoke(lastrun);

                while (!final_refresh_done)
                {

                }

                if (this.RadicalVM.Mode == RefreshMode.Silent)
                {
                    EndSilentMode();
                }

                throw new OperationCanceledException();
            }

            DateTime end_iteration = DateTime.Now;

            if ((this.StoredMainValues.Count + this.RadicalVM.ObjectiveEvolution.Count) % 10 == 0)
            {
                this.RadicalVM.TimeEvolution.Add(end_iteration - start_iteration);
            }
            else
            {
                this.RadicalVM.TimeEvolution.Add(new TimeSpan(0,0,-10));
            }

            return objective;
        }

        public bool AllConstraintsSatisfied()
        {
            foreach (Constraint c in this.Design.Constraints)
            {
                if (c.MyType.Equals(Constraint.ConstraintType.lessthan))
                {
                    if(!(c.CurrentValue < c.LimitValue))
                    {
                        return false;
                    }
                }
                else if (c.MyType.Equals(Constraint.ConstraintType.morethan))
                {
                    if (!(c.CurrentValue > c.LimitValue))
                    {
                        return false;
                    }
                }
                else if (c.MyType.Equals(Constraint.ConstraintType.equalto))
                {
                    if(!(c.CurrentValue == c.LimitValue))
                    {
                        return false; 
                    }
                }
            }
            return true; 
        }

        //Why are we returning here?
        public NloptResult RunOptimization()
        {
            //STARTED
            this.RadicalWindow.OptimizationStarted();

            // Run optimization with only the activeVariables
            double[] x = Design.ActiveVariables.Select(t => t.CurrentValue).ToArray();
            double[] query = x;
            double startingObjective = Design.Objectives[0];
            NloptResult result = Solver.Optimize(x, out MinValue);

            //FINISHED
            //Updates graph if the optimization ends in silent mode
            if (this.RadicalWindow.RadicalVM.Mode == RefreshMode.Silent)
            {
                EndSilentMode();
            }

            bool all_enabled = false; 
            System.Action run = delegate ()
            {
                //turn disabled guys back on 
                Grasshopper.Instances.ActiveCanvas.Document.SetEnabledFlags(Disable, true);
                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false, Grasshopper.Kernel.GH_SolutionMode.Default);
                all_enabled = true;
            };
            Rhino.RhinoApp.MainApplicationWindow.Invoke(run);
            while (!all_enabled)
            {
            }


            this.RadicalWindow.OptimizationFinished();

            return result;
        }

        //Steps to take when we end using silent mode, particularly when:
            //optimization finishes in silent mode
            //we pause optimization when it is in silent mode
        public void EndSilentMode()
        {
            AppendStoredValues();
            this.RadicalVM.AutomateStepSize(true);
            this.RadicalVM.UpdateCurrentScoreDisplay();
            System.Action run = delegate ()
            {
                if (this.Design.Geometries.Any())
                {
                    Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
                }
                else
                {
                    Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false);
                }
            };
            Rhino.RhinoApp.MainApplicationWindow.Invoke(run);
        }

        //Adds optimization values to array that updates graph display 
        public void AppendStoredValues()
        {
            this.RadicalVM.ObjectiveEvolution.AddRange(StoredMainValues);
            StoredMainValues.Clear();

            for (int i = 0; i < this.RadicalVM.ConstraintsEvolution.Count; i++)
            {
                this.RadicalVM.ConstraintsEvolution[i].AddRange(StoredConstraintValues[i]);
                StoredConstraintValues[i].Clear();
            }
        }

        public double constraint(double[] x, Constraint c)
        {
            return c.CurrentValue - c.LimitValue;
        }

        //UNIMPLEMENTED
        //Can we delete this?
        public double Objective(double[] x, ref double[] grad)
        {
            for (int i = 0; i < nVars; i++)
            {
                IVariable var = Design.Variables[i];
                var.UpdateValue(x[i]);
            }

            if (grad != null) { }//update gradient, for gradient-based algs.

            double objective = Design.Objectives[0];
            this.RadicalVM.ObjectiveEvolution.Add(objective);
            return objective;
        }
    }
}
