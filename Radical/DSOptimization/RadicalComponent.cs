using System;
using System.Collections.Generic;
using Radical;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using Radical.Components;
using Radical.Integration;
using System.Threading;
using Grasshopper.Kernel.Special;

namespace DSOptimization
{ 
    public class RadicalComponent : GH_Component
    {
        public List<double> Objectives { get; set; }
        public List<double> Constraints { get; set; }
        public List<double> NumVariables { get; set; }
        public List<NurbsSurface> SrfVariables { get; set; }
        public List<NurbsCurve> CrvVariables { get; set; }

        public IList<IGH_Param> NumObjects{ get { return this.Params.Input[2].Sources; } }
        public IList<IGH_Param> CrvObjects { get { return this.Params.Input[3].Sources; } }
        public IList<IGH_Param> SrfObjects { get { return this.Params.Input[4].Sources; } }

        private DataTree<double> ObjectiveHistory;
        private DataTree<double> VariableHistory;
        private DataTree<double?> GradientHistory;

        public int numVars;

        //Checks to see if there is an objective and that at least one variable is connected (can change to make it so that only one variable is connected)
        public bool InputsSatisfied { get; set; }

        /// <summary>
        /// Initializes a new instance of the RadicalComponent class.
        /// </summary>
        public RadicalComponent()
          : base("Radical", "Radical",
              "Optimization component featuring Radical",
              "DSE", "Optimize")
        {
            this.Objectives = new List<double>();
            this.ObjectiveHistory = new DataTree<double>();

            this.NumVariables = new List<double>();
            this.SrfVariables = new List<NurbsSurface>();
            this.CrvVariables = new List<NurbsCurve>();
            this.VariableHistory = new DataTree<double>();

            this.GradientHistory = new DataTree<double?>();

            this.Constraints = new List<double>();

            this.open = false; //Is window open
            this.InputsSatisfied = false;

            //this.NumObjects = new List<IGH_ActiveObject>();
        }

        //Determine whether there is already a Radical window open
        private bool open;
        public bool IsWindowOpen
        {
            get { return this.open; }
            set { this.open = value; }
        }

