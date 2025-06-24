using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using LiveCharts;
using NLoptNet;
using LiveCharts.Helpers;
using LiveCharts.Wpf;

namespace DSOptimization
{
    //DESIGN
    //Collection of problem variables, constraints, and objectives to be optimized by Radical
    public class Design
    {
        public DSOptimizerComponent MyComponent { get; set; }

        //INPUT PROPERTIES
        public List<IVariable> Variables { get; set; }
        public List<IDesignGeometry> Geometries { get; set; }
        public List<Constraint> Constraints { get; set; }

        //OBJECTIVES
        public List<double> Objectives{ get { return MyComponent.Objectives; } }

        //ACTIVE VARIABLES
        public List<IVariable> ActiveVariables{ get { return Variables.Where(var => var.IsActive).ToList(); } }

        //CONSTRUCTOR
        public Design(DSOptimizerComponent component)
        {
            //Access the component
            this.MyComponent = component;

            this.Variables = new List<IVariable>();
            this.Geometries = new List<IDesignGeometry>();
            this.Constraints = new List<Constraint>();

            // ADD VARIABLES
            //Sliders
            foreach (IGH_Param param in MyComponent.Params.Input[2].Sources)
            {
                SliderVariable s = new SliderVariable(param);
                if(s.CurrentValue == 0)
                {
                    if(s.Max >= 0.001)
                    {
                        s.UpdateValue(0.001);
                    }
                    else
                    {
                        s.UpdateValue(-0.001);
                    }
                }
                this.Variables.Add(new SliderVariable(param));
            }
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false, Grasshopper.Kernel.GH_SolutionMode.Silent);

            //Surfaces
            for (int i = 0; i < MyComponent.Params.Input[3].Sources.Count; i++)
            {
                IGH_Param param = MyComponent.Params.Input[3].Sources[i];
                NurbsSurface surf = MyComponent.SrfVariables[i];
                Geometries.Add(new DesignSurface(param, surf));
            }
            //Curves
            for (int i = 0; i < MyComponent.Params.Input[4].Sources.Count; i++)
            {
                IGH_Param param = MyComponent.Params.Input[4].Sources[i];
                NurbsCurve curv = MyComponent.CrvVariables[i];
                this.Geometries.Add(new DesignCurve(param, curv));
            }

            // Add geometries to variables list 
            // not the cleanest way to do it, review code structure
            if (Geometries.Any()) { this.Variables.AddRange(Geometries.Select(x => x.Variables).SelectMany(x => x).ToList()); } 

            // ADD CONSTRAINTS
            for (int i = 0; i < component.Constraints.Count; i++)
            {
                this.Constraints.Add(new Constraint(MyComponent, Constraint.ConstraintType.morethan, i));
            }
            
            MyComponent.numVars = this.Variables.Where(var => var.IsActive).Count();
        }

        public void UpdateComponentOutputs(List<List<double>> GradientData)
        {
            MyComponent.AppendToObjectives(this.Objectives);
            MyComponent.AppendToVariables(this.Variables.Select(var => var.CurrentValue).ToList());
            MyComponent.AppendToGradients(GradientData);
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
