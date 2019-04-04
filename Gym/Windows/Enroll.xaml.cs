using Gym.Data;
using Gym.Domain;
using Gym.ViewModels;
using Microsoft.Expression.Encoder.Devices;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
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
    /// Interaction logic for Enroll.xaml
    /// </summary>
    public partial class Enroll : Window, INotifyPropertyChanged
    {

        //ObservableCollection<CourseVM> Courses
        //{
        //    get
        //    {
        //        return CourseSelector.Courses.Items;
        //    }
        //    set
        //    {
        //        var x = value;
        //    }
        //}

        public Visibility MemberVisible { get { return Section == 0 ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility CourseVisible { get { return Section == 1 ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility FinancialVisible { get { return Section == 2 ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility TransactionsVisible { get { return Section == 3 ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility HistoryVisible { get { return Section == 4 ? Visibility.Visible : Visibility.Collapsed; } }

        bool IsClosing { get; set; }
        bool NotClosing { get { return !IsClosing; } }

        Actions _State = Actions.None;
        public Actions State
        {
            get { return _State; }
            set
            {
                _State = value;
                OnPropertyChanged("IsEditing");
                OnPropertyChanged("IsNew");
                OnPropertyChanged("IsSearched");
                OnPropertyChanged("IsReadonly");

            }
        }
        public bool IsEditing { get { return State == Actions.Editing; } }
        public bool IsNew { get { return State == Actions.Inserting; } }
        public bool IsSearched { get { return State == Actions.Selected; } }
        public bool IsReadonly { get { return State == Actions.Selected || State == Actions.None; } }

        Actions _EnrollState = Actions.None;
        public Actions EnrollState
        {
            get { return _EnrollState; }
            set
            {
                _EnrollState = value;

                FacilitySelector.IsReadOnly = IsReadonlyEnroll;
                CourseSelector.IsReadOnly = IsReadonlyEnroll;
                pdFinishDate.IsReadOnly = pdStartDate.IsReadOnly = IsReadonlyEnroll;

                OnPropertyChanged("IsNewEnroll");
                OnPropertyChanged("IsEditingEnroll");
                OnPropertyChanged("IsSearchedEnroll");
                OnPropertyChanged("IsReadonlyEnroll");
            }
        }

        internal void LoadMember(int memberId)
        {
            //var member = new Controls.MemberCard(memberId).Member;
            var m = db.Members.Where(x => x.Id == memberId).FirstOrDefault();
            var member = new MemberVM
            {
                Id = m.Id,
                Firstname = m.Firstname,
                Lastname = m.Lastname,
                Address = m.Address,
                BirthDate = m.Birthdate?.ToEn(),
                Description = m.Description,
                Fathername = m.Dadsname,
                Mobile = m.Mobile,
                NationalCode = m.NationalCode,
                Referrer = m.Referrer,
                ReferrerMobile = m.ReferrerMobile,
                IsActive = m.IsActive,
                Credit = m.Credit,
                Debtor = m.Debtor,
                InsuranceNo = m.InsuranceNo,
                InsuranceExpireDate = m.InsuranceExpiry.ToEn(),
                ClosetId = db.Closets.Where(c => c.RentorId == m.Id).FirstOrDefault()?.Id,
                FingerId = m.FingerId
            };
            Mems_MemberSelected(member);
        }

        public bool IsEditingEnroll { get { return EnrollState == Actions.Editing; } }
        public bool IsNewEnroll { get { return EnrollState == Actions.Inserting; } }
        public bool IsSearchedEnroll { get { return EnrollState == Actions.Selected; } }
        public bool IsReadonlyEnroll { get { return EnrollState == Actions.Selected || EnrollState == Actions.None; } }


        Data.GymContextDataContext db = new Data.GymContextDataContext();
        int _Section = 0;

        public int Section
        {
            get { return _Section; }
            set
            {
                _Section = value;
                OnPropertyChanged("MemberVisible");
                OnPropertyChanged("CourseVisible");
                OnPropertyChanged("FinancialVisible");
                OnPropertyChanged("TransactionsVisible");
                OnPropertyChanged("HistoryVisible");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Enroll()
        {
            InitializeComponent();
            //Member = db.Members.Select(m => new MemberVM
            //{
            //    Firstname = m.Firstname,
            //    Lastname = m.Lastname
            //}).FirstOrDefault();


            progress.DataContext = this;
            this.DataContext = this;
            txtCredit.DataContext = this;
            txtOldDebtor.DataContext = this;
            Frequencies = new ObservableCollection<string>();
            Frequencies.Add("هر روز");
            Frequencies.Add("یک روز در میان");
            Frequencies.Add("جلسه ای");
            //cmbFrequency.DataContext = Frequencies;

            Days = new ObservableCollection<string>();
            for (int i = 1; i <= 30; i++)
            {
                Days.Add(i.ToString());
            }
            //cmbDaysCount.DataContext = Frequencies;

            Months = new ObservableCollection<string>();
            Months.Add("یک ماه");
            Months.Add("دو ماه");
            Months.Add("سه ماه");
            Months.Add("شش ماه");
            Months.Add("نه ماه");
            Months.Add("یک سال");
            //cmbMonthCount.DataContext = Frequencies;
            this.PreviewKeyUp += Window_PreviewKeyUp;
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (DebtorWarningtStack.Visibility == Visibility.Visible) {
                    DebtorWarningtStack.Visibility = Visibility.Collapsed;
                    return;
                }
                var escTime = (DateTime)(Dynamics.LastEscapeTime ?? DateTime.Now.AddDays(-1));
                if ((DateTime.Now - escTime) > TimeSpan.FromMilliseconds(100))
                    this.Close();
            }
            if (e.Key == Key.Enter)
            {

                var control = FocusManager.GetFocusedElement(this);
                if (control == null || (control is ComboBox && (control as ComboBox).Name.ToLower() == "cmbyear"))
                    return;
                else
                {
                    switch (section.SelectedIndex)
                    {
                        case 0:
                            btnSaveMember_Click(null, null);
                            break;
                        case 1:
                            btnEnroll_Click(null, null);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public ObservableCollection<string> Frequencies { get; set; }
        public ObservableCollection<string> Days { get; set; }
        public ObservableCollection<string> Months { get; set; }

        public int CurrentSectionCode { get { return section.SelectedIndex + 1; } set { int i = value; } }
        public EnrollVM EnrollViewModel = new EnrollVM();
        MemberVM _Member = new MemberVM();
        public MemberVM Member
        {
            get { return _Member; }
            set
            {
                _Member = value; OnPropertyChanged("Member");
                LoadMembersTransactions();
                LoadMembersHistory();
                //SetCoursesData();
            }
        }
        dynamic userTransactions;
        public void LoadMembersTransactions(TransactionType type = TransactionType.All)
        {
            if (Member.Id > 0)
            {
                var transactions = db.Transactions
                    .Where(t => t.MemberId == Member.Id
                        && t.Type == (type == TransactionType.All ? t.Type : (byte)type)).ToList()
                    .Select(t => new
                    {
                        Date = t.Datetime.ToFa(),
                        Amount = t.Amount.ToString("#,##0"),
                        Type = ((TransactionType)t.Type).GetName(),
                        Method = ((PaymentMethods)t.Method).GetName(),
                        User = t.User.Username,
                        Info = t.Info
                    }).ToList();
                userTransactions = transactions;
                TransactionsGrid.DataContext = transactions;
            }
        }
        dynamic userHistory;
        public void LoadMembersHistory()
        {
            if (Member.Id > 0)
            {
                var history = db.Enrolls
                    .Where(n => n.MemberId == Member.Id).ToList()
                    .Select(n => new HistoryVM
                    {
                        Type = "تمدید",
                        EnrollDate = n.EnrollDate.ToFa(),
                        StartDate = n.StartDate.HasValue ? n.StartDate.Value.ToFa() : "",
                        ExpireDate = n.ExpireDate.HasValue ? n.ExpireDate.Value.ToFa() : "",
                        Price = n.Price.ToString("#,##0"),
                        Discount = n.Discount.ToString("#,##0"),
                        Debtor = (n.Price - (n.Transaction != null ? n.Transaction.Amount : 0) - n.Discount).ToString("#,##0"),
                        Info = String.Join(",",n.EnrollCourses.Select(c => c.Course.Name).Distinct().
                        ToList())
                    }).ToList();
                if (history.Count > 0)
                    history.OrderBy(h=>h.EnrollDate).FirstOrDefault().Type = "ثبت نام";
                userHistory = history.OrderByDescending(h=>h.EnrollDate);
                HistoryGrid.DataContext = history;
            }
        }

        public MemberVM MemberShadow = null;
        public Data.Enroll CurrentEnroll;

        public ObservableCollection<string> MemberEnrolls { get; set; }

        ObservableCollection<Data.POS> PosList;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnNewEnroll.IsEnabled = btnNewPerson.IsEnabled = btnEditPerson.IsEnabled = btnSaveEnroll.IsEnabled = btnCancelEnroll.IsEnabled =
               btnPay.IsEnabled = btnChargeCredit.IsEnabled = Main.CurrentUser.Role.Access[0] == '1';


            PosList = new ObservableCollection<Data.POS>(db.POS.ToList());
            cmbPos.DataContext = PosList;

            pdStartDate.Date = DateTime.Now.ToFa();
            try
            {
                var VideoDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);
                WebcamViewer.VideoDevice = VideoDevices[0];
                WebcamViewer.AudioDevice = EncoderDevices.FindDevices(EncoderDeviceType.Audio)[0];
            }
            catch { }
            CourseSelector.Checked += CourseSelector_Checked;
            cmbFrequency.SelectionChanged += (z, x) =>
            {
                if (cmbFrequency.SelectedIndex == 2 && pdStartDate.Date.Length > 0)
                {
                    PersianCalendar pc = new PersianCalendar();
                    pdFinishDate.Date = pc.AddMonths(pdStartDate.Date.ToEn().Value, 1).ToFa();

                };
            };
            cmbMonthCount.SelectionChanged += (z, x) =>
            {
                PersianCalendar pc = new PersianCalendar();
                if (pdStartDate.Date.Length > 0)
                    switch (cmbMonthCount.SelectedIndex)
                    {
                        case 0:
                            pdFinishDate.Date = pc.AddMonths(pdStartDate.Date.ToEn().Value, 1).ToFa();
                            break;
                        case 1:
                            pdFinishDate.Date = pc.AddMonths(pdStartDate.Date.ToEn().Value, 2).ToFa();
                            break;
                        case 2:
                            pdFinishDate.Date = pc.AddMonths(pdStartDate.Date.ToEn().Value, 3).ToFa();
                            break;
                        case 3:
                            pdFinishDate.Date = pc.AddMonths(pdStartDate.Date.ToEn().Value, 6).ToFa();
                            break;
                        case 4:
                            pdFinishDate.Date = pc.AddMonths(pdStartDate.Date.ToEn().Value, 9).ToFa();
                            break;
                        case 5:
                            pdFinishDate.Date = pc.AddMonths(pdStartDate.Date.ToEn().Value, 12).ToFa();
                            break;
                        default:
                            break;
                    }
            };

            btnNewPerson_Click(null, null);
            btnNewEnroll_Click(null, null);
            //SetCoursesData();
        }


        private void CourseSelector_Checked(object sender, EventArgs e)
        {
            var hasVIP = CourseSelector.HasSelectedVIP;
            if (hasVIP)
            {
                FacilitySelector.ClearSelection();
            }
            FacilitySelector.IsEnabled = !hasVIP;
        }

        private void Memebr_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Course_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex == -1)
            {
                e.Handled = true;
                (e.RemovedItems[0] as ListBoxItem).IsSelected = true;
            }
            try
            {
                switch (section.SelectedIndex)
                {
                    case 0: //Member
                        MemberSection.Visibility = Visibility.Visible;
                        CourseSection.Visibility = Visibility.Collapsed;
                        FinancialSection.Visibility = Visibility.Collapsed;
                        TransactionsSection.Visibility = Visibility.Collapsed;
                        HistorySection.Visibility = Visibility.Collapsed;
                        break;
                    case 1: //Enroll
                        MemberSection.Visibility = Visibility.Collapsed;
                        CourseSection.Visibility = Visibility.Visible;
                        FinancialSection.Visibility = Visibility.Collapsed;
                        TransactionsSection.Visibility = Visibility.Collapsed;
                        HistorySection.Visibility = Visibility.Collapsed;
                        break;
                    case 2: //Financial
                        MemberSection.Visibility = Visibility.Collapsed;
                        CourseSection.Visibility = Visibility.Collapsed;
                        TransactionsSection.Visibility = Visibility.Collapsed;
                        FinancialSection.Visibility = Visibility.Visible;
                        HistorySection.Visibility = Visibility.Collapsed;
                        break;
                    case 3: //Transaction
                        MemberSection.Visibility = Visibility.Collapsed;
                        CourseSection.Visibility = Visibility.Collapsed;
                        FinancialSection.Visibility = Visibility.Collapsed;
                        TransactionsSection.Visibility = Visibility.Visible;
                        HistorySection.Visibility = Visibility.Collapsed;
                        break;
                    case 4: //History
                        MemberSection.Visibility = Visibility.Collapsed;
                        CourseSection.Visibility = Visibility.Collapsed;
                        FinancialSection.Visibility = Visibility.Collapsed;
                        TransactionsSection.Visibility = Visibility.Collapsed;
                        HistorySection.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }

        private void cmbFrequency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFrequency.SelectedIndex == (int)Domain.Enums.Frequencies.SingleSessions)
            {
                cmbDaysCount.Visibility = Visibility.Visible;
                cmbMonthCount.Visibility = Visibility.Collapsed;
            }
            else
            {
                cmbMonthCount.Visibility = Visibility.Visible;
                cmbDaysCount.Visibility = Visibility.Collapsed;

            }
            CourseSelector.SetFrequency((Frequencies)cmbFrequency.SelectedIndex);
        }

        private void cmbDaysCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDaysCount.SelectedIndex >= 0)
                CourseSelector.SetDuration(cmbDaysCount.SelectedIndex + 1);
        }

        private void cmbMonthCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = 0;
            switch (cmbMonthCount.SelectedIndex)
            {
                case 0: i = 1; break;
                case 1: i = 2; break;
                case 2: i = 3; break;
                case 3: i = 6; break;
                case 4: i = 9; break;
                case 5: i = 12; break;
            }
            CourseSelector.SetDuration(i);
        }
        bool ImageChanged = false;
        private void btnSetImage_Click(object sender, RoutedEventArgs e)
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
                        ImageBrush.ImageSource = _Image;
                        Member.Image = _Image;
                        ImageChanged = true;
                    }
                }
            }
        }

        private void btnClearImage_Click(object sender, RoutedEventArgs e)
        {
            ImageBrush.ImageSource = null;
        }

        private void btnNewPerson_Click(object sender, RoutedEventArgs e)
        {
            State = Actions.Inserting;
            Member = new MemberVM();
            ImageChanged = false;
            ImageBrush.ImageSource = Member.Image;

            ClearEnroll();
            if (MemberEnrolls != null)
                MemberEnrolls.Clear();

        }

        private void btnEditPerson_Click(object sender, RoutedEventArgs e)
        {
            State = Actions.Editing;
            MemberShadow = Member;

        }

        void ClearMember()
        {
            if (Member.Id > 0)
            {
                Member = new Controls.MemberCard(Member.Id).Member;
            }
            else
            {
                Member = new MemberVM();
                Birthdate.Clear();
            }
            MemberShadow = null;
        }

        Windows.MembersSearch mems;
        private void btnSearchMember_Click(object sender, RoutedEventArgs e)
        {


            mems = new MembersSearch();
            mems.showIrregularMembers = false;
            mems.showOnlyPresentMembers = false;
            mems.MemberSelected += Mems_MemberSelected;

            mems.ShowDialog();

        }

        private void Mems_MemberSelected(MemberVM member)
        {
            this.Member = member;
            if (db.Closets.Any(c => c.RentorId == member.Id))
                Member.ClosetId = db.Closets.Where(c => c.RentorId == member.Id).OrderByDescending(c => c.Id).FirstOrDefault().Id;

            if (member == null)
                State = Actions.None;
            else
                State = Actions.Selected;
            if (mems != null)
            {
                mems.Close();
                mems = null;
            }
            Birthdate.Date = member.BirthDateFa;

            var enrolls = db.Enrolls.Where(e => e.MemberId == member.Id);
            if (enrolls.Count() > 0)
                CurrentEnroll = enrolls.OrderByDescending(e => e.EnrollDate).First();
            else
                CurrentEnroll = new Data.Enroll();
            ClearEnroll();
            var memenrolls = db.Enrolls.Where(e => e.MemberId == member.Id).ToList().OrderByDescending(e => e.Id).Select(e => $"{e.EnrollDate.ToFa()} ({e.Id})").ToList();
            MemberEnrolls = new ObservableCollection<string>(memenrolls);
            OnPropertyChanged("MemberEnrolls");
            //if (ImageBrush.ImageSource == null && Member.Image != null)
            ImageBrush.ImageSource = Member.Image;

            if (MemberEnrolls.Count == 1)
                cmbEnrolls.SelectedIndex = 0;

            btnNewEnroll.Content = MemberEnrolls.Count > 0 ? "تمدید دوره" : "دوره جدید";
            pdStartDate.Date = DateTime.Now.ToFa();

            ImageChanged = false;
        }

        private void btnCancelMember_Click(object sender, RoutedEventArgs e)
        {
            ClearMember();
            State = Actions.None;
            btnNewPerson_Click(null, null);
        }

        private void btnSaveMember_Click(object sender, RoutedEventArgs e)
        {
            string imagePath = "";
            if (IsNew)
            {
                Member.Id = Member.Insert() ?? 0;

                if (Member.Id > 0)
                {
                    ShowToastMessage("عضو جدید با موفقیت ثبت شد", TimeSpan.FromSeconds(3));
                    btnNewEnroll_Click(null, null);
                }
                else
                {
                    ShowToastMessage("ثبت عضو جدید با خطا مواجه شد", TimeSpan.FromSeconds(10));
                    System.Windows.Forms.MessageBox.Show("Test");
                    return;
                }

            }
            if (IsEditing)
            {
                imagePath = Guid.NewGuid().ToString();
                Member.ImagePath = imagePath;
                var res = Member.Update();
                ShowToastMessage("اطلاعات عضو به روز شد", TimeSpan.FromSeconds(3));
            }
            SaveImageToFile(Member.ImagePath);
            State = Actions.Selected;
            Main.Home.Closets.LoadClosets();
        }

        private void btnEnroll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Member.Id == 0)
                {
                    //var messageQueue = MessageSnackBar.MessageQueue;
                    var message = "عضو انتخاب نشده است";

                    //the message queue can be called from any thread
                    //Task.Factory.StartNew(() => messageQueue.Enqueue(message));
                    Alert.Content = message;
                    MessageSnackBar.IsActive = true;
                    return;
                }

                if (!CourseSelector.Courses.Items.Cast<CourseVM>().ToList().Any(c => c.IsSelected))
                {
                    var message = "هیچ دوره ای برای ثبت نام انتخاب نشده است";

                    Alert.Content = message;
                    MessageSnackBar.IsActive = true;
                    return;
                }

                if (cmbFrequency.SelectedIndex == -1 ||
                    (cmbMonthCount.SelectedIndex == -1 && cmbDaysCount.SelectedIndex == -1) ||
                    string.IsNullOrEmpty(pdStartDate.Date) ||
                    string.IsNullOrEmpty(pdFinishDate.Date)) {

                    var message = "شروع دوره، پایان دوره، نوع ثبت نام، تعداد ماه/جلسات ثبت نام اجباری هستند";

                    Alert.Content = message;
                    MessageSnackBar.IsActive = true;
                    return;
                }

                var courseIds = CourseSelector.Courses.Items.Where(c => c.IsSelected).Select(c => c.Id).ToList();
                var startDate = pdStartDate.Date.ToEn();
                if (db.Enrolls.Any(en => en.MemberId == Member.Id &&
                 en.ExpireDate > DateTime.Today && en.EnrollCourses.Any(
                    c => courseIds.Contains(c.CourseId) && c.SessionsLeft > 0)))
                //en.EnrollCourses.Any(c =>
                //    en.StartDate == startDate)))
                {
                    var message = "دوره تکراری می باشد";

                    Alert.Content = message;
                    MessageSnackBar.IsActive = true;
                    return;
                }

                var frequency = (Frequencies)cmbFrequency.SelectedIndex;
                byte duration = 0;
                switch (frequency)
                {
                    case Enums.Frequencies.Everyday:
                    case Enums.Frequencies.EveryOtherDay:
                        duration = GetDurationInMonth();
                        break;
                    case Enums.Frequencies.SingleSessions:
                        duration = (byte)(cmbDaysCount.SelectedIndex + 1);
                        break;
                    default:
                        return;
                }

                Data.Enroll enroll = new Data.Enroll();
                enroll.MemberId = Member.Id;
                //enroll.Discount = txtDiscount.Value;
                enroll.Frequency = (byte)frequency;
                enroll.Duration = duration;
                enroll.EnrollDate = DateTime.Now;
                enroll.StartDate = pdStartDate.Date.ToEn();
                enroll.ExpireDate = pdFinishDate.Date.ToEn();
                enroll.IsActive = ActiveSwitch.IsChecked ?? false;
                enroll.Price = txtPayableAmount.Value;
                enroll.Sessions = 0;
                enroll.InsuranceFee = chkExtendInsurance.IsChecked.Value ? Domain.Dynamics.InsuranceFee : 0;
                enroll.UserId = Main.CurrentUser.Id;
                enroll.DailyPasses = txtDailyPasses.Value;

                Member.Debtor += enroll.Price;
                var member = db.Members.SingleOrDefault(m => m.Id == enroll.MemberId);
                member.Debtor += enroll.Price;
                if (chkExtendInsurance.IsChecked == true && pdInsuranceDate.Date != "")
                    member.InsuranceExpiry = pdInsuranceDate.Date;
                bool hasVIPcourse = false;
                CourseSelector.Courses.Items.Where(c => c.IsSelected).ToList().ForEach(course =>
                {
                    if (course.FreezeDays > enroll.Freeze)
                    {
                        enroll.Freeze = course.FreezeDays;
                    }

                    var sessions = 0;
                    switch (frequency)
                    {
                        case Enums.Frequencies.Everyday:
                            sessions = 30 * course.Duration; break;
                        case Enums.Frequencies.EveryOtherDay:
                            sessions = 15 * GetDurationInMonth(); break;
                        case Enums.Frequencies.SingleSessions:
                            sessions = (byte)(cmbDaysCount.SelectedIndex + 1);
                            break;
                        default:
                            return;
                    }
                    enroll.EnrollCourses.Add(new Data.EnrollCourse { CourseId = course.Id, MentorId = course.MentorId, SessionsLeft = sessions });
                    if (course.IsVIP)
                    {
                        hasVIPcourse = true;
                        var vipFacilities = db.Facilities.ToList();
                        vipFacilities.ForEach(f =>
                        {
                            var vipFacilitiesAlreadyAdded = enroll.EnrollFacilities.Any(ef => ef.FacilityId == f.Id);
                            if (!vipFacilitiesAlreadyAdded)
                                enroll.EnrollFacilities.Add(new Data.EnrollFacility { FacilityId = f.Id, SessionsLeft = f.Sessions });
                        });
                    }
                });

                if (!hasVIPcourse)
                    FacilitySelector.Facilities.Items.Where(f => f.IsSelected).ToList().ForEach(facility =>
                    {
                        enroll.EnrollFacilities.Add(new Data.EnrollFacility { FacilityId = facility.Id, SessionsLeft = facility.Sessions });
                    });

                db.Enrolls.InsertOnSubmit(enroll);
                db.SubmitChanges();

                ShowToastMessage("ثبت نام دوره با موفقیت انجام شد", TimeSpan.FromSeconds(3));

                CurrentEnroll = enroll;
                //SaveImageToFile();
                UpdateEnrollDebtor();
                LoadMembersHistory();
                LoadMembersTransactions();
                var memenrolls = db.Enrolls.Where(en => en.MemberId == Member.Id).ToList().OrderByDescending(en => en.Id).Select(en => $"{en.EnrollDate.ToFa()} ({en.Id})").ToList();
                MemberEnrolls = new ObservableCollection<string>(memenrolls);
                OnPropertyChanged("MemberEnrolls");
                if (memenrolls.Count > 0) cmbEnrolls.SelectedIndex = 0;
                Section = 2;
            }
            catch (Exception ex)
            {
                ;
            }
        }

        private void SaveImageToFile(string fileName)
        {
            if (Member.Image != null) {
                if (ImageChanged) {
                    var address = $@".\Images\{fileName}.jpg";
                    //if (System.IO.File.Exists(address))
                        //System.IO.File.Delete(address);
                    Member.Image.Save(address);
                    
                }
            }
        }

        byte GetDurationInMonth()
        {
            switch (cmbMonthCount.SelectedIndex)
            {
                case 0: return 1;
                case 1: return 2;
                case 2: return 3;
                case 3: return 6;
                case 4: return 9;
                case 5: return 12;
                default: return 0;
            }
        }

        byte ConvertMonthToDuration(int month)
        {
            switch (month)
            {
                case 1: return 0;
                case 2: return 1;
                case 3: return 2;
                case 6: return 3;
                case 9: return 4;
                case 12: return 5;
                default: return 0;
            }
        }


        PayingItems PaymentItem = PayingItems.None;
        private void UserDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            try
            {
                // Display webcam video
                WebcamViewer.StopPreview();
            }
            catch { }
            if (eventArgs.Parameter is int)
            {
                if ((int)eventArgs.Parameter == 1)
                {
                    IsClosing = true;
                    this.Close();
                }
                return;
            }
            var confirmed = (bool)eventArgs.Parameter;
            if (confirmed)
            {
                if ((Member?.Id ?? 0) > 0)
                {
                    if (PaymentItem == PayingItems.CourseTuition && (CurrentEnroll?.Id ?? 0) == 0)
                    {
                        ShowToastMessage("دوره ای جهت پرداخت انتخاب نشده است", TimeSpan.FromSeconds(10));
                        eventArgs.Cancel();
                        return;
                    }
                    if (PaymentItem != PayingItems.GrantDiscount && txtChargeCredit.Value <= 0)
                    {
                        ShowToastMessage("مبلغ پرداختی را به طور صحیح وارد نمایید", TimeSpan.FromSeconds(10));
                        eventArgs.Cancel();
                        return;
                    }
                    byte method = (byte)(rdCash.IsChecked == true ? 0 : (rdPos.IsChecked == true ? 1 : (rdCard.IsChecked == true ? 2 : (rdCheque.IsChecked == true ? 4 : 255))));
                    if (PaymentItem == PayingItems.CourseTuition && method == 255)
                    {
                        ShowToastMessage("روش پرداخت را مشخص نکرده اید", TimeSpan.FromSeconds(10));
                        eventArgs.Cancel();
                        return;
                    }
                    //if (PaymentItem == PayingItems.GrantDiscount && txtDiscountToGrant.Value == 0)
                    //{
                    //    ShowToastMessage("مبلغ تخفیف را وارد نمایید", TimeSpan.FromSeconds(10));
                    //    eventArgs.Cancel();
                    //    return;
                    //}
                    if (PaymentItem == PayingItems.GrantDiscount && (CurrentEnroll?.Id ?? 0) == 0)
                    {
                        ShowToastMessage("دوره ای جهت تخفیف انتخاب نشده است", TimeSpan.FromSeconds(10));
                        eventArgs.Cancel();
                        return;
                    }

                    if (method == 1 && cmbPos.SelectedIndex == -1)
                    {

                        ShowToastMessage("دستگاه کارتخوان انتخاب نشده است", TimeSpan.FromSeconds(10));
                        eventArgs.Cancel();
                        return;
                    }

                    Data.Transaction transaction = new Data.Transaction();
                    transaction.Amount = txtChargeCredit.Value;
                    transaction.Datetime = DateTime.Now;
                    transaction.Info = txtInfo.Text;
                    transaction.MemberId = Member.Id;
                    transaction.Method = method;
                    transaction.UserId = Main.CurrentUser.Id;
                    transaction.PosId = method == 1 ? (int)cmbPos.SelectedValue : default(int?);

                    switch (PaymentItem)
                    {
                        case PayingItems.GrantDiscount:

                            if (txtDiscountToGrant.Value >= 0)
                            {
                                var en = db.Enrolls.Where(e => e.Id == CurrentEnroll.Id).FirstOrDefault();
                                var diff = en.Discount - txtDiscountToGrant.Value;
                                en.Discount = txtDiscountToGrant.Value;
                                var mem = db.Members.Where(m => m.Id == Member.Id).FirstOrDefault();
                                mem.Debtor += diff;
                                Member.Debtor = mem.Debtor;
                                db.SubmitChanges();

                                UpdateEnrollDebtor();
                                LoadMembersTransactions();
                            }
                            break;
                        case PayingItems.CourseTuition:
                            //var enroll = db.Enrolls.Where(e => e.Id == CurrentEnroll.Id).FirstOrDefault();

                            //db.EnrollPayments.InsertOnSubmit(new Data.EnrollPayment { EnrollId = CurrentEnroll.Id, PaymentId = transaction.Id });
                            //transaction.EnrollPayments.Add(new Data.EnrollPayment { EnrollId = CurrentEnroll.Id });
                            transaction.Type = (byte)TransactionType.Tuition;
                            db.Members.Where(m => m.Id == Member.Id).FirstOrDefault().Debtor -= transaction.Amount;
                            Member.Debtor -= transaction.Amount;
                            //UpdateEnrollDebtor();
                            db.Transactions.InsertOnSubmit(transaction);
                            db.SubmitChanges();

                            db.EnrollPayments.InsertOnSubmit(new EnrollPayment { EnrollId = CurrentEnroll.Id, PaymentId = transaction.Id });
                            db.SubmitChanges();


                            UpdateEnrollDebtor();
                            LoadMembersTransactions();

                            break;
                        case PayingItems.ChargeCredit:
                            Member.Credit += transaction.Amount;
                            db.Members.Where(m => m.Id == Member.Id).FirstOrDefault().Credit += transaction.Amount;
                            transaction.Type = (byte)TransactionType.Credit;
                            db.Transactions.InsertOnSubmit(transaction);
                            db.SubmitChanges();
                            LoadMembersTransactions();
                            break;
                        case PayingItems.None:
                            return;
                    }

                    txtChargeCredit.Clear();
                    txtInfo.Clear();

                    //if (transaction.Id > 0)
                    //{
                    //    var mc = Member.Credit;
                    //    var dmc = db.Members.Where(m => m.Id == 1).FirstOrDefault().Credit;
                    //}
                }
            }
            else
            {
                Dynamics.LastEscapeTime = DateTime.Now;
            }
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPayTuition_Click(object sender, RoutedEventArgs e)
        {

            DiscountStack.Visibility = Visibility.Collapsed;
            DebtorWarningtStack.Visibility = Visibility.Collapsed;
            PaymentStack.Visibility = Visibility.Visible;
            PaymentItem = PayingItems.CourseTuition;
            rdCard.IsChecked = rdCash.IsChecked = rdCheque.IsChecked = rdPos.IsChecked = false;
        }

        private void btnChargeCredit_Click(object sender, RoutedEventArgs e)
        {
            PaymentItem = PayingItems.ChargeCredit;
            rdCard.IsChecked = rdCash.IsChecked = rdCheque.IsChecked = rdPos.IsChecked = false;
        }

        private void btnEditEnroll_Click(object sender, RoutedEventArgs e)
        {
            EnrollState = Actions.Editing;

        }

        private void btnNewEnroll_Click(object sender, RoutedEventArgs e)
        {
            EnrollState = Actions.Inserting;
            pdStartDate.Date = DateTime.Now.ToFa();
            txtFreezeLeft.Value = 0;
            txtDailyPasses.Value = 1;
            CurrentEnroll = new Data.Enroll();
            if (CourseSelector.CourseList.Items.Cast<CourseVM>().ToList().Any(c => c.IsVIP && c.IsSelected))
            {
                FacilitySelector.SetEnroll(null);
            }
        }

        void UpdateEnrollDebtor()
        {
            db = new GymContextDataContext();
            if (CurrentEnroll == null) return;
            var enroll = db.Enrolls.Where(e => e.Id == CurrentEnroll.Id).FirstOrDefault();
            int payed = 0;
            if (db.Transactions.Where(t => t.EnrollPayments.Where(p => p.EnrollId == enroll.Id).Count() > 0).Count() > 0)
                payed = db.Transactions.Where(t => t.EnrollPayments.Any(p => p.EnrollId == enroll.Id))?.Sum(t => t.Amount) ?? 0;
            var debt = enroll.Price - payed - enroll.Discount;
            //db.EnrollPayments.Where(ep=>ep.EnrollId == enroll.Id).Sum(p => p.Transaction.Amount);
            txtEnrollDebtor.Text = debt.ToString();
            txtDiscount.Value = enroll.Discount;
        }

        private void cmbEnrolls_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                EnrollState = Actions.None;
                //FacilitySelector.IsReadOnly = false;
                //CourseSelector.IsReadOnly = false;
                //pdFinishDate.IsReadOnly = pdStartDate.IsReadOnly = false;
            }
            else
            {
                db = new GymContextDataContext();
                EnrollState = Actions.Selected;
                var enrollId = 0;
                enrollId = int.Parse(e.AddedItems[0].ToString().Split('(')[1].Split(')')[0]);

                var enroll = db.Enrolls.SingleOrDefault(en => en.Id == enrollId);
                cmbFrequency.SelectedIndex = (int)enroll.Frequency;

                var frequency = (Frequencies)cmbFrequency.SelectedIndex;
                byte duration = enroll.Duration;
                switch (frequency)
                {
                    case Enums.Frequencies.Everyday:
                    case Enums.Frequencies.EveryOtherDay:
                        cmbMonthCount.SelectedIndex = ConvertMonthToDuration(enroll.Duration);
                        break;
                    case Enums.Frequencies.SingleSessions:
                        cmbDaysCount.SelectedIndex = duration - 1;
                        break;
                    default:
                        return;
                }
                pdStartDate.Date = enroll.StartDate?.ToFa();
                pdFinishDate.Date = enroll.ExpireDate?.ToFa();
                chkExtendInsurance.IsChecked = enroll.InsuranceFee > 0;

                CourseSelector.SetEnroll(enrollId);
                FacilitySelector.SetEnroll(enrollId);

                CurrentEnroll = enroll;

                txtFreezeLeft.Value = enroll.Freeze;
                txtFreeze.Value = 0;
                txtDailyPasses.Value = enroll.DailyPasses;


                if (CourseSelector.CourseList.Items.Cast<CourseVM>().ToList().Any(c => c.IsVIP && c.IsSelected))
                {
                    FacilitySelector.SetEnroll(null);
                }

                UpdateEnrollDebtor();
                //FacilitySelector.IsReadOnly = true;
                //CourseSelector.IsReadOnly = true;
                //pdFinishDate.IsReadOnly = pdStartDate.IsReadOnly = true;
            }
        }

        private void btnCancelEnroll_Click(object sender, RoutedEventArgs e)
        {
            var decap_enrolls = db.Enrolls.Where(r => r.MemberId == 4).ToList();
            EnrollState = Actions.None;
            ClearEnroll();
            btnNewEnroll_Click(null, null);
            cmbEnrolls.SelectedIndex = -1;
        }

        private void ClearEnroll()
        {
            pdFinishDate.Clear();
            pdStartDate.Clear();
            chkExtendInsurance.IsChecked = false;
            cmbDaysCount.SelectedIndex = -1;
            cmbMonthCount.SelectedIndex = -1;
            cmbFrequency.SelectedIndex = -1;
            pdInsuranceDate.Clear();
            txtInsuranceNo.Text = "";
            CourseSelector.SetEnroll(null);
            FacilitySelector.SetEnroll(null);
        }

        private void chkExtendInsurance_Checked(object sender, RoutedEventArgs e)
        {
            pdInsuranceDate.IsReadOnly = false;
            txtInsuranceFee.Value = Domain.Dynamics.InsuranceFee;
        }

        private void chkExtendInsurance_Unchecked(object sender, RoutedEventArgs e)
        {
            pdInsuranceDate.IsReadOnly = true;
            txtInsuranceFee.Text = "0";

        }

        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {
            MessageSnackBar.IsActive = false;
        }

        private void btnReserveCloset_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show(txtFirstname.FontFamily.ToString());return;
            InitializeClosets closetSelector = new InitializeClosets { Owner = this, Type = ClosetFormType.Selection };
            closetSelector.Show();
            closetSelector.ClosetSelected += (id) =>
            {
                Member.ClosetId = id;
            };

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            AlertHeader.Visibility = AlertContent.Visibility = Member.Debtor > 0 ? Visibility.Visible : Visibility.Collapsed;
            if (!IsClosing)//&& (Member.Debtor > 0))
            {
                //
                e.Cancel = true;
                DiscountStack.Visibility = Visibility.Collapsed;
                PaymentStack.Visibility = Visibility.Collapsed;
                DebtorWarningtStack.Visibility = Visibility.Visible;
                PaymentDialogHost.IsOpen = true;
            }
            else
            {
                Main.Home.CheckupTuitionDebtors();
                Main.Home.CheckupCreditDebtors();
                Main.Home.CheckupExtenders();
                ImageBrush.ImageSource = null;
                WebcamViewer = null;
                System.IO.Directory.GetFiles(@".\Images", "Snapshot*").ToList().ForEach(snapshot =>
                 {
                     try
                     {
                         System.IO.File.Delete(snapshot);
                     }
                     catch (Exception x) { }
                 });

                Application.Current.MainWindow.Activate();
            }
        }

        bool isFirstCamLoad = true;
        private void btnWebcam_Click(object sender, RoutedEventArgs e)
        {

            MemberGrid.Visibility = Visibility.Collapsed;
            WebcamStack.Visibility = Visibility.Visible;
            PersonButtonBar.IsEnabled = false;
            try
            {
                // Display webcam video
                WebcamViewer.StartPreview();
                if (isFirstCamLoad)
                {

                    WebcamViewer.StopPreview();
                    isFirstCamLoad = false;
                    WebcamViewer.StartPreview();

                }
            }
            catch (Microsoft.Expression.Encoder.SystemErrorException ex)
            {
                MessageBox.Show("Device is in use by another application");
            }


            //PaymentStack.Visibility = Visibility.Collapsed;
            ////WebcamViewer.TakeSnapshot();
            //WebcamStack.Visibility = Visibility.Visible;
            //PaymentDialogHost.IsOpen = true;
            //try
            //{
            //    // Display webcam video
            //    WebcamViewer.StopPreview();
            //}
            //catch { }
            ////MemberGrid.Visibility = Visibility.Visible;
            ////WebcamStack.Visibility = Visibility.Collapsed;

            ////MemberGrid.Visibility = Visibility.Collapsed;
            ////WebcamStack.Visibility = Visibility.Visible;
            //try
            //{
            //    // Display webcam video
            //    WebcamViewer.StartPreview();
            //    //if (isFirstCamLoad)
            //    //{
            //    //    WebcamViewer.StopPreview();
            //    //    isFirstCamLoad = false;
            //    //    WebcamViewer.StartPreview();

            //    //    WebcamStack.Visibility = Visibility.Visible;

            //    //    WebcamStack.Visibility = Visibility.Collapsed;
            //    //}
            //}
            //catch (Microsoft.Expression.Encoder.SystemErrorException ex)
            //{
            //    MessageBox.Show("Device is in use by another application");
            //}
        }

        List<string> tempImages = new List<string>();
        private void btnWebcamSnaphot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var x = WebcamViewer.TakeSnapshot();
                WebcamViewer.StopPreview();
                tempImages.Add(x);

                if (System.IO.File.Exists(x))
                {
                    var _Image = new BitmapImage();
                    _Image.BeginInit();
                    _Image.StreamSource = System.IO.File.OpenRead(x);
                    _Image.EndInit();
                    //ImageBrush.ImageSource = _Image;
                    Member.Image = _Image;
                    ImageBrush.ImageSource = Member.Image;
                }

                MemberGrid.Visibility = Visibility.Visible;
                WebcamStack.Visibility = Visibility.Collapsed;
                PersonButtonBar.IsEnabled = true;
            }
            catch { }
        }

        private void btnWebcamSnaphotCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Display webcam video
                WebcamViewer.StopPreview();
            }
            catch { }
            MemberGrid.Visibility = Visibility.Visible;
            WebcamStack.Visibility = Visibility.Collapsed;
            PersonButtonBar.IsEnabled = true;
        }

        void ShowToastMessage(string message, TimeSpan duration)
        {

            //Alert.Content = message;
            //MessageSnackBar.IsActive = true;
            //return;
            Alert.Content = message;
            MessageSnackBar.IsActive = true;
            var started = DateTime.Now;

            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (x, z) =>
            {
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                sw.Start();
                while (sw.ElapsedMilliseconds <= duration.TotalMilliseconds)
                    ;
            };
            bg.RunWorkerCompleted += (x, z) =>
            {
                MessageSnackBar.IsActive = false;
            };
            bg.RunWorkerAsync();

            //System.Threading.Thread t = new System.Threading.Thread(() =>
            //{
            //    while (DateTime.Now.Subtract(started).TotalSeconds < duration.TotalSeconds)
            //    { }

            //    MessageSnackBar.Dispatcher.Invoke(() =>
            //    {
            //    });
            //});
            //t.Start();
        }

        private void btnFreeze_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentEnroll != null)
            {
                if (txtFreeze.Value > 0)
                {
                    var freeze = txtFreeze.Value;
                    if (freeze <= txtFreezeLeft.Value)
                    {
                        db = new GymContextDataContext();
                        var enroll = db.Enrolls.Where(en => en.Id == CurrentEnroll.Id).FirstOrDefault();
                        var newExpire = pdFinishDate.Date.ToEn().Value.AddDays(freeze);
                        enroll.Freeze = enroll.Freeze - freeze;
                        enroll.ExpireDate = newExpire;
                        db.SubmitChanges();
                        //CurrentEnroll.ExpireDate = CurrentEnroll.ExpireDate.Value.Add(TimeSpan.FromDays(txtFreeze.Value));
                        pdFinishDate.Date = enroll.ExpireDate.Value.ToFa();
                        txtFreezeLeft.Value = enroll.Freeze;
                        txtFreeze.Value = 0;
                    }
                    else
                    {
                        ShowToastMessage($"شما مجاز هستید این دوره را نهایتا {txtFreezeLeft.Value} روز فریز کنید", TimeSpan.FromSeconds(3));
                    }
                }
                else
                {
                    ShowToastMessage($"تعداد روز جهت فریز را وارد کنید", TimeSpan.FromSeconds(3));

                }
            }
            else
            {
                ShowToastMessage($"دوره انتخاب نشده است", TimeSpan.FromSeconds(3));

            }
        }

        private void btnGrantDiscount_Click(object sender, RoutedEventArgs e)
        {
            txtDiscountToGrant.Value = txtDiscount.Value;
            DiscountStack.Visibility = Visibility.Visible;
            DebtorWarningtStack.Visibility = Visibility.Collapsed;
            PaymentStack.Visibility = Visibility.Collapsed;
            PaymentItem = PayingItems.GrantDiscount;
            //rdCard.IsChecked = rdCash.IsChecked = rdCheque.IsChecked = rdPos.IsChecked = false;
        }

        private void ExcelButton_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackageCreate();
        }

        public void ExcelPackageCreate()
        {
            try
            {
                //FileInfo template = new FileInfo(AppDomain.CurrentDomain.BaseDirectory +
                //    @"\template.xlsx");

                //FileInfo newFile = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory +
                //    @"\report-" + Guid.NewGuid().ToString() + ".xlsx");

                var hssfWorkbook = new HSSFWorkbook(
                    File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + @"\template.xls"));
                ISheet sheet = hssfWorkbook.GetSheetAt(0);
                int i = 0;
                ((IList<dynamic>)userTransactions).ToList().ForEach(r =>
                {
                    IRow row = sheet.GetRow(2 + i);
                    row.Cells[0] = r.Date;
                    row.Cells[1] = r.Amount;
                    row.Cells[2] = r.Type;
                    row.Cells[3] = r.Method;
                    row.Cells[4] = r.User;
                    row.Cells[5] = r.Info;
                    //WriteToFile();

                    i++;
                });

                FileStream file = new FileStream(@"test.xls", FileMode.Create);
                hssfWorkbook.Write(file);

                file.Close();
                //


                // Using the template to create the newFile...
                //using (ExcelPackage excelPackage = new ExcelPackage(newFile, template))
                //{
                //    // Getting the complete workbook...
                //    ExcelWorkbook myWorkbook = excelPackage.Workbook;

                //    // Getting the worksheet by its name...
                //    ExcelWorksheet myWorksheet = myWorkbook.Worksheets["Sheet1"];

                //    // Setting the value 77 at row 5 column 1...
                //    myWorksheet.Cell(5, 1).Value = 77.ToString();

                //    // Saving the change...
                //    excelPackage.Save();
                //}

                //TempData["Message"] = "Excel report created successfully!";

                return;
            }
            catch (Exception ex)
            {
                //TempData["Message"] = "Oops! Something went wrong.";

                return;
            }
        }

        private void rdPosChecked(object sender, RoutedEventArgs e)
        {
            cmbPos.Visibility = Visibility.Visible;
        }

        private void rdPosUnchecked(object sender, RoutedEventArgs e)
        {
            cmbPos.Visibility = Visibility.Collapsed;

        }
        

        private void EditHistoryItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TxtFreeze_GotFocus(object sender, RoutedEventArgs e)
        {
            txtFreeze.SelectAll();
        }
    }

    internal class HistoryVM
    {
        public string Type { get; set; }
        public string EnrollDate { get; set; }
        public string StartDate { get; set; }
        public string ExpireDate { get; set; }
        public string Price { get; set; }
        public string Discount { get; set; }
        public string Debtor { get; set; }
        public string Info { get; set; }
    }
}
