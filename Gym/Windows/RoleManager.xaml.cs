using Gym.Data;
using Gym.Domain;
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

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for RoleManager.xaml
    /// </summary>
    public partial class RoleManager : Window
    {
        public RoleManager()
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

        Data.GymContextDataContext db = new GymContextDataContext();
        bool isEditing() => roleId != -1;
        int roleId = -1;


        private void NewRole_Click(object sender, RoutedEventArgs e)
        {
            roleId = -1;
            txtName.IsEnabled = true;
            txtName.Text = "";
            //FormsList.Children.Cast<CheckBox>().ToList().ForEach(c => c.IsChecked = false);
            //SetupFormsList();
            ClearAddedForms();
        }

        private void ClearAddedForms()
        {
            AddedForms.Items.Cast<string>().ToList().ForEach(af => FormsList.Items.Add(af));
            AddedForms.Items.Clear();
            //for (int i = 0; i < Forms.Count; i++)
            //{
            //    FormsList.Items.Add(Forms[i]);
            //    AddedForms.Items.Clear();
            //}
        }

        private void EditRole_Click(object sender, RoutedEventArgs e)
        {
            var role = ((FrameworkElement)sender).DataContext as Data.Role;
            var access = db.Roles.Where(r => r.Id == role.Id).FirstOrDefault().Access;

            ReadAccess(access);


            txtName.Text = role.Name;
            UserDialogHost.IsOpen = true;
            txtName.IsEnabled = false;

            roleId = role.Id;
            //access.ToList().ForEach(a=>  )
            //FormsList.Children.Cast<CheckBox>().ToList().ForEach(c => access += (c.IsChecked == true ? "1" : "0"));
        }

        private void ReadAccess(string access)
        {
            AddedForms.Items.Clear();
            FormsList.Items.Clear();
            for (int i = 0; i < access.Length; i++)
            {
                var a = access[i];
                if (a == '1')
                    AddedForms.Items.Add(Forms[i]);
                else
                    FormsList.Items.Add(Forms[i]);
                //        .Items[i] = a == '1' ? Forms[i] : "";
                //FormsList.Items[i] = a == '0' ? Forms[i] : "";

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupFormsList();
            RefreshGrid();
        }

        void SetupFormsList()
        {
            //FormsList.Items.Clear();

            var i = 0;
            AddBox("ثبت نام", i++);
            AddBox("فهرست اعضا", i++);
            AddBox("اعضای حاضر", i++);
            AddBox("حذف عضو", i++);
            AddBox("پرسنل و مربی ها", i++);

            AddBox("افزودن به انبار", i++);
            AddBox("فروش کالا", i++);
            AddBox("افزایش اعتبار", i++);

            AddBox("لیست اعضا", i++);
            AddBox("فروشگاه", i++);
            AddBox("مالی کاربران", i++);
            AddBox("تردد اعضا", i++);
            AddBox("تردد پرسنل", i++);
            AddBox("کمدها", i++);
            AddBox("تک چلسه ای", i++);
            AddBox("نمودار مالی", i++);

            AddBox("ثبت هزینه", i++);
            AddBox("مشاهده هزینه", i++);
            AddBox("تراز مالی", i++);

            AddBox("رشته های ورزشی", i++);
            AddBox("دوره های باشگاه", i++);
            AddBox("امکانات باشگاه", i++);
            AddBox("کمدها", i++);
            AddBox("حساب های هزینه", i++);
            AddBox("کارتخوان", i++);
            AddBox("کاربران", i++);
            AddBox("نقش ها", i++);
            AddBox("تلگرام", i++);

            //for (int c = 0; c < 27; c++)
            //{
            //    FormsList.Items.Add("");
            //    AddedForms.Items.Add("");
            //}
        }

        Dictionary<int, string> Forms = new Dictionary<int, string>();
        void AddBox(string txt, int i)
        {
            Forms.Add(i, txt);
            FormsList.Items.Add(txt);
            /*FormsList.Items[i] = txt;*/ //new CheckBox { Content = txt, IsChecked = false, Margin = new Thickness(0, 0, 4, 2) });
        }

        void RefreshGrid()
        {
            UsersGrid.DataContext = new ObservableCollection<dynamic>(new Data.GymContextDataContext().Roles.ToList());
        }

        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            var confirmed = (bool)eventArgs.Parameter;
            if (confirmed)
            {
                switch (isEditing())
                {
                    case false:
                        {
                            Role r = new Role();
                            r.Name = txtName.Text;
                            r.Access = CalculateAccess();

                            db.Roles.InsertOnSubmit(r);
                            db.SubmitChanges();
                            break;
                        }
                    case true:
                        {
                            Role o = db.Roles.Where(r => r.Id == roleId).FirstOrDefault();
                            //o.Name = txtName.Text;
                            o.Access = CalculateAccess();
                            db.SubmitChanges();
                            roleId = -1;
                            break;
                        }
                }
                RefreshGrid();

            }
            else
            {
                Dynamics.LastEscapeTime = DateTime.Now;
            }
        }

        private string CalculateAccess()
        {
            var access = new string('0', Forms.Count);
            AddedForms.Items.Cast<string>().ToList().ForEach(i =>
            {
                var result = access.ToArray();
                result[Forms.FirstOrDefault(f => f.Value == i).Key] = '1';
                access = new string(result);
            });
            //i.Length > 0 ? "1" : "0");
            return access;
        }

        private void FormsList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (FormsList.SelectedItem == null) return;
            //var i = FormsList.SelectedIndex;
            //if (FormsList.Items[i].ToString() != "")
            //{
            //    FormsList.Items[i] = "";
            //    AddedForms.Items[i] = Forms[i];
            //}
            var a = FormsList.SelectedItem.ToString();
            FormsList.Items.RemoveAt(FormsList.SelectedIndex);
            AddedForms.Items.Add(a);
        }

        private void AddedForms_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (AddedForms.SelectedItem == null) return;
            var a = AddedForms.SelectedItem.ToString();
            AddedForms.Items.RemoveAt(AddedForms.SelectedIndex);
            FormsList.Items.Add(a);
            //var i = Forms.Values.Where(v=>v == a).
            //var i = AddedForms.SelectedIndex;
            //if (AddedForms.Items[i].ToString() != "")
            //{
            //    AddedForms.Items[i] = "";
            //    FormsList.Items[i] = Forms[i];
            //}
        }
    }
}
