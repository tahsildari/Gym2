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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            this.PreviewKeyUp += Window_PreviewKeyUp;
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }
        private void ChangeBackground_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as Main).ChangeBackground_Click(null, null);
        }
        private void ResetBackground_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as Main).ResetBackground();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var db = new Data.GymContextDataContext();

            Data.Setting insurance = db.Settings.Where(s => s.Key == "insurance_price").FirstOrDefault();
            insurance.Value = (txtInsurancePrice.Value > 0 ? txtInsurancePrice.Value.ToString() : insurance.Value);

            Data.Setting singlesession = db.Settings.Where(s => s.Key == "single_price").FirstOrDefault();
            singlesession.Value = (txtSingleSessionPrice.Value > 0 ? txtSingleSessionPrice.Value.ToString() : singlesession.Value);

            Data.Setting extend = db.Settings.Where(s => s.Key == "extend_days").FirstOrDefault();
            extend.Value = (txtExtendDays.Value > 0 ? txtExtendDays.Value.ToString() : extend.Value);

            Data.Setting transithours = db.Settings.Where(s => s.Key == "transit_hours").FirstOrDefault();
            transithours.Value = (txtTransitHours.Value > 0 ? txtTransitHours.Value.ToString() : transithours.Value);

            Data.Setting title = db.Settings.Where(s => s.Key == "app_title").FirstOrDefault();
            title.Value = txtTitle.Text;

            Data.Setting closetwidth = db.Settings.Where(s => s.Key == "closets_width").FirstOrDefault();
            closetwidth.Value = (txtClosetsWidth.Value > 0 ? (txtClosetsWidth.Value).ToString() : "70");

            Data.Setting infotimeout = db.Settings.Where(s => s.Key == "info_timeout").FirstOrDefault();
            infotimeout.Value = (txtInfoTimeout.Value > 0 ? (txtInfoTimeout.Value).ToString() : "30");

            Data.Setting guestfinger = db.Settings.Where(s => s.Key == "guest_finger_id").FirstOrDefault();
            guestfinger.Value = (txtGuestFinger.Value > 0 ? (txtGuestFinger.Value).ToString() : "0");

            db.SubmitChanges();

            Domain.Dynamics.Refresh();
            Main.Home.TransitList.UpdatePassages();
            Main.Home.CheckupExtenders();
            Main.Home.LoadTitle();
            Main.Home.ResizeClosets();
            Main.Home.ShowMessage("تنظیمات با موفقیت ذخیره شد",10);
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var db = new Data.GymContextDataContext();

            Data.Setting insurance = db.Settings.Where(s => s.Key == "insurance_price").FirstOrDefault();
            txtInsurancePrice.Value = int.Parse(insurance.Value);

            Data.Setting singlesession = db.Settings.Where(s => s.Key == "single_price").FirstOrDefault();
            txtSingleSessionPrice.Value = int.Parse(singlesession.Value);

            Data.Setting extend = db.Settings.Where(s => s.Key == "extend_days").FirstOrDefault();
            txtExtendDays.Value = int.Parse(extend.Value);

            Data.Setting transithours = db.Settings.Where(s => s.Key == "transit_hours").FirstOrDefault();
            txtTransitHours.Value = int.Parse(transithours.Value);

            Data.Setting title = db.Settings.Where(s => s.Key == "app_title").FirstOrDefault();
            txtTitle.Text = title.Value.ToString();

            Data.Setting closetwidth = db.Settings.Where(s => s.Key == "closets_width").FirstOrDefault();
            txtClosetsWidth.Value = int.Parse(closetwidth.Value);

            Data.Setting infotimeout = db.Settings.Where(s => s.Key == "info_timeout").FirstOrDefault();
            txtInfoTimeout.Value = int.Parse(infotimeout.Value);

            Data.Setting guestfinger = db.Settings.Where(s => s.Key == "guest_finger_id").FirstOrDefault();
            txtGuestFinger.Value = int.Parse(guestfinger.Value);


        }
    }
}
