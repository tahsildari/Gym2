using Gym.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.ViewModels
{
    public class ClosetVM : INotifyPropertyChanged
    {
        int _Id;
        public int Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                RaisePropertyChanged("Id");
            }
        }

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

        bool _IsBroken;
        public bool IsBroken
        {
            get { return _IsBroken; }
            set
            {
                _IsBroken = value;
                RaisePropertyChanged("IsBroken");
            }
        }

        bool _IsRented;
        public bool IsRented
        {
            get { return _IsRented; }
            set
            {
                _IsRented = value;
                RaisePropertyChanged("IsRented");
            }
        }

        bool _IsFree;
        public bool IsFree
        {
            get { return _IsFree; }
            set
            {
                _IsFree = value;
                RaisePropertyChanged("IsFree");
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
        public Data.Member User;

        public Data.Member Rentor;

        public string Hint
        {
            get
            {
                string hint = $"[{Id}]";
                hint += Environment.NewLine;
                if(IsCoach) hint += "کمد مربی";
                else if (IsVIP) hint += "کمد VIP";
                else hint += "کمد معمولی";
                hint += Environment.NewLine;
                hint += (IsBroken ? "خراب" : "سالم");
                hint += Environment.NewLine;
                hint += (IsFree ? "خالی" : ($"در حال استفاده توسط {User?.Fullname() ?? ""}"));
                hint += Environment.NewLine;
                hint += (IsRented ? ($"اجاره {Rentor?.Fullname() ?? ""}") : "اجاره نشده");
                //if (Rentor != null)
                //    hint += Environment.NewLine + Rentor.Fullname();

                return hint;
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
