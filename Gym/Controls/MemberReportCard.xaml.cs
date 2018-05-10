using Gym.Data;
using Gym.Domain;
using Gym.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gym.Controls
{
    /// <summary>
    /// Interaction logic for MemberReportCard.xaml
    /// </summary>
    public partial class MemberReportCard : UserControl
    {
        Member member;

        public MemberReportCard(Member member)
        {
            InitializeComponent();

            this.member = member;
            FillData();
            //txtName.Text = member.Firstname + " " + member.Lastname;
            //txtDesc.Text = member.Address + " - " + member.Mobile;
            //txtOperator.Text = member.Operator;
            //btnDeleted.IsEnabled = member.IsDeleted;
        }

        public Window Owner;

        void FillData()
        {
            txtName.Text = member.Fullname();
            txtDesc.Text = member.Address + " - " + member.Mobile;
            txtOperator.Text = member.User.Username;
            btnDeleted.IsEnabled = member.IsDeleted;

            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + $"/Images/{member.Id}.jpg"))
            {
               var  _Image = new BitmapImage();
                _Image.BeginInit();
                _Image.StreamSource = System.IO.File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + $"/Images/{member.Id}.jpg");
                _Image.EndInit();

                Image.Source = _Image;
            }
        }

        public class MemberReportData
        {
            public int Id { get; internal set; }
            public string Firstname { get; internal set; }
            public string Lastname { get; internal set; }
            public string Address { get; internal set; }
            public string Mobile { get; internal set; }
            public bool IsDeleted { get; internal set; }
            public string Operator { get; internal set; }
        }

        Data.GymContextDataContext db = new Data.GymContextDataContext();

        private void btnFinancial_Click(object sender, RoutedEventArgs e)
        {
            Windows.Enroll enroll = new Windows.Enroll();
            enroll.Section = 3;
            enroll.Owner = this.Owner;
            enroll.Show();
            enroll.LoadMember(member.Id);
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            Windows.Enroll enroll = new Windows.Enroll();
            enroll.Owner = this.Owner;
            enroll.Show();
            enroll.LoadMember(member.Id);
        }
    }
}
