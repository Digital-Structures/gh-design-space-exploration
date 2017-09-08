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
            this.OverallAvg = new List<double>();
            this.IndEffectsSum = new List<List<List<List<double>>>>();
            this.IndEffectsAvg = new List<List<List<double>>>();
            this.OverallEff = new List<List<double>>();

        }


        // Create variables
        
        public List<List<int>> EffectIndicesList;
        public List<List<double>> DesignMapEffects;
        public int numRows;
        public List<double> OverallAvg;
        public List<List<List<List<double>>>> IndEffectsSum;
        public List<List<List<double>>> IndEffectsAvg;
        public List<List<double>> OverallEff;

        int numVars;
        int numLevels;
        int numFactors;
        int numObj;
        int[,] EffectIndices;


        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {

            // Convert orthogonal matrix to list
            numLevels = MyComponent.numLevels;
            
            numObj = MyComponent.numObjs;

            

            
            //Three level matrices
           int[,] EffectIndicesOpt1 = new int[,] { { 0, 0, 0, 0 }, { 0, 1, 1, 1 }, { 0, 2, 2, 2 }, { 1, 0, 1, 2 }, { 1, 1, 2, 0 }, { 1, 2, 0, 1 }, { 2, 0, 2, 1 }, { 2, 1, 0, 2 }, { 2, 2, 1, 0 } };
           int[,] EffectIndicesOpt2 = new int[,] { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2 }, { 0, 1, 1, 1, 0, 0, 0, 1, 2, 1, 2, 1, 2 }, { 0, 1, 1, 1, 1, 1, 1, 2, 0, 2, 0, 2, 0 }, { 0, 1, 1, 1, 2, 2, 2, 0, 1, 0, 1, 0, 1 }, { 0, 2, 2, 2, 0, 0, 0, 2, 1, 2, 1, 2, 1 }, { 0, 2, 2, 2, 1, 1, 1, 0, 2, 0, 2, 0, 2 }, { 0, 2, 2, 2, 2, 2, 2, 1, 0, 1, 0, 1, 0 }, { 1, 0, 1, 2, 0, 1, 2, 0, 0, 1, 2, 2, 1 }, { 1, 0, 1, 2, 1, 2, 0, 1, 1, 2, 0, 0, 2 }, { 1, 0, 1, 2, 2, 0, 1, 2, 2, 0, 1, 1, 0 }, { 1, 1, 2, 0, 0, 1, 2, 1, 2, 2, 1, 0, 0 }, { 1, 1, 2, 0, 1, 2, 0, 2, 0, 0, 2, 1, 1 }, { 1, 1, 2, 0, 2, 0, 1, 0, 1, 1, 0, 2, 2 }, { 1, 2, 0, 1, 0, 1, 2, 2, 1, 0, 0, 1, 2 }, { 1, 2, 0, 1, 1, 2, 0, 0, 2, 1, 1, 2, 0 }, { 1, 2, 0, 1, 2, 0, 1, 1, 0, 2, 2, 0, 1 }, { 2, 0, 2, 1, 0, 2, 1, 0, 0, 2, 1, 1, 2 }, { 2, 0, 2, 1, 1, 0, 2, 1, 1, 0, 2, 2, 0 }, { 2, 0, 2, 1, 2, 1, 0, 2, 2, 1, 0, 0, 1 }, { 2, 1, 0, 2, 0, 2, 1, 1, 2, 0, 0, 2, 1 }, { 2, 1, 0, 2, 1, 0, 2, 2, 0, 1, 1, 0, 2 }, { 2, 1, 0, 2, 2, 1, 0, 0, 1, 2, 2, 1, 0 }, { 2, 2, 1, 0, 0, 2, 1, 2, 1, 1, 2, 0, 0 }, { 2, 2, 1, 0, 1, 0, 2, 0, 2, 2, 0, 1, 1 }, { 2, 2, 1, 0, 2, 1, 0, 1, 0, 0, 1, 2, 2 } };


            //2 level matrices
            int[,] EffectIndicesOpt3 = new int[,] { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 1, 1, 1, 1 }, { 0, 1, 1, 0, 0, 1, 1 }, { 0, 1, 1, 1, 1, 0, 0 }, { 1, 0, 1, 0, 1, 0, 1 }, { 1, 0, 1, 1, 0, 1, 0 }, { 1, 1, 0, 0, 1, 1, 0 }, { 1, 1, 0, 1, 0, 0, 1 }};
            int[,] EffectIndicesOpt4 = new int[,] { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0,0,0,0,0,0,0,1,1,1,1,1,1,1,1},{ 0,0,0,1,1,1,1,0,0,0,0,1,1,1,1},{ 0,0,0,1,1,1,1,1,1,1,1,0,0,0,0},{ 0,1,1,0,0,1,1,0,0,1,1,0,0,1,1},{ 0,1,1,0,0,1,1,1,1,0,0,1,1,0,0},{ 0,1,1,1,1,0,0,0,0,1,1,1,1,0,0},{ 0,1,1,1,1,0,0,1,1,0,0,0,0,1,1},{ 1,0,1,0,1,0,1,0,1,0,1,0,1,0,1},{ 1,0,1,0,1,0,1,1,0,1,0,1,0,1,0},{ 1,0,1,1,0,1,0,0,1,0,1,1,0,1,0},{ 1,0,1,1,0,1,0,1,0,1,0,0,1,0,1},{ 1,1,0,0,1,1,0,0,1,1,0,0,1,1,0},{ 1,1,0,0,1,1,0,1,0,0,1,1,0,0,1},{ 1,1,0,1,0,0,1,0,1,1,0,1,0,0,1},{ 1,1,0,1,0,0,1,1,0,0,1,0,1,1,0} };



            if (MyComponent.numLevels == 2)
            {
                if (MyComponent.numVars < 7)
                {
                    EffectIndices = EffectIndicesOpt3;
                    numFactors = MyComponent.numVars;
                    numRows = 8;
                }
                else
                {
                    EffectIndices = EffectIndicesOpt4;
                    numFactors = MyComponent.numVars;
                    numRows = 16;
                }
            }

                if (MyComponent.numLevels == 3)
            {
                if (MyComponent.numVars < 5)
                {
                    EffectIndices = EffectIndicesOpt1;
                    numFactors = MyComponent.numVars;
                    numRows = 9;

                }
                else
                {
                    EffectIndices = EffectIndicesOpt2;
                    numFactors = MyComponent.numVars;
                    numRows = 27;
                }

            }

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


            // Calculate overall averages
            for (int i = 0; i < numObj; i++)
            {

                double objSum = 0;
                int count = 0;

                for (int j = 0; j < numRows; j++)
                {
                    objSum = objSum + MyComponent.ObjValues[j][i];
                    count++;
                }
                OverallAvg.Add(objSum/(double)count);
            }

           
            
            // Calculate effects averages for each variable
            for (int i = 0; i < MyComponent.numObjs; i++)
            {
                IndEffectsSum.Add(new List<List<List<double>>>());

                for (int j = 0; j < MyComponent.numVars; j++)
                {
                    IndEffectsSum[i].Add(new List<List<double>>());

                    for (int k = 0; k < MyComponent.numLevels; k++)
                    {
                        IndEffectsSum[i][j].Add(new List<double>());
                    }
                }
            }


            // Create list of objective values for each variable level
            for (int l = 0; l < MyComponent.numObjs; l++)
            {
                for (int i = 0; i < MyComponent.numVars; i++)
                {
                    for (int j = 0; j < numLevels; j++)
                    {
                        for (int k = 0; k < numRows; k++)
                        {
                            if (EffectIndices[k, i] == j)
                            {
                               IndEffectsSum[l][i][j].Add(MyComponent.ObjValues[k][l]);
                            }
                            else { };
                        }
                    }
                }
            }

            // Calculate raw effects
            // Create list of objective values for each variable level
            for (int l = 0; l < MyComponent.numObjs; l++)
            {
                IndEffectsAvg.Add(new List<List<double>>());

                for (int i = 0; i < MyComponent.numVars; i++)
                {
                    IndEffectsAvg[l].Add(new List<double>());
                }
            }


            for (int l = 0; l < MyComponent.numObjs; l++)
            {
                for (int i = 0; i < MyComponent.numVars; i++)
                {
                    for (int j = 0; j < numLevels; j++)
                    {

                        IndEffectsAvg[l][i].Add(IndEffectsSum[l][i][j].Average() - OverallAvg[l]);

                    }
                }
            }


            //Calculate Average effect of each variable 
            for (int i = 0; i < MyComponent.numObjs; i++)
            {
                OverallEff.Add(new List<double>());
            }

            for (int i = 0; i < MyComponent.numObjs; i++)
            {
                for (int j = 0; j < MyComponent.numVars; j++)
                {
                    double effsum = 0;
                    int count = 0;

                    for (int k = 0; k < IndEffectsAvg[i][j].Count; k++)
                    {
                        effsum = effsum + Math.Abs(IndEffectsAvg[i][j][k]);
                        count++;
                    }
                    OverallEff[i].Add(effsum / (double)count);
                }
            }



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








