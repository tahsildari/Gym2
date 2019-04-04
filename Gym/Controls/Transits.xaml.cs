using Gym.Domain;
using Gym.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for Transits.xaml
    /// </summary>
    public partial class Transits : UserControl
    {
        public Transits()
        {
            InitializeComponent();
        }

        DateTime lastcheck = DateTime.Now;
        int lastId = 0;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePassages();
        }

        public void UpdatePassages()
        {
            //lastId = 0;
            //List.Children.Clear();

            Data.GymContextDataContext db = new Data.GymContextDataContext();
            var h = Domain.Dynamics.TransitFarthestHour;
            //var today = DateTime.Today;
            var passages =
                (from p in db.Passages
                 where !p.Member.IsStaff
                    && !p.Member.IsMentor
                    && p.Id > lastId
                    && p.Time >= DateTime.Now.AddHours(-1 * h)
                 select new
                 {
                     p.Id,
                     p.IsEntrance,
                     p.Time,
                     Member = p.Member.Firstname + " " + p.Member.Lastname,
                     MemberId = p.MemberId,
                     IsRegular = p.Member.IsRegular,
                     Closet = db.ClosetUsages.Where(u => u.MemberId == p.MemberId && u.FromTime >= p.Time && u.FromTime <= p.Time.AddSeconds(30)).Select(c => c).FirstOrDefault()
                 }).ToList();
            lastId = passages.Any() ? passages.Select(p => p.Id).Max() : lastId;

            if (passages.Count == 0)
                return;

            var style = new Style();
            var trigger = new Trigger { Property = IsMouseOverProperty, Value = true };
            trigger.Setters.Add(new Setter { Property = OpacityProperty, Value = 1.0 });
            var triggeroff = new Trigger { Property = IsMouseOverProperty, Value = false };
            triggeroff.Setters.Add(new Setter { Property = OpacityProperty, Value = 0.5 });
            style.Triggers.Add(trigger);
            style.Triggers.Add(triggeroff);

            passages.ToList().ForEach(pass =>
            {



                StackPanel panel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    FlowDirection = FlowDirection.RightToLeft,
                    Style = style,

                };
                panel.ContextMenu = new ContextMenu();

                var enrollitem = new MenuItem { Header = "فرم ثبت نام" };
                enrollitem.Click += (s, ce) =>
                {
                    Enroll member = new Enroll();
                    member.Owner = Main.Home;
                    member.Show();
                    member.LoadMember(pass.MemberId);
                };
                enrollitem.IsEnabled = pass.IsRegular;
                panel.ContextMenu.Items.Add(enrollitem);

                string tip = "";
                if (pass.IsEntrance)
                {
                    var firstUsageAfter = db.ClosetUsages.Where(u => u.MemberId == pass.MemberId && u.FromTime >= pass.Time).OrderBy(u => u.FromTime).FirstOrDefault();
                    if (firstUsageAfter != null)
                    {
                        tip += "کمد: " + firstUsageAfter.ClosetId.ToString();
                    }
                }
                else
                {
                    var closetAtTheTime = db.ClosetUsages.Where(u => u.MemberId == pass.MemberId && u.FromTime <= pass.Time).OrderByDescending(u => u.FromTime).FirstOrDefault();
                    if (closetAtTheTime != null)
                    {
                        tip += "کمد: " + closetAtTheTime.ClosetId.ToString();
                    }
                }

                tip += Environment.NewLine + "تاریخ: " + pass.Time.Date.ToFa()
                  + Environment.NewLine + "زمان: " + pass.Time.ToLongTimeString()
                 + Environment.NewLine + "عضو: " + pass.Member
                 + Environment.NewLine + "وضعیت تردد : " + (pass.IsEntrance ? "ورود" : "خروج");

                panel.ToolTip = tip;

                var infoitem = new MenuItem { Header = "مشاهده اطلاعات عضو" };
                List<string> facilities = new List<string>();
                infoitem.Click += (s, ce) =>
                {
                    bool facilitiesCached = false;
                    if (!facilitiesCached)
                    {
                        var mmbr = db.Members.Where(m => m.Id == pass.MemberId).FirstOrDefault();
                        var nrols = mmbr.Enrolls.Where(e => e.StartDate <= DateTime.Today
                                            && (e.ExpireDate == null || e.ExpireDate >= DateTime.Today)).ToList();
                        var nf = nrols.SelectMany(n => n.EnrollFacilities).ToList();
                        facilities = nf.Select(f => f.Facility.Name).ToList();
                    }

                    MemberInformation member = new MemberInformation();
                    member.Owner = Main.Home;
                    member.SetMember(pass.MemberId, pass.IsEntrance ? "ورود" : "خروج", facilities);
                    member.Show();
                };
                infoitem.IsEnabled = pass.IsRegular;
                panel.ContextMenu.Items.Add(infoitem);

                var financial = new MenuItem { Header = "لیست تراکنش های عضو" };
                financial.Click += (s, ce) =>
                {
                    Enroll member = new Enroll();
                    member.Owner = Main.Home;
                    member.Show();
                    member.LoadMember(pass.MemberId);
                    member.Section = 3;
                };
                panel.ContextMenu.Items.Add(financial);

                MaterialDesignThemes.Wpf.PackIcon icon = new MaterialDesignThemes.Wpf.PackIcon
                {
                    Kind = pass.IsEntrance ? MaterialDesignThemes.Wpf.PackIconKind.TrendingUp : MaterialDesignThemes.Wpf.PackIconKind.TrendingDown,
                    Foreground = new SolidColorBrush(pass.IsEntrance ? Colors.DodgerBlue : Colors.DarkRed),
                    Margin = new Thickness(0, 6, 0, 0)
                };
                Label time = new Label
                {
                    Foreground = new SolidColorBrush(pass.IsRegular ? Colors.DodgerBlue : Colors.Gray),
                    Content = $"{pass.Time.ToString("HH:mm")} {pass.Member}",
                    Margin = new Thickness(2, 1, 2, 1),
                    //FontWeight = pass.IsRegular ? FontWeights.Bold : FontWeights.Normal
                };

                var mem = db.Members.First(m => m.Id == pass.MemberId);

                var validUntill = DateTime.Now.AddDays(4);
                var needsExtension = db.Enrolls.Where(n =>
                n.MemberId == mem.Id
                && (n.ExpireDate <= validUntill && n.ExpireDate >= DateTime.Now)).Any();

                panel.Children.Add(icon);
                panel.Children.Add(time);
                if (needsExtension)
                {
                    MaterialDesignThemes.Wpf.PackIcon ext = new MaterialDesignThemes.Wpf.PackIcon
                    {
                        Kind = MaterialDesignThemes.Wpf.PackIconKind.ClockEnd,
                        Foreground = new SolidColorBrush(pass.IsEntrance ? Colors.DodgerBlue : Colors.DarkRed),
                        Margin = new Thickness(0, 6, 0, 0),
                        ToolTip = "نیازمند تمدید"
                    };
                    panel.Children.Add(ext);
                }
                if (mem.Debtor > 0) {
                    panel.Children.Add(new MaterialDesignThemes.Wpf.PackIcon
                    {
                        Kind = MaterialDesignThemes.Wpf.PackIconKind.CurrencyUsd,
                        Foreground = new SolidColorBrush(pass.IsEntrance ? Colors.DodgerBlue : Colors.DarkRed),
                        Margin = new Thickness(0, 6, 0, 0),
                        ToolTip = "بدهکار شهریه"
                    });
                }
                if (mem.Debtor > 0)
                {
                    panel.Children.Add(new MaterialDesignThemes.Wpf.PackIcon
                    {
                        Kind = MaterialDesignThemes.Wpf.PackIconKind.Carrot,
                        Foreground = new SolidColorBrush(pass.IsEntrance ? Colors.DodgerBlue : Colors.DarkRed),
                        Margin = new Thickness(0, 6, 0, 0),
                        ToolTip = "بدهکار بوفه"
                    });
                }
                List.Children.Add(panel);

                if (List.Children.Count > 100)
                    List.Children.RemoveRange(0, List.Children.Count - 100);
            });





            //while (true)
            //{
            //    //return;
            //    if (DateTime.Now.Subtract(lastcheck).TotalMilliseconds < 1000)
            //        continue;

            //    lastcheck = DateTime.Now;
            //    List.Children.Add(new Label
            //    {
            //        Foreground = new SolidColorBrush(Colors.Gray),
            //        Content = DateTime.Now.ToString("HH:mm:ss")
            //    });
            //    if (List.Children.Count > 100)
            //        List.Children.RemoveAt(0);
            //}
        }
    }
}
