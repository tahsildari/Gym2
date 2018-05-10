using Gym.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using static Gym.ViewModels.FacilityVM;

namespace Gym.Controls
{
    /// <summary>
    /// Interaction logic for FacilitySelector.xaml
    /// </summary>
    public partial class FacilitySelector : UserControl, INotifyPropertyChanged
    {
        Data.GymContextDataContext db = new Data.GymContextDataContext();


        public FacilityListVM Facilities { get; set; }

        public FacilitySelector()
        {
            InitializeComponent();

            try
            {
                var fac = db.Facilities.Select(f =>
                    new FacilityVM
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Price = f.Price,
                        IsSelected = false,
                        Sessions = f.Sessions,
                        Code = f.Name[0].ToString()
                    }
                ).ToList();
                Facilities = new FacilityListVM(new ObservableCollection<FacilityVM>(fac));

                this.DataContext = this;
            }
            catch (Exception ex)
            {
                ;
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Facilities.PropertyChanged += Facilities_PropertyChanged;
            }
            catch (Exception ex)
            {

            }
        }

        private void Facilities_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Total")
                //OnPropertyChanged("Total");
                Total = Facilities.Items.Where(i => i.IsSelected).Sum(i => i.Price);
        }

        int _Total;
        public int Total
        {
            get { return _Total; }
            set { _Total = value; OnPropertyChanged("Total"); }
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

        private void SetReadonly(bool value)
        {
            Facilities.Items.ToList().ForEach(f => f.IsReadOnly = value);
        }

        internal void ClearSelection()
        {
            Facilities.Items.ToList().ForEach(f => f.IsSelected = false);
        }

        public void SetEnroll(int? enrollId)
        {
            var now = DateTime.Now;

            if (enrollId == null || enrollId == 0)
            {
                Facilities.Items.ToList().ForEach(f =>
                {
                    f.IsSelected = false;
                });
            }
            else
            {
                Facilities.Items.ToList().ForEach(
                    f =>
                    {
                        f.IsSelected = db.Enrolls.Any(e => e.Id == enrollId && e.EnrollFacilities.Any(ef => ef.FacilityId == f.Id));
                    });
            }
        }

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly",
                typeof(bool),
                typeof(FacilitySelector),
                new UIPropertyMetadata(false, new PropertyChangedCallback(valueChangedCallBack)));

        private static void valueChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
