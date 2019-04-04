using Gym.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
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
    /// Interaction logic for FinancialChartReport.xaml
    /// </summary>
    public partial class FinancialChartReport : Window
    {
        public FinancialChartReport()
        {
            InitializeComponent();
            this.PreviewKeyUp += Window_PreviewKeyUp;
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter && !PaymentDialogHost.IsOpen)
                {
                    PaymentDialogHost.IsOpen = true;
                }
            };
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                var escTime = (DateTime)(Dynamics.LastEscapeTime ?? DateTime.Now.AddDays(-1));
                if ((DateTime.Now - escTime) > TimeSpan.FromMilliseconds(100))
                    this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBarChartData();
        }

        private void LoadBarChartData()
        {
            var db = new Data.GymContextDataContext();

            var costs = db.Transactions.Where(t => t.CostId != null);
            if (!string.IsNullOrEmpty(Date1.Date))
                costs = costs.Where(c => c.Datetime >= Date1.Date.ToEn());
            if (!string.IsNullOrEmpty(Date2.Date))
                costs = costs.Where(c => c.Datetime <= Date2.Date.ToEn().Value.Date.AddDays(1).AddSeconds(-1));

            var incomes = db.Transactions.Where(t => new int[] { (int)TransactionType.Tuition, (int)TransactionType.SingleSession }.Contains(t.Type));
            if (!string.IsNullOrEmpty(Date1.Date))
                incomes = incomes.Where(i => i.Datetime >= Date1.Date.ToEn());
            if (!string.IsNullOrEmpty(Date2.Date))
                incomes = incomes.Where(i => i.Datetime <= Date2.Date.ToEn().Value.Date.AddDays(1).AddSeconds(-1));

            var pair1 = new List<KeyValuePair<string, int>>();
            costs.GroupBy(c => c.Cost).ToList().ForEach(c =>
              {
                  pair1.Add(new KeyValuePair<string, int>(c.Key.Category, c.Sum(t => t.Amount)));
              });

            var pair2 = new List<KeyValuePair<string, int>>();
            incomes.GroupBy(i => i.Type).ToList().ForEach(i =>
            {
                pair2.Add(new KeyValuePair<string, int>(((TransactionType)i.Key).GetName(), i.Sum(t => t.Amount)));
            });

            ((ColumnSeries)chart.Series[0]).ItemsSource =
                pair2;
            ((ColumnSeries)chart.Series[1]).ItemsSource =
                pair1;
            //        new KeyValuePair<string, int>[]{
            //new KeyValuePair<string,int>("Project Manager", 12),
            //new KeyValuePair<string,int>("CEO", 25),
            //new KeyValuePair<string,int>("Software Engg.", 5),
            //new KeyValuePair<string,int>("Team Leader", 6),
            //new KeyValuePair<string,int>("Project Leader", 10),
            //new KeyValuePair<string,int>("Developer", 4) };
        }

        private void UserDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            string command = (string)eventArgs.Parameter;
            switch (command)
            {
                case "reset":
                    Date1.Date = "";
                    Date2.Date = "";
                    Date1.Clear();
                    Date2.Clear();
                    LoadBarChartData();
                    break;
                case "confirm":
                    LoadBarChartData();
                    break;
                case "cancel":
                    Dynamics.LastEscapeTime = DateTime.Now;
                    break;
                default:
                    break;
            }

        }

    }
}
