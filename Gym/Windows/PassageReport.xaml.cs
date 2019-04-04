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
    /// Interaction logic for PassageReport.xaml
    /// </summary>
    public partial class PassageReport : Window
    {
        public PassageReport()
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

        ObservableCollection<PassageItem> PassageList;
        Data.GymContextDataContext db = new Data.GymContextDataContext();

        PassageReportType _Type;
        public PassageReportType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                switch (value)
                {
                    case PassageReportType.MembersReport:
                        Title = "گزارش تردد اعضا";
                        break;
                    case PassageReportType.PersonnelReport:
                        Title = "گزارش تردد پرسنل و مربیان";
                        break;
                    case PassageReportType.SingleSessionsReport:
                        Title = "گزارش تک جلسه ای";
                        break;
                    default:
                        break;
                }
                _Type = value;
            }
        }

        private void RefreshGrid()
        {
            var query =
            from t in db.Passages
                //join m in db.Members on t.MemberId equals m.Id
            select t;

            switch (Type)
            {
                case PassageReportType.MembersReport:
                    query = query.Where(t => t.Member.IsStaff == false && t.Member.IsMentor == false && t.Member.IsRegular);
                    break;
                case PassageReportType.PersonnelReport:
                    query = query.Where(t => t.Member.IsStaff == true || t.Member.IsMentor == true);
                    break;
                case PassageReportType.SingleSessionsReport:
                    query = query.Where(t => t.Member.IsRegular == false);
                    break;
                default:
                    break;
            }

            if (txtName.Text.Length > 0) query = query.Where(t => (t.Member.Firstname + " " + t.Member.Lastname).Contains(txtName.Text));

            if (Date1.Date != "")
            {
                if (Time1.SelectedTime.HasValue)
                    query = query.Where(t => t.Time >= Date1.Date.ToEn().Value.Add(Time1.SelectedTime.Value.TimeOfDay));
                else
                    query = query.Where(t => t.Time >= Date1.Date.ToEn());
            }
            if (Date2.Date != "")
            {
                if (Time2.SelectedTime.HasValue)
                    query = query.Where(t => t.Time <= Date2.Date.ToEn().Value.Add(Time2.SelectedTime.Value.TimeOfDay));
                else
                    query = query.Where(t => t.Time <= Date2.Date.ToEn());
            }

            if (InRadioButton.IsChecked.Value != OutRadioButton.IsChecked.Value)
                query = query.Where(t => t.IsEntrance == InRadioButton.IsChecked.Value);

            query = query.OrderByDescending(i => i.Time);

            var items = query.ToList().Select(
                            t => new PassageItem
                            {
                                Name = t.Member.Fullname(),
                                Time = t.Time.ToFaTime(),
                                Type = t.IsEntrance ? "ورود" : "خروج"
                            }).ToList();

            PassageList = new ObservableCollection<PassageItem>(items);

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
                    InRadioButton.IsChecked = OutRadioButton.IsChecked = false;
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
            Date1.Date = DateTime.Now.AddDays(-7).ToFa();
            Time1.SelectedTime = new DateTime();
            Date2.Date = DateTime.Now.ToFa();
            Time2.SelectedTime = new DateTime().AddDays(1).AddSeconds(-1);
            RefreshGrid();
        }

        // 1396/12/22 25000 هزینه - ناهار MahdiTahsildari 
        class PassageItem
        {
            public string Time { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }

        }
    }
}
