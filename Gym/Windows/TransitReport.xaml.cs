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
using static Gym.Domain.Enums;

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for TransitReport.xaml
    /// </summary>
    public partial class TransitReport : Window
    {
        public TransitReport()
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
            LoadTransits();
        }

        Data.GymContextDataContext db = new Data.GymContextDataContext();
        MemberSelectionCategory _Type = MemberSelectionCategory.SelectionOnly;
        public MemberSelectionCategory Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
                switch (value)
                {
                    case MemberSelectionCategory.SelectionOnly:
                    case MemberSelectionCategory.DeleteMembers:
                        break;
                    case MemberSelectionCategory.MembersTransit:
                        this.Title = "گزارش تردد اعضا";
                        break;
                    case MemberSelectionCategory.IrregularTransit:
                        this.Title = "گزارش ترددهای تک جلسه ای";
                        break;
                    case MemberSelectionCategory.PersonnelTransit:
                        //zone.Background = new SolidColorBrush(Color.FromRgb(255, 193, 7));
                        //zone.Mode = MaterialDesignThemes.Wpf.ColorZoneMode.PrimaryLight;
                        this.Title = "گزارش تردد پرسنل و مربیان";
                        break;
                    default:
                        break;
                }
            }
        }


        public int LoadTransits()
        {
            List.Children.Clear();

            bool IsStaff = Type == MemberSelectionCategory.PersonnelTransit;
            bool IsMentor = Type == MemberSelectionCategory.PersonnelTransit;
            var h = Domain.Dynamics.TransitFarthestHour;

            Data.GymContextDataContext db = new Data.GymContextDataContext();
            var passages =
                (from enter in db.Passages
                 where (enter.Member.IsStaff == IsStaff
                 || enter.Member.IsMentor == IsMentor)
                 && enter.Member.IsRegular == (Type != MemberSelectionCategory.IrregularTransit)
                 && enter.Time >= DateTime.Now.AddHours(-1 * h)
                 select new
                 {
                     enter.MemberId,
                     enter.IsEntrance,
                     enter.Time,
                     Member = enter.Member.Firstname + " " + enter.Member.Lastname,
                     IsRegular = enter.Member.IsRegular
                 }).ToList();



            if (passages.Count == 0)
                return passages.Count;

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
                MaterialDesignThemes.Wpf.PackIcon icon = new MaterialDesignThemes.Wpf.PackIcon
                {
                    Kind = pass.IsEntrance ? MaterialDesignThemes.Wpf.PackIconKind.TrendingUp : MaterialDesignThemes.Wpf.PackIconKind.TrendingDown,
                    Foreground = new SolidColorBrush(pass.IsEntrance ? Colors.DodgerBlue : Colors.Maroon),
                    Margin = new Thickness(0, 6, 0, 0)
                };
                Label time = new Label
                {
                    Foreground = new SolidColorBrush(Colors.White),
                    Content = $"{pass.Time.ToString("HH:mm")} {pass.Member}",
                    Margin = new Thickness(2, 1, 2, 1),
                    Tag = pass.MemberId,
                    Cursor = Cursors.Hand,
                    //FontWeight = pass.IsRegular ? FontWeights.Bold : FontWeights.Normal
                };

                panel.Children.Add(icon);
                panel.Children.Add(time);
                List.Children.Add(panel);

                if (List.Children.Count > 100)
                    List.Children.RemoveRange(0, List.Children.Count - 100);
            });
            return passages.Count;

        }
    }
}
