using Gym.Controls;
using Gym.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
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
using System.Windows.Shapes;
using static Gym.Domain.Enums;

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for InitializeClosets.xaml
    /// </summary>
    public partial class InitializeClosets : Window
    {
        public InitializeClosets()
        {
            InitializeComponent();
            //Closets.DataContext = Closets;
            this.PreviewKeyUp += Window_PreviewKeyUp;
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        Data.GymContextDataContext db = new Data.GymContextDataContext();

        //ObservableCollection<Data.Closet> Closets
        //{
        //    get
        //    {
        //        return new ObservableCollection<Data.Closet>(db.Closets.ToList());
        //    }
        //}
        ClosetFormType _Type = ClosetFormType.Selection;
        public ClosetFormType Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
                if (value == ClosetFormType.Selection)
                {
                    this.Closets.ClosetSelected += Closets_ClosetSelected;
                    //this.Closets.closetsStackPanel.Children.Cast<Closet>().ToList().ForEach(c => {
                    //    c.MouseDoubleClick += Closet_MouseDoubleClick;
                    //});
                }
            }
        }

        private void Closets_ClosetSelected(int closetId)
        {
            if (ClosetSelected != null)
            {
                //var id = (sender as Closet).ClosetModel.Id;
                ClosetSelected(closetId);
                this.Close();
            }
        }

        public delegate void ClosetEvent(int closetId);
        public event ClosetEvent ClosetSelected;
        

        private void UserDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            ClosetRangeVM Closet = new ClosetRangeVM();
            int from, to;
            if (int.TryParse(txtFrom.Text, out from) && int.TryParse(txtTo.Text, out to))
            {
                Closet.FromLabel = from;
                Closet.ToLabel = to;
                Closet.IsVIP = cmbType.SelectedIndex == 1;

                var closetsList = Closet.GetClosets();
                if (!db.Closets.Any(c => closetsList.Select(l => l.Id).Contains(c.Id)))
                {
                    db.Closets.InsertAllOnSubmit(closetsList);
                    db.SubmitChanges();

                    Main.Home.Closets.LoadClosets();
                    this.Closets.LoadClosets();
                }
            }
        }
    }
}
