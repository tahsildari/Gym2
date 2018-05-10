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
using System.Windows.Shapes;

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for GoodsSold.xaml
    /// </summary>
    public partial class GoodsSold : Window
    {
        public GoodsSold()
        {
            InitializeComponent();
            this.PreviewKeyUp += Window_PreviewKeyUp;
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void EditGood_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewGood_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
