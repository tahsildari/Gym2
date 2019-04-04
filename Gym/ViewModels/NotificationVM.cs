using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.ViewModels
{
    public class NotificationVM : INotifyPropertyChanged
    {
        string _TuitionDetors;
        public string TuitionDetors
        {
            get { return _TuitionDetors; }
            set
            {
                _TuitionDetors = value;
                RaisePropertyChanged(nameof(TuitionDetors));
            }
        }

        string _CaffeDebtors;
        public string CaffeDebtors
        {
            get { return _CaffeDebtors; }
            set
            {
                _CaffeDebtors = value;
                RaisePropertyChanged(nameof(CaffeDebtors));
            }
        }

        string _Extenders;
        public string Extenders
        {
            get { return _Extenders; }
            set
            {
                _Extenders = value;
                RaisePropertyChanged(nameof(Extenders));
            }
        }

        string _OrderTimes;
        public string OrderTimes
        {
            get { return _OrderTimes; }
            set
            {
                _OrderTimes = value;
                RaisePropertyChanged(nameof(OrderTimes));
            }
        }

        string _Tresspasses = "0";
        public string Tresspasses
        {
            get { return _Tresspasses; }
            set
            {
                _Tresspasses = value;
                RaisePropertyChanged(nameof(Tresspasses));
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
