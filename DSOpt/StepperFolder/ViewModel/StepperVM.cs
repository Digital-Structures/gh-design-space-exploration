using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using LiveCharts;
using LiveCharts.Wpf;
using System.Threading;
using Radical;
using Radical.Integration;
using DSOptimization;

namespace Stepper
{
    //STEPPER VM
    //View Model to mediate communication between StepperWindow and StepperComponent
    public class StepperVM : BaseVM, IOptimizeToolVM
    {
        //Variables and properties
        public DSOptimizerComponent Component { get; set; }

        public ChartValues<ChartValues<double>> ObjectiveEvolution_Norm;
        public ChartValues<ChartValues<double>> ObjectiveEvolution_Abs;
        public List<List<double>> VariableEvolution;
        public List<List<List<double?>>> GradientEvolution;
        public List<TimeSpan> TimeEvolution; 

        public List<ObjectiveVM> Objectives;
        public List<VarVM> Variables { get; set; }
        public List<VarVM> NumVars { get; set; }
        public List<List<VarVM>> GeoVars { get; set; }
        public List<GroupVarVM> GroupVars { get; set; }

        public StepperGraphVM ObjectiveChart_Norm;
        public StepperGraphVM ObjectiveChart_Abs;
        public Design Design;

        public StepperVM() { }

        //CONSTRUCTOR
        public StepperVM(Design design, DSOptimizerComponent stepper)
        {
            //StepperComponent
            this.Component = stepper;

            //DesignSystem
            this.Design = design;
            this.index = 0;
            this.step = 0.05;
            this.fdstep = 0.01;
            this.trackedstep = 0;

            //Warn user that system can't handle constraints
            this._opendialog = false;
            if (this.Design.Constraints.Any())
                this.OpenDialog = true;

            this._openisodialog = false; 

            //Variable Lists
            //Separate for display
            this.NumVars = new List<VarVM>();
            this.GeoVars = new List<List<VarVM>>();
            this.GroupVars = new List<GroupVarVM>();
            SortVariables();

            //Variable lists
            //Combined for easy value updates
            this.Variables = new List<VarVM>();
            this.Variables.AddRange(this.NumVars);
            this.Variables.AddRange(this.GeoVars.SelectMany(x => x).ToList());
            

            //Set up Objective View Models and list of objective value evolution 
            this.ObjectiveEvolution_Norm = new ChartValues<ChartValues<double>>();
            this.ObjectiveEvolution_Abs = new ChartValues<ChartValues<double>>();
            this.Objectives = new List<ObjectiveVM>();

            this.TimeEvolution = new List<TimeSpan>();
            this.TimeEvolution.Add(TimeSpan.Zero);

            this.GradientEvolution = new List<List<List<double?>>>();

            int i = 0;
            //Set up list of objective evolution
            foreach (double objective in this.Design.Objectives)
            {
                ObjectiveVM Obj = new ObjectiveVM(objective, this);
                Obj.Name = this.Component.Params.Input[0].Sources[i].NickName;

                this.Objectives.Add(Obj);
                this.ObjectiveEvolution_Norm.Add(new ChartValues<double> { 1 });
                this.ObjectiveEvolution_Abs.Add(new ChartValues<double> { objective });
                this.GradientEvolution.Add(new List<List<double?>>());
                i++;
            }

            //Set up list of variable value and gradient evolution 
            this.VariableEvolution = new List<List<double>>();
            foreach (VarVM var in this.Variables)
            {
                this.VariableEvolution.Add(new List<double> { var.Value });

                foreach (List<List<double?>> objective in this.GradientEvolution)
                    objective.Add(new List<double?>());
            }

            //Set up both charts
            this.ObjectiveChart_Norm = new StepperGraphVM(ObjectiveEvolution_Norm);
            this.ObjectiveChart_Abs = new StepperGraphVM(ObjectiveEvolution_Abs);
            this.ObjectiveNamesChanged();

            this._disablingNotAllowed = false;

            //Check for zero's that will cause trouble

        }

        //OPEN DIALOG
        //Boolean to notify user if he's entered constraints
        private bool _opendialog;
        public virtual bool OpenDialog
        {
            get { return this._opendialog; }
            set { CheckPropertyChanged<bool>("OpenDialog", ref _opendialog, ref value); }
        }

