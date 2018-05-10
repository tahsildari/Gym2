using Gym.Controls;
using Gym.Domain;
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
    /// Interaction logic for IrregularExit.xaml
    /// </summary>
    public partial class IrregularExit : Window
    {
        public IrregularExit()
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
            LoadIrregularEnters();
        }


        public int LoadIrregularEnters()
        {
            //lastId = 0;
            //List.Children.Clear();
            List.Children.Clear();

            Data.GymContextDataContext db = new Data.GymContextDataContext();
            //var twentyfour = DateTime.Now.AddHours(-24);
            var passages =
                (from enter in db.Passages
                 where
                  //enter.Time > twentyfour &&
                  enter.IsEntrance
                 && !enter.Member.IsRegular
                 && !enter.Member.IsStaff 
                 && !enter.Member.IsMentor
                 && !db.Passages.Any(exit => exit.Time > enter.Time && exit.MemberId == enter.MemberId && exit.IsEntrance == false)
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
                time.MouseDoubleClick += Time_MouseDoubleClick;

                panel.Children.Add(icon);
                panel.Children.Add(time);
                List.Children.Add(panel);

                if (List.Children.Count > 100)
                    List.Children.RemoveRange(0, List.Children.Count - 100);
            });
            return passages.Count;

        }
        Data.GymContextDataContext db = new Data.GymContextDataContext();

        private void Time_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var id = ((int)((Label)sender).Tag);
            Data.Passage exit = new Data.Passage { IsEntrance = false, MemberId = id, Time = DateTime.Now };

            db.Passages.InsertOnSubmit(exit);
            db.SubmitChanges();

            var member = db.Members.Where(m => m.Id == id).FirstOrDefault();
            var Member = new MemberCard(member.Id);
            Member.Member.FreeUpCloset(db);
            //member.FreeUpCloset(db);
            
            Main.Home.TransitList.UpdatePassages();
            Main.Home.Closets.LoadClosets();
            LoadIrregularEnters();
        }
    }
}
