using Gym.Controls;
using Gym.Domain;
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
using static Gym.Controls.MemberReportCard;

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for MembersReport.xaml
    /// </summary>
    public partial class MembersReport : Window
    {
        public MembersReport()
        {
            InitializeComponent();
            this.PreviewKeyUp += Window_PreviewKeyUp;
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //txtFirstname.Text = "مهدی";
            //txtLastname.Text = "تحصیلداری";
            //txtMobile.Text = "09156606004";
            fromDate.SetToday();
            toDate.SetToday();
            //Transactions.SetFilters(null, null, 1, Domain.Enums.TransactionType.Tuition);
        }

        Data.GymContextDataContext db = new Data.GymContextDataContext();

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var from = fromDate.Date.ToEn();
            var to = toDate.Date.ToEn();

            var matches = db.Members.Where(m =>
                m.IsStaff == false && m.IsMentor == false
                && (m.Firstname.Contains(txtSearchBox.Text) || m.Lastname.Contains(txtSearchBox.Text))
                && m.Date <= (to ?? m.Date) && m.Date >= (from ?? m.Date)).Select(m => m).ToList();
            //(m =>
            //    new MemberReportData
            //    {
            //        Id = m.Id,
            //        Firstname = m.Firstname,
            //        Lastname = m.Lastname,
            //        Address = m.Address,
            //        Mobile = m.Mobile,
            //        IsDeleted = m.IsDeleted,
            //        Operator = m.User.Username
            //    }
            //    );
            MembersArea.Children.Clear();
            foreach (var member in matches)
            {
                var card = new MemberReportCard(member);
                card.Owner = this;
                MembersArea.Children.Add(card);
            }

        }

        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TransitDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {

        }

        private void txtSearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSearch_Click(null, null);
            }
        }
    }
}
