using HelixToolkit.Wpf;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _3DTest
{
    public enum Algorithm
    {
        NONE = 0,
        BRUTE = 1,
        SORTED = 2,
        HASHMAP = 3,
        TREE = 4,
    }
    public enum Dimensions
    {
        ONE = 1,
        TWO = 2,
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            this.Algo.ItemsSource = Enum.GetValues(typeof(Algorithm));
            this.Algo.SelectedIndex = 0;
            this.Dims.ItemsSource = Enum.GetValues(typeof(Dimensions));
            this.Dims.SelectedIndex = 0;

            Create3DViewPort();
        }
        public void OnInput(object sender, RoutedEventArgs e)
        {
            if (!this.IsInitialized) return;
            Create3DViewPort();
        }

        private void Create3DViewPort()
        {
            // Generate 'random' points
            var r = new Random(0);
            var points = new List<Point3D>();
            for (int i = 0; i < this.Points.Value; i++)
            {
                var x = r.NextDouble();
                var y = r.NextDouble();
                points.Add(new Point3D(x, this.Dims.SelectedIndex == 0 ? 0 : y, 0));
            }
            points.Add(new Point3D(points.Last().X, points.Last().Y, points.Last().Z));
            points = points.OrderBy(p => r.NextDouble()).ToList();  // shuffle

            // Get and set tolerance (HACK)
            double tol = this.Tol20.Value / 20;
            this.Tol.Content = tol;

            // Invoke script
            var script = new Script_Instance();
            object geo = null;
            script.RunScript(points, this.Algo.SelectedIndex, ref geo, tol, Setup.IsChecked.Value);
            var items = geo as List<System.Windows.Media.Media3D.ModelVisual3D>;

            // Render items to view
            this.View.Children.Clear();
            //this.View.Children.Add(new Teapot());
            foreach (var item in items) this.View.Children.Add(item);
        }
    }
}
