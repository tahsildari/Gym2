using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.ViewModels
{
    public class FacilityVM : INotifyPropertyChanged
    {
        int _Id;
        string _Name;
        string _Code;
        int _Price;
        byte _Sessions;
        bool _IsSelected;
        bool _IsReadOnly;

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }
        public int Price
        {
            get { return _Price; }
            set
            {
                _Price = value;
                RaisePropertyChanged(nameof(Price));
            }
        }
        public byte Sessions
        {
            get { return _Sessions; }
            set
            {
                _Sessions = value;
                RaisePropertyChanged(nameof(Sessions));
            }
        }
        public string Code
        {
            get { return _Code; }
            set
            {
                _Code = value;
                RaisePropertyChanged(nameof(Code));
            }
        }
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                RaisePropertyChanged(nameof(IsSelected));
            }
        }

        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }
            set
            {
                _IsReadOnly = value;
                RaisePropertyChanged(nameof(IsReadOnly));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class FacilityListVM : INotifyPropertyChanged
        {
            public ObservableCollection<FacilityVM> Items { get; set; }

            public FacilityListVM(ObservableCollection<FacilityVM> Items)
            {
                this.Items = Items;
                Items.CollectionChanged += Items_CollectionChanged;
                Items.ToList().ForEach(t => t.PropertyChanged += T_PropertyChanged);
            }

            private void T_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "IsSelected")
                    OnPropertyChanged("Total");
            }

            private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                OnPropertyChanged("Total");
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
        }
    }
}
