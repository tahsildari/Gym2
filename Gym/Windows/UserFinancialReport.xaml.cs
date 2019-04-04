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
    /// Interaction logic for UserFinancialReport.xaml
    /// </summary>
    public partial class UserFinancialReport : Window
    {
        public UserFinancialReport()
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

            Date1.Date = DateTime.Today.ToFa();
            Date2.Date = DateTime.Today.ToFa();
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

        ObservableCollection<UserTransaction> CostsList;
        Data.GymContextDataContext db = new Data.GymContextDataContext();

        private void RefreshGrid()
        {
            var query =
            from t in db.Transactions
            select t;

            if (txtAmount1.Value > 0) query = query.Where(t => t.Amount >= txtAmount1.Value);
            if (txtAmount2.Value > 0) query = query.Where(t => t.Amount <= txtAmount2.Value);
            if (Date1.Date != "") query = query.Where(t => t.Datetime >= Date1.Date.ToEn());
            if (Date2.Date != "") query = query.Where(t => t.Datetime <= Date2.Date.ToEn().Value.AddDays(1).AddSeconds(-1));
            if ((int)(cmbUsers.SelectedValue ?? 0) > 0) query = query.Where(t => t.UserId == (int)cmbUsers.SelectedValue);
            if (txtInfo.Text != "") query = query.Where(t => t.Info.Contains(txtInfo.Text));
            var methods = new List<int>();
            if (rdCash.IsChecked == true) methods.Add(0);
            if (rdPos.IsChecked == true) methods.Add(1);
            if (rdCard.IsChecked == true) methods.Add(2);
            if (rdCheque.IsChecked == true) methods.Add(3);
            if (methods.Count == 0) methods.AddRange(Enumerable.Range(0, 4));
            query = query.Where(t => methods.Contains(t.Method));

            var costs = query.ToList().Select(
                        t => new UserTransaction
                        {
                            Date = t.Datetime.ToFa(),
                            Amount = t.Amount,
                            Type = ((Enums.TransactionType)t.Type).GetName(),
                            Info = t.Info,
                            Method = ((PaymentMethods)t.Method).GetName(),
                            User = t.User.Username,
                            Member = t.Member?.Fullname(),
                            Debtor = t.Member?.Debtor ?? 0,
                            MemberId = t.MemberId ?? -1
                        }).ToList();

            CostsList = new ObservableCollection<UserTransaction>(costs);

            CostsGrid.DataContext = CostsList;

            MoreData.Debtors = string.Join(", ", db.Members.Where(m => m.Debtor > 0).Select(m => m.Fullname()).Distinct().ToList());
            var enrolls = from e in db.Enrolls select e;
            if (Date1.Date != "") enrolls = enrolls.Where(e => e.EnrollDate >= Date1.Date.ToEn());
            if (Date2.Date != "") enrolls = enrolls.Where(e => e.EnrollDate <= Date2.Date.ToEn().Value.AddDays(1).AddSeconds(-1));
            if ((int)(cmbUsers.SelectedValue ?? 0) > 0) enrolls = enrolls.Where(e => e.UserId == (int)cmbUsers.SelectedValue);
            MoreData.TotalDiscount = enrolls.Count() > 0 ? enrolls.Select(e => e.Discount).Sum() : 0;
            MoreData.TotalDebtor = db.Members.Where(m => m.Debtor > 0).Select(m => m.Debtor).Sum();
            MoreData.RealIncome = query.ToList().Where(c => c.Type == 0 || c.Type == 4).Sum(c => c.Amount);
            MoreData.Income = enrolls.Count() > 0 ? enrolls.Select(e => e.Price).Sum() : 0;

            txtDebtors.Text = MoreData.Debtors;
            txtDiscount.Value = MoreData.TotalDiscount;
            txtTotalDebtor.Value = MoreData.TotalDebtor;
            txtRealIncome.Value = MoreData.RealIncome;
            txtIncome.Value = MoreData.Income;
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
                    cmbUsers.SelectedIndex = -1;
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
            var list = new List<Data.User>();
            list.Add(new Data.User { Id = 0, Username = "همه" });
            list.AddRange(db.Users.ToList());
            cmbUsers.DataContext = list;
        }

        More MoreData = new More { Debtors = "", TotalDiscount = 0 };

        class More
        {
            public string Debtors { get; set; }
            public int TotalDiscount { get; set; }
            public int Income { get; set; }
            public int RealIncome { get; set; }
            public int TotalDebtor { get; set; }

        }

        // 1396/12/22 25000 هزینه - ناهار MahdiTahsildari 
        class UserTransaction
        {
            public string Date { get; set; }
            public int Amount { get; set; }
            public string Type { get; set; }
            public string Info { get; set; }
            public string User { get; set; }
            public string Method { get; set; }
            public string Member { get; set; }
            public int Debtor { get; set; }
            public int MemberId { get; set; }
        }

        private void StackPanel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void MoreInfo_Click(object sender, RoutedEventArgs e)
        {
            MoreInfoStack.Visibility = Visibility.Visible;
            PaymentStack.Visibility = Visibility.Collapsed;
        }

        private void SetFilter_Click(object sender, RoutedEventArgs e)
        {
            PaymentStack.Visibility = Visibility.Visible;
            MoreInfoStack.Visibility = Visibility.Collapsed;
        }

        private void ShowMemberForm_Click(object sender, RoutedEventArgs e)
        {
            var record = ((FrameworkElement)sender).DataContext as UserTransaction;
            if (record.MemberId > 0)
            {
                Enroll member = new Enroll();
                member.Owner = Main.Home;
                member.Show();
                member.LoadMember(record.MemberId);// db.Members.FirstOrDefault(m=>m.Id == record.MemberId));
                member.section.SelectedIndex = 3;
            }
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application excel;
                Microsoft.Office.Interop.Excel.Workbook worKbooK;
                Microsoft.Office.Interop.Excel.Worksheet worksheet;
                Microsoft.Office.Interop.Excel.Range celLrangE;

                excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = false;
                excel.DisplayAlerts = false;
                worKbooK = excel.Workbooks.Add(Type.Missing);
                worksheet = (Microsoft.Office.Interop.Excel.Worksheet)worKbooK.ActiveSheet;
                worksheet.Name = "تراکنش ها";
                worksheet.Cells.Font.Size = 14;

                var row = 0;
                worksheet.Cells.Font.Color = System.Drawing.Color.Black;

                foreach (var cost in CostsList)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        if (row == 0)
                        {
                            switch (i)
                            {
                                case 0:
                                    worksheet.Cells[1, i] = "تاریخ";
                                    break;
                                case 1:
                                    worksheet.Cells[1, i] = "مبلغ";
                                    break;
                                case 2:
                                    worksheet.Cells[1, i] = "بابت";
                                    break;
                                case 3:
                                    worksheet.Cells[1, i] = "جزئیات";
                                    break;
                                case 4:
                                    worksheet.Cells[1, i] = "اپراتور";
                                    break;
                                case 5:
                                    worksheet.Cells[1, i] = "روش پرداخت";
                                    break;
                                case 6:
                                    worksheet.Cells[1, i] = "نام عضو";
                                    break;
                                case 7:
                                    worksheet.Cells[1, i] = "بدهی عضو";
                                    break;
                                case 8:
                                    worksheet.Cells[1, i] = "کد عضو";
                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                        {
                            switch (i)
                            {
                                case 0:
                                    worksheet.Cells[row, i] = cost.Date;// "تاریخ";
                                    break;
                                case 1:
                                    worksheet.Cells[row, i] = cost.Amount;// "مبلغ";
                                    break;
                                case 2:
                                    worksheet.Cells[row, i] = cost.Type;// "بابت";
                                    break;
                                case 3:
                                    worksheet.Cells[row, i] = cost.Info;// "جزئیات";
                                    break;
                                case 4:
                                    worksheet.Cells[row, i] = cost.User;// "اپراتور";
                                    break;
                                case 5:
                                    worksheet.Cells[row, i] = cost.Method;// "روش پرداخت";
                                    break;
                                case 6:
                                    worksheet.Cells[row, i] = cost.Member;//"نام عضو";
                                    break;
                                case 7:
                                    worksheet.Cells[row, i] = cost.Debtor;//"بدهی عضو";
                                    break;
                                case 8:
                                    worksheet.Cells[row, i] = cost.MemberId;//"کد عضو";
                                    break;

                                default:
                                    break;
                            }
                        }
                        row++;
                    }
                }

                celLrangE = worksheet.Range[worksheet.Cells[0, 0], worksheet.Cells[row, 9]];
                celLrangE.EntireColumn.AutoFit();

                Microsoft.Office.Interop.Excel.Borders border = celLrangE.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;

                //celLrangE = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[2, 9]];

                worKbooK.SaveAs(Guid.NewGuid().ToString());
                worKbooK.Close();
                excel.Quit();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void TelegramMessage_Click(object sender, RoutedEventArgs e)
        {
            var message = "گزارش باشگاه، از " + Date1.Date + " تا " + Date2.Date + " \n\n" ;
            CostsList.ToList().ForEach(c => message +=
            c.Date + "عضو " + c.Member + " مبلغ " + c.Amount + " به روش " + c.Method + " بابت  " + c.Type + " اپراتور " + c.User + " مانده بدهی " + c.Debtor + " جزئیات " + c.Info+"\n");
            TelegramAPI.SendMessage(message);
        }
    }
}
