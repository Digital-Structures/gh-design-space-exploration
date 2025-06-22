using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radical.Integration;
using NLoptNet;
using System.Windows.Markup;
using System.Windows.Data;
using LiveCharts;
using System.Windows;
using DSOptimization;

namespace Radical
{
    public class RadicalVM : BaseVM, IOptimizeToolVM
    {
        public List<GroupVarVM> GroupVars { get; set; }
        public DSOptimizerComponent Component { get; set; }
        public Design Design { get; set; }
        public List<ConstVM> Constraints { get; set; }
        public List<VarVM> NumVars { get; set; }
        public List<List<VarVM>> GeoVars { get; set; }
        public Dictionary<string, List<GraphVM>> Graphs { get; set; }
        public enum Direction { X, Y, Z }

        //EVOLUTIONS for all objectives and constraints 
        public ChartValues<double> ObjectiveEvolution { get; set; }
        public ChartValues<ChartValues<double>> ConstraintsEvolution { get; set; }
        public List<List<double>> NumVarEvolution { get; set; }
        public List<TimeSpan> TimeEvolution;
        public TimeSpan TotalRunTime;

        public double OriginalObjectiveValue { get; set; }
        public double SmallestObjectiveValue { get; set; }

        public RadicalVM()
        {
        }

        public RadicalVM(DSOptimizerComponent component)
        {
            this.Component = component;
        }

        //CONSTRUCTOR
        public RadicalVM(Design design, DSOptimizerComponent component)
        {
            this.Component = component;
            this.Design = design;

            //Warn user that system can't handle multiple objectives
            this.opendialog = false;
            if (this.Design.Objectives.Count>1)
                this.OpenDialog = true;

            this.Constraints = new List<ConstVM>();

            this.ObjectiveEvolution = new ChartValues<double>();
            this.ObjectiveEvolution.Add(this.Design.Objectives[0]);
            this.ConstraintsEvolution = new ChartValues<ChartValues<double>>();
            foreach (Constraint c in this.Design.Constraints)
            {
                ChartValues<double> cv = new ChartValues<double>();
                cv.Add(c.CurrentValue);
                this.ConstraintsEvolution.Add(cv);
            }
            this.NumVarEvolution = new List<List<double>>();
            foreach(IVariable v in this.Design.ActiveVariables)
            {
                List<double> list_var = new List<double>();
                list_var.Add(v.CurrentValue);
                this.NumVarEvolution.Add(list_var);
            }

            this.TimeEvolution = new List<TimeSpan>();
            this.TimeEvolution.Add(new TimeSpan(0,0,-10)); //make first timespan negative because no time has passed, this will not be returned in csv file

            this.Graphs = new Dictionary<string, List<GraphVM>>();
            this.Graphs.Add("Main", new List<GraphVM>());
            this.Graphs.Add("Constraints", new List<GraphVM>());
            SetUpGraphs();

            this.NumVars = new List<VarVM> { };
            this.GeoVars = new List<List<VarVM>> { };
            this.GroupVars = new List<GroupVarVM> { };
            SortVariables();

            this.OptRunning = false;
            this.OptRunning = false;
            this._advancedOptions = false;
            this._disablingNotAllowed = false; 

            this.OriginalObjectiveValue = this.Design.Objectives.ElementAt(0);
            this.SmallestObjectiveValue = this.Design.Objectives.ElementAt(0);
        }

        //OPEN DIALOG
        //Boolean to notify the user if he's using multiple objectives
        private bool opendialog;
        public virtual bool OpenDialog
        {
            get { return this.opendialog; }
            set { CheckPropertyChanged<bool>("OpenDialog", ref opendialog, ref value); }
        }

        //ACTIVE GRAPHS
        //A list of active constraint graphs to populate the window graph grid
        public List<GraphVM> ActiveGraphs
        {
            get
            {
                List<GraphVM> list = new List<GraphVM>();
                list.Add(this.Graphs["Main"][0]);

                foreach (ConstVM c in Constraints.Where(c => c.IsActive))
                    list.Add(c.GraphVM);

                return list;
            }
        }

