using Gym.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.ViewModels
{

    public class CourseVM : INotifyPropertyChanged
    {
        string _Name;
        string _Code;
        bool _IsSelected;
        bool _IsReadonly;
        bool _IsVIP;
        int _HalfYearPrice;
        int _YearPrice;
        int _NineMonthPrice;
        int _SeasonPrice;
        int _TwoMonthPrice;
        int _MonthPrice;
        int _SessionPrice;
        int _HalfYearPrice2;
        int _YearPrice2;
        int _NineMonthPrice2;
        int _SeasonPrice2;
        int _TwoMonthPrice2;
        int _MonthPrice2;
        int _Id;
        int _Freeze;
        int _Freeze2;
        int _Freeze3;
        int _Freeze6;
        int _Freeze9;
        int _Freeze12;
        int _Sport;
        int? _MentorId;
        int? _MentorPrice;
        bool _IsActive;
        DateTime? _StartDate;
        DateTime? _FinishDate;
        int _Duration;

        Domain.Enums.Frequencies _Frequency;
        public Domain.Enums.Frequencies Frequency
        {
            get { return _Frequency; }
            set
            {
                _Frequency = value;
                RaisePropertyChanged("Price");
            }
        }

        List<MentorVM> _Mentors;
        public List<MentorVM> Mentors
        {
            get { return _Mentors; }
            set
            {
                _Mentors = value;
                _Mentors.Add(new CourseVM.MentorVM { Id = null, Price = 0, Name = "(بدون مربی)" });
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
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                RaisePropertyChanged(nameof(IsSelected));
            }
        }
        public bool IsVIP
        {
            get { return _IsVIP; }
            set
            {
                _IsVIP = value;
                RaisePropertyChanged(nameof(IsVIP));
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
        public int SessionPrice
        {
            get
            {
                return _SessionPrice;
            }
            set
            {
                _SessionPrice = value;
                RaisePropertyChanged(nameof(SessionPrice));
            }
        }
        public int MonthPrice
        {
            get
            {
                return _MonthPrice;
            }
            set
            {
                _MonthPrice = value;
                RaisePropertyChanged(nameof(MonthPrice));
            }
        }
        public int TwoMonthPrice
        {
            get
            {
                return _TwoMonthPrice;
            }
            set
            {
                _TwoMonthPrice = value;
                RaisePropertyChanged(nameof(TwoMonthPrice));
            }
        }
        public int SeasonPrice
        {
            get
            {
                return _SeasonPrice;
            }
            set
            {
                _SeasonPrice = value;
                RaisePropertyChanged(nameof(SeasonPrice));
            }
        }
        public int HalfYearPrice
        {
            get
            {
                return _HalfYearPrice;
            }
            set
            {
                _HalfYearPrice = value;
                RaisePropertyChanged(nameof(HalfYearPrice));
            }
        }
        public int NineMonthPrice
        {
            get
            {
                return _NineMonthPrice;
            }
            set
            {
                _NineMonthPrice = value;
                RaisePropertyChanged(nameof(NineMonthPrice));
            }
        }
        public int YearPrice
        {
            get
            {
                return _YearPrice;
            }
            set
            {
                _YearPrice = value;
                RaisePropertyChanged(nameof(YearPrice));
            }
        }
        public int MonthPrice2
        {
            get
            {
                return _MonthPrice2;
            }
            set
            {
                _MonthPrice2 = value;
                RaisePropertyChanged(nameof(MonthPrice2));
            }
        }
        public int TwoMonthPrice2
        {
            get
            {
                return _TwoMonthPrice2;
            }
            set
            {
                _TwoMonthPrice2 = value;
                RaisePropertyChanged(nameof(TwoMonthPrice2));
            }
        }
        public int SeasonPrice2
        {
            get
            {
                return _SeasonPrice2;
            }
            set
            {
                _SeasonPrice2 = value;
                RaisePropertyChanged(nameof(SeasonPrice2));
            }
        }
        public int HalfYearPrice2
        {
            get
            {
                return _HalfYearPrice2;
            }
            set
            {
                _HalfYearPrice2 = value;
                RaisePropertyChanged(nameof(HalfYearPrice2));
            }
        }
        public int NineMonthPrice2
        {
            get
            {
                return _NineMonthPrice2;
            }
            set
            {
                _NineMonthPrice2 = value;
                RaisePropertyChanged(nameof(NineMonthPrice2));
            }
        }
        public int YearPrice2
        {
            get
            {
                return _YearPrice2;
            }
            set
            {
                _YearPrice2 = value;
                RaisePropertyChanged(nameof(YearPrice2));
            }
        }
        public int Duration
        {
            get
            {
                return _Duration;
            }
            set
            {
                _Duration = value;
                RaisePropertyChanged(nameof(Price));
            }
        }
        public DateTime? StartDate
        {
            get { return _StartDate; }
            set
            {
                _StartDate = value;
                RaisePropertyChanged(nameof(StartDate));
                RaisePropertyChanged(nameof(DateInfo));
            }
        }
        public DateTime? FinishDate
        {
            get { return _FinishDate; }
            set
            {
                _FinishDate = value;
                RaisePropertyChanged(nameof(FinishDate));
                RaisePropertyChanged(nameof(DateInfo));
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
        public int Freeze
        {
            get
            {
                return _Freeze;
            }
            set
            {
                _Freeze = value;
                RaisePropertyChanged(nameof(Freeze));
            }
        }
        public int Freeze2
        {
            get
            {
                return _Freeze2;
            }
            set
            {
                _Freeze2 = value;
                RaisePropertyChanged(nameof(Freeze2));
            }
        }
        public int Freeze3
        {
            get
            {
                return _Freeze3;
            }
            set
            {
                _Freeze3 = value;
                RaisePropertyChanged(nameof(Freeze3));
            }
        }
        public int Freeze6
        {
            get
            {
                return _Freeze6;
            }
            set
            {
                _Freeze6 = value;
                RaisePropertyChanged(nameof(Freeze6));
            }
        }
        public int Freeze9
        {
            get
            {
                return _Freeze9;
            }
            set
            {
                _Freeze9 = value;
                RaisePropertyChanged(nameof(Freeze9));
            }
        }
        public int Freeze12
        {
            get
            {
                return _Freeze12;
            }
            set
            {
                _Freeze12 = value;
                RaisePropertyChanged(nameof(Freeze12));
            }
        }

        public int FreezeDays
        {
            get
            {
                if (this.Frequency == Enums.Frequencies.SingleSessions) return 0;
                switch (Duration)
                {
                    case 1: return Freeze;
                    case 2: return Freeze2;
                    case 3: return Freeze3;
                    case 6: return Freeze6;
                    case 9: return Freeze9;
                    case 12: return Freeze12;
                    default: return 0;
                }
            }
        }
        public int Sport
        {
            get
            {
                return _Sport;
            }
            set
            {
                _Sport = value;
                RaisePropertyChanged(nameof(Sport));
            }
        }

        public int? MentorId
        {
            get
            {
                return _MentorId;
            }
            set
            {
                _MentorId = value;
                RaisePropertyChanged(nameof(MentorId));
                RaisePropertyChanged(nameof(Price));
            }
        }
        public int? MentorPrice
        {
            get
            {
                return _MentorPrice;
            }
            set
            {
                _MentorPrice = value;
                RaisePropertyChanged(nameof(MentorPrice));
            }
        }
        public int Price
        {
            get
            {
                Func<int> getCourseCost = () =>
                {
                    switch (Frequency)
                    {
                        case Domain.Enums.Frequencies.Everyday:
                            if (Duration == 1) return MonthPrice;
                            if (Duration == 2) return TwoMonthPrice;
                            if (Duration == 3) return SeasonPrice;
                            if (Duration == 6) return HalfYearPrice;
                            if (Duration == 9) return NineMonthPrice;
                            if (Duration == 12) return YearPrice;
                            else return 0;
                        case Domain.Enums.Frequencies.EveryOtherDay:
                            if (Duration == 1) return MonthPrice2;
                            if (Duration == 2) return TwoMonthPrice2;
                            if (Duration == 3) return SeasonPrice2;
                            if (Duration == 6) return HalfYearPrice2;
                            if (Duration == 9) return NineMonthPrice2;
                            if (Duration == 12) return YearPrice2;
                            else return 0;
                        case Domain.Enums.Frequencies.SingleSessions:
                            return Duration * SessionPrice;
                        default:
                            return 0;
                    }
                };

                Func<int> getMentorCost = () =>
                {
                    if (MentorId == null) return 0;
                    else return Mentors.FirstOrDefault(m => m.Id == _MentorId).Price;
                };
                var x = getCourseCost() + getMentorCost();
                return x;
            }
            set
            {
                int shit = value;
                RaisePropertyChanged("Price");

            }
        }
        public string DateInfo
        {
            get
            {

                string s = "";
                if (StartDate.HasValue)
                    s += $"از {StartDate?.ToFa()}";
                if (FinishDate.HasValue)
                    s += $"تا {FinishDate?.ToFa()}";
                return s;
            }
        }
        public bool IsReadonly
        {
            get
            {
                return _IsReadonly;
            }
            set
            {
                _IsReadonly = value;
                RaisePropertyChanged(nameof(IsReadonly));
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

        public class MentorVM
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
        }

        public class CourseListVM : INotifyPropertyChanged
        {
            private bool _HasSelectedVIP;
            public ObservableCollection<CourseVM> Items { get; set; }

            public CourseListVM(ObservableCollection<CourseVM> Items)
            {
                this.Items = Items;
                Items.CollectionChanged += Items_CollectionChanged;
                Items.ToList().ForEach(t => t.PropertyChanged += T_PropertyChanged);
                OnPropertyChanged("Items");
            }

            private void T_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "Count" || e.PropertyName == "Price" ||
                    e.PropertyName == "MentorId" || e.PropertyName == "IsSelected")
                    OnPropertyChanged("Total");
            }

            private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                OnPropertyChanged("VIP");
            }

            public bool HasSelectedVIP
            {
                get
                {
                    return Items.Where(x => x.IsVIP && x.IsSelected).Any();
                }
                set
                {
                    _HasSelectedVIP = value;
                    OnPropertyChanged("HasSelectedVIP");
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

}
