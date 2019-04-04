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
using Gym.Domain;

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for PayCosts.xaml
    /// </summary>
    public partial class PayCosts : Window
    {
        public PayCosts()
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
        CostData CurrentCost = null;

        Actions action = Actions.Inserting;
        bool IsNew() => action == Actions.Inserting;

        private void cmbCosts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
            cmbCosts.DataContext = db.Costs.ToList();
        }

        private void RefreshGrid()
        {
            var costs = (from t in db.Transactions
                         where t.Type == (int)TransactionType.Cost
                         select t
                        ).ToList().Select(
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

        public class CostData
        {
            public int ID { get; set; }

            public string Date { get; set; }
            public int Amount { get; set; }
            public string Cost { get; set; }
            public int CostId { get; set; }
            public string Info { get; set; }
            public string Method { get; set; }
            public byte MethodId { get; set; }
        }

        private void EditCost_Click(object sender, RoutedEventArgs e)
        {
            var cost = ((FrameworkElement)sender).DataContext as CostData;
            CurrentCost = cost;
            action = Actions.Editing;

            txtAmount.Value = cost.Amount;
            txtInfo.Text = cost.Info;
            cmbCosts.SelectedValue = cost.CostId;

            PaymentDialogHost.IsOpen = true;
            cmbCosts.IsEnabled = false;
        }

        private void NewCost_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = CurrentCost = new CostData();
            action = Actions.Inserting;
            txtAmount.Text = "";
            txtInfo.Text = "";
            cmbCosts.SelectedIndex = 0;
            rdCash.IsChecked = true;

            cmbCosts.IsEnabled = true;
        }


        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {
            DuplicateUsernameSnackBar.IsActive = false;// = TimeSpan.FromSeconds(3);

        }

        private void UserDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            var confirmed = (bool)eventArgs.Parameter;
            if (confirmed)
            {
                if (txtAmount.Value > 0)
                {
                    byte method = (byte)(rdCash.IsChecked == true ? 0 : (rdPos.IsChecked == true ? 1 : (rdCard.IsChecked == true ? 2 : 3)));
                    switch (action)
                    {
                        case Actions.Inserting:

                            db.Transactions.InsertOnSubmit(
                                new Data.Transaction
                                {
                                    Amount = txtAmount.Value,
                                    CostId = (int)cmbCosts.SelectedValue,
                                    Datetime = DateTime.Now,
                                    Info = txtInfo.Text,
                                    Method = method,
                                    Type = (byte)TransactionType.Cost,
                                    UserId = Main.CurrentUser.Id
                                });
                            db.SubmitChanges();

                            RefreshGrid();
                            break;
                        case Actions.Editing:
                            var transaction = db.Transactions.Where(t => t.Id == CurrentCost.ID).FirstOrDefault();

                            transaction.Amount = txtAmount.Value;
                            //transaction.CostId = (int)cmbCosts.SelectedValue;
                            transaction.Info = txtInfo.Text;
                            transaction.Method = method;

                            db.SubmitChanges();
                            RefreshGrid();
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                Dynamics.LastEscapeTime = DateTime.Now;
            }
        }
    }
}