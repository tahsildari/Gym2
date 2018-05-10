using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.ViewModels
{
   public  class EnrollVM
    {
        string _StartDateFa;
        public string StartDateFa
        {
            get { return _StartDateFa; }
            set
            {
                _StartDateFa = value;
                try
                {
                    var p = value.Split('/').Select(int.Parse).ToList();
                    StartDate = new DateTime(p[0], p[1], p[2], new PersianCalendar());

                }
                catch (Exception)
                {

                    throw;
                }
                RaisePropertyChanged(nameof(StartDateFa));
            }
        }

        DateTime _StartDate;
        public DateTime StartDate
        {
            get { return _StartDate; }
            set
            {
                _StartDate = value;
                
                RaisePropertyChanged(nameof(StartDate));
            }
        }
        DateTime _ExpireDate;
        public DateTime ExpireDate
        {
            get { return _ExpireDate; }
            set
            {
                _ExpireDate = value;

                RaisePropertyChanged(nameof(ExpireDate));
            }
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
