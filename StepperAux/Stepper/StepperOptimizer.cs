using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Rhino;

namespace Stepper
{
    // Token: 0x02000018 RID: 24
    public class StepperOptimizer
    {
        // Token: 0x060000C0 RID: 192 RVA: 0x000041AC File Offset: 0x000023AC
        public StepperOptimizer(Design design, double FDStepSize, bool disablingAllowed)
        {
            this.Design = design;
            this.numVars = this.Design.ActiveVariables.Count;
            this.numObjs = this.Design.Objectives.Count;
            this.ObjectiveData = new List<List<double>>();
            this.IsoPerf = new List<List<double>>();
            this.DisablingAllowed = disablingAllowed;
            this.FindWhichOnesToDisable();
            this.FDstep = FDStepSize;
        }

        // Token: 0x060000C1 RID: 193 RVA: 0x0000422A File Offset: 0x0000242A
        public void ConvertFromCalculatorToOptimizer(int objIndex, StepperOptimizer.Direction dir, double stepSize)
        {
            this.ObjIndex = objIndex;
            this.Dir = dir;
            this.StepSize = stepSize;
        }

        // Token: 0x060000C2 RID: 194 RVA: 0x00004244 File Offset: 0x00002444
        public void FindWhichOnesToDisable()
        {
            List<IGH_ActiveObject> list = new List<IGH_ActiveObject>();
            foreach (IGH_ActiveObject igh_ActiveObject in Instances.ActiveCanvas.Document.ActiveObjects())
            {
                bool flag = !Instances.ActiveCanvas.Document.DisabledObjects().Contains(igh_ActiveObject);
                if (flag)
                {
                    List<IGH_ActiveObject> list2 = Instances.ActiveCanvas.Document.FindAllDownstreamObjects(igh_ActiveObject);
                    bool flag2 = !list2.Contains(this.Design.MyComponent);
                    if (flag2)
                    {
                        bool flag3 = igh_ActiveObject != this.Design.MyComponent;
                        if (flag3)
                        {
                            list.Add(igh_ActiveObject);
                        }
                    }
                }
            }
            List<IGH_ActiveObject> list3 = new List<IGH_ActiveObject>();
            List<IGH_ActiveObject> list4 = new List<IGH_ActiveObject>();
            List<List<IGH_ActiveObject>> list5 = new List<List<IGH_ActiveObject>>();
            List<List<IGH_ActiveObject>> list6 = new List<List<IGH_ActiveObject>>();
            List<List<IGH_ActiveObject>> list7 = new List<List<IGH_ActiveObject>>();
            foreach (IGH_Param igh_Param in this.Design.MyComponent.NumObjects)
            {
                List<IGH_ActiveObject> item = Instances.ActiveCanvas.Document.FindAllDownstreamObjects(igh_Param);
                list5.Add(item);
            }
            foreach (IGH_ActiveObject item2 in list)
            {
                bool flag4 = false;
                foreach (List<IGH_ActiveObject> list8 in list5)
                {
                    bool flag5 = list8.Contains(item2);
                    if (flag5)
                    {
                        flag4 = true;
                        break;
                    }
                }
                bool flag6 = flag4;
                if (flag6)
                {
                    list3.Add(item2);
                }
                else
                {
                    list4.Add(item2);
                }
            }
            bool disablingAllowed = this.DisablingAllowed;
            if (disablingAllowed)
            {
                foreach (IGH_ActiveObject item3 in list3)
                {
                    this.Disable.Add(item3);
                }
            }
            this.SetExpiredFalse = list4;
        }

        // Token: 0x060000C3 RID: 195 RVA: 0x000044B4 File Offset: 0x000026B4
        public List<List<double>> CalculateGradient()
        {
            bool flag = false;
            foreach (IVariable variable in this.Design.ActiveVariables)
            {
                bool flag2 = variable.Min == variable.CurrentValue || variable.Max == variable.CurrentValue;
                if (flag2)
                {
                    flag = true;
                    break;
                }
            }
            bool flag3 = flag;
            List<List<double>> result;
            if (flag3)
            {
                result = this.CalculateGradientHalfStep();
            }
            else
            {
                result = this.CalculateGradientForwardStep();
            }
            return result;
        }

