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
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : Window
    {
        public Users()
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

        ObservableCollection<Data.User> UsersList;
        ObservableCollection<Data.Role> RolesList;
        Data.GymContextDataContext db = new Data.GymContextDataContext();
        UserVM UserModel = new UserVM();

        Actions action = Actions.Inserting;
        bool IsNew() => action == Actions.Inserting;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
            this.DataContext = UserModel;
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            var user = ((FrameworkElement)sender).DataContext as Data.User;
            UserModel.Username = user.Username;
            UserModel.Password = user.Password;
            UserModel.RoleId = user.RoleId;
            action = Actions.Editing;
            cmbRoles.SelectedValue = UserModel.RoleId;
            UsernameTextBox.IsReadOnly = true;

            UserDialogHost.IsOpen = true;
        }

        private void NewUser_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = UserModel = new UserVM();
            action = Actions.Inserting;
            UsernameTextBox.IsReadOnly = false;
        }

        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {
            DuplicateUsernameSnackBar.IsActive = false;// = TimeSpan.FromSeconds(3);

        }

        //User User;
        //public string Password { get; set; }
        //public string Username { get; set; }

        private void UserDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            var confirmed = (bool)eventArgs.Parameter;
            if (confirmed)
            {
                if (!string.IsNullOrEmpty(UserModel.Username) && !string.IsNullOrEmpty(UserModel.Password)
                     && cmbRoles.SelectedIndex > -1)
                {
                    switch (action)
                    {
                        case Actions.Inserting:
                            if (!db.Users.Any(u => u.Username == UserModel.Username))
                            {
                                db.Users.InsertOnSubmit(
                                    new Data.User
                                    {
                                        Username = UserModel.Username,
                                        Password = UserModel.Password,
                                        RoleId = UserModel.RoleId
                                    });
                                db.SubmitChanges();

                                RefreshGrid();
                            }
                            else
                            {
                                DuplicateUsernameSnackBar.IsActive = true;


                                System.Threading.Thread t = new System.Threading.Thread(() =>
                                {
                                    var started = DateTime.Now;
                                    while (DateTime.Now.Subtract(started).TotalSeconds < 3)
                                    { }
                                    this.Dispatcher.Invoke(
                                    new Action(() => { DuplicateUsernameSnackBar.IsActive = false; }));
                                }); t.Start();
                            }
                            break;
                        case Actions.Editing:
                            var user = db.Users.Where(u => u.Username == UserModel.Username).FirstOrDefault();
                            user.Password = UserModel.Password;
                            user.RoleId = UserModel.RoleId;
                            db.SubmitChanges();
                            RefreshGrid();
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                Dynamics.LastEscapeTime = DateTime.Now;
            }
        }

        private void RefreshGrid()
        {
            var users = db.Users.ToList();
            UsersList = new ObservableCollection<Data.User>(users);

            UsersGrid.DataContext = UsersList;

            RolesList = new ObservableCollection<Data.Role>(db.Roles.ToList());
            cmbRoles.DataContext = RolesList;
        }

        private void UserSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] != null)
                UserModel.RoleId = (e.AddedItems[0] as Data.Role).Id;
        }
    }
}
