using System;
using System.Collections.Generic;
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
using Gym.Data;
using System.Collections.ObjectModel;
using Gym.ViewModels;
using static Gym.Domain.Enums;

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for Goods.xaml
    /// </summary>
    public partial class Goods : Window
    {
        public Goods()
        {
            InitializeComponent();
            this.PreviewKeyUp += Window_PreviewKeyUp;
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }
        string Count { get; set; }
        Data.GymContextDataContext db = new Data.GymContextDataContext();

        public ObservableCollection<Good> DataList { get; private set; }
        GoodVM Model = new GoodVM();

        Actions action = Actions.Inserting;
        bool IsNew() => action == Actions.Inserting;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
            this.DataContext = Model;
        }

        private void NewGoods_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = Model = new GoodVM();
            action = Actions.Inserting;
            txtCount.IsReadOnly = false;
            BuyGoods.Visibility = Visibility.Collapsed;
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
                                    db.Goods.InsertOnSubmit(
                                        new Data.Good
                                        {
                                            Name = Model.Name,
                                            Count = Model.Count,
                                            OrderPoint = Model.OrderPoint
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
                                var good = db.Goods.Where(g => g.Id == Model.Id).FirstOrDefault();
                                good.Name = Model.Name;
                                good.OrderPoint = Model.OrderPoint;
                                good.Count = Model.Count;

                                db.SubmitChanges();
                                RefreshGrid();
                                break;
                            default:
                                break;
                        }
                    }

                    Main.Home.CheckupOrderPoints();

                }
                else if (BuyGoods.Visibility == Visibility.Visible)
                {
                    if (txtBuyCount.Value > 0 && txtBuyPrice.Value > 0)
                    {
                        db.Trades.InsertOnSubmit(new Trade
                        {
                            GoodId = BuyingGoodId,
                            Count = txtBuyCount.Value,
                            IsSold = false,
                            Price = txtBuyPrice.Value
                        });
                        db.Goods.Where(g => g.Id == BuyingGoodId).FirstOrDefault().Count += txtBuyCount.Value;
                        db.SubmitChanges();
                        RefreshGrid();
                        Main.Home.CheckupOrderPoints();
                    }

                }
            }
        }

        public bool FilterGoodsBelowOrderPoint = false;
        private void RefreshGrid()
        {
            db = new GymContextDataContext();
            IQueryable<Good> query = db.Goods;
            if (FilterGoodsBelowOrderPoint)
                query = query.Where(g => g.Count <= g.OrderPoint);
                var goods = query.ToList();
            DataList = new ObservableCollection<Data.Good>(goods);

            GoodsGrid.DataContext = goods;
        }

        private void EditGood_Click(object sender, RoutedEventArgs e)
        {
            var good = ((FrameworkElement)sender).DataContext as Data.Good;
            Model.Name = good.Name;
            Model.Count = good.Count;
            Model.OrderPoint = good.OrderPoint;
            Model.Id = good.Id;
            action = Actions.Editing;
            //txtCount.IsReadOnly = true;

            BuyGoods.Visibility = Visibility.Collapsed;
            DefineGoods.Visibility = Visibility.Visible;

            DialogHost.IsOpen = true;
        }

        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {
            DuplicateNameSnackBar.IsActive = false;
        }

        int BuyingGoodId = 0;
        private void BuyGood_Click(object sender, RoutedEventArgs e)
        {
            BuyGoods.Visibility = Visibility.Visible;
            DefineGoods.Visibility = Visibility.Collapsed;

            db = new GymContextDataContext();

            var good = ((FrameworkElement)sender).DataContext as Data.Good;
            good = db.Goods.Where(g => g.Id == good.Id).FirstOrDefault();
            BuyingGoodId = good.Id;
            txtBuyName.Text = good.Name;
            txtBuyCount.Value = 0;
            if (good.Trades.Any(t => !t.IsSold))
                txtBuyPrice.Value = good.Trades.Where(t => !t.IsSold).OrderByDescending(t => t.Id)
                    .FirstOrDefault().Price;
            else
                txtBuyPrice.Value = 0;

            DialogHost.IsOpen = true;
            Main.Home.CheckupOrderPoints();
        }
        
    }
}