        //OPEN ISO DIALOG
        //Bool to notify user of isoperformance inavailability 
        private bool _openisodialog;
        public virtual bool OpenIsoDialog
        {
            get { return this._openisodialog; }
            set { CheckPropertyChanged<bool>("OpenIsoDialog", ref _openisodialog, ref value); }
        }


        //OBJECTIVE NAMES
        //For combo box drop down
        public List<string> ObjectiveNames
        {
            get
            {
                var names = new List<string>();

                foreach (ObjectiveVM objective in this.Objectives)
                    names.Add(objective.Name);

                return names;
            }
        }

        //OBJECTIVE INDEX
        private int index;
        public int ObjIndex
        {
            get { return index; }
            set { CheckPropertyChanged<int>("ObjIndex", ref index, ref value); }
        }

        //OBJECTIVE NAME
        public string CurrentObjectiveName
        {
            get { return this.Objectives[ObjIndex].Name; }
        }

        //STEP SIZE
        private double step;
        public double StepSize
        {
            get { return step; }
            set { CheckPropertyChanged<double>("StepSize", ref step, ref value); }
        }

        //FD STEP SIZE
        private double fdstep;
        public double FDStepSize
        {
            get { return fdstep; }
            set
            {
                if(value > 0 && value <= 0.5)
                {
                    CheckPropertyChanged<double>("FDStepSize", ref fdstep, ref value);
                }
            }
        }

        //TRACKED STEP
        //Step number user is tracking with the UI slider
        //To potentially be reverted back to
        private int trackedstep;
        public int TrackedStep
        {
            get { return this.trackedstep; }
            set
            {
                if (CheckPropertyChanged<int>("TrackedStep", ref trackedstep, ref value))
                {
                    this.ObjectiveChart_Norm.GraphStep = value;
                    this.ObjectiveChart_Abs.GraphStep = value;
                }
                    
            }
        }

                //DISABLING ALLOWED
        private bool _disablingNotAllowed;
        public bool DisablingNotAllowed
        {
            get { return _disablingNotAllowed; }
            set
            {
                if (CheckPropertyChanged<bool>("DisablingNotAllowed", ref _disablingNotAllowed, ref value))
                {
                }
            }
        }

        //NUM STEPS
        //The number of steps taken so far (for graph tracking purposes)
        public int NumSteps
        {
            get { return this.ObjectiveEvolution_Norm[0].Count - 1; }
        }

        //SORT VARIABLES
        //Separate geometric and numeric variables
        //Sorting helps with UI stack panel organization
        private void SortVariables()
        {
            //GEOMETRIES
            int geoIndex = 1;
            foreach (IDesignGeometry geo in this.Design.Geometries)
            {
                List<VarVM> singleGeoVars = new List<VarVM> { };

                //Add all the variables for that geometry to a sublist of varVMs
                int varIndex = 0;
                foreach (GeoVariable var in geo.Variables)
                {
                    VarVM geoVar = new VarVM(var, this.Design);
                    int dir = var.Dir;

                    //Logical default naming of variable
                    //e.g. G1.u1v1.X
                    geoVar.Name += ((GeoVariable)geoVar.DesignVar).PointName;

                    singleGeoVars.Add(geoVar);
                    varIndex++;
                }

                this.GeoVars.Add(singleGeoVars);
                geoIndex++;
            }

            //SLIDERS
            /***This is probably not the best way to do this as it involves looping over geometry variables already stored***/

            int _count = 0;
            foreach (var numVar in this.Design.Variables.Where(numVar => numVar is SliderVariable))
            {
                VarVM v = new VarVM(numVar, this.Design);

                //following code is to ensure that sliders still have variable names in Stepper / Radical 
                //when users don't give them nicknames
                if (v.Name == "")
                {
                    v.Name = v.DesignVar.Parameter.TypeName + " " + _count;
                }
                this.NumVars.Add(v);
                _count += 1; 
            }
        }

