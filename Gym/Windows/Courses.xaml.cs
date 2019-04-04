using Gym.Domain;
using Gym.ViewModels;
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
    /// Interaction logic for Courses.xaml
    /// </summary>
    public partial class Courses : Window
    {
        public Courses()
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

        ObservableCollection<Data.Course> CoursesList;
        ObservableCollection<Data.Sport> SportsList;
        Data.GymContextDataContext db = new Data.GymContextDataContext();
        CourseVM CourseModel = new CourseVM();

        Actions action = Actions.Inserting;
        bool IsNew() => action == Actions.Inserting;

        private void Dialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            var confirmed = (bool)eventArgs.Parameter;
            if (confirmed)
            {
                if (!string.IsNullOrEmpty(CourseModel.Name) && CourseModel.Sport >= 0)
                {
                    switch (action)
                    {
                        case Actions.Inserting:
                            if (!db.Courses.Any(u => u.Name == CourseModel.Name))
                            {
                                db.Courses.InsertOnSubmit(
                                    new Data.Course
                                    {
                                        SportId = CourseModel.Sport,
                                        Name = CourseModel.Name,
                                        IsVIP = CourseModel.IsVIP,

                                        Freeze = CourseModel.Freeze,
                                        Freeze2 = CourseModel.Freeze2,
                                        Freeze3 = CourseModel.Freeze3,
                                        Freeze6 = CourseModel.Freeze6,
                                        Freeze9 = CourseModel.Freeze9,
                                        Freeze12 = CourseModel.Freeze12,

                                        SessionPrice = CourseModel.SessionPrice,
                                        MonthPrice = CourseModel.MonthPrice,
                                        MonthPrice2 = CourseModel.MonthPrice2,
                                        TwoMonthPrice = CourseModel.TwoMonthPrice,
                                        TwoMonthPrice2 = CourseModel.TwoMonthPrice2,
                                        SeasonPrice = CourseModel.SeasonPrice,
                                        SeasonPrice2 = CourseModel.SeasonPrice2,
                                        HalfYearPrice = CourseModel.HalfYearPrice,
                                        HalfYearPrice2 = CourseModel.HalfYearPrice2,
                                        NineMonthPrice = CourseModel.NineMonthPrice,
                                        NineMonthPrice2 = CourseModel.NineMonthPrice2,
                                        YearPrice = CourseModel.YearPrice,
                                        YearPrice2 = CourseModel.YearPrice2,

                                    });
                                db.SubmitChanges();

                                RefreshGrid();
                            }
                            else
                            {
                                DuplicateUsernameSnackBar.IsActive = true;


                                System.Threading.Thread t = new System.Threading.Thread(() =>
                                {
                                    var started = DateTime.Now;
                                    while (DateTime.Now.Subtract(started).TotalSeconds < 3)
                                    { }
                                    this.Dispatcher.Invoke(
                                    new Action(() => { DuplicateUsernameSnackBar.IsActive = false; }));
                                }); t.Start();
                            }
                            break;
                        case Actions.Editing:
                            var update = db.Courses.Where(u => u.Id == CourseModel.Id).FirstOrDefault();
                            update.Freeze = CourseModel.Freeze;
                            update.Freeze2 = CourseModel.Freeze2;
                            update.Freeze3 = CourseModel.Freeze3;
                            update.Freeze6 = CourseModel.Freeze6;
                            update.Freeze9 = CourseModel.Freeze9;
                            update.Freeze12 = CourseModel.Freeze12;
                            update.SessionPrice = CourseModel.SessionPrice;
                            update.MonthPrice = CourseModel.MonthPrice;
                            update.MonthPrice2 = CourseModel.MonthPrice2;
                            update.TwoMonthPrice = CourseModel.TwoMonthPrice;
                            update.TwoMonthPrice2 = CourseModel.TwoMonthPrice2;
                            update.SeasonPrice = CourseModel.SeasonPrice;
                            update.SeasonPrice2 = CourseModel.SeasonPrice2;
                            update.HalfYearPrice = CourseModel.HalfYearPrice;
                            update.HalfYearPrice2 = CourseModel.HalfYearPrice2;
                            update.NineMonthPrice = CourseModel.NineMonthPrice;
                            update.NineMonthPrice2 = CourseModel.NineMonthPrice2;
                            update.YearPrice = CourseModel.YearPrice;
                            update.YearPrice2 = CourseModel.YearPrice2;

                            db.SubmitChanges();
                            RefreshGrid();
                            break;
                        default:
                            break;
                    }
                }
            }
            else {
                Dynamics.LastEscapeTime = DateTime.Now;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
            this.DataContext = CourseModel;
        }

        private void RefreshGrid()
        {
            var courses = db.Courses.ToList();
            CoursesList = new ObservableCollection<Data.Course>(courses);

            CoursesGrid.DataContext = CoursesList;

            SportsList = new ObservableCollection<Data.Sport>(db.Sports.ToList());
            cmbSport.DataContext = SportsList;

        }

        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {
            DuplicateUsernameSnackBar.IsActive = false;// = TimeSpan.FromSeconds(3);

        }

        private void AddNewCourse_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = CourseModel = new CourseVM();
            action = Actions.Inserting;

            NameTextBox.IsReadOnly = false;
            IsVIPSwitch.IsEnabled = true;
            cmbSport.IsReadOnly = false;

        }

        private void EditCourse_Click(object sender, RoutedEventArgs e)
        {
            var course = ((FrameworkElement)sender).DataContext as Data.Course;
            CourseModel.Id = course.Id;
            CourseModel.Freeze = course.Freeze;
            CourseModel.Freeze2 = course.Freeze2;
            CourseModel.Freeze3 = course.Freeze3;
            CourseModel.Freeze6 = course.Freeze6;
            CourseModel.Freeze9 = course.Freeze9;
            CourseModel.Freeze12 = course.Freeze12;
            CourseModel.Name = course.Name;
            CourseModel.Sport = course.SportId;
            CourseModel.IsVIP = course.IsVIP;
            //prices
            CourseModel.SessionPrice = course.SessionPrice;
            CourseModel.MonthPrice = course.MonthPrice;
            CourseModel.MonthPrice2 = course.MonthPrice2;
            CourseModel.TwoMonthPrice = course.TwoMonthPrice;
            CourseModel.TwoMonthPrice2 = course.TwoMonthPrice2;
            CourseModel.SeasonPrice = course.SeasonPrice;
            CourseModel.SeasonPrice2 = course.SeasonPrice2;
            CourseModel.HalfYearPrice = course.HalfYearPrice;
            CourseModel.HalfYearPrice2 = course.HalfYearPrice2;
            CourseModel.NineMonthPrice = course.NineMonthPrice;
            CourseModel.NineMonthPrice2 = course.NineMonthPrice2;
            CourseModel.YearPrice = course.YearPrice;
            CourseModel.YearPrice2 = course.YearPrice2;

            cmbSport.SelectedValue = course.SportId;

            action = Actions.Editing;
            NameTextBox.IsReadOnly = true;
            IsVIPSwitch.IsEnabled = false;
            cmbSport.IsReadOnly = true;

            DialogHost.IsOpen = true;
        }

        private void cmbSport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count> 0 && e.AddedItems[0] != null)
                CourseModel.Sport = (e.AddedItems[0] as Data.Sport).Id;
        }

        private void MonthPriceNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            if(TwoMonthPriceNumber.Value == 0) TwoMonthPriceNumber.Value = MonthPriceNumber.Value * 2;
            if(SeasonPriceNumber.Value == 0) SeasonPriceNumber.Value = MonthPriceNumber.Value * 3;
            if(HalfYearPriceNumber.Value == 0) HalfYearPriceNumber.Value = MonthPriceNumber.Value * 6;
            if(NineMonthPriceNumber.Value == 0) NineMonthPriceNumber.Value = MonthPriceNumber.Value * 9;
            if(YearPriceNumber.Value == 0) YearPriceNumber.Value = MonthPriceNumber.Value * 12;


        }

        private void MonthPrice2Number_LostFocus(object sender, RoutedEventArgs e)
        {

            if (TwoMonthPrice2Number.Value == 0) TwoMonthPrice2Number.Value = MonthPrice2Number.Value * 2;
            if (SeasonPrice2Number.Value == 0) SeasonPrice2Number.Value = MonthPrice2Number.Value * 3;
            if (HalfYearPrice2Number.Value == 0) HalfYearPrice2Number.Value = MonthPrice2Number.Value * 6;
            if (NineMonthPrice2Number.Value == 0) NineMonthPrice2Number.Value = MonthPrice2Number.Value * 9;
            if (YearPrice2Number.Value == 0) YearPrice2Number.Value = MonthPrice2Number.Value * 12;
        }

        private void FreezeNumber_LostFocus(object sender, RoutedEventArgs e)
        {

            if (FreezeNumber2.Value == 0) FreezeNumber2.Value = FreezeNumber.Value * 2;
            if (FreezeNumber3.Value == 0) FreezeNumber3.Value = FreezeNumber.Value * 3;
            if (FreezeNumber6.Value == 0) FreezeNumber6.Value = FreezeNumber.Value * 6;
            if (FreezeNumber9.Value == 0) FreezeNumber9.Value = FreezeNumber.Value * 9;
            if (FreezeNumber12.Value == 0) FreezeNumber12.Value = FreezeNumber.Value * 12;

        }
    }
}
