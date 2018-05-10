using Gym.ViewModels;
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
using Gym.Domain;

namespace Gym.Controls
{
    /// <summary>
    /// Interaction logic for MemberCard.xaml
    /// </summary>
    public partial class MemberCard : UserControl
    {
        public MemberCard(int id)
        {
            InitializeComponent();
            if (id == -1)
            {
                ImageBox.Stroke = Brushes.White;
                ImageBox.Fill = Brushes.DodgerBlue;
                var vm = new MemberVM {
                Firstname = "عضو را",
                Lastname = "انتخاب کنید"
            };

                this.DataContext = vm;
                //ImageBrush.ImageSource = new BitmapImage(new Uri(@"\Images\body.png", UriKind.Relative));
                return;
            }
                
            var m = db.Members.Where(x => x.Id == id).FirstOrDefault();
            Member = new MemberVM
            {
                Id = m.Id,
                Firstname = m.Firstname,
                Lastname = m.Lastname,
                //Address = m.Address,
                //BirthDate = m.Birthdate?.ToEn(),
                //Description = m.Description,
                //Fathername = m.Dadsname,
                //Mobile = m.Mobile,
                //NationalCode = m.NationalCode,
                //Referrer = m.Referrer,
                //ReferrerMobile = m.ReferrerMobile,
                //IsActive = m.IsActive,
                //Credit = m.Credit,
                //Debtor = m.Debtor,
                //InsuranceNo = m.InsuranceNo,
                //InsuranceExpireDate = m.InsuranceExpiry.ToEn(),
                //ClosetId = db.Closets.Where(c=>c.RentorId == m.Id).FirstOrDefault()?.Id
            };
            if (Member.Image == null)
            {
                ImageBox.Visibility = Visibility.Collapsed;
                NoImage.Visibility = Visibility.Visible;
            }
            this.DataContext = Member;
        }
        Data.GymContextDataContext db = new Data.GymContextDataContext();

        public MemberVM Member;

        public enum MemberGraphics {
            Card, Chip
        }
        private MemberGraphics MemberGraphic = MemberGraphics.Card;
        public void SetGraphics(MemberGraphics graphics)
        {
            MemberGraphic = graphics;
            switch (MemberGraphic)
            {
                case MemberGraphics.Card:
                    Card.Visibility = Visibility.Visible;
                    Chip.Visibility = Visibility.Collapsed;
                    break;
                case MemberGraphics.Chip:
                    Card.Visibility = Visibility.Collapsed;
                    Chip.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        public delegate void MemberEvent(MemberVM member);
        public event MemberEvent MemberSelected;

        public bool IsInteractive { get; set; }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (MemberSelected != null)
            {
               var m = db.Members.Where(x => x.Id == Member.Id).FirstOrDefault();
                Member = new MemberVM
                {
                    Id = m.Id,
                    Firstname = m.Firstname,
                    Lastname = m.Lastname,
                    Address = m.Address,
                    BirthDate = m.Birthdate?.ToEn(),
                    Description = m.Description,
                    Fathername = m.Dadsname,
                    Mobile = m.Mobile,
                    NationalCode = m.NationalCode,
                    Referrer = m.Referrer,
                    ReferrerMobile = m.ReferrerMobile,
                    IsActive = m.IsActive,
                    Credit = m.Credit,
                    Debtor = m.Debtor,
                    InsuranceNo = m.InsuranceNo,
                    InsuranceExpireDate = m.InsuranceExpiry.ToEn(),
                    ClosetId = db.Closets.Where(c => c.RentorId == m.Id).FirstOrDefault()?.Id
                };
                MemberSelected(Member);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BigButton.IsEnabled = Chip.IsEnabled = IsInteractive;
        }
    }
}
