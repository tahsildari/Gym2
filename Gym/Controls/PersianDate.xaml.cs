using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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

namespace Gym.Controls
{
    /// <summary>
    /// Interaction logic for PersianDate.xaml
    /// </summary>
    public partial class PersianDate : UserControl//, INotifyPropertyChanged
    {
        public PersianDate()
        {
            InitializeComponent();

            MonthNames = new ObservableCollection<string>();
            MonthNames.Add("فروردین");
            MonthNames.Add("اردیبهشت");
            MonthNames.Add("خرداد");
            MonthNames.Add("تیر");
            MonthNames.Add("مرداد");
            MonthNames.Add("شهریور");
            MonthNames.Add("مهر");
            MonthNames.Add("آبان");
            MonthNames.Add("آذر");
            MonthNames.Add("دی");
            MonthNames.Add("بهمن");
            MonthNames.Add("اسفند");

            Years = new ObservableCollection<string>();
            PersianCalendar pc = new PersianCalendar();
            var emsaal = pc.GetYear(DateTime.Now);
            for (int i = emsaal - 90; i < emsaal + 10; i++)
            {
                Years.Add(i.ToString());
            }

            Days = new ObservableCollection<string>();

            Container.DataContext = this;
            //label.DataContext = this;
        }
        public ObservableCollection<string> Years { get; set; }
        public ObservableCollection<string> MonthNames { get; set; }
        public ObservableCollection<string> Days { get; set; }
        public string Label { get; set; } = "تاریخ";

