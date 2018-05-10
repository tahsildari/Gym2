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
using static Gym.Domain.Enums;

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for Balance.xaml
    /// </summary>
    public partial class Balance : Window
    {
        public Balance()
        {
            InitializeComponent();
            this.PreviewKeyUp += Window_PreviewKeyUp;
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CalculateTaraz();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            CalculateTaraz();
        }

        void CalculateTaraz()
        {
            var db = new Data.GymContextDataContext();
            int? enrollsIncome = db.Transactions.Where(t => t.Type == (byte)TransactionType.Tuition).Select(t => t.Amount).ToList().Sum();
            int? singleSessionIncome = db.Transactions.Where(t => t.Type == (byte)TransactionType.SingleSession).ToList().Select(t => t.Amount).Sum();
            int? cost = db.Transactions.Where(t => t.Type == (byte)TransactionType.Cost).ToList().Select(t => t.Amount).Sum();

            txtEnrollIncome.Value = enrollsIncome ?? 0;
            txtSingleSessionIncome.Value = singleSessionIncome ?? 0;
            txtCost.Value = cost ?? 0;

            var balance = txtEnrollIncome.Value + txtSingleSessionIncome.Value - txtCost.Value;
            if (balance >= 0)
            {
                txtBalancePositive.Value = balance;
                txtBalanceNegative.Text = "";
            }
            else
            {
                txtBalanceNegative.Value = balance;
                txtBalancePositive.Text = "";
            }
        }

    }
}
