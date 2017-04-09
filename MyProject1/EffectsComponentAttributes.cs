using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using DSECommon;
using System.Drawing;






namespace Effects
{
    class EffectsComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public EffectsComponent MyComponent;

        public EffectsComponentAttributes(IGH_Component component)
            : base(component)
        {
            MyComponent = (EffectsComponent)component;


            this.EffectIndicesList = new List<List<int>>();
            this.DesignMapEffects = new List<List<double>>();

        }


        // Create variables
        
        public List<List<int>> EffectIndicesList;
        public List<List<double>> DesignMapEffects;
        public int numRows;

        int numVars;
        int numLevels;
        int numFactors;
       

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {

            // Convert orthogonal matrix to list
            numFactors = 4;
            numRows = 9;

            int[,] EffectIndices = new int[,] { { 0, 0, 0, 0 }, { 0, 1, 1, 1 }, { 0, 2, 2, 2 }, { 1, 0, 1, 2 }, { 1, 1, 2, 0 }, { 1, 2, 0, 1 }, { 2, 0, 2, 1 }, { 2, 1, 0, 2 }, { 2, 2, 1, 0 } };


            for (int i = 0; i < numRows; i++)
            {
                EffectIndicesList.Add(new List<int>());
            }

            for (int i = 1; i < numRows + 1; i++)
            {
                for (int j = 1; j < numFactors + 1; j++)
                {
                    EffectIndicesList[i - 1].Add(EffectIndices[i - 1, j - 1]);
                }
            }

            // Create Design Map of levels samples 
            for (int i = 0; i < numRows; i++)
            {
                DesignMapEffects.Add(new List<double>());
            }

            for (int k = 0; k < numRows; k++)
            {
                for (int j = 0; j < MyComponent.numVars; j++)
                {
                        double setting = MyComponent.MinVals[j] + (MyComponent.LevelSettings[EffectIndices[k, j]] * (MyComponent.MaxVals[j] - MyComponent.MinVals[j]));
                        DesignMapEffects[k].Add(setting);
                }
            }


            // Reset list of objective values
            MyComponent.ObjValues = new List<List<double>>();

            MyComponent.Iterating = true;
            this.Iterate();
            MyComponent.Iterating = false;





            MyComponent.EffectsDone = true;
                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);

                return base.RespondToMouseDoubleClick(sender, e);
            }

        private void Iterate()
        {
            int i = 1;

            foreach (List<double> sample in this.DesignMapEffects)
            {
                GHUtilities.ChangeSliders(MyComponent.SlidersList, sample);

                // If we're taking screen shots, this happens here.

                
                i++;
            }

           

        }


    }

    }