        // Token: 0x060000C4 RID: 196 RVA: 0x00004554 File Offset: 0x00002754
        public List<List<double>> GenerateDesignMapForwardStep()
        {
            List<List<double>> list = new List<List<double>>();
            List<List<double>> list2 = new List<List<double>>();
            for (int i = 0; i < this.numVars; i++)
            {
                list.Add(new List<double>());
                for (int j = 0; j < this.numVars; j++)
                {
                    list[i].Add(this.Design.ActiveVariables[j].CurrentValue);
                }
                IVariable variable = this.Design.ActiveVariables[i];
                double num = 0.5 * this.FDstep * (variable.Max - variable.Min);
                double value = variable.CurrentValue - num;
                list[i][i] = value;
            }
            list2.AddRange(list);
            list2.Add((from var in this.Design.ActiveVariables
                       select var.CurrentValue).ToList<double>());
            return list2;
        }

        // Token: 0x060000C5 RID: 197 RVA: 0x00004674 File Offset: 0x00002874
        public List<List<double>> CalculateGradientForwardStep()
        {
            List<List<double>> designMap = this.GenerateDesignMapForwardStep();
            this.Iterate(designMap);
            List<List<double>> list = new List<List<double>>();
            double num = double.MinValue;
            double num2 = double.MaxValue;
            for (int i = 0; i < this.numObjs; i++)
            {
                list.Add(new List<double>());
                int num3 = 0;
                for (int j = 0; j < this.Design.Variables.Count; j++)
                {
                    bool isActive = this.Design.Variables[j].IsActive;
                    if (isActive)
                    {
                        double num4 = this.ObjectiveData[num3][i];
                        double num5 = (this.Design.Objectives[i] - num4) / this.FDstep;
                        bool flag = num5 > num;
                        if (flag)
                        {
                            num = num5;
                        }
                        bool flag2 = num5 < num2;
                        if (flag2)
                        {
                            num2 = num5;
                        }
                        list[i].Add(num5);
                        num3++;
                    }
                    else
                    {
                        list[i].Add(0.0);
                    }
                }
                double num6 = double.MinValue;
                double num7 = 0.0;
                bool flag3 = Math.Abs(num) > num6;
                if (flag3)
                {
                    num6 = Math.Abs(num);
                }
                bool flag4 = Math.Abs(num2) > num6;
                if (flag4)
                {
                    num6 = Math.Abs(num2);
                }
                for (int k = 0; k < this.Design.Variables.Count; k++)
                {
                    bool flag5 = num6 != 0.0;
                    if (flag5)
                    {
                        list[i][k] = list[i][k] / num6;
                    }
                    else
                    {
                        list[i][k] = 0.0;
                    }
                    num7 += list[i][k] * list[i][k];
                }
                for (int l = 0; l < this.Design.Variables.Count; l++)
                {
                    bool flag6 = list[i][l] != 0.0;
                    if (flag6)
                    {
                        list[i][l] = list[i][l] / Math.Sqrt(num7);
                    }
                }
            }
            return list;
        }

        // Token: 0x060000C6 RID: 198 RVA: 0x00004920 File Offset: 0x00002B20
        public List<List<double>> GenerateDesignMapHalfStep()
        {
            List<List<double>> list = new List<List<double>>();
            List<List<double>> list2 = new List<List<double>>();
            List<List<double>> list3 = new List<List<double>>();
            for (int i = 0; i < this.numVars; i++)
            {
                list.Add(new List<double>());
                list2.Add(new List<double>());
                for (int j = 0; j < this.numVars; j++)
                {
                    list[i].Add(this.Design.ActiveVariables[j].CurrentValue);
                    list2[i].Add(this.Design.ActiveVariables[j].CurrentValue);
                }
                IVariable variable = this.Design.ActiveVariables[i];
                double num = 0.5 * this.FDstep * (variable.Max - variable.Min);
                double value = variable.CurrentValue - num;
                double value2 = variable.CurrentValue + num;
                list[i][i] = value;
                list2[i][i] = value2;
            }
            list3.AddRange(list);
            list3.AddRange(list2);
            list3.Add((from var in this.Design.ActiveVariables
                       select var.CurrentValue).ToList<double>());
            return list3;
        }

