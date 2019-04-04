using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.ViewModels
{
    public class ClosetRangeVM : INotifyPropertyChanged
    {
        int _FromLabel;
        public int FromLabel
        {
            get { return _FromLabel; }
            set
            {
                _FromLabel = value;
                RaisePropertyChanged("FromLabel");
            }
        }

        int _ToLabel;
        public int ToLabel
        {
            get { return _ToLabel; }
            set
            {
                _ToLabel = value;
                RaisePropertyChanged("ToLabel");
            }
        }

        //string _VIP;
        //public string VIP
        //{
        //    get { return _VIP; }
        //    set
        //    {
        //        _VIP = value;
        //        RaisePropertyChanged("VIP");
        //    }
        //}

        bool _IsVIP;
        public bool IsVIP
        {
            get { return _IsVIP; }
            set
            {
                _IsVIP = value;
                RaisePropertyChanged("IsVIP");
            }
        }

        bool _IsCoach;
        public bool IsCoach
        {
            get { return _IsCoach; }
            set
            {
                _IsCoach = value;
                RaisePropertyChanged("IsCoach");
            }
        }
        public List<Data.Closet> GetClosets()
        {
            List<Data.Closet> result = new List<Data.Closet>();
            for (int i = FromLabel; i <= ToLabel; i++)
            {
                result.Add(new Data.Closet { Id = i, IsVip = IsVIP, IsFree = true, IsCoach = IsCoach });
            }
            return result;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
