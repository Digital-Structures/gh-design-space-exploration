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
            if (MyComponent.DebugLogging)
            {
                System.Console.WriteLine("=== Effects Analysis Started ===");
            }
            
            // Clear previous results
            EffectIndicesList.Clear();
            DesignMapEffects.Clear();
            OverallAvg.Clear();
            IndEffectsSum.Clear();
            IndEffectsAvg.Clear();
            OverallEff.Clear();

            // Get basic parameters
            numLevels = MyComponent.numLevels;
            numVars = MyComponent.numVars;
            numObj = MyComponent.numObjs;

            if (MyComponent.DebugLogging)
            {
                System.Console.WriteLine($"Configuration:");
                System.Console.WriteLine($"  Number of Variables: {numVars}");
                System.Console.WriteLine($"  Number of Levels: {numLevels}");
                System.Console.WriteLine($"  Number of Objectives: {numObj}");
                System.Console.WriteLine($"  Level Settings: [{string.Join(", ", MyComponent.LevelSettings)}]");
            }

            // Validate inputs
            if (numVars < 1 || numVars > 13)
            {
                throw new ArgumentException("Number of variables must be between 1 and 13");
            }

            if (numLevels < 2 || numLevels > 3)
            {
                throw new ArgumentException("Number of levels must be 2 or 3");
            }

            if (MyComponent.LevelSettings.Count < numLevels)
            {
                throw new ArgumentException($"Need {numLevels} level settings, but only {MyComponent.LevelSettings.Count} provided.");
            }

            // Orthogonal arrays for different configurations
            int[,] orthogonalArray = null;
            
            if (numLevels == 2)
            {
                if (numVars <= 7)
                {
                    if (MyComponent.DebugLogging)
                    {
                        System.Console.WriteLine($"Using L8(2^7) orthogonal array for {numVars} variables at 2 levels");
                    }
                    // L8(2^7) array
                    orthogonalArray = new int[,] {
                        { 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 1, 1, 1, 1 },
                        { 0, 1, 1, 0, 0, 1, 1 },
                        { 0, 1, 1, 1, 1, 0, 0 },
                        { 1, 0, 1, 0, 1, 0, 1 },
                        { 1, 0, 1, 1, 0, 1, 0 },
                        { 1, 1, 0, 0, 1, 1, 0 },
                        { 1, 1, 0, 1, 0, 0, 1 }
                    };
                    numRows = 8;
                }
                else
                {
                    if (MyComponent.DebugLogging)
                    {
                        System.Console.WriteLine($"Using L16(2^15) orthogonal array for {numVars} variables at 2 levels");
                    }
                    // L16(2^15) array
                    orthogonalArray = new int[,] {
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1 },
                        { 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1 },
                        { 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 },
                        { 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1 },
                        { 0, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0 },
                        { 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0 },
                        { 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1 },
                        { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
                        { 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0 },
                        { 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0 },
                        { 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 1 },
                        { 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0 },
                        { 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1 },
                        { 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1 },
                        { 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0 }
                    };
                    numRows = 16;
                }
            }
            else if (numLevels == 3)
            {
                if (numVars <= 4)
                {
                    if (MyComponent.DebugLogging)
                    {
                        System.Console.WriteLine($"Using L9(3^4) orthogonal array for {numVars} variables at 3 levels");
                    }
                    // L9(3^4) array
                    orthogonalArray = new int[,] {
                        { 0, 0, 0, 0 },
                        { 0, 1, 1, 1 },
                        { 0, 2, 2, 2 },
                        { 1, 0, 1, 2 },
                        { 1, 1, 2, 0 },
                        { 1, 2, 0, 1 },
                        { 2, 0, 2, 1 },
                        { 2, 1, 0, 2 },
                        { 2, 2, 1, 0 }
                    };
                    numRows = 9;
                }
                else
                {
                    if (MyComponent.DebugLogging)
                    {
                        System.Console.WriteLine($"Using L27(3^13) orthogonal array for {numVars} variables at 3 levels");
                    }
                    // L27(3^13) array
                    orthogonalArray = new int[,] {
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        { 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2 },
                        { 0, 1, 1, 1, 0, 0, 0, 1, 2, 1, 2, 1, 2 },
                        { 0, 1, 1, 1, 1, 1, 1, 2, 0, 2, 0, 2, 0 },
                        { 0, 1, 1, 1, 2, 2, 2, 0, 1, 0, 1, 0, 1 },
                        { 0, 2, 2, 2, 0, 0, 0, 2, 1, 2, 1, 2, 1 },
                        { 0, 2, 2, 2, 1, 1, 1, 0, 2, 0, 2, 0, 2 },
                        { 0, 2, 2, 2, 2, 2, 2, 1, 0, 1, 0, 1, 0 },
                        { 1, 0, 1, 2, 0, 1, 2, 0, 0, 1, 2, 2, 1 },
                        { 1, 0, 1, 2, 1, 2, 0, 1, 1, 2, 0, 0, 2 },
                        { 1, 0, 1, 2, 2, 0, 1, 2, 2, 0, 1, 1, 0 },
                        { 1, 1, 2, 0, 0, 1, 2, 1, 2, 2, 1, 0, 0 },
                        { 1, 1, 2, 0, 1, 2, 0, 2, 0, 0, 2, 1, 1 },
                        { 1, 1, 2, 0, 2, 0, 1, 0, 1, 1, 0, 2, 2 },
                        { 1, 2, 0, 1, 0, 1, 2, 2, 1, 0, 0, 1, 2 },
                        { 1, 2, 0, 1, 1, 2, 0, 0, 2, 1, 1, 2, 0 },
                        { 1, 2, 0, 1, 2, 0, 1, 1, 0, 2, 2, 0, 1 },
                        { 2, 0, 2, 1, 0, 2, 1, 0, 0, 2, 1, 1, 2 },
                        { 2, 0, 2, 1, 1, 0, 2, 1, 1, 0, 2, 2, 0 },
                        { 2, 0, 2, 1, 2, 1, 0, 2, 2, 1, 0, 0, 1 },
                        { 2, 1, 0, 2, 0, 2, 1, 1, 2, 0, 0, 2, 1 },
                        { 2, 1, 0, 2, 1, 0, 2, 2, 0, 1, 1, 0, 2 },
                        { 2, 1, 0, 2, 2, 1, 0, 0, 1, 2, 2, 1, 0 },
                        { 2, 2, 1, 0, 0, 2, 1, 2, 1, 1, 2, 0, 0 },
                        { 2, 2, 1, 0, 1, 0, 2, 0, 2, 2, 0, 1, 1 },
                        { 2, 2, 1, 0, 2, 1, 0, 1, 0, 0, 1, 2, 2 }
                    };
                    numRows = 27;
                }
            }

            // Determine how many columns we can use from the orthogonal array
            int availableColumns = orthogonalArray.GetLength(1);
            int columnsToUse = Math.Min(numVars, availableColumns);

            // Build the design map
            for (int i = 0; i < numRows; i++)
            {
                DesignMapEffects.Add(new List<double>());
                for (int j = 0; j < numVars; j++)
                {
                    int levelIndex;
                    if (j < columnsToUse)
                    {
                        // Use orthogonal array value
                        levelIndex = orthogonalArray[i, j];
                    }
                    else
                    {
                        // For extra variables, use a simple pattern
                        levelIndex = i % numLevels;
                    }

                    // Ensure level index is valid
                    levelIndex = Math.Min(levelIndex, MyComponent.LevelSettings.Count - 1);
                    
                    // Calculate actual variable value
                    double minVal = MyComponent.MinVals[j];
                    double maxVal = MyComponent.MaxVals[j];
                    double levelSetting = MyComponent.LevelSettings[levelIndex];
                    double value = minVal + levelSetting * (maxVal - minVal);
                    
                    DesignMapEffects[i].Add(value);
                }
            }

            // Run the iterations
            MyComponent.ObjValues = new List<List<double>>();
            MyComponent.Iterating = true;
            this.Iterate();
            MyComponent.Iterating = false;

            // Calculate overall averages
            for (int objIndex = 0; objIndex < numObj; objIndex++)
            {
                double sum = 0;
                int count = 0;
                
                if (MyComponent.DebugLogging)
                {
                    System.Console.WriteLine($"Calculating overall average for objective {objIndex}:");
                    System.Console.WriteLine($"  numRows: {numRows}");
                    System.Console.WriteLine($"  MyComponent.ObjValues.Count: {MyComponent.ObjValues.Count}");
                }
                
                for (int runIndex = 0; runIndex < numRows; runIndex++)
                {
                    try
                    {
                        if (MyComponent.ObjValues.Count > runIndex && 
                            MyComponent.ObjValues[runIndex].Count > objIndex)
                        {
                            sum += MyComponent.ObjValues[runIndex][objIndex];
                            count++;
                        }
                        else
                        {
                            if (MyComponent.DebugLogging)
                            {
                                System.Console.WriteLine($"  Warning: Missing objective data at run {runIndex}, obj {objIndex}");
                                System.Console.WriteLine($"    ObjValues.Count: {MyComponent.ObjValues.Count}");
                                if (MyComponent.ObjValues.Count > runIndex)
                                {
                                    System.Console.WriteLine($"    ObjValues[{runIndex}].Count: {MyComponent.ObjValues[runIndex].Count}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (MyComponent.DebugLogging)
                        {
                            System.Console.WriteLine($"  Error accessing objective data at run {runIndex}, obj {objIndex}: {ex.Message}");
                        }
                    }
                }
                
                double avgValue = count > 0 ? sum / count : 0.0;
                OverallAvg.Add(avgValue);
                if (MyComponent.DebugLogging)
                {
                    System.Console.WriteLine($"  Overall average for obj {objIndex}: {avgValue} (from {count} values)");
                }
            }

            // Initialize effect calculations
            for (int objIndex = 0; objIndex < numObj; objIndex++)
            {
                IndEffectsSum.Add(new List<List<List<double>>>());
                IndEffectsAvg.Add(new List<List<double>>());
                OverallEff.Add(new List<double>());
                
                for (int varIndex = 0; varIndex < numVars; varIndex++)
                {
                    IndEffectsSum[objIndex].Add(new List<List<double>>());
                    IndEffectsAvg[objIndex].Add(new List<double>());
                    
                    for (int levelIndex = 0; levelIndex < numLevels; levelIndex++)
                    {
                        IndEffectsSum[objIndex][varIndex].Add(new List<double>());
                    }
                }
            }

            // Collect objective values by variable level
            for (int objIndex = 0; objIndex < numObj; objIndex++)
            {
                for (int varIndex = 0; varIndex < numVars; varIndex++)
                {
                    for (int levelIndex = 0; levelIndex < numLevels; levelIndex++)
                    {
                        for (int runIndex = 0; runIndex < numRows; runIndex++)
                        {
                            try
                            {
                                // Determine what level this variable was set to in this run
                                int actualLevel;
                                if (varIndex < columnsToUse)
                                {
                                    actualLevel = orthogonalArray[runIndex, varIndex];
                                }
                                else
                                {
                                    actualLevel = runIndex % numLevels;
                                }
                                
                                if (actualLevel == levelIndex &&
                                    MyComponent.ObjValues.Count > runIndex &&
                                    MyComponent.ObjValues[runIndex].Count > objIndex)
                                {
                                    IndEffectsSum[objIndex][varIndex][levelIndex].Add(MyComponent.ObjValues[runIndex][objIndex]);
                                }
                            }
                            catch (Exception ex)
                            {
                                if (MyComponent.DebugLogging)
                                {
                                    System.Console.WriteLine($"Error in effects collection: obj={objIndex}, var={varIndex}, level={levelIndex}, run={runIndex}");
                                    System.Console.WriteLine($"  Error: {ex.Message}");
                                    System.Console.WriteLine($"  ObjValues.Count: {MyComponent.ObjValues.Count}");
                                    if (MyComponent.ObjValues.Count > runIndex)
                                    {
                                        System.Console.WriteLine($"  ObjValues[{runIndex}].Count: {MyComponent.ObjValues[runIndex].Count}");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Calculate effects
            if (MyComponent.DebugLogging)
            {
                System.Console.WriteLine($"Calculating effects for {numObj} objectives, {numVars} variables:");
            }
            for (int objIndex = 0; objIndex < numObj; objIndex++)
            {
                for (int varIndex = 0; varIndex < numVars; varIndex++)
                {
                    double totalEffect = 0;
                    int effectCount = 0;
                    
                    try
                    {
                        for (int levelIndex = 0; levelIndex < numLevels; levelIndex++)
                        {
                            var levelValues = IndEffectsSum[objIndex][varIndex][levelIndex];
                            if (levelValues.Count > 0)
                            {
                                double levelAvg = levelValues.Average();
                                double effect = levelAvg - OverallAvg[objIndex];
                                IndEffectsAvg[objIndex][varIndex].Add(effect);
                                totalEffect += Math.Abs(effect);
                                effectCount++;
                                
                                if (MyComponent.DebugLogging)
                                {
                                    System.Console.WriteLine($"  Obj{objIndex} Var{varIndex} Level{levelIndex}: avg={levelAvg:F6}, effect={effect:F6} (from {levelValues.Count} samples)");
                                }
                            }
                            else
                            {
                                IndEffectsAvg[objIndex][varIndex].Add(0.0);
                                if (MyComponent.DebugLogging)
                                {
                                    System.Console.WriteLine($"  Obj{objIndex} Var{varIndex} Level{levelIndex}: No samples, effect=0.0");
                                }
                            }
                        }
                        
                        double avgEffect = effectCount > 0 ? totalEffect / effectCount : 0.0;
                        OverallEff[objIndex].Add(avgEffect);
                        if (MyComponent.DebugLogging)
                        {
                            System.Console.WriteLine($"  Overall effect for Obj{objIndex} Var{varIndex}: {avgEffect:F6}");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (MyComponent.DebugLogging)
                        {
                            System.Console.WriteLine($"Error calculating effects for obj={objIndex}, var={varIndex}: {ex.Message}");
                        }
                        OverallEff[objIndex].Add(0.0);
                    }
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
                i++;
            }
        }
    }
}