        // Token: 0x060000C7 RID: 199 RVA: 0x00004A9C File Offset: 0x00002C9C
        public List<List<double>> CalculateGradientHalfStep()
        {
            List<List<double>> designMap = this.GenerateDesignMapHalfStep();
            this.Iterate(designMap);
            List<List<double>> list = new List<List<double>>();
            double num = double.MinValue;
            double num2 = double.MaxValue;
            for (int i = 0; i < this.numObjs; i++)
            {
                list.Add(new List<double>());
                int num3 = 0;
                for (int j = 0; j < this.Design.Variables.Count; j++)
                {
                    bool isActive = this.Design.Variables[j].IsActive;
                    if (isActive)
                    {
                        double num4 = this.ObjectiveData[num3][i];
                        double num5 = this.ObjectiveData[this.numVars + num3][i];
                        double num6 = (num5 - num4) / this.FDstep;
                        bool flag = num6 > num;
                        if (flag)
                        {
                            num = num6;
                        }
                        bool flag2 = num6 < num2;
                        if (flag2)
                        {
                            num2 = num6;
                        }
                        list[i].Add(num6);
                        num3++;
                    }
                    else
                    {
                        list[i].Add(0.0);
                    }
                }
                double num7 = double.MinValue;
                double num8 = 0.0;
                bool flag3 = Math.Abs(num) > num7;
                if (flag3)
                {
                    num7 = Math.Abs(num);
                }
                bool flag4 = Math.Abs(num2) > num7;
                if (flag4)
                {
                    num7 = Math.Abs(num2);
                }
                for (int k = 0; k < this.numVars; k++)
                {
                    bool flag5 = num7 != 0.0;
                    if (flag5)
                    {
                        list[i][k] = list[i][k] / num7;
                    }
                    else
                    {
                        list[i][k] = 0.0;
                    }
                    num8 += list[i][k] * list[i][k];
                }
                for (int l = 0; l < this.numVars; l++)
                {
                    bool flag6 = list[i][l] != 0.0;
                    if (flag6)
                    {
                        list[i][l] = list[i][l] / Math.Sqrt(num8);
                    }
                }
            }
            return list;
        }

        // Token: 0x060000C8 RID: 200 RVA: 0x00004D40 File Offset: 0x00002F40
        public void Iterate(List<List<double>> DesignMap)
        {
            bool finished = false;
            Action action = delegate ()
            {
                Instances.ActiveCanvas.Document.SetEnabledFlags(this.Disable, false);
                foreach (IGH_ActiveObject igh_ActiveObject in this.SetExpiredFalse)
                {
                    igh_ActiveObject.ExpireSolution(false);
                }
                foreach (List<double> list in DesignMap)
                {
                    int num = 0;
                    foreach (double x in list)
                    {
                        this.Design.ActiveVariables[num].UpdateValue(x);
                        num++;
                    }
                    bool flag = this.Design.Geometries.Any<IDesignGeometry>();
                    if (flag)
                    {
                        foreach (IDesignGeometry designGeometry in this.Design.Geometries)
                        {
                            designGeometry.Update();
                        }
                    }
                    Instances.ActiveCanvas.Document.NewSolution(false, (Grasshopper.Kernel.GH_SolutionMode)2);
                    this.ObjectiveData.Add(this.Design.Objectives);
                }
                Instances.ActiveCanvas.Document.SetEnabledFlags(this.Disable, true);
                Instances.ActiveCanvas.Document.NewSolution(false, 0);
                finished = true;
            };
            RhinoApp.MainApplicationWindow.Invoke(action);
            while (!finished)
            {
            }
        }

