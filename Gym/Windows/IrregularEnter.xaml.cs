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
using System.Windows.Shapes;
using Gym.Domain;
using static Gym.Domain.Enums;

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for IrregularEnter.xaml
    /// </summary>
    public partial class IrregularEnter : Window
    {
        public IrregularEnter()
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
            this.DataContext = Member;
            DialogHost.IsOpen = true;
            txtSingleSessionPrice.Value = Dynamics.SingleSessionPrice;
            txtMobile.Focus();
        }

        MemberVM Member = new MemberVM();
        Data.GymContextDataContext db = new Data.GymContextDataContext();
        private void txtMobile_TextChanged(object sender, TextChangedEventArgs e)
        {
            var matches = db.Members.Where(m => !m.IsDeleted && !m.IsRegular && m.Mobile == txtMobile.Text);
            if (matches.Count() == 1)
            {
                this.Member = matches.Select(m =>
                new MemberVM
                {
                    Id = m.Id,
                    Firstname = m.Firstname,
                    Lastname = m.Lastname,
                    Mobile = m.Mobile,
                    IsRegular = false
                }).FirstOrDefault();
                txtFirstName.Text = Member.Firstname;
                txtLastName.Text = Member.Lastname;

            }
            else
            {
                Member = new MemberVM();
                txtFirstName.Text = "";
                txtLastName.Text = "";
            }
        }

        class IrregularMmeber
        {
            public string Mobile { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string NationalCode { get; set; }
        }

        private void Dialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            var confirmed = (bool)eventArgs.Parameter;
            if (confirmed)
            {
                if (!string.IsNullOrEmpty(txtFirstName.Text) && !string.IsNullOrEmpty(txtLastName.Text)
                    && !string.IsNullOrEmpty(txtMobile.Text))
                {
                    int? id;
                    if (Member.Id == 0)
                    {
                        Member = new MemberVM();
                        Member.Firstname = txtFirstName.Text;
                        Member.Lastname = txtLastName.Text;
                        Member.Mobile = txtMobile.Text;
                        Member.NationalCode = txtNationalCode.Text;
                        Member.IsRegular = false;
                         id = Member.Insert();
                        db = new Data.GymContextDataContext();
                    }
                    else
                        id = Member.Id;
                    if (id.HasValue)
                    {
                        Member.Id = id.Value;
                        Data.Passage enter = new Data.Passage { IsEntrance = true, MemberId = id.Value, Time = DateTime.Now };

                        byte method = (byte)(rdCash.IsChecked == true ? 0 : (rdPos.IsChecked == true ? 1 : (rdCard.IsChecked == true ? 2 : 3)));

                        Data.Transaction pay = new Data.Transaction
                        {
                            Amount = txtPayable.Value,
                            Datetime = DateTime.Now,
                            Info = "تک جلسه" + (txtFacilityCost.Value > 0 ? " + امکانات" : ""),
                            MemberId = id.Value,
                            Method = method,
                            UserId = Main.CurrentUser.Id,
                            Type = (byte)TransactionType.SingleSession
                        };

                        db.Passages.InsertOnSubmit(enter);
                        db.Transactions.InsertOnSubmit(pay);
                        db.SubmitChanges();

                        Member.UseCloset(db);

                        var main = Application.Current.Windows.Cast<Window>().ToList().Find(w => w.GetType().Name == "Main") as Main;
                        main.TransitList.UpdatePassages();

                        Main.Home.Closets.LoadClosets();
                    }
                }
            }
            this.Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            txtSingleSessionPrice.Value = Dynamics.SingleSessionPrice;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            txtSingleSessionPrice.Value = 0;

        }
    }
}
