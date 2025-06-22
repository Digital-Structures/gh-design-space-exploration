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
using Radical;

namespace Stepper
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        private StepperWindow MyWindow;
        private StepperVM Stepper;
        private String PreviousText;

        public SettingsControl()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR
        public SettingsControl(StepperVM stepper, StepperWindow window)
        {
            this.MyWindow = window;
            this.Stepper = stepper;
            this.PreviousText = "";

            this.DataContext = Stepper;
            InitializeComponent();
        }

        //CHECKED
        //Enables absolute objective value graph
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            this.MyWindow.DisplayAbsolute();
        }

        //UNCHECKED
        //Enables normalized objective value graph
        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.MyWindow.DisplayNormalized();
        }

        //GOT FOCUS
        //Clear box contents when it's active
        protected void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            PreviousText = box.Text;
            box.Clear();
        }

        //LOST FOCUS
        //If TextBox is left empty, set value to 0
        protected void TextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;

            if (box.Text == "")
                box.Text = PreviousText; //Should default to previous value
        }

        //PREVIEW KEY DOWN
        //Allow pressing enter to save textbox content
        protected void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //Exit the Text Box
                Keyboard.ClearFocus();
                TextBox_LostFocus(sender, e);

                //Update the value of the Text Box after exiting
                TextBox box = (TextBox)sender;
                DependencyProperty prop = TextBox.TextProperty;
                BindingExpression binding = BindingOperations.GetBindingExpression(box, prop);
                if (binding != null) { binding.UpdateSource(); }
            }
        }
    }
}
