using Gym.Controls;
using Gym.Domain;
using Gym.ViewModels;
using MaterialDesignThemes.Wpf;
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
using static Gym.Domain.Enums;

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for MembersSearch.xaml
    /// </summary>
    public partial class MembersSearch : Window
    {
        public MembersSearch()
        {
            InitializeComponent();
            //Closing += (s, e) =>
            //    {
            //        if (!HasSelectedMember)
            //        {
            //            Card_MemberSelected(null);
            //        }
            //    };
        }
        Data.GymContextDataContext db = new Data.GymContextDataContext();
        public bool showOnlyPresentMembers = false;
        public bool showIrregularMembers = true;
        bool formIsLoading = true;

        MemberSelectionCategory _Type = MemberSelectionCategory.SelectionOnly;
        public MemberSelectionCategory Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
                switch (value)
                {
                    case MemberSelectionCategory.SelectionOnly:
                        break;
                    case MemberSelectionCategory.DeleteMembers:
                        this.Title = "حذف عضو";
                        Staff = false;
                        Mentors = false;

                        break;
                    case MemberSelectionCategory.MembersTransit:
                        this.Width = 800;
                        this.Height = 327;
                        this.Title = "ثبت ورود و خروج اعضا";
                        break;
                    case MemberSelectionCategory.PersonnelTransit:
                        this.Width = 800;
                        this.Height = 327;
                        //zone.Mode = ColorZoneMode.Standard;
                        zone.Background = new SolidColorBrush(Color.FromRgb(255, 193, 7));
                        Staff = true;
                        Mentors = true;
                        this.Title = "ثبت تردد پرسنل و مربیان";
                        break;
                    default:
                        break;
                }
            }
        }

        private void ViewType_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (MembersArea != null)

                if (ViewType.SelectedIndex == 0) //small
                {
                    MembersArea.Children.Cast<MemberCard>().ToList().ForEach(c => c.SetGraphics(MemberCard.MemberGraphics.Chip));
                }
                else if (ViewType.SelectedIndex == 1)//card
                {
                    MembersArea.Children.Cast<MemberCard>().ToList().ForEach(c => c.SetGraphics(MemberCard.MemberGraphics.Card));
                }
                else //grid
                {
                    MembersArea.Children.Cast<MemberCard>().ToList().ForEach(c => c.SetGraphics(MemberCard.MemberGraphics.Grid));
                }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var text = txtSearchBox.Text;
            ListMembers(text);
        }
        public delegate void MemberEvent(MemberVM member);
        public event MemberEvent MemberSelected;
        bool HasSelectedMember = false;
        MemberVM SelectedMember = null;
        public bool Mentors = false;
        public bool Staff = false;
        public List<MemberFacility> PayedFacilities = new List<MemberFacility>();
        public List<MemberCourse> PayedCourses = new List<MemberCourse>();
        MemberInformation Info = new MemberInformation() { Owner = Main.Home };

        //public void PreSelect(ViewModels.MemberVM member)
        //{
        //    automatic = true;
        //    Card_MemberSelected(member);
        //}
        bool automatic = false;
        private void Card_MemberSelected(ViewModels.MemberVM member)
        {
            if (Type == MemberSelectionCategory.SelectionOnly)
            {
                if (MemberSelected != null)
                    MemberSelected(member);

                if (member != null)
                    this.Close();
            }
            else if (Type == MemberSelectionCategory.MembersTransit)
            {
                db = new Data.GymContextDataContext();
                var lastTransit = db.Passages.Where(p => p.MemberId == member.Id).OrderByDescending(p => p.Time).FirstOrDefault();
                var isEntrance = !(lastTransit?.IsEntrance ?? false);

                var mmbr = db.Members.Where(m => m.Id == member.Id).FirstOrDefault();
                var nrols = mmbr.Enrolls.Where(e => e.StartDate <= DateTime.Today
                                    && (e.ExpireDate == null || e.ExpireDate >= DateTime.Today)).ToList();



                if (isEntrance && !nrols.Any())
                {
                    Alert.Content = $"در حال حاضر {member.Fullname} هیچ دوره فعالی ندارند";
                    MessageSnackBar.IsActive = true;
                    Info.SetMember(member.Id, "غیر مجاز: دوره فعال ندارد", null);
                    (Application.Current.MainWindow as Main).Focus();
                    Info.Show();
                    Info.Focus();
                    return;
                }

                var nc = nrols.SelectMany(n => n.EnrollCourses).ToList();
                var todayTransits = db.Passages.Where(p => p.MemberId == member.Id &&
                p.IsEntrance && p.Time >= DateTime.Today && p.Time < DateTime.Today.AddDays(1)).Count();

                if (isEntrance && nrols.Count == 1 && todayTransits >= nrols.FirstOrDefault().DailyPasses)
                {

                    Alert.Content = $" تجاوز از تعداد تردد مجاز روزانه";
                    MessageSnackBar.IsActive = true;
                    Info.SetMember(member.Id, "غیر مجاز: تردد بیش از حد مجاز روزانه", null);
                    Info.Show();
                    Info.Focus();
                    return;
                }

                if (!nc.Any(c => c.SessionsLeft > 0))
                {
                    if (isEntrance)
                    {
                        Alert.Content = $"{member.Fullname} از تمام جلسات خود استفاده کرده اند و اجازه ورود ندارند";
                        MessageSnackBar.IsActive = true;
                        Info.SetMember(member.Id, "غیر مجاز: جلسات به پایان رسیده است", null);
                        Info.Show();
                        return;
                    }
                }

                //var courses = nc.Select(n => n.Course).ToList();

                PayedCourses = nc.Select(c =>
                    new MemberCourse
                    {
                        EId = c.EnrollId,
                        CId = c.CourseId,
                        Name = c.Course.Name,
                        SessionsLeft = c.SessionsLeft,
                        ExpiresAt = c.Enroll.ExpireDate,
                        Used = false
                    }
                ).ToList();

                var nf = nrols.SelectMany(n => n.EnrollFacilities).ToList();

                PayedFacilities = nf.Select(f =>
                    new MemberFacility
                    {
                        EId = f.EnrollId,
                        FId = f.FacilityId,
                        Name = f.Facility.Name,
                        SessionsLeft = f.SessionsLeft,
                        Used = false
                    }
                ).ToList();

                FacilitiesBox.Children.Clear();
                PayedFacilities.ForEach(f =>
                {
                    var check = new CheckBox
                    {
                        IsThreeState = false,
                        IsChecked = false,
                        Content = f.Name,
                        IsEnabled = f.SessionsLeft > 0,
                        Tag = f
                    };

                    FacilitiesBox.Children.Add(check);
                });



                //PayedCourses = (from m in db.Members
                //                join e in db.Enrolls on m.Id equals e.MemberId
                //                join ec in db.EnrollCourses on e.Id equals ec.CourseId
                //                join c in db.Courses on ec.CourseId equals c.Id
                //                where m.Id == member.Id
                //                && e.StartDate <= DateTime.Today
                //                && (e.ExpireDate == null || e.ExpireDate >= DateTime.Today)
                //                select new MemberCourse
                //                {
                //                    EId = ec.EnrollId,
                //                    CId = ec.CourseId,
                //                    Name = c.Name,
                //                    ExpiresAt = e.ExpireDate,
                //                    SessionsLeft = ec.SessionsLeft
                //                }).ToList();
                //PayedCourses = db.Members.Where(m => m.Id == SelectedMember.Id && m.EnrollCourses.SelectMany(ec => ec.Enroll))

                CoursesBox.Children.Clear();
                var oneCourseAvailable = PayedCourses.Where(x => x.SessionsLeft > 0).Count() == 1;
                PayedCourses.ForEach(c =>
                {
                    var check = new CheckBox
                    {
                        IsThreeState = false,
                        IsChecked = oneCourseAvailable ? true : false,
                        Content = c.Name,
                        IsEnabled = c.SessionsLeft > 0,
                        Tag = c
                    };

                    CoursesBox.Children.Add(check);
                });




                //TransitDialogHost.BringIntoView();
                PersonnelTransitDialog.Visibility = Visibility.Collapsed;
                MemberDeleteDialog.Visibility = Visibility.Collapsed;
                MembersTransitDialog.Visibility = Visibility.Visible;
                SelectedMember = member;
                rdIn.IsChecked = isEntrance;
                rdOut.IsChecked = !isEntrance;
                if (oneCourseAvailable)
                    ProcessTransit(null);
                else
                {
                    TransitDialogHost.IsOpen = true;
                    txtMemberName.Text = member.Fullname;

                    

                }


                //Close();
                //TransitDialogHost.IsEnabled = true;
                //TransitDialogHost.Visibility = Visibility.Visible;
            }
            else if (Type == MemberSelectionCategory.PersonnelTransit)
            {
                MembersTransitDialog.Visibility = Visibility.Collapsed;
                MemberDeleteDialog.Visibility = Visibility.Collapsed;
                PersonnelTransitDialog.Visibility = Visibility.Visible;
                TransitDialogHost.IsOpen = true;
                txtPersonnelName.Text = member.Fullname;
                var lastTransit = db.Passages.Where(p => p.MemberId == member.Id).OrderByDescending(p => p.Time).FirstOrDefault();
                var isEntrance = !(lastTransit?.IsEntrance ?? false);
                prdIn.IsChecked = isEntrance;
                prdOut.IsChecked = !isEntrance;
                SelectedMember = member;

            }
            else if (Type == MemberSelectionCategory.DeleteMembers)
            {
                MembersTransitDialog.Visibility = Visibility.Collapsed;
                PersonnelTransitDialog.Visibility = Visibility.Collapsed;
                MemberDeleteDialog.Visibility = Visibility.Visible;
                TransitDialogHost.IsOpen = true;
                deletingMemberName.Text = member.Fullname;
                deletingMemberCredit.Value = member.Credit < 0 ? (-1 * member.Credit) : 0;
                btnClearCreditDebtor.IsEnabled = member.Credit < 0;
                deletingMemberTuitionDebtor.Value = member.Credit < 0 ? (member.Debtor - member.Credit) : member.Debtor;
                btnClearTutionDebtor.IsEnabled = deletingMemberTuitionDebtor.Value > 0;
                SelectedMember = member;
                btnDeleteMember.IsEnabled = deletingMemberCredit.Value + deletingMemberTuitionDebtor.Value <= 0;
            }
            else if (Type == MemberSelectionCategory.TutitionDebtorsList ||
                Type == MemberSelectionCategory.ShoppingDebtorsList ||
                Type == MemberSelectionCategory.NearExpiryList)
            {
                Enroll enroll = new Enroll();
                enroll.Section = Type == MemberSelectionCategory.NearExpiryList ? 1 : 2;
                enroll.Owner = Main.Home;
                enroll.Show();
                enroll.LoadMember(member.Id);
            }
            if (Info.IsVisible)
            {
                Info.Focus();
                Info.BringIntoView();
            }
            //Close();
        }
        private void TransitDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            if (SelectedMember == null) return;

            var confirmed = (bool)eventArgs.Parameter;
            if (confirmed)
            {
                ProcessTransit(eventArgs);
            }
        }

        void ProcessTransit(MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            if (Type == MemberSelectionCategory.PersonnelTransit)
            {
                Data.Passage pass = new Data.Passage();
                pass.MemberId = SelectedMember.Id;
                pass.IsEntrance = prdIn.IsChecked == true;
                pass.Time = DateTime.Now;
                db.Passages.InsertOnSubmit(pass);

                db.SubmitChanges();
                Main.Home.TransitList.UpdatePassages();

                if (SelectedMember != null)
                {
                    this.Close();
                }
            }
            else if (Type == MemberSelectionCategory.MembersTransit)
            {
                if (rdIn.IsChecked == true)
                {
                    var usedCoursesCount = CoursesBox.Children.Cast<CheckBox>().Where(c => c.IsChecked == true).Count();
                    if (usedCoursesCount == 0)
                    {
                        Alert.Content = $"هیچ دوره ای انتخاب نشده بنابراین ورود ثبت نشد، مجددا تلاش فرمایید";
                        MessageSnackBar.IsActive = true;
                        eventArgs.Cancel();
                        return;
                    }

                    CoursesBox.Children.Cast<CheckBox>().Where(c => c.IsChecked == true).ToList().ForEach(check =>
                    {
                        //if (!needsSave) needsSave = true;
                        var item = check.Tag as MemberCourse;

                        var courseUsage = db.EnrollCourses.Where(ec =>
                            ec.CourseId == item.CId &&
                            ec.EnrollId == item.EId
                        ).FirstOrDefault();

                        if(courseUsage.Enroll.Frequency != 2) //2 = جلسه ای
                        courseUsage.SessionsLeft -= 1;
                    });
                }

                //bool needsSave = false;

                //if (needsSave) db.SubmitChanges();

                Data.Passage pass = new Data.Passage();
                pass.MemberId = SelectedMember.Id;
                pass.IsEntrance = Type == MemberSelectionCategory.MembersTransit ? rdIn.IsChecked == true : prdIn.IsChecked == true;
                pass.Time = DateTime.Now;
                db.Passages.InsertOnSubmit(pass);


                if (rdOut.IsChecked == true)
                {
                    var mmbr = db.Members.Where(m => m.Id == SelectedMember.Id).FirstOrDefault();
                    var nrols = mmbr.Enrolls.Where(e => e.StartDate <= DateTime.Today
                                        && (e.ExpireDate == null || e.ExpireDate >= DateTime.Today)).ToList();
                    var nc = nrols.SelectMany(n => n.EnrollCourses).ToList();


                    if (!nc.Any(c => c.SessionsLeft > 0))
                    {
                        nrols.SelectMany(n => n.EnrollFacilities).ToList().ForEach(f => f.SessionsLeft = 0);
                        nrols.ForEach(n => n.IsActive = false);
                    }
                    else
                    {
                        FacilitiesBox.Children.Cast<CheckBox>().Where(c => c.IsChecked == true).ToList().ForEach(check =>
                        {
                            var item = check.Tag as MemberFacility;

                            var facilityUsage = db.EnrollFacilities.Where(ef =>
                                ef.FacilityId == item.FId &&
                                ef.EnrollId == item.EId
                            ).FirstOrDefault();

                            facilityUsage.SessionsLeft -= 1;
                        });
                    }
                }

                db.SubmitChanges();
                Info.SetMember(SelectedMember.Id, pass.IsEntrance ? "ورود" : "خروج", PayedFacilities.Select(f => f.Name).ToList());
                Info.Show();
                Info.Focus();
                Close();

                if (pass.IsEntrance)
                    SelectedMember.UseCloset(db);
                else
                    SelectedMember.FreeUpCloset(db);

                Main.Home.Closets.LoadClosets();
                Main.Home.TransitList.UpdatePassages();

                //Reduce one Session from Course Sessions & Facility Sessions

                //bool needsSave = false;
                //FacilitiesBox.Children.Cast<CheckBox>().Where(c => c.IsChecked == true).ToList().ForEach(check =>
                //  {
                //      if (!needsSave) needsSave = true;
                //      var item = check.Tag as MemberFacility;

                //      var facilityUsage = db.EnrollFacilities.Where(ef =>
                //          ef.FacilityId == item.FId &&
                //          ef.EnrollId == item.EId
                //      ).FirstOrDefault();

                //      facilityUsage.SessionsLeft -= 1;
                //  });
                //if (needsSave) db.SubmitChanges();


                //if (SelectedMember != null)
                //{
                //    this.Close();
                //}
            }
            else if (Type == MemberSelectionCategory.DeleteMembers)
            {
                db = new Data.GymContextDataContext();
                db.Members.Where(m => m.Id == SelectedMember.Id).FirstOrDefault().IsDeleted = true;

                if (tglGetCardBack.IsChecked == true)
                    ;//Make Card Clear to be used by other members

                db.SubmitChanges();

                ListMembers(txtSearchBox.Text);
            }
        }

        public void ListMembers(string text)
        {
            text = text ?? "";
            MembersArea.Children.Clear();
            IQueryable<Data.Member> queryable;
            List<int> list;

            if (Type == MemberSelectionCategory.PersonnelTransit)
                queryable = db.Members.Where(m => (m.IsMentor == true || m.IsStaff == true) && !m.IsDeleted);
            else
                queryable = db.Members.Where(m => m.IsMentor == false && m.IsStaff == false && !m.IsDeleted);

            if (!string.IsNullOrEmpty(text))
                if (int.TryParse(text, out int code))
                    queryable = queryable.Where(m => m.Id == code);
                else
                    queryable = queryable.Where(m => m.Firstname.Contains(text) || m.Lastname.Contains(text));

            if (showOnlyPresentMembers)
            {
                queryable = queryable.Where(m => m.Passages.Any() && m.Passages.OrderByDescending(p => p.Id).FirstOrDefault().IsEntrance == true);
            }

            if (!showIrregularMembers)
                queryable = queryable.Where(m => m.IsRegular);

            if (Type == MemberSelectionCategory.TutitionDebtorsList)
                queryable = queryable.Where(m => m.Debtor > 0);
            else if (Type == MemberSelectionCategory.ShoppingDebtorsList)
                queryable = queryable.Where(m => m.Credit < 0);
            else if (Type == MemberSelectionCategory.NearExpiryList)
                queryable = queryable.Where(m => m.Enrolls.Where(e => e.ExpireDate != null
                    && e.StartDate <= DateTime.Today
                    && e.ExpireDate >= DateTime.Today
                    && e.ExpireDate < DateTime.Today.AddDays(4)
                    && e.IsActive).Count() > 0);

            list = queryable.Select(q => q.Id).ToList();

            list.ForEach(m =>
            {
                MemberCard card = new MemberCard(m);
                card.IsInteractive = Type != MemberSelectionCategory.None;
                card.SetGraphics(ViewType.SelectedIndex == 0 ? MemberCard.MemberGraphics.Chip : MemberCard.MemberGraphics.Card);
                MembersArea.Children.Add(card);
                card.MemberSelected += Card_MemberSelected;
            });

        }
        public void LoadSingleMember(Data.Member member)
        {
            MemberCard card = new MemberCard(member.Id);
            card.IsInteractive = Type != MemberSelectionCategory.None;
            card.SetGraphics(ViewType.SelectedIndex == 0 ? MemberCard.MemberGraphics.Chip : MemberCard.MemberGraphics.Card);
            MembersArea.Children.Add(card);
            card.MemberSelected += Card_MemberSelected;
            automatic = true;
            Card_MemberSelected(card.Member);
        }

        private void txtSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSearch_Click(null, null);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Info.Closing += (f, ea) =>
            {
                ea.Cancel = true;
                Info.Hide();
            };
            txtSearchBox.TextChanged += (x, y) => { btnSearch_Click(null, null); };
            btnRegular.IsChecked = showIrregularMembers;
            btnPresentOnly.IsChecked = showOnlyPresentMembers;
            ListMembers("");
            formIsLoading = false;
            if (Mentors)
            {
                btnPresentOnly.IsEnabled = false;
                btnRegular.IsEnabled = false;
            }
            SetHint();
            //txtSearchBox.Focus();

            var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            timer.Start();
            timer.Tick += (s, e2) =>
            {
                txtSearchBox.Focus();
                timer.Stop();
            };
        }

        private void SetHint()
        {
            var hint = "جستجوی اعضا";
            if (btnRegular.IsChecked == true)
            {
                hint += "، همه";
            }
            else { hint += "، ثبت نامی"; }

            if (btnPresentOnly.IsChecked == true)
            {
                hint += "، حاضر";
            }
            else { hint += "، حاضر و غایب"; }
            Tip.Text = hint;
        }


        private void btnPresent_Unchecked(object sender, RoutedEventArgs e)
        {

            showOnlyPresentMembers = false;
            if (!formIsLoading)
                ListMembers(txtSearchBox?.Text ?? "");
            SetHint();
        }

        private void btnPresent_Checked(object sender, RoutedEventArgs e)
        {

            showOnlyPresentMembers = true;
            if (!formIsLoading)
                ListMembers(txtSearchBox?.Text ?? "");
            SetHint();
        }

        private void btnRegular_Checked(object sender, RoutedEventArgs e)
        {
            showIrregularMembers = true;
            if (!formIsLoading)
                ListMembers(txtSearchBox?.Text ?? "");
            SetHint();
        }

        private void btnRegular_Unchecked(object sender, RoutedEventArgs e)
        {
            showIrregularMembers = false;
            if (!formIsLoading)
                ListMembers(txtSearchBox?.Text ?? "");
            SetHint();
        }

        private void btnClearCreditDebtor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClearTutionDebtor_Click(object sender, RoutedEventArgs e)
        {
            var enroll = new Enroll();
            enroll.Member = SelectedMember;
            enroll.Section = 2;
            enroll.section.IsEnabled = false;
            var membersLastEnroll = db.Enrolls.Where(en => en.MemberId == SelectedMember.Id).OrderByDescending(en => en.Id).FirstOrDefault();
            if (membersLastEnroll != null)
            {
                enroll.CurrentEnroll = membersLastEnroll;
                var dialogResult = enroll.ShowDialog();

                SelectedMember = new MemberCard(SelectedMember.Id).Member;
                deletingMemberCredit.Value = SelectedMember.Credit < 0 ? (-1 * SelectedMember.Credit) : 0;
                btnClearCreditDebtor.IsEnabled = SelectedMember.Credit < 0;
                deletingMemberTuitionDebtor.Value = SelectedMember.Credit < 0 ? (SelectedMember.Debtor - SelectedMember.Credit) : SelectedMember.Debtor;
                btnClearTutionDebtor.IsEnabled = deletingMemberTuitionDebtor.Value > 0;
                btnDeleteMember.IsEnabled = deletingMemberCredit.Value + deletingMemberTuitionDebtor.Value <= 0;
            }
        }

        public class MemberFacility
        {
            public int EId;
            public int FId;
            public string Name;
            public int SessionsLeft;
            public bool Used = false;
        }

        public class MemberCourse
        {
            public int EId;
            public int CId;
            public string Name;
            public int SessionsLeft;
            public bool Used = false;
            public DateTime? ExpiresAt;
        }

        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {
            MessageSnackBar.IsActive = false;
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            return;
            if (Info != null)
                Info.Close();
        }
    }
}
