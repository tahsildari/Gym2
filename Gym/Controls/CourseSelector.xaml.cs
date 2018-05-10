using Gym.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Gym.Domain.Enums;
using static Gym.ViewModels.CourseVM;

namespace Gym.Controls
{
    /// <summary>
    /// Interaction logic for CourseSelector.xaml
    /// </summary>
    public partial class CourseSelector : UserControl, INotifyPropertyChanged
    {
        Data.GymContextDataContext db = new Data.GymContextDataContext();
        public CourseListVM Courses { get; set; }

        public CourseSelector()
        {
            InitializeComponent();
            try
            {
                //var memberId = 1;
                var now = DateTime.Now;
                var data = (from c in db.Courses
                            select
                     new CourseVM
                     {
                         Id = c.Id,
                         MentorId = null,
                         MentorPrice = 0,
                         Mentors = c.Sport.SportMentors.Select(m => new CourseVM.MentorVM { Id = m.MentorId, Name = m.Member.Firstname + " " + m.Member.Lastname, Price = m.Price }).ToList(),
                         Name = c.Name,
                         Duration = 1,
                         Code = c.Sport.Name[0].ToString(),
                         SessionPrice = c.SessionPrice,
                         MonthPrice = c.MonthPrice,
                         TwoMonthPrice = c.TwoMonthPrice,
                         SeasonPrice = c.SeasonPrice,
                         HalfYearPrice = c.HalfYearPrice,
                         NineMonthPrice = c.NineMonthPrice,
                         YearPrice = c.YearPrice,
                         MonthPrice2 = c.MonthPrice2,
                         TwoMonthPrice2 = c.TwoMonthPrice2,
                         SeasonPrice2 = c.SeasonPrice2,
                         HalfYearPrice2 = c.HalfYearPrice2,
                         NineMonthPrice2 = c.NineMonthPrice2,
                         YearPrice2 = c.YearPrice2,
                         IsSelected = false,
                         IsVIP = c.IsVIP,
                         Freeze = c.Freeze,
                         Freeze2 = c.Freeze2,
                         Freeze3 = c.Freeze3,
                         Freeze6 = c.Freeze6,
                         Freeze9 = c.Freeze9,
                         Freeze12 = c.Freeze12
                     }
                   ).ToList();
                Courses = new CourseListVM(new ObservableCollection<CourseVM>(data));
                Courses.PropertyChanged += Courses_PropertyChanged;
                this.DataContext = this;


            }
            catch (Exception ex)
            {
                ;
            }
        }

        public void SetEnroll(int? enrollId)
        {
            var now = DateTime.Now;

            if (enrollId == null || enrollId == 0)
            {
                Courses.Items.ToList().ForEach(c =>
                {
                    c.IsActive = false;
                    c.StartDate = null;
                    c.FinishDate = null;
                    c.IsSelected = false;
                });
            }
            else
            {
                Courses.Items.ToList().ForEach(
                    c =>
                    {
                        c.IsSelected = c.IsActive = db.Enrolls.Any(e => e.Id == enrollId && e.EnrollCourses.Any(ec => ec.Course.Id == c.Id));
                        //c.StartDate = db.Enrolls.Where(e => e.Id == enrollId && e.EnrollCourses.Any(ec => ec.Course.Id == c.Id)).FirstOrDefault().StartDate;
                        //c.FinishDate = db.Enrolls.Where(e => e.Id == enrollId && e.EnrollCourses.Any(ec => ec.Course.Id == c.Id)).FirstOrDefault().ExpireDate;
                        c.MentorId = db.EnrollCourses.Where(ec => ec.EnrollId == enrollId && ec.Course.Id == c.Id).FirstOrDefault()?.MentorId;
                        
                    });
            }
        }

        public void SetReadonly(bool isReadonly)
        {
            Courses.Items.ToList().ForEach(c =>
            {
                c.IsReadonly = isReadonly;
            });
        }

        //public void SetData(List<CourseVM> data)
        //{
        //    Courses = new CourseListVM(new ObservableCollection<CourseVM>(data));
        //    Courses.PropertyChanged += Courses_PropertyChanged;
        //    this.DataContext = this;
        //}

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {


            //var rows = GetDataGridRows(dgCourses);

            //foreach (DataGridRow r in rows)
            //{
            //    CourseVM rv = (CourseVM)r.Item;
            //    foreach (DataGridColumn column in dgCourses.Columns)
            //    {
            //        if (column.DisplayIndex == 3)
            //        {
            //            //TextBlock cellContent = column.GetCellContent(r) as TextBlock;
            //            MaterialDesignThemes.Wpf.MaterialDataGridComboBoxColumn combo = (MaterialDesignThemes.Wpf.MaterialDataGridComboBoxColumn)column;
            //            var data = new ObservableCollection<CourseVM.MentorVM>(rv.Mentors);
            //            //Binding b = new Binding();
            //            //b.Path = new PropertyPath("Id");
            //            //b.Mode = BindingMode.OneWay;
            //            //b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //            column.GetCellContent(r).DataContext = data;

            //            //MessageBox.Show(cellContent.Text);
            //        }
            //    }
            //}
        }

        private void Courses_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Total")
                //OnPropertyChanged("Total");
                Total = Courses.Items.Where(i => i.IsSelected).Sum(i => i.Price);
        }

        bool _IsReadOnly;
        public bool IsReadOnly
        {
            get { return _IsReadOnly; }
            set
            {
                _IsReadOnly = value;
                SetReadonly(value);
                OnPropertyChanged("IsReadOnly");
            }
        }

        public bool HasSelectedVIP { get { return Courses.HasSelectedVIP; } }

        int _Total;
        public int Total
        {
            get { return _Total; }
            set { _Total = value; OnPropertyChanged("Total"); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        public event EventHandler Checked;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void OnChecked()
        {
            EventHandler handler = Checked;
            if (null != handler) handler(this, EventArgs.Empty);
        }

        public void SetFrequency(Frequencies Frequency)
        {
            Courses.Items.ToList().ForEach(c => c.Frequency = Frequency);

            return;
            switch (Frequency)
            {
                case Frequencies.Everyday:
                    break;
                case Frequencies.EveryOtherDay:
                    break;
                case Frequencies.SingleSessions:
                    break;
                default:
                    break;
            }
        }

        public void SetDuration(int duration)
        {
            Courses.Items.ToList().ForEach(c => c.Duration = duration);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            OnChecked();
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly",
                typeof(bool),
                typeof(CourseSelector),
                new UIPropertyMetadata(false, new PropertyChangedCallback(valueChangedCallBack)));

        private static void valueChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }
    }
}
