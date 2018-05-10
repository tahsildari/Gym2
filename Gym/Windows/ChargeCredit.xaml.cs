using Gym.Controls;
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
using static Gym.Domain.Enums;

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for ChargeCredit.xaml
    /// </summary>
    public partial class ChargeCredit : Window
    {
        int MemberId;

        public ChargeCredit()
        {
            InitializeComponent();



            var card = new MemberCard(-1);
            card.IsInteractive = false;

            MemberHolder.Children.Clear();
            MemberHolder.Children.Add(card);
            this.PreviewKeyUp += Window_PreviewKeyUp;
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            if (MemberId > 0)
            {
                PaymentDialogHost.IsOpen = true;
            }
        }

        private void SelectMember_Click(object sender, RoutedEventArgs e)
        {
            MembersSearch members = new MembersSearch();
            members.showIrregularMembers = true;
            members.showOnlyPresentMembers = true;
            members.Type = Domain.Enums.MemberSelectionCategory.SelectionOnly;
            members.MemberSelected += Members_MemberSelected;
            members.ShowDialog();
        }

        private void Members_MemberSelected(ViewModels.MemberVM member)
        {
            MemberHolder.Children.Clear();
            if (member != null)
            {
                MemberHolder.Children.Add(new MemberCard(member.Id));
                txtCredit.Text = $"{member.Credit:n0}";
                MemberId = member.Id;
            }
        }

        private void Dialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            if (MemberId > 0)
            {

                var confirmed = (bool)eventArgs.Parameter;
                if (confirmed)
                {
                    Data.GymContextDataContext db = new Data.GymContextDataContext();

                    byte method = (byte)(rdCash.IsChecked == true ? 0 : (rdPos.IsChecked == true ? 1 : (rdCard.IsChecked == true ? 2 : 3)));

                    Data.Transaction transaction = new Data.Transaction();
                    transaction.Amount = txtChargeCredit.Value;
                    transaction.Datetime = DateTime.Now;
                    transaction.Info = txtInfo.Text;
                    transaction.MemberId = MemberId;
                    transaction.UserId = Main.CurrentUser.Id;
                    transaction.Method = method;
                    transaction.Type = (byte)TransactionType.Credit;

                    var member = db.Members.Where(m => m.Id == MemberId).FirstOrDefault();
                    member.Credit += transaction.Amount;

                    db.Transactions.InsertOnSubmit(transaction);
                    db.SubmitChanges();

                    txtCredit.Text = $"{member.Credit:n0}";
                    Main.Home.CheckupCreditDebtors();
                }
            }
        }
    }
}
