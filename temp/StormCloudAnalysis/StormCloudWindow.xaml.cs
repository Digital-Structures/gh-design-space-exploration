using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using StormCloud.ViewModel;
using System.Threading;
using HelixToolkit.Wpf;
using Rhino.Geometry;
using StormCloud.Evolutionary;
//using StructureEngine.ViewModel;

namespace StormCloud
{
    /// <summary>
    /// Interaction logic for StormCloudWindow.xaml
    /// </summary>
    public partial class StormCloudWindow : Window
    {
        public StormCloudWindow(DesignToolVM designtoolvm, IGH_Component comp)
        {
            InitializeComponent();
            this.Component = (InterOptComponent)comp;
            this.myDesignToolVM = designtoolvm;
            this.myDesignToolVM.Pos = new System.Windows.Media.Media3D.Point3D(0, 0, 20);
            this.myDesignToolVM.LookDir = new System.Windows.Media.Media3D.Vector3D(0, 0, -1);
            this.Pop.Minimum = NCOL;

            this.DataContext = designtoolvm;

        }

        public InterOptComponent Component;

        public DesignToolVM myDesignToolVM;

        public List<Design> GetSeeds(int rowIndex)
        {
            List<Design> seeds = new List<Design>();
            List<DesignVM> lastgen = myDesignToolVM.Generations[rowIndex];
            foreach (DesignVM designvm in lastgen)
            {
                if (designvm.IsSelected)
                {
                    seeds.Add(designvm.Design.DesignClone()); // clone for safety
                }
            }
            return seeds;
        }

        public List<DesignVM> GenerateAndSelectTop(int number, int popsize, double rate)
        {
            // get new generation
            DesignVM myFirstDesign = this.myDesignToolVM.InitialDesign;
            int rowIndex = this.DesignGrid.RowDefinitions.Count - 1;
            List<Design> seeds = new List<Design>();
            if (rowIndex >= 0)
            {
                seeds = this.GetSeeds(rowIndex);
            }
            List<Design> Designs = EvolutionaryUtilities.NewGeneration(myFirstDesign.Design, seeds, EvolutionaryUtilities.NormalGenerator, popsize, rate);//, null);
            List<DesignVM> generatedDesigns = new List<DesignVM>();
            // Run designs through grasshopper solution
            foreach (Design design in Designs)
            {
                for (int i = 0; i < Component.Params.Input[2].Sources.Count; i++)
                {
                    GH_NumberSlider slider = Component.Params.Input[2].Sources[i] as GH_NumberSlider;
                    if (slider != null)
                    {
                        slider.SetSliderValue((decimal)design.DesignVariables[i].Value);
                    }
                }
                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
                List<Rhino.Geometry.Line> lines = Component.DesignLines; // write csv here??
                List<Rhino.Geometry.Curve> curves = Component.DesignCurves;
                List<Rhino.Geometry.Mesh> meshes = Component.DesignMeshes;
                List<Rhino.Geometry.Brep> breps = Component.DesignBreps;

                double normscore = Component.Score /this.myDesignToolVM.getinitscore();
                DesignVM genDesign = new DesignVM(lines, curves, meshes, breps, false, true, normscore, design.DesignClone());
                this.myDesignToolVM.ExplorationRec.AppendLine(csvWriter.writeDesign(genDesign));
                generatedDesigns.Add(genDesign);
            }
            // sort and identify top performers
            List<DesignVM> best = EvolutionaryUtilities.FindTopDesignsVM(generatedDesigns, number, rate);

            // Change main viewport design to top best design
            DesignVM topbest = best[0];
            for (int i = 0; i < Component.Params.Input[2].Sources.Count; i++)
            {
                GH_NumberSlider slider = Component.Params.Input[2].Sources[i] as GH_NumberSlider;
                if (slider != null)
                {
                    slider.SetSliderValue((decimal)topbest.Design.DesignVariables[i].Value);
                }
            }
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);

            // number = number of designs to select
            return best;
        }