        // Token: 0x060000C9 RID: 201 RVA: 0x00004D94 File Offset: 0x00002F94
        public void Optimize(List<List<double>> Gradient)
        {
            List<List<string>> list = new List<List<string>>();
            double[,] array = new double[Gradient.Count, Gradient[0].Count];
            for (int i = 0; i < Gradient.Count; i++)
            {
                for (int j = 0; j < Gradient[i].Count; j++)
                {
                    bool flag = double.IsNaN(Gradient[i][j]);
                    if (flag)
                    {
                        return;
                    }
                    array[i, j] = Gradient[i][j];
                }
            }
            DenseMatrix denseMatrix = DenseMatrix.OfArray(array);
            Vector<double>[] array2 = denseMatrix.Kernel();
            int index = 0;
            bool flag2 = this.Dir == StepperOptimizer.Direction.Isoperformance;
            if (flag2)
            {
                bool flag3 = this.numVars > this.numObjs;
                if (flag3)
                {
                    for (int k = 0; k < this.numVars - this.numObjs; k++)
                    {
                        this.IsoPerf.Add(new List<double>());
                        double[] collection = array2[k].ToArray();
                        this.IsoPerf[k].AddRange(collection);
                    }
                }
                Random random = new Random();
                int num = random.Next(this.numVars - this.numObjs);
                index = num;
            }
            int num2 = 0;
            for (int l = 0; l < this.Design.Variables.Count; l++)
            {
                bool isActive = this.Design.Variables[l].IsActive;
                if (isActive)
                {
                    IVariable variable = this.Design.ActiveVariables[num2];
                    switch (this.Dir)
                    {
                        case StepperOptimizer.Direction.Maximize:
                            {
                                double currentValue = variable.CurrentValue + Gradient[this.ObjIndex][l] * this.StepSize * (variable.Max - variable.Min);
                                variable.CurrentValue = currentValue;
                                break;
                            }
                        case StepperOptimizer.Direction.Minimize:
                            {
                                double currentValue = variable.CurrentValue - Gradient[this.ObjIndex][l] * this.StepSize * (variable.Max - variable.Min);
                                variable.CurrentValue = currentValue;
                                break;
                            }
                        case StepperOptimizer.Direction.Isoperformance:
                            {
                                List<double> list2 = this.IsoPerf[index];
                                double currentValue = variable.CurrentValue + list2[l] * this.StepSize * (double)this.numVars;
                                variable.CurrentValue = currentValue;
                                break;
                            }
                    }
                    num2++;
                }
            }
            this.Design.UpdateComponentOutputs(Gradient);
        }

        // Token: 0x04000046 RID: 70
        private Design Design;

        // Token: 0x04000047 RID: 71
        private int ObjIndex;

        // Token: 0x04000048 RID: 72
        private StepperOptimizer.Direction Dir;

        // Token: 0x04000049 RID: 73
        private double StepSize;

        // Token: 0x0400004A RID: 74
        private int numVars;

        // Token: 0x0400004B RID: 75
        private int numObjs;

        // Token: 0x0400004C RID: 76
        private double FDstep;

        // Token: 0x0400004D RID: 77
        private List<List<double>> ObjectiveData;

        // Token: 0x0400004E RID: 78
        private List<List<double>> IsoPerf;

        // Token: 0x0400004F RID: 79
        private List<IGH_DocumentObject> Disable = new List<IGH_DocumentObject>();

        // Token: 0x04000050 RID: 80
        private List<IGH_ActiveObject> SetExpiredFalse;

        // Token: 0x04000051 RID: 81
        private bool DisablingAllowed;

        // Token: 0x0200002D RID: 45
        public enum Direction
        {
            // Token: 0x040000E3 RID: 227
            Maximize,
            // Token: 0x040000E4 RID: 228
            Minimize,
            // Token: 0x040000E5 RID: 229
            Isoperformance
        }
    }
}
