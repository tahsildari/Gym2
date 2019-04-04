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
    /// Interaction logic for MemberInformation.xaml
    /// </summary>
    public partial class MemberInformation : Window
    {
        public MemberInformation()
        {
            InitializeComponent();
            this.IsVisibleChanged += MemberInformation_IsVisibleChanged;
        }
        int timeout, timer;

        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        private void MemberInformation_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                timeout = Domain.Dynamics.InfoTimeout;
                timer = timeout;

                dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                dispatcherTimer.Start();

                updateTitle();

            }
        }
        void updateTitle()
        {
            this.Title = "اطلاعات عضو - (" + timer.ToString() + " ثانیه)";
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            timer--;
            updateTitle();
            if (timer <= 0)
            // code goes here
            {
                Close();
                (Application.Current.MainWindow).Focus();
                dispatcherTimer.Stop();
            }

        }

        public void SetMember(int id, string transitStat, List<string> facilities)
        {
            Info.SetMember(id, transitStat, facilities);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