        private bool _IsReadOnly;
        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }
            set
            {
                _IsReadOnly = value;
                cmbDay.IsHitTestVisible = cmbMonth.IsHitTestVisible = cmbYear.IsHitTestVisible =
                    cmbDay.IsTabStop = cmbMonth.IsTabStop = cmbYear.IsTabStop = !value;
            }
        }

        //int _year, _month, _day;

        //public int year
        //{
        //    get
        //    {
        //        try
        //        {
        //            return int.Parse(Years[cmbYear.SelectedIndex]);
        //        }
        //        catch { return 0; }
        //    }
        //    set
        //    {
        //        _year = value;
        //        cmbYear.SelectedIndex = Years.IndexOf(_year.ToString());
        //        //RaisePropertyChanged("year");
        //    }
        //}
        //public int month
        //{
        //    get
        //    {
        //        return cmbMonth.SelectedIndex + 1;
        //    }
        //    set
        //    {
        //        _month = value;
        //        cmbMonth.SelectedIndex = value - 1;
        //        //RaisePropertyChanged("month");
        //    }
        //}
        //public int day
        //{
        //    get
        //    {
        //        return cmbDay.SelectedIndex + 1;
        //    }
        //    set
        //    {
        //        _day = value;
        //        cmbDay.SelectedIndex = value - 1;
        //        //RaisePropertyChanged("day");
        //    }
        //}
        bool IsValid(string date)
        {
            return !date.Split('/').Select(int.Parse).ToArray().Any(p => p == 0);
        }

        public void Clear()
        {
            cmbYear.SelectedIndex = -1;
            cmbMonth.SelectedIndex = -1;
            cmbDay.SelectedIndex = -1;
        }

        public string Date
        {
            get { return (string)GetValue(DateProperty); }
            set
            {
                if (value == "" || IsValid(value))
                {
                    SetValue(DateProperty, value);
                    try
                    {
                        cmbYear.Text = year.ToString();
                        cmbMonth.Text = MonthNames[month-1];
                        cmbDay.Text = day.ToString();
                    }
                    catch (Exception ex)
                    {
                    }
                    //DateChanged?.Invoke();
                }
                if (value == "")
                {
                    cmbYear.SelectedIndex = -1;
                    cmbMonth.SelectedIndex = -1;
                    cmbDay.SelectedIndex = -1;
                }
               
            }
        }

        public static readonly DependencyProperty DateProperty =
      DependencyProperty.Register("Date", typeof(string),
        typeof(PersianDate), new PropertyMetadata(""));

        //public string Date
        //{
        //    get
        //    {
        //        if (year > 0 &&
        //            month > 0 &&
        //            day > 0)
        //        {
        //            return $"{year}/{month}/{day}";
        //        }
        //        return "";
        //    }
        //    set
        //    {
        //        try
        //        {
        //            var parts = value.Split('/').Select(int.Parse).ToArray();
        //            year = parts[0];
        //            month = parts[1];
        //            day = parts[2];
        //            //cmbYear.Text = parts[0];
        //            //cmbMonth.SelectedIndex = int.Parse(parts[1]) - 1;
        //            //cmbDay.SelectedValue = parts[2];
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //    }
        //}

        //public string DateEn
        //{
        //    get
        //    {
        //        return (string)this.GetValue(DateEnProperty);
        //    }
        //    set
        //    {
        //        Date = value;
        //        //var pc = new PersianCalendar();
        //        //var y = pc.GetYear(value);
        //        //var m = pc.GetMonth(value);
        //        //var d = pc.GetDayOfMonth(value);
        //        //Date = $"{y}/{m}/{d}";
        //        this.SetValue(DateEnProperty, value);
        //    }
        //}

        public int year { get { try { return int.Parse(Date.Split('/')[0]); } catch { return 0; } } }
        public int month { get { try { return int.Parse(Date.Split('/')[1]); } catch { return 0; } } }
        public int day { get { try { return int.Parse(Date.Split('/')[2]); } catch { return 0; } } }

        private void cmbMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var date = (cmbYear.SelectedItem.ToString()) + "/" +
                    (cmbMonth.SelectedIndex + 1).ToString() + "/" +
                    (cmbDay.SelectedIndex + 1).ToString();

                PersianCalendar pc = new PersianCalendar();
                int y = 0, m = 0;
                int.TryParse(cmbYear.SelectedValue.ToString(), out y);
                m = cmbMonth.SelectedIndex + 1;
                var days = pc.GetDaysInMonth(y, m);
                var dayList = new List<string>();
                int selectedDay = -1;
                if (int.TryParse(cmbDay.SelectedItem?.ToString() ?? "", out selectedDay))
                {
                    selectedDay = Math.Min(selectedDay, days);
                }
                if (pc.GetDaysInMonth(y, m) != Days.Count)
                {
                    Days.Clear();
                    for (int i = 0; i < days; i++)
                    {
                        Days.Add($"{i + 1}");
                    }
                }
                if (year > 0 && month > 0 && selectedDay > pc.GetDaysInMonth(year, month))
                    selectedDay = pc.GetDaysInMonth(year, month);
                cmbDay.SelectedIndex = selectedDay > -1 ? (selectedDay - 1) : 0;



                if (Date == null || (cmbMonth.SelectedIndex + 1).ToString() != Date.Split('/')[1])
                {


                    Date = date;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void cmbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbYear.SelectedIndex != -1)
            {
                var date = (cmbYear.SelectedItem.ToString()) + "/" +
                        (cmbMonth.SelectedIndex + 1).ToString() + "/" +
                        (cmbDay.SelectedIndex + 1).ToString();
                if (Date == null || cmbYear.SelectedItem.ToString() != Date.Split('/')[0])
                {
                    Date = date;
                }
            }
        }

        private void cmbDay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var date = (cmbYear.SelectedItem.ToString()) + "/" +
                                    (cmbMonth.SelectedIndex + 1).ToString() + "/" +
                                    (cmbDay.SelectedIndex + 1).ToString();
                if (Date == null || cmbDay.SelectedIndex > -1 && (cmbDay.SelectedIndex + 1).ToString() != Date.Split('/')[2])
                {
                    Date = date;
                }
            }
            catch { }
        }

        private void cmbYear_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                PersianCalendar pc = new PersianCalendar();
                Date = $"{pc.GetYear(DateTime.Today)}/{pc.GetMonth(DateTime.Today)}/{pc.GetDayOfMonth(DateTime.Today)}";
            }
            else if (e.Key == Key.Delete)
            {
                Date = "";
            }
            else if (e.Key == Key.Escape)
            {
                cmbYear.SelectedIndex = Years.IndexOf(year.ToString());

            }
        }

        //public delegate void MyEvent();
        //public event MyEvent DateChanged;

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var parts = TextBox.Text.Split('/').Select(int.Parse).ToArray();
                //year = parts[0];
                //month = parts[1];
                //day = parts[2];


                cmbYear.SelectedIndex = Years.IndexOf(parts[0].ToString());
                cmbMonth.SelectedIndex = parts[1] - 1;
                cmbDay.SelectedIndex = parts[2] - 1;

            }
            catch (Exception)
            {

            }
        }

        //  public String Label
        //  {
        //      get { return (String)GetValue(LabelProperty); }
        //      set { SetValue(LabelProperty, value); }
        //  }
        //  public static readonly DependencyProperty LabelProperty =
        //DependencyProperty.Register("Label", typeof(string),
        //  typeof(PersianDate), new PropertyMetadata(""));

        //  //    public static readonly DependencyProperty DateProperty =
        //  //DependencyProperty.Register("DateEn", typeof(DateTime), typeof(PersianDate),
        //  //new PropertyMetadata(DateTime.Now));
        //  public static readonly DependencyProperty DateEnProperty = DependencyProperty.RegisterAttached("DateEn",
        //      typeof(string), typeof(PersianDate),
        //      new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnItemsPropertyChanged)));

        //  private static void OnItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //  {
        //      PersianDate control = d as PersianDate;
        //      control.Date = e.NewValue.ToString();
        //      // AutocompleteTextBox source = d as AutocompleteTextBox;
        //      // Do something...
        //  }



        #region INotifyPropertyChanged Members

        //public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        //private void RaisePropertyChanged(string propertyName)
        //{
        //    // take a copy to prevent thread issues
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
        #endregion


    }
}