        public List<DesignVM> GenerateAndSelectTopFromSeeds(int number, int popsize, double rate, List<Design> seeds)
        {
            // get new generation
            DesignVM myFirstDesign = this.myDesignToolVM.InitialDesign;
            int rowIndex = this.DesignGrid.RowDefinitions.Count - 1;
            List<Design> Designs = EvolutionaryUtilities.NewGeneration(myFirstDesign.Design, seeds, EvolutionaryUtilities.NormalGenerator, popsize, rate);//, null);
            List<DesignVM> generatedDesigns = new List<DesignVM>();
            // Run designs through grasshopper solution
            foreach (Design design in Designs)
            {
                for (int i = 0; i < Component.Params.Input[2].Sources.Count; i++)
                {
                    GH_NumberSlider slider = Component.Params.Input[2].Sources[i] as GH_NumberSlider;
                    if (slider != null)
                    {
                        slider.SetSliderValue((decimal)design.DesignVariables[i].Value);
                    }
                }
                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
                List<Rhino.Geometry.Line> lines = Component.DesignLines;
                List<Rhino.Geometry.Curve> curves = Component.DesignCurves;
                List<Rhino.Geometry.Mesh> meshes = Component.DesignMeshes;
                List<Rhino.Geometry.Brep> breps = Component.DesignBreps;

                double normscore = Component.Score / this.myDesignToolVM.getinitscore();
                DesignVM genDesign = new DesignVM(lines, curves, meshes, breps, false, true, normscore, design.DesignClone());

                this.myDesignToolVM.ExplorationRec.AppendLine(csvWriter.writeDesign(genDesign));
                generatedDesigns.Add(genDesign);
            }
            // sort and identify top performers
            List<DesignVM> best = EvolutionaryUtilities.FindTopDesignsVM(generatedDesigns, number, rate);

            // Change main viewport design to top best design
            DesignVM topbest = best[0];
            for (int i = 0; i < Component.Params.Input[2].Sources.Count; i++)
            {
                GH_NumberSlider slider = Component.Params.Input[2].Sources[i] as GH_NumberSlider;
                if (slider != null)
                {
                    slider.SetSliderValue((decimal)topbest.Design.DesignVariables[i].Value);
                }
            }
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);

