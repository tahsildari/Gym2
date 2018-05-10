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

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for SellReport.xaml
    /// </summary>
    public partial class SellReport : Window
    {
        public SellReport()
        {
            InitializeComponent();
            this.PreviewKeyUp += Window_PreviewKeyUp;
            this.KeyDown += (s,e)=> {
                if (e.Key == Key.Enter && !PaymentDialogHost.IsOpen)
                {
                    PaymentDialogHost.IsOpen = true;
                }
            };
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        ObservableCollection<SellItem> SellList;
        Data.GymContextDataContext db = new Data.GymContextDataContext();

        private void RefreshGrid()
        {
            var query =
            from t in db.Trades
            //where t. == (int)TransactionType.BuyStuff
            select t;

            if (txtAmount1.Value > 0) query = query.Where(t => t.Price >= txtAmount1.Value);
            if (txtAmount2.Value > 0) query = query.Where(t => t.Price <= txtAmount2.Value);
            if (Date1.Date != "") query = query.Where(t => t.Time >= Date1.Date.ToEn());
            if (Date2.Date != "") query = query.Where(t => t.Time <= Date2.Date.ToEn().Value.AddDays(1).AddSeconds(-1));
            if ((int)(cmbItems.SelectedValue ?? 0) > 0) query = query.Where(t => t.GoodId == (int)cmbItems.SelectedValue);
            //if (txtInfo.Text != "") query = query.Where(t => t.Info.Contains(txtInfo.Text));
            //var methods = new List<int>();
            //if (rdCash.IsChecked == true) methods.Add(0);
            //if (rdPos.IsChecked == true) methods.Add(1);
            //if (rdCard.IsChecked == true) methods.Add(2);
            //if (rdCheque.IsChecked == true) methods.Add(3);
            //if (methods.Count == 0) methods.AddRange(Enumerable.Range(0, 4));
            //query = query.Where(t => methods.Contains(t.Method));

            var costs = query.ToList().Select(
                        t => new SellItem
                        {
                            Amount = t.Price,
                            Item = t.Good.Name,
                            Date = t.Time.ToFa(),
                            Count = t.Count
                        }).ToList();

            SellList = new ObservableCollection<SellItem>(costs);

            CostsGrid.DataContext = SellList;
        }

        private void UserDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            string command = (string)eventArgs.Parameter;
            switch (command)
            {
                case "reset":
                    txtAmount1.Value = 0;
                    txtAmount2.Value = 0;
                    txtAmount1.Text = "";
                    txtAmount2.Text = "";
                    Date1.Date = "";
                    Date2.Date = "";
                    Date1.Clear();
                    Date2.Clear();
                    //txtInfo.Text = "";
                    cmbItems.SelectedIndex = -1;
                    //rdCard.IsChecked = rdCash.IsChecked = rdCheque.IsChecked = rdPos.IsChecked = false;
                    RefreshGrid();
                    break;
                case "confirm":
                    RefreshGrid();
                    break;
                case "cancel":
                    break;
                default:
                    break;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            RefreshGrid();
            var list = new List<Data.Good>();
            list.Add(new Data.Good { Id = 0, Name = "همه" });
            list.AddRange(db.Goods.ToList());
            cmbItems.DataContext = list;
        }

        class SellItem
        {
            public string Date { get; set; }
            public string Item { get; set; }
            public int Count { get; set; }
            public int Amount { get; set; }
        }
    }
}
