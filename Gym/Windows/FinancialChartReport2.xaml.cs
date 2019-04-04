using Gym.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Interaction logic for FinancialChartReport2.xaml
    /// </summary>
    public partial class FinancialChartReport2 : Window
    {

        public ObservableCollection<string> Years { get; set; }
        public ObservableCollection<string> MonthNames { get; set; }

        public FinancialChartReport2()
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

            MonthNames = new ObservableCollection<string>();
            MonthNames.Add("فروردین");
            MonthNames.Add("اردیبهشت");
            MonthNames.Add("خرداد");
            MonthNames.Add("تیر");
            MonthNames.Add("مرداد");
            MonthNames.Add("شهریور");
            MonthNames.Add("مهر");
            MonthNames.Add("آبان");
            MonthNames.Add("آذر");
            MonthNames.Add("دی");
            MonthNames.Add("بهمن");
            MonthNames.Add("اسفند");

            Years = new ObservableCollection<string>();
            PersianCalendar pc = new PersianCalendar();
            var emsaal = pc.GetYear(DateTime.Now);
            for (int i = emsaal - 10; i <= emsaal; i++)
            {
                Years.Add(i.ToString());
            }


            Container.DataContext = this;
            cmbYear.SelectedValue = emsaal.ToString();
            cmbMonth.SelectedIndex = pc.GetMonth(DateTime.Now) - 1;
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


        private void rdYearReport_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbMonth.Visibility = Visibility.Collapsed;
            }
            catch { }
        }

        private void rdMonthReport_Checked(object sender, RoutedEventArgs e)
        {
            cmbMonth.Visibility = Visibility.Visible;

        }



        private void LoadBarChartData()
        {
            var db = new Data.GymContextDataContext();
            var pc = new PersianCalendar();
            int year = int.Parse(cmbYear.SelectedValue as string),
                month1 = rdMonthReport.IsChecked == true ? cmbMonth.SelectedIndex + 1 : 1,
                month2 = rdMonthReport.IsChecked == true ? cmbMonth.SelectedIndex + 1 : 12,
                day1 = 1,
                day2 = pc.GetDaysInMonth(year, month2);

            var from = new DateTime(year, month1, day1, pc);
            var to = new DateTime(year, month2, day2, pc);

            var costs = db.Transactions.Where(t => new[] { (int)TransactionType.Cost, (int)TransactionType.Withdraw }.Contains(t.Type));
            costs = costs.Where(c => c.Datetime >= from);
            costs = costs.Where(c => c.Datetime <= to.AddDays(1).AddSeconds(-1));

            var incomes = db.Transactions.Where(t => new int[] { (int)TransactionType.Tuition, (int)TransactionType.SingleSession }.Contains(t.Type));
            incomes = incomes.Where(i => i.Datetime >= from);
            incomes = incomes.Where(i => i.Datetime <= to.AddDays(1).AddSeconds(-1));

            var pair1 = new List<KeyValuePair<string, int>>();
            //costs.GroupBy(c => c.Cost).ToList().ForEach(c =>
            //{
            pair1.Add(new KeyValuePair<string, int>("هزینه", costs.ToList().Select(c => c.Amount).Sum()));
            //});

            var pair2 = new List<KeyValuePair<string, int>>();
            //incomes.GroupBy(i => i.Type).ToList().ForEach(i =>
            //{
            pair2.Add(new KeyValuePair<string, int>("درآمد", incomes.ToList().Select(t => t.Amount).Sum()));
            //});

            ((ColumnSeries)chart.Series[0]).ItemsSource =
                pair2;
            ((ColumnSeries)chart.Series[1]).ItemsSource =
                pair1;
        }

        private void UserDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            string command = (string)eventArgs.Parameter;
            switch (command)
            {
                case "reset":
                    cmbYear.SelectedValue = new PersianCalendar().GetYear(DateTime.Now);
                    cmbMonth.SelectedIndex = new PersianCalendar().GetMonth(DateTime.Now) - 1;
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
