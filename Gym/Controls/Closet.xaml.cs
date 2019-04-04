using Gym.Domain;
using Gym.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Gym.Domain.Enums;

namespace Gym.Controls
{
    /// <summary>
    /// Interaction logic for Closet.xaml
    /// </summary>
    public partial class Closet : UserControl
    {
        public Closet()
        {
            InitializeComponent();
        }

        //ClosetStates _ClosetState = ClosetStates.Free;
        //public ClosetStates ClosetState
        //{
        //    get { return _ClosetState; }
        //    set
        //    {
        //        _ClosetState = value;
        //        if (value == ClosetStates.Broken)
        //        { SetFixed.IsEnabled = true; SetBroken.IsEnabled = false; }
        //        else
        //        { SetFixed.IsEnabled = false; SetBroken.IsEnabled = true; }
        //        SetClosetColor();
        //    }
        //}

        public bool IsBroken { get { return ClosetModel.IsBroken; } }
        public bool NotBroken { get { return !ClosetModel.IsBroken; } }

        //public int Number { get { return ClosetModel.Id; } }
        #region Context Menu

        bool _IsManipulatable = false;
        [Category("Miscellaneous")]
        [Description("If true, context menu will work. Otherwise, context menu will be removed.")]
        public bool IsManipulatable
        {
            get { return _IsManipulatable; }
            set
            {
                _IsManipulatable = value;
                BrokenContextMenu.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        #endregion

        //public enum ClosetStates
        //{
        //    /// <summary>
        //    /// Black
        //    /// </summary>
        //    Free,
        //    /// <summary>
        //    /// White
        //    /// </summary>
        //    Broken,
        //    /// <summary>
        //    /// Red
        //    /// </summary>
        //    InUse,
        //    /// <summary>
        //    /// Green
        //    /// </summary>
        //    VIP,

        //}
        //public int Number = 0;

        //void SetClosetColor()
        //{
        //    return;
        //    //switch (ClosetState)
        //    //{
        //    //    case ClosetStates.Free:
        //    //        Icon.Foreground = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString("#212121") };
        //    //        break;
        //    //    case ClosetStates.Broken:
        //    //        Icon.Foreground = new SolidColorBrush { Color = Colors.White };
        //    //        break;
        //    //    case ClosetStates.InUse:
        //    //        Icon.Foreground = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString("#f44336") };
        //    //        break;
        //    //    case ClosetStates.VIP:
        //    //        Icon.Foreground = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString("#00e676") };
        //    //        break;
        //    //    default:
        //    //        break;
        //    //}
        //}

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ClosetModel.PropertyChanged += ClosetModel_PropertyChanged;
            ClosetModel_PropertyChanged(null, null);
        }



        Data.GymContextDataContext db = new Data.GymContextDataContext();
        public ClosetFormType Type = ClosetFormType.Registration;

        //ClosetVM _ClosetModel;
        public ClosetVM ClosetModel;
        private void ClosetModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string color;
            if (ClosetModel.IsVIP)
                color = "#1E90FF";
            else if (ClosetModel.IsCoach)
                color = Colors.Orange.ToString();
            else
                color = "#505050";
            //var color = ClosetModel.IsVIP ? "#1E90FF" : "#505050";
            Icon.Foreground = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(color) };
            brokenBadge.Visibility = ClosetModel.IsBroken ? Visibility.Visible : Visibility.Hidden;
            Icon.Opacity = numberBadge.Opacity = ClosetModel.IsRented ? .4 : 1;
            Icon.Kind = ClosetModel.IsFree ? MaterialDesignThemes.Wpf.PackIconKind.Fridge : MaterialDesignThemes.Wpf.PackIconKind.FridgeFilled;
            numberBadge.Text = ClosetModel.Id.ToString();
            if (!ClosetModel.IsFree) numberBadge.Foreground = new SolidColorBrush(Colors.White);
            else numberBadge.Foreground = Icon.Foreground;
            SetBroken.IsEnabled = !(SetFixed.IsEnabled = ClosetModel.IsBroken);
            //if (db.ClosetUsages.Any(u => u.ClosetId == ClosetModel.Id && u.ToTime == null))
            //{
            //    ClosetModel.User = db.ClosetUsages?.Where(u => u.ClosetId == ClosetModel.Id && u.ToTime == null)
            //       .OrderByDescending(u => u.Id).FirstOrDefault().Member.Fullname() ?? "";
            //}
            Icon.ToolTip = ClosetModel.Hint;
        }

        private void SetBroken_Click(object sender, RoutedEventArgs e)
        {
            db.Closets.FirstOrDefault(c => c.Id == ClosetModel.Id).IsBroken = true;
            db.SubmitChanges();
            this.ClosetModel.IsBroken = true;
        }

        private void SetFixed_Click(object sender, RoutedEventArgs e)
        {
            db.Closets.FirstOrDefault(c => c.Id == ClosetModel.Id).IsBroken = false;
            db.SubmitChanges();
            this.ClosetModel.IsBroken = false;
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Type != ClosetFormType.Selection)
            {
                if (ClosetModel.IsFree) return;

                var closet = db.Closets.Where(c => c.Id == ClosetModel.Id).FirstOrDefault();
                closet.IsFree = true;
                closet.UserId = null;

                closet.ClosetUsages.Where(u => u.ToTime == null).ToList().ForEach(u => u.ToTime = DateTime.Now);
                db.SubmitChanges();
                //Change these 2 lines of code later to open the closet door only (after hardware matching)
                ClosetModel.IsFree = !ClosetModel.IsFree;
                Icon.Kind = ClosetModel.IsFree ? MaterialDesignThemes.Wpf.PackIconKind.Fridge : MaterialDesignThemes.Wpf.PackIconKind.FridgeFilled;
            }

        }
    }
}
