using Gym.Data;
using Gym.Domain;
using Gym.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for POS.xaml
    /// </summary>
    public partial class POS : Window
    {
        public POS()
        {
            InitializeComponent();
            this.PreviewKeyUp += Window_PreviewKeyUp;
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                var escTime = (DateTime)(Dynamics.LastEscapeTime ?? DateTime.Now.AddDays(-1));
                if ((DateTime.Now - escTime) > TimeSpan.FromMilliseconds(100))
                    this.Close();
            }
        }
        Data.GymContextDataContext db = new Data.GymContextDataContext();

        public ObservableCollection<Data.POS> DataList { get; private set; }
        PosVM Model = new PosVM();

        Actions action = Actions.Inserting;
        bool IsNew() => action == Actions.Inserting;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
            this.DataContext = Model;
        }

        private void NewGoods_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = Model = new PosVM();
            action = Actions.Inserting;
            DefineGoods.Visibility = Visibility.Visible;
        }

        private void MentorDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            var confirmed = (bool)eventArgs.Parameter;
            if (confirmed)
            {
                if (DefineGoods.Visibility == Visibility.Visible)
                {
                    if (!string.IsNullOrEmpty(Model.Name))
                    {
                        switch (action)
                        {
                            case Actions.Inserting:
                                if (!db.Goods.Any(g => g.Name == Model.Name))
                                {
                                    db.POS.InsertOnSubmit(
                                        new Data.POS
                                        {
                                            Name = Model.Name
                                        });
                                    db.SubmitChanges();

                                    RefreshGrid();
                                }
                                else
                                {
                                    DuplicateNameSnackBar.IsActive = true;


                                    System.Threading.Thread t = new System.Threading.Thread(() =>
                                    {
                                        var started = DateTime.Now;
                                        while (DateTime.Now.Subtract(started).TotalSeconds < 3)
                                        { }
                                        this.Dispatcher.Invoke(
                                        new Action(() => { DuplicateNameSnackBar.IsActive = false; }));
                                    }); t.Start();
                                }
                                break;
                            case Actions.Editing:
                                var item = db.POS.Where(p => p.Id == Model.Id).FirstOrDefault();
                                item.Name = Model.Name;

                                db.SubmitChanges();
                                RefreshGrid();
                                break;
                            default:
                                break;
                        }
                    }

                    

                }
            }
            else
            {
                Dynamics.LastEscapeTime = DateTime.Now;
            }
        }

        public bool FilterGoodsBelowOrderPoint = false;
        private void RefreshGrid()
        {
            db = new GymContextDataContext();
            List<Data.POS> poses = db.POS.ToList();
            DataList = new ObservableCollection<Data.POS>(poses);

            GoodsGrid.DataContext = poses;
        }

        private void EditGood_Click(object sender, RoutedEventArgs e)
        {
            var pos = ((FrameworkElement)sender).DataContext as Data.POS;
            Model.Name = pos.Name;
            Model.Id = pos.Id;
            action = Actions.Editing;
            //txtCount.IsReadOnly = true;

            DefineGoods.Visibility = Visibility.Visible;

            DialogHost.IsOpen = true;
        }

        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {
            DuplicateNameSnackBar.IsActive = false;
        }

        int BuyingGoodId = 0;
    }
}
