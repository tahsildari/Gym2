using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Gym.Domain;

namespace Gym.ViewModels
{
    public class MemberVM : INotifyPropertyChanged
    {
        int _Id;
        string _Firstname;
        string _Lastname;
        string _Fathername;
        DateTime? _BirthDate;
        string _NationalCode;
        string _Mobile;
        string _Referrer;
        string _ReferrerMobile;
        string _Address;
        string _Description;
        string _InsuranceNo;
        BitmapImage _Image;
        bool _IsActive = true;
        int _Credit;
        int _Debtor;
        DateTime? _InsuranceExpireDate;
        int? _ClosetId;

        public int Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + $"/Images/{value}.jpg"))
                {
                    _Image = new BitmapImage();
                    _Image.BeginInit();
                    _Image.StreamSource = System.IO.File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + $"/Images/{value}.jpg");
                    _Image.EndInit();
                }
                RaisePropertyChanged(nameof(Id));
                //RaisePropertyChanged(nameof(Image));
            }
        }
        public string Firstname
        {
            get { return _Firstname; }
            set
            {
                _Firstname = value;
                RaisePropertyChanged(nameof(Firstname));
            }
        }
        public string Lastname
        {
            get { return _Lastname; }
            set
            {
                _Lastname = value;
                RaisePropertyChanged(nameof(Lastname));
            }
        }
        public string Fullname
        {
            get { return $"{_Firstname} {_Lastname}"; }
        }
        public BitmapImage Image
        {
            get { return _Image; }
            set
            {
                _Image = value;
                RaisePropertyChanged(nameof(Image));
            }
        }
        public string Fathername
        {
            get { return _Fathername; }
            set
            {
                _Fathername = value;
                RaisePropertyChanged(nameof(Fathername));
            }
        }
        public DateTime? BirthDate
        {
            get { return _BirthDate; }
            set
            {
                _BirthDate = value;
                RaisePropertyChanged(nameof(BirthDate));
                RaisePropertyChanged(nameof(BirthDateFa));
            }
        }
        public string NationalCode
        {
            get { return _NationalCode; }
            set
            {
                _NationalCode = value;
                RaisePropertyChanged(nameof(NationalCode));
            }
        }
        public string Mobile
        {
            get { return _Mobile; }
            set
            {
                _Mobile = value;
                RaisePropertyChanged(nameof(Mobile));
            }
        }
        public string Referrer
        {
            get { return _Referrer; }
            set
            {
                _Referrer = value;
                RaisePropertyChanged(nameof(Referrer));
            }
        }
        public string ReferrerMobile
        {
            get { return _ReferrerMobile; }
            set
            {
                _ReferrerMobile = value;
                RaisePropertyChanged(nameof(ReferrerMobile));
            }
        }
        public string Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
                RaisePropertyChanged(nameof(Address));
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
        public string InsuranceNo
        {
            get { return _InsuranceNo; }
            set
            {
                _InsuranceNo = value;
                RaisePropertyChanged(nameof(InsuranceNo));
            }
        }
        public string BirthDateFa
        {
            get { return _BirthDate?.ToFa() ?? ""; }
            set
            {
                _BirthDate = value.ToEn();
                RaisePropertyChanged(nameof(BirthDate));
                RaisePropertyChanged(nameof(BirthDateFa));
            }
        }
        public string InsuranceExpireDateFa
        {
            get { return _InsuranceExpireDate?.ToFa() ?? ""; }
            set
            {
                _InsuranceExpireDate = value.ToEn();
                RaisePropertyChanged(nameof(InsuranceExpireDate));
                RaisePropertyChanged(nameof(InsuranceExpireDateFa));
            }
        }
        public DateTime? InsuranceExpireDate
        {
            get { return _InsuranceExpireDate; }
            set
            {
                _InsuranceExpireDate = value;
                RaisePropertyChanged(nameof(InsuranceExpireDate));
                RaisePropertyChanged(nameof(InsuranceExpireDateFa));
            }
        }
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                _IsActive = value;
                RaisePropertyChanged(nameof(IsActive));
            }
        }
        public int Credit
        {
            get { return _Credit; }
            set
            {
                _Credit = value;
                RaisePropertyChanged(nameof(Credit));
            }
        }
        public int Debtor {
                get { return _Debtor;
                }
                set
            {
                _Debtor = value;
                    RaisePropertyChanged(nameof(Debtor));
                }
            }
        public int? ClosetId
        {
                get { return _ClosetId;
                }
                set
            {
                _ClosetId = value;
                    RaisePropertyChanged(nameof(ClosetId));
                }
            }
        public bool IsRegular = true;


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
