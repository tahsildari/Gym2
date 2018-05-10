using Gym.Controls;
using Gym.Data;
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
    /// Interaction logic for Trades.xaml
    /// </summary>
    public partial class Trades : Window
    {
        public Trades()
        {
            InitializeComponent();
            this.PreviewKeyUp += Window_PreviewKeyUp;
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        Data.GymContextDataContext db = new Data.GymContextDataContext();
        //public ObservableCollection<TradeVM> Goods { get; private set; }
        public TradesVM Goods { get; private set; }
        int MemberId = 0;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //PickedCount = new List<int>(Enumerable.Range(0, 20));
            //this.DataContext = PickedCount;
            var card = new MemberCard(-1);
            card.IsInteractive = false;

            MemberHolder.Children.Clear();
            MemberHolder.Children.Add(card);


            var goods = db.Goods.ToList();
            var trades = new List<TradeVM>();
            goods.ForEach(g =>
            {
                var lastSold = g.Trades?.LastOrDefault(t => t.IsSold)?.Price ?? 0;
                var lastBought = g.Trades?.LastOrDefault(t => !t.IsSold)?.Price ?? 0;

                trades.Add(new TradeVM
                {
                    Id = g.Id,
                    Code = g.Name[0].ToString(),
                    Name = g.Name,
                    Description = $"موجود:{g.Count}",
                    IsSelected = false,
                    Price = lastSold,
                    Hint = $"\tخرید:{ lastBought} - فروش: { lastSold}"
                });
            }
            );
            Goods = new TradesVM(Items: new ObservableCollection<TradeVM>(trades));

            this.DataContext = Goods;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MembersSearch search = new MembersSearch();
            search.MemberSelected += Search_MemberSelected;
            search.ShowDialog();
        }

        private void Search_MemberSelected(MemberVM member)
        {
            if (member != null)
            {
                var card = new MemberCard(member.Id);
                card.IsInteractive = false;

                MemberHolder.Children.Clear();
                MemberHolder.Children.Add(card);

                MemberId = member.Id;
                txtCredit.Text = member.Credit.ToString("#,##0");
            }

            btnPay.IsEnabled = member != null;
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            if (Goods.Total > 0 && MemberId > 0)
            {
                Goods.Items.Where(i => i.IsSelected).ToList().ForEach(i =>
                {
                    db.Trades.InsertOnSubmit(new Trade
                    {
                        GoodId = i.Id,
                        Count = i.Count,
                        IsSold = true,
                        MemberId = MemberId,
                        Price = i.Price,
                        Time= DateTime.Now
                    });

                    db.Goods.Where(g => g.Id == i.Id).FirstOrDefault().Count -= i.Count;
                });

                db.Transactions.InsertOnSubmit(new Transaction
                {
                    Amount = Goods.Total,
                    Datetime = DateTime.Now,
                    MemberId = MemberId,
                    UserId = Main.CurrentUser.Id,
                    Method = (byte)PaymentMethods.Credit,
                    Type = (byte)TransactionType.BuyStuff,
                    Info = "خرید از بوفه"
                });

                var member = db.Members.Where(m => m.Id == MemberId).FirstOrDefault();
                member.Credit -= Goods.Total;

                db.SubmitChanges();

                var needOrder = db.Goods.Where(g => g.OrderPoint <= g.Count && Goods.Items.Select(i => i.Id).Contains(g.Id)).ToList();
                Main.Home.CheckupOrderPoints();
                if (member.Credit < 0)
                    Main.Home.CheckupCreditDebtors();

                (this.Owner as Main).AlertOrderPoints(needOrder);
                Close();
            }
        }
    }
}