        public override void CreateAttributes()
        {
            base.m_attributes = new DSOptimizationComponentAttributes(this);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Objective", "O", "Objective to Minimize", GH_ParamAccess.list);
            pManager.AddNumberParameter("Constraints", "C", "Optimization Constraints", GH_ParamAccess.list);
            pManager.AddNumberParameter("Numerical Variables", "numVar", "Numerical Optimization Variables", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("Variable Surfaces", "srfVar", "Geometrical Optimization Variables (Surfaces)", GH_ParamAccess.list);
            pManager.AddCurveParameter("Variable Curves", "crvVar", "Geometrical Optimization Variables (Curves)", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Objective Evolution", "Obj", "Objective value history", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Variable Evolution", "Var", "Variable value history", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Gradient Evolution", "Grad", "Gradient value history", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// Stores variables to be accessed later from the Window thread
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //assign objective
            List<double> obj = new List<double>();
            if (!DA.GetDataList(0, obj))
            {
                this.InputsSatisfied = false; 
                return;
            }
            this.Objectives = obj;

            if (Params.Input[0].Sources.Count > 0 && this.Objectives.Count == 0)
            {
                this.InputsSatisfied = false;
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One or more objectives are invalid");
                return;
            }

            //assign constraints
            List<double> constraints = new List<double>();
            DA.GetDataList(1, constraints);
            this.Constraints = constraints;

            //assign numerical variables
            List<double> variables = new List<double>();
            DA.GetDataList(2, variables);
            this.NumVariables = variables;

            //Check if inputs are valid type by checking GUID, might not actually work between diff computers and different versions of GH
            Guid numGuid = new Guid("57da07bd-ecab-415d-9d86-af36d7073abc");
            foreach (IGH_Param param in this.Params.Input[2].Sources)
            {
                if(param.Name != "Number Slider")
                {
                    this.InputsSatisfied = false;
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One or more NumVars are invalid");
                    return;
                }
            }

            //assign surface variables
            List<Surface> surfaces= new List<Surface>();
            DA.GetDataList(3, surfaces);
            if (Params.Input[3].Sources.Count > 0 && surfaces.Count == 0)
            {
                this.InputsSatisfied = false;
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One or more surfaces are invalid");
                return;
            }
            foreach (Surface s in surfaces)
            {
                if (s.UserData == null)
                {
                    this.InputsSatisfied = false;
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One or more surfaces are invalid");
                    return;
                }
            }
            this.SrfVariables = surfaces.Select(x => x.ToNurbsSurface()).ToList();

            //assign curve variables
            List<Curve> curves = new List<Curve>();
            DA.GetDataList(4, curves);
            if (Params.Input[4].Sources.Count > 0 && curves.Count == 0)
            {
                this.InputsSatisfied = false;
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One or more curves are invalid");
                return;
            }
            foreach (Curve c in curves)
            {
                if (c == null)
                {
                    this.InputsSatisfied = false;
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One or more curves are invalid");
                    return;
                }
            }
            this.CrvVariables = curves.Select(x => x.ToNurbsCurve()).ToList();

            if(NumVariables.Count + SrfVariables.Count + CrvVariables.Count == 0)
            {
                this.InputsSatisfied = false;
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "You have no inputs");
                return;
            }

            this.InputsSatisfied = true;

            DA.SetDataTree(0, this.ObjectiveHistory);
            DA.SetDataTree(1, this.VariableHistory);
            DA.SetDataTree(2, this.GradientHistory);
        }


        //I DONT THINK THIS STUFF IS CURRENTLY BEING USED BY RADICAL
        #region Update Ouput Data
        //Takes a list of all objective values for a step
        //Appends values to the output tree
        public void AppendToObjectives(List<double> values)
        {
            int i = 0;
            foreach (double obj in values)
            {
                GH_Path path = new GH_Path(i);
                this.ObjectiveHistory.Add(obj, path);

                i++;
            }
        }

        public void AppendToVariables (List<double> values)
        {
            int i = 0;
            foreach (double var in values)
            {
                GH_Path path = new GH_Path(i);
                this.VariableHistory.Add(var, path);

                i++;
            }
        }

        public void AppendToGradients (List<List<double>> values)
        { 
            int i = 0;
            foreach (double obj in this.Objectives)
            {
                for (int j= 0; j < this.numVars; j++)
                {
                    GH_Path path = new GH_Path(i,j);

                    if (values.Any())
                        this.GradientHistory.Add(values[i][j], path);
                    else
                        this.GradientHistory.Add(null, path);
                }
                i++;
            }
        }
        #endregion

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Radical.Properties.Resources.DSOpt2;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("65ec1771-a3e9-4cba-946e-c7b3ed26d98a"); }
        }
    }


    public class DSOptimizationComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        // custom attribute to override double click mouse event on component and open a WPF window

        RadicalComponent MyComponent;

        public DSOptimizationComponentAttributes(IGH_Component component) : base(component)
        {
            MyComponent = (RadicalComponent)component;
        }

        //[STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            //Prevent opening of multiple windows at once
            if (!MyComponent.IsWindowOpen && MyComponent.InputsSatisfied)
            {
                MyComponent.IsWindowOpen = true;

                Design design = new Design(MyComponent);

                Thread viewerThread = new Thread(delegate ()
                {
                    RadicalVM radVM = new RadicalVM(design, this.MyComponent);
                    Window viewer = new RadicalWindow(radVM);
                    viewer.Show();
                    System.Windows.Threading.Dispatcher.Run();
                });

                viewerThread.SetApartmentState(ApartmentState.STA); // needs to be STA or throws exception
                viewerThread.Start();
            }
            return base.RespondToMouseDoubleClick(sender, e);
        }

    }
}