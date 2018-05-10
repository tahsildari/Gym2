using Gym.Domain;
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
using static Gym.Controls.Closet;

namespace Gym.Controls
{
    /// <summary>
    /// Interaction logic for Doors.xaml
    /// </summary>
    public partial class Closets : UserControl
    {
        public Closets()
        {
            InitializeComponent();
        }
        Data.GymContextDataContext db = new Data.GymContextDataContext();

        int _ClosetSize = 24;
        [Category("Miscellaneous")]
        [Description("The width and height of each Closet in the series")]
        public int ClosetSize
        {
            get { return _ClosetSize; }
            set
            {
                _ClosetSize = value;
                closetsStackPanel.Children.Cast<FrameworkElement>().ToList().ForEach(
                    c => { c.Width = value; closetsStackPanel.Height = value; });
            }
        }

        public delegate void ClosetEvent(int closetId);
        public event ClosetEvent ClosetSelected;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadClosets();
            if (Window.GetWindow(this).GetType().Name != "Main")
            {
                closetsStackPanel.Style = null;
            }
        }

        public void LoadClosets()
        {
            db = new Data.GymContextDataContext();
            var closets = db.Closets;
            closetsStackPanel.Children.Clear();
            foreach (Data.Closet closet in closets)
            {
                //ClosetStates state;
                //if (closet.IsBroken)
                //    state = ClosetStates.Broken;
                //else if (closet.UserId != null)
                //    state = ClosetStates.InUse;
                //else
                //{
                //    if (closet.IsVip) state = ClosetStates.VIP;
                //    else state = ClosetStates.Free;
                //}


                //Closet c = new Closet { ClosetState = state, Number = closet.Id + 1, Height = ClosetSize, DataContext = closet };
                Closet c = new Closet
                {
                    //ClosetState = state,
                    Height = ClosetSize,
                    ClosetModel = new ViewModels.ClosetVM
                    {
                        Id = closet.Id,
                        IsBroken = closet.IsBroken,
                        IsVIP = closet.IsVip,
                        IsRented = closet.Rentor != null,
                        IsFree = closet.IsFree,
                        Rentor = closet.Rentor,
                        User = closet.User
                        //closet.ClosetUsages.Any(u => u.ClosetId == closet.Id && u.ToTime == null) ?
                        //         closet.ClosetUsages.Where(u => u.ClosetId == closet.Id && u.ToTime == null)
                        //        .OrderByDescending(u => u.Id).FirstOrDefault().Member.Fullname() : ""
                    }
                };

                if (ClosetSelected != null)
                {
                    c.Type = Enums.ClosetFormType.Selection;
                    c.MouseDoubleClick += (s, me) =>
                    {
                        if (closet.RentorId == null)
                            ClosetSelected(closet.Id);
                    };
                }
                //c.MouseDoubleClick += Closet_MouseDoubleClick;
                closetsStackPanel.Children.Add(c);
            }


        }

        //private void Closet_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    //var closet = ((Closet)sender);
        //    ////if (closet.ClosetState == ClosetStates.InUse)
        //    ////    closet.ClosetState = ClosetStates.Free;
        //    //if (!closet.ClosetModel.IsFree)
        //    //    closet.ClosetModel.IsFree = true;
        //    //else
        //    //    closet.ClosetModel.IsFree = false;

        //}


        bool _IsManipulatable = false;
        [Category("Miscellaneous")]
        [Description("If true, context menu will work. Otherwise, context menu will be removed.")]
        public bool IsManipulatable
        {
            get { return _IsManipulatable; }
            set
            {
                _IsManipulatable = value;
                closetsStackPanel.Children.Cast<Closet>().ToList().ForEach(
                   c => { c.IsManipulatable = value; });
            }
        }
    }
}