        //OBJECTIVE NAMES CHANGED
        //Need to rebind chart legend to names list
        public void ObjectiveNamesChanged()
        {
            var index = this.ObjIndex;

            FirePropertyChanged("ObjectiveNames");

            if (this.ObjectiveChart_Abs != null)
            {
                int i = 0;
                foreach (LineSeries objectiveSeries in this.ObjectiveChart_Norm.ObjectiveSeries)
                {
                    objectiveSeries.SetBinding(LineSeries.TitleProperty, new Binding { Source = ObjectiveNames[i] });
                    ((LineSeries)this.ObjectiveChart_Abs.ObjectiveSeries[i]).SetBinding(LineSeries.TitleProperty, new Binding { Source = ObjectiveNames[i] });

                    i++;
                }
            }

            this.ObjIndex = index;
        }

        //UPDATE EVOLUTION DATA
        public void UpdateEvolutionData(List<List<double>> GradientData, DateTime start)
        {
            //Update objective evolution
            int i = 0;
            foreach (ChartValues<double> objective in this.ObjectiveEvolution_Abs)
            {
                double value = this.Design.Objectives[i];
                objective.Add(value);

                double normalized = value / Math.Abs(objective[0]);
                this.ObjectiveEvolution_Norm[i].Add(normalized);

                i++;
            }

            //Store corresponding variable values for potential reset
            i = 0;
            foreach (VarVM var in this.Variables)
            {
                this.VariableEvolution[i].Add(var.Value);
                i++;
            }


            //Store gradient data for export csv
            for (int j=0; j<this.Objectives.Count; j++)
            {
                for (int k = 0; k < this.Variables.Count(); k++)
                {
                    var subList = this.GradientEvolution[j][k];

                    if (GradientData.Any())
                        subList.Add(GradientData[j][k]);
                    else
                    {
                        subList.Add(null);
                    }                        
                }
            }

            //Rescale X-Axis every 10 steps for appearance
            FirePropertyChanged("NumSteps");
            this.ObjectiveChart_Norm.XAxisSteps = this.NumSteps / 10 + 1;
            this.ObjectiveChart_Abs.XAxisSteps = this.NumSteps / 10 + 1;

            this.TimeEvolution.Add(DateTime.Now.Subtract(start));
        }

        //OPTIMIZE
        public void Optimize(StepperOptimizer.Direction dir, List<List<double>> GradientData, StepperOptimizer optimizer)
        {
            //StepperOptimizer optimizer = new StepperOptimizer(this.Design, this.ObjIndex, dir, this.StepSize);
            optimizer.ConvertFromCalculatorToOptimizer(this.ObjIndex, dir, this.StepSize);

            DateTime start = DateTime.Now;

            optimizer.Optimize(GradientData);

            //Update variable values at the end of the optimization
            foreach (GroupVarVM var in this.GroupVars)
                var.OptimizationFinished();
            foreach (List<VarVM> geo in this.GeoVars)
                foreach (VarVM var in geo)
                    var.OptimizationFinished();

            foreach (IDesignGeometry geo in this.Design.Geometries)
            {
                geo.Update();
            }

            //optimizer.DownStreamExpire();
            //Grasshopper.Instances.ActiveCanvas.Document.ExpirePreview(false);
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false, Grasshopper.Kernel.GH_SolutionMode.Silent);
            this.UpdateEvolutionData(GradientData, start);
        }

        //RESET
        //Allow user to return to previous step systems
        public void Reset()
        {
            bool finished = false;
            var step = this.TrackedStep;

            System.Action run = delegate ()
            {
                int i = 0;
                foreach (VarVM var in this.Variables)
                {
                    //var.Value = this.VariableEvolution[i][step];

                    this.Variables[i].Value = this.VariableEvolution[i][step];
                    i++;
                }

                foreach (IDesignGeometry geo in this.Design.Geometries)
                {
                    geo.Update();
                }


                if (this.Design.Geometries.Any())
                {
                    Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true, GH_SolutionMode.Default);
                    finished = true;
                }
                else
                {
                    Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false, GH_SolutionMode.Default);
                    finished = true;
                }

