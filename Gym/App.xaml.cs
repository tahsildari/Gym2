using Gym.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Gym.Domain.Utilities;

namespace Gym
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        KeyboardListener KListener = new KeyboardListener();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
        }

        void KListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            if (!(Current.MainWindow is Windows.Main)) return;
            if (Gym.Windows.Main.CurrentUser == null) return;
            var mainWindows = Current.MainWindow as Windows.Main;
                
            if (args.Key == Key.F1)
            {
                mainWindows.ShortCutButton.IsPopupOpen = !mainWindows.ShortCutButton.IsPopupOpen;

                //System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                //dispatcherTimer.Tick += (s, e) =>
                //{
                //    mainWindows.ShortCutButton.IsPopupOpen = false;
                //};
                //dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
                //dispatcherTimer.Start();


            }
            if (args.Key == Key.F2)
            {
                mainWindows.ClosetsExpander.IsExpanded = !mainWindows.ClosetsExpander.IsExpanded;
            }
            if (args.Key == Key.F3)
            {
                mainWindows.PassagesExpander.IsExpanded = !mainWindows.PassagesExpander.IsExpanded;
            }
            if (args.Key == Key.F4)
            {
                (new Enroll { Owner = mainWindows }).Show();
            }
            if (args.Key == Key.F6)
            {
                var main = (Application.Current.MainWindow as Windows.Main);
                main.MenuShortcut.IsChecked = !main.MenuShortcut.IsChecked;
            }
            if (mainWindows.ShortCutButton.IsPopupOpen)
            {
                if (args.Key == Key.D1)
                {
                    mainWindows.btnRegularTransit_Click(null, null);
                    mainWindows.ShortCutButton.IsPopupOpen = false;
                }
                if (args.Key == Key.D2)
                {
                    mainWindows.btnIrregularTransit_Click(null, null);
                    mainWindows.ShortCutButton.IsPopupOpen = false;
                }
                if (args.Key == Key.D3)
                {
                    mainWindows.btnIrregularExit_Click(null, null);
                    mainWindows.ShortCutButton.IsPopupOpen = false;
                }
                if (args.Key == Key.D4)
                {
                    mainWindows.btnPersonnelTransit_Click(null, null);
                    mainWindows.ShortCutButton.IsPopupOpen = false;
                }
            }
            // I tried writing the data in file here also, to make sure the problem is not in Console.WriteLine
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            KListener.Dispose();
        }
    }
}