        //SET UP GRAPHS
        public void SetUpGraphs()
        {
            GraphVM main = new GraphVM(this.ObjectiveEvolution, "Objective");
            this.Graphs["Main"].Add(main);

            for (int i = 0; i < Design.Constraints.Count; i++)
            {
                GraphVM gvm = new GraphVM(ConstraintsEvolution[i], String.Format("C{0}", i));
                this.Graphs["Constraints"].Add(gvm);
                this.Constraints.Add(new ConstVM(Design.Constraints[i], gvm));
            }

            UpdateCurrentScoreDisplay();
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

        //OPTIMIZE
        public void Optimize(RadicalWindow radicalWindow)
        {
            RadicalOptimizer opt = new RadicalOptimizer(this.Design, radicalWindow);
            DateTime start = DateTime.Now;

            opt.RunOptimization();

            DateTime end = DateTime.Now;
            TotalRunTime = end - start;
        }

        //OPTIMIZATION STARTED
        //Disable changes to all optimization variables and constraints
        public void OptimizationStarted()
        {
            this.ChangesEnabled = false;

            foreach (ConstVM constraint in this.Constraints)
                constraint.ChangesEnabled = false;

            this.Graphs["Main"][0].OptimizerDone = false;
            this.Graphs["Main"][0].ChartLineVisibility = Visibility.Collapsed;
            foreach (GraphVM g in this.Graphs["Constraints"])
            {
                g.OptimizerDone = false;
                g.ChartLineVisibility = Visibility.Collapsed;
            }
        }

        //OPTIMIZATION FINISHED
        //Enable changes to all optimization variables and constraints
        public void OptimizationFinished()
        {
            this.ChangesEnabled = true;

            foreach (ConstVM constraint in this.Constraints)
                constraint.OptimizationFinished();

            this.Graphs["Main"][0].OptimizerDone = true;
            //this.Graphs["Main"][0].ChartLineVisibility = Visibility.Visible;
            foreach (GraphVM g in this.Graphs["Constraints"])
            {
                g.OptimizerDone = true;
                //g.ChartLineVisibility = Visibility.Visible;
            }
        }

        public void UpdateGraphLines(int iteration)
        {
            foreach (GraphVM graph in this.ActiveGraphs)
            {
                graph.MouseIteration = iteration;
                if (graph.DisplayLine())
                    graph.ChartLineVisibility = Visibility.Visible;
                else
                    graph.ChartLineVisibility = Visibility.Collapsed;
            }
        }

        public void UpdateCurrentScoreDisplay()
        {
            double objective = this.Design.Objectives[0];
            this.Graphs["Main"][0].FinalOptimizedValue = objective;

            for (int i = 0; i < this.Graphs["Constraints"].Count; i++)
            {
                double score = this.Design.Constraints[i].CurrentValue;
                this.Graphs["Constraints"][i].FinalOptimizedValue = score;
            }
        }

        //If ObjectiveEvolution has the same amount of objects as the max value on the x axis then the x-axis display
        //will be automated to display all values in objective evolution
        public void AutomateStepSize(bool Force)
        {
            if (ObjectiveEvolution.Count == this.Graphs["Main"][0].MaxXAxis || Force)
            {
                this.Graphs["Main"][0].XAxisStep = double.NaN;
                this.Graphs["Main"][0].MaxXAxis = double.NaN;
                foreach (GraphVM g in this.Graphs["Constraints"])
                {
                    g.XAxisStep = double.NaN;
                    g.MaxXAxis = double.NaN;
                }
            }
        }

        public void ResetObjective()
        {
            this.SmallestObjectiveValue = this.OriginalObjectiveValue;
        }

        public void ClearGraphs()
        {
            GraphVM MainGVM = this.Graphs["Main"][0];
            int max = MainGVM.DefaultMaxXAxis;

            MainGVM.ChartValues.Clear();
            MainGVM.ChartValues.Add(this.Design.Objectives[0]);
            MainGVM.XAxisStep = 1;
            MainGVM.MaxXAxis = max;
            MainGVM.ChartLineVisibility = Visibility.Collapsed;

            for (int i = 0; i < this.Graphs["Constraints"].Count; i++)
            {
                GraphVM ConstraintGVM = this.Graphs["Constraints"].ElementAt(i);
                ConstraintGVM.ChartValues.Clear();
                ConstraintGVM.ChartValues.Add(this.Design.Constraints.ElementAt(i).CurrentValue);
                ConstraintGVM.XAxisStep = 1;
                ConstraintGVM.MaxXAxis = max;
                ConstraintGVM.ChartLineVisibility = Visibility.Collapsed;
            }
        }

        //Believe that this is no longer in use
        //ON WINDOW CLOSING
        //Alert the component that the window has been closed
        //(and therefore a new window can open on double click)
        //public void OnWindowClosing()
        //{
        //    this.Component.IsWindowOpen = false;
        //}

        //GRAPH COLUMNS
        private int _cols;
        public int Cols
        {
            get
            {
                return _cols;
            }
            set
            {
                if(CheckPropertyChanged<int>("Cols", ref _cols, ref value))
                {
                }
            }
        }

        //REFRESH MODE
        private RefreshMode _mode;
        public RefreshMode Mode
        {
            get
            { return _mode; }
            set
            {
                if (CheckPropertyChanged<RefreshMode>("Mode", ref _mode, ref value))
                {
                }
            }
        }

        //NUMBER OF ITERATIONS
        private int _niterations;
        public int Niterations
        {
            get
            { return _niterations; }
            set
            {
                if (CheckPropertyChanged<int>("Niterations", ref _niterations, ref value))
                {
                }
            }

        }

        //CONVERGENCE
        private double _convcrit;
        public double ConvCrit
        {
            get
            { return _convcrit; }
            set
            {
                if (CheckPropertyChanged<double>("ConvCrit", ref _convcrit, ref value))
                {
                }
            }
        }

        #region Algorithms 
        //PRIMARY ALGORITHM
        private NLoptAlgorithm _primaryalgorithm;
        public NLoptAlgorithm PrimaryAlgorithm
        {
            get
            { return _primaryalgorithm; }
            set
            {
                if (CheckPropertyChanged<NLoptAlgorithm>("PrimaryAlgorithm", ref _primaryalgorithm, ref value))
                {
                }
            }
        }

        //SECONDARY ALGORITHM
        private NLoptAlgorithm _secondaryalgorithm;
        public NLoptAlgorithm SecondaryAlgorithm
        {
            get
            { return _secondaryalgorithm; }
            set
            {
                if (CheckPropertyChanged<NLoptAlgorithm>("SecondaryAlgorithm", ref _secondaryalgorithm, ref value))
                {
                }
            }
        }

        //AVAILABLE ALGORITHMS
        public List<NLoptAlgorithm> AvailableAlgs
        {
            get
            {
                if (AdvancedOptions)
                {
                    if (this.Constraints.Any())
                    {
                        return DFreeAlgs_INEQ.ToList();
                    }
                    else
                    {
                        return DFreeAlgs.ToList();
                    }
                }
                else
                {
                    if (this.Constraints.Any())
                    {
                        return BasicAlgs_INEQ.ToList();
                    }
                    else
                    {
                        return BasicAlgs.ToList();
                    }
                }
                
            }
        }

        //AVAILABLE SECONDARY ALGORITHMS
        public List<NLoptAlgorithm> AvailableSecondaryAlgs
        {
            get
            {
                return DFreeAlgs_Secondary.ToList();
            }
        }

        //ADVANCED OPTIONS
        private bool _advancedOptions;
        public bool AdvancedOptions
        {
            get { return _advancedOptions; }
            set
            {
                if (CheckPropertyChanged<bool>("AdvancedOptions", ref _advancedOptions, ref value))
                {
                    FirePropertyChanged("AvailableAlgs");
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

        public IEnumerable<NLoptAlgorithm> BasicAlgs = new[]
        {
            NLoptAlgorithm.AUGLAG, //Calls for secondary alg
            NLoptAlgorithm.LN_COBYLA,
        };

        public IEnumerable<NLoptAlgorithm> BasicAlgs_EQ = new[]
        {
            NLoptAlgorithm.LN_COBYLA
        };

        public IEnumerable<NLoptAlgorithm> BasicAlgs_INEQ = new[]
        {
            NLoptAlgorithm.LN_COBYLA
        };

        //List of those that require secondary and the secondary options remain the same. 

        public IEnumerable<NLoptAlgorithm> DFreeAlgs = new[]
        {
            NLoptAlgorithm.AUGLAG, //Calls for secondary alg
            NLoptAlgorithm.AUGLAG_EQ, //Calls for secondary alg
            NLoptAlgorithm.GN_CRS2_LM,
            NLoptAlgorithm.GN_DIRECT,
            NLoptAlgorithm.GN_DIRECT_L,
            NLoptAlgorithm.GN_DIRECT_L_NOSCAL,
            NLoptAlgorithm.GN_DIRECT_L_RAND,
            NLoptAlgorithm.GN_DIRECT_L_RAND_NOSCAL,
            NLoptAlgorithm.GN_DIRECT_NOSCAL,
            NLoptAlgorithm.GN_ESCH,
            NLoptAlgorithm.GN_ISRES,
            NLoptAlgorithm.GN_ORIG_DIRECT,
            NLoptAlgorithm.GN_ORIG_DIRECT_L,
            NLoptAlgorithm.G_MLSL, //calls for secondary alg (local)
            NLoptAlgorithm.G_MLSL_LDS, //calls for secondary alg (local)
            NLoptAlgorithm.LN_BOBYQA,
            NLoptAlgorithm.LN_COBYLA,
            //NLoptAlgorithm.LN_NELDERMEAD, // supersed by SBPLX
            //NLoptAlgorithm.LN_NEWUOA_BOUND,// superseded by BOBYQA
            //NLoptAlgorithm.LN_PRAXIS, // COBYLA OR BOBYQA better
            NLoptAlgorithm.LN_SBPLX
        };

        public IEnumerable<NLoptAlgorithm> DFreeAlgs_EQ = new[]
        {
            NLoptAlgorithm.AUGLAG, //Calls for secondary alg
            NLoptAlgorithm.AUGLAG_EQ, //Calls for secondary alg
            NLoptAlgorithm.GN_ISRES,
            NLoptAlgorithm.LN_COBYLA
        };

        public IEnumerable<NLoptAlgorithm> DFreeAlgs_INEQ = new[]
        {
            NLoptAlgorithm.AUGLAG, //Calls for secondary alg
            NLoptAlgorithm.AUGLAG_EQ, //Calls for secondary algorithm that can handle inequality constraints
            NLoptAlgorithm.GN_ISRES,
            NLoptAlgorithm.GN_ORIG_DIRECT,
            NLoptAlgorithm.GN_ORIG_DIRECT_L,
            NLoptAlgorithm.LN_COBYLA
        };

        //Algorithms that require a secondary algorithm 
        public IEnumerable<NLoptAlgorithm> DFreeAlgs_ReqSec = new[]
        {
            NLoptAlgorithm.AUGLAG, //Calls for secondary alg
            NLoptAlgorithm.AUGLAG_EQ, //Calls for secondary alg
            NLoptAlgorithm.G_MLSL, //calls for secondary alg (local)
            NLoptAlgorithm.G_MLSL_LDS, //calls for secondary alg (local)
        };

        public IEnumerable<NLoptAlgorithm> DFreeAlgs_Secondary = new[]
        {
            NLoptAlgorithm.LN_BOBYQA,
            NLoptAlgorithm.LN_COBYLA,
        };
        #endregion

        #region File Writing

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
            var ObjData = this.ObjectiveEvolution;
            var ConData = this.ConstraintsEvolution;
            var NumVarData = this.NumVarEvolution;

            var numObjs = ObjData.Count;
            var numCons = ConData.Count;
            var numVars = NumVarData.Count;

            string output = "";

            //There's a lot of data to be exported
            //Construct some headers in the csv so things make sense in excel
            //These rows can always be deleted in post-processing
            #region Construct Headers

            //First row has Objective, Constraints, and Variable headers
            string line = "Objectives,";

            if(numCons != 0)
            {
                line += ","; //extra break
                line += "Constraints,";
                for (int i = 0; i < numCons; i++)
                {
                    line += ",";
                }
            }

            line += ","; //extra break

            line += "Variables,";
            for (int i = 0; i < numVars; i++)
            {
                line += ",";
            }

            //line += ","; //extra break
            line += "Time Spans,";

            line += ","; //extra break
            line += "Total Calculation Time";

            output += line + "\r\n";

            //Second row has name headers
            line = "";
            line += "Objective,"; //we shall have it a generic name


            if (numCons != 0)
            {
                line += ",";
                foreach (ConstVM c in this.Constraints)
                {
                    line += c.Name + ",";
                }
            }


            line += ",";
            foreach (VarVM var in this.NumVars)
            {
                line += var.Name + ",";
            }

            line += ",";
            line += ",";
            line += ",";
            line += TotalRunTime.ToString("G");

            output += line + "\r\n";
            #endregion

            #region Add Data
            //should be number of iterations
            line = "";
            for (int i = 0; i < this.ObjectiveEvolution.Count; i++)
            {
                line = "";
                line += this.ObjectiveEvolution[i] + ",";
                
                if(numCons != 0)
                {
                    line += ",";
                    for (int j = 0; j < numCons; j++)
                    {
                        line += this.ConstraintsEvolution[j][i] + ",";
                    }
                }

                line += ",";
                for (int j = 0; j < numVars; j++)
                {
                    line += this.NumVarEvolution[j][i] + ",";
                }

                line += ",";
                if (this.TimeEvolution[i] > TimeSpan.Zero)
                {
                    line += this.TimeEvolution[i].ToString("G");
                }

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
            var ObjData = this.ObjectiveEvolution;
            var VarData = this.NumVarEvolution;

            var numObjs = ObjData.Count;
            var numVars = VarData.Count;

            string output = "";

            #region Add Data
            string line;
            for (int i = 0; i < this.ObjectiveEvolution.Count; i++)
            {
                line = "";
                for (int j = 0; j < numVars; j++)
                {
                    line += this.NumVarEvolution[j][i] + ",";
                }

                line += this.ObjectiveEvolution[i] + ",";

                output += line + "\r\n";
            }
            #endregion

            System.IO.StreamWriter file = new System.IO.StreamWriter(@"" + filename + "_raw.csv");
            file.Write(output);
            file.Close();
        }
        #endregion
    }
}
