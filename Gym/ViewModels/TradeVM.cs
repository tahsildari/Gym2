using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.ViewModels
{
    public class TradeVM : INotifyPropertyChanged
    {
        int _Id;
        string _Code;
        string _Name;
        string _Description;
        bool _IsSelected;
        int _Count;
        string _Hint;
        int _Price;

        public int Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                RaisePropertyChanged(nameof(Name));
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
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                RaisePropertyChanged(nameof(Description));
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
        public int Count
        {
            get { return _Count; }
            set
            {
                _Count = value;
                RaisePropertyChanged(nameof(Count));
                RaisePropertyChanged(nameof(Total));
            }
        }
        public string Hint {
            get { return _Hint; }
            set
            {
                _Hint = value;
                RaisePropertyChanged(nameof(Hint));
            }
        }
        public int Price
        {
            get { return _Price; }
            set
            {
                _Price = value;
                RaisePropertyChanged(nameof(Price));
                RaisePropertyChanged(nameof(Total));
            }
        }
        public int Total {
            get { return Count * Price; }
            set { }
        }

        public TradeVM()
        {
            PickedCount = new List<int>(Enumerable.Range(0, 30));
        }

        public IList<int> PickedCount { get; set; }

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
    }

    public class TradesVM: INotifyPropertyChanged
    {
        private int _total;
        public ObservableCollection<TradeVM> Items { get; set; }

        public TradesVM(ObservableCollection<TradeVM> Items)
        {
            this.Items = Items;
            Items.CollectionChanged += Items_CollectionChanged;
            Items.ToList().ForEach(t => t.PropertyChanged += T_PropertyChanged);
        }

        private void T_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Count" || e.PropertyName == "Price" ||
                e.PropertyName == "IsSelected")
                OnPropertyChanged("Total");
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Total");
        }

        public int Total
        {
            get
            {
                return Items.Where(x=>x.IsSelected).Sum(x => x.Total);
            }
            set
            {
                _total = value;
                OnPropertyChanged("Total");
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
    }
}
