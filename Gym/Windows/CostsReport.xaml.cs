using Gym.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static Gym.Windows.PayCosts;

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for CostsReport.xaml
    /// </summary>
    public partial class CostsReport : Window
    {
        public CostsReport()
        {
            InitializeComponent();
            this.PreviewKeyUp += Window_PreviewKeyUp;
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
        ObservableCollection<CostData> CostsList;
        Data.GymContextDataContext db = new Data.GymContextDataContext();

        private void RefreshGrid()
        {
            var query =
            from t in db.Transactions
            where t.Type == (int)TransactionType.Cost
            select t;

            if (txtAmount1.Value > 0) query = query.Where(t => t.Amount >= txtAmount1.Value);
            if (txtAmount2.Value > 0) query = query.Where(t => t.Amount <= txtAmount2.Value);
            if (Date1.Date != "") query = query.Where(t => t.Datetime >= Date1.Date.ToEn());
            if (Date2.Date != "") query = query.Where(t => t.Datetime <= Date2.Date.ToEn().Value.AddDays(1).AddSeconds(-1));
            if ((int)(cmbCosts.SelectedValue ?? 0) > 0) query = query.Where(t => t.CostId == (int)cmbCosts.SelectedValue);
            if (txtInfo.Text != "") query = query.Where(t => t.Info.Contains(txtInfo.Text));
            var methods = new List<int>();
            if (rdCash.IsChecked == true) methods.Add(0);
            if (rdPos.IsChecked == true) methods.Add(1);
            if (rdCard.IsChecked == true) methods.Add(2);
            if (rdCheque.IsChecked == true) methods.Add(3);
            if (methods.Count == 0) methods.AddRange(Enumerable.Range(0, 4));
            query = query.Where(t => methods.Contains(t.Method));

            var costs = query.ToList().Select(
                        t => new CostData
                        {
                            ID = t.Id,
                            Amount = t.Amount,
                            CostId = t.CostId.Value,
                            Cost = t.Cost.Category,
                            Date = t.Datetime.ToFa(),
                            Info = t.Info,
                            MethodId = t.Method,
                            Method = ((PaymentMethods)t.Method).GetName()
                        }).ToList();

            CostsList = new ObservableCollection<CostData>(costs);

            CostsGrid.DataContext = CostsList;
        }

        private void UserDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            string command = (string)eventArgs.Parameter;
            switch (command)
            {
                case "reset":
                    txtAmount1.Text = "";
                    txtAmount2.Text = "";
                    Date1.Date = "";
                    Date2.Date = "";
                    Date1.Clear();
                    Date2.Clear();
                    txtInfo.Text = "";
                    cmbCosts.SelectedIndex = -1;
                    rdCard.IsChecked = rdCash.IsChecked = rdCheque.IsChecked = rdPos.IsChecked = false;
                    RefreshGrid();
                    break;
                case "confirm":
                    RefreshGrid();
                    break;
                case "cancel":
                        Dynamics.LastEscapeTime = DateTime.Now;
                    break;
                default:
                    break;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            RefreshGrid();
            var list = new List<Data.Cost>();
            list.Add(new Data.Cost { Id = 0, Category = "همه" });
            list.AddRange(db.Costs.ToList());
            cmbCosts.DataContext = list;
        }
    }
}
