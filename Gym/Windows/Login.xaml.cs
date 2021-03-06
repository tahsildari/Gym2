﻿using System;
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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            txtUsername.Focus();
        }

        private void Login_OnClick(object sender, RoutedEventArgs e)
        {
            //if (DateTime.Now >= new DateTime(2019, 3, 3)) {
            //    throw new Exception("Microsoft.akin.software error. Please update and try");
            //    Exit_Click(null, null);
            //};
            var db = new Data.GymContextDataContext();
            var isvalid = db.Users.Where(u => u.Username == txtUsername.Text && u.Password == txtPassword.Password).Any();

            if (isvalid)
            {
                //Windows.Main home = new Main();
                //home.Show();
                Main.Home.SetUser(db.Users.Where(u => u.Username == txtUsername.Text && u.Password == txtPassword.Password).FirstOrDefault().Id);
                Main.Home.Show();
                this.Close();

            }
            else {
            MessageSnackBar.IsActive = true;
            }
        }
        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {
            MessageSnackBar.IsActive = false;
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Login_OnClick(null, null);
            else if (e.Key == Key.Escape)
                Exit_Click(null, null);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            (Application.Current.MainWindow as Main).Exit();

        }
    }
}
