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
using Gym.Data;
using Gym.Domain;

namespace Gym.Controls
{
    /// <summary>
    /// Interaction logic for MemberInfo.xaml
    /// </summary>
    public partial class MemberInfo : UserControl
    {
        public MemberInfo()
        {
            InitializeComponent();
        }

        public void SetMember(int Id, string transitStat, List<string> facilities)
        {
            var db = new Data.GymContextDataContext();
            var member = db.Members.Where(m => m.Id == Id).FirstOrDefault();
            var enrolls = member.Enrolls.Where(e => e.ExpireDate >= DateTime.Today).ToList();
            string expire = "";
            if (enrolls == null)
                expire = "-- دوره فعال ندارد --";
            else
            {
                if (!enrolls.Any(e => e.EnrollCourses.Any(c => c.SessionsLeft > 0)))
                {
                    expire = "-- از تمامی جلسات استفاده کرده --";
                }
                else
                {
                    var activeEnrolls = enrolls.Where(e => e.EnrollCourses.Any(c => c.SessionsLeft > 0))
                        .OrderBy(e => e.ExpireDate).ToList();
                    activeEnrolls.ForEach(a =>
                    {
                        if (a.ExpireDate.HasValue)
                            expire += a.ExpireDate.Value.ToFa() + " ["+ a.EnrollCourses.FirstOrDefault().SessionsLeft +" جلسه مانده]" + " - ";
                        else
                            expire += "";
                    });
                    if (expire.Length > 0)
                        expire = expire.TrimEnd(' ', '-');
                }
            }
            txtName.Text = $"{member.Id} - {member.Fullname()}";
            txtExpire.Text = expire;
            txtCaffeDebtor.Value = member.Credit < 0 ? (-1 * member.Credit) : 0;
            txtTuitionDebtor.Value = member.Debtor;
            if (member.Closets.Any())
                txtCloset.Value = member.Closets.FirstOrDefault().Id;
            txtTransit.Text = transitStat;
            if ((facilities?.Count ?? 0) == 0)
                txtFacilities.Text = "ندارد";
            else
            {
                var facs = "";
                facilities.ForEach(f => facs += f + " - ");
                if (facs.Length > 0)
                    facs = facs.TrimEnd(' ', '-');

                txtFacilities.Text = facs;
            }

            //{
            var _Image = new BitmapImage();
            _Image.BeginInit();
            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + $"/Images/{member.Id}.jpg"))
                _Image.StreamSource = System.IO.File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + $"/Images/{member.Id}.jpg");
            else
                _Image.StreamSource = System.IO.File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + $"/Images/defaultUser.png");
            _Image.EndInit();
            Image.ImageSource = _Image;


            if (transitStat == "ورود" || transitStat == "خروج")
            {
                txtTransit.Foreground = Brushes.DodgerBlue;
            }
            else
                txtTransit.Foreground = Brushes.Red;

            //}
            //{
            //    Image.ImageSource = Resources..;
            //}
        }
    }
}