            // number = number of designs to select
            return best;
        }

        public List<DesignVM> MultiGenerateAndSelectTop(int number, int popsize, double rate)
        {
            List<DesignVM> gen = GenerateAndSelectTop(number, popsize, rate);
            for (int i = 1; i < this.autogenSlider.Value; i++)
            {
                List<Design>seeds = new List<Design>();
                seeds.Add(gen[0].Design);
                seeds.Add(gen[0].Design);
                gen = GenerateAndSelectTopFromSeeds(number, popsize, rate,seeds);
            }
            return gen;
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="popsize"></param>
        /// <param name="mutrate"></param>
        /// <returns></returns>

        //public List<DesignVM> NewGeneration(int popsize, double mutrate)
        //{
        //    List<DesignVM> newgen = new List<DesignVM>();
        //    Random r = new Random();
        //    for (int i = 0; i < popsize; i++)
        //    {
        //        List<decimal> paramvalues = new List<decimal>();
        //        foreach (IGH_Param param in Component.Params.Input[2].Sources) // dvar is input 2
        //        {
        //            GH_NumberSlider slider = param as GH_NumberSlider;
        //            decimal n = r.Next(10);
        //            Console.WriteLine(n);
        //            if (slider != null)
        //                slider.SetSliderValue(n);
        //            paramvalues.Add(n);
        //        }
        //        Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
        //        //ComputedStructure comp = (ComputedStructure)Component.compstruc.CloneImpl();
        //        List<Rhino.Geometry.Line> lines = Component.DesignLines;
        //        newgen.Add(new DesignVM(lines, false,true, Component.Score, paramvalues));
        //    }
        //    List<DesignVM> newgensorted = newgen.OrderBy(x => x.Score).ToList(); //here?
        //    return newgensorted;
        //}

        public List<DesignVM> GenDesignVMs(List<Design> listDesigns)
        {
            List<DesignVM> generatedDesigns = new List<DesignVM>();
            foreach (Design design in listDesigns)
            {
                for (int i=0; i<Component.Params.Input[2].Sources.Count;i++)
                {
                    GH_NumberSlider slider = Component.Params.Input[2].Sources[i] as GH_NumberSlider;
                    if (slider!=null)
                    {
                        slider.SetSliderValue((decimal)design.DesignVariables[i].Value);
                    }
                }
                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
                List<Rhino.Geometry.Line> lines = Component.DesignLines;
                List<Rhino.Geometry.Curve> curves = Component.DesignCurves;
                List<Rhino.Geometry.Mesh> meshes = Component.DesignMeshes;
                List<Rhino.Geometry.Brep> breps = Component.DesignBreps;

                generatedDesigns.Add(new DesignVM(lines, curves, meshes, breps, false, true, Component.Score, design));
            }
            return generatedDesigns;
        }

        public void NewRow()        
        {
            if (this.autoGenerateCheck.IsChecked == true)
            {
                myDesignToolVM.Generations.Add(MultiGenerateAndSelectTop(NCOL, (int)this.Pop.Value, this.Mut.Value));
            }
            else
            {
                myDesignToolVM.Generations.Add(GenerateAndSelectTop(NCOL, (int)this.Pop.Value, this.Mut.Value));
            }
            
            RowDefinition row = new RowDefinition() { Height = new GridLength(RowHeight)};
            this.DesignGrid.RowDefinitions.Add(row);
            int rowIndex = this.DesignGrid.RowDefinitions.Count - 1;

            List<DesignVM> currentgen = myDesignToolVM.Generations[rowIndex];
            //List<DesignVM> best = myDesignToolVM.Generations
            // add row number
            TextBlock GenerationNumber = new TextBlock();
            GenerationNumber.Text = rowIndex.ToString();
            GenerationNumber.Style = (Style)(this.Resources["RowLabel"]);
            GenerationNumber.Padding = new Thickness(0, 0, 0, 10);

            // add gen size and mutation rate
            TextBlock GenSize = new TextBlock();
            GenSize.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            GenSize.Text = "n: " + this.Pop.Value.ToString();
            TextBlock MutRate = new TextBlock();
            MutRate.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            MutRate.Text = "r: " + this.Mut.Value.ToString("0.00");

            StackPanel InfoStack = new StackPanel();
            InfoStack.Width = RowHeight;
            InfoStack.Children.Add(GenerationNumber);
            InfoStack.Children.Add(GenSize);
            InfoStack.Children.Add(MutRate);
            InfoStack.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            InfoStack.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            this.DesignGrid.Children.Add(InfoStack);
            Grid.SetColumn(InfoStack, 0);
            Grid.SetRow(InfoStack, rowIndex);

            // create fittest members
             
            for (int i = 0; i < NCOL; i++)
            {
                    // create new design control for structure
                Console.WriteLine(i);
                DesignVM dvm = currentgen[i];
                DesignControl d = new DesignControl(dvm, RowHeight, RowHeight, true, this.Component);// this.DesignToolVM);
                //d.Margin = new Thickness(3);
                this.DesignGrid.Children.Add(d);
                Grid.SetColumn(d, i + 1);
                Grid.SetRow(d, rowIndex);
            }

             //add "go back" button
            TextBlock GoBack = new TextBlock();
            GoBack.Text = "<";
            GoBack.Width = RowHeight;
            GoBack.Style = (Style)(this.Resources["RowLabel"]);
            GoBack.MouseLeftButtonDown += GoBack_Click;
            GoBack.MouseEnter += GoBack_Hover;
            GoBack.MouseLeave += GoBack_Leave;

            this.DesignGrid.Children.Add(GoBack);
            Grid.SetColumn(GoBack, NCOL + 2);
            Grid.SetRow(GoBack, rowIndex);
            
            myScroll.UpdateLayout();
            myScroll.ScrollToVerticalOffset(RowHeight);
        }

        public const int NCOL = 8;
        
        private double _RowHeight;
        private double RowHeight
        {
            get
            {
                if (_RowHeight == 0)
                {
                    _RowHeight = (DesignGrid.ActualWidth) / (NCOL + 2);
                }
                return _RowHeight;
            }

            set 
            { 
                _RowHeight = value; 
            }
        }

        //useless
        public void Initialize()
        {
            for (int i = 0; i< NCOL + 3; i++)
            {
                ColumnDefinition column = new ColumnDefinition() { Width = new GridLength(RowHeight) };
                DesignGrid.ColumnDefinitions.Add(column);
            }
        }
        //

        private IList<UIElement> GetRow(int rowIndex)
        {
            IList<UIElement> rowMembers = new List<UIElement>();
            int rowSize = NCOL + 2; // plus index and button and parameters
            int rowDotIndex = rowSize * rowIndex + 1;
            for (int i = rowDotIndex; i < rowDotIndex + NCOL; i++)
            {
                rowMembers.Add(this.DesignGrid.Children[i]);
            }
            return rowMembers;
        }

        private void Button_Click_Generate(object sender, RoutedEventArgs e)
        {
            this.NewRow();
            this.myDesignToolVM.Seeds.Clear();// clear seeds for next gen
            // Evolutionary Code Happens Here
            // generate new solutions from seeds
            // Change ViewModel -> Add to list of generations
        }

        private void GoBack_Hover(object sender, RoutedEventArgs e)
        {
            TextBlock myGo = sender as TextBlock;
            if (myGo != null)
            {
                myGo.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }
        private void GoBack_Leave(object sender, RoutedEventArgs e)
        {
            TextBlock myGo = sender as TextBlock;
            if (myGo != null)
            {
                myGo.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            // find clicked row button
            TextBlock clickedButton = e.OriginalSource as TextBlock;
            int newRowIndex = Grid.GetRow(clickedButton);

            // remove extra rows
            int rowSize = NCOL + 2; // plus index and button
            int newChildCount = rowSize * (newRowIndex + 1);
            while (this.DesignGrid.Children.Count > newChildCount)
            {
                this.DesignGrid.Children.RemoveAt(this.DesignGrid.Children.Count - 1);
                //this.myDesignToolVM.Generations.RemoveAt(this.DesignGrid.Children.Count - 1); // remove generation
            }
            int nGentoDel = this.myDesignToolVM.Generations.Count - newRowIndex - 1; // number of generations to delete
            this.myDesignToolVM.Generations.RemoveRange(newRowIndex + 1, nGentoDel);
            // remove extra row definitions
            while (this.DesignGrid.RowDefinitions.Count > newRowIndex + 1)
            {
                this.DesignGrid.RowDefinitions.RemoveAt(this.DesignGrid.RowDefinitions.Count - 1);
            }
            // should remove geom from generations

            // enable new last row
            foreach (DesignControl d in GetRow(newRowIndex))
            {
                d.myViewModel.IsClickable = true;
            }
        }


        private void Button_Click_Build(object sender, RoutedEventArgs e)
        {
            Rhino.RhinoDoc doc = Rhino.RhinoDoc.ActiveDoc;
            int ExpLayerIndex = doc.Layers.Find("Exploration", true);
            if (ExpLayerIndex < 0)
            {
                Rhino.DocObjects.Layer ExpLayer = new Rhino.DocObjects.Layer();
                ExpLayer.Name = "Exploration";
                doc.Layers.Add(ExpLayer);
                ExpLayerIndex = doc.Layers.Find("Exploration", true);
            }
            int subExpLayerIndex = 0;
            if (!(doc.Layers[ExpLayerIndex].GetChildren() == null))
            {
                subExpLayerIndex = doc.Layers[ExpLayerIndex].GetChildren().Length;
            }

            Rhino.DocObjects.Layer subExpLayer = new Rhino.DocObjects.Layer();
            subExpLayer.Name = "Exploration" + subExpLayerIndex.ToString();
            subExpLayer.ParentLayerId = doc.Layers[ExpLayerIndex].Id;
            doc.Layers.Add(subExpLayer);
            subExpLayerIndex = doc.Layers.Find(subExpLayer.Name, true);

            Rhino.DocObjects.ObjectAttributes attr = new Rhino.DocObjects.ObjectAttributes();
            attr.LayerIndex = subExpLayerIndex;
            foreach (Rhino.Geometry.Line l in Component.DesignLines)
            {
                doc.Objects.AddLine(l, attr);
            }
            foreach (Rhino.Geometry.Curve c in Component.DesignCurves)
            {
                doc.Objects.AddCurve(c, attr);
            }
            foreach (Rhino.Geometry.Brep b in Component.DesignBreps)
            {
                doc.Objects.AddBrep(b,attr);
            }
            foreach (Rhino.Geometry.Mesh m in Component.DesignMeshes)
            {
                doc.Objects.AddMesh(m,attr);
            }
        }

        private void Button_MouseRightButtonDown_Build(object sender, MouseButtonEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string path2 = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path = path.Replace(@"\", @"\\"); 
            csvWriter.CreateRecord(path,"ExplorationRecord",this.myDesignToolVM.ExplorationRec, this.Component.Params.Input[2].Sources.Count);
        }
    }

    # region converters

    public class StringFormatConverter : IValueConverter
    {
        static StringFormatConverter() { }
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            string formatString = parameter as string;
            if (!string.IsNullOrEmpty(formatString))
            {
                return string.Format(culture, formatString, value);

            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            // TODO: this is specific to the type of string being converted: make a DoubleFormatter converter
            return double.Parse((string)value);
        }
    }

    public class BoolObsCollConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)value > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CheckRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool ischecked = (bool)value;
            if (ischecked)
            {
                return 10;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
