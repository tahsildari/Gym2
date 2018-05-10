using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.ViewModels
{
    public class UserVM : INotifyPropertyChanged
    {
        string _Username;
        public string Username
        {
            get { return _Username; }
            set
            {
                _Username = value;
                RaisePropertyChanged(nameof(Username));
            }
        }

        string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                RaisePropertyChanged(nameof(Password));
            }
        }
        int _RoleId;
        public int RoleId
        {
            get { return _RoleId; }
            set
            {
                _RoleId = value;
                RaisePropertyChanged(nameof(RoleId));
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
