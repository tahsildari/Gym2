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

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for ClosetUsageReport.xaml
    /// </summary>
    public partial class ClosetUsageReport : Window
    {
        public ClosetUsageReport()
        {
            InitializeComponent();
            this.PreviewKeyUp += Window_PreviewKeyUp;

            this.KeyDown += (s, e) => {
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

        ObservableCollection<dynamic> PassageList;
        Data.GymContextDataContext db = new Data.GymContextDataContext();

        private void RefreshGrid()
        {
            var db = new Data.GymContextDataContext();

            var query =
            from u in db.ClosetUsages
            select u;

            if (txtName.Text.Length > 0) query = query.Where(t => (t.Member.Firstname + " " + t.Member.Lastname).Contains(txtName.Text));

            if (Date1.Date != "")
            {
                if (Time1.SelectedTime.HasValue)
                    query = query.Where(t => t.FromTime >= Date1.Date.ToEn().Value.Add(Time1.SelectedTime.Value.TimeOfDay));
                else
                    query = query.Where(t => t.FromTime >= Date1.Date.ToEn());
            }
            if (Date2.Date != "")
            {
                if (Time2.SelectedTime.HasValue)
                    query = query.Where(t => t.ToTime <= Date2.Date.ToEn().Value.Add(Time2.SelectedTime.Value.TimeOfDay));
                else
                    query = query.Where(t => t.ToTime <= Date2.Date.ToEn());
            }

            //if (InRadioButton.IsChecked.Value != OutRadioButton.IsChecked.Value)
            //    query = query.Where(t => t.IsEntrance == InRadioButton.IsChecked.Value);

            query = query.OrderByDescending(i => i.FromTime).ThenByDescending(i=>i.ToTime);

            var items = query.ToList().Select(
                            t => new
                            {
                                Id = t.ClosetId,
                                IsVip = t.Closet.IsVip ? "VIP" : "",
                                Member = t.Member.Fullname(),
                                FromTime = t.FromTime.ToFaTime(),
                                ToTime = t.ToTime.HasValue ? t.ToTime.Value.ToFaTime() : "بدون خروج"
                            }).ToList();

            PassageList = new ObservableCollection<dynamic>(items);

            CostsGrid.DataContext = PassageList;
        }

        private void UserDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            string command = (string)eventArgs.Parameter;
            switch (command)
            {
                case "reset":
                    txtName.Text = "";
                    Date1.Date = "";
                    Date2.Date = "";
                    Date1.Clear();
                    Date2.Clear();
                    Time1.Text = "";
                    Time2.Text = "";
                    //InRadioButton.IsChecked = OutRadioButton.IsChecked = false;
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
            Date1.SetToday();
            Date2.SetToday();
            Time1.SelectedTime = DateTime.Today;
            Time2.SelectedTime = DateTime.Today.AddDays(1).AddSeconds(-1);
            RefreshGrid();
        }
    }
}
