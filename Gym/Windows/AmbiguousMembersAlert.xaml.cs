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

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for AmbiguousMembersAlert.xaml
    /// </summary>
    public partial class AmbiguousMembersAlert : Window
    {
        public AmbiguousMembersAlert()
        {
            InitializeComponent();
        }
        bool flag = false;  
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var db = new Data.GymContextDataContext();
            var amb = (from m in db.Members
                       where m.Passages.Any() &&
                       m.Passages.OrderByDescending(p => p.Id).FirstOrDefault().IsEntrance == true
                       select new { Id = m.Id, Name = m.Firstname + " " + m.Lastname }).ToList();
            amb.ForEach(m =>
            {
                Holder.Children.Add(new CheckBox { Content = m.Name, Style = sample.Style, Tag = m.Id,HorizontalContentAlignment= HorizontalAlignment.Right });
            });
            DialogHost.IsOpen = true;
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void btnIgnore_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            var db = new Data.GymContextDataContext();
            Holder.Children.Cast<CheckBox>().ToList().ForEach(member =>
            {
                if (member.IsChecked == true)
                {
                    db.Passages.InsertOnSubmit(new Data.Passage
                    {
                        IsEntrance = false,
                        MemberId = (int)member.Tag,
                        Time = DateTime.Now
                    });
                }
            });
            db.SubmitChanges();
            DialogResult = true;
            this.Close();
        }

        private void Dialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {

        }

        private void btnCheckAll_Click(object sender, RoutedEventArgs e)
        {
            Holder.Children.Cast<CheckBox>().ToList().ForEach(c => c.IsChecked = !flag);
            flag = !flag;
        }
    }
}
