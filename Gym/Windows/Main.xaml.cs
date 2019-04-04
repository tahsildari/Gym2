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
using Gym.Controls;
using System.Windows.Controls.Primitives;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Gym.Data;
using Gym.ViewModels;
using static Gym.Domain.Enums;

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            Home = this;
            ClosetsInfo.ToolTip = $"دسترسی سریع F2" + Environment.NewLine + "رنگ مشکلی: کمد های معمولی" + Environment.NewLine + "رنگ آبی: کمد های ویژه" + Environment.NewLine + "رنگ نارنجی: کمد مربی"  + Environment.NewLine + "خط دور: کمد خالی است" + Environment.NewLine + "تمام رنگ: کمد پر است" + Environment.NewLine + "کم رنگ: کمد رزرو شده است";

            StateChanged += Main_StateChanged;
        }


        public DateTime LastMinimize = DateTime.MinValue;
        private void Main_StateChanged(object sender, EventArgs e)
        {
            if (DateTime.Now - LastMinimize > TimeSpan.FromSeconds(0.25))
            {
                this.WindowState = WindowState.Maximized;
                return;
            }
        }

        public static Main Home;
        public NotificationVM Notifications = new NotificationVM();

        #region Notifications

        public void CheckupTuitionDebtors()
        {
            var db = new Data.GymContextDataContext();
            
            var debtors = db.Members.Where(m => !m.IsDeleted && m.Debtor > 0);
            //var debtors = db.Enrolls.Where(e => e.EnrollPayments.Count == 0 || e.Price < e.EnrollPayments.Sum(p => p.Transaction.Amount)).GroupBy(e => e.Member);
            var count = debtors.Count();
            //var names = debtors.Select(g => g.Key.Firstname + " " + g.Key.Lastname);

            //var d3 = db.Enrolls.Where(e=>e.Id == 3).Select(e=>e.EnrollPayments.Sum(p => p.Transaction.Amount ?? 0)).ToList();
            Notifications.TuitionDetors = count > 0 ? count.ToString() : "";

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MembersSearch && window.Title == "بدهکاران شهریه") {
                    var search = window as MembersSearch;
                    search.ListMembers(search.txtSearchBox.Text);
                }
            }
        }

        internal void LoadTitle()
        {
            lblTitle.Text = Domain.Dynamics.AppTitle;
        }

        public void CheckupCreditDebtors()
        {
            var db = new Data.GymContextDataContext();

            var count = db.Members.Where(m => !m.IsDeleted && m.Credit < 0).Count();

            //var d3 = db.Enrolls.Where(e=>e.Idd == 3).Select(e=>e.EnrollPayments.Sum(p => p.Transaction.Amount ?? 0)).ToList();
            Notifications.CaffeDebtors = count > 0 ? count.ToString() : "";
        }

        internal void ResizeClosets()
        {
            Closets.Width = this.Width * Domain.Dynamics.ClosetsWidth / 100;
            //Closets.Width = Domain.Dynamics.ClosetsWidth;
        }

        System.Windows.Threading.DispatcherTimer messageTimer = new System.Windows.Threading.DispatcherTimer();
        internal void ShowMessage(string msg, int sec = 0)
        {
            MessageSnackBar.IsActive = false;
            messageTimer.Stop();
            messageTimer = null;
            messageTimer = new System.Windows.Threading.DispatcherTimer();
            Alert.Content = "تنظیمات ذخیره شد";
            MessageSnackBar.IsActive = true;
            if (sec > 0) {
                messageTimer.Stop();
                var ticks = 0;
                messageTimer.Tick += (s, e) =>
                {
                    Alert.ActionContent = (sec-ticks).ToString();
                    ticks++;
                    if (ticks >= sec)
                    {
                        MessageSnackBar.IsActive = false;
                        messageTimer.Stop();
                    }
                };
                messageTimer.Interval = TimeSpan.FromSeconds(1);
                messageTimer.Start();
            }
        }

        public void CheckupExtenders()
        {
            var db = new Data.GymContextDataContext();

            var d = Domain.Dynamics.ExtendDaysInterval;

            var count = db.Enrolls.Where(e =>
                e.IsActive
                && e.StartDate < DateTime.Now
                && e.ExpireDate >= DateTime.Today
                && e.ExpireDate <= DateTime.Today.AddDays(d)).GroupBy(e => e.MemberId).Count();

            //var d3 = db.Enrolls.Where(e=>e.Id == 3).Select(e=>e.EnrollPayments.Sum(p => p.Transaction.Amount ?? 0)).ToList();
            Notifications.Extenders = count > 0 ? count.ToString() : "";
        }


        public void CheckupOrderPoints()
        {
            var db = new Data.GymContextDataContext();

            var count = db.Goods.Where(g => g.Count <= g.OrderPoint).Count();

            //var d3 = db.Enrolls.Where(e=>e.Id == 3).Select(e=>e.EnrollPayments.Sum(p => p.Transaction.Amount ?? 0)).ToList();
            Notifications.OrderTimes = count > 0 ? count.ToString() : "";
        }

        #endregion
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();

            this.DataContext = Notifications;

            Domain.Dynamics.Refresh();
            ResizeClosets();
            CheckupTuitionDebtors();
            CheckupCreditDebtors();
            CheckupExtenders();
            CheckupOrderPoints();
            Notifications.Tresspasses = "";
            LoadTitle();

            Data.GymContextDataContext db = new Data.GymContextDataContext();
            //if (db.Closets.Count() == 0)
            //{
            //    for (int i = 0; i < 80; i++)
            //    {
            //        var c = new Data.Closet { Id = i + 1, IsVip = false, IsFree = true };
            //        if (i > 10 && i < 20)
            //            c.IsBroken = true;
            //        //if (i > 35 && i <= 40)
            //        //    c.IsFree = false;
            //        db.Closets.InsertOnSubmit(c);
            //    }
            //    for (int i = 80; i < 120; i++)
            //    {
            //        var c = new Data.Closet { Id = i + 1, IsVip = true, IsFree = true };
            //        if (i > 115)
            //            c.IsBroken = true;
            //        db.Closets.InsertOnSubmit(c);
            //    }
            //    db.SubmitChanges();
            //}
            //db.Closets.FirstOrDefault().UserId = 1;
            //db.Closets.Where(c => c.Id == 2 || c.Id == 3).ToList().ForEach(c =>
            //    { c.IsFree = false; c.RentorId = c.Id - 1; }
            //);
            //db.ClosetUsages.InsertOnSubmit(new ClosetUsage
            //{
            //    ClosetId = 1,
            //    MemberId = 1,
            //    FromTime = DateTime.Now.AddHours(-1)
            //});
            //db.Closets.Where(c => c.Id == 100).FirstOrDefault().UserId = 2;
            //db.Closets.Where(c => c.Id == 2).FirstOrDefault().UserId = 4;
            //db.SubmitChanges();
            //db.Passages.ToList().ForEach(p => p.Time = p.Time.AddDays((DateTime.Today - p.Time.Date).Days));
            //db.SubmitChanges();
            

            (new Login()).ShowDialog();
            SetBackground();
        }

        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {
            MessageSnackBar.IsActive = false;
        }
        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            System.Windows.MessageBox.Show(e.Exception.Message);
            e.Handled = true;
            //Environment.Exit(1);
            //   throw new NotImplementedException();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            var form = new Settings { Owner = this };
            form.Closed += Form_Closed;
            form.Show();

            //try
            //{
            //    (new Windows.Mentors()).Show(); return;
            //}
            //catch (Exception ex)
            //{
            //    ;
            //}
            //var sampleMessageDialog = new QuestionDialog
            //{
            //    Message = { Text = "خب بچه ها چطوره؟" }
            //};

            //await DialogHost.Show(sampleMessageDialog);
        }

        private void Form_Closed(object sender, EventArgs e)
        {
            try
            {
                this.Show();
            }
            catch (Exception)
            {
                
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void LoggedInUser_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            TransitList.UpdatePassages();
        }

        private void GoodsMenuItem_Click(object sender, RoutedEventArgs e)
        {
           
            var form = new Goods { Owner = this };
            form.Closed += Form_Closed;
            form.Show();
        }

        private void PosesMenuItem_Click(object sender, RoutedEventArgs e) {

            var form = new POS { Owner = this };
            form.Closed += Form_Closed;
            form.Show();
        }
        private void EnrollMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var form = new Enroll { Owner = this };
            form.Closed += Form_Closed;
            form.Show();
            //new MembersSearch().Show();
        }

        private void FacilitiesMenuItem_Click(object sender, RoutedEventArgs e)
        {

            var form = new Facilities { Owner = this };
            form.Closed += Form_Closed;
            form.Show();
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dependencyObject = Mouse.Captured as DependencyObject;
            if (dependencyObject is ToggleButton) return;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private void UsersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var form = new Users { Owner = this };
            form.Closed += Form_Closed;
            form.Show();
        }

        private void ClosetsMenuItem_Click(object sender, RoutedEventArgs e)
        {

            var form = new InitializeClosets { Owner = this };
            form.Closed += Form_Closed;
            form.Show();
        }

        private void TradesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var form = new SellReport { Owner = this };
            form.Closed += Form_Closed;
            form.Show();

        }

        private void MembersListMenuItem_Click(object sender, RoutedEventArgs e)
        {

            var form = new MembersSearch { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterScreen, Type = Domain.Enums.MemberSelectionCategory.None };
            form.Closed += Form_Closed;
            form.Show();
        }

        internal void AlertOrderPoints(List<Good> needOrder)
        {

        }

        void SetBackground() {

            var file = "CamelBackground.jpg";
            var back = (new Data.GymContextDataContext()).Settings.Where(s => s.Key == "background_image");
            if (back != null)
                file = back.FirstOrDefault().Value;
            if (!System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "/" + file))
                file = "CamelBackground.jpg";
            var _Image = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/" + file));
            BackGroundImageBrush.ImageSource = _Image;
            //Domain.Dynamics.BackgroundImage = "background.jpg";
            var db = new Data.GymContextDataContext();
            Data.Setting background = db.Settings.Where(s => s.Key == "background_image").FirstOrDefault();
            background.Value = "CamelBackground.jpg";
        }
        internal void ResetBackground()
        {
            var _Image = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory+"/CamelBackground.jpg"));
            BackGroundImageBrush.ImageSource = _Image;
            //Domain.Dynamics.BackgroundImage = "background.jpg";
            var db = new Data.GymContextDataContext();
            Data.Setting background = db.Settings.Where(s => s.Key == "background_image").FirstOrDefault();
            background.Value = "CamelBackground.jpg";
        }
        public void ChangeBackground_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "jpg images|*.jpg";
            if (ofd.ShowDialog() == true)
            {
                if (ofd.FileName != "")
                {
                    if (System.IO.File.Exists(ofd.FileName))
                    {
                        var _Image = new BitmapImage();
                        _Image.BeginInit();
                        _Image.StreamSource = System.IO.File.OpenRead(ofd.FileName);
                        _Image.EndInit();
                        BackGroundImageBrush.ImageSource = _Image;
                        var filename = ofd.FileName.Split('\\')[ofd.FileName.Split('\\').Count()-1];
                        if (filename.Length > 45) filename = Guid.NewGuid().ToString();
                        System.IO.File.Copy(ofd.FileName, System.AppDomain.CurrentDomain.BaseDirectory+ filename, true);
                        //Domain.Dynamics.BackgroundImage = "background.jpg";
                        var db = new Data.GymContextDataContext();
                        Data.Setting background = db.Settings.Where(s => s.Key == "background_image").FirstOrDefault();
                        if (background == null)
                        {
                            db.Settings.InsertOnSubmit(new Setting { Key = "background_image", Value = filename });
                            db.SubmitChanges();

                        }
                        else
                        {
                            background.Value = filename;
                            db.SubmitChanges();
                        }
                    }
                }
            }
        }


        public void btnRegularTransit_Click(object sender, RoutedEventArgs e)
        {
            var transit = new Windows.MembersSearch();
            transit.Type = Domain.Enums.MemberSelectionCategory.MembersTransit;
            transit.showIrregularMembers = false;
            transit.Owner = this;
            transit.Closed += Form_Closed;

            transit.Show();
        }

        public void btnIrregularTransit_Click(object sender, RoutedEventArgs e)
        {
            //var irregularEnter = new IrregularEnter();
            //irregularEnter.Show();


            var form = new IrregularEnter { Owner = this };
            form.Closed += Form_Closed;
            form.Show();
        }

        public void btnPersonnelTransit_Click(object sender, RoutedEventArgs e)
        {
            //var transit = new Windows.MembersSearch();
            //transit.Type = Domain.Enums.MemberSelectionCategory.PersonnelTransit;
            //transit.Show();


            var form = new MembersSearch { Owner = this, Type = Domain.Enums.MemberSelectionCategory.PersonnelTransit };
            form.Closed += Form_Closed;
            form.Show();
        }

        public void btnIrregularExit_Click(object sender, RoutedEventArgs e)
        {
            var form = new IrregularExit();
            form.Closed += Form_Closed;

            var dudesIn = form.LoadIrregularEnters();
            if (dudesIn > 0)
                form.Show();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void CoursesMenuItem_Click(object sender, RoutedEventArgs e)
        {

            var form = new Courses { Owner = this };
            form.Closed += Form_Closed;
            form.Show();
            //new Courses { Owner = this }.Show();
        }

        private void SportsMenuItem_Click(object sender, RoutedEventArgs e)
        {

            var form = new Sports { Owner = this };
            form.Closed += Form_Closed;
            form.Show();

            //new Sports { Owner = this }.Show();
        }

        private void CostsMenuItem_Click(object sender, RoutedEventArgs e)
        {

            var form = new CostCategories { Owner = this };
            form.Closed += Form_Closed;
            form.Show();

            //new CostCategories { Owner = this }.Show();
        }

        private void PresentMembersListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var form = new MembersSearch();
            form.showIrregularMembers = true;
            form.showOnlyPresentMembers = true;
            form.Owner = this;
            form.Closed += Form_Closed;
            form.Show();
        }

        private void SellGoodsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            
            var form = new Trades { Owner = this };
            form.Closed += Form_Closed;
            form.Show();
        }

        private void ChargeCreditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var form = new ChargeCredit { Owner = this };
            form.Closed += Form_Closed;
            form.Show();
        }

        private void MentorsListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var form = new Mentors { Owner = this };
            form.Closed += Form_Closed;
            form.Show();
        }



        private void btnMembersTransitReport_Click(object sender, RoutedEventArgs e)
        {

            var form = new PassageReport { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterScreen, Type = PassageReportType.MembersReport };
            form.Closed += Form_Closed;
            form.Show();

            //(new PassageReport { Owner = this, WindowStartupLocation=WindowStartupLocation.CenterScreen, Type = PassageReportType.MembersReport }).Show();
        }

        private void btnPersonnelTransitReport_Click(object sender, RoutedEventArgs e)
        {

            var form = new PassageReport { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterScreen, Type = PassageReportType.PersonnelReport };
            form.Closed += Form_Closed;
            form.Show();

            //(new PassageReport { Owner = this, WindowStartupLocation=WindowStartupLocation.CenterScreen, Type = PassageReportType.PersonnelReport }).Show();
            //(new TransitReport { Type = Domain.Enums.MemberSelectionCategory.PersonnelTransit, Owner = this }).Show();
        }

        private void DeleteMember_Click(object sender, RoutedEventArgs e)
        {

            var form = new MembersSearch { Type = Domain.Enums.MemberSelectionCategory.DeleteMembers, Owner = this };
            form.Closed += Form_Closed;
            form.Show();

            //(new MembersSearch { Type = Domain.Enums.MemberSelectionCategory.DeleteMembers, Owner = this }).Show();
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            var shallLogOut = AlertMembersWithoutExit();
            if (!shallLogOut) return;

            System.Windows.Application.Current.Shutdown();
        }

        private void LogOutUser_Click(object sender, RoutedEventArgs e)
        {
            var shallLogOut = AlertMembersWithoutExit();
            if (!shallLogOut) return;
            
            this.OwnedWindows.Cast<Window>().ToList().ForEach(f => f.Close());
            this.Hide();
            //this.IsEnabled = false;
            (new Login
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                //WindowStyle =WindowStyle.None ,
                //BorderThickness=new Thickness(0,0,0,0),
                //WindowState = WindowState.Maximized
            }).ShowDialog();

        }

        bool AlertMembersWithoutExit()
        {
            var db = new Data.GymContextDataContext();

            bool any = (from m in db.Members
                       where m.Passages.Any() &&
                       m.Passages.OrderByDescending(p => p.Id).FirstOrDefault().IsEntrance == true
                       select m).Any();

            if (any) return new AmbiguousMembersAlert().ShowDialog().Value;
            else return true;
        }

        //public static int CurrentUserId = 0;
        public static User CurrentUser = null;

        public void SetUser(int id)
        {
            var db = new Data.GymContextDataContext();
            var user = db.Users.Where(u => u.Id == id).FirstOrDefault();
            UserChip.Content = user.Username;
            CurrentUser = user; //.Id;

            dynamic icon = UserChip.Icon;

            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + $"/Images/User_{user.Id}.jpg"))
            {
                var _Image = new BitmapImage();
                _Image.BeginInit();
                _Image.StreamSource = System.IO.File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + $"/Images/User_{user.Id}.jpg");
                _Image.EndInit();

                //var image = UserChip.Icon.GetType().GetProperty("Icon").GetType().GetProperty("Image");
                //image.SetValue(image, _Image);
                icon.Source = _Image;
                icon.Stretch = Stretch.UniformToFill;
            }
            else
                icon.Source = null;

            int i = 0;
            var access = user.Role.Access;
            if (access.Length < 27) {
                access += new string('0', 27 - access.Length);
            }
            EnrollMenuItem.IsEnabled = access[i++] == '1';
            MembersListMenuItem.IsEnabled = access[i++] == '1';
            PresentMembersListMenuItem.IsEnabled = access[i++] == '1';
            DeleteMember.IsEnabled = access[i++] == '1';
            MentorsList.IsEnabled = access[i++] == '1';

            AddGoods.IsEnabled = access[i++] == '1';
            SellGoods.IsEnabled = access[i++] == '1';
            ChargeCredit.IsEnabled = access[i++] == '1';

            RepMemList.IsEnabled = access[i++] == '1';
            trades.IsEnabled = access[i++] == '1';
            userFinancialReport.IsEnabled = access[i++] == '1';
            memTransRep.IsEnabled = access[i++] == '1';
            personnelTransRep.IsEnabled = access[i++] == '1';
            closetsRep.IsEnabled = access[i++] == '1';
            irregTransRep.IsEnabled = access[i++] == '1';
            chart.IsEnabled = access[i++] == '1';

            logCost.IsEnabled = access[i++] == '1';
            viewCost.IsEnabled = access[i++] == '1';
            financialBalance.IsEnabled = access[i++] == '1';

            sports.IsEnabled = access[i++] == '1';
            facilities.IsEnabled = access[i++] == '1';
            courses.IsEnabled = access[i++] == '1';
            closets.IsEnabled = access[i++] == '1';
            costs.IsEnabled = access[i++] == '1';
            poses.IsEnabled = access[i++] == '1';
            users.IsEnabled = access[i++] == '1';
            roles.IsEnabled = access[i++] == '1';

            Domain.GymHost.Run();
        }

        private void GoodsBelowOrderPointBadged_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            var form = new Goods { FilterGoodsBelowOrderPoint = true, Owner = this };
            form.Closed += Form_Closed;
            form.Show();
            //(new Goods { FilterGoodsBelowOrderPoint = true, Owner = this }).Show();
        }

        private void CreditDetorsBadged_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var form = new MembersSearch { Owner = this };
            form.Type = Domain.Enums.MemberSelectionCategory.ShoppingDebtorsList;
            form.Closed += Form_Closed;
            form.Show();
        }

        private void NearExpiryBadged_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var form = new MembersSearch { Owner = this };
            form.Type = Domain.Enums.MemberSelectionCategory.NearExpiryList;
            form.Title = "نزدیک به تمدید";
            form.Closed += Form_Closed;
            form.Show();
        }

        private void TuitionDetorsBadged_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var form = new MembersSearch { Owner = this };
            form.Type = Domain.Enums.MemberSelectionCategory.TutitionDebtorsList;
            form.Title = "بدهکاران شهریه";
            form.Closed += Form_Closed;
            form.Show();
        }

        private void btnReportMembersList_Click(object sender, RoutedEventArgs e)
        {
            //var form = new MembersSearch();
            //form.Type = Domain.Enums.MemberSelectionCategory.None;
            //form.showOnlyPresentMembers = false;
            //form.showIrregularMembers = true;
            //form.Show();

            //new Gym.Windows.MainWindow().Show();

            var form = new MembersReport { Owner = this };
            form.Closed += Form_Closed;
            form.Show();
        }

        private void btnIrregularTransitReport_Click(object sender, RoutedEventArgs e)
        {
            var form = new PassageReport { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterScreen, Type = PassageReportType.SingleSessionsReport };
            form.Closed += Form_Closed;

            form.Show();

            //(new TransitReport { Type = Domain.Enums.MemberSelectionCategory.IrregularTransit, Owner = this }).Show();
        }

        private void PayCostsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var form = new PayCosts { Owner = this };
            form.Closed += Form_Closed;

            form.Show();
        }

        private void CostsReportMenuItem_Click(object sender, RoutedEventArgs e)
        {
           var form = (new CostsReport { Owner = this });
            form.Closed += Form_Closed;

            form.Show();
        }

        private void BalanceMenuItem_Click(object sender, RoutedEventArgs e)
        {
           var form = (new Balance { Owner = this });
            form.Closed += Form_Closed;

            form.Show();
        }

        private void UserFinancialReportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var form = new UserFinancialReport { Owner = this };
            form.Closed += Form_Closed;

            form.Show();
        }

        private void btnFinancialChartMenuItem_Click(object sender, RoutedEventArgs e)
        {
           var form=  (new FinancialChartReport { Owner = this });
            form.Closed += Form_Closed;

            form.Show();
        }

        private void RolesMenuItem_Click(object sender, RoutedEventArgs e)
        {

           var form = (new RoleManager { Owner = this });
            form.Closed += Form_Closed;

            form.Show();
        }

        private void ClosetsReportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ClosetUsageReport initClosets = new ClosetUsageReport { Owner = this };
            initClosets.Closed += Form_Closed;

            initClosets.Show();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            LastMinimize = DateTime.Now;
            this.WindowState = WindowState.Minimized;
        }

        private void btnFinancialChart2MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var form = (new FinancialChartReport2 { Owner = this });
            form.Closed += Form_Closed;

            form.Show();
            //(new FinancialChartReport2 { Owner = this }).Show();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Width > 50 && this.Visibility == Visibility.Visible) {
                ResizeClosets();
            }
        }

        public bool PassingMember(string code, DateTime date) {
            int finger = 0;
            int.TryParse(code, out finger);
            var db = new Data.GymContextDataContext();
            var member = db.Members.Where(m => m.FingerId == finger).FirstOrDefault();
            if (member == null) //finger-owner not found
            {
                if (Notifications.Tresspasses == "") Notifications.Tresspasses = "0";
                Notifications.Tresspasses = (int.Parse(Notifications.Tresspasses) + 1).ToString();
                return false;
            }
            else //finger-owner found
            {
                MembersSearch search = new MembersSearch();
                search.Closed += Form_Closed;
                search.Type = Domain.Enums.MemberSelectionCategory.MembersTransit;
                search.LoadSingleMember(member);

                try
                {
                    search.Show();
                    return true;
                }
                catch {
                    return false;
                }
            }
        }

        private void TresspassesBadged_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Notifications.Tresspasses = "";
        }

        private void TestOpen_Click(object sender, RoutedEventArgs e)
        {
            var svc = new Domain.GymService();
            svc.PassMember(txtInfo.Text, DateTime.Now);
        }



        public void Exit() {
            Application.Current.Shutdown();
        }

        private void CloseClosetDoor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(lblTitle.FontFamily.ToString());
        }
    }
}
