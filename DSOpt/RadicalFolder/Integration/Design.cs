using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Radical.Components;
using Radical.TestComponents;
using LiveCharts;
using NLoptNet;
using LiveCharts.Helpers;
using LiveCharts.Wpf;


namespace Radical.Integration
{
    //DESIGN
    //Collection of problem variables, constraints, and objectives to be optimized by Radical
    public class Design : IDesign
    {
        public DSOptimizerComponent MyComponent { get; set; }

                    //Move Score and ConstraintEvolution 
                    public ChartValues<double> ScoreEvolution { get; set; }
                    public ChartValues<ChartValues<double>> ConstraintEvolution { get; set; }

                    //OPTIMIZE for Radical (Should be moved to RadicalVM)
                    //Runs the optimizer and stores the objective data
                    public void Optimize(RadicalWindow radicalWindow)
                    {
                        Optimizer opt = new Optimizer(this, radicalWindow);
                        opt.RunOptimization();
                    }

                    //public void Optimize()
                    //{
                    //    Optimizer opt = new Optimizer(this);
                    //    opt.RunOptimization();
                    //    this.OptComponent.Evolution = this.ScoreEvolution.ToList();
                    //    Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
                    //}

        //INPUT PROPERTIES
        public List<IVariable> Variables { get; set; }
        public List<IDesignGeometry> Geometries { get; set; }
        public List<Constraint> Constraints { get; set; }

        //OBJECTIVES
        public List<double> Objectives { get { return MyComponent.Objectives; } }

        //ACTIVE VARIABLES
        public List<IVariable> ActiveVariables { get { return Variables.Where(var => var.IsActive).ToList(); } }


        public Design(DSOptimizerComponent component)
        {
            //Access the component
            this.MyComponent = component;

            this.Variables = new List<IVariable>();
            this.Geometries = new List<IDesignGeometry>();
            this.Constraints = new List<Constraint>();

            // ADD VARIABLES
            //Sliders
            foreach (IGH_Param param in component.Params.Input[2].Sources)
            {
                this.Variables.Add(new SliderVariable(param));
            }
            //Curves
            for (int i = 0; i < component.Params.Input[3].Sources.Count; i++)
            {
                IGH_Param param = component.Params.Input[3].Sources[i];
                NurbsSurface surf = component.SrfVariables[i];
                Geometries.Add(new DesignSurface(param, surf));
            }
            //Surfaces
            for (int i = 0; i < component.Params.Input[4].Sources.Count; i++)
            {
                IGH_Param param = component.Params.Input[4].Sources[i];
                NurbsCurve surf = component.CrvVariables[i];
                this.Geometries.Add(new DesignCurve(param, surf));
            }

            // ADD CONSTRAINTS
            for (int i = 0; i < component.Constraints.Count; i++)
            {
                this.Constraints.Add(new Constraint(component, ConstraintType.morethan, i));
            }
        }

        //SAMPLE
        //public void Sample(int alg)
        //{

        //    Sampler.ISamplingAlg samplingAlg;
        //    switch (alg)
        //    {
        //        case 0:
        //            samplingAlg = new Sampler.GSampling();
        //            break;
        //        case 1:
        //            samplingAlg = new Sampler.RUSampling();
        //            break;
        //        case 2:
        //            samplingAlg = new Sampler.LHSampling();
        //            break;
        //        default:
        //            samplingAlg = new Sampler.LHSampling();
        //            break;
        //    }
        //    Sampler sam = new Sampler(this, samplingAlg, ExpComponent.nSamples);
        //    sam.RunSampling();
        //}
    }
}