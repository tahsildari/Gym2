using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.ViewModels
{
    public class GoodVM : INotifyPropertyChanged
    {
        string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        int _Count;
        public int Count
        {
            get { return _Count; }
            set
            {
                _Count = value;
                RaisePropertyChanged(nameof(Count));
            }
        }

        int _OrderPoint;
        public int OrderPoint
        {
            get { return _OrderPoint; }
            set
            {
                _OrderPoint = value;
                RaisePropertyChanged(nameof(OrderPoint));
            }
        }

        int _Id;
        public int Id {
            get { return _Id; }
            set
            {
                _Id = value;
                RaisePropertyChanged(nameof(Id));
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
    }
}
