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
    /// Interaction logic for Mentors.xaml
    /// </summary>
    public partial class Mentors : Window
    {
        public Mentors()
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

            RefreshGrid();

            SportsList = new ObservableCollection<SportVM>(db.Sports.Select(s =>
                new SportVM
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsSelected = s.SportMentors.Any(m => m.MentorId == MentorModel.Id)
                }).ToList());
            SportsItemsControl.ItemsSource = SportsList;

            this.DataContext = MentorModel;
        }

        Data.GymContextDataContext db = new Data.GymContextDataContext();
        MemberVM MentorModel = new MemberVM();
        ObservableCollection<SportVM> SportsList;
        ObservableCollection<Data.Member> MentorsList;
        private Actions action;


        private void RefreshGrid()
        {
            var mentors = db.Members.Where(m => m.IsMentor || m.IsStaff).ToList();
            MentorsList = new ObservableCollection<Data.Member>(mentors);

            MentorsGrid.DataContext = MentorsList;
        }

        private void UpdateSportPricesForMentor()
        {
            SportsList = new ObservableCollection<SportVM>(db.Sports.Select(s =>
                new SportVM
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsSelected = s.SportMentors.Any(m => m.MentorId == MentorModel.Id)
                }).ToList());
            SportsItemsControl.ItemsSource = SportsList;

            switch (action)
            {
                case Actions.Inserting:
                    SportsList.ToList().ForEach(s => { s.Price = 0; s.IsSelected = false; });
                    break;
                case Actions.Editing:
                    SportsList.ToList().ForEach(s =>
                    {
                        s.IsSelected = db.SportMentors.Any(sm => sm.SportId == s.Id && sm.MentorId == MentorModel.Id);
                        s.Price = db.SportMentors.Where(sm => sm.SportId == s.Id && sm.MentorId == MentorModel.Id).FirstOrDefault()?.Price ?? 0;
                    });
                    break;
                case Actions.Selected:
                case Actions.None:
                default:
                    break;
            }
        }



        private void MentorDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            var confirmed = (bool)eventArgs.Parameter;
            if (confirmed)
                if (!string.IsNullOrEmpty(MentorModel.Firstname) && !string.IsNullOrEmpty(MentorModel.Lastname)
                     && !string.IsNullOrEmpty(MentorModel.Mobile))
                {
                    var sports = SportsList.Where(s => s.IsSelected).ToList();
                    switch (action)
                    {
                        case Actions.Inserting:
                            {
                                if (!db.Members.Any(m => m.IsMentor && m.Mobile == MentorModel.Mobile))
                                {
                                    Data.Member mentor;
                                    db.Members.InsertOnSubmit(
                                        mentor = new Data.Member
                                        {
                                            Firstname = MentorModel.Firstname,
                                            Lastname = MentorModel.Lastname,
                                            Mobile = MentorModel.Mobile,
                                            NationalCode = MentorModel.NationalCode,
                                            Address = MentorModel.Address,
                                            IsMentor = rdbMentor.IsChecked == true,
                                            IsStaff = rdbPersonnel.IsChecked == true,
                                            UserId = Main.CurrentUser.Id
                                        });
                                    db.SubmitChanges();
                                    sports.ForEach(s => db.SportMentors.InsertOnSubmit(new Data.SportMentor
                                    {
                                        MentorId = mentor.Id,
                                        SportId = s.Id,
                                        Price = s.Price
                                    }));

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
                            }
                        case Actions.Editing:
                            {
                                var mentor = db.Members.Where(m => m.Id == MentorModel.Id).FirstOrDefault();
                                mentor.Firstname = MentorModel.Firstname;
                                mentor.Lastname = MentorModel.Lastname;
                                mentor.Mobile = MentorModel.Mobile;
                                mentor.NationalCode = MentorModel.NationalCode;
                                mentor.Address = MentorModel.Address;

                                var currentMentorSports = db.SportMentors.Where(sm => sm.MentorId == MentorModel.Id).ToList();

                                var removeList = currentMentorSports.Where(sm => !sports.Select(s => s.Id).Contains(sm.SportId));
                                var addList = sports.Where(s => !currentMentorSports.Select(ms => ms.SportId).Contains(s.Id)).ToList();

                                addList.ForEach(s => db.SportMentors.InsertOnSubmit(new Data.SportMentor
                                {
                                    MentorId = MentorModel.Id,
                                    SportId = s.Id,
                                    Price = s.Price
                                }));

                                db.SportMentors.DeleteAllOnSubmit(removeList);

                                db.SubmitChanges();

                                RefreshGrid();
                                break;
                            }
                        default:
                            break;
                    }
                }
        }

        private void NewMentor_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = MentorModel = new MemberVM();
            action = Actions.Inserting;
            UpdateSportPricesForMentor();
        }

        private void EditMentor_Click(object sender, RoutedEventArgs e)
        {
            var mentor = ((FrameworkElement)sender).DataContext as Data.Member;
            MentorModel.Id = mentor.Id;
            MentorModel.Firstname = mentor.Firstname;
            MentorModel.Lastname = mentor.Lastname;
            MentorModel.Mobile = mentor.Mobile;
            MentorModel.Address = mentor.Address;
            MentorModel.NationalCode = mentor.NationalCode;
            rdbPersonnel.IsChecked = mentor.IsStaff;
            rdbMentor.IsChecked = mentor.IsMentor;
            action = Actions.Editing;

            MentorDialogHost.IsOpen = true;

            UpdateSportPricesForMentor();
        }

        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {
            DuplicateUsernameSnackBar.IsActive = false;
        }

        private void rdbMentor_Checked(object sender, RoutedEventArgs e)
        {
            SportsItemsControl.Visibility = Visibility.Visible;
        }

        private void rdbPersonnel_Checked(object sender, RoutedEventArgs e)
        {
            SportsItemsControl.Visibility = Visibility.Collapsed;

        }
    }
}
