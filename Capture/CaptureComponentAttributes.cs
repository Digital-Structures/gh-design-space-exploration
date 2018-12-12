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

namespace Capture
{
    class CaptureComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        CaptureComponent MyComponent;

        public CaptureComponentAttributes(IGH_Component component)
            : base(component)
        {
            MyComponent = (CaptureComponent)component;
        }

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            // Reset list of objective values
            MyComponent.ObjValues = new List<List<double>>();
            MyComponent.PropertyValues = new List<List<double>>();
            MyComponent.FirstRead = true;
            MyComponent.Iterating = true;
            this.Iterate();
            MyComponent.Iterating = false;
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true,GH_SolutionMode.Silent);

            return base.RespondToMouseDoubleClick(sender, e);
        }

        private void Iterate()
        {
            int i = 1;

            MyComponent.Index = i;

            foreach (List<double> sample in MyComponent.DesignMap)
            {
                GHUtilities.ChangeSliders(MyComponent.SlidersList, sample);

                MyComponent.Index = i;
                // If we're taking screen shots, this happens here.

                if (MyComponent.Mode == CaptureComponent.CaptureMode.SaveScreenshot || MyComponent.Mode == CaptureComponent.CaptureMode.Both)
                {

                    if (MyComponent.SSDir == "None")
                    {
                        throw new Exception("No screenshot directory given! Please add valid directory");
                    }

                    BeforeScreenShots();
                    ScreenShot(i);
                    AfterScreenShots();
                    MyComponent.ImagesWritten = "Yes";
                }

                // Write intermediate Screenshots
                if (MyComponent.Mode == CaptureComponent.CaptureMode.SaveCSV || MyComponent.Mode == CaptureComponent.CaptureMode.Both)
                {

                    if (MyComponent.SaveFreq > 0)
                    {
                        if (i % MyComponent.SaveFreq == 0)
                        {
                            WriteProgressToFile(MyComponent.AssembleDMO(MyComponent.DesignMap, MyComponent.ObjValues), MyComponent.CSVDir, MyComponent.CSVFilename, ".csv", i);
                            int Last = i - MyComponent.SaveFreq;
                            System.IO.File.Delete(MyComponent.CSVDir + MyComponent.CSVFilename + "_progress_" + Last.ToString() + ".csv");
                        }
                    }
                }

                    i++;
            }

            

            // If we're saving a CSV, this happens here.
            if (MyComponent.Mode == CaptureComponent.CaptureMode.SaveCSV || MyComponent.Mode == CaptureComponent.CaptureMode.Both)
            {
                WriteOutputToFile(MyComponent.AssembleDMO(MyComponent.DesignMap, MyComponent.ObjValues), MyComponent.CSVDir, MyComponent.CSVFilename, ".csv");
                MyComponent.DataWritten = "Yes";

                if (MyComponent.CSVDir == "None")
                {
                    throw new Exception("No CSV directory given! Please add valid directory");
                }

            }

           

        }

        private Color currentColor;
        private bool grid, axes, worldAxes;

        private void BeforeScreenShots()
        {
            // Change Rhino appearance settings for best screenshot properties, and remember settings so we can change them back.

            currentColor = Rhino.ApplicationSettings.AppearanceSettings.ViewportBackgroundColor;
            grid = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ConstructionGridVisible;
            axes = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ConstructionAxesVisible;
            worldAxes = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.WorldAxesVisible;

            Rhino.ApplicationSettings.AppearanceSettings.ViewportBackgroundColor = Color.White;
            Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ConstructionGridVisible = false;
            Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ConstructionAxesVisible = false;
            Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.WorldAxesVisible = false;
        }

        private void AfterScreenShots()
        {
            // Change Rhino appearance settings back after screen shots.

            Rhino.ApplicationSettings.AppearanceSettings.ViewportBackgroundColor = currentColor;
            Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ConstructionGridVisible = grid;
            Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ConstructionAxesVisible = axes;
            Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.WorldAxesVisible = worldAxes;
        }

        private void ScreenShot(int i)
        {
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
            Rhino.Display.RhinoView view = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView;
            if (view == null)
            {
                return;
            }

            string fileName = @"" + MyComponent.SSDir + MyComponent.SSFilename + "-" + i + ".png";
            Bitmap image = view.CaptureToBitmap();

            if (image == null)
            {
                return;
            }

            image.Save(fileName);
            image = null;
        }

        private void WriteOutputToFile(List<List<double>> output, string path, string filename, string extension)
        {

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"" + path + filename + extension))
            {
                for (int i = 0; i < output.Count; i++)
                {
                    string b = null;
                    for (int j = 0; j < output[i].Count - 1; j++)
                    {
                        b = b + output[i][j] + ",";
                    }

                    b = b + output[i][output[i].Count - 1];

                    file.WriteLine(b);
                }

            }
        }

        private void WriteProgressToFile(List<List<double>> output, string path, string filename, string extension, int count)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"" + path + filename + "_progress_" + count.ToString() + extension))
            {
                for (int i = 0; i < output.Count; i++)
                {
                    string b = null;
                    for (int j = 0; j < output[i].Count - 1; j++)
                    {
                        b = b + output[i][j] + ",";
                    }

                    b = b + output[i][output[i].Count - 1];

                    file.WriteLine(b);
                }
            }
        }

    }
}
