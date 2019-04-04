using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Gym.Controls
{
    public class NumberBox : TextBox
    {
        public NumberBox()
        {
            PreviewTextInput += NumberBox_PreviewTextInput;
            GotFocus += NumberBox_GotFocus;
            LostFocus += NumberBox_LostFocus;
            //Loaded += NumberBox_Loaded;
            TextChanged += NumberBox_TextChanged;

        }

        private void NumberBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //return;
            if (ThousandSeparate)
            {
                long value = 0;
                if (long.TryParse(Text.Replace(",", ""), out value))
                {
                    if((int)value != Value)
                    Value = (int)value;
                    Text = Value.ToString("#,##0");
                    Select(Text.Length, 0);
                }
            }

        }

        public bool ThousandSeparate { get; set; } = true;

        private void NumberBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //long value = 0;
            //var x = long.Parse(Text.Replace(",","")).ToString("#,##0");
            //Text = string.Format("{0:#,##0}", double.Parse(Text.Replace(",", "")));
        }

        private void NumberBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            SelectAll();
        }

        private void NumberBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9,]"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        //[DependencyProperty("Value")]
        [BindableAttribute(true)]
        public int Value
        {
            get
            {
                //var txt = Text.Replace(",", "");
                //var val = 0;
                //int.TryParse(txt, out val);
                //return val;
                return (int)GetValue(ValueProperty);
            }
            //get { return (int)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
                if (value == 0)
                    Text = "0";
            }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value",
                typeof(int),
                typeof(NumberBox),
                new UIPropertyMetadata(0, new PropertyChangedCallback(valueChangedCallBack)));

        private static void valueChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox nbox = (NumberBox)d;
                nbox.Text = nbox.ThousandSeparate ? ((int)e.NewValue).ToString("#,##0") : e.NewValue.ToString();

        }
    }
}
