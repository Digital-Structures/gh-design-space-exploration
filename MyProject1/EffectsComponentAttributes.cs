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
            

        }


        // Create variables
        Array[][] EffectIndices;
        public List<List<int>> EffectIndicesList;

        int numVars;
        int numLevels;
        int numFactors;
        int j;
        int n;
        int q;
        int dimx;
        int dimy;

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {


            numFactors = 4;
            int numRows = 9;

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


        //    numLevels = MyComponent.numLevels;
        //    j = 2;
        //    n = 4;
        //    q = numLevels;

        //    dimx = q ^ j;
        //    dimy = (q ^ j - 1) / (q - 1);

        //    //Initialize Effects Indices lists
        //    for (int i = 0; i < dimx; i++)
        //    {
        //        for (int j = 0; j < dimy; j++)
        //        {
        //            EffectIndices[i].Add((double) 0);
        //        }
        //    }

        //    //for (int i = 0; i < MyComponent.numClusters; i++)

        //    // Compute the basic columns
        //    for (int k = 1; k < j + 1; k++)

        //    {
        //        int capJ = ((q ^ (k - 1) - 1) / (q - 1)) + 1;


        //        for (int i = 1; i < dimx + 1; i++)
        //        { 

        //        EffectIndices[i][capJ] = Math.Floor(((i - 1) / (q ^ (j - k))));
        //        }
        //}

        //    // Compute the non basic columns

        //    for (int k = 2; k < j + 1; k++)
        //    {
        //        int capJ = ((q ^ (k - 1) - 1) / (q - 1)) + 1;

        //        for (int s = 1; s < capJ - 1; s++)
        //        {

        //            for (int t = 1; t < q - 1; t++)
        //            {

        //                int x = capJ + (s - 1) * (q - 1) + t;

        //                EffectIndices[x] = Math.mod(EffectIndices[s] * t + EffectIndices(:, capJ), q);

        //            }
        //        }
        //    }

        //    A = mod(A, q);







            MyComponent.EffectsDone = true;
                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);

                return base.RespondToMouseDoubleClick(sender, e);
            }




        }

    }








