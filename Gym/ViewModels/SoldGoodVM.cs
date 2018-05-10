using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.ViewModels
{
    public class SoldGoodVM : INotifyPropertyChanged
    {
        string _Good;
        public string Good
        {
            get { return _Good; }
            set
            {
                _Good = value;
                RaisePropertyChanged(nameof(Good));
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

        int _Member;
        public int Member
        {
            get { return _Member; }
            set
            {
                _Member = value;
                RaisePropertyChanged(nameof(Member));
            }
        }

        DateTime _Time;
        public DateTime Time
        {
            get { return _Time; }
            set
            {
                _Time = value;
                RaisePropertyChanged(nameof(Time));
            }
        }

        int _Price;
        public int Price
        {
            get { return _Price; }
            set
            {
                _Price = value;
                RaisePropertyChanged(nameof(Price));
            }
        }

        int _Id;
        public int Id
        {
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