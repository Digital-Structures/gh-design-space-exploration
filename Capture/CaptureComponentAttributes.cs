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
            MyComponent.Iterating = true;
            this.Iterate();
            MyComponent.Iterating = false;
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
            return base.RespondToMouseDoubleClick(sender, e);
        }

        private void Iterate()
        {
            foreach (List<double> sample in MyComponent.DesignMap)
            {
                GHUtilities.ChangeSliders(MyComponent.SlidersList, sample);

                // If we're taking screen shots, this happens here.
                int i = 1;
                if (MyComponent.Mode == CaptureComponent.CaptureMode.Screenshot || MyComponent.Mode == CaptureComponent.CaptureMode.Both)
                {
                    BeforeScreenShots();
                    ScreenShot(i);
                    AfterScreenShots();
                    i++;
                }

                // If we're saving a CSV, this happens here.
                if (MyComponent.Mode == CaptureComponent.CaptureMode.Evaluation || MyComponent.Mode == CaptureComponent.CaptureMode.Both)
                {

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

            string fileName = @"" + MyComponent.Dir + MyComponent.Filename + "-" + i + ".png";
            Bitmap image = view.CaptureToBitmap();

            if (image == null)
            {
                return;
            }

            image.Save(fileName);
            image = null;
        }

    }
}
