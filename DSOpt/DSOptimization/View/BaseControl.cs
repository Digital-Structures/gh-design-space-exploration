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

namespace DSOptimization
{
    //STYLES
    //Int and Float input parsing styles
    public static class Styles
    {
        public const System.Globalization.NumberStyles STYLEFLOAT = System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign;
        public const System.Globalization.NumberStyles STYLEINT = System.Globalization.NumberStyles.Integer;
    }

    //BASE CONTROL
    //Base class for variable control and constraint control UI elements
    public class BaseControl : UserControl
    {
        public BaseControl()
        {
            this.DataContext = MyVM;            
        }

        //CONSTRUCTOR
        public BaseControl(IViewModel VM)
        {
            this.MyVM = VM;
            this.DataContext = MyVM;
            this.PreviousText = "";
        }
        public IViewModel MyVM { get; set; }
        private string PreviousText;

        //PREVIEW MOUSE DOWN
        //Disable changes during optimization
        protected void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox box = (TextBox)sender;

            if (this.MyVM.ChangesEnabled)
                box.IsReadOnly = false;
            else
                box.IsReadOnly = true;
        }

        //MEASURE STRING
        //Obtain the size of the current user input
        protected Size MeasureString(string candidate, TextBox box)
        {
            var formattedText = new FormattedText(candidate, System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(box.FontFamily, box.FontStyle, box.FontWeight, box.FontStretch), box.FontSize, Brushes.Black);
            return new Size(formattedText.Width, formattedText.Height);
        }

        //TEXT CHANGED
        //Resize textbox to fit user input
        protected void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            if (box.Text != "")
                box.Width = MeasureString(box.Text, box).Width;
        }

        //GOT FOCUS
        //Clear box contents when it's active
        protected void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //Disable changes during optimization
            if (this.MyVM.ChangesEnabled)
            {
                TextBox box = (TextBox)sender;
                PreviousText = box.Text;
                box.Clear();
            }
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

        //PREVIVEW FLOAT INPUT
        //Only allow user to input parseable float text
        protected void TextBox_PreviewTextInput_Float(object sender, TextCompositionEventArgs e)
        {
            TextBox box = (TextBox)sender;

            //Accept negative sign only as first character
            if (box.Text == "" && e.Text == "-") { return; }

            e.Handled = !(IsTextAllowedFloat(box.Text + e.Text));
        }

        //IS TEXT ALLOWED FLOAT
        //Determine if float input is parseable
        protected static bool IsTextAllowedFloat(string text)
        {
            double val = 0;
            return double.TryParse(text, Styles.STYLEFLOAT, System.Globalization.CultureInfo.InvariantCulture, out val);
        }

        //PREVIVEW FLOAT INPUT
        //Only allow user to input parseable float text
        protected void TextBox_PreviewTextInput_Int(object sender, TextCompositionEventArgs e)
        {
            TextBox box = (TextBox)sender;

            //Accept negative sign only as first character
            if (box.Text == "" && e.Text == "-") { return; }

            e.Handled = !(IsTextAllowedInt(box.Text + e.Text));
        }

        //IS TEXT ALLOWED INT
        //Determines if int input is parseable
        protected static bool IsTextAllowedInt(string text)
        {
            int val = 0;
            return int.TryParse(text, Styles.STYLEINT, System.Globalization.CultureInfo.InvariantCulture, out val);
        }

    }
}