                //this.Design.UpdateComponentOutputs(new List<List<double>>());
            };
            Rhino.RhinoApp.MainApplicationWindow.Invoke(run);
            while (!finished)
            {
            }

            //this.UpdateEvolutionData(new List<List<double>>());
        }

        public bool IsoperformancePossible()
        {
            if (Design.ActiveVariables.Count > Design.Objectives.Count && Design.ActiveVariables.Count >= 2)
            {
                return true; 
            }
            return false; 
        }

        private Visibility _filepatherrorvisibility;
        public Visibility FilePathErrorVisibility
        {
            get { return this._filepatherrorvisibility; }
            set { CheckPropertyChanged<Visibility>("FilePathErrorVisibility", ref _filepatherrorvisibility, ref value); }
        }

        //EXPORT CSV LOG
        //Formats and exports all data for readability
        public void ExportCSV_Log(string filename)
        {
            var ObjData = this.ObjectiveEvolution_Abs;
            var VarData = this.VariableEvolution;
            var GradData = this.GradientEvolution;

            var numObjs = ObjData.Count;
            var numVars = VarData.Count;

            string output = "";

            //There's a lot of data to be exported
            //Construct some headers in the csv so things make sense in excel
            //These rows can always be deleted in post-processing
            #region Construct Headers
            //First row is only Gradients header
            for (int i = 0; i < (numObjs + numVars + 2); i++)
            {
                output += ",";
            }
            output += "Gradients" + "\r\n";

            //Second row has Objective and Variable headers
            string line = "Objectives,";
            for (int i = 0; i < numObjs; i++)
            {
                line += ",";
            }

            line += "Variables,";
            for (int i = 0; i < numVars; i++)
            {
                line += ",";
            }

            for (int i = 0; i < numObjs; i++)
            {
                line += this.ObjectiveNames[i] + ",";

                for (int j = 0; j < numVars; j++)
                {
                    line += ",";
                }
            }

            line += "Times, ";

            output += line + "\r\n";

            //Third row has Objective and Variable name headers
            line = "";
            for (int i = 0; i < numObjs; i++)
            {
                line += this.ObjectiveNames[i] + ",";
            }

            line += ",";
            for (int i = 0; i < numObjs+1; i++)
            {
                foreach (VarVM var in this.Variables)
                {
                    line += var.Name + ",";
                }

                line += ",";
            }
            output += line + "\r\n";
            #endregion

            #region Add Data
            for (int i=0; i<this.NumSteps+1; i++)
            {
                line = "";
                for (int j= 0; j<numObjs; j++)
                {
                    line += this.ObjectiveEvolution_Abs[j][i] + ",";
                }

                line += ",";
                for (int j = 0; j < numVars; j++)
                {
                    line += this.VariableEvolution[j][i] + ",";
                }

                line += ",";

                if (i<this.GradientEvolution[0][0].Count && this.GradientEvolution[0][0][i] != null)
                {
                    for (int j = 0; j < numObjs; j++)
                    {
                        for (int k = 0; k < numVars; k++)
                            line += this.GradientEvolution[j][k][i] + ",";
                        line += ",";
                    }
                    
                }
                if (i == this.NumSteps)
                {
                    for (int j = 0; j < numObjs; j++)
                    {
                        for (int k = 0; k < numVars; k++)
                            line += ",";
                        line += ",";
                    }
                }

                line += this.TimeEvolution[i].ToString("G");
                line += ",";

                output += line + "\r\n";
            }
            #endregion 

            System.IO.StreamWriter file = new System.IO.StreamWriter(filename + "_log.csv");
            file.Write(output);
            file.Close();
        }

        //EXPORT CSV RAW
        //Formats and exports only variable and objective data for easy processing
        public void ExportCSV_Raw(string filename)
        {
            var ObjData = this.ObjectiveEvolution_Abs;
            var VarData = this.VariableEvolution;
            var GradData = this.GradientEvolution;

            var numObjs = ObjData.Count;
            var numVars = VarData.Count;

            string output = "";

            #region Add Data
            string line;
            for (int i = 0; i < this.NumSteps + 1; i++)
            {
                line = "";
                for (int j = 0; j < numVars; j++)
                {
                    line += this.VariableEvolution[j][i] + ",";
                }

                for (int j = 0; j < numObjs; j++)
                {
                    line += this.ObjectiveEvolution_Abs[j][i] + ",";
                }

                output += line + "\r\n";
            }
            #endregion
            
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"" + filename + "_raw.csv");
            file.Write(output);
            file.Close();
        }

        //ON WINDOW CLOSING
        //Alert the component that the window has been closed
        //(and therefore a new window can open on double click)
        public void OnWindowClosing()
        {
            this.Component.IsWindowOpen = false;
        }
    }
}